using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MongoDB.Bson.Serialization.Serializers.SerializerHelper;
using System.Xml.Linq;

namespace Identity.Domain.User
{
    public class POSPAccountDetailModel
    {
        public string InterestInProduct { get; set; }
        public string SellingExperience { get; set; }
        public string ICName { get; set; }
        public string PremiumSold { get; set; }
        public string OnboardingDate { get; set; }
        public string StampNumber { get; set; }
        public string SourcedBy { get; set; }
        public string CreatedBy { get; set; }
        public string ServicedBy { get; set; }
        public string PreSale { get; set; }
        public string PostSale { get; set; }
        public string Marketing { get; set; }
        public string Claim { get; set; }
        public string PreSaleUserId { get; set; }
        public string PostSaleUserId { get; set; }
        public string MarketingUserId { get; set; }
        public string ClaimUserId { get; set; }
    }
}
