using AutoMapper;
using Admin.Core.Contracts.Persistence;
using Admin.Domain.User;
using Admin.Core.Responses;
using MediatR;
using Admin.Domain.Roles;
using System.Text.Json.Serialization;
using ThirdPartyUtilities.Abstraction;

namespace Admin.Core.Features.User.Commands.InsertUserRoleMappingDetaill
{
    public record InsertUserRoleMappingDetaillCommand : IRequest<HeroResult<bool>>
    {
        public string? UserName { get; set; }
        public string? EmpID { get; set; }
        public string? MobileNo { get; set; }
        public string? EmailId { get; set; }
        public string? Gender { get; set; }
        public string? DOB { get; set; }

        [JsonIgnore]
        public string? ProfilePictureID { get; set; }
        public string? RoleTypeId { get; set; }
        public string? BUId { get; set; }
        public string? RoleId { get; set; }
        public string? ReportingIdentityRoleId { get; set; }
        public string? ReportingUserId { get; set; }
        public string? CategoryId { get; set; }
        public string? CreatedBy { get; set; }

        [JsonIgnore]
        public byte[]? ImageStream { get; set; }

    }
    public class InsertUserRoleMappingDetaillCommandHandler : IRequestHandler<InsertUserRoleMappingDetaillCommand, HeroResult<bool>>
    {
        private readonly IUserRepository _userMappingRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="userPersonalDetailRepository"></param>
        /// <param name="mapper"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public InsertUserRoleMappingDetaillCommandHandler(IUserRepository userMappingRepository, IMapper mapper,IEmailService email)
        {
            _userMappingRepository = userMappingRepository ?? throw new ArgumentNullException(nameof(userMappingRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _emailService = email ?? throw new ArgumentNullException(nameof(email));
        }

        /// <summary>
        /// Handler
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<HeroResult<bool>> Handle(InsertUserRoleMappingDetaillCommand userMappingCommand, CancellationToken cancellationToken)
        {
            var userMappingModel = _mapper.Map<UserRoleModel>(userMappingCommand);
            var result = await _userMappingRepository.InsertUserRoleMappingDetail(userMappingModel, cancellationToken);
            bool res = false;
            if(result is not null && result.Any())
            {
             res = await _emailService.SendUserCredentialMail(userMappingCommand.EmailId, result, cancellationToken);
            }
            return HeroResult<bool>.Success(res);
        }
    }
}

































//using AutoMapper;
//using Admin.Core.Contracts.Persistence;
//using Admin.Domain.User;
//using Admin.Core.Responses;
//using MediatR;
//using Admin.Domain.Roles;

//namespace Admin.Core.Features.User.Commands.InsertUserRoleMappingDetaill
//{
//    public record InsertUserRoleMappingDetaillCommand : IRequest<HeroResult<bool>>
//    {
//        public string RoleTypeId { get; set; }
//        public string RoleName { get; set; }
//        public string BUId { get; set; }
//        public string RoleLevelId { get; set; }
//        public IList<UserRoleMappingDetailCommandInsertModel> UserRoleMappingDetailCommandInsertList { get; set; }
//    }
//    public record UserRoleMappingDetailCommandInsertModel
//    {
//        public bool AddPermission { get; set; }
//        public bool EditPermission { get; set; }
//        public bool ViewPermission { get; set; }
//        public bool DeletePermission { get; set; }
//        public bool DownloadPermission { get; set; }

//        public class InsertUserRoleMappingDetaillCommandHandler : IRequestHandler<InsertUserRoleMappingDetaillCommand, HeroResult<bool>>
//        {
//            private readonly IUserRepository _insertUserRoleMappingRepository;
//            private readonly IMapper _mapper;

//            public InsertUserRoleMappingDetaillCommandHandler(IUserRepository insertUserRoleMappingRepository, IMapper mapper)
//            {
//                _insertUserRoleMappingRepository = insertUserRoleMappingRepository ?? throw new ArgumentNullException(nameof(insertUserRoleMappingRepository));
//                _mapper = mapper;
//            }

//            public async Task<HeroResult<bool>> Handle(InsertUserRoleMappingDetaillCommand insertUserRoleMappingDetaillCommand, CancellationToken cancellationToken)
//            {
//                var UserRoleMappingDetailPermissionModel = _mapper.Map<UserRoleMappingDetailPermissionModel>(insertUserRoleMappingDetaillCommand);
//                var result = await _insertUserRoleMappingRepository.InsertUserRoleMappingDetail(UserRoleMappingDetailPermissionModel, cancellationToken);

//                return HeroResult<bool>.Success(result);
//            }

//        }
//    }
//}
