using AutoMapper;
using Identity.Core.Contracts.Persistence;
using Identity.Core.Responses;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Identity.Core.Features.Authenticate.Commands.ResetPasswordAdmin
{
    public class ResetPasswordAdminQuery : IRequest<HeroResult<ResetPasswordAdminVm>>
    {
        [Required]
        public string UserId { get; set; }
    }
    public class ResetPasswordAdminQueryHandler : IRequestHandler<ResetPasswordAdminQuery, HeroResult<ResetPasswordAdminVm>>
    {
        private readonly IAuthenticateRepository _authenticateRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="authenticateRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ResetPasswordAdminQueryHandler(IAuthenticateRepository authenticateRepository, IMapper mapper)
        {
            _authenticateRepository = authenticateRepository ?? throw new ArgumentNullException(nameof(authenticateRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        /// <summary>
        /// Handle
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<HeroResult<ResetPasswordAdminVm>> Handle(ResetPasswordAdminQuery request, CancellationToken cancellationToken)
        {
            var authResponse = await _authenticateRepository.ResetPasswordAdmin(request.UserId, cancellationToken).ConfigureAwait(false);
            if (authResponse != null)
            {
                var result = _mapper.Map<ResetPasswordAdminVm>(authResponse);
                return HeroResult<ResetPasswordAdminVm>.Success(result);
            }
            return HeroResult<ResetPasswordAdminVm>.Fail("Invalid Email or Failed to send Email");
        }
    }
    }
