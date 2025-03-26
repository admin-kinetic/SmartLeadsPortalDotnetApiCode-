using System;

namespace ImportLeadsFromLeadsPortal.Models;

public class LeadsPortalResponse
{
    public int id { get; set; }
    public DateTime exportedDate { get; set; }  
    public string? email { get; set; }
    public string? contactSource { get; set; }
    public int rate { get; set; }

}
