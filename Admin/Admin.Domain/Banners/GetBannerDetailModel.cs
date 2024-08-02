using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.Banners
{
    public class GetBannerDetailModel
    {
        public string? BannerId { get; set; }
        public string? DocumentId { get; set; }
        public string? ProductCategoryId { get; set; }
        public string? ProductCategoryName { get; set; }
        public string? ImageStream { get; set; }
    }
}
