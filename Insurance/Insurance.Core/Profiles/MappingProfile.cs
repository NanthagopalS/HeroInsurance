using AutoMapper;
using Insurance.Core.Features.AllReportAndMIS.Query.BusinessSummery;
using Insurance.Core.Features.AllReportAndMIS.Query.GetAllReportAndMIS;
using Insurance.Core.Features.AllReportAndMIS.RequestandResponseH;
using Insurance.Core.Features.Bajaj.Command.CheckBreakinPinStatus;
using Insurance.Core.Features.Bajaj.Queries.GetPayment;
using Insurance.Core.Features.Bajaj.Queries.GetPaymentLink;
using Insurance.Core.Features.Chola.Queries.GetPaymentLink;
using Insurance.Core.Features.Chola.Queries.GetPaymentStatus;
using Insurance.Core.Features.Chola.Queries.GetPaymentWrapper;
using Insurance.Core.Features.Customer.Queries.GetCustomersList;
using Insurance.Core.Features.Customer.Queries.GetParticularCustomerDetailById;
using Insurance.Core.Features.Customer.Queries.GetRenewalDetailsById;
using Insurance.Core.Features.GoDigit.Queries.GetPaymentCKYC;
using Insurance.Core.Features.GoDigit.Queries.GetPaymentLink;
using Insurance.Core.Features.HDFC.Command.CreatePOSP;
using Insurance.Core.Features.HDFC.Command.CreateProposal;
using Insurance.Core.Features.HDFC.Queries.GetCKYCPOA;
using Insurance.Core.Features.HDFC.Queries.GetCKYCPOAStatus;
using Insurance.Core.Features.HDFC.Queries.GetPaymentLink;
using Insurance.Core.Features.ICICI.Queries.GetPaymentLink;
using Insurance.Core.Features.IFFCO.Command.SavePaymentStatus;
using Insurance.Core.Features.IFFCO.Queries.GetPaymentLink;
using Insurance.Core.Features.InsuranceMaster;
using Insurance.Core.Features.InsuranceMaster.Command.LeadDetails;
using Insurance.Core.Features.InsuranceMaster.Queries.GetQuoteMaster;
using Insurance.Core.Features.InsuranceMaster.Queries.GetVehicleDetails;
using Insurance.Core.Features.Leads;
using Insurance.Core.Features.Leads.GetPaymentStatus;
using Insurance.Core.Features.ManualPolicy.Command;
using Insurance.Core.Features.ManualPolicy.Query;
using Insurance.Core.Features.ManualPolicy.Query.GetManualPolicyNature;
using Insurance.Core.Features.Quote.Query.GetLeadDetails;
using Insurance.Core.Features.Quote.Query.GetPaymentStatus;
using Insurance.Core.Features.Quote.Query.GetPolicyDocumentPDF;
using Insurance.Core.Features.Reliance.Queries.GetPaymentWrapper;
using Insurance.Core.Features.Quote.Query.GetPreviousCoverMaster;
using Insurance.Domain;
using Insurance.Domain.AllReportAndMIS;
using Insurance.Domain.AllReportAndMIS.BusinessSummery;
using Insurance.Domain.AllReportAndMIS.BusinessSummerym;
using Insurance.Domain.Bajaj;
using Insurance.Domain.Chola;
using Insurance.Domain.Customer;
using Insurance.Domain.GoDigit;
using Insurance.Domain.HDFC;
using Insurance.Domain.ICICI;
using Insurance.Domain.IFFCO;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.Leads;
using Insurance.Domain.ManualPolicy;
using Insurance.Domain.Quote;
using Insurance.Domain.Reliance;
using Insurance.Domain.TATA;
using Insurance.Core.Features.InsuranceMaster.Queries.GetInsuranceMaster;
using Insurance.Domain.Oriental;
using Insurance.Core.Features.TATA.Queries.GetPaymentLink;
using Insurance.Core.Features.HDFC.Queries.GetQuote;
using Insurance.Domain.CommercialMaster;
using Insurance.Core.Features.CommercialMaster.Query.GetCommercialMaster;
using Insurance.Core.Features.CommercialMaster.Query.GetCommercialVehicleOtherDetailsAskOptions;
using Insurance.Domain.CommercialVehicle;
using Insurance.Core.Features.Reliance.Command.GetQuote;
using Insurance.Domain.ICICI.Response;
using Insurance.Core.Features.IFFCO.Queries.GetQuote;
using Insurance.Core.Features.Reliance.Queries.GetPaymentLink;
using Insurance.Domain.UnitedIndia;

namespace Hero.Core.Profiles;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<VariantModel, VariantVm>();
        CreateMap<StateModel, StateVm>();
        CreateMap<InsurerModel, InsurerVm>();
        CreateMap<NCBModel, NCBVm>();
        CreateMap<InsuranceTypeModel, InsuranceTypeVm>();
        CreateMap<YearModel, YearVm>();
        CreateMap<FuelModel, FuelVm>();
        CreateMap<VehicleRegistrationModel, VehicleDetailVm>();
        CreateMap<PreviousPolicyTypeModel, PreviousPolicyTypeVm>();
        CreateMap<CreateLeadCommand, CreateLeadModel>();
        CreateMap<QuoteMasterModel, QuoteMasterVm>();
        CreateMap<GetPaymentCKYCQuery, GodigitPaymentCKYCReqModel>();
        CreateMap<PaymentCKCYResponseModel, PaymentDetailsVm>();
        CreateMap<GetPaymentStatusQuery, PaymentStatusRequestModel>();
        CreateMap<GetPolicyDocumentPdfQuery, PaymentStatusRequestModel>();
        CreateMap<GetCKYCPOAQuery, HDFCCkycPOAModel>();
        CreateMap<GetCKYCPOAStatusQuery, HDFCCkycPOAModel>();
        CreateMap<PaymentCKCYResponseModel, QuoteResponseModel>()
         .ForMember(d => d.ApplicationId, o => o.MapFrom(s => s.ApplicationId))
         .ForMember(d => d.GrossPremium, o => o.MapFrom(s => s.Amount))
         .ForMember(d => d.PaymentStatus, o => o.MapFrom(s => s.PaymentStatus))
         .ForMember(d => d.PaymentTransactionNumber, o => o.MapFrom(s => s.PaymentTransactionNumber))
         .ForMember(d => d.Type, o => o.MapFrom(s => "UPDATE"));
        CreateMap<QuoteConfirmRequestModel, QuoteConfirmDataModel>();

        CreateMap<ProposalRequest, ProposalLeadDetails>()
         .ForMember(d => d.FirstName, o => o.MapFrom(s => s.PersonalDetails.firstName))
         .ForMember(d => d.MiddleName, o => o.MapFrom(s => s.PersonalDetails.middleName))
         .ForMember(d => d.LastName, o => o.MapFrom(s => s.PersonalDetails.lastName))
         .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.PersonalDetails.mobile))
         .ForMember(d => d.Email, o => o.MapFrom(s => s.PersonalDetails.emailId))
         .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.PersonalDetails.companyName))
         .ForMember(d => d.GSTNo, o => o.MapFrom(s => s.PersonalDetails.gstno))
         .ForMember(d => d.DOB, o => o.MapFrom(s => s.PersonalDetails.dateOfBirth))
         .ForMember(d => d.Gender, o => o.MapFrom(s => s.PersonalDetails.gender))
         .ForMember(d => d.NomineeFirstName, o => o.MapFrom(s => s.NomineeDetails.nomineeFirstName))
         .ForMember(d => d.NomineeLastName, o => o.MapFrom(s => s.NomineeDetails.nomineeLastName))
         .ForMember(d => d.NomineeDOB, o => o.MapFrom(s => s.NomineeDetails.nomineeDateOfBirth))
         .ForMember(d => d.NomineeRelation, o => o.MapFrom(s => s.NomineeDetails.nomineeRelation))
         .ForMember(d => d.AddressType, o => o.MapFrom(s => "PRIMARY"))
         .ForMember(d => d.Perm_Address1, o => o.MapFrom(s => s.AddressDetails.street))
         .ForMember(d => d.Perm_Pincode, o => o.MapFrom(s => s.AddressDetails.pincode))
         .ForMember(d => d.Perm_City, o => o.MapFrom(s => s.AddressDetails.city))
         .ForMember(d => d.Perm_State, o => o.MapFrom(s => s.AddressDetails.state))
         .ForMember(d => d.IsFinancier, o => o.MapFrom(s => s.VehicleDetails.isFinancier))
         .ForMember(d => d.FinancierName, o => o.MapFrom(s => s.VehicleDetails.financer))
         .ForMember(d => d.EngineNumber, o => o.MapFrom(s => s.VehicleDetails.engineNumber))
         .ForMember(d => d.ChassisNumber, o => o.MapFrom(s => s.VehicleDetails.chassisNumber));

        CreateMap<ICICIProposalRequest, ProposalLeadDetails>()
         .ForMember(d => d.FirstName, o => o.MapFrom(s => s.PersonalDetails.customerName))
         .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.PersonalDetails.mobile))
         .ForMember(d => d.Email, o => o.MapFrom(s => s.PersonalDetails.emailId))
         .ForMember(d => d.DOB, o => o.MapFrom(s => s.PersonalDetails.dateOfBirth))
         .ForMember(d => d.Gender, o => o.MapFrom(s => s.PersonalDetails.gender))
         .ForMember(d => d.NomineeFirstName, o => o.MapFrom(s => s.NomineeDetails.nomineeName))
         .ForMember(d => d.NomineeAge, o => o.MapFrom(s => s.NomineeDetails.nomineeAge))
         .ForMember(d => d.NomineeRelation, o => o.MapFrom(s => s.NomineeDetails.nomineeRelation))
         .ForMember(d => d.AddressType, o => o.MapFrom(s => "PRIMARY"))
         .ForMember(d => d.Perm_Address1, o => o.MapFrom(s => s.AddressDetails.addressLine1))
         .ForMember(d => d.Perm_Pincode, o => o.MapFrom(s => s.AddressDetails.pincode))
         .ForMember(d => d.Perm_City, o => o.MapFrom(s => s.AddressDetails.city))
         .ForMember(d => d.Perm_State, o => o.MapFrom(s => s.AddressDetails.state))
         .ForMember(d => d.IsFinancier, o => o.MapFrom(s => s.VehicleDetails.isFinancier))
         .ForMember(d => d.FinancierName, o => o.MapFrom(s => s.VehicleDetails.financer))
         .ForMember(d => d.EngineNumber, o => o.MapFrom(s => s.VehicleDetails.engineNumber))
         .ForMember(d => d.ChassisNumber, o => o.MapFrom(s => s.VehicleDetails.chassisNumber));

        CreateMap<BajajProposalRequest, ProposalLeadDetails>()
         .ForMember(d => d.FirstName, o => o.MapFrom(s => s.PersonalDetails.firstName))
         .ForMember(d => d.MiddleName, o => o.MapFrom(s => s.PersonalDetails.middleName))
         .ForMember(d => d.LastName, o => o.MapFrom(s => s.PersonalDetails.lastName))
         .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.PersonalDetails.mobile))
         .ForMember(d => d.Email, o => o.MapFrom(s => s.PersonalDetails.emailId))
         .ForMember(d => d.DOB, o => o.MapFrom(s => s.PersonalDetails.dateOfBirth))
         .ForMember(d => d.Gender, o => o.MapFrom(s => s.PersonalDetails.gender))
         .ForMember(d => d.NomineeFirstName, o => o.MapFrom(s => s.NomineeDetails.nomineeFirstName))
         .ForMember(d => d.NomineeLastName, o => o.MapFrom(s => s.NomineeDetails.nomineeLastName))
         .ForMember(d => d.NomineeRelation, o => o.MapFrom(s => s.NomineeDetails.nomineeRelation))
         .ForMember(d => d.AddressType, o => o.MapFrom(s => "PRIMARY"))
         .ForMember(d => d.Perm_Address1, o => o.MapFrom(s => s.AddressDetails.addLine1))
         .ForMember(d => d.Perm_Pincode, o => o.MapFrom(s => s.AddressDetails.pincode))
         .ForMember(d => d.Perm_City, o => o.MapFrom(s => s.AddressDetails.city))
         .ForMember(d => d.Perm_State, o => o.MapFrom(s => s.AddressDetails.state))
         .ForMember(d => d.IsFinancier, o => o.MapFrom(s => s.VehicleDetails.isFinancier))
         .ForMember(d => d.FinancierName, o => o.MapFrom(s => s.VehicleDetails.financer))
         .ForMember(d => d.EngineNumber, o => o.MapFrom(s => s.VehicleDetails.engineNumber))
         .ForMember(d => d.ChassisNumber, o => o.MapFrom(s => s.VehicleDetails.chassisNumber));

        CreateMap<HDFCProposalRequest, ProposalLeadDetails>()
         .ForMember(d => d.FirstName, o => o.MapFrom(s => s.PersonalDetails.firstName))
         .ForMember(d => d.LastName, o => o.MapFrom(s => s.PersonalDetails.lastName))
         .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.PersonalDetails.mobile))
         .ForMember(d => d.Email, o => o.MapFrom(s => s.PersonalDetails.emailId))
         .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.PersonalDetails.companyName))
         .ForMember(d => d.GSTNo, o => o.MapFrom(s => s.PersonalDetails.gstNo))
         .ForMember(d => d.DOB, o => o.MapFrom(s => s.PersonalDetails.dateOfBirth))
         .ForMember(d => d.Gender, o => o.MapFrom(s => s.PersonalDetails.gender))
         .ForMember(d => d.NomineeFirstName, o => o.MapFrom(s => s.NomineeDetails.nomineeName))
         .ForMember(d => d.NomineeAge, o => o.MapFrom(s => s.NomineeDetails.nomineeAge))
         .ForMember(d => d.NomineeRelation, o => o.MapFrom(s => s.NomineeDetails.nomineeRelation))
         .ForMember(d => d.AddressType, o => o.MapFrom(s => "PRIMARY"))
         .ForMember(d => d.Perm_Address1, o => o.MapFrom(s => s.AddressDetails.perm_address))
         .ForMember(d => d.Perm_Pincode, o => o.MapFrom(s => s.AddressDetails.perm_pincode))
         .ForMember(d => d.Perm_City, o => o.MapFrom(s => s.AddressDetails.perm_city))
         .ForMember(d => d.Perm_State, o => o.MapFrom(s => s.AddressDetails.perm_state))
         .ForMember(d => d.IsFinancier, o => o.MapFrom(s => s.VehicleDetails.isFinancier))
         .ForMember(d => d.FinancierName, o => o.MapFrom(s => s.VehicleDetails.financer))
         .ForMember(d => d.FinancierBranch, o => o.MapFrom(s => s.VehicleDetails.branch))
         .ForMember(d => d.EngineNumber, o => o.MapFrom(s => s.VehicleDetails.engineNumber))
         .ForMember(d => d.ChassisNumber, o => o.MapFrom(s => s.VehicleDetails.chassisNumber));

        CreateMap<CholaProposalRequest, ProposalLeadDetails>()
         .ForMember(d => d.FirstName, o => o.MapFrom(s => s.PersonalDetails.firstName))
         .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.PersonalDetails.companyName))
         .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.PersonalDetails.mobile))
         .ForMember(d => d.Email, o => o.MapFrom(s => s.PersonalDetails.emailId))
         .ForMember(d => d.DOB, o => o.MapFrom(s => s.PersonalDetails.dateOfBirth))
         .ForMember(d => d.Gender, o => o.MapFrom(s => s.PersonalDetails.gender))
         .ForMember(d => d.AddressType, o => o.MapFrom(s => "PRIMARY"))
         .ForMember(d => d.Perm_Address1, o => o.MapFrom(s => s.AddressDetails.addressLine1))
         .ForMember(d => d.Perm_Pincode, o => o.MapFrom(s => s.AddressDetails.pincode))
         .ForMember(d => d.Perm_State, o => o.MapFrom(s => s.AddressDetails.state))
         .ForMember(d => d.Perm_City, o => o.MapFrom(s => s.AddressDetails.city))
         .ForMember(d => d.NomineeFirstName, o => o.MapFrom(s => s.NomineeDetails.nomineeName))
         .ForMember(d => d.NomineeDOB, o => o.MapFrom(s => s.NomineeDetails.nomineeDateOfBirth))
         .ForMember(d => d.NomineeRelation, o => o.MapFrom(s => s.NomineeDetails.nomineeRelation))
         .ForMember(d => d.EngineNumber, o => o.MapFrom(s => s.VehicleDetails.engineNumber))
         .ForMember(d => d.ChassisNumber, o => o.MapFrom(s => s.VehicleDetails.chassisNumber))
         .ForMember(d => d.IsFinancier, o => o.MapFrom(s => s.VehicleDetails.isFinancier))
         .ForMember(d => d.FinancierName, o => o.MapFrom(s => s.VehicleDetails.financer))
         .ForMember(d => d.FinancierBranch, o => o.MapFrom(s => s.VehicleDetails.branch));

        CreateMap<RelianceProposalRequest, ProposalLeadDetails>()
         .ForMember(d => d.FirstName, o => o.MapFrom(s => s.PersonalDetails.firstName))
         .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.PersonalDetails.companyName))
         .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.PersonalDetails.mobile))
         .ForMember(d => d.Email, o => o.MapFrom(s => s.PersonalDetails.emailId))
         .ForMember(d => d.DOB, o => o.MapFrom(s => s.PersonalDetails.dateOfBirth))
         .ForMember(d => d.Gender, o => o.MapFrom(s => s.PersonalDetails.gender))
         .ForMember(d => d.AddressType, o => o.MapFrom(s => "PRIMARY"))
         .ForMember(d => d.Perm_Address1, o => o.MapFrom(s => s.AddressDetails.addressLine1))
         .ForMember(d => d.Perm_Pincode, o => o.MapFrom(s => s.AddressDetails.pincode))
         .ForMember(d => d.Perm_State, o => o.MapFrom(s => s.AddressDetails.state))
         .ForMember(d => d.Perm_City, o => o.MapFrom(s => s.AddressDetails.city))
         .ForMember(d => d.NomineeFirstName, o => o.MapFrom(s => s.NomineeDetails.nomineeName))
         .ForMember(d => d.NomineeDOB, o => o.MapFrom(s => s.NomineeDetails.nomineeDateOfBirth))
         .ForMember(d => d.NomineeRelation, o => o.MapFrom(s => s.NomineeDetails.nomineeRelation))
         .ForMember(d => d.EngineNumber, o => o.MapFrom(s => s.VehicleDetails.engineNumber))
         .ForMember(d => d.ChassisNumber, o => o.MapFrom(s => s.VehicleDetails.chassisNumber))
         .ForMember(d => d.IsFinancier, o => o.MapFrom(s => s.VehicleDetails.isFinancier))
         .ForMember(d => d.FinancierName, o => o.MapFrom(s => s.VehicleDetails.financer))
         .ForMember(d => d.FinancierBranch, o => o.MapFrom(s => s.VehicleDetails.branch));

        CreateMap<IFFCOProposalDynamicDetails, ProposalLeadDetails>()
          .ForMember(d => d.FirstName, o => o.MapFrom(s => s.PersonalDetails.firstName))
          .ForMember(d => d.LastName, o => o.MapFrom(s => s.PersonalDetails.lastName))
          .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.PersonalDetails.companyName))
          .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.PersonalDetails.mobile))
          .ForMember(d => d.Email, o => o.MapFrom(s => s.PersonalDetails.emailId))
          .ForMember(d => d.DOB, o => o.MapFrom(s => s.PersonalDetails.dateOfBirth))
          .ForMember(d => d.Gender, o => o.MapFrom(s => s.PersonalDetails.gender))
          .ForMember(d => d.Perm_Address1, o => o.MapFrom(s => s.AddressDetails.street))
          .ForMember(d => d.Perm_Pincode, o => o.MapFrom(s => s.AddressDetails.pincode))
          .ForMember(d => d.Perm_State, o => o.MapFrom(s => s.AddressDetails.state))
          .ForMember(d => d.Perm_City, o => o.MapFrom(s => s.AddressDetails.city))
          .ForMember(d => d.NomineeFirstName, o => o.MapFrom(s => s.NomineeDetails.nomineeName))
          .ForMember(d => d.NomineeRelation, o => o.MapFrom(s => s.NomineeDetails.nomineeRelation))
          .ForMember(d => d.EngineNumber, o => o.MapFrom(s => s.VehicleDetails.engineNumber))
          .ForMember(d => d.ChassisNumber, o => o.MapFrom(s => s.VehicleDetails.chassisNumber))
          .ForMember(d => d.IsFinancier, o => o.MapFrom(s => s.VehicleDetails.isFinancier))
          .ForMember(d => d.FinancierName, o => o.MapFrom(s => s.VehicleDetails.financer));

        CreateMap<GetLeadDetailsModel, GetLeadDetailsVm>();
        CreateMap<GetLeadManagementDetailModel, GetLeadManagementDetailVm>(); // For Lead Management detail
        CreateMap<GetLeadManagementDetailQuery, GetLeadManagementDetailModel>();
        CreateMap<BajajBreakinPinStatusCommand, BreakinPinStatusModel>();
        CreateMap<HDFCCreateProposalCommand, ProposalRequestModel>();
        CreateMap<GetPaymentQuery, QuoteResponseModel>()
            .ForMember(d => d.Type, o => o.MapFrom(s => "UPDATE"))
            .ForMember(d => d.PolicyNumber, o => o.MapFrom(s => s.ReferenceNo))
            .ForMember(d => d.PaymentTransactionNumber, o => o.MapFrom(s => s.TransactionNo))
            .ForMember(d => d.ApplicationId, o => o.MapFrom(s => s.TransactionId))
            .ForMember(d => d.CKYCStatus, o => o.MapFrom(s => "DONE"))
            .ForMember(d => d.LeadId, o => o.MapFrom(s => s.LeadId))
            .ForMember(d => d.IsTP, o => o.MapFrom(s => s.IsTP));

        CreateMap<Insurance.Core.Features.HDFC.Queries.GetPaymentStatus.GetPaymentQuery, QuoteResponseModel>()
            .ForMember(d => d.Type, o => o.MapFrom(s => "UPDATE"))
            .ForMember(d => d.TransactionID, o => o.MapFrom(s => s.QuoteTransactionId))
            .ForMember(d => d.ApplicationId, o => o.MapFrom(s => s.TransactionNo))
            .ForMember(d => d.DocumentId, o => o.MapFrom(s => s.CheckSum))
            .ForMember(d => d.PaymentTransactionNumber, o => o.MapFrom(s => s.TransctionRefNo))
            .ForMember(d => d.CKYCStatus, o => o.MapFrom(s => "DONE"))
            .ForMember(d => d.BankName, o => o.MapFrom(s => s.BankCode))
            .ForMember(d => d.BankPaymentRefNum, o => o.MapFrom(s => s.BankReferenceNo))
            .ForMember(d => d.PaymentMode, o => o.MapFrom(s => s.PaymentMode))
            .ForMember(d => d.GrossPremium, o => o.MapFrom(s => s.TxnAmount))
            .ForMember(d => d.PaymentDate, o => o.MapFrom(s => s.TransactionDate));
        CreateMap<GetCustomersResponseModel, GetCustomersListVm>();
        CreateMap<GetRenewalDetailsByIdResponceModel, GetRenewalDetailsByIdVm>();
        CreateMap<CreatePOSPCommand, HDFCCreateIMBrokerRequestDto>();

        CreateMap<GetCustomerLeadDetailModel, GetParticularLeadDetailByIdVm>();
        CreateMap<BusinessSummeryResponceModel, BusinessSummeryVm>();
        CreateMap<GetCustomersListQuery, GetCustomersListRequest>();
        CreateMap<GetGoDigitPaymentLinkQuery, PaymentStatusRequestModel>();
        CreateMap<GetICICIPaymentLinkQuery, PaymentStatusRequestModel>();
        CreateMap<GetBajajPaymentLinkQuery, PaymentStatusRequestModel>();
        CreateMap<GetHDFCPaymentLinkQuery, PaymentStatusRequestModel>();
        CreateMap<GetCholaPaymentLinkQuery, PaymentStatusRequestModel>();
        CreateMap<GetCholaPaymentStatusQuery, QuoteResponseModel>();
        CreateMap<GetIFFCOPaymentLinkQuery, PaymentStatusRequestModel>();
        CreateMap<GetPaymentWrapperQuery, QuoteResponseModel>()
            .ForMember(d => d.Type, o => o.MapFrom(s => "UPDATE"))
            .ForMember(d => d.TransactionID, o => o.MapFrom(s => s.QuoteTransactionId))
            .ForMember(d => d.ApplicationId, o => o.MapFrom(s => s.UniqueTxnID))
            .ForMember(d => d.GrossPremium, o => o.MapFrom(s => s.TxnAmount))
            .ForMember(d => d.PaymentTransactionNumber, o => o.MapFrom(s => s.TxnReferenceNo))
            .ForMember(d => d.CKYCStatus, o => o.MapFrom(s => "DONE"))
            .ForMember(d => d.BankName, o => o.MapFrom(s => s.BankID))
            .ForMember(d => d.BankPaymentRefNum, o => o.MapFrom(s => s.BankReferenceNo))
            .ForMember(d => d.PaymentMode, o => o.MapFrom(s => "PG"))
            .ForMember(d => d.PaymentDate, o => o.MapFrom(s => s.TxnDate));
        CreateMap<AllReportAndMISResponseModel, AllReportAndMISVm>();
        CreateMap<AllReportAndMISQuery, AllReportAndMISRequestModel>();
        CreateMap<PaymentStatusListResponceModel, GetPaymentStatusListVm>();
        CreateMap<GetReliancePaymentWrapperQuery, QuoteResponseModel>()
            .ForMember(d => d.Type, o => o.MapFrom(s => "UPDATE"))
            .ForMember(d => d.TransactionID, o => o.MapFrom(s => s.QuoteTransactionId))
            .ForMember(d => d.PaymentTransactionNumber, o => o.MapFrom(s => s.TransactionNumber))
            .ForMember(d => d.CKYCStatus, o => o.MapFrom(s => "DONE"))
            .ForMember(d => d.PaymentMode, o => o.MapFrom(s => "PG"));
        CreateMap<SavePaymentStatusCommand, IFFCOPaymentResponseModel>();
        CreateMap<IFFCOPaymentResponseModel, SavePaymentStatusCommand>();
        CreateMap<BusinessSummeryResponceModel, BusinessSummeryVm>();
        CreateMap<GetPreviousCoverResponseModel, PreviousCoverVm>();


        CreateMap<TATAProposalRequest, ProposalLeadDetails>()
         .ForMember(d => d.FirstName, o => o.MapFrom(s => s.PersonalDetails.firstName))
         .ForMember(d => d.LastName, o => o.MapFrom(s => s.PersonalDetails.lastName))
         .ForMember(d => d.DOB, o => o.MapFrom(s => s.PersonalDetails.dateOfBirth))
         .ForMember(d => d.Gender, o => o.MapFrom(s => s.PersonalDetails.gender))
         .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.PersonalDetails.mobile))
         .ForMember(d => d.Email, o => o.MapFrom(s => s.PersonalDetails.emailId))
         .ForMember(d => d.PanNumber, o => o.MapFrom(s => s.PersonalDetails.panNumber))
         .ForMember(d => d.NomineeFirstName, o => o.MapFrom(s => s.NomineeDetails.nomineeName))
         .ForMember(d => d.NomineeRelation, o => o.MapFrom(s => s.NomineeDetails.nomineeRelation))
         .ForMember(d => d.NomineeAge, o => o.MapFrom(s => s.NomineeDetails.nomineeAge))
         .ForMember(d => d.Perm_Address1, o => o.MapFrom(s => s.AddressDetails.addressLine1))
         .ForMember(d => d.Perm_Pincode, o => o.MapFrom(s => s.AddressDetails.pincode))
         .ForMember(d => d.IsFinancier, o => o.MapFrom(s => s.VehicleDetails.isFinancier))
         .ForMember(d => d.FinancierName, o => o.MapFrom(s => s.VehicleDetails.financer))
         .ForMember(d => d.ChassisNumber, o => o.MapFrom(s => s.VehicleDetails.chassisNumber))
         .ForMember(d => d.EngineNumber, o => o.MapFrom(s => s.VehicleDetails.engineNumber))
         .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.PersonalDetails.companyName))
         .ForMember(d => d.GSTNo, o => o.MapFrom(s => s.PersonalDetails.gstno));
        CreateMap<RequestandResponseModel, RequestandResponseVM>();
        CreateMap<ManualPolicyReponseModel, ManualPolicyUploadCommandVm>();
        CreateMap<GetManualPolicyListModel, GetManualPolicyListVM>();
        CreateMap<GetManualPolicyNatureResponseModel, GetManualPolicyNatureVm>();
        CreateMap<InsuranceMasterDataModel, GetInsuranceMasterVm>();

        CreateMap<OrientalProposalDynamicDetail, ProposalLeadDetails>()
         .ForMember(d => d.FirstName, o => o.MapFrom(s => s.PersonalDetails.customerName))
         .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.PersonalDetails.companyName))
         .ForMember(d => d.DOB, o => o.MapFrom(s => s.PersonalDetails.dateOfBirth))
         .ForMember(d => d.Gender, o => o.MapFrom(s => s.PersonalDetails.gender))
         .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.PersonalDetails.mobile))
         .ForMember(d => d.Email, o => o.MapFrom(s => s.PersonalDetails.emailId))
         .ForMember(d => d.NomineeFirstName, o => o.MapFrom(s => s.NomineeDetails.nomineeName))
         .ForMember(d => d.NomineeRelation, o => o.MapFrom(s => s.NomineeDetails.nomineeRelation))
         .ForMember(d => d.NomineeDOB, o => o.MapFrom(s => s.NomineeDetails.nomineeDateOfBirth))
         .ForMember(d => d.Perm_Address1, o => o.MapFrom(s => s.AddressDetails.addressLine1))
         .ForMember(d => d.Perm_Pincode, o => o.MapFrom(s => s.AddressDetails.pincode))
         .ForMember(d => d.Perm_City, o => o.MapFrom(s => s.AddressDetails.city))
         .ForMember(d => d.Perm_State, o => o.MapFrom(s => s.AddressDetails.state))
         .ForMember(d => d.IsFinancier, o => o.MapFrom(s => s.VehicleDetails.isFinancier))
         .ForMember(d => d.FinancierName, o => o.MapFrom(s => s.VehicleDetails.financer))
         .ForMember(d => d.ChassisNumber, o => o.MapFrom(s => s.VehicleDetails.chassisNumber))
         .ForMember(d => d.EngineNumber, o => o.MapFrom(s => s.VehicleDetails.engineNumber))
         .ForMember(d => d.VehicleColour, o => o.MapFrom(s => s.VehicleDetails.vehicleColour));
        CreateMap<TATAGetPaymentLinkQuery, PaymentStatusRequestModel>();
        CreateMap<GetCommercialHDFCQuoteQuery, GetHdfcQuoteQuery>();
        CreateMap<CommercialVehicleCategory, CommercialVehicleCategoryVm>();
        CreateMap<CommercialVehicleAskAdditionalsDetailsModel, GetCommercialVehicleOtherDetailsAskOptionsVm>();

        CreateMap<GetCommercialIFFCOQuoteQuery, GetIFFCOQuery>();
        CreateMap<GetCVRelianceQuery, GetRelianceQuery>();
        CreateMap<cvRiskdetails, RiskDetails>();
        CreateMap<Generalinformation, GeneralInformation>();
        CreateMap<GetReliancePaymentLinkQuery, PaymentStatusRequestModel>();
        CreateMap<ICICICVResponseDto, ICICIResponseDto>()
                 .ForMember(d => d.riskDetails, o => o.MapFrom(s => s.riskDetails))
                 .ForMember(d => d.generalInformation, o => o.MapFrom(s => s.generalInformation))
                 .ForMember(d => d.finalPremium, o => o.MapFrom(s => s.premiumDetails.finalPremium))
                 .ForMember(d => d.packagePremium, o => o.MapFrom(s => s.premiumDetails.packagePremium))
                 .ForMember(d => d.totalOwnDamagePremium, o => o.MapFrom(s => s.premiumDetails.totalOwnDamagePremium))
                 .ForMember(d => d.totalLiabilityPremium, o => o.MapFrom(s => s.premiumDetails.totalLiabilityPremium))
                 .ForMember(d => d.specialDiscount, o => o.MapFrom(s => s.premiumDetails.specialDiscount))
                 .ForMember(d => d.totalTax, o => o.MapFrom(s => s.premiumDetails.totalTax))
                 .ForMember(d => d.message, o => o.MapFrom(s => s.message))
                 .ForMember(d => d.status, o => o.MapFrom(s => s.status))
                 .ForMember(d => d.correlationId, o => o.MapFrom(s => s.correlationId))
                 .ForMember(d => d.isApprovalRequired, o => o.MapFrom(s => s.isApprovalRequired))
                 .ForMember(d => d.proposalStatus, o => o.MapFrom(s => s.proposalStatus))
                 .ForMember(d => d.breakingFlag, o => o.MapFrom(s => s.breakingFlag))
                 .ForMember(d => d.isQuoteDeviation, o => o.MapFrom(s => s.isQuoteDeviation))
                 .ForMember(d => d.deviationMessage, o => o.MapFrom(s => s.deviationMessage));
        CreateMap<UnitedProposalDynamicDetail, ProposalLeadDetails>()
              .ForMember(d => d.FirstName, o => o.MapFrom(s => s.PersonalDetails.customerName))
              .ForMember(d => d.CompanyName, o => o.MapFrom(s => s.PersonalDetails.companyName))
              .ForMember(d => d.PhoneNumber, o => o.MapFrom(s => s.PersonalDetails.mobile))
              .ForMember(d => d.Email, o => o.MapFrom(s => s.PersonalDetails.emailId))
              .ForMember(d => d.DOB, o => o.MapFrom(s => s.PersonalDetails.dateOfBirth))
              .ForMember(d => d.Gender, o => o.MapFrom(s => s.PersonalDetails.gender))
              .ForMember(d => d.PanNumber, o => o.MapFrom(s => s.PersonalDetails.panNumber))
              .ForMember(d => d.Perm_Address1, o => o.MapFrom(s => s.AddressDetails.addressLine1))
              .ForMember(d => d.NomineeFirstName, o => o.MapFrom(s => s.NomineeDetails.nomineeName))
              .ForMember(d => d.NomineeRelation, o => o.MapFrom(s => s.NomineeDetails.nomineeRelation))
              .ForMember(d => d.EngineNumber, o => o.MapFrom(s => s.VehicleDetails.engineNumber))
              .ForMember(d => d.ChassisNumber, o => o.MapFrom(s => s.VehicleDetails.chassisNumber))
              .ForMember(d => d.IsFinancier, o => o.MapFrom(s => s.VehicleDetails.isFinancier))
              .ForMember(d => d.FinancierName, o => o.MapFrom(s => s.VehicleDetails.financer))
              .ForMember(d => d.FinancierBranch, o => o.MapFrom(s => s.VehicleDetails.branch));
    }
}
