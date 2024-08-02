using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insurance.Domain.HDFC
{
    public class HDFCSubmitPaymentResponseDto
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public string Error { get; set; }
        public string Warning { get; set; }
        public string GC_PolicyNo { get; set; }
        public string AgentID { get; set; }
        public string Authentication { get; set; }
        public Pay_Resp_Customer_Details Customer_Details { get; set; }
        public Pay_Resp_Policy_Details Policy_Details { get; set; }
        public Pay_Payment_Detail Payment_Details { get; set; }
        public string Resp_GCV { get; set; }
        public string Resp_MISD { get; set; }
        public string Resp_PCV { get; set; }
        public string CalculatedIDV { get; set; }
        public string TransactionID { get; set; }
        public string Resp_ExtendedWarranty { get; set; }
        public string Resp_Policy_Document { get; set; }
        public Pay_Resp_TW Resp_TW { get; set; }
        public string Resp_RE { get; set; }
        public string Resp_ClaimIntimation { get; set; }
        public string Resp_ClaimStatus { get; set; }
        public string Resp_Fire2111 { get; set; }
        public Pay_Resp_Pvtcar Resp_PvtCar { get; set; }
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
        public Pay_Paymentstatusdetails PaymentStatusDetails { get; set; }
        public string Resp_PospStatus { get; set; }
        public string Resp_energy { get; set; }
        public string Response_Data_OS { get; set; }
        public string Res_ProposalStatus { get; set; }
        public string Resp_GHCIP { get; set; }
        public string Resp_CyberSachet { get; set; }
        public string Response_MarineOpen { get; set; }
        public string Response_BreakinDetails { get; set; }
    }


    public class Pay_Payment_Detail
    {
        public string PaymentID { get; set; }
        public string Payment_Date { get; set; }
        public string Payment_status { get; set; }
    }

    public class Pay_Paymentstatusdetails
    {
        public string PaymentID { get; set; }
        public string PaymentDate { get; set; }
        public string PaymentStatus { get; set; }
        public string ErrorMessage { get; set; }
        public string TransInitiatedDateTime { get; set; }
    }
    public class Pay_Resp_Customer_Details
    {
        public string CustomerID { get; set; }
        public string Customer_Name { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerMobile { get; set; }
        public string MailingAddress1 { get; set; }
        public string MailingAddress2 { get; set; }
        public string MailingAreaVillage { get; set; }
        public string MailingCityDistrict { get; set; }
        public string MailingPinCode { get; set; }
        public string MailingPinCodeLocality { get; set; }
        public string MailingState { get; set; }
        public string PermanentAddress1 { get; set; }
        public string PermanentAddress2 { get; set; }
        public string PermanentAreaVillage { get; set; }
        public string PermanentCityDistrict { get; set; }
        public string PermanentPinCode { get; set; }
        public string PermanentPinCodeLocality { get; set; }
        public string PermanentState { get; set; }
        public string Customer_UniqueRefNo { get; set; }
    }

    public class Pay_Resp_Policy_Details
    {
        public string ProposalNumber { get; set; }
        public string CertificateNumber { get; set; }
        public string PolicyNumber { get; set; }
        public string PolicyNumber_Combo { get; set; }
        public string ProposalNumber_Combo { get; set; }
        public double SumInsured { get; set; }
        public double NetPremium { get; set; }
        public double ServiceTax { get; set; }
        public double TotalPremium { get; set; }
        public double CGST { get; set; }
        public double SGST { get; set; }
        public double IGST { get; set; }
        public double UTGST { get; set; }
        public double KeralaFloodCess_Premium { get; set; }
        public string PolicyStartDate { get; set; }
        public string PolicyEndDate { get; set; }
    }

    public class Pay_Resp_TW
    {
        public double IDV { get; set; }
        public double Net_Premium { get; set; }
        public double Service_Tax { get; set; }
        public double Total_Premium { get; set; }
        public double SGST_Premium { get; set; }
        public double CGST_Premium { get; set; }
        public double IGST_Premium { get; set; }
        public double UTGST_Premium { get; set; }
        public double KeralaFloodCess_Premium { get; set; }
        public double GeogExtension_ODPremium { get; set; }
        public double GeogExtension_TPPremium { get; set; }
        public double PAOwnerDriver_Premium { get; set; }
        public double NumberOfEmployees_Premium { get; set; }
        public double PaidDriver_Premium { get; set; }
        public double BiFuel_Kit_OD_Premium { get; set; }
        public double BiFuel_Kit_TP_Premium { get; set; }
        public double Basic_TP_Premium { get; set; }
        public double Basic_OD_Premium { get; set; }
        public double InBuilt_BiFuel_Kit_Premium { get; set; }
        public double Vehicle_Base_ZD_Premium { get; set; }
        public double Elec_ZD_Premium { get; set; }
        public double NonElec_ZD_Premium { get; set; }
        public double Bifuel_ZD_Premium { get; set; }
        public double Electical_Acc_Premium { get; set; }
        public double NonElectical_Acc_Premium { get; set; }
        public double LimitedtoOwnPremises_OD_Premium { get; set; }
        public double LimitedtoOwnPremises_TP_Premium { get; set; }
        public double OtherLoading_Premium { get; set; }
        public double OtherDiscount_Premium { get; set; }
        public double AntiTheftDisc_Premium { get; set; }
        public double HandicapDisc_Premium { get; set; }
        public double NCBBonusDisc_Premium { get; set; }
        public double Vehicle_Base_NCB_Premium { get; set; }
        public double Elec_NCB_Premium { get; set; }
        public double NonElec_NCB_Premium { get; set; }
        public double Bifuel_NCB_Premium { get; set; }
        public double Vehicle_Base_RTI_Premium { get; set; }
        public double Elec_RTI_Premium { get; set; }
        public double NonElec_RTI_Premium { get; set; }
        public double Bifuel_RTI_Premium { get; set; }
        public double EA_premium { get; set; }
        public double TPPD_premium { get; set; }
        public double VoluntartDisc_premium { get; set; }
        public double UnnamedPerson_premium { get; set; }
        public double NamedPerson_premium { get; set; }
        public double Automobile_Disc_premium { get; set; }
        public double MinimumIDV { get; set; }
        public double MaximumIDV { get; set; }
    }

    public class Pay_Resp_Pvtcar
    {
        public double IDV { get; set; }
        public double Net_Premium { get; set; }
        public double Service_Tax { get; set; }
        public double Total_Premium { get; set; }
        public double SGST_Premium { get; set; }
        public double CGST_Premium { get; set; }
        public double IGST_Premium { get; set; }
        public double UTGST_Premium { get; set; }
        public double KeralaFloodCess_Premium { get; set; }
        public double GeogExtension_ODPremium { get; set; }
        public double GeogExtension_TPPremium { get; set; }
        public double PAOwnerDriver_Premium { get; set; }
        public double NumberOfEmployees_Premium { get; set; }
        public double PaidDriver_Premium { get; set; }
        public double BiFuel_Kit_OD_Premium { get; set; }
        public double BiFuel_Kit_TP_Premium { get; set; }
        public double Basic_TP_Premium { get; set; }
        public double Basic_OD_Premium { get; set; }
        public double InBuilt_BiFuel_Kit_Premium { get; set; }
        public double Vehicle_Base_ZD_Premium { get; set; }
        public double Vehicle_Base_TySec_Premium { get; set; }
        public double Elec_ZD_Premium { get; set; }
        public double NonElec_ZD_Premium { get; set; }
        public double Bifuel_ZD_Premium { get; set; }
        public double Electical_Acc_Premium { get; set; }
        public double NonElectical_Acc_Premium { get; set; }
        public double LimitedtoOwnPremises_OD_Premium { get; set; }
        public double LimitedtoOwnPremises_TP_Premium { get; set; }
        public double OtherLoading_Premium { get; set; }
        public double OtherDiscount_Premium { get; set; }
        public double AntiTheftDisc_Premium { get; set; }
        public double HandicapDisc_Premium { get; set; }
        public double NCBBonusDisc_Premium { get; set; }
        public double Vehicle_Base_NCB_Premium { get; set; }
        public double Elec_NCB_Premium { get; set; }
        public double NonElec_NCB_Premium { get; set; }
        public double Bifuel_NCB_Premium { get; set; }
        public double Vehicle_Base_RTI_Premium { get; set; }
        public double Elec_RTI_Premium { get; set; }
        public double NonElec_RTI_Premium { get; set; }
        public double Bifuel_RTI_Premium { get; set; }
        public double EA_premium { get; set; }
        public double TPPD_premium { get; set; }
        public double VoluntartDisc_premium { get; set; }
        public double UnnamedPerson_premium { get; set; }
        public double NamedPerson_premium { get; set; }
        public double Automobile_Disc_premium { get; set; }
        public double Towing_premium { get; set; }
        public double EAAdvance_premium { get; set; }
        public double EAW_premium { get; set; }
        public double EMI_PROTECTOR_PREMIUM { get; set; }
        public double Towing_Limit_SI { get; set; }
        public double Vehicle_Base_COC_Premium { get; set; }
        public double Elec_COC_Premium { get; set; }
        public double NonElec_COC_Premium { get; set; }
        public double Bifuel_COC_Premium { get; set; }
        public double Vehicle_Base_ENG_Premium { get; set; }
        public double Elec_ENG_Premium { get; set; }
        public double NonElec_ENG_Premium { get; set; }
        public double Bifuel_ENG_Premium { get; set; }
        public double Loss_of_Use_Premium { get; set; }
        public double BreakIN_Premium { get; set; }
        public double PAPaidDriver_Premium { get; set; }
        public double MinimumIDV { get; set; }
        public double MaximumIDV { get; set; }
        public string PayAsYouDrive { get; set; }
        public string InitialOdometerReading { get; set; }
        public string InitialOdometerReadingDate { get; set; }
        public double HighProtection_Premium { get; set; }
        public double LossOfPersonalBelongings_Premium { get; set; }

    }

}
