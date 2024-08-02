using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.Roles
{
    public class GetProductCategoryModel
    {
        public string ProductCategoryId { get; set; }
        public string ProductCategoryName { get; set; }
        public bool IsActive { get; set; }
        public string Icon { get; set; }


    }
}
