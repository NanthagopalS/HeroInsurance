using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.User
{
    public class POSPOnboardingDetailModel
    {
        public int? Id { get; set; }
        public string? ColumnName { get; set;}
        public string? ColumnValue { get; set;}
        public string? GroupNumber { get; set; }

    }
}
