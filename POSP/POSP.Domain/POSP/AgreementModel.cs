using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace POSP.Domain.POSP
{
    public class AgreementModel
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
        /// AgreementId
        /// </summary>
        public string? AgreementId { get; set; }

        /// <summary>
        /// PreSignedAgreementId
        /// </summary>
        public string? PreSignedAgreementId { get; set; }
        public string? Image64 { get; set; }
        public string? ConfigurationValue { get; set; }
        public byte[]? SignatureImage { get; set; }

        [JsonIgnore]
        public string? EmailId { get; set; }

        [JsonIgnore]
        public string? UserName { get; set; }

        [JsonIgnore]
        public string? POSPId { get; set; }

        [JsonIgnore]
        public string? MobileNo { get; set; }
        public string? Environment { get; set; }

    }
}
