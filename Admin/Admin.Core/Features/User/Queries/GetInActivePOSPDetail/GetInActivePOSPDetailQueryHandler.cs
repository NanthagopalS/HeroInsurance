using Admin.Core.Contracts.Persistence;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Queries.GetInActivePOSPDetail
{
    public class GetInActivePOSPDetailQuery : IRequest<HeroResult<IEnumerable<GetInActivePOSPDetailVm>>>
    {
        public string? CriteriaType { get; set; }
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public int? PageIndex { get; set; }
    }
    public class GetInActivePOSPDetailQueryHandler : IRequestHandler<GetInActivePOSPDetailQuery, HeroResult<IEnumerable<GetInActivePOSPDetailVm>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        public GetInActivePOSPDetailQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<HeroResult<IEnumerable<GetInActivePOSPDetailVm>>> Handle(GetInActivePOSPDetailQuery request, CancellationToken cancellationToken)
        {
            //var roleDetailMapInput = _mapper.Map<RoleDetailInputModel>(request);
            var modelResult = await _userRepository.GetInActivePOSPDetail(request.CriteriaType, request.FromDate, request.ToDate, request.PageIndex, cancellationToken).ConfigureAwait(false);
            if (modelResult.Any())
            {
                var listUserRoleModel = _mapper.Map<IEnumerable<GetInActivePOSPDetailVm>>(modelResult);
                return HeroResult<IEnumerable<GetInActivePOSPDetailVm>>.Success(listUserRoleModel);
            }
            return HeroResult<IEnumerable<GetInActivePOSPDetailVm>>.Fail("No Record Found");
        }

    }
}
