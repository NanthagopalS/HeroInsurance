namespace POSP.Domain.POSP;

public class ExamCertificateModel
{
    /// <summary>
    /// Id
    /// </summary>
    public string? Id { get; set; }
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? EmailId { get; set; }


    /// <summary>
    /// BannerFileName
    /// </summary>
    public string? BannerFileName { get; set; }

    /// <summary>
    /// BannnerImage
    /// </summary>
    public byte[]? BannnerImage { get; set; }

    public string? DocumentId { get; set; }
    public string? ConfigurationValue { get; set; }
    public bool? IsCleared { get; set; }

    public string? Image64 { get; set; }
    public string? StateName { get; set; }
    public string? AadhaarNumber { get; set; }
    public string? PAN { get; set; }
    public string? AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public string? POSPId { get; set; }

}
