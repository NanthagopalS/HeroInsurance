using Admin.Core.Contracts.Persistence;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Queries.CheckUserExistOrNot
{
    public record CheckUserExistOrNotQuery : IRequest<HeroResult<IEnumerable<CheckUserExistOrNotVm>>>
    {
        public string? UserId { get; set; }

        public string? EmpId { get; set; }
        public string? MobileNo { get; set; }
        public string? EmailId { get; set; }


    }
    public class CheckUserExistOrNotQueryHandler : IRequestHandler<CheckUserExistOrNotQuery, HeroResult<IEnumerable<CheckUserExistOrNotVm>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="pospRepository"></param>
        /// <param name="mapper"></param>
        public CheckUserExistOrNotQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<HeroResult<IEnumerable<CheckUserExistOrNotVm>>> Handle(CheckUserExistOrNotQuery request, CancellationToken cancellationToken)
        {
            var modelResult = await _userRepository.CheckUserExistOrNot(request.UserId,request.EmpId, request.MobileNo, request.EmailId, cancellationToken).ConfigureAwait(false);
            if (modelResult.Any())
            {
                var listUserRoleModel = _mapper.Map<IEnumerable<CheckUserExistOrNotVm>>(modelResult);
                return HeroResult<IEnumerable<CheckUserExistOrNotVm>>.Success(listUserRoleModel);
            }
            return HeroResult<IEnumerable<CheckUserExistOrNotVm>>.Fail("No Record Found");
        }
    }
}
