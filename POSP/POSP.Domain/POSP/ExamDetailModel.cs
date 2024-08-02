using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Domain.POSP;

public class ExamDetailModel
{

    /// <summary>
    /// Id
    /// </summary>
    public string? Id { get; set; }


    /// <summary>
    /// UserId
    /// </summary>
    public string? UserId { get; set; }


    /// <summary>
    /// ExamStartDateTime
    /// </summary>
    public DateTime? ExamStartDateTime { get; set; }


    /// <summary>
    /// ExamEndDateTime
    /// </summary>
    public DateTime? ExamEndDateTime { get; set; }


    /// <summary>
    /// CorrectAnswered
    /// </summary>
    public int? CorrectAnswered { get; set; }


    /// <summary>
    /// InCorrectAnswered
    /// </summary>
    public int? InCorrectAnswered { get; set; }


    /// <summary>
    /// FinalResult
    /// </summary>
    public float? FinalResult { get; set; }


    /// <summary>
    /// IsCleared
    /// </summary>
    public bool? IsCleared { get; set; }

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
    public string? UpdatedOn { get; set; }
}


public class ExamDetailResponse
{
    /// <summary>
    /// Id
    /// </summary>
    public string? Id { get; set; }


    /// <summary>
    /// ExamStartDateTime
    /// </summary>
    public DateTime? ExamStartDateTime { get; set; }

    /// <summary>
    /// ExamEndDateTime
    /// </summary>
    public DateTime? ExamEndDateTime { get; set; }

    /// <summary>
    /// ExamIdealEndDateTime
    /// </summary>
    public DateTime? ExamIdealEndDateTime { get; set; }
}