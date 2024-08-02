using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.Roles
{
    public class GetAllUserRoleMappingInputModel
    {
        public string? EmployeeIdorName { get; set; }
        public string? RoleTypeId { get; set; }
        public bool? StatusId { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public int? CurrentPageIndex { get; set; }
        public int? CurrentPageSize { get; set; }
    }
}
