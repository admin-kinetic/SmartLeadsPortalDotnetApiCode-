using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using SmartLeadsPortalDotNetApi.Model;
using SmartLeadsPortalDotNetApi.Services.Model;

namespace SmartLeadsPortalDotNetApi.Services
{
    public class BlobService
    {
        public async Task<BlobTokenModel> GetStorageSecuredToken(StorageConfig _config)
        {
            BlobTokenModel resultModel = new BlobTokenModel();

            try
            {
                StorageSharedKeyCredential credential = new StorageSharedKeyCredential(_config.AccountName, _config.AccountKey);

                string blobcontainer_url = string.Format("https://{0}.blob.core.windows.net/{1}", _config.AccountName, _config.Container);

                BlobContainerClient containerClient = new BlobContainerClient(new Uri(blobcontainer_url), credential);

                if (containerClient.CanGenerateSasUri)
                {
                    BlobSasBuilder sasBuilder = new BlobSasBuilder()
                    {
                        BlobContainerName = _config.Container,
                        Resource = "c"
                    };

                    sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(10);
                    sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);

                    Uri sasUri = await Task.Run(() => containerClient.GenerateSasUri(sasBuilder));

                    resultModel.Token = sasUri.Query;
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            return resultModel;
        }
    }
}
