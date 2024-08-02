﻿using AutoMapper;
using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Queries.GetRoleType;
using Admin.Core.Features.User.Queries.GetUserBreadcrumStatusDetail;
using Admin.Core.Features.User.Queries.GetUserDetail;
using Admin.Core.Features.User.Queries.UserDocument;
using Admin.Core.Features.User.Querries.GetMasterType;
using Admin.Domain.Roles;
using Admin.Domain.User;
using Admin.Core.Responses;
using MediatR;
using System.Threading;
namespace Admin.Core.Features.User.Queries.GetRolePermissionAll
{
    public record GetRoleQueryAllCommand : IRequest<HeroResult<IEnumerable<RoleSearchAllVM>>>;
   
    public  class GetRoleQueryAllHandler :IRequestHandler<GetRoleQueryAllCommand, HeroResult<IEnumerable<RoleSearchAllVM>>>
    {
        
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        public GetRoleQueryAllHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<HeroResult<IEnumerable<RoleSearchAllVM>>> Handle(GetRoleQueryAllCommand request, CancellationToken cancellationToken)
        {
          
            var modelResult = await _userRepository.GetPermissionMappingAll(cancellationToken).ConfigureAwait(false);
            if (modelResult.Any())
            {
                var listInsurerModel = _mapper.Map<IEnumerable<RoleSearchAllVM>>(modelResult);
                return HeroResult<IEnumerable<RoleSearchAllVM>>.Success(listInsurerModel);
            }
            return HeroResult<IEnumerable<RoleSearchAllVM>>.Fail("No Record Found");
        }
    }
}
