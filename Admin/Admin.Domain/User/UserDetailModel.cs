using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Domain.User
{
    public class UserDetailModel
    {
        public string UserId { get; set; }
        public string Gender { get; set; }
        public string DateofBirth { get; set; }
        public string AlternateContactNo { get; set; }
        public string PAN { get; set; }
        public string AadhaarNumber { get; set; }
        public string IsNameDifferentOnDocument { get; set; }
        public string AliasName { get; set; }
        public string NOCAvailable { get; set; }
        public string IsSelling { get; set; }
        public string EducationQualificationTypeId { get; set; }
        public string EducationQualificationType { get; set; }
        public string InsuranceSellingExperienceRangeId { get; set; }
        public string PremiumRangeType { get; set; }        
        public string InsuranceProductsofInterestTypeId { get; set; }
        public string ProductName { get; set; }
        public string ServicedByUserId { get; set; }
        public string InsurerCompanyName { get; set; }
        //public string POSPSourceMode { get; set; }
        //public string POSPSourceTypeId { get; set; }
        //public string SourcedByUserId { get; set; }
        //public string ServicedByUserId { get; set; }        
        //public string ProfilePictureID { get; set; }
        //public string ProfilePictureFileName { get; set; }
        //public string ProfilePictureStoragePath { get; set; }
        //public string NameDifferentDocument { get; set; }
        //public string NameDifferentDocumentFilePath { get; set; }
    }
}
