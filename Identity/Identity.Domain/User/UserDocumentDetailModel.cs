using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThirdPartyUtilities.Models.MongoDB;

namespace Identity.Domain.User;

/// <summary>
/// UserDocumentDetailModel
/// </summary>
public class UserDocumentDetailModel
{
    public string UserId { get; set; }
    public string DocumentTypeId { get; set; }
    public string DocumentType { get; set; }
    public bool IsMandatory { get; set; }
    public string DocumentFileName { get; set; }
    public bool IsVerify { get; set; }
    public bool IsApprove { get; set; }

    /// <summary>
    /// FileSize
    /// </summary>
    public string FileSize { get; set; }
    /// <summary>
    /// CreatedOn
    /// </summary>
    public DateTime CreatedOn { get; set; }
    /// <summary>
    /// UpdatedOn
    /// </summary>
    public DateTime UpdatedOn { get; set; }


    /// <summary>
    /// ImageStream
    /// </summary>
    public byte[] ImageStream { get; set; }

    public string DocumentId { get; set; }

    public string Image64 { get; set; }
    public bool IsDraft { get; set; }
    public bool IsAdminUpdating { get; set; }



}
