using System;

namespace SmartLeadsPortalDotNetApi.Model;

public class ExportedContactsPayload
{
    public int id { get; set; }
    public DateTime exportedDate { get; set; }
    public string? email { get; set; }
    public string? contactSource { get; set; }
    public int rate { get; set; }
}
