using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Admin.Domain.Roles
{
    public class BUSearchInputModel
    {
        public string BUName { get; set; }
        public string CreatedFrom { get; set; }
        public string CreatedTo { get; set; }     
    }
}
