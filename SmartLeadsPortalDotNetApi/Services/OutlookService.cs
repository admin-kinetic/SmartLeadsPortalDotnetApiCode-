using System;
using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using SmartLeadsPortalDotNetApi.Extensions;
using SmartLeadsPortalDotNetApi.Factories;
using SmartLeadsPortalDotNetApi.Repositories;
using SmartLeadsPortalDotNetApi.Configs;
using System.Net;
using FluentFTP;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace SmartLeadsPortalDotNetApi.Services;

public class OutlookService
{
    private readonly GraphClientWrapper graphClientWrapper;
    private readonly OutboundCallRepository outboundCallRepository;
    private readonly BlobContainerClient blobContainerClient;
    private readonly CallRecordingFtpCredentials ftpCredentials;
    private readonly IConfiguration configuration;
    private readonly ILogger<OutlookService> logger;
    private readonly AsyncRetryPolicy<MessageCollectionResponse> retryPolicy;
    private const string VOIP_EMAIL = "do_not_reply@au.voipcloud.online";
    private const string RECORDING_EMAIL = "smartleadscallrecording@kineticstaff.com";

    public OutlookService(
        GraphClientWrapper graphServiceClient,
        OutboundCallRepository outboundCallRepository,
        BlobContainerClient blobContainerClient,
        IOptions<CallRecordingFtpCredentials> ftpCredentials,
        IConfiguration configuration,
        ILogger<OutlookService> logger)
    {
        this.graphClientWrapper = graphServiceClient;
        this.outboundCallRepository = outboundCallRepository;
        this.blobContainerClient = blobContainerClient;
        this.ftpCredentials = ftpCredentials.Value;
        this.configuration = configuration;
        this.logger = logger;

        // Configure retry policy
        this.retryPolicy = Policy<MessageCollectionResponse>
            .Handle<ApplicationException>()
            .WaitAndRetryAsync(
                5, // retry 5 times
                retryAttempt => TimeSpan.FromSeconds(60), // wait 10 seconds between retries
                onRetry: (outcome, timeSpan, retryCount, context) =>
                {
                    this.logger.LogWarning(
                        "Attempt {RetryCount} to fetch email failed. Waiting {TimeSpan} seconds before next retry. Error: {Error}",
                        retryCount,
                        timeSpan.TotalSeconds,
                        outcome.Exception?.Message);
                }
            );
    }

    private async Task<MessageCollectionResponse> GetEmailsWithRetryAsync(GraphServiceClient graphServiceClient, string uniqueCallId)
    {
        return await this.retryPolicy.ExecuteAsync(async () =>
        {
            var emails = await graphServiceClient.Users[RECORDING_EMAIL].MailFolders["Inbox"].Messages
                .GetAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Filter = $"from/emailAddress/address eq '{VOIP_EMAIL}' and contains(body/content, 'Unique call id {uniqueCallId}')" +
                        $"or from/emailAddress/address eq '{RECORDING_EMAIL}' and contains(body/content, 'Unique call id {uniqueCallId}')";
                    requestConfiguration.QueryParameters.Expand = new[] { "attachments" };
                });

            if (emails?.Value == null || !emails.Value.Any())
            {
                throw new ApplicationException($"No emails found for unique call id {uniqueCallId}.");
            }

            return emails;
        });
    }

    public async Task MoveCallRecordingToAzureStorageAndGoDaddyFtp(string uniqueCallId)
    {
        try
        {
            var graphServiceClient = await this.graphClientWrapper.GetClientAsync();
            if (graphServiceClient == null)
            {
                throw new ApplicationException("Graph service client is not initialized.");
            }

            var emails = await GetEmailsWithRetryAsync(graphServiceClient, uniqueCallId);

            if (emails?.Value == null || !emails.Value.Any())
            {
                throw new ApplicationException($"No emails found for unique call id {uniqueCallId}.");
            }

            foreach (var email in emails.Value)
            {
                if (email.Attachments == null) continue;
                
                foreach (var attachment in email.Attachments)
                {
                    var fileAttachment = attachment as FileAttachment;
                    if (fileAttachment == null) continue;
                    
                    var attachmentContent = fileAttachment.ContentBytes;
                    if (attachmentContent == null) continue;

                    var fileName = $"{uniqueCallId}.mp3";

                    try
                    {
                        // upload to Azure Blob Storage
                        await this.blobContainerClient.CreateIfNotExistsAsync();
                        var blobClient = this.blobContainerClient.GetBlobClient(fileName);
                        using (var stream = new MemoryStream(attachmentContent))
                        {                            
                            await blobClient.UploadAsync(stream, new Azure.Storage.Blobs.Models.BlobUploadOptions { HttpHeaders = new Azure.Storage.Blobs.Models.BlobHttpHeaders { ContentType = "audio/mpeg" } }, CancellationToken.None);
                        }

                        var uri = $"/{this.configuration["AzureStorage:Container"]}/{fileName}";

                        await this.outboundCallRepository.UpdateAzureStorageRecordingLik(uniqueCallId, uri);

                        // upload to FTP server
                        await using (var ftpClient = new AsyncFtpClient(this.ftpCredentials.Host, this.ftpCredentials.Username, this.ftpCredentials.Password, this.ftpCredentials.Port))
                        {
                            await ftpClient.Connect();

                            await using (var stream = new MemoryStream(attachmentContent))
                            {
                                stream.Position = 0;
                                await ftpClient.UploadStream(stream, fileName, FtpRemoteExists.Overwrite);
                            }
                        }
                        this.logger.LogInformation("Successfully uploaded call recording to FTP server for unique call id {UniqueCallId}", uniqueCallId);

                    }
                    catch (Exception ex)
                    {
                        this.logger.LogError(ex, "Failed to upload call recording to FTP server for unique call id {UniqueCallId}", uniqueCallId);
                        continue;
                    }
                }
            }

        }
        catch (Exception ex)
        {
            // Log error and rethrow or handle appropriately
            throw new ApplicationException($"Error retrieving user: {ex.Message}", ex);
        }
    }

    private async Task ProcessCallRecording(string uniqueCallId)
    {
        var graphServiceClient = await this.graphClientWrapper.GetClientAsync();
        if (graphServiceClient == null) return;

        var emails = await graphServiceClient.Users[RECORDING_EMAIL].MailFolders["Inbox"].Messages
            .GetAsync(requestConfiguration =>
            {
                requestConfiguration.QueryParameters.Filter = $"from/emailAddress/address eq '{VOIP_EMAIL}' and contains(body/content, 'Unique call id {uniqueCallId}')" +
                    $"or from/emailAddress/address eq '{RECORDING_EMAIL}' and contains(body/content, 'Unique call id {uniqueCallId}')";
                requestConfiguration.QueryParameters.Expand = new[] { "attachments" };
            });

        if (emails?.Value == null || !emails.Value.Any()) return;

        foreach (var email in emails.Value)
        {
            if (email.Attachments == null) continue;

            foreach (var attachment in email.Attachments)
            {
                var fileAttachment = attachment as FileAttachment;
                var attachmentContent = fileAttachment?.ContentBytes;
                if (attachmentContent == null) continue;

                var fileName = $"{uniqueCallId}.mp3";

                using (var ftpClient = new AsyncFtpClient(this.ftpCredentials.Host, this.ftpCredentials.Username, this.ftpCredentials.Password, this.ftpCredentials.Port))
                {
                    await ftpClient.Connect();

                    using (var stream = new MemoryStream(attachmentContent))
                    {
                        stream.Position = 0;
                        await ftpClient.UploadStream(stream, fileName, FtpRemoteExists.Overwrite);
                    }
                }

                var uri = $"/recordings/{fileName}";
                await this.outboundCallRepository.UpdateAzureStorageRecordingLik(uniqueCallId, uri);
            }
        }
    }
}
