using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.User
{
    public class GetDeactivatedUserRequestModel
    {
        public string? SearchText { get; set; }
        public string? RelationManagerId { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public int CurrentPageIndex { get; set; }
        public int CurrentPageSize { get; set; }
    }
}
