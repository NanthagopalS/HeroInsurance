using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.Roles
{
    public class ModuleResponseModel
    {
        public string? ModuleGroupName { get; set; }
        public string? ModuleId { get; set; }
        public string? ModuleName { get; set; }
        public int? PriorityIndex { get; set; }
        public bool? AddOption { get; set; }
        public bool? EditOption { get; set; }
        public bool? ViewOption { get; set; }
        public bool? DeleteOption { get; set; }
        public bool? DownloadOption { get; set; }
        public bool? IsActive { get; set; }
    }
}






