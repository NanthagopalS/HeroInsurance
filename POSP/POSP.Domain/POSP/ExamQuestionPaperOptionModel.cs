using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Domain.POSP;

/// <summary>
/// ExamQuestionPaperOptionModel
/// </summary>
public class ExamQuestionPaperOptionModel
{
    /// <summary>
    /// Id
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// QuestionId
    /// </summary>
    public string? QuestionId { get; set; }

    /// <summary>
    /// OptionIndex
    /// </summary>
    public string?OptionIndex { get; set; }

    /// <summary>
    /// OptionValue
    /// </summary>
    public string?OptionValue { get; set; }

    /// <summary>
    /// IsCorrectAnswer
    /// </summary>
    public bool? IsCorrectAnswer { get; set; }

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
    public DateTime? UpdatedOn { get; set; }
}
