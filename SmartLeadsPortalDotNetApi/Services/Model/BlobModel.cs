namespace SmartLeadsPortalDotNetApi.Services.Model
{
    public class BlobModel
    {
        public string? SecuredContainerUrl { get; set; }
        public string? Token { get; set; }
        public string? ContainerPath { get; set; }
    }
    public class BlobTokenModel
    {
        public string? Token { get; set; }

    }
}
