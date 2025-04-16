using System;
using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using SmartLeadsPortalDotNetApi.Extensions;
using SmartLeadsPortalDotNetApi.Factories;
using SmartLeadsPortalDotNetApi.Repositories;

namespace SmartLeadsPortalDotNetApi.Services;

public class OutlookService
{
    private readonly GraphClientWrapper graphClientWrapper;
    private readonly BlobContainerClient blobContainerClient;
    private readonly OutboundCallRepository outboundCallRepository;
    private readonly IConfiguration configuration;

    public OutlookService(
        GraphClientWrapper graphServiceClient,
        BlobContainerClient blobContainerClient,
        OutboundCallRepository outboundCallRepository,
        IConfiguration configuration)
    {
        this.graphClientWrapper = graphServiceClient;
        this.blobContainerClient = blobContainerClient;
        this.outboundCallRepository = outboundCallRepository;
        this.configuration = configuration;
    }

    public async Task MoveCallRecordingToAzureStorage(string uniqueCallId)
    {
        try
        {
            var accountEmail = "smartleadscallrecording@kineticstaff.com";
            var graphServiceClient = await this.graphClientWrapper.GetClientAsync();
            var emails = await graphServiceClient.Users[accountEmail].MailFolders["Inbox"].Messages
                .GetAsync(requestConfiguration =>
                    {
                        requestConfiguration.QueryParameters.Filter = $"from/emailAddress/address eq 'do_not_reply@au.voipcloud.online' and contains(body/content, 'Unique call id {uniqueCallId}')";
                        requestConfiguration.QueryParameters.Expand = new[] { "attachments" };
                    });
            if (emails.Value.Count() > 0)
            {
                foreach (var email in emails.Value)
                {
                    foreach (var attachment in email.Attachments)
                    {
                        var fileAttachment = attachment as FileAttachment;
                        var attachmentContent = fileAttachment.ContentBytes;

                        await this.blobContainerClient.CreateIfNotExistsAsync();
                        var fileName = $"{uniqueCallId}.mp3";
                        var blobClient = this.blobContainerClient.GetBlobClient(fileName);
                        using (var stream = new MemoryStream(attachmentContent))
                        {
                            await blobClient.UploadAsync(stream, overwrite: true);
                        }

                        var uri = $"{this.configuration["AzureStorage:Url"]}/{fileName}";

                        await this.outboundCallRepository.UpdateAzureStorageRecordingLik(uniqueCallId, uri);
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
}
