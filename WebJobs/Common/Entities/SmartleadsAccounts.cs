using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities;

public class SmartleadsAccounts
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ApiKey { get; set; }
    public bool? IsDeleted { get; set; }
}
