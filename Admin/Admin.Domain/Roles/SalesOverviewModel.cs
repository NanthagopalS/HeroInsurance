using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.Roles
{
    public class SalesOverviewModel
    {
        //public string? TotalPolicySold { get; set; }
        //public string? TotalPremiumSold { get; set; }
        public int? Id { get; set; }
        public string? TitleName { get; set; }
        public float? Amount { get; set; }
        public float? PercentageValue { get; set; }
        public string? ArrowSign { get; set; }
       

    }
}
