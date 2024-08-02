using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.HDFC
{

    public class IDVResponseModel
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Error { get; set; }
        public string Warning { get; set; }
        public string GC_PolicyNo { get; set; }
        public string AgentID { get; set; }
        public string Authentication { get; set; }
        public string Customer_Details { get; set; }
        public string Policy_Details { get; set; }
        public string Payment_Details { get; set; }
        public string Resp_GCV { get; set; }
        public string Resp_MISD { get; set; }
        public string Resp_PCV { get; set; }
        public CalculatedIDV CalculatedIDV { get; set; }
        public string TransactionID { get; set; }
        public string Resp_ExtendedWarranty { get; set; }
        public string Resp_Policy_Document { get; set; }
        public string Resp_TW { get; set; }
        public string Resp_RE { get; set; }
        public string Resp_ClaimIntimation { get; set; }
        public string Resp_ClaimStatus { get; set; }
        public string Resp_Fire2111 { get; set; }
        public string Resp_PvtCar { get; set; }
        public string Resp_IPA { get; set; }
        public string Resp_HSTPI { get; set; }
        public string Resp_HSTPF { get; set; }
        public string Resp_Discount { get; set; }
        public string Resp_POSP { get; set; }
        public string Res_NSTPDecision { get; set; }
        public string Res_GetStatus { get; set; }
        public string Resp_UploadDocument { get; set; }
        public string Resp_PolicyDetails { get; set; }
        public string Res_PolicyStatus { get; set; }
        public string Res_MasterData { get; set; }
        public string Res_appstatus { get; set; }
        public string Resp_CBDetails { get; set; }
        public string Resp_OptimaRestore { get; set; }
        public string Resp_AMIPA { get; set; }
        public string Resp_Ican { get; set; }
        public string Resp_ECB_ClaimStatus { get; set; }
        public string Resp_OptimaSuper { get; set; }
        public string Resp_CDBalance { get; set; }
        public string Resp_HSI { get; set; }
        public string Resp_HF { get; set; }
        public string Resp_HW { get; set; }
        public string PaymentStatusDetails { get; set; }
        public string Resp_PospStatus { get; set; }
        public string Resp_energy { get; set; }
        public string Response_Data_OS { get; set; }
        public string Res_ProposalStatus { get; set; }
        public string Resp_GHCIP { get; set; }
        public string Resp_CyberSachet { get; set; }
        public string Response_MarineOpen { get; set; }
    }

    public class CalculatedIDV
    {
        public double IDV_AMOUNT { get; set; }
        public double MAX_IDV_AMOUNT { get; set; }
        public double MIN_IDV_AMOUNT { get; set; }
        public double EX_SHOWROOM_PRICE { get; set; }
        public Addonplanstocoversmapping[] addonPlansToCoversMapping { get; set; }
    }

    public class Addonplanstocoversmapping
    {
        public string coverGroup { get; set; }
        public string planType { get; set; }
        public bool isEligibile { get; set; }
        public bool isMandatory { get; set; }
        public double userRate { get; set; }
        public string coverStartDate { get; set; }
        public string coverEndDate { get; set; }
        public bool isDeleted { get; set; }
        public bool isNewlyAdded { get; set; }
        public string coverType { get; set; }
        public string coverClass { get; set; }
        public string previuosYearRate { get; set; }
        public double computedRate { get; set; }
        public double minRate { get; set; }
        public double premium { get; set; }
        public string newPremium { get; set; }
        public double endorsementAmount { get; set; }
        public string coverLevelDiscountLoading { get; set; }
        public int coverLevelDiscountLoadingLength { get; set; }
    }

}
