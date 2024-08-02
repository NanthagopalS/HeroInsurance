using AutoMapper;
using Identity.Core.Features.Authenticate;
using Identity.Core.Features.Authenticate.Commands.ResetPasswordAdmin;
using Identity.Core.Features.Banner.Commands.InsertBanner;
using Identity.Core.Features.Benefits.Commands.BenefitsDetail;
using Identity.Core.Features.Registration.Commands.AuthenticateAdmin;
using Identity.Core.Features.Registration.Commands.CreateAdmin;
using Identity.Core.Features.Registration.Commands.SendOTP;
using Identity.Core.Features.Registration.Commands.UpdateAdmin;
using Identity.Core.Features.User.Commands.BUDetails;
using Identity.Core.Features.User.Commands.BUInsert;
using Identity.Core.Features.User.Commands.BUUpdate;
using Identity.Core.Features.User.Commands.CategoryInsert;
using Identity.Core.Features.User.Commands.InsertBenifitsDetail;
using Identity.Core.Features.User.Commands.ReuploadDocument;
using Identity.Core.Features.User.Commands.RoleModulePermission;
using Identity.Core.Features.User.Commands.RoleModuleUpdatePermission;
using Identity.Core.Features.User.Commands.SendCompletedRegisterationMail;
using Identity.Core.Features.User.Commands.UpdateUserPasswordFromUserLinkCommand;
using Identity.Core.Features.User.Commands.UserAddressDetail;
using Identity.Core.Features.User.Commands.UserBankDetail;
using Identity.Core.Features.User.Commands.UserCreation;
using Identity.Core.Features.User.Commands.UserDocumentUpload;
using Identity.Core.Features.User.Commands.UserEmailId;
using Identity.Core.Features.User.Commands.UserInquiryDetail;
using Identity.Core.Features.User.Commands.UserMappingInsert;
using Identity.Core.Features.User.Commands.UserPersonalDetail;
using Identity.Core.Features.User.Commands.UserProfilePicture;
using Identity.Core.Features.User.Commands.UserRoleGetAllMapping;
using Identity.Core.Features.User.Commands.UserRoleGetMapping;
using Identity.Core.Features.User.Commands.UserRoleMapping;
using Identity.Core.Features.User.Commands.UserRoleSearch;
using Identity.Core.Features.User.Commands.UserRoleUpdateModel;
using Identity.Core.Features.User.Commands.VerifyEmail;
using Identity.Core.Features.User.Queries.GetAllRelationshipManager;
using Identity.Core.Features.User.Queries.GetBenefitsDetail;
using Identity.Core.Features.User.Queries.GetErrorCode;
using Identity.Core.Features.User.Queries.GetModel;
using Identity.Core.Features.User.Queries.GetPOSPConfigurationDetail;
using Identity.Core.Features.User.Queries.GetPOSPSourceType;
using Identity.Core.Features.User.Queries.GetRoleBULevel;
using Identity.Core.Features.User.Queries.GetRoleLevel;
using Identity.Core.Features.User.Queries.GetRolePermission;
using Identity.Core.Features.User.Queries.GetRolePermissionAll;
using Identity.Core.Features.User.Queries.GetRoleType;
using Identity.Core.Features.User.Queries.GetSourcedByUserList;
using Identity.Core.Features.User.Queries.GetStateCitybyPincode;
using Identity.Core.Features.User.Queries.GetUserBreadcrumStatusDetail;
using Identity.Core.Features.User.Queries.GetUserDetail;
using Identity.Core.Features.User.Queries.GetUserListForDepartmentTagging;
using Identity.Core.Features.User.Queries.GetUserPersonalVerificationDetail;
using Identity.Core.Features.User.Queries.GetUserProfileDetail;
using Identity.Core.Features.User.Queries.GetUserProfilePictureDetail;
using Identity.Core.Features.User.Queries.PanVerificationDetails;
using Identity.Core.Features.User.Querries.GetMasterType;
using Identity.Domain.Authentication;
using Identity.Domain.Banner;
using Identity.Domain.Roles;
using Identity.Domain.User;
using Identity.Domain.UserAddressDetail;
using Identity.Domain.UserBankDetail;
using Identity.Domain.UserCreation;
using Identity.Domain.UserInquiryDetail;
using Identity.Domain.UserLogin;
using Identity.Domain.UserPersonalDetail;

namespace Identity.Core.Profiles;
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserCreationCommand, UserCreationModel>();
        CreateMap<UserPersonalDetailCommand, UserPersonalDetailModel>();
        CreateMap<UserBankDetailCommand, UserBankDetailModel>();
        CreateMap<UserDocumentUploadCommand, UserDocumentDetailModel>();
        CreateMap<UserInquiryDetailCommand, UserInquiryDetailModel>();
        CreateMap<PanDetailsModel, PanVerificationVM>();
        CreateMap<POSPUserMasterModel, MasterTypeVm>();
        CreateMap<UserDetailModel, GetUserDetailVm>();
        CreateMap<UserLoginResponseModel, SendOTPVm>();
        CreateMap<AuthenticationResponse, AuthenticateVm>();
        // Create Admin
        CreateMap<CreateAdminCommand, AdminUserModel>();
        CreateMap<AdminUserResponseModel, AdminVm>();
        CreateMap<AdminUserValidationModel, AdminVm>();
        // UpdateAdmin
        CreateMap<UpdateAdminCommand, AdminUpdateUserModel>();
        CreateMap<AdminUpdateUserResponseModel, AdminUpdateVM>();
        CreateMap<UserEmailIdCommand, UserModel>();
        CreateMap<UserBreadcrumStatusDetailModel, GetUserBreadcrumStatusDetailVm>();


        CreateMap<BenefitDetailModel, GetBenefitsDetailvm>();
        CreateMap<BenefitsDetailCommand, BenefitDetailModel>();
        CreateMap<BenefitDetailModel, BenefitsDetailCommand>();
        CreateMap<InsertBenefitsDetailCommand, BenefitDetailModel>();
        CreateMap<UserAddressDetailCommand, UserAddressDetailModel>();

        CreateMap<AuthenticateAdminCommand, AdminEmailModel>();
        CreateMap<AuthenticationAdminResponse, AuthenticateVM>();
        CreateMap<POSPConfigurationDetailModel, GetPOSPConfigurationDetailVm>();
        CreateMap<POSPConfigurationDetailModel, GetPOSPConfigurationDetailVm>();

        CreateMap<BannerUploadCommand, BannerDetailModel>();
        CreateMap<ReUploadDocumentCommand, UserDocumentDetailModel>();

        CreateMap<UserProfilePictureCommand, UserProfilePictureModel>();
        CreateMap<UserProfilePictureModel, GetUserProfilePictureDetailVm>();

        CreateMap<UserProfileDetailModel, GetUserProfileDetailVm>();


        CreateMap<RoleTypeResponseModel, RoleTypeVm>();
        CreateMap<ModuleResponseModel, ModelVm>();

        CreateMap<VerifyEmailCommand, VerifyEmailModel>();
        CreateMap<RoleModulePermissionCommand, RoleModulePermissionModel>();
        CreateMap<RoleModulePermissionCommandInsert, RoleModulePermissionCommandInsertModel>();

        CreateMap<UpdateRoleModulePermissionCommand, RoleModuleUpdatePermissionModel>();

        CreateMap<GetRoleQueryCommand, RoleSearchInputModel>();
        CreateMap<RoleSearchResponseModel, RoleSearchVM>();

        CreateMap<RoleSearchResponseAllModel, RoleSearchAllVM>();

        CreateMap<RoleBULevelResponseModel, BULevelVm>();
        CreateMap<BUDetailsCommand, BUSearchInputModel>();
        CreateMap<BUSearchResponseModel, BUDetailsVM>();
        CreateMap<BUUpdateCommand, BUUpdateInputModel>();
        CreateMap<BUInsertCommand, BUInsertInputModel>();
        CreateMap<CategoryInsertCommand, CategoryInputModel>();
        CreateMap<UserMappingInsertCommand, UserMappingInsertInputModel>();
        CreateMap<UserRoleSearchCommand, UserRoleSearchInputModel>();
        CreateMap<UserRoleSearchResponseModel, UserRoleVM>();
        CreateMap<UserRoleMappingInsertCommand, UserRoleModel>();
        CreateMap<UserRoleMappingUpdateCommand, UserRoleUpdateModels>();

        CreateMap<RoleMappingResponseModel, UserRoleGetVM>();
        CreateMap<UserRoleMappingGetCommand, RoleMappingInputModel>();
        CreateMap<RoleMappingResponseModel, UserRoleGetAllVM>();

        CreateMap<PanDetailMasterModel, GetUserPersonalVerificationDetailVm>();
        CreateMap<RoleMappingResponseModel, UserRoleGetAllVM>();
        CreateMap<RoleLevelResponseModel, RoleLevelVM>();
        CreateMap<ErrorCodeModel, GetErrorCodeVm>();
        CreateMap<StateCitybyPincodeModel, GetStateCitybyPincodeVm>();
        CreateMap<ResetPasswordResponseModel, ResetPasswordVM>(); // Reset Password Model Mapping
        CreateMap<LogoutResponseModel, LogoutVM>(); // Logout Model Mapping

        CreateMap<ResetPasswordVerificationModel, ResetPasswordVerificationVM>();

        CreateMap<GetUserListForDepartmentTaggingModel, GetUserListForDepartmentTaggingVm>();

        CreateMap<POSPSourceTypeModel, GetPOSPSourceTypeVm>();
        CreateMap<GetSourcedByUserListResponseModel, GetSourcedByUserListVm>();
        CreateMap<GetAllRelationshipManagerResponseModel, GetAllRelationshipManagerVM>();
        CreateMap<SendCompletedRegisterationMailQuery, SendCompletedRegisterationMailResponseModel>();
        CreateMap<UpdateUserPasswordFromUserLinkResponceModel, UpdateUserPasswordFromUserLinkVm>();

        CreateMap<ResetPasswordAdminResponseModel, ResetPasswordAdminVm>();
    }

}
