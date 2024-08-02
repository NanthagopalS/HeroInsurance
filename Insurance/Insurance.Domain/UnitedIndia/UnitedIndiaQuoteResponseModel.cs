using System.Xml.Serialization;

namespace Insurance.Domain.UnitedIndia;

[XmlRoot(ElementName = "PropLoadingDiscount_Col")]
public class PropLoadingDiscount_Col
{
    [XmlElement(ElementName = "LoadingDiscount")]
    public List<LoadingDiscount> LoadingDiscount { get; set; }

}

[XmlRoot(ElementName = "LoadingDiscount")]
public class LoadingDiscount
{
    [XmlElement(ElementName = "ExtensionData")]
    public string ExtensionData { get; set; }
    [XmlElement(ElementName = "PropLoadingDiscount_Applicable")]
    public string PropLoadingDiscount_Applicable { get; set; }
    [XmlElement(ElementName = "PropLoadingDiscount_CalculatedAmount")]
    public string PropLoadingDiscount_CalculatedAmount { get; set; }
    [XmlElement(ElementName = "PropLoadingDiscount_Description")]
    public string PropLoadingDiscount_Description { get; set; }
    [XmlElement(ElementName = "PropLoadingDiscount_DifferentialSI")]
    public string PropLoadingDiscount_DifferentialSI { get; set; }
    [XmlElement(ElementName = "PropLoadingDiscount_EndorsementAmount")]
    public string PropLoadingDiscount_EndorsementAmount { get; set; }
    [XmlElement(ElementName = "PropLoadingDiscount_FixedAmount")]
    public string PropLoadingDiscount_FixedAmount { get; set; }
    [XmlElement(ElementName = "PropLoadingDiscount_IsDataDeleted")]
    public string PropLoadingDiscount_IsDataDeleted { get; set; }
    [XmlElement(ElementName = "PropLoadingDiscount_IsOldDataDeleted")]
    public string PropLoadingDiscount_IsOldDataDeleted { get; set; }
    [XmlElement(ElementName = "PropLoadingDiscount_MaxCap")]
    public string PropLoadingDiscount_MaxCap { get; set; }
    [XmlElement(ElementName = "PropLoadingDiscount_MinCap")]
    public string PropLoadingDiscount_MinCap { get; set; }
    [XmlElement(ElementName = "PropLoadingDiscount_Premium")]
    public string PropLoadingDiscount_Premium { get; set; }
    [XmlElement(ElementName = "PropLoadingDiscount_Rate")]
    public string PropLoadingDiscount_Rate { get; set; }
    [XmlElement(ElementName = "PropLoadingDiscount_SumInsured")]
    public string PropLoadingDiscount_SumInsured { get; set; }
}

[XmlRoot(ElementName = "CoverDetails_LoadingDiscount")]
public class CoverDetails_LoadingDiscount
{
    [XmlElement(ElementName = "ExtensionData")]
    public string ExtensionData { get; set; }
    [XmlElement(ElementName = "PropLoadingDiscount_Applicable")]
    public string PropLoadingDiscount_Applicable { get; set; }
    [XmlElement(ElementName = "PropLoadingDiscount_CalculatedAmount")]
    public string PropLoadingDiscount_CalculatedAmount { get; set; }
    [XmlElement(ElementName = "PropLoadingDiscount_Description")]
    public string PropLoadingDiscount_Description { get; set; }
    [XmlElement(ElementName = "PropLoadingDiscount_DifferentialSI")]
    public string PropLoadingDiscount_DifferentialSI { get; set; }
    [XmlElement(ElementName = "PropLoadingDiscount_EndorsementAmount")]
    public string PropLoadingDiscount_EndorsementAmount { get; set; }
    [XmlElement(ElementName = "PropLoadingDiscount_FixedAmount")]
    public string PropLoadingDiscount_FixedAmount { get; set; }
    [XmlElement(ElementName = "PropLoadingDiscount_IsDataDeleted")]
    public string PropLoadingDiscount_IsDataDeleted { get; set; }
    [XmlElement(ElementName = "PropLoadingDiscount_IsOldDataDeleted")]
    public string PropLoadingDiscount_IsOldDataDeleted { get; set; }
    [XmlElement(ElementName = "PropLoadingDiscount_Premium")]
    public string PropLoadingDiscount_Premium { get; set; }
    [XmlElement(ElementName = "PropLoadingDiscount_Rate")]
    public string PropLoadingDiscount_Rate { get; set; }
    [XmlElement(ElementName = "PropLoadingDiscount_SumInsured")]
    public string PropLoadingDiscount_SumInsured { get; set; }
}

[XmlRoot(ElementName = "PropCoverDetails_LoadingDiscount_Col")]
public class PropCoverDetails_LoadingDiscount_Col
{
    [XmlElement(ElementName = "CoverDetails_LoadingDiscount")]
    public List<CoverDetails_LoadingDiscount> CoverDetails_LoadingDiscount { get; set; }
}

[XmlRoot(ElementName = "Risks_CoverDetails")]
public class Risks_CoverDetails
{
    [XmlElement(ElementName = "ExtensionData")]
    public string ExtensionData { get; set; }
    [XmlElement(ElementName = "PropCoverDetails_Applicable")]
    public string PropCoverDetails_Applicable { get; set; }
    [XmlElement(ElementName = "PropCoverDetails_CoverGroups")]
    public string PropCoverDetails_CoverGroups { get; set; }
    [XmlElement(ElementName = "PropCoverDetails_DifferentialSI")]
    public string PropCoverDetails_DifferentialSI { get; set; }
    [XmlElement(ElementName = "PropCoverDetails_EndorsementAmount")]
    public string PropCoverDetails_EndorsementAmount { get; set; }
    [XmlElement(ElementName = "PropCoverDetails_IsDataDeleted")]
    public string PropCoverDetails_IsDataDeleted { get; set; }
    [XmlElement(ElementName = "PropCoverDetails_IsOldDataDeleted")]
    public string PropCoverDetails_IsOldDataDeleted { get; set; }
    //[XmlElement(ElementName = "PropCoverDetails_LoadingDiscount_Col")]
    //public string PropCoverDetails_LoadingDiscount_Col { get; set; }
    [XmlElement(ElementName = "PropCoverDetails_LoadingDiscount_Col")]
    public PropCoverDetails_LoadingDiscount_Col PropCoverDetails_LoadingDiscount_Col { get; set; }
    [XmlElement(ElementName = "PropCoverDetails_Premium")]
    public string PropCoverDetails_Premium { get; set; }
    [XmlElement(ElementName = "PropCoverDetails_Rate")]
    public string PropCoverDetails_Rate { get; set; }
    [XmlElement(ElementName = "PropCoverDetails_SumInsured")]
    public string PropCoverDetails_SumInsured { get; set; }
}

[XmlRoot(ElementName = "PropRisks_CoverDetails_Col")]
public class PropRisks_CoverDetails_Col
{
    [XmlElement(ElementName = "Risks_CoverDetails")]
    public List<Risks_CoverDetails> Risks_CoverDetails { get; set; }
}

[XmlRoot(ElementName = "GeneralProposal_PreviousPolicyDetails")]
public class GeneralProposal_PreviousPolicyDetails
{
    [XmlElement(ElementName = "ExtensionData")]
    public string ExtensionData { get; set; }
    [XmlElement(ElementName = "PropPreviousPolicyDetails_ClaimAmount")]
    public string PropPreviousPolicyDetails_ClaimAmount { get; set; }
    [XmlElement(ElementName = "PropPreviousPolicyDetails_ClaimNo")]
    public string PropPreviousPolicyDetails_ClaimNo { get; set; }
    [XmlElement(ElementName = "PropPreviousPolicyDetails_ClaimPremium")]
    public string PropPreviousPolicyDetails_ClaimPremium { get; set; }
    [XmlElement(ElementName = "PropPreviousPolicyDetails_ClaimSettled")]
    public string PropPreviousPolicyDetails_ClaimSettled { get; set; }
    [XmlElement(ElementName = "PropPreviousPolicyDetails_ClaimsMode")]
    public string PropPreviousPolicyDetails_ClaimsMode { get; set; }
    [XmlElement(ElementName = "PropPreviousPolicyDetails_CorporateCustomerId_Mandatary")]
    public string PropPreviousPolicyDetails_CorporateCustomerId_Mandatary { get; set; }
    [XmlElement(ElementName = "PropPreviousPolicyDetails_DateofLoss")]
    public string PropPreviousPolicyDetails_DateofLoss { get; set; }
    [XmlElement(ElementName = "PropPreviousPolicyDetails_DateofSale")]
    public string PropPreviousPolicyDetails_DateofSale { get; set; }
    [XmlElement(ElementName = "PropPreviousPolicyDetails_DocumentProof")]
    public string PropPreviousPolicyDetails_DocumentProof { get; set; }
    [XmlElement(ElementName = "PropPreviousPolicyDetails_IncurredClaimRatio")]
    public string PropPreviousPolicyDetails_IncurredClaimRatio { get; set; }
    [XmlElement(ElementName = "PropPreviousPolicyDetails_InspectionDate")]
    public string PropPreviousPolicyDetails_InspectionDate { get; set; }
    [XmlElement(ElementName = "PropPreviousPolicyDetails_InspectionDone")]
    public string PropPreviousPolicyDetails_InspectionDone { get; set; }
    [XmlElement(ElementName = "PropPreviousPolicyDetails_InspectionDoneByWhom")]
    public string PropPreviousPolicyDetails_InspectionDoneByWhom { get; set; }
    [XmlElement(ElementName = "PropPreviousPolicyDetails_IsDataDeleted")]
    public string PropPreviousPolicyDetails_IsDataDeleted { get; set; }
    [XmlElement(ElementName = "PropPreviousPolicyDetails_IsOldDataDeleted")]
    public string PropPreviousPolicyDetails_IsOldDataDeleted { get; set; }
    [XmlElement(ElementName = "PropPreviousPolicyDetails_NCBAbroadCheck")]
    public string PropPreviousPolicyDetails_NCBAbroadCheck { get; set; }
    [XmlElement(ElementName = "PropPreviousPolicyDetails_NatureofLoss")]
    public string PropPreviousPolicyDetails_NatureofLoss { get; set; }
    [XmlElement(ElementName = "PropPreviousPolicyDetails_NoOfClaims")]
    public string PropPreviousPolicyDetails_NoOfClaims { get; set; }
    [XmlElement(ElementName = "PropPreviousPolicyDetails_OfficeAddress")]
    public string PropPreviousPolicyDetails_OfficeAddress { get; set; }
    [XmlElement(ElementName = "PropPreviousPolicyDetails_OfficeCode")]
    public string PropPreviousPolicyDetails_OfficeCode { get; set; }
    [XmlElement(ElementName = "PropPreviousPolicyDetails_PolicyEffectiveFrom")]
    public string PropPreviousPolicyDetails_PolicyEffectiveFrom { get; set; }
    [XmlElement(ElementName = "PropPreviousPolicyDetails_PolicyEffectiveTo")]
    public string PropPreviousPolicyDetails_PolicyEffectiveTo { get; set; }
    [XmlElement(ElementName = "PropPreviousPolicyDetails_PolicyNo")]
    public string PropPreviousPolicyDetails_PolicyNo { get; set; }
    [XmlElement(ElementName = "PropPreviousPolicyDetails_PolicyPremium")]
    public string PropPreviousPolicyDetails_PolicyPremium { get; set; }
    [XmlElement(ElementName = "PropPreviousPolicyDetails_PolicyYear_Mandatary")]
    public string PropPreviousPolicyDetails_PolicyYear_Mandatary { get; set; }
    [XmlElement(ElementName = "PropPreviousPolicyDetails_ProductCode")]
    public string PropPreviousPolicyDetails_ProductCode { get; set; }
    [XmlElement(ElementName = "PropPreviousPolicyDetails_ReferenceNo")]
    public string PropPreviousPolicyDetails_ReferenceNo { get; set; }
    [XmlElement(ElementName = "PropPreviousPolicyDetails_VehicleSold")]
    public string PropPreviousPolicyDetails_VehicleSold { get; set; }
}

[XmlRoot(ElementName = "PropGeneralProposal_PreviousPolicyDetails_Col")]
public class PropGeneralProposal_PreviousPolicyDetails_Col
{
    [XmlElement(ElementName = "GeneralProposal_PreviousPolicyDetails")]
    public GeneralProposal_PreviousPolicyDetails GeneralProposal_PreviousPolicyDetails { get; set; }
}

[XmlRoot(ElementName = "Risks")]
public class Risks
{
    [XmlElement(ElementName = "ExtensionData")]
    public string ExtensionData { get; set; }
    [XmlElement(ElementName = "IsOptionalCover")]
    public string IsOptionalCover { get; set; }
    [XmlElement(ElementName = "PropRisks_CoverDetails_Col")]
    public PropRisks_CoverDetails_Col PropRisks_CoverDetails_Col { get; set; }
    [XmlElement(ElementName = "PropRisks_DifferentialSI")]
    public string PropRisks_DifferentialSI { get; set; }
    [XmlElement(ElementName = "PropRisks_EndorsementAmount")]
    public string PropRisks_EndorsementAmount { get; set; }
    [XmlElement(ElementName = "PropRisks_IsDataDeleted")]
    public string PropRisks_IsDataDeleted { get; set; }
    [XmlElement(ElementName = "PropRisks_IsOldDataDeleted")]
    public string PropRisks_IsOldDataDeleted { get; set; }
    [XmlElement(ElementName = "PropRisks_Premium")]
    public string PropRisks_Premium { get; set; }
    [XmlElement(ElementName = "PropRisks_Rate")]
    public string PropRisks_Rate { get; set; }
    [XmlElement(ElementName = "PropRisks_SumInsured")]
    public string PropRisks_SumInsured { get; set; }
    [XmlElement(ElementName = "PropRisks_VehicleSIComponent")]
    public string PropRisks_VehicleSIComponent { get; set; }
}

[XmlRoot(ElementName = "PropRisks_Col")]
public class PropRisks_Col
{
    [XmlElement(ElementName = "Risks")]
    public List<Risks> Risks { get; set; }
}

[XmlRoot(ElementName = "WorkSheet")]
public class WorkSheet
{
    [XmlElement(ElementName = "PropReferenceNoDate_ReferenceDate_Mandatary")]
    public string PropReferenceNoDate_ReferenceDate_Mandatary { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_ProposalDate_Mandatary")]
    public string PropGeneralProposalInformation_ProposalDate_Mandatary { get; set; }
    [XmlElement(ElementName = "PropIntermediaryDetails_IntermediaryCode")]
    public string PropIntermediaryDetails_IntermediaryCode { get; set; }
    [XmlElement(ElementName = "PropIntermediaryDetails_IntermediaryName")]
    public string PropIntermediaryDetails_IntermediaryName { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_ChannelNumber")]
    public string PropGeneralProposalInformation_ChannelNumber { get; set; }
    [XmlElement(ElementName = "PropDistributionChannel_BusineeChanneltype")]
    public string PropDistributionChannel_BusineeChanneltype { get; set; }
    [XmlElement(ElementName = "PropDistributionChannel_BusinessSource_Mandatary")]
    public string PropDistributionChannel_BusinessSource_Mandatary { get; set; }
    [XmlElement(ElementName = "PropDistributionChannel_BusinessSourcetype")]
    public string PropDistributionChannel_BusinessSourcetype { get; set; }
    [XmlElement(ElementName = "ExtensionData")]
    public string ExtensionData { get; set; }
    [XmlElement(ElementName = "ErrorTrackingNeeded")]
    public string ErrorTrackingNeeded { get; set; }
    [XmlElement(ElementName = "PropBranchDetails_IMDBranchCode")]
    public string PropBranchDetails_IMDBranchCode { get; set; }
    [XmlElement(ElementName = "PropBranchDetails_IMDBranchName")]
    public string PropBranchDetails_IMDBranchName { get; set; }
    [XmlElement(ElementName = "PropCalculation_CalculateRate")]
    public string PropCalculation_CalculateRate { get; set; }
    [XmlElement(ElementName = "PropCalculation_Validate")]
    public string PropCalculation_Validate { get; set; }
    [XmlElement(ElementName = "PropCalculation_ValidateData")]
    public string PropCalculation_ValidateData { get; set; }
    [XmlElement(ElementName = "PropClauseDetails_Component51")]
    public string PropClauseDetails_Component51 { get; set; }
    [XmlElement(ElementName = "PropClauseDetails_DepartmentCode_Mandatary")]
    public string PropClauseDetails_DepartmentCode_Mandatary { get; set; }
    [XmlElement(ElementName = "PropClauseDetails_ProductCode_Mandatary")]
    public string PropClauseDetails_ProductCode_Mandatary { get; set; }
    [XmlElement(ElementName = "PropClauseDetails_SectionCode")]
    public string PropClauseDetails_SectionCode { get; set; }
    [XmlElement(ElementName = "PropCoinsuranceDetails_AddRow")]
    public string PropCoinsuranceDetails_AddRow { get; set; }
    [XmlElement(ElementName = "PropCoinsuranceDetails_CoinsuranceType")]
    public string PropCoinsuranceDetails_CoinsuranceType { get; set; }
    [XmlElement(ElementName = "PropCoinsuranceDetails_Commissiontobepaidbytheleader")]
    public string PropCoinsuranceDetails_Commissiontobepaidbytheleader { get; set; }
    [XmlElement(ElementName = "PropCoinsuranceDetails_PolicyNooftheleader")]
    public string PropCoinsuranceDetails_PolicyNooftheleader { get; set; }
    [XmlElement(ElementName = "PropCoinsuranceDetails_ReferenceNumber")]
    public string PropCoinsuranceDetails_ReferenceNumber { get; set; }
    [XmlElement(ElementName = "PropCoinsuranceDetails_Servicetaxtobepaidbytheleader")]
    public string PropCoinsuranceDetails_Servicetaxtobepaidbytheleader { get; set; }
    [XmlElement(ElementName = "PropCoinsuranceDetails_Validate")]
    public string PropCoinsuranceDetails_Validate { get; set; }
    [XmlElement(ElementName = "PropConditionDetails_ConditionDescription")]
    public string PropConditionDetails_ConditionDescription { get; set; }
    [XmlElement(ElementName = "PropConditionDetails_DepartmentCode")]
    public string PropConditionDetails_DepartmentCode { get; set; }
    [XmlElement(ElementName = "PropConditionDetails_ProductCode")]
    public string PropConditionDetails_ProductCode { get; set; }
    [XmlElement(ElementName = "PropConditionDetails_SectionCode")]
    public string PropConditionDetails_SectionCode { get; set; }
    [XmlElement(ElementName = "PropConditionDetails_SpecialConditionDescription")]
    public string PropConditionDetails_SpecialConditionDescription { get; set; }
    [XmlElement(ElementName = "PropCoverNoteDetails_CoverNoteBookNo")]
    public string PropCoverNoteDetails_CoverNoteBookNo { get; set; }
    [XmlElement(ElementName = "PropCoverNoteDetails_CoverNoteLeafNo")]
    public string PropCoverNoteDetails_CoverNoteLeafNo { get; set; }
    [XmlElement(ElementName = "PropCoverNoteDetails_IssuedOnDt")]
    public string PropCoverNoteDetails_IssuedOnDt { get; set; }
    [XmlElement(ElementName = "PropCoverNoteDetails_IsuedonTime")]
    public string PropCoverNoteDetails_IsuedonTime { get; set; }
    [XmlElement(ElementName = "PropCoverNoteDetails_LeafNo")]
    public string PropCoverNoteDetails_LeafNo { get; set; }
    [XmlElement(ElementName = "PropCoverNoteDetails_RecievedOn")]
    public string PropCoverNoteDetails_RecievedOn { get; set; }
    [XmlElement(ElementName = "PropCoverNoteDetails_Validate")]
    public string PropCoverNoteDetails_Validate { get; set; }
    [XmlElement(ElementName = "PropCustomerDtls_CustomerID_Mandatary")]
    public string PropCustomerDtls_CustomerID_Mandatary { get; set; }
    [XmlElement(ElementName = "PropCustomerDtls_CustomerName")]
    public string PropCustomerDtls_CustomerName { get; set; }
    [XmlElement(ElementName = "PropCustomerDtls_CustomerType")]
    public string PropCustomerDtls_CustomerType { get; set; }
    [XmlElement(ElementName = "PropCustomerDtls_PayeeCustomerID")]
    public string PropCustomerDtls_PayeeCustomerID { get; set; }
    [XmlElement(ElementName = "PropCustomerDtls_PayeeCustomerName")]
    public string PropCustomerDtls_PayeeCustomerName { get; set; }
    [XmlElement(ElementName = "PropCustomerReferenceInfo_OldPolicyNumber")]
    public string PropCustomerReferenceInfo_OldPolicyNumber { get; set; }
    [XmlElement(ElementName = "PropCustomerReferenceInfo_PolicyConversionDate")]
    public string PropCustomerReferenceInfo_PolicyConversionDate { get; set; }
    [XmlElement(ElementName = "PropDistributionChannel_BranchDetails")]
    public string PropDistributionChannel_BranchDetails { get; set; }
    [XmlElement(ElementName = "PropDistributionChannel_BusineeChannelBrunch")]
    public string PropDistributionChannel_BusineeChannelBrunch { get; set; }
    [XmlElement(ElementName = "PropDistributionChannel_BusinessServicingChannelType")]
    public string PropDistributionChannel_BusinessServicingChannelType { get; set; }
    [XmlElement(ElementName = "PropDistributionChannel_Commision")]
    public string PropDistributionChannel_Commision { get; set; }
    [XmlElement(ElementName = "PropDistributionChannel_EndorsementDtls")]
    public string PropDistributionChannel_EndorsementDtls { get; set; }
    [XmlElement(ElementName = "PropDistributionChannel_IntermediaryDetails")]
    public string PropDistributionChannel_IntermediaryDetails { get; set; }
    [XmlElement(ElementName = "PropDistributionChannel_MODetails")]
    public string PropDistributionChannel_MODetails { get; set; }
    [XmlElement(ElementName = "PropDistributionChannel_OperationMode")]
    public string PropDistributionChannel_OperationMode { get; set; }
    [XmlElement(ElementName = "PropDistributionChannel_SPDetails")]
    public string PropDistributionChannel_SPDetails { get; set; }
    [XmlElement(ElementName = "PropDistributionChannel_SerIntermediaryDetails")]
    public string PropDistributionChannel_SerIntermediaryDetails { get; set; }
    [XmlElement(ElementName = "PropDistributionChannel_ServiceProvider")]
    public string PropDistributionChannel_ServiceProvider { get; set; }
    [XmlElement(ElementName = "PropDistributionChannel_SpecialDiscount")]
    public string PropDistributionChannel_SpecialDiscount { get; set; }
    [XmlElement(ElementName = "PropDistributionChannel_Validate")]
    public string PropDistributionChannel_Validate { get; set; }
    [XmlElement(ElementName = "PropDistributionChannel_VerticalDtls")]
    public string PropDistributionChannel_VerticalDtls { get; set; }
    [XmlElement(ElementName = "PropEndorsementDtls_CancelDueToClaim")]
    public string PropEndorsementDtls_CancelDueToClaim { get; set; }
    [XmlElement(ElementName = "PropEndorsementDtls_CancellationOption")]
    public string PropEndorsementDtls_CancellationOption { get; set; }
    [XmlElement(ElementName = "PropEndorsementDtls_CancellationReason")]
    public string PropEndorsementDtls_CancellationReason { get; set; }
    [XmlElement(ElementName = "PropEndorsementDtls_CertificateofVintageCar")]
    public string PropEndorsementDtls_CertificateofVintageCar { get; set; }
    [XmlElement(ElementName = "PropEndorsementDtls_CoverType")]
    public string PropEndorsementDtls_CoverType { get; set; }
    [XmlElement(ElementName = "PropEndorsementDtls_DataEntryError")]
    public string PropEndorsementDtls_DataEntryError { get; set; }
    [XmlElement(ElementName = "PropEndorsementDtls_DateofEndfrominsured")]
    public string PropEndorsementDtls_DateofEndfrominsured { get; set; }
    [XmlElement(ElementName = "PropEndorsementDtls_Description")]
    public string PropEndorsementDtls_Description { get; set; }
    [XmlElement(ElementName = "PropEndorsementDtls_DocForCommPrivate")]
    public string PropEndorsementDtls_DocForCommPrivate { get; set; }
    [XmlElement(ElementName = "PropEndorsementDtls_DocOfRequisition")]
    public string PropEndorsementDtls_DocOfRequisition { get; set; }
    [XmlElement(ElementName = "PropEndorsementDtls_DocProofothersspecify")]
    public string PropEndorsementDtls_DocProofothersspecify { get; set; }
    [XmlElement(ElementName = "PropEndorsementDtls_DocproofforNCB")]
    public string PropEndorsementDtls_DocproofforNCB { get; set; }
    [XmlElement(ElementName = "PropEndorsementDtls_DoubleINSOption")]
    public string PropEndorsementDtls_DoubleINSOption { get; set; }
    [XmlElement(ElementName = "PropEndorsementDtls_EndorsementTypeCode")]
    public string PropEndorsementDtls_EndorsementTypeCode { get; set; }
    [XmlElement(ElementName = "PropEndorsementDtls_INSCoName")]
    public string PropEndorsementDtls_INSCoName { get; set; }
    [XmlElement(ElementName = "PropEndorsementDtls_INSCoOffCodeAdd")]
    public string PropEndorsementDtls_INSCoOffCodeAdd { get; set; }
    [XmlElement(ElementName = "PropEndorsementDtls_IsSellerInsured")]
    public string PropEndorsementDtls_IsSellerInsured { get; set; }
    [XmlElement(ElementName = "PropEndorsementDtls_LaidUpFromDate")]
    public string PropEndorsementDtls_LaidUpFromDate { get; set; }
    [XmlElement(ElementName = "PropEndorsementDtls_LaidUpToDate")]
    public string PropEndorsementDtls_LaidUpToDate { get; set; }
    [XmlElement(ElementName = "PropEndorsementDtls_LaidupVehicleCheck")]
    public string PropEndorsementDtls_LaidupVehicleCheck { get; set; }
    [XmlElement(ElementName = "PropEndorsementDtls_MajorRepairRenovation")]
    public string PropEndorsementDtls_MajorRepairRenovation { get; set; }
    [XmlElement(ElementName = "PropEndorsementDtls_NOCFinancier")]
    public string PropEndorsementDtls_NOCFinancier { get; set; }
    [XmlElement(ElementName = "PropEndorsementDtls_PolicyFrom")]
    public string PropEndorsementDtls_PolicyFrom { get; set; }
    [XmlElement(ElementName = "PropEndorsementDtls_PolicyIssueDate")]
    public string PropEndorsementDtls_PolicyIssueDate { get; set; }
    [XmlElement(ElementName = "PropEndorsementDtls_PolicyNo")]
    public string PropEndorsementDtls_PolicyNo { get; set; }
    [XmlElement(ElementName = "PropEndorsementDtls_PolicyTo")]
    public string PropEndorsementDtls_PolicyTo { get; set; }
    [XmlElement(ElementName = "PropEndorsementDtls_Reasonfornametransfer")]
    public string PropEndorsementDtls_Reasonfornametransfer { get; set; }
    [XmlElement(ElementName = "PropEndorsementDtls_TypeOfTransfer")]
    public string PropEndorsementDtls_TypeOfTransfer { get; set; }
    [XmlElement(ElementName = "PropEndorsementDtls_TypeofEndorsement")]
    public string PropEndorsementDtls_TypeofEndorsement { get; set; }
    [XmlElement(ElementName = "PropEndorsementDtls_UFormFromDate")]
    public string PropEndorsementDtls_UFormFromDate { get; set; }
    [XmlElement(ElementName = "PropEndorsementDtls_UFormToDate")]
    public string PropEndorsementDtls_UFormToDate { get; set; }
    [XmlElement(ElementName = "PropEndorsementDtls_UsageinInsuredPremises")]
    public string PropEndorsementDtls_UsageinInsuredPremises { get; set; }
    [XmlElement(ElementName = "PropEndorsementDtls_ValuationReport")]
    public string PropEndorsementDtls_ValuationReport { get; set; }
    [XmlElement(ElementName = "PropEndorsementDtls_VehicleLaidUptype")]
    public string PropEndorsementDtls_VehicleLaidUptype { get; set; }
    [XmlElement(ElementName = "PropEndorsementDtls_orgcertificatesurrendered")]
    public string PropEndorsementDtls_orgcertificatesurrendered { get; set; }
    [XmlElement(ElementName = "PropEndorsementEffectiveDate_EffectiveDate")]
    public string PropEndorsementEffectiveDate_EffectiveDate { get; set; }
    [XmlElement(ElementName = "PropEndorsementEffectiveDate_EffectiveTime")]
    public string PropEndorsementEffectiveDate_EffectiveTime { get; set; }
    [XmlElement(ElementName = "PropExclusionDetails_DepartmentCode")]
    public string PropExclusionDetails_DepartmentCode { get; set; }
    [XmlElement(ElementName = "PropExclusionDetails_ProductCode")]
    public string PropExclusionDetails_ProductCode { get; set; }
    [XmlElement(ElementName = "PropExclusionDetails_SectionCode")]
    public string PropExclusionDetails_SectionCode { get; set; }
    [XmlElement(ElementName = "PropFinancierDetails_AddRow")]
    public string PropFinancierDetails_AddRow { get; set; }
    [XmlElement(ElementName = "PropFinancierDetails_Validate")]
    public string PropFinancierDetails_Validate { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalGroup_CoverNoteDetails")]
    public string PropGeneralProposalGroup_CoverNoteDetails { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalGroup_DistributionChannel")]
    public string PropGeneralProposalGroup_DistributionChannel { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalGroup_GeneralProposalInformation")]
    public string PropGeneralProposalGroup_GeneralProposalInformation { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_ActiveFlag")]
    public string PropGeneralProposalInformation_ActiveFlag { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_ApplicationNumber")]
    public string PropGeneralProposalInformation_ApplicationNumber { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_BankID")]
    public string PropGeneralProposalInformation_BankID { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_BranchOfficeCode")]
    public string PropGeneralProposalInformation_BranchOfficeCode { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_BusinessSourceInfo")]
    public string PropGeneralProposalInformation_BusinessSourceInfo { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_BusinessType_Mandatary")]
    public string PropGeneralProposalInformation_BusinessType_Mandatary { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_CertificateNumber")]
    public string PropGeneralProposalInformation_CertificateNumber { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_CoverNotePlace")]
    public string PropGeneralProposalInformation_CoverNotePlace { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_CovernoteGenType")]
    public string PropGeneralProposalInformation_CovernoteGenType { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_CustomerDtls")]
    public string PropGeneralProposalInformation_CustomerDtls { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_CustomerReferenceInfo")]
    public string PropGeneralProposalInformation_CustomerReferenceInfo { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_DealId")]
    public string PropGeneralProposalInformation_DealId { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_DepartmentCode")]
    public string PropGeneralProposalInformation_DepartmentCode { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_DepartmentName")]
    public string PropGeneralProposalInformation_DepartmentName { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_DisplayOfficeCode")]
    public string PropGeneralProposalInformation_DisplayOfficeCode { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_EmployeeName")]
    public string PropGeneralProposalInformation_EmployeeName { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_EndorsementDescription")]
    public string PropGeneralProposalInformation_EndorsementDescription { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_EndorsementEffectiveDate")]
    public string PropGeneralProposalInformation_EndorsementEffectiveDate { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_EndorsementNo")]
    public string PropGeneralProposalInformation_EndorsementNo { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_EndorsementRequestType")]
    public string PropGeneralProposalInformation_EndorsementRequestType { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_EndorsementRequestTypeCode")]
    public string PropGeneralProposalInformation_EndorsementRequestTypeCode { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_EndorsementSi")]
    public string PropGeneralProposalInformation_EndorsementSi { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_EndorsementType")]
    public string PropGeneralProposalInformation_EndorsementType { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_EndorsementWording")]
    public string PropGeneralProposalInformation_EndorsementWording { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_IndustryCode")]
    public string PropGeneralProposalInformation_IndustryCode { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_InwardNo")]
    public string PropGeneralProposalInformation_InwardNo { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_IsNCBApplicable")]
    public string PropGeneralProposalInformation_IsNCBApplicable { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_ManualCovernoteNo")]
    public string PropGeneralProposalInformation_ManualCovernoteNo { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_MasterPolicy")]
    public string PropGeneralProposalInformation_MasterPolicy { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_MethodOfCalculation")]
    public string PropGeneralProposalInformation_MethodOfCalculation { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_NCBpercentage")]
    public string PropGeneralProposalInformation_NCBpercentage { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_NextB")]
    public string PropGeneralProposalInformation_NextB { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_NoPrevInsuranceFlag")]
    public string PropGeneralProposalInformation_NoPrevInsuranceFlag { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_NonLiablePeriod")]
    public string PropGeneralProposalInformation_NonLiablePeriod { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_NumCovernoteNo")]
    public string PropGeneralProposalInformation_NumCovernoteNo { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_OfficeCode")]
    public string PropGeneralProposalInformation_OfficeCode { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_OfficeName")]
    public string PropGeneralProposalInformation_OfficeName { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_OldCovernoteNo")]
    public string PropGeneralProposalInformation_OldCovernoteNo { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_OptionForCalculation")]
    public string PropGeneralProposalInformation_OptionForCalculation { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_PolicyEffectivedate")]
    public string PropGeneralProposalInformation_PolicyEffectivedate { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_PolicyNo")]
    public string PropGeneralProposalInformation_PolicyNo { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_PolicyNumberChar")]
    public string PropGeneralProposalInformation_PolicyNumberChar { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_PolicySchedule_Mandatary")]
    public string PropGeneralProposalInformation_PolicySchedule_Mandatary { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_PolicyTerm")]
    public string PropGeneralProposalInformation_PolicyTerm { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_ProductCode")]
    public string PropGeneralProposalInformation_ProductCode { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_ProductName")]
    public string PropGeneralProposalInformation_ProductName { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_ProposalFormNumber")]
    public string PropGeneralProposalInformation_ProposalFormNumber { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_ProvisionalBooking")]
    public string PropGeneralProposalInformation_ProvisionalBooking { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_ProvisionalBookingReason")]
    public string PropGeneralProposalInformation_ProvisionalBookingReason { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_QuickId")]
    public string PropGeneralProposalInformation_QuickId { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_QuoteNumber")]
    public string PropGeneralProposalInformation_QuoteNumber { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_ReInsuranceInward")]
    public string PropGeneralProposalInformation_ReInsuranceInward { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_ReferenceNoDate")]
    public string PropGeneralProposalInformation_ReferenceNoDate { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_ReferralCode")]
    public string PropGeneralProposalInformation_ReferralCode { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_RelationshipType")]
    public string PropGeneralProposalInformation_RelationshipType { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_Remarks")]
    public string PropGeneralProposalInformation_Remarks { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_RetainCancellationPremium")]
    public string PropGeneralProposalInformation_RetainCancellationPremium { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_SalesTax")]
    public string PropGeneralProposalInformation_SalesTax { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_Sector_Mandatary")]
    public string PropGeneralProposalInformation_Sector_Mandatary { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_SerDealId")]
    public string PropGeneralProposalInformation_SerDealId { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_ServiceTaxExemptionCategory_Mandatary")]
    public string PropGeneralProposalInformation_ServiceTaxExemptionCategory_Mandatary { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_TotalSi")]
    public string PropGeneralProposalInformation_TotalSi { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_TypeOfBusiness")]
    public string PropGeneralProposalInformation_TypeOfBusiness { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_TypeOfCalculation")]
    public string PropGeneralProposalInformation_TypeOfCalculation { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_TypeOfIndustry")]
    public string PropGeneralProposalInformation_TypeOfIndustry { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_TypeOfPolicy")]
    public string PropGeneralProposalInformation_TypeOfPolicy { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_UWLocation")]
    public string PropGeneralProposalInformation_UWLocation { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_UWLocationCode")]
    public string PropGeneralProposalInformation_UWLocationCode { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_ValidationFlag")]
    public string PropGeneralProposalInformation_ValidationFlag { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_VehicleLaidUpFrom")]
    public string PropGeneralProposalInformation_VehicleLaidUpFrom { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_VehicleLaidUpTo")]
    public string PropGeneralProposalInformation_VehicleLaidUpTo { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_covernoteissuedate")]
    public string PropGeneralProposalInformation_covernoteissuedate { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_covernoteissuetime")]
    public string PropGeneralProposalInformation_covernoteissuetime { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_disprntoffcd")]
    public string PropGeneralProposalInformation_disprntoffcd { get; set; }
    [XmlElement(ElementName = "PropGeneralProposalInformation_iscovernoteused")]
    public string PropGeneralProposalInformation_iscovernoteused { get; set; }
    [XmlElement(ElementName = "PropGeneralProposal_ClauseDetails_Col")]
    public string PropGeneralProposal_ClauseDetails_Col { get; set; }
    [XmlElement(ElementName = "PropGeneralProposal_CoinsuranceDetails_Col")]
    public string PropGeneralProposal_CoinsuranceDetails_Col { get; set; }
    [XmlElement(ElementName = "PropGeneralProposal_ConditionDetails_Col")]
    public string PropGeneralProposal_ConditionDetails_Col { get; set; }
    [XmlElement(ElementName = "PropGeneralProposal_ExclusionDetails_Col")]
    public string PropGeneralProposal_ExclusionDetails_Col { get; set; }
    [XmlElement(ElementName = "PropGeneralProposal_FinancierDetails_Col")]
    public GeneralProposal_FinancierDetails PropGeneralProposal_FinancierDetails_Col { get; set; }
    [XmlElement(ElementName = "PropGeneralProposal_GeneralProposalGroup")]
    public string PropGeneralProposal_GeneralProposalGroup { get; set; }
    //[XmlElement(ElementName = "PropGeneralProposal_PreviousPolicyDetails_Col")]
    //public string PropGeneralProposal_PreviousPolicyDetails_Col { get; set; }
    [XmlElement(ElementName = "PropGeneralProposal_PreviousPolicyDetails_Col")]
    public PropGeneralProposal_PreviousPolicyDetails_Col PropGeneralProposal_PreviousPolicyDetails_Col { get; set; }
    [XmlElement(ElementName = "PropGeneralProposal_Warranty_Col")]
    public string PropGeneralProposal_Warranty_Col { get; set; }
    [XmlElement(ElementName = "PropHigherExcessDiscount_HE_Disc_RateType_In")]
    public string PropHigherExcessDiscount_HE_Disc_RateType_In { get; set; }
    [XmlElement(ElementName = "PropIntermediaryDetails_IntermediaryType")]
    public string PropIntermediaryDetails_IntermediaryType { get; set; }
    //[XmlElement(ElementName = "PropLoadingDiscount_Col")]
    //public string PropLoadingDiscount_Col { get; set; }

    [XmlElement(ElementName = "PropLoadingDiscount_Col")]
    public PropLoadingDiscount_Col PropLoadingDiscount_Col { get; set; }

    [XmlElement(ElementName = "PropMODetails_PrimaryMOCode")]
    public string PropMODetails_PrimaryMOCode { get; set; }
    [XmlElement(ElementName = "PropMODetails_PrimaryMOName")]
    public string PropMODetails_PrimaryMOName { get; set; }
    [XmlElement(ElementName = "PropMODetails_SecondaryMOCode")]
    public string PropMODetails_SecondaryMOCode { get; set; }
    [XmlElement(ElementName = "PropMODetails_SecondaryMOName")]
    public string PropMODetails_SecondaryMOName { get; set; }
    [XmlElement(ElementName = "PropMODetails_TertiaryMOCode")]
    public string PropMODetails_TertiaryMOCode { get; set; }
    [XmlElement(ElementName = "PropMODetails_TertiaryMOName")]
    public string PropMODetails_TertiaryMOName { get; set; }
    [XmlElement(ElementName = "PropMotorOtherNodes_AfterFetch")]
    public string PropMotorOtherNodes_AfterFetch { get; set; }
    [XmlElement(ElementName = "PropMotorOtherNodes_CancelationDetail")]
    public string PropMotorOtherNodes_CancelationDetail { get; set; }
    [XmlElement(ElementName = "PropMotorOtherNodes_EndorsementDtls")]
    public string PropMotorOtherNodes_EndorsementDtls { get; set; }
    [XmlElement(ElementName = "PropMotorOtherNodes_validate_aftercalc")]
    public string PropMotorOtherNodes_validate_aftercalc { get; set; }
    [XmlElement(ElementName = "PropMotorOtherNodes_validate_beforecalc")]
    public string PropMotorOtherNodes_validate_beforecalc { get; set; }
    [XmlElement(ElementName = "PropNonLiablePeriod_NonLiableEndTime")]
    public string PropNonLiablePeriod_NonLiableEndTime { get; set; }
    [XmlElement(ElementName = "PropNonLiablePeriod_NonLiableFromDate")]
    public string PropNonLiablePeriod_NonLiableFromDate { get; set; }
    [XmlElement(ElementName = "PropNonLiablePeriod_NonLiableStartTime")]
    public string PropNonLiablePeriod_NonLiableStartTime { get; set; }
    [XmlElement(ElementName = "PropNonLiablePeriod_NonLiableToDate")]
    public string PropNonLiablePeriod_NonLiableToDate { get; set; }
    [XmlElement(ElementName = "PropParameters_AddClause")]
    public string PropParameters_AddClause { get; set; }
    [XmlElement(ElementName = "PropParameters_AddCondition")]
    public string PropParameters_AddCondition { get; set; }
    [XmlElement(ElementName = "PropParameters_AddExclusion")]
    public string PropParameters_AddExclusion { get; set; }
    [XmlElement(ElementName = "PropParameters_AddWarranty")]
    public string PropParameters_AddWarranty { get; set; }
    [XmlElement(ElementName = "PropParameters_AfterFetch")]
    public string PropParameters_AfterFetch { get; set; }
    [XmlElement(ElementName = "PropParameters_ClauseApplicable")]
    public string PropParameters_ClauseApplicable { get; set; }
    [XmlElement(ElementName = "PropParameters_CoInsuranceApplicability")]
    public string PropParameters_CoInsuranceApplicability { get; set; }
    [XmlElement(ElementName = "PropParameters_CommissionApplicable")]
    public string PropParameters_CommissionApplicable { get; set; }
    [XmlElement(ElementName = "PropParameters_ConditionApplicable")]
    public string PropParameters_ConditionApplicable { get; set; }
    [XmlElement(ElementName = "PropParameters_CoverNoteApplicability")]
    public string PropParameters_CoverNoteApplicability { get; set; }
    [XmlElement(ElementName = "PropParameters_EQHigherExcessDiscount")]
    public string PropParameters_EQHigherExcessDiscount { get; set; }
    [XmlElement(ElementName = "PropParameters_ExclusionApplicable")]
    public string PropParameters_ExclusionApplicable { get; set; }
    [XmlElement(ElementName = "PropParameters_FamilyPackageDiscount")]
    public string PropParameters_FamilyPackageDiscount { get; set; }
    [XmlElement(ElementName = "PropParameters_FinancierApplicability")]
    public string PropParameters_FinancierApplicability { get; set; }
    [XmlElement(ElementName = "PropParameters_FloaterRiskLoading")]
    public string PropParameters_FloaterRiskLoading { get; set; }
    [XmlElement(ElementName = "PropParameters_GroupDiscount")]
    public string PropParameters_GroupDiscount { get; set; }
    [XmlElement(ElementName = "PropParameters_HighClaimLoading")]
    public string PropParameters_HighClaimLoading { get; set; }
    [XmlElement(ElementName = "PropParameters_HigherExcessDiscount")]
    public string PropParameters_HigherExcessDiscount { get; set; }
    [XmlElement(ElementName = "PropParameters_Installment")]
    public string PropParameters_Installment { get; set; }
    [XmlElement(ElementName = "PropParameters_LongTermPolicy")]
    public string PropParameters_LongTermPolicy { get; set; }
    [XmlElement(ElementName = "PropParameters_LowClaimDiscount")]
    public string PropParameters_LowClaimDiscount { get; set; }
    [XmlElement(ElementName = "PropParameters_MinimumPremium")]
    public string PropParameters_MinimumPremium { get; set; }
    [XmlElement(ElementName = "PropParameters_NoClaimDiscount")]
    public string PropParameters_NoClaimDiscount { get; set; }
    [XmlElement(ElementName = "PropParameters_OverageLoading")]
    public string PropParameters_OverageLoading { get; set; }
    [XmlElement(ElementName = "PropParameters_PasPolicyApplicable")]
    public string PropParameters_PasPolicyApplicable { get; set; }
    [XmlElement(ElementName = "PropParameters_PastPolicyApplicable")]
    public string PropParameters_PastPolicyApplicable { get; set; }
    [XmlElement(ElementName = "PropParameters_PolicyCancellation")]
    public string PropParameters_PolicyCancellation { get; set; }
    [XmlElement(ElementName = "PropParameters_PopulateTransactionBegin")]
    public string PropParameters_PopulateTransactionBegin { get; set; }
    [XmlElement(ElementName = "PropParameters_PreviousPolicyApplicable")]
    public string PropParameters_PreviousPolicyApplicable { get; set; }
    [XmlElement(ElementName = "PropParameters_RenewalAllowed")]
    public string PropParameters_RenewalAllowed { get; set; }
    [XmlElement(ElementName = "PropParameters_SectionDiscount")]
    public string PropParameters_SectionDiscount { get; set; }
    [XmlElement(ElementName = "PropParameters_ServiceTaxExempted")]
    public string PropParameters_ServiceTaxExempted { get; set; }
    [XmlElement(ElementName = "PropParameters_ShortPeriodLongPeriod")]
    public string PropParameters_ShortPeriodLongPeriod { get; set; }
    [XmlElement(ElementName = "PropParameters_SpecialDiscountApplicable")]
    public string PropParameters_SpecialDiscountApplicable { get; set; }
    [XmlElement(ElementName = "PropParameters_StaffDiscount")]
    public string PropParameters_StaffDiscount { get; set; }
    [XmlElement(ElementName = "PropParameters_StampDutyChargeable")]
    public string PropParameters_StampDutyChargeable { get; set; }
    [XmlElement(ElementName = "PropParameters_TariffProduct")]
    public string PropParameters_TariffProduct { get; set; }
    [XmlElement(ElementName = "PropParameters_TransferFees")]
    public string PropParameters_TransferFees { get; set; }
    [XmlElement(ElementName = "PropParameters_VDFDiscountAllowed")]
    public string PropParameters_VDFDiscountAllowed { get; set; }
    [XmlElement(ElementName = "PropParameters_ValidateClause")]
    public string PropParameters_ValidateClause { get; set; }
    [XmlElement(ElementName = "PropParameters_ValidateCondition")]
    public string PropParameters_ValidateCondition { get; set; }
    [XmlElement(ElementName = "PropParameters_ValidateEndorsementData")]
    public string PropParameters_ValidateEndorsementData { get; set; }
    [XmlElement(ElementName = "PropParameters_ValidateExclusion")]
    public string PropParameters_ValidateExclusion { get; set; }
    [XmlElement(ElementName = "PropParameters_ValidateWarranty")]
    public string PropParameters_ValidateWarranty { get; set; }
    [XmlElement(ElementName = "PropParameters_WarrentyApplicable")]
    public string PropParameters_WarrentyApplicable { get; set; }
    [XmlElement(ElementName = "PropPolicyEffectivedate_Fromdate_Mandatary")]
    public string PropPolicyEffectivedate_Fromdate_Mandatary { get; set; }
    [XmlElement(ElementName = "PropPolicyEffectivedate_Fromhour_Mandatary")]
    public string PropPolicyEffectivedate_Fromhour_Mandatary { get; set; }
    [XmlElement(ElementName = "PropPolicyEffectivedate_Todate_Mandatary")]
    public string PropPolicyEffectivedate_Todate_Mandatary { get; set; }
    [XmlElement(ElementName = "PropPolicyEffectivedate_Tohour_Mandatary")]
    public string PropPolicyEffectivedate_Tohour_Mandatary { get; set; }
    [XmlElement(ElementName = "PropPremiumCalculation_BasicPremium")]
    public string PropPremiumCalculation_BasicPremium { get; set; }
    [XmlElement(ElementName = "PropPremiumCalculation_Cess")]
    public string PropPremiumCalculation_Cess { get; set; }
    [XmlElement(ElementName = "PropPremiumCalculation_CustomCoverLDPremium")]
    public string PropPremiumCalculation_CustomCoverLDPremium { get; set; }
    [XmlElement(ElementName = "PropPremiumCalculation_ERFAmount")]
    public string PropPremiumCalculation_ERFAmount { get; set; }
    [XmlElement(ElementName = "PropPremiumCalculation_EndorsementERFAmount")]
    public string PropPremiumCalculation_EndorsementERFAmount { get; set; }
    [XmlElement(ElementName = "PropPremiumCalculation_EndorsementPremium")]
    public string PropPremiumCalculation_EndorsementPremium { get; set; }
    [XmlElement(ElementName = "PropPremiumCalculation_EndorsementServiceTax")]
    public string PropPremiumCalculation_EndorsementServiceTax { get; set; }
    [XmlElement(ElementName = "PropPremiumCalculation_EndorsementStampDuty")]
    public string PropPremiumCalculation_EndorsementStampDuty { get; set; }
    [XmlElement(ElementName = "PropPremiumCalculation_EndorsementTerrorismPremium")]
    public string PropPremiumCalculation_EndorsementTerrorismPremium { get; set; }
    [XmlElement(ElementName = "PropPremiumCalculation_GrossCoverPremium")]
    public string PropPremiumCalculation_GrossCoverPremium { get; set; }
    [XmlElement(ElementName = "PropPremiumCalculation_NetODPremium")]
    public string PropPremiumCalculation_NetODPremium { get; set; }
    [XmlElement(ElementName = "PropPremiumCalculation_NetPremium")]
    public string PropPremiumCalculation_NetPremium { get; set; }
    [XmlElement(ElementName = "PropPremiumCalculation_NetTPPremium")]
    public string PropPremiumCalculation_NetTPPremium { get; set; }
    [XmlElement(ElementName = "PropPremiumCalculation_SP_PR_LTFactor")]
    public string PropPremiumCalculation_SP_PR_LTFactor { get; set; }
    [XmlElement(ElementName = "PropPremiumCalculation_ServiceTax")]
    public string PropPremiumCalculation_ServiceTax { get; set; }
    [XmlElement(ElementName = "PropPremiumCalculation_StampDuty")]
    public string PropPremiumCalculation_StampDuty { get; set; }
    [XmlElement(ElementName = "PropPremiumCalculation_StampDutyApplicability")]
    public string PropPremiumCalculation_StampDutyApplicability { get; set; }
    [XmlElement(ElementName = "PropPremiumCalculation_TerrorismPremium")]
    public string PropPremiumCalculation_TerrorismPremium { get; set; }
    [XmlElement(ElementName = "PropPremiumCalculation_TotalPremium")]
    public string PropPremiumCalculation_TotalPremium { get; set; }
    [XmlElement(ElementName = "PropPremiumCalculation_TransferFeeAmount")]
    public string PropPremiumCalculation_TransferFeeAmount { get; set; }
    [XmlElement(ElementName = "PropPreviousPolicyDetails_AddRow")]
    public string PropPreviousPolicyDetails_AddRow { get; set; }
    [XmlElement(ElementName = "PropPreviousPolicyDetails_CalculateClaimRatio")]
    public string PropPreviousPolicyDetails_CalculateClaimRatio { get; set; }
    [XmlElement(ElementName = "PropPreviousPolicyDetails_ClaimRatio")]
    public string PropPreviousPolicyDetails_ClaimRatio { get; set; }
    [XmlElement(ElementName = "PropPreviousPolicyDetails_NCBPercentage")]
    public string PropPreviousPolicyDetails_NCBPercentage { get; set; }
    [XmlElement(ElementName = "PropPreviousPolicyDetails_OldTariffSlab")]
    public string PropPreviousPolicyDetails_OldTariffSlab { get; set; }
    [XmlElement(ElementName = "PropPreviousPolicyDetails_PreviousPolicyType")]
    public string PropPreviousPolicyDetails_PreviousPolicyType { get; set; }
    [XmlElement(ElementName = "PropPreviousPolicyDetails_Validate")]
    public string PropPreviousPolicyDetails_Validate { get; set; }
    [XmlElement(ElementName = "PropProductDetails_Component17")]
    public string PropProductDetails_Component17 { get; set; }
    [XmlElement(ElementName = "PropProductDetails_DepartmentCode")]
    public string PropProductDetails_DepartmentCode { get; set; }
    [XmlElement(ElementName = "PropProductDetails_ProductCode")]
    public string PropProductDetails_ProductCode { get; set; }
    [XmlElement(ElementName = "PropProductDetails_ProductName")]
    public string PropProductDetails_ProductName { get; set; }
    [XmlElement(ElementName = "PropProductName")]
    public string PropProductName { get; set; }
    [XmlElement(ElementName = "PropReferenceNoDate_ReferenceNo_Mandatary")]
    public string PropReferenceNoDate_ReferenceNo_Mandatary { get; set; }
    [XmlElement(ElementName = "PropRisks_AAMembership")]
    public string PropRisks_AAMembership { get; set; }
    [XmlElement(ElementName = "PropRisks_AAOmembershipno")]
    public string PropRisks_AAOmembershipno { get; set; }
    [XmlElement(ElementName = "PropRisks_Age")]
    public string PropRisks_Age { get; set; }
    [XmlElement(ElementName = "PropRisks_AntiTheftCheck")]
    public string PropRisks_AntiTheftCheck { get; set; }
    [XmlElement(ElementName = "PropRisks_ApprvlRemark")]
    public string PropRisks_ApprvlRemark { get; set; }
    [XmlElement(ElementName = "PropRisks_AuthorityLocation_Mandatary")]
    public string PropRisks_AuthorityLocation_Mandatary { get; set; }
    [XmlElement(ElementName = "PropRisks_AutoMobileAssocName")]
    public string PropRisks_AutoMobileAssocName { get; set; }
    [XmlElement(ElementName = "PropRisks_AutoMobileAssocNoCode")]
    public string PropRisks_AutoMobileAssocNoCode { get; set; }
    [XmlElement(ElementName = "PropRisks_Bangladesh")]
    public string PropRisks_Bangladesh { get; set; }
    [XmlElement(ElementName = "PropRisks_Bhutan")]
    public string PropRisks_Bhutan { get; set; }
    [XmlElement(ElementName = "PropRisks_CalcType")]
    public string PropRisks_CalcType { get; set; }
    [XmlElement(ElementName = "PropRisks_CertifiedbyARAI")]
    public string PropRisks_CertifiedbyARAI { get; set; }
    [XmlElement(ElementName = "PropRisks_Chassisn_Mandatary")]
    public string PropRisks_Chassisn_Mandatary { get; set; }
    [XmlElement(ElementName = "PropRisks_Col")]
    public PropRisks_Col PropRisks_Col { get; set; }
    [XmlElement(ElementName = "PropRisks_Colorofthevehicle")]
    public string PropRisks_Colorofthevehicle { get; set; }
    [XmlElement(ElementName = "PropRisks_CompulsoryExcess")]
    public string PropRisks_CompulsoryExcess { get; set; }
    [XmlElement(ElementName = "PropRisks_CourtesyCar")]
    public string PropRisks_CourtesyCar { get; set; }
    [XmlElement(ElementName = "PropRisks_CourtesyNoOfDays")]
    public string PropRisks_CourtesyNoOfDays { get; set; }
    [XmlElement(ElementName = "PropRisks_DateOfTmpRegistration")]
    public string PropRisks_DateOfTmpRegistration { get; set; }
    [XmlElement(ElementName = "PropRisks_DateofRegistration")]
    public string PropRisks_DateofRegistration { get; set; }
    [XmlElement(ElementName = "PropRisks_Dateofdelivery")]
    public string PropRisks_Dateofdelivery { get; set; }
    [XmlElement(ElementName = "PropRisks_Dateofpurchase")]
    public string PropRisks_Dateofpurchase { get; set; }
    [XmlElement(ElementName = "PropRisks_Details")]
    public string PropRisks_Details { get; set; }
    [XmlElement(ElementName = "PropRisks_DetariffFlag")]
    public string PropRisks_DetariffFlag { get; set; }
    [XmlElement(ElementName = "PropRisks_Drivingexperience")]
    public string PropRisks_Drivingexperience { get; set; }
    [XmlElement(ElementName = "PropRisks_Drivingliscensetype")]
    public string PropRisks_Drivingliscensetype { get; set; }
    [XmlElement(ElementName = "PropRisks_DrvTutionLDChk")]
    public string PropRisks_DrvTutionLDChk { get; set; }
    [XmlElement(ElementName = "PropRisks_Educationalqualification")]
    public string PropRisks_Educationalqualification { get; set; }
    [XmlElement(ElementName = "PropRisks_Engineno_Mandatary")]
    public string PropRisks_Engineno_Mandatary { get; set; }
    [XmlElement(ElementName = "PropRisks_Expirydate")]
    public string PropRisks_Expirydate { get; set; }
    [XmlElement(ElementName = "PropRisks_Exshowroomprice_Mandatary")]
    public string PropRisks_Exshowroomprice_Mandatary { get; set; }
    [XmlElement(ElementName = "PropRisks_ExtraNode1")]
    public string PropRisks_ExtraNode1 { get; set; }
    [XmlElement(ElementName = "PropRisks_ExtraNode10")]
    public string PropRisks_ExtraNode10 { get; set; }
    [XmlElement(ElementName = "PropRisks_ExtraNode2")]
    public string PropRisks_ExtraNode2 { get; set; }
    [XmlElement(ElementName = "PropRisks_ExtraNode3")]
    public string PropRisks_ExtraNode3 { get; set; }
    [XmlElement(ElementName = "PropRisks_ExtraNode4")]
    public string PropRisks_ExtraNode4 { get; set; }
    [XmlElement(ElementName = "PropRisks_ExtraNode5")]
    public string PropRisks_ExtraNode5 { get; set; }
    [XmlElement(ElementName = "PropRisks_ExtraNode6")]
    public string PropRisks_ExtraNode6 { get; set; }
    [XmlElement(ElementName = "PropRisks_ExtraNode7")]
    public string PropRisks_ExtraNode7 { get; set; }
    [XmlElement(ElementName = "PropRisks_ExtraNode8")]
    public string PropRisks_ExtraNode8 { get; set; }
    [XmlElement(ElementName = "PropRisks_ExtraNode9")]
    public string PropRisks_ExtraNode9 { get; set; }
    [XmlElement(ElementName = "PropRisks_ForeignEmbassyCheck")]
    public string PropRisks_ForeignEmbassyCheck { get; set; }
    [XmlElement(ElementName = "PropRisks_FuelUsed_Mandatary")]
    public string PropRisks_FuelUsed_Mandatary { get; set; }
    [XmlElement(ElementName = "PropRisks_Gender")]
    public string PropRisks_Gender { get; set; }
    [XmlElement(ElementName = "PropRisks_Geographicalareaextention")]
    public string PropRisks_Geographicalareaextention { get; set; }
    [XmlElement(ElementName = "PropRisks_HPCubicCapacity_Mandatary")]
    public string PropRisks_HPCubicCapacity_Mandatary { get; set; }
    [XmlElement(ElementName = "PropRisks_HandicapLDCheck")]
    public string PropRisks_HandicapLDCheck { get; set; }
    [XmlElement(ElementName = "PropRisks_HealthHabbits")]
    public string PropRisks_HealthHabbits { get; set; }
    [XmlElement(ElementName = "PropRisks_IDVofthevehicle")]
    public string PropRisks_IDVofthevehicle { get; set; }
    [XmlElement(ElementName = "PropRisks_ImposedExcessAmount")]
    public string PropRisks_ImposedExcessAmount { get; set; }
    [XmlElement(ElementName = "PropRisks_InBuiltLPGkit")]
    public string PropRisks_InBuiltLPGkit { get; set; }
    [XmlElement(ElementName = "PropRisks_InbuiltCNGKit")]
    public string PropRisks_InbuiltCNGKit { get; set; }
    [XmlElement(ElementName = "PropRisks_IsNilDepreciation")]
    public string PropRisks_IsNilDepreciation { get; set; }
    [XmlElement(ElementName = "PropRisks_IsPersonalEffect")]
    public string PropRisks_IsPersonalEffect { get; set; }
    [XmlElement(ElementName = "PropRisks_IsTheCarCeritifiedAsVcVcccI")]
    public string PropRisks_IsTheCarCeritifiedAsVcVcccI { get; set; }
    [XmlElement(ElementName = "PropRisks_IsVehicle")]
    public string PropRisks_IsVehicle { get; set; }
    [XmlElement(ElementName = "PropRisks_IsVehicleImported")]
    public string PropRisks_IsVehicleImported { get; set; }
    [XmlElement(ElementName = "PropRisks_LicenseIssueDate")]
    public string PropRisks_LicenseIssueDate { get; set; }
    [XmlElement(ElementName = "PropRisks_Lifemember")]
    public string PropRisks_Lifemember { get; set; }
    [XmlElement(ElementName = "PropRisks_LiscenseNo")]
    public string PropRisks_LiscenseNo { get; set; }
    [XmlElement(ElementName = "PropRisks_LiscensedCarryingCapacity_Mandatary")]
    public string PropRisks_LiscensedCarryingCapacity_Mandatary { get; set; }
    [XmlElement(ElementName = "PropRisks_Liscenseexpirydate")]
    public string PropRisks_Liscenseexpirydate { get; set; }
    [XmlElement(ElementName = "PropRisks_Liscenseissuingauthority")]
    public string PropRisks_Liscenseissuingauthority { get; set; }
    [XmlElement(ElementName = "PropRisks_LocationCode")]
    public string PropRisks_LocationCode { get; set; }
    [XmlElement(ElementName = "PropRisks_LtdOwnPremisesCheck")]
    public string PropRisks_LtdOwnPremisesCheck { get; set; }
    [XmlElement(ElementName = "PropRisks_Make")]
    public string PropRisks_Make { get; set; }
    [XmlElement(ElementName = "PropRisks_Maldives")]
    public string PropRisks_Maldives { get; set; }
    [XmlElement(ElementName = "PropRisks_Manufacture")]
    public string PropRisks_Manufacture { get; set; }
    [XmlElement(ElementName = "PropRisks_ManufactureCode")]
    public string PropRisks_ManufactureCode { get; set; }
    [XmlElement(ElementName = "PropRisks_ManufactureMonth")]
    public string PropRisks_ManufactureMonth { get; set; }
    [XmlElement(ElementName = "PropRisks_MedicalCoverOpted")]
    public string PropRisks_MedicalCoverOpted { get; set; }
    [XmlElement(ElementName = "PropRisks_MedicalExpOption")]
    public string PropRisks_MedicalExpOption { get; set; }
    [XmlElement(ElementName = "PropRisks_Model")]
    public string PropRisks_Model { get; set; }
    [XmlElement(ElementName = "PropRisks_ModelCode")]
    public string PropRisks_ModelCode { get; set; }
    [XmlElement(ElementName = "PropRisks_Name")]
    public string PropRisks_Name { get; set; }
    [XmlElement(ElementName = "PropRisks_NeWCombo15")]
    public string PropRisks_NeWCombo15 { get; set; }
    [XmlElement(ElementName = "PropRisks_Nepal")]
    public string PropRisks_Nepal { get; set; }
    [XmlElement(ElementName = "PropRisks_NewBool10")]
    public string PropRisks_NewBool10 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewBool11")]
    public string PropRisks_NewBool11 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewBool12")]
    public string PropRisks_NewBool12 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewBool13")]
    public string PropRisks_NewBool13 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewBool14")]
    public string PropRisks_NewBool14 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewBool15")]
    public string PropRisks_NewBool15 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewBool2")]
    public string PropRisks_NewBool2 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewBool3")]
    public string PropRisks_NewBool3 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewBool4")]
    public string PropRisks_NewBool4 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewBool5")]
    public string PropRisks_NewBool5 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewBool6")]
    public string PropRisks_NewBool6 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewBool7")]
    public string PropRisks_NewBool7 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewBool8")]
    public string PropRisks_NewBool8 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewBool9")]
    public string PropRisks_NewBool9 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewCombo10")]
    public string PropRisks_NewCombo10 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewCombo11")]
    public string PropRisks_NewCombo11 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewCombo12")]
    public string PropRisks_NewCombo12 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewCombo13")]
    public string PropRisks_NewCombo13 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewCombo14")]
    public string PropRisks_NewCombo14 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewCombo6")]
    public string PropRisks_NewCombo6 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewCombo7")]
    public string PropRisks_NewCombo7 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewCombo8")]
    public string PropRisks_NewCombo8 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewCombo9")]
    public string PropRisks_NewCombo9 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewDD1")]
    public string PropRisks_NewDD1 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewDD2")]
    public string PropRisks_NewDD2 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewDD3")]
    public string PropRisks_NewDD3 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewDD4")]
    public string PropRisks_NewDD4 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewDD5")]
    public string PropRisks_NewDD5 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewNumeric1")]
    public string PropRisks_NewNumeric1 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewNumeric10")]
    public string PropRisks_NewNumeric10 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewNumeric16")]
    public string PropRisks_NewNumeric16 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewNumeric17")]
    public string PropRisks_NewNumeric17 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewNumeric18")]
    public string PropRisks_NewNumeric18 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewNumeric19")]
    public string PropRisks_NewNumeric19 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewNumeric2")]
    public string PropRisks_NewNumeric2 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewNumeric20")]
    public string PropRisks_NewNumeric20 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewNumeric3")]
    public string PropRisks_NewNumeric3 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewNumeric4")]
    public string PropRisks_NewNumeric4 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewNumeric5")]
    public string PropRisks_NewNumeric5 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewNumeric6")]
    public string PropRisks_NewNumeric6 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewNumeric7")]
    public string PropRisks_NewNumeric7 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewNumeric8")]
    public string PropRisks_NewNumeric8 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewNumeric9")]
    public string PropRisks_NewNumeric9 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewText1")]
    public string PropRisks_NewText1 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewText10")]
    public string PropRisks_NewText10 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewText11")]
    public string PropRisks_NewText11 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewText12")]
    public string PropRisks_NewText12 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewText13")]
    public string PropRisks_NewText13 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewText14")]
    public string PropRisks_NewText14 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewText15")]
    public string PropRisks_NewText15 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewText2")]
    public string PropRisks_NewText2 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewText3")]
    public string PropRisks_NewText3 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewText4")]
    public string PropRisks_NewText4 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewText5")]
    public string PropRisks_NewText5 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewText6")]
    public string PropRisks_NewText6 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewText7")]
    public string PropRisks_NewText7 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewText8")]
    public string PropRisks_NewText8 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewText9")]
    public string PropRisks_NewText9 { get; set; }
    [XmlElement(ElementName = "PropRisks_NewwBool1")]
    public string PropRisks_NewwBool1 { get; set; }
    [XmlElement(ElementName = "PropRisks_NilDep")]
    public string PropRisks_NilDep { get; set; }
    [XmlElement(ElementName = "PropRisks_NoClaimBonusApplicable")]
    public string PropRisks_NoClaimBonusApplicable { get; set; }
    [XmlElement(ElementName = "PropRisks_NoOfEmployees")]
    public string PropRisks_NoOfEmployees { get; set; }
    [XmlElement(ElementName = "PropRisks_NoOfPersonsNamed")]
    public string PropRisks_NoOfPersonsNamed { get; set; }
    [XmlElement(ElementName = "PropRisks_NoOfPersonsUnnamed")]
    public string PropRisks_NoOfPersonsUnnamed { get; set; }
    [XmlElement(ElementName = "PropRisks_NoOfSafetyFeature")]
    public string PropRisks_NoOfSafetyFeature { get; set; }
    [XmlElement(ElementName = "PropRisks_NoOfWorkmen")]
    public string PropRisks_NoOfWorkmen { get; set; }
    [XmlElement(ElementName = "PropRisks_NoofDrivers")]
    public string PropRisks_NoofDrivers { get; set; }
    [XmlElement(ElementName = "PropRisks_NoofTrailers")]
    public string PropRisks_NoofTrailers { get; set; }
    [XmlElement(ElementName = "PropRisks_NoofViloationsConvictions")]
    public string PropRisks_NoofViloationsConvictions { get; set; }
    [XmlElement(ElementName = "PropRisks_ObsoleteIDV")]
    public string PropRisks_ObsoleteIDV { get; set; }
    [XmlElement(ElementName = "PropRisks_ObsoleteVehicleOtherMake")]
    public string PropRisks_ObsoleteVehicleOtherMake { get; set; }
    [XmlElement(ElementName = "PropRisks_OtherInfo1")]
    public string PropRisks_OtherInfo1 { get; set; }
    [XmlElement(ElementName = "PropRisks_OtherInfo2")]
    public string PropRisks_OtherInfo2 { get; set; }
    [XmlElement(ElementName = "PropRisks_OtherInfo3")]
    public string PropRisks_OtherInfo3 { get; set; }
    [XmlElement(ElementName = "PropRisks_OtherInfo4")]
    public string PropRisks_OtherInfo4 { get; set; }
    [XmlElement(ElementName = "PropRisks_OtherInfo5")]
    public string PropRisks_OtherInfo5 { get; set; }
    [XmlElement(ElementName = "PropRisks_OtherInfo6")]
    public string PropRisks_OtherInfo6 { get; set; }
    [XmlElement(ElementName = "PropRisks_PaidDriverCleaner")]
    public string PropRisks_PaidDriverCleaner { get; set; }
    [XmlElement(ElementName = "PropRisks_PaidDriverSI")]
    public string PropRisks_PaidDriverSI { get; set; }
    [XmlElement(ElementName = "PropRisks_Pakistan")]
    public string PropRisks_Pakistan { get; set; }
    [XmlElement(ElementName = "PropRisks_PersonalEffectOption")]
    public string PropRisks_PersonalEffectOption { get; set; }
    [XmlElement(ElementName = "PropRisks_PrevYearNCB")]
    public string PropRisks_PrevYearNCB { get; set; }
    [XmlElement(ElementName = "PropRisks_RTOCode")]
    public string PropRisks_RTOCode { get; set; }
    [XmlElement(ElementName = "PropRisks_RallyCheck")]
    public string PropRisks_RallyCheck { get; set; }
    [XmlElement(ElementName = "PropRisks_Rallyfromdate")]
    public string PropRisks_Rallyfromdate { get; set; }
    [XmlElement(ElementName = "PropRisks_Rallytodate")]
    public string PropRisks_Rallytodate { get; set; }
    [XmlElement(ElementName = "PropRisks_RegistrationNUmber3")]
    public string PropRisks_RegistrationNUmber3 { get; set; }
    [XmlElement(ElementName = "PropRisks_RegistrationNumber")]
    public string PropRisks_RegistrationNumber { get; set; }
    [XmlElement(ElementName = "PropRisks_RegistrationNumber2")]
    public string PropRisks_RegistrationNumber2 { get; set; }
    [XmlElement(ElementName = "PropRisks_RegistrationNumber4")]
    public string PropRisks_RegistrationNumber4 { get; set; }
    [XmlElement(ElementName = "PropRisks_ReliabilityTrialCheck")]
    public string PropRisks_ReliabilityTrialCheck { get; set; }
    [XmlElement(ElementName = "PropRisks_Reliabilitytrialorracingdate")]
    public string PropRisks_Reliabilitytrialorracingdate { get; set; }
    [XmlElement(ElementName = "PropRisks_Reliabilitytrialorracingtodate")]
    public string PropRisks_Reliabilitytrialorracingtodate { get; set; }
    [XmlElement(ElementName = "PropRisks_ResnForAppOldTPRt")]
    public string PropRisks_ResnForAppOldTPRt { get; set; }
    [XmlElement(ElementName = "PropRisks_RiskStartDate_Mandatary")]
    public string PropRisks_RiskStartDate_Mandatary { get; set; }
    [XmlElement(ElementName = "PropRisks_RiskType")]
    public string PropRisks_RiskType { get; set; }
    [XmlElement(ElementName = "PropRisks_RiskVariant")]
    public string PropRisks_RiskVariant { get; set; }
    [XmlElement(ElementName = "PropRisks_SeatingCapacity_Mandatary")]
    public string PropRisks_SeatingCapacity_Mandatary { get; set; }
    [XmlElement(ElementName = "PropRisks_Srilanka")]
    public string PropRisks_Srilanka { get; set; }
    [XmlElement(ElementName = "PropRisks_TPPDCover_Mandatary")]
    public string PropRisks_TPPDCover_Mandatary { get; set; }
    [XmlElement(ElementName = "PropRisks_ThreeWheelerHandicapCheck")]
    public string PropRisks_ThreeWheelerHandicapCheck { get; set; }
    [XmlElement(ElementName = "PropRisks_TowingCharges")]
    public string PropRisks_TowingCharges { get; set; }
    [XmlElement(ElementName = "PropRisks_TrailerOD")]
    public string PropRisks_TrailerOD { get; set; }
    [XmlElement(ElementName = "PropRisks_TrailerODRate")]
    public string PropRisks_TrailerODRate { get; set; }
    [XmlElement(ElementName = "PropRisks_Typeofbody_Mandatary")]
    public string PropRisks_Typeofbody_Mandatary { get; set; }
    [XmlElement(ElementName = "PropRisks_UnnamedPASI")]
    public string PropRisks_UnnamedPASI { get; set; }
    [XmlElement(ElementName = "PropRisks_ValidDrivingLicense")]
    public string PropRisks_ValidDrivingLicense { get; set; }
    [XmlElement(ElementName = "PropRisks_VechilesLDSailors")]
    public string PropRisks_VechilesLDSailors { get; set; }
    [XmlElement(ElementName = "PropRisks_VehicleAge_Mandatary")]
    public string PropRisks_VehicleAge_Mandatary { get; set; }
    [XmlElement(ElementName = "PropRisks_VehicleIDV")]
    public string PropRisks_VehicleIDV { get; set; }
    [XmlElement(ElementName = "PropRisks_VehicleLaidUpRefund")]
    public string PropRisks_VehicleLaidUpRefund { get; set; }
    [XmlElement(ElementName = "PropRisks_VehicleLaidUpRefund2")]
    public string PropRisks_VehicleLaidUpRefund2 { get; set; }
    [XmlElement(ElementName = "PropRisks_VehicleTypeCode")]
    public string PropRisks_VehicleTypeCode { get; set; }
    [XmlElement(ElementName = "PropRisks_Vintagecarfromdate")]
    public string PropRisks_Vintagecarfromdate { get; set; }
    [XmlElement(ElementName = "PropRisks_VoluntaryExcessAmount")]
    public string PropRisks_VoluntaryExcessAmount { get; set; }
    [XmlElement(ElementName = "PropRisks_WhthrPrevTPRtChrgbl")]
    public string PropRisks_WhthrPrevTPRtChrgbl { get; set; }
    [XmlElement(ElementName = "PropRisks_YearofManufacture")]
    public string PropRisks_YearofManufacture { get; set; }
    [XmlElement(ElementName = "PropRisks_Zone_Mandatary")]
    public string PropRisks_Zone_Mandatary { get; set; }
    [XmlElement(ElementName = "PropRisks_laidupvehiclepremium")]
    public string PropRisks_laidupvehiclepremium { get; set; }
    [XmlElement(ElementName = "PropRisks_racingOD")]
    public string PropRisks_racingOD { get; set; }
    [XmlElement(ElementName = "PropRisks_racingTP")]
    public string PropRisks_racingTP { get; set; }
    [XmlElement(ElementName = "PropRisks_rallyOD")]
    public string PropRisks_rallyOD { get; set; }
    [XmlElement(ElementName = "PropRisks_rallyTP")]
    public string PropRisks_rallyTP { get; set; }
    [XmlElement(ElementName = "PropSPDetails_SPCode")]
    public string PropSPDetails_SPCode { get; set; }
    [XmlElement(ElementName = "PropSPDetails_SPName")]
    public string PropSPDetails_SPName { get; set; }
    [XmlElement(ElementName = "PropSerIntermediaryDetails_SerIntermediaryCode")]
    public string PropSerIntermediaryDetails_SerIntermediaryCode { get; set; }
    [XmlElement(ElementName = "PropSerIntermediaryDetails_SerIntermediaryName")]
    public string PropSerIntermediaryDetails_SerIntermediaryName { get; set; }
    [XmlElement(ElementName = "PropSerIntermediaryDetails_SerIntermediaryType")]
    public string PropSerIntermediaryDetails_SerIntermediaryType { get; set; }
    [XmlElement(ElementName = "PropServiceTaxExemptionCategory_Component43")]
    public string PropServiceTaxExemptionCategory_Component43 { get; set; }
    [XmlElement(ElementName = "PropVerticalDtls_PrimaryVerticalCode")]
    public string PropVerticalDtls_PrimaryVerticalCode { get; set; }
    [XmlElement(ElementName = "PropVerticalDtls_PrimaryVerticalName")]
    public string PropVerticalDtls_PrimaryVerticalName { get; set; }
    [XmlElement(ElementName = "PropVerticalDtls_SecondaryVerticalCode")]
    public string PropVerticalDtls_SecondaryVerticalCode { get; set; }
    [XmlElement(ElementName = "PropVerticalDtls_SecondaryVerticalName")]
    public string PropVerticalDtls_SecondaryVerticalName { get; set; }
    [XmlElement(ElementName = "PropVerticalDtls_TertiaryVerticalCode")]
    public string PropVerticalDtls_TertiaryVerticalCode { get; set; }
    [XmlElement(ElementName = "PropVerticalDtls_TertiaryVerticalName")]
    public string PropVerticalDtls_TertiaryVerticalName { get; set; }
    [XmlElement(ElementName = "PropWarranty_DepartmentCode")]
    public string PropWarranty_DepartmentCode { get; set; }
    [XmlElement(ElementName = "PropWarranty_ProductCode")]
    public string PropWarranty_ProductCode { get; set; }
    [XmlElement(ElementName = "PropWarranty_SectionCode")]
    public string PropWarranty_SectionCode { get; set; }
}

[XmlRoot(ElementName = "TXT_PRODUCT_USERDATA")]
public class TXT_PRODUCT_USERDATA
{
    [XmlElement(ElementName = "WorkSheet")]
    public WorkSheet WorkSheet { get; set; }
}

[XmlRoot(ElementName = "HEADER")]
public class UnitedIndiaResponseHeader
{
    [XmlElement(ElementName = "TXT_ERR_MSG")]
    public string TXT_ERR_MSG { get; set; }
    [XmlElement(ElementName = "NUM_REFERENCE_NUMBER")]
    public string NUM_REFERENCE_NUMBER { get; set; }
    [XmlElement(ElementName = "TXT_WORKFLOW_ID")]
    public string TXT_WORKFLOW_ID { get; set; }
    [XmlElement(ElementName = "DAT_REFERENCE_DATE")]
    public string DAT_REFERENCE_DATE { get; set; }
    [XmlElement(ElementName = "TXT_CUST_POLICY_NO")]
    public string TXT_CUST_POLICY_NO { get; set; }
    [XmlElement(ElementName = "TXT_CUSTOMER_ID")]
    public string TXT_CUSTOMER_ID { get; set; }
    [XmlElement(ElementName = "NUM_OFFICE_CODE")]
    public string NUM_OFFICE_CODE { get; set; }
    [XmlElement(ElementName = "CUR_BASIC_OD_PREMIUM")]
    public string CUR_BASIC_OD_PREMIUM { get; set; }
    [XmlElement(ElementName = "CUR_BASIC_TP_PREMIUM")]
    public string CUR_BASIC_TP_PREMIUM { get; set; }
    [XmlElement(ElementName = "CUR_NET_TP_PREMIUM")]
    public string CUR_NET_TP_PREMIUM { get; set; }
    [XmlElement(ElementName = "CUR_NET_OD_PREMIUM")]
    public string CUR_NET_OD_PREMIUM { get; set; }
    [XmlElement(ElementName = "CUR_NET_FINAL_PREMIUM")]
    public string CUR_NET_FINAL_PREMIUM { get; set; }
    [XmlElement(ElementName = "CUR_FINAL_TOTAL_PREMIUM")]
    public string CUR_FINAL_TOTAL_PREMIUM { get; set; }
    [XmlElement(ElementName = "NUM_IEV_BASE_VALUE")]
    public string NUM_IEV_BASE_VALUE { get; set; }
    [XmlElement(ElementName = "TXT_STUMP_DUTY")]
    public string TXT_STUMP_DUTY { get; set; }
    [XmlElement(ElementName = "NUM_TOTAL_ADDITION_OF_PREMIUM")]
    public string NUM_TOTAL_ADDITION_OF_PREMIUM { get; set; }
    [XmlElement(ElementName = "NUM_TOTAL_ADDITION_OF_TP_PREM")]
    public string NUM_TOTAL_ADDITION_OF_TP_PREM { get; set; }
    [XmlElement(ElementName = "NUM_TOTAL_DEDUCTION_OF_PREMIUM")]
    public string NUM_TOTAL_DEDUCTION_OF_PREMIUM { get; set; }
    [XmlElement(ElementName = "NUM_TOTAL_DEDUCTION_OF_TP_PREM")]
    public string NUM_TOTAL_DEDUCTION_OF_TP_PREM { get; set; }
    [XmlElement(ElementName = "CUR_FINAL_NET_TP_PREMIUM")]
    public string CUR_FINAL_NET_TP_PREMIUM { get; set; }
    [XmlElement(ElementName = "CUR_FINAL_NET_OD_PREMIUM")]
    public string CUR_FINAL_NET_OD_PREMIUM { get; set; }
    [XmlElement(ElementName = "TXT_TRANSACTION_ID")]
    public string TXT_TRANSACTION_ID { get; set; }
    [XmlElement(ElementName = "CUR_TOTAL_PA_PREMIUM")]
    public string CUR_TOTAL_PA_PREMIUM { get; set; }
    [XmlElement(ElementName = "CUR_TOTAL_LL_PREMIUM")]
    public string CUR_TOTAL_LL_PREMIUM { get; set; }
    [XmlElement(ElementName = "TXT_AGENT_NAME")]
    public string TXT_AGENT_NAME { get; set; }
    [XmlElement(ElementName = "NUM_LICENCE_NUMBER")]
    public string NUM_LICENCE_NUMBER { get; set; }
    [XmlElement(ElementName = "NUM_TOTAL_SI")]
    public string NUM_TOTAL_SI { get; set; }
    [XmlElement(ElementName = "DAT_DATE_OF_EXPIRY_OF_POLICY")]
    public string DAT_DATE_OF_EXPIRY_OF_POLICY { get; set; }
    [XmlElement(ElementName = "TXT_IS_TOWING_COVERAVALABLE")]
    public string TXT_IS_TOWING_COVERAVALABLE { get; set; }
    [XmlElement(ElementName = "PropRisks_TowingCharges")]
    public string PropRisks_TowingCharges { get; set; }
    [XmlElement(ElementName = "TXT_CHASSIS_NUMBER")]
    public string TXT_CHASSIS_NUMBER { get; set; }
    [XmlElement(ElementName = "TXT_ENGINE_NUMBER")]
    public string TXT_ENGINE_NUMBER { get; set; }
    [XmlElement(ElementName = "YN_STAMP_DUTY_CHARGEABLE")]
    public string YN_STAMP_DUTY_CHARGEABLE { get; set; }
    [XmlElement(ElementName = "CUR_DIGITAL_DISCOUNT_AMOUNT")]
    public string CUR_DIGITAL_DISCOUNT_AMOUNT { get; set; }
    [XmlElement(ElementName = "CUR_DIGITAL_DISCOUNT_RATE")]
    public string CUR_DIGITAL_DISCOUNT_RATE { get; set; }
    [XmlElement(ElementName = "TXT_PRODUCT_USERDATA")]
    public TXT_PRODUCT_USERDATA TXT_PRODUCT_USERDATA { get; set; }
    [XmlElement(ElementName = "TXT_WORKSHEET")]
    public string TXT_WORKSHEET { get; set; }
    [XmlElement(ElementName = "CUR_FINAL_SERVICE_TAX")]
    public string CUR_FINAL_SERVICE_TAX { get; set; }
    [XmlElement(ElementName = "TXT_VAHAN_WARNING_TEXT")]
    public string TXT_VAHAN_WARNING_TEXT { get; set; }
}
[XmlRoot(ElementName = "GeneralProposal_FinancierDetails")]
public class GeneralProposal_FinancierDetails
{
    [XmlElement(ElementName = "ExtensionData")]
    public string ExtensionData { get; set; }
    [XmlElement(ElementName = "PropFinancierDetails_Address_Mandatary")]
    public string PropFinancierDetails_Address_Mandatary { get; set; }
    [XmlElement(ElementName = "PropFinancierDetails_AgreementType")]
    public string PropFinancierDetails_AgreementType { get; set; }
    [XmlElement(ElementName = "PropFinancierDetails_BranchName")]
    public string PropFinancierDetails_BranchName { get; set; }
    [XmlElement(ElementName = "PropFinancierDetails_CityCode")]
    public string PropFinancierDetails_CityCode { get; set; }
    [XmlElement(ElementName = "PropFinancierDetails_City_Mandatary")]
    public string PropFinancierDetails_City_Mandatary { get; set; }
    [XmlElement(ElementName = "PropFinancierDetails_DistrictCode")]
    public string PropFinancierDetails_DistrictCode { get; set; }
    [XmlElement(ElementName = "PropFinancierDetails_District_Mandatary")]
    public string PropFinancierDetails_District_Mandatary { get; set; }
    [XmlElement(ElementName = "PropFinancierDetails_FinancierCode_Mandatary")]
    public string PropFinancierDetails_FinancierCode_Mandatary { get; set; }
    [XmlElement(ElementName = "PropFinancierDetails_FinancierName")]
    public string PropFinancierDetails_FinancierName { get; set; }
    [XmlElement(ElementName = "PropFinancierDetails_IsDataDeleted")]
    public string PropFinancierDetails_IsDataDeleted { get; set; }
    [XmlElement(ElementName = "PropFinancierDetails_IsOldDataDeleted")]
    public string PropFinancierDetails_IsOldDataDeleted { get; set; }
    [XmlElement(ElementName = "PropFinancierDetails_Pincode_Mandatary")]
    public string PropFinancierDetails_Pincode_Mandatary { get; set; }
    [XmlElement(ElementName = "PropFinancierDetails_SrNo_Mandatary")]
    public string PropFinancierDetails_SrNo_Mandatary { get; set; }
    [XmlElement(ElementName = "PropFinancierDetails_StateCode")]
    public string PropFinancierDetails_StateCode { get; set; }
    [XmlElement(ElementName = "PropFinancierDetails_State_Mandatary")]
    public string PropFinancierDetails_State_Mandatary { get; set; }
}

[XmlRoot(ElementName = "TIMESPAN")]
public class UnitedIndiaResponseTimeSpan
{
    [XmlElement(ElementName = "REQUESTTS")]
    public string REQUESTTS { get; set; }
    [XmlElement(ElementName = "RESPONSETS")]
    public string RESPONSETS { get; set; }
    [XmlElement(ElementName = "RESPONSEIP")]
    public string RESPONSEIP { get; set; }
}

[XmlRoot(ElementName = "calculatePremiumResponse", Namespace = "http://ws.uiic.com/")]
public class CalculatePremiumResponse
{
    [System.Xml.Serialization.XmlElementAttribute(Namespace = "")]
    public @return @return { get; set; }
}
/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
public partial class @return
{
    public UnitedIndiaResponseRoot ROOT { get; set; }
}

/// <remarks/>
[System.SerializableAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
public partial class UnitedIndiaResponseRoot
{
    public UnitedIndiaResponseHeader HEADER { get; set; }
    public UnitedIndiaResponseTimeSpan TIMESPAN { get; set; }
}


[XmlRoot(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
public class UnitedIndiaResponseBody
{
    [XmlElement(ElementName = "calculatePremiumResponse", Namespace = "http://ws.uiic.com/")]
    public CalculatePremiumResponse CalculatePremiumResponse { get; set; }
}

[XmlRoot(ElementName = "Envelope", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
public class UnitedIndiaResponseEnvelope
{
    [XmlElement(ElementName = "Body", Namespace = "http://schemas.xmlsoap.org/soap/envelope/")]
    public UnitedIndiaResponseBody Body { get; set; }
    [XmlAttribute(AttributeName = "S", Namespace = "http://www.w3.org/2000/xmlns/")]
    public string S { get; set; }
}


