using AutoMapper;
using Insurance.Core.Features.Bajaj.Command.QuoteConfirm;
using Insurance.Core.Features.Bajaj.Queries.GetQuote;
using Insurance.Core.Features.Chola.Command.QuoteConfirm;
using Insurance.Core.Features.Chola.Queries.GetPaymentWrapper;
using Insurance.Core.Features.Chola.Queries.GetQuote;
using Insurance.Core.Features.GoDigit.Command.QuoteConfirm;
using Insurance.Core.Features.GoDigit.Queries.GetQuote;
using Insurance.Core.Features.HDFC.Command.QuoteConfirm;
using Insurance.Core.Features.HDFC.Queries.GetPaymentStatus;
using Insurance.Core.Features.HDFC.Queries.GetQuote;
using Insurance.Core.Features.ICICI.Command.CreateIMBroker;
using Insurance.Core.Features.ICICI.Command.QuoteConfirm;
using Insurance.Core.Features.ICICI.Queries.ConfirmDetails;
using Insurance.Core.Features.ICICI.Queries.GetQuote;
using Insurance.Core.Features.IFFCO.Command.QuoteConfirm;
using Insurance.Core.Features.IFFCO.Queries.GetQuote;
using Insurance.Core.Features.Magma.Queries.GetQuote;
using Insurance.Core.Features.Oriental.Command.QuoteConfirm;
using Insurance.Core.Features.Oriental.Queries.GetQuote;
using Insurance.Core.Features.Quote.Command.InsertQuoteConfirmRequest;
using Insurance.Core.Features.Quote.Command.InsertQuoteRequest;
using Insurance.Core.Features.Quote.Command.SaveForLater;
using Insurance.Core.Features.Quote.Query.GetPreviousPolicyDetails;
using Insurance.Core.Features.Quote.Query.GetProposalDetails;
using Insurance.Core.Features.Reliance.Command.GetQuote;
using Insurance.Core.Features.Reliance.Command.QuoteConfirm;
using Insurance.Core.Features.Reliance.Queries.GetPaymentWrapper;
using Insurance.Core.Features.TATA.Command.QuoteConfirm;
using Insurance.Core.Features.TATA.Command.VerifyAadharOTP;
using Insurance.Core.Features.TATA.Queries.GetPaymentStatus;
using Insurance.Core.Features.TATA.Queries.GetQuote;
using Insurance.Core.Features.UnitedIndia.Command.QuoteConfirm;
using Insurance.Core.Features.UnitedIndia.Queries;
using Insurance.Core.Features.UnitedIndia.Queries.GetQuote;
using Insurance.Core.Models;
using Insurance.Domain.Chola;
using Insurance.Domain.HDFC;
using Insurance.Domain.ICICI;
using Insurance.Domain.Quote;
using Insurance.Domain.Reliance;
using Insurance.Domain.TATA;
using Insurance.Core.Features.Quote.Command.InsertQuoteConfirmRequest;
using Insurance.Core.Features.Oriental.Queries.GetQuote;
using Insurance.Core.Features.Oriental.Command.QuoteConfirm;
using Insurance.Core.Features.TATA.Command.VerifyAadharOTP;
using Insurance.Domain.UnitedIndia;

namespace Insurance.API.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<QuoteBaseCommand, GetBajajQuoteQuery>();
        CreateMap<QuoteBaseCommand, GetGoDigitQuery>();
        CreateMap<QuoteBaseCommand, GetIciciQuoteQuery>();
        CreateMap<QuoteBaseCommand, GetHdfcQuoteQuery>();
        CreateMap<QuoteBaseCommand, GetCholaQuery>();
        CreateMap<QuoteBaseCommand, GetRelianceQuery>();
        CreateMap<QuoteBaseCommand, GetTATAQuoteQuery>();
        CreateMap<QuoteBaseCommand, GetMagmaQuoteQuery>();
        CreateMap<QuoteBaseCommand, GetIFFCOQuery>();
        CreateMap<QuoteBaseCommand, GetOrientalQuoteQuery>();
        CreateMap<QuoteBaseCommand, GetUnitedIndiaQuoteQuery>();
        CreateMap<QuoteBaseCommand, InsertQuoteRequestCommand>();
        CreateMap<QuoteConfirmRequestModel, InsertQuoteConfirmRequestCommand>();
        CreateMap<QuoteBaseCommand, GetCommercialHDFCQuoteQuery>();
        
        CreateMap<QuoteBaseCommand, GetCommercialICICIQuoteQuery>();

        CreateMap<GetGoDigitQuery, GetPolicyDatesQuery>()
           .ForMember(d => d.RegistrationYear, o => o.MapFrom(s => s.RegistrationYear))
           .ForMember(d => d.VehicleType, o => o.MapFrom(s => s.VehicleTypeId))
           .ForMember(d => d.IsPreviousPolicy, o => o.MapFrom(s => GetIsPreviousPolicy(s.PreviousPolicy)))
           .ForMember(d => d.ODPolicyExpiry, o => o.MapFrom(s => GetSAODExpiryDate(s.PreviousPolicy)))
           .ForMember(d => d.TPPolicyExpiry, o => o.MapFrom(s => GetTPExpiryDate(s.PreviousPolicy)))
           .ForMember(d => d.PreviousPolicyTypeId, o => o.MapFrom(s => GetPreviousPolicyTypeId(s.PreviousPolicy)));
        CreateMap<GetCholaQuery, GetPolicyDatesQuery>()
          .ForMember(d => d.RegistrationYear, o => o.MapFrom(s => s.RegistrationYear))
          .ForMember(d => d.VehicleType, o => o.MapFrom(s => s.VehicleTypeId))
          .ForMember(d => d.IsPreviousPolicy, o => o.MapFrom(s => GetIsPreviousPolicy(s.PreviousPolicy)))
          .ForMember(d => d.ODPolicyExpiry, o => o.MapFrom(s => GetSAODExpiryDate(s.PreviousPolicy)))
          .ForMember(d => d.TPPolicyExpiry, o => o.MapFrom(s => GetTPExpiryDate(s.PreviousPolicy)))
          .ForMember(d => d.PreviousPolicyTypeId, o => o.MapFrom(s => GetPreviousPolicyTypeId(s.PreviousPolicy)));
        CreateMap<GetIciciQuoteQuery, GetPolicyDatesQuery>()
          .ForMember(d => d.RegistrationYear, o => o.MapFrom(s => s.RegistrationYear))
          .ForMember(d => d.VehicleType, o => o.MapFrom(s => s.VehicleTypeId))
          .ForMember(d => d.IsBrandNewVehicle, o => o.MapFrom(s => s.IsBrandNewVehicle))
          .ForMember(d => d.IsPreviousPolicy, o => o.MapFrom(s => GetIsPreviousPolicy(s.PreviousPolicy)))
          .ForMember(d => d.ODPolicyExpiry, o => o.MapFrom(s => GetSAODExpiryDate(s.PreviousPolicy)))
          .ForMember(d => d.TPPolicyExpiry, o => o.MapFrom(s => GetTPExpiryDate(s.PreviousPolicy)))
          .ForMember(d => d.PreviousPolicyTypeId, o => o.MapFrom(s => GetPreviousPolicyTypeId(s.PreviousPolicy)));
        CreateMap<GetBajajQuoteQuery, GetPolicyDatesQuery>()
          .ForMember(d => d.RegistrationYear, o => o.MapFrom(s => s.RegistrationYear))
          .ForMember(d => d.VehicleType, o => o.MapFrom(s => s.VehicleTypeId))
          .ForMember(d => d.IsPreviousPolicy, o => o.MapFrom(s => GetIsPreviousPolicy(s.PreviousPolicy)))
          .ForMember(d => d.ODPolicyExpiry, o => o.MapFrom(s => GetSAODExpiryDate(s.PreviousPolicy)))
          .ForMember(d => d.TPPolicyExpiry, o => o.MapFrom(s => GetTPExpiryDate(s.PreviousPolicy)))
          .ForMember(d => d.PreviousPolicyTypeId, o => o.MapFrom(s => GetPreviousPolicyTypeId(s.PreviousPolicy)));
        CreateMap<GetHdfcQuoteQuery, GetPolicyDatesQuery>()
          .ForMember(d => d.RegistrationYear, o => o.MapFrom(s => s.RegistrationYear))
          .ForMember(d => d.VehicleType, o => o.MapFrom(s => s.VehicleTypeId))
          .ForMember(d => d.IsPreviousPolicy, o => o.MapFrom(s => GetIsPreviousPolicy(s.PreviousPolicy)))
          .ForMember(d => d.ODPolicyExpiry, o => o.MapFrom(s => GetSAODExpiryDate(s.PreviousPolicy)))
          .ForMember(d => d.TPPolicyExpiry, o => o.MapFrom(s => GetTPExpiryDate(s.PreviousPolicy)))
          .ForMember(d => d.PreviousPolicyTypeId, o => o.MapFrom(s => GetPreviousPolicyTypeId(s.PreviousPolicy)));
        CreateMap<GetIFFCOQuery, GetPolicyDatesQuery>()
          .ForMember(d => d.RegistrationYear, o => o.MapFrom(s => s.RegistrationYear))
          .ForMember(d => d.VehicleType, o => o.MapFrom(s => s.VehicleTypeId))
          .ForMember(d => d.IsPreviousPolicy, o => o.MapFrom(s => GetIsPreviousPolicy(s.PreviousPolicy)))
          .ForMember(d => d.ODPolicyExpiry, o => o.MapFrom(s => GetSAODExpiryDate(s.PreviousPolicy)))
          .ForMember(d => d.TPPolicyExpiry, o => o.MapFrom(s => GetTPExpiryDate(s.PreviousPolicy)))
          .ForMember(d => d.TPPolicyExpiry, o => o.MapFrom(s => GetTPExpiryDate(s.PreviousPolicy)))
          .ForMember(d => d.PreviousPolicyTypeId, o => o.MapFrom(s => GetPreviousPolicyTypeId(s.PreviousPolicy)));
        CreateMap<GetTATAQuoteQuery, GetPolicyDatesQuery>()
          .ForMember(d => d.RegistrationYear, o => o.MapFrom(s => s.RegistrationYear))
          .ForMember(d => d.VehicleType, o => o.MapFrom(s => s.VehicleTypeId))
          .ForMember(d => d.IsPreviousPolicy, o => o.MapFrom(s => GetIsPreviousPolicy(s.PreviousPolicy)))
          .ForMember(d => d.ODPolicyExpiry, o => o.MapFrom(s => GetSAODExpiryDate(s.PreviousPolicy)))
          .ForMember(d => d.TPPolicyExpiry, o => o.MapFrom(s => GetTPExpiryDate(s.PreviousPolicy)))
          .ForMember(d => d.TPPolicyExpiry, o => o.MapFrom(s => GetTPExpiryDate(s.PreviousPolicy)))
          .ForMember(d => d.PreviousPolicyTypeId, o => o.MapFrom(s => GetPreviousPolicyTypeId(s.PreviousPolicy)));
        CreateMap<GetOrientalQuoteQuery, GetPolicyDatesQuery>()
          .ForMember(d => d.RegistrationYear, o => o.MapFrom(s => s.RegistrationYear))
          .ForMember(d => d.VehicleType, o => o.MapFrom(s => s.VehicleTypeId))
          .ForMember(d => d.IsPreviousPolicy, o => o.MapFrom(s => GetIsPreviousPolicy(s.PreviousPolicy)))
          .ForMember(d => d.ODPolicyExpiry, o => o.MapFrom(s => GetSAODExpiryDate(s.PreviousPolicy)))
          .ForMember(d => d.TPPolicyExpiry, o => o.MapFrom(s => GetTPExpiryDate(s.PreviousPolicy)))
          .ForMember(d => d.TPPolicyExpiry, o => o.MapFrom(s => GetTPExpiryDate(s.PreviousPolicy)))
          .ForMember(d => d.PreviousPolicyTypeId, o => o.MapFrom(s => GetPreviousPolicyTypeId(s.PreviousPolicy)));
        CreateMap<GetUnitedIndiaQuoteQuery, GetPolicyDatesQuery>()
          .ForMember(d => d.RegistrationYear, o => o.MapFrom(s => s.RegistrationYear))
          .ForMember(d => d.VehicleType, o => o.MapFrom(s => s.VehicleTypeId))
          .ForMember(d => d.IsPreviousPolicy, o => o.MapFrom(s => GetIsPreviousPolicy(s.PreviousPolicy)))
          .ForMember(d => d.ODPolicyExpiry, o => o.MapFrom(s => GetSAODExpiryDate(s.PreviousPolicy)))
          .ForMember(d => d.TPPolicyExpiry, o => o.MapFrom(s => GetTPExpiryDate(s.PreviousPolicy)))
          .ForMember(d => d.TPPolicyExpiry, o => o.MapFrom(s => GetTPExpiryDate(s.PreviousPolicy)))
          .ForMember(d => d.PreviousPolicyTypeId, o => o.MapFrom(s => GetPreviousPolicyTypeId(s.PreviousPolicy)));
        CreateMap<QuoteConfirmRequestModel, GodigitQuoteConfirmCommand>();
        CreateMap<GodigitQuoteConfirmCommand, QuoteConfirmRequestModel>();
        CreateMap<QuoteConfirmRequestModel, BajajQuoteConfirmCommand>();
        CreateMap<BajajQuoteConfirmCommand, QuoteConfirmRequestModel>();
        CreateMap<QuoteConfirmRequestModel, QuoteConfirmCommand>();
        CreateMap<QuoteConfirmCommand, QuoteConfirmRequestModel>();
        CreateMap<QuoteConfirmRequestModel, ICICIQuoteConfirmCommand>();
        CreateMap<ICICIQuoteConfirmCommand, QuoteConfirmRequestModel>();
        CreateMap<QuoteConfirmRequestModel, HDFCQuoteConfirmCommand>();
        CreateMap<HDFCQuoteConfirmCommand, QuoteConfirmRequestModel>();
        CreateMap<QuoteConfirmRequestModel, CholaQuoteConfirmCommand>();
        CreateMap<CholaQuoteConfirmCommand, QuoteConfirmRequestModel>();
        CreateMap<QuoteConfirmRequestModel, IffcoQuoteConfirmCommand>();
        CreateMap<IffcoQuoteConfirmCommand, QuoteConfirmRequestModel>();
        CreateMap<ICICICreateIMBrokerModel, CreateIMBrokerCommand>();
        CreateMap<HDFCPaymentResponseModel, GetPaymentQuery>();
        CreateMap<CholaPaymentResponseModel, GetPaymentWrapperQuery>();
        CreateMap<ReliancePaymentResponseModel, GetReliancePaymentWrapperQuery>();
        CreateMap<QuoteConfirmRequestModel, SaveForLaterCommand>();
        CreateMap<QuoteConfirmRequestModel, TATAQuoteConfirmCommand>();
        CreateMap<QuoteConfirmRequestModel, OrientalQuoteConfirmCommand>();
        CreateMap<OrientalQuoteConfirmCommand, QuoteConfirmRequestModel>();
        CreateMap<TATAQuoteConfirmCommand, QuoteConfirmRequestModel>();
        CreateMap<SaveForLaterCommand, QuoteConfirmRequestModel>();
        CreateMap<QuoteConfirmRequestModel, RelianceQuoteConfirmCommand>(); // Rel
        CreateMap<RelianceQuoteConfirmCommand, QuoteConfirmRequestModel>(); // Rel
        CreateMap<QuoteConfirmRequestModel, UnitedIndiaQuoteConfirmCommand>();
        CreateMap<UnitedIndiaQuoteConfirmCommand, QuoteConfirmRequestModel>();
        CreateMap<QuoteBaseCommand, GetRelianceQuery>();
        CreateMap<GetRelianceQuery, GetPolicyDatesQuery>()
         .ForMember(d => d.RegistrationYear, o => o.MapFrom(s => s.RegistrationYear))
         .ForMember(d => d.VehicleType, o => o.MapFrom(s => s.VehicleTypeId))
         .ForMember(d => d.IsPreviousPolicy, o => o.MapFrom(s => GetIsPreviousPolicy(s.PreviousPolicy)))
         .ForMember(d => d.ODPolicyExpiry, o => o.MapFrom(s => GetSAODExpiryDate(s.PreviousPolicy)))
         .ForMember(d => d.TPPolicyExpiry, o => o.MapFrom(s => GetTPExpiryDate(s.PreviousPolicy)))
         .ForMember(d => d.PreviousPolicyTypeId, o => o.MapFrom(s => GetPreviousPolicyTypeId(s.PreviousPolicy)));
        CreateMap<TATAPaymentResponseModel, TATAVerifyPaymentStatus>();
        CreateMap<TATAVerifyPaymentStatus, TATAPaymentResponseModel>();
        CreateMap<POAAadharOTPSubmitRequestModel, TATAVerifyAadharOTPCommand>();
        CreateMap<QuoteBaseCommand, GetCommercialIFFCOQuoteQuery>();
        CreateMap<QuoteBaseCommand, GetCVRelianceQuery>();
        CreateMap<GetProposalDetailsForPaymentResponceModel, GetProposalDetailsForPaymentVm>();
        CreateMap<InitiatePaymentRequestDto, GetPaymentStatusQuery>();
        CreateMap<GetPaymentStatusQuery, InitiatePaymentRequestDto>();
    }

    private static bool GetIsPreviousPolicy(PreviousPolicyModel previousPolicyModel)
    {
        if (previousPolicyModel != null)
        {
            return previousPolicyModel.IsPreviousPolicy;
        }
        return false;
    }

    private static string GetSAODExpiryDate(PreviousPolicyModel previousPolicyModel)
    {
        if (previousPolicyModel != null)
        {
            return previousPolicyModel.SAODPolicyExpiryDate;
        }
        return string.Empty;
    }
    private static string GetTPExpiryDate(PreviousPolicyModel previousPolicyModel)
    {
        if (previousPolicyModel != null)
        {
            return previousPolicyModel.TPPolicyExpiryDate;
        }
        return string.Empty;
    }

    private static string GetPreviousPolicyTypeId(PreviousPolicyModel previousPolicyModel)
    {
        if (previousPolicyModel != null)
        {
            return previousPolicyModel.PreviousPolicyTypeId;
        }
        return string.Empty;
    }
}
