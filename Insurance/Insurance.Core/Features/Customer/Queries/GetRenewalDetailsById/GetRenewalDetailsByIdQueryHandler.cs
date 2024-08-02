using AutoMapper;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Responses;
using MediatR;
using Microsoft.Extensions.Configuration;
using NETCore.MailKit.Core;
using ThirdPartyUtilities.Abstraction;
using static Org.BouncyCastle.Math.EC.ECCurve;
using ThirdPartyUtilities.Helpers;
using ThirdPartyUtilities.Models.SMS;

namespace Insurance.Core.Features.Customer.Queries.GetRenewalDetailsById
{
    public record GetRenewalDetailsByIdQuery : IRequest<HeroResult<GetRenewalDetailsByIdVm>>
    {
        public string LeadId { get; set; }
    }
    public class GetRenewalDetailsByIdQueryHandler : IRequestHandler<GetRenewalDetailsByIdQuery, HeroResult<GetRenewalDetailsByIdVm>>
    {
        private readonly ICustomerRepository _benifitsRepository;
        private readonly IMapper _mapper;
        private readonly ThirdPartyUtilities.Abstraction.IEmailService _emailService;
        private readonly ISmsService _smsService;
        private readonly IConfiguration _confg;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="quoteRepository"></param>
        /// <param name="mapper"></param>
        public GetRenewalDetailsByIdQueryHandler(ICustomerRepository quoteRepository, IMapper mapper, ThirdPartyUtilities.Abstraction.IEmailService emailService,ISmsService smsService,IConfiguration config)
        {
            _benifitsRepository = quoteRepository;
            _mapper = mapper;
            _emailService = emailService;
            _smsService = smsService;
            _confg = config;
        }

        public async Task<HeroResult<GetRenewalDetailsByIdVm>> Handle(GetRenewalDetailsByIdQuery request, CancellationToken cancellationToken)
        {
            var benifitsDetailResult = await _benifitsRepository.GetRenewalDetailsById(request,cancellationToken).ConfigureAwait(false);
            if (benifitsDetailResult!=null)
            {
                var renewalDetails = _mapper.Map<GetRenewalDetailsByIdVm>(benifitsDetailResult);
                string contactSupportNumber = _confg.GetSection("Hero").GetSection("supportNumber").Value.ToString();
                string renewalUrl = "";
                RenewalSMSModel renewalSMSModel = new RenewalSMSModel
                {
                    mobileNumber = renewalDetails.PhoneNumber,
                    daysLeft = renewalDetails.Days,
                    vechicleMaker = renewalDetails.MakeName,
                    vechicleModel = renewalDetails.ModelName,
                    leadName = renewalDetails.LeadName,
                    insuranceName = renewalDetails.InsuranceName,
                    renewalURL = renewalUrl,
                    vechicleVariant = renewalDetails.VariantName
                };
                _emailService.SendEmailForRenewalsNotification(renewalDetails.Email, renewalDetails.InsuranceType, renewalDetails.LeadName, contactSupportNumber, renewalDetails.PolicyNumber, cancellationToken);
                _smsService.SendSMSForRenewalNotification(renewalSMSModel, cancellationToken);
                return HeroResult<GetRenewalDetailsByIdVm>.Success(renewalDetails);
            }

            return HeroResult<GetRenewalDetailsByIdVm>.Fail("No Record Found");
        }
    }
}
