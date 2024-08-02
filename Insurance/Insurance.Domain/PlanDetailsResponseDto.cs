using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain
{
    public class PlanDetailsResponseDto
    {
        public string logo { get; set; }
        public string tenure { get; set; }
        public string policyStartDate { get; set; }
        public bool isRecommended { get; set; }
        public BasicDetails basicDetails { get; set; }
        public PremiumBreakup premiumBreakup { get; set; }
        public CashlessGarage cashlessGarage { get; set; }
        public int IDV { get; set; }
        public int actualPrice { get; set; }
        public int netPrice { get; set; }
    }

    public class BasicDetails
    {
        public string title { get; set; }
        public string icon { get; set; }
        public string subTitle { get; set; }
        public string description { get; set; }
    }
    public class PremiumBreakup
    {
        public string vehicleNumber { get; set; }
        public string varient { get; set; }
        public string cubicCapacity { get; set; }
        public string vehicleType { get; set; } 
        public float ownDamagePremium { get; set; }
        public float thirdPartyPremium { get; set; }
        public float noClaimBonus { get; set; }
        public AddOns addOns { get; set; }
        public float packagePremium { get; set; }
        public float gst { get; set; }
    }

    public class AddOns
    {
        public float zeroDepreciation { get; set; }
        public float roadsideAssistance { get; set; }
        public float engineProtect { get; set; }
        public float NCBProtect { get; set; }
        public float keyAndLockReplacement { get; set; }
        public float consumables { get; set; }
        public float dailyAllowance { get; set; }
        public float retrunToInvoice { get; set; }
        public float tyreProtect { get; set; }
        public float RIMDamageCover { get; set; }
        public float personalBelongings { get; set; }
    }
    public class CashlessGarage
    {
        public string workshopName { get; set; }
        public string fullAddress { get; set; }
        public  string city { get; set; }
        public string state { get; set; }
        public int pincode { get; set; }    
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string productType { get; set; }
        public string emailId { get; set; }
        public int mobileNumber { get; set; }
        public string contactPerson { get; set; }
    }
}
