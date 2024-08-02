using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.Quote
{
    public class PackageModel
    {
        public string PackageName { get; set; }
        public string PackageFlag { get; set; }
        public int PackageValidityInDays { get; set; }
    }
}
