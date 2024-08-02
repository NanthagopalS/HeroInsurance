using Admin.Core.Contracts.Persistence;
using Admin.Domain.Roles;
using AutoMapper;
using Admin.Core.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Admin.Core.Features.User.Commands.UpdateUserRoleMappingDetail
{
    public record UpdateUserRoleMappingDetailCommand : IRequest<HeroResult<bool>>
    {
        public string RoleId { get; set; }
        public string RoleTypeId { get; set; }
        public string RoleName { get; set; }
        public string BUId { get; set; }
        public string RoleLevelId { get; set; }
        public IList<UserRoleMappingDetailCommandUpdateModel> UserRoleMappingDetailCommandUpdate { get; set; }
    }
    public record UserRoleMappingDetailCommandUpdateModel
    {
        public bool AddPermission { get; set; }
        public bool EditPermission { get; set; }
        public bool ViewPermission { get; set; }
        public bool DeletePermission { get; set; }
        public bool DownloadPermission { get; set; }

        public class UpdateUserRoleMappingDetailCommandHandler : IRequestHandler<UpdateUserRoleMappingDetailCommand, HeroResult<bool>>
        {
            private readonly IUserRepository _insertUserRoleMappingRepository;
            private readonly IMapper _mapper;

            public UpdateUserRoleMappingDetailCommandHandler(IUserRepository insertUserRoleMappingRepository, IMapper mapper)
            {
                _insertUserRoleMappingRepository = insertUserRoleMappingRepository ?? throw new ArgumentNullException(nameof(insertUserRoleMappingRepository));
                _mapper = mapper;
            }

            public async Task<HeroResult<bool>> Handle(UpdateUserRoleMappingDetailCommand insertUserRoleMappingDetaillCommand, CancellationToken cancellationToken)
            {
                var UserRoleMappingDetailPermissionModel = _mapper.Map<UpdateUserRoleMappingDetailModel>(insertUserRoleMappingDetaillCommand);
                var result = await _insertUserRoleMappingRepository.UpdateUserRoleMappingDetail(UserRoleMappingDetailPermissionModel, cancellationToken);

                return HeroResult<bool>.Success(result);
            }

        }
    }
}
