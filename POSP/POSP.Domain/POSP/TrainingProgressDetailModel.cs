using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Domain.POSP
{
    public class TrainingProgressDetailModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// TrainingId
        /// </summary>
        public string TrainingId { get; set; }
        public string UserId { get; set; }


        /// <summary>
        /// TrainingMaterialId
        /// </summary>
        public string TrainingMaterialId { get; set; }

        /// <summary>
        /// TrainingStartDateTime
        /// </summary>
        public DateTime TrainingStartDateTime { get; set; }

        /// <summary>
        /// IsTrainingCompleted
        /// </summary>
        public bool IsTrainingCompleted { get; set; }

        /// <summary>
        /// IsActive
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// CreatedBy
        /// </summary>
        public string CreatedBy { get; set; }

        /// <summary>
        /// CreatedOn
        /// </summary>
        public DateTime CreatedOn { get; set; }        

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
