using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReprocessSmartleadsFullName.Models;

internal class Leads;

public class LeadsToBeUpdated {
    public string? Email { get; set; }
    public int AccountId { get; set; }
    public string? AccountName { get; set; }
    public string? AccountApiKey { get; set; }
}

