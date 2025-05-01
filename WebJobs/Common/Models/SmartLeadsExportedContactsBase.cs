using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class SmartLeadsExportedContactsBase
    {
        public int Id { get; set; }
        public string? Email { get; set; }
        public string? ContactSource { get; set; }
        public int? Rate { get; set; }
    }
}
