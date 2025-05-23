namespace SmartLeadsPortalDotNetApi.Configs;

public interface ISmartLeadConfig
{
    string? BaseUrl { get; set; }
    string? ApiKey { get; set; }
}

public class SmartLeadConfig : ISmartLeadConfig
{
    public string? BaseUrl { get; set; }
    public string? ApiKey { get; set; }
}

