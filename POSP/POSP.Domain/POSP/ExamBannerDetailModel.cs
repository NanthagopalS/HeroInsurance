using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Domain.POSP;


/// <summary>
/// ExamBannerDetailModel
/// </summary>
public class ExamBannerDetailModel
{

    /// <summary>
    /// Id
    /// </summary>
    public string? Id { get; set; }


    /// <summary>
    /// BannerType
    /// </summary>
    public string? BannerType { get; set; }

    /// <summary>
    /// BannerFileName
    /// </summary>
    public string? BannerFileName { get; set; }

    /// <summary>
    /// BannerStoragePath
    /// </summary>
    public string? BannerStoragePath { get; set; }

    /// <summary>
    /// BannnerImage
    /// </summary>
    public byte[]? BannnerImage { get; set; }

    public string? DocumentId { get; set; }

    public string? Image64 { get; set; }

}
