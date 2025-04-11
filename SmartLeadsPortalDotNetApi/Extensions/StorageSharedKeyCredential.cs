using System;
using System.Reflection.Metadata;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace SmartLeadsPortalDotNetApi.Extensions;

public static class BlobContainerClientExtensions
{
    public static string GetSasToken(this BlobContainerClient blobContainerClient, StorageSharedKeyCredential sharedKeyCredential)
    {
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = "upload-container",
            Resource = "c", // "c" for container-level SAS
            ExpiresOn = DateTimeOffset.UtcNow.AddHours(8) // 8 hours expiration time
        };

        // Set permissions (e.g., read-only access)
        sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);
        return sasBuilder.ToSasQueryParameters(sharedKeyCredential).ToString();
    }
}
