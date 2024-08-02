using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.User
{
    public class POSPConfigurationDetailModel
    {
        public string Id { get; set; }
        public string ConfigurationKey { get; set; }
        public string ConfigurationValue { get; set; }
        public string ConfigurationLable { get; set; }
        public string ConfigurationIcon { get; set; }
        public string ConfigurationName { get; set; }
        public string IsActive { get; set; }

    }

}
