using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.Magma
{
    public class MagmaServiceResponseModel
    {
        public string ServiceResult { get; set; }
        public Outputresult OutputResult { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorText { get; set; }
    }

    public class Outputresult
    {
        public string ProposalNumber { get; set; }
        public string ProposalDate { get; set; }
        public string HigherIDV { get; set; }
        public string LowerIDV { get; set; }
        public string IDVofthevehicle { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerType { get; set; }
        public string CurrentNCBPer { get; set; }
        public int MaxDiscountAppicable { get; set; }
        public int AppliedDiscount { get; set; }
        public Premiumbreakup PremiumBreakUp { get; set; }
        public bool IsProposalPending { get; set; }
        public Proposalpendingdetails ProposalPendingDetails { get; set; }
    }

    public class Premiumbreakup
    {
        public Vehiclebasevalue VehicleBaseValue { get; set; }
        public string[] OptionalAddOnCovers { get; set; }
        public Discount[] Discount { get; set; }
        public string Loading { get; set; }
        public float NetPremium { get; set; }
        public float SGST { get; set; }
        public float CGST { get; set; }
        public float TotalPremium { get; set; }
    }

    public class Vehiclebasevalue
    {
        public Addoncover[] AddOnCover { get; set; }
        public float AddOnCoverTotalPremium { get; set; }
    }

    public class Addoncover
    {
        public string AddOnCoverType { get; set; }
        public float AddOnCoverTypePremium { get; set; }
    }

    public class Discount
    {
        public string DiscountType { get; set; }
        public float DiscountTypeAmount { get; set; }
    }

    public class Proposalpendingdetails
    {
        public bool IsApprovalRequired { get; set; }
        public bool IsDocumentRequired { get; set; }
        public bool IsInspectionRequired { get; set; }
        public bool IsInternalProcess { get; set; }
        public bool IsQCRequired { get; set; }
        public string InspectionCategory { get; set; }
        public string PreInspectionNumber { get; set; }
        public bool MHDI_ZONE_UNDERWRITER { get; set; }
    }

}
