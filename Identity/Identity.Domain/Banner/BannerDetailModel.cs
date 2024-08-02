using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Banner;

/// <summary>
/// BannerDetailModel
/// </summary>
public class BannerDetailModel
{

    /// <summary>
    /// Id
    /// </summary>
    public string Id { get; set; }


    /// <summary>
    /// BannerFileName
    /// </summary>
    public string BannerFileName { get; set; }
    //public List<string> DocumentTypeId { get; set; }

    /// <summary>
    /// BannerStoragePath
    /// </summary>
    public string BannerStoragePath { get; set; }



    /// <summary>
    /// BannerType
    /// </summary>
    public string BannerType { get; set; }

    /// <summary>
    /// BannnerImage
    /// </summary>
    public byte[] BannnerImage { get; set; }

    public string DocumentId { get; set; }

    public string Image64 { get; set; }

}
