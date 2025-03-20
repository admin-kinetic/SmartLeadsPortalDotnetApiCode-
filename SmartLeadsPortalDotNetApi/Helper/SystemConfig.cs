namespace SmartLeadsPortalDotNetApi.Helper
{
    public class SystemConfig
    {
        private static IConfiguration configuration;
        public SystemConfig()
        {
            configuration = new ConfigurationBuilder()
           .SetBasePath(System.IO.Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
           .Build();
        }
    }
}
