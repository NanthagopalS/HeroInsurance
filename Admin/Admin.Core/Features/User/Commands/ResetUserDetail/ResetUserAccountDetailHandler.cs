using Admin.Core.Contracts.Persistence;
using Admin.Core.Responses;
using AutoMapper;
using MediatR;
using ThirdPartyUtilities.Abstraction;
using ThirdPartyUtilities.Models.PanRejectionModels;

namespace Admin.Core.Features.User.Commands.ResetUserDetail
{
    public class ResetUserAccountDetailQuery : IRequest<HeroResult<ResetUserAccountDetailVM>>
    {
        public string UserId { get; set; }
        public string PanRejectionReasonsCSV { get; set; }
    }
    public class ResetUserAccountDetailHandler : IRequestHandler<ResetUserAccountDetailQuery, HeroResult<ResetUserAccountDetailVM>>
    {
        private readonly IUserRepository _userCreationRepository;
        private readonly IMapper _mapper;
        public readonly IMongoDBService _mongoDBService;
        public readonly IEmailService _emailService;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="userCreationRepository"></param>
        /// <param name="mapper"></param>
        /// <param name="mongoDBService"></param>
        /// <param name="emailService"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ResetUserAccountDetailHandler(IUserRepository userCreationRepository, IMapper mapper, IMongoDBService mongoDBService, IEmailService emailService)
        {
            _userCreationRepository = userCreationRepository ?? throw new ArgumentNullException(nameof(userCreationRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mongoDBService = mongoDBService ?? throw new ArgumentNullException(nameof(mongoDBService));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }


        public async Task<HeroResult<ResetUserAccountDetailVM>> Handle(ResetUserAccountDetailQuery resetUserAccountDetailQuery, CancellationToken cancellationToken)
        {

            var result = await _userCreationRepository.ResetUserIdDetail(resetUserAccountDetailQuery, cancellationToken);

            if (result is not null)
            {
                if (result.documentDetail.Count() > 0)
                {
                    foreach (var item in result.documentDetail)
                    {
                        if (item.DocumentType == "POSP_DOCUMENT")
                        {
                            _mongoDBService.DeletePOSPDocument(item.DocumentId);
                        }
                        else if (item.DocumentType == "POSP_EXAMDOCUMENT")
                        {
                            _mongoDBService.DeletePOSPExamCertificate(item.DocumentId);
                        }
                    }
                }
                var userDetails = result.userDetail.First();
                // code to send email 
                ResetUserDetailsForPanVerificationEmailModel resetUserDetailsForPanVerification = new ResetUserDetailsForPanVerificationEmailModel()
                {
                    EmailId = userDetails.EmailId,
                    MobileNo = userDetails.MobileNo,
                    UserId = userDetails.UserId,
                    UserName = userDetails.UserName
                };
                await _emailService.SendPanVerificationFailedEmail(resetUserDetailsForPanVerification, cancellationToken);
                // add code to send SMS once Template is approved from DLT

                var userDetailById = _mapper.Map<ResetUserAccountDetailVM>(result);
                userDetailById.userDetail.First().ResetSuccessfull = true;
                return HeroResult<ResetUserAccountDetailVM>.Success(userDetailById);
            }
            return HeroResult<ResetUserAccountDetailVM>.Fail("User not deleted");
        }
    }
}
