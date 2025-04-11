using System;

namespace SmartLeadsPortalDotNetApi.Configs;

public class MicrosoftGraphSettings
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string TenantId { get; set; }
    public string Scope { get; set; }
    public string Authority { get; set; }
}
