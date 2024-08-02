using System.Xml;
using System.Xml.Serialization;

namespace Insurance.Domain.UnitedIndia;

/// <summary>
/// If any proprty added here  then update the calculatePremiumRequest as well
/// </summary>
[XmlRoot(ElementName = "calculatePremium")]
public class CalculatePremium
{
    [XmlElement(ElementName = "application")]
    public string application { get; set; }
    [XmlElement(ElementName = "userid")]
    public string userid { get; set; }
    [XmlElement(ElementName = "password")]
    public string password { get; set; }
    [XmlText]
    [XmlElement(ElementName = "proposalXml")]
    public string proposalXml { get; set; }
    [XmlElement(ElementName = "productCode")]
    public string productCode { get; set; }
    [XmlElement(ElementName = "subproductCode")]
    public string subproductCode { get; set; }
}

[XmlRoot(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
public class UnitedIndiaBody
{
    [XmlElement(ElementName = "calculatePremium")]
    public CalculatePremium calculatePremium { get; set; }
}

[XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")] //xmlns:ws="http://ws.uiic.com/
public class UnitedIndiaEnvelope
{
    [XmlElement(ElementName = "Header", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public string Header { get; set; }
    [XmlElement(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public UnitedIndiaBody Body { get; set; }
}

[XmlRoot(ElementName = "HEADER")]
public class UnitedIndiaHeader
{
    [XmlElement(ElementName = "TXT_POSP_CODE")]
    public string TXT_POSP_CODE { get; set; }
    [XmlElement(ElementName = "TXT_POSP_NAME")]
    public string TXT_POSP_NAME { get; set; }
    [XmlElement(ElementName = "CUR_BONUS_MALUS_PERCENT")]
    public string CUR_BONUS_MALUS_PERCENT { get; set; }
    [XmlElement(ElementName = "CUR_DEALER_GROSS_PREM")]
    public string CUR_DEALER_GROSS_PREM { get; set; }
    [XmlElement(ElementName = "CUR_DEALER_NET_OD_PREM")]
    public string CUR_DEALER_NET_OD_PREM { get; set; }
    [XmlElement(ElementName = "CUR_DEALER_NET_TP_PREM")]
    public string CUR_DEALER_NET_TP_PREM { get; set; }
    [XmlElement(ElementName = "CUR_DEALER_SERVICE_TAX")]
    public string CUR_DEALER_SERVICE_TAX { get; set; }
    [XmlElement(ElementName = "DAT_AA_EXPIRY_DATE")]
    public string DAT_AA_EXPIRY_DATE { get; set; }
    [XmlElement(ElementName = "DAT_DATE_OF_EXPIRY_OF_POLICY")]
    public string DAT_DATE_OF_EXPIRY_OF_POLICY { get; set; }
    [XmlElement(ElementName = "DAT_DATE_OF_ISSUE_OF_POLICY")]
    public string DAT_DATE_OF_ISSUE_OF_POLICY { get; set; }
    [XmlElement(ElementName = "DAT_DATE_OF_REGISTRATION")]
    public string DAT_DATE_OF_REGISTRATION { get; set; }
    [XmlElement(ElementName = "DAT_DRIVING_LICENSE_EXP_DATE")]
    public string DAT_DRIVING_LICENSE_EXP_DATE { get; set; }
    [XmlElement(ElementName = "DAT_HOURS_EFFECTIVE_FROM")]
    public string DAT_HOURS_EFFECTIVE_FROM { get; set; }
    [XmlElement(ElementName = "DAT_PREV_POLICY_EXPIRY_DATE")]
    public string DAT_PREV_POLICY_EXPIRY_DATE { get; set; }
    [XmlElement(ElementName = "DAT_PROPOSAL_DATE")]
    public string DAT_PROPOSAL_DATE { get; set; }
    [XmlElement(ElementName = "DAT_UTR_DATE")]
    public string DAT_UTR_DATE { get; set; }
    [XmlElement(ElementName = "NUM_IEV_ELEC_ACC_VALUE")]
    public string NUM_IEV_ELEC_ACC_VALUE { get; set; }
    [XmlElement(ElementName = "ELECTRICALACCESSORIESPREM")]
    public string ELECTRICALACCESSORIESPREM { get; set; }
    [XmlElement(ElementName = "MEM_ADDRESS_OF_INSURED")]
    public string MEM_ADDRESS_OF_INSURED { get; set; }
    [XmlElement(ElementName = "NOCLAIMBONUSDISCOUNT")]
    public string NOCLAIMBONUSDISCOUNT { get; set; }
    [XmlElement(ElementName = "NUM_IEV_NON_ELEC_ACC_VALUE")]
    public string NUM_IEV_NON_ELEC_ACC_VALUE { get; set; }
    [XmlElement(ElementName = "NONELECTRICALACCESSORIESPREM")]
    public string NONELECTRICALACCESSORIESPREM { get; set; }
    [XmlElement(ElementName = "NUM_AGREEMENT_NAME_1")]
    public string NUM_AGREEMENT_NAME_1 { get; set; }
    [XmlElement(ElementName = "NUM_AGREEMENT_NAME_2")]
    public string NUM_AGREEMENT_NAME_2 { get; set; }
    [XmlElement(ElementName = "NUM_BUSINESS_CODE")]
    public string NUM_BUSINESS_CODE { get; set; }
    [XmlElement(ElementName = "NUM_CLIENT_TYPE")]
    public string NUM_CLIENT_TYPE { get; set; }
    [XmlElement(ElementName = "NUM_COMPULSORY_EXCESS_AMOUNT")]
    public string NUM_COMPULSORY_EXCESS_AMOUNT { get; set; }
    [XmlElement(ElementName = "NUM_CUBIC_CAPACITY")]
    public string NUM_CUBIC_CAPACITY { get; set; }
    [XmlElement(ElementName = "NUM_DAYS_COVER_FOR_COURTESY")]
    public string NUM_DAYS_COVER_FOR_COURTESY { get; set; }
    [XmlElement(ElementName = "NUM_FINANCIER_NAME_1")]
    public string NUM_FINANCIER_NAME_1 { get; set; }
    [XmlElement(ElementName = "NUM_FINANCIER_NAME_2")]
    public string NUM_FINANCIER_NAME_2 { get; set; }
    [XmlElement(ElementName = "NUM_GEOGRAPHICAL_EXTN_PREM")]
    public string NUM_GEOGRAPHICAL_EXTN_PREM { get; set; }
    [XmlElement(ElementName = "NUM_IEV_BASE_VALUE")]
    public string NUM_IEV_BASE_VALUE { get; set; }
    [XmlElement(ElementName = "NUM_IEV_CNG_VALUE")]
    public string NUM_IEV_CNG_VALUE { get; set; }
    [XmlElement(ElementName = "NUM_IEV_FIBRE_TANK_VALUE")]
    public string NUM_IEV_FIBRE_TANK_VALUE { get; set; }
    [XmlElement(ElementName = "NUM_IEV_LPG_VALUE")]
    public List<string> NUM_IEV_LPG_VALUE { get; set; }
    [XmlElement(ElementName = "NUM_IEV_SIDECAR_VALUE")]
    public string NUM_IEV_SIDECAR_VALUE { get; set; }
    [XmlElement(ElementName = "NUM_IMPOSED_EXCESS_AMOUNT")]
    public string NUM_IMPOSED_EXCESS_AMOUNT { get; set; }
    [XmlElement(ElementName = "NUM_LD_CLEANER_CONDUCTOR")]
    public string NUM_LD_CLEANER_CONDUCTOR { get; set; }
    [XmlElement(ElementName = "NUM_LL1")]
    public string NUM_LL1 { get; set; }
    [XmlElement(ElementName = "NUM_LL2")]
    public string NUM_LL2 { get; set; }
    [XmlElement(ElementName = "NUM_LL3")]
    public string NUM_LL3 { get; set; }
    [XmlElement(ElementName = "NUM_MONTH_OF_MANUFACTURE")]
    public string NUM_MONTH_OF_MANUFACTURE { get; set; }
    [XmlElement(ElementName = "NUM_NO_OF_NAMED_DRIVERS")]
    public string NUM_NO_OF_NAMED_DRIVERS { get; set; }
    [XmlElement(ElementName = "NUM_PAID_UP_CAPITAL")]
    public string NUM_PAID_UP_CAPITAL { get; set; }
    [XmlElement(ElementName = "NUM_PA_NAME1_AMOUNT")]
    public string NUM_PA_NAME1_AMOUNT { get; set; }
    [XmlElement(ElementName = "NUM_PA_NAME2_AMOUNT")]
    public string NUM_PA_NAME2_AMOUNT { get; set; }
    [XmlElement(ElementName = "NUM_PA_NAMED_AMOUNT")]
    public string NUM_PA_NAMED_AMOUNT { get; set; }
    [XmlElement(ElementName = "NUM_PA_NAMED_NUMBER")]
    public string NUM_PA_NAMED_NUMBER { get; set; }
    [XmlElement(ElementName = "NUM_PA_UNNAMED_NUMBER")]
    public string NUM_PA_UNNAMED_NUMBER { get; set; }
    [XmlElement(ElementName = "NUM_PA_UNNAMED_AMOUNT")]
    public string NUM_PA_UNNAMED_AMOUNT { get; set; }
    [XmlElement(ElementName = "NUM_PIN_CODE")]
    public string NUM_PIN_CODE { get; set; }
    [XmlElement(ElementName = "NUM_POLICY_NUMBER")]
    public string NUM_POLICY_NUMBER { get; set; }
    [XmlElement(ElementName = "NUM_POLICY_TYPE")]
    public string NUM_POLICY_TYPE { get; set; }
    [XmlElement(ElementName = "NUM_PREVIOUS_IDV")]
    public string NUM_PREVIOUS_IDV { get; set; }
    [XmlElement(ElementName = "NUM_RGSTRD_SEATING_CAPACITY")]
    public string NUM_RGSTRD_SEATING_CAPACITY { get; set; }
    [XmlElement(ElementName = "NUM_SPECIAL_DISCOUNT_RATE")]
    public string NUM_SPECIAL_DISCOUNT_RATE { get; set; }
    [XmlElement(ElementName = "NUM_TPPD_AMOUNT")]
    public string NUM_TPPD_AMOUNT { get; set; }
    [XmlElement(ElementName = "NUM_UTR_PAYMENT_AMOUNT")]
    public string NUM_UTR_PAYMENT_AMOUNT { get; set; }
    [XmlElement(ElementName = "NUM_VOLUNTARY_EXCESS_AMOUNT")]
    public string NUM_VOLUNTARY_EXCESS_AMOUNT { get; set; }
    [XmlElement(ElementName = "NUM_YEAR_OF_MANUFACTURE")]
    public string NUM_YEAR_OF_MANUFACTURE { get; set; }
    [XmlElement(ElementName = "ODDiscount")]
    public string ODDiscount { get; set; }
    [XmlElement(ElementName = "PAODPremium")]
    public string PAODPremium { get; set; }
    [XmlElement(ElementName = "TXT_AA_DISC_PREM")]
    public string TXT_AA_DISC_PREM { get; set; }
    [XmlElement(ElementName = "TXT_AA_FLAG")]
    public string TXT_AA_FLAG { get; set; }
    [XmlElement(ElementName = "TXT_AA_MEMBERSHIP_NAME")]
    public string TXT_AA_MEMBERSHIP_NAME { get; set; }
    [XmlElement(ElementName = "TXT_AA_MEMBERSHIP_NUMBER")]
    public string TXT_AA_MEMBERSHIP_NUMBER { get; set; }
    [XmlElement(ElementName = "TXT_BANK_CODE")]
    public string TXT_BANK_CODE { get; set; }
    [XmlElement(ElementName = "TXT_BANK_NAME")]
    public string TXT_BANK_NAME { get; set; }
    [XmlElement(ElementName = "TXT_CHASSIS_NUMBER")]
    public string TXT_CHASSIS_NUMBER { get; set; }
    [XmlElement(ElementName = "TXT_DOB")]
    public string TXT_DOB { get; set; }
    [XmlElement(ElementName = "TXT_DRIVING_LICENSE_NO")]
    public string TXT_DRIVING_LICENSE_NO { get; set; }
    [XmlElement(ElementName = "TXT_ELEC_DESC")]
    public string TXT_ELEC_DESC { get; set; }
    [XmlElement(ElementName = "TXT_EMAIL_ADDRESS")]
    public string TXT_EMAIL_ADDRESS { get; set; }
    [XmlElement(ElementName = "TXT_ENGINE_NUMBER")]
    public string TXT_ENGINE_NUMBER { get; set; }
    [XmlElement(ElementName = "TXT_FINANCIER_BRANCH_ADDRESS1")]
    public string TXT_FINANCIER_BRANCH_ADDRESS1 { get; set; }
    [XmlElement(ElementName = "TXT_FINANCIER_BRANCH_ADDRESS2")]
    public string TXT_FINANCIER_BRANCH_ADDRESS2 { get; set; }
    [XmlElement(ElementName = "TXT_FIN_ACCOUNT_CODE_1")]
    public string TXT_FIN_ACCOUNT_CODE_1 { get; set; }
    [XmlElement(ElementName = "TXT_FIN_ACCOUNT_CODE_2")]
    public string TXT_FIN_ACCOUNT_CODE_2 { get; set; }
    [XmlElement(ElementName = "TXT_FIN_BRANCH_NAME_1")]
    public string TXT_FIN_BRANCH_NAME_1 { get; set; }
    [XmlElement(ElementName = "TXT_FIN_BRANCH_NAME_2")]
    public string TXT_FIN_BRANCH_NAME_2 { get; set; }
    [XmlElement(ElementName = "TXT_FUEL")]
    public string TXT_FUEL { get; set; }
    [XmlElement(ElementName = "TXT_GENDER")]
    public string TXT_GENDER { get; set; }
    [XmlElement(ElementName = "TXT_GEOG_AREA_EXTN_COUNTRY")]
    public string TXT_GEOG_AREA_EXTN_COUNTRY { get; set; }
    [XmlElement(ElementName = "TXT_GSTIN_NUMBER")]
    public string TXT_GSTIN_NUMBER { get; set; }
    [XmlElement(ElementName = "TXT_LICENSE_ISSUING_AUTHORITY")]
    public string TXT_LICENSE_ISSUING_AUTHORITY { get; set; }
    [XmlElement(ElementName = "TXT_MEDICLE_COVER_LIMIT")]
    public string TXT_MEDICLE_COVER_LIMIT { get; set; }
    [XmlElement(ElementName = "TXT_MEMBERSHIP_CODE")]
    public string TXT_MEMBERSHIP_CODE { get; set; }
    [XmlElement(ElementName = "TXT_MERCHANT_ID")]
    public string TXT_MERCHANT_ID { get; set; }
    [XmlElement(ElementName = "TXT_MOBILE")]
    public string TXT_MOBILE { get; set; }
    [XmlElement(ElementName = "TXT_NAMED_PA_NOMINEE1")]
    public string TXT_NAMED_PA_NOMINEE1 { get; set; }
    [XmlElement(ElementName = "TXT_NAMED_PA_NOMINEE2")]
    public string TXT_NAMED_PA_NOMINEE2 { get; set; }
    [XmlElement(ElementName = "TXT_NAME_OF_INSURED")]
    public string TXT_NAME_OF_INSURED { get; set; }
    [XmlElement(ElementName = "TXT_NAME_OF_MANUFACTURER")]
    public string TXT_NAME_OF_MANUFACTURER { get; set; }
    [XmlElement(ElementName = "TXT_NAME_OF_NOMINEE")]
    public string TXT_NAME_OF_NOMINEE { get; set; }
    [XmlElement(ElementName = "TXT_NON_ELEC_DESC")]
    public string TXT_NON_ELEC_DESC { get; set; }
    [XmlElement(ElementName = "TXT_OCCUPATION")]
    public string TXT_OCCUPATION { get; set; }
    [XmlElement(ElementName = "TXT_OEM_DEALER_CODE")]
    public string TXT_OEM_DEALER_CODE { get; set; }
    [XmlElement(ElementName = "TXT_OEM_TRANSACTION_ID")]
    public string TXT_OEM_TRANSACTION_ID { get; set; }
    [XmlElement(ElementName = "TXT_OTHER_MAKE")]
    public string TXT_OTHER_MAKE { get; set; }
    [XmlElement(ElementName = "TXT_PAN_NO")]
    public string TXT_PAN_NO { get; set; }
    [XmlElement(ElementName = "TXT_PA_NAME1")]
    public string TXT_PA_NAME1 { get; set; }
    [XmlElement(ElementName = "TXT_PA_NAME2")]
    public string TXT_PA_NAME2 { get; set; }
    [XmlElement(ElementName = "TXT_PREVIOUS_INSURER")]
    public string TXT_PREVIOUS_INSURER { get; set; }
    [XmlElement(ElementName = "TXT_PREV_INSURER_CODE")]
    public string TXT_PREV_INSURER_CODE { get; set; }
    [XmlElement(ElementName = "TXT_REGISTRATION_NUMBER_1")]
    public string TXT_REGISTRATION_NUMBER_1 { get; set; }
    [XmlElement(ElementName = "TXT_REGISTRATION_NUMBER_2")]
    public string TXT_REGISTRATION_NUMBER_2 { get; set; }
    [XmlElement(ElementName = "TXT_REGISTRATION_NUMBER_3")]
    public string TXT_REGISTRATION_NUMBER_3 { get; set; }
    [XmlElement(ElementName = "TXT_REGISTRATION_NUMBER_4")]
    public string TXT_REGISTRATION_NUMBER_4 { get; set; }
    [XmlElement(ElementName = "TXT_RELATION_WITH_NOMINEE")]
    public string TXT_RELATION_WITH_NOMINEE { get; set; }
    [XmlElement(ElementName = "TXT_RTA_DESC")]
    public string TXT_RTA_DESC { get; set; }
    [XmlElement(ElementName = "TXT_TELEPHONE")]
    public string TXT_TELEPHONE { get; set; }
    [XmlElement(ElementName = "TXT_TITLE")]
    public string TXT_TITLE { get; set; }
    [XmlElement(ElementName = "TXT_TRANSACTION_ID")]
    public string TXT_TRANSACTION_ID { get; set; }
    [XmlElement(ElementName = "TXT_TYPE_BODY")]
    public string TXT_TYPE_BODY { get; set; }
    [XmlElement(ElementName = "TXT_UTR_NUMBER")]
    public string TXT_UTR_NUMBER { get; set; }
    [XmlElement(ElementName = "TXT_VAHICLE_COLOR")]
    public string TXT_VAHICLE_COLOR { get; set; }
    [XmlElement(ElementName = "TXT_VARIANT")]
    public string TXT_VARIANT { get; set; }
    [XmlElement(ElementName = "TXT_VEHICLE_ZONE")]
    public string TXT_VEHICLE_ZONE { get; set; }
    [XmlElement(ElementName = "YN_ANTI_THEFT")]
    public string YN_ANTI_THEFT { get; set; }
    [XmlElement(ElementName = "YN_CLAIM")]
    public string YN_CLAIM { get; set; }
    [XmlElement(ElementName = "YN_COMMERCIAL_FOR_PRIVATE")]
    public string YN_COMMERCIAL_FOR_PRIVATE { get; set; }
    [XmlElement(ElementName = "TXT_CPA_COVER_PERIOD")]
    public string TXT_CPA_COVER_PERIOD { get; set; }
    [XmlElement(ElementName = "YN_VALID_DRIVING_LICENSE")]
    public string YN_VALID_DRIVING_LICENSE { get; set; }
    [XmlElement(ElementName = "YN_COMPULSORY_PA_DTLS")]
    public string YN_COMPULSORY_PA_DTLS { get; set; }
    [XmlElement(ElementName = "YN_PAID_DRIVER")]
    public string YN_PAID_DRIVER { get; set; }
    [XmlElement(ElementName = "YN_COURTESY_CAR")]
    public string YN_COURTESY_CAR { get; set; }
    [XmlElement(ElementName = "YN_DELETION_OF_IMT26")]
    public string YN_DELETION_OF_IMT26 { get; set; }
    [XmlElement(ElementName = "YN_DRIVING_TUTION")]
    public string YN_DRIVING_TUTION { get; set; }
    [XmlElement(ElementName = "YN_FOREIGN_EMBASSY")]
    public string YN_FOREIGN_EMBASSY { get; set; }
    [XmlElement(ElementName = "YN_HANDICAPPED")]
    public string YN_HANDICAPPED { get; set; }
    [XmlElement(ElementName = "YN_IMT32")]
    public string YN_IMT32 { get; set; }
    [XmlElement(ElementName = "YN_INBUILT_CNG")]
    public string YN_INBUILT_CNG { get; set; }
    [XmlElement(ElementName = "YN_INBUILT_LPG")]
    public string YN_INBUILT_LPG { get; set; }
    [XmlElement(ElementName = "YN_LIMITED_TO_OWN_PREMISES")]
    public string YN_LIMITED_TO_OWN_PREMISES { get; set; }
    [XmlElement(ElementName = "YN_MEDICLE_EXPENSE")]
    public string YN_MEDICLE_EXPENSE { get; set; }
    [XmlElement(ElementName = "YN_NIL_DEPR_WITHOUT_EXCESS")]
    public string YN_NIL_DEPR_WITHOUT_EXCESS { get; set; }
    [XmlElement(ElementName = "YN_PERSONAL_EFFECT")]
    public string YN_PERSONAL_EFFECT { get; set; }
    [XmlElement(ElementName = "YN_RTI_APPLICABLE")]
    public string YN_RTI_APPLICABLE { get; set; }
    [XmlElement(ElementName = "YN_ENGINE_GEAR_COVER_PLATINUM")]
    public string YN_ENGINE_GEAR_COVER_PLATINUM { get; set; }
    [XmlElement(ElementName = "YN_TYRE_RIM_PROTECTOR")]
    public string YN_TYRE_RIM_PROTECTOR { get; set; }
    [XmlElement(ElementName = "NUM_TYRE_RIM_SUM_INSURED")]
    public string NUM_TYRE_RIM_SUM_INSURED { get; set; }
    [XmlElement(ElementName = "YN_ENGINE_GEAR_COVER")]
    public string YN_ENGINE_GEAR_COVER { get; set; }
    [XmlElement(ElementName = "YN_LOSS_OF_KEY")]
    public string YN_LOSS_OF_KEY { get; set; }
    [XmlElement(ElementName = "NUM_LOSS_OF_KEY_SUM_INSURED")]
    public string NUM_LOSS_OF_KEY_SUM_INSURED { get; set; }
    [XmlElement(ElementName = "YN_CONSUMABLE")]
    public string YN_CONSUMABLE { get; set; }
    [XmlElement(ElementName = "NUM_VEHICLE_MODEL_CODE")]
    public string NUM_VEHICLE_MODEL_CODE { get; set; }
    [XmlElement(ElementName = "TXT_TP_POLICY_NUMBER")]
    public string TXT_TP_POLICY_NUMBER { get; set; }
    [XmlElement(ElementName = "TXT_TP_POLICY_INSURER")]
    public string TXT_TP_POLICY_INSURER { get; set; }
    [XmlElement(ElementName = "TXT_TP_POLICY_START_DATE")]
    public string TXT_TP_POLICY_START_DATE { get; set; }
    [XmlElement(ElementName = "TXT_TP_POLICY_END_DATE")]
    public string TXT_TP_POLICY_END_DATE { get; set; }
    [XmlElement(ElementName = "TXT_TP_POLICY_INSURER_ADDRESS")]
    public string TXT_TP_POLICY_INSURER_ADDRESS { get; set; }
    [XmlElement(ElementName = "YN_RSA_COVER")]
    public string YN_RSA_COVER { get; set; }
    [XmlElement(ElementName = "YN_NCB_PROTECT")]
    public string YN_NCB_PROTECT { get; set; }
}

[XmlRoot(ElementName = "ROOT")]
public class UnitedIndiaRoot
{
    [XmlElement(ElementName = "HEADER")]
    public UnitedIndiaHeader HEADER { get; set; }
}

#region For Request Body Deserialization Purpose
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
public partial class calculatePremiumRequest
{
    public string application { get; set; }
    public string userid { get; set; }
    public string password { get; set; }
    public calculatePremiumProposalXml proposalXml { get; set; }
    public ushort productCode { get; set; }
    public byte subproductCode { get; set; }
}

[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class calculatePremiumProposalXml
{
    public UnitedIndiaRoot ROOT { get; set; }
}

[XmlRoot(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
public class UIICRequestBody
{
    [XmlElement(ElementName = "calculatePremium", Namespace = "http://ws.uiic.com/")]
    public calculatePremiumRequest CalculatePremium { get; set; }
}

[XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
public class UIICRequestEnvelopeRequest
{
    [XmlElement(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public UIICRequestBody Body { get; set; }
    [XmlAttribute(AttributeName = "ws", Namespace = "http://www.w3.org/2000/xmlns/")]
    public string Ws { get; set; }
    [XmlAttribute(AttributeName = "soapenv", Namespace = "http://www.w3.org/2000/xmlns/")]
    public string Soapenv { get; set; }
}
#endregion