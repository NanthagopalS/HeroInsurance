using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POSP.Domain.POSP
{
    /// <summary>
    /// ManagePOSPTrainingModel
    /// </summary>
    public class POSPTrainingModel
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
        /// trainingmoduletype 
        /// </summary>
        public string? TrainingModuleType { get; set; }

        /// <summary>
        /// GeneralInsuranceStartDateTime
        /// </summary>
        public DateTime? GeneralInsuranceStartDateTime { get; set; }

        /// <summary>
        /// GeneralInsuranceEndDateTime
        /// </summary>
        public DateTime? GeneralInsuranceEndDateTime { get; set; }

        /// <summary>
        /// LifeInsuranceStartDateTime
        /// </summary>
        public DateTime? LifeInsuranceStartDateTime { get; set; }

        /// <summary>
        /// LifeInsuranceEndDateTime
        /// </summary>
        public DateTime? LifeInsuranceEndDateTime { get; set; }

        /// <summary>
        /// IsGeneralInsuranceCompleted
        /// </summary>
        public bool? IsGeneralInsuranceCompleted { get; set; }

        /// <summary>
        /// IsLifeInsuranceCompleted
        /// </summary>
        public bool? IsLifeInsuranceCompleted { get; set; }

        /// <summary>
        /// IsTrainingCompleted
        /// </summary>
        public bool? IsTrainingCompleted { get; set; }

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

    public class POSPResponseTrainingModel
    {
        /// <summary>
        /// Id
        /// </summary>
        public string? TrainingId { get; set; }

        /// <summary>
        /// GeneralInsuranceStartDateTime
        /// </summary>
        public DateTime? GeneralInsuranceStartDateTime { get; set; }

        /// <summary>
        /// GeneralInsuranceIdealEndDateTime
        /// </summary>
        public DateTime? GeneralInsuranceIdealEndDateTime { get; set; }

        /// <summary>
        /// GeneralInsuranceEndDateTime
        /// </summary>
        public DateTime? GeneralInsuranceEndDateTime { get; set; }

        /// <summary>
        /// LifeInsuranceStartDateTime
        /// </summary>
        public DateTime? LifeInsuranceStartDateTime { get; set; }

        /// <summary>
        /// LifeInsuranceIdealEndDateTime
        /// </summary>
        public DateTime? LifeInsuranceIdealEndDateTime { get; set; }

        /// <summary>
        /// LifeInsuranceEndDateTime
        /// </summary>
        public DateTime? LifeInsuranceEndDateTime { get; set; }


    }

}
