using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.Roles
{
    public class GetAssistedBUDetailsResponseModel
    {
        public IEnumerable<AssistedBUDetails>? AssistedBUDetails { get; set; }
        public IEnumerable<AssistedSelectedBUDetails>? AssistedSelectedBUDetails { get; set; }
    }
    public class AssistedBUDetails
    {
        public string? BUId { get; set; }
        public string? BUName { get; set; }
    }
    public class AssistedSelectedBUDetails
    {
        public string? SelectedBUId { get; set; }
        public string? SelectedBuName { get; set; }
    }
}
