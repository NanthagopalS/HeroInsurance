using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Domain.POSP;

/// <summary>
/// TrainingMaterialDetailModel
/// </summary>
public class TrainingMaterialDetailModel
{
    /// <summary>
    /// Id
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// TrainingModuleType
    /// </summary>
    public string? TrainingModuleType { get; set; }

    /// <summary>
    /// MaterialFormatType
    /// </summary>
    public string? MaterialFormatType { get; set; }

    /// <summary>
    /// VideoDuration
    /// </summary>
    public string? VideoDuration { get; set; }


    /// <summary>
    /// LessonNumber
    /// </summary>
    public string? LessonNumber { get; set; }

    /// <summary>
    /// LessonTitle
    /// </summary>
    public string? LessonTitle { get; set; }

    /// <summary>
    /// DocumentFileName
    /// </summary>
    public string? DocumentFileName { get; set; }

    /// <summary>
    /// DocumentId
    /// </summary>
    public string? DocumentId { get; set; }

    /// <summary>
    /// PriorityIndex
    /// </summary>
    public int? PriorityIndex { get; set; }

    /// <summary>
    /// IsActive
    /// </summary>
    public bool? IsActive { get; set; }

    /// <summary>
    /// CreatedBy
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// CreatedOn
    /// </summary>
    public DateTime? CreatedOn { get; set; }

    /// <summary>
    /// UpdatedBy
    /// </summary>
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// UpdatedOn
    /// </summary>
    public DateTime?UpdatedOn { get; set; }

    /// <summary>
    /// IsTrainingCompleted
    /// </summary>
    public bool? IsTrainingCompleted { get; set; }

    public string? Image64 { get; set; }


}



