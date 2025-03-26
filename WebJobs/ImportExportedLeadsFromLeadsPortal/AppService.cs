using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportedLeadsFromLeadsPortal;

public class AppService
{
    public void Run()
    {
        Console.WriteLine("Exported Leads from Leads Portal");

        var LeadsPortalService = new LeadsPortalService();
        var fromDate = DateTime.Now.AddDays(-1);
        var toDate = DateTime.Now;
        var exportedContacts = LeadsPortalService.GetExportedToSmartleadsContacts(fromDate, toDate).Result;

        Console.WriteLine($"Exported contacts from {fromDate} to {toDate}:");
    }
}

