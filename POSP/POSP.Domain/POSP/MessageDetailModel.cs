using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Domain.POSP
{
    public class MessageDetailModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// MessageKey
        /// </summary>
        public string MessageKey { get; set; }

        /// <summary>
        /// MessageValue
        /// </summary>
        public string MessageValue { get; set; }

        /// <summary>
        /// MessageValue
        /// </summary>
        public int PriorityIndex { get; set; }

        /// <summary>
        /// IsActive
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// CreatedOn
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// CreatedBy
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// UpdatedBy
        /// </summary>
        public string UpdatedBy { get; set; }

        /// <summary>
        /// UpdatedOn
        /// </summary>
        public DateTime UpdatedOn { get; set; }
    }

}
public class MessageDetailResponseModel
{
    /// <summary>
    /// Id
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// TitleValue
    /// </summary>
    public string TitleValue { get; set; }
    /// <summary>
    /// Subtitle1
    /// </summary>
    public string Subtitle1 { get; set; }
    /// <summary>
    /// Subtitle2
    /// </summary>
    public string Subtitle2 { get; set; }
    /// <summary>
    /// Subtitle3
    /// </summary>
    public string Subtitle3 { get; set; }

}

