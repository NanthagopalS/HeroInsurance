﻿using Identity.Core.Features.User.Commands.UpdateUserPasswordFromUserLinkCommand;
using Identity.Core.Features.User.Queries.GetAllRelationshipManager;
using Identity.Core.Features.User.Queries.GetPOSPSourceType;
using Identity.Domain.Roles;
using Identity.Domain.User;
using Identity.Domain.UserAddressDetail;
using Identity.Domain.UserBankDetail;
using Identity.Domain.UserCreation;
using Identity.Domain.UserInquiryDetail;
using Identity.Domain.UserPersonalDetail;

namespace Identity.Core.Contracts.Persistence;
public interface IUserRepository
{
    Task<UserCreateResponseModel> InsertUserCreationDetail(UserCreationModel userCreationModel);
    Task<bool> UpdateUserPersonalDetail(UserPersonalDetailModel userPersonalDetailModel);
    Task<UserBankDetailUpdateResponce> UpdateUserBankDetail(UserBankDetailModel userBankDetailModel, CancellationToken cancellationToken);
    Task<bool> UpdateUserAddressDetail(UserAddressDetailModel userAddressDetailModel, CancellationToken cancellationToken);
    Task<bool> InsertUserInquiryDetail(UserInquiryDetailModel userInqiryDetailModel);
    Task<POSPUserMasterModel> GetPOSPUserMaster(CancellationToken cancellationToken);
    Task<PanDetailsModel> VerifyPanDetails(string userId, string panNumber, CancellationToken cancellationToken);
    Task<bool> VerifyOTP(string otp, string userId);
    Task<bool> VerifyEmail(string GuId, string userId);
    Task<IEnumerable<UserDocumentTypeModel>> GetUserDocumentType(string UserId, CancellationToken cancellationToken);
    Task<bool> UserDocumentUpload(UserDocumentDetailModel userdocumentdetailmodel);
    Task<IEnumerable<UserDetailModel>> GetUserDetail(string userId, CancellationToken cancellationToken);
    Task<bool> UpdateUserEmailIdDetail(UserModel userModel, CancellationToken cancellation);
    Task<AdminUserValidationModel> ValidateAdminLogin(AdminUserModel loginModel, CancellationToken cancellationToken);
    Task<AdminUpdateUserResponseModel> UpdateAdminUserPasswordSelf(AdminUpdateUserModel updateLoginModel, CancellationToken cancellationToken);
    Task<UpdateUserPasswordFromUserLinkResponceModel> UpdateUserPasswordFromUserLink(UpdateUserPasswordFromUserLinkCommand updateLoginModel, CancellationToken cancellationToken);
    Task<IEnumerable<UserBreadcrumStatusDetailModel>> GetUserBreadcrumStatusDetail(string UserId, CancellationToken cancellationToken);
    Task<bool> InsertBenefitsDetailCreationDetail(BenefitDetailModel benefitsDetailModel, CancellationToken cancellationToken);
    Task<bool> UpdateBenefitsDetail(BenefitDetailModel benefitsDetailModel, CancellationToken cancellationToken);
    Task<bool> UserProfilePictureUpload(UserProfilePictureModel userProfilePictureModel);
    Task<IEnumerable<UserProfilePictureModel>> GetUserProfilePictureDetail(string UserId, CancellationToken cancellationToken);
    Task<UserProfileDetailModel> GetUserProfileDetail(string UserId, CancellationToken cancellationToken);
    Task<bool> DeleteBenefitsDetail(string userId,CancellationToken cancellationToken);
    Task<IEnumerable<POSPConfigurationDetailModel>> GetPOSPConfigurationDetail(CancellationToken cancellationToken);
    Task<IEnumerable<BenefitDetailModel>> GetBenefitsDetail(CancellationToken cancellationToken);
    Task<IEnumerable<UserDocumentDetailModel>> GetUserDocumentDetail(string UserId, CancellationToken cancellationToken);
    Task<bool> ReUploadDocument(UserDocumentDetailModel userdocumentdetailmodel);
    //Task<bool> DeleteBenefitsDetail(string userId, bool IsActive, CancellationToken cancellationToken);
    //Task<IEnumerable<BenefitDetailModel>> GetBenefitsDetail(string Id, CancellationToken cancellationToken);
    Task<IEnumerable<RoleTypeResponseModel>> GetRoleTypeDetails(CancellationToken cancellationToken);
    Task<IEnumerable<ModuleResponseModel>> GetModuleDetails(CancellationToken cancellationToken);
    Task<VerifyEmailResponseModel> VerfyIdentityAdminEmail(string UserId, string EmailId, CancellationToken cancellationToken);
    Task<bool> RoleModulePermissionMapping(RoleModulePermissionModel objPermissionModel, CancellationToken cancellationToken);
    Task<bool> UpdateRoleModulePermissionMapping(RoleModuleUpdatePermissionModel objPermissionModel, CancellationToken cancellationToken);
    Task<IEnumerable<RoleSearchResponseModel>> GetPermissionMapping(RoleSearchInputModel request, CancellationToken cancellationToken);
    Task<IEnumerable<RoleSearchResponseAllModel>> GetPermissionMappingAll(CancellationToken cancellationToken);
    Task<IEnumerable<RoleBULevelResponseModel>> GetRoleBULevelDetails(CancellationToken cancellationToken);
    Task<IEnumerable<BUSearchResponseModel>> GetBUDetails(BUSearchInputModel objBUInputModel, CancellationToken cancellationToken);
    Task<bool> BUUpdateDetails(BUUpdateInputModel objBUUpdateModel, CancellationToken cancellationToken);
    Task<bool> BUInsertDetails(BUInsertInputModel objBUInputModel, CancellationToken cancellationToken);
    Task<bool> CategoryInsert(CategoryInputModel objCategoryModel, CancellationToken cancellationToken);
    Task<bool> UserRoleMappingInsert(UserMappingInsertInputModel objMapInputModel, CancellationToken cancellationToken);
    Task<IEnumerable<UserRoleSearchResponseModel>> GetUserRoleMapping(UserRoleSearchInputModel objSearchModel, CancellationToken cancellationToken);
    Task<bool> InsertUserAndRoleMapping(UserRoleModel objUserRoleModel, CancellationToken cancellationToken);
    Task<bool> UpdateUserAndRoleMapping(UserRoleUpdateModels objUserRoleModel, CancellationToken cancellationToken);
    Task<IEnumerable<RoleMappingResponseModel>> GetUserandRoleMapping(RoleMappingInputModel objMappingInputModel, CancellationToken cancellationToken);
    Task<IEnumerable<RoleMappingResponseModel>> GetUserandRoleMappingAll(CancellationToken cancellationToken);
    Task<bool> ResetUserAccountDetail(string MobileNo, CancellationToken cancellationToken);
    Task<PanDetailMasterModel> GetUserPersonalVerificationDetail(string UserId, CancellationToken cancellationToken);
    Task<IEnumerable<RoleLevelResponseModel>> GetRoleLevelDetails(CancellationToken cancellationToken);
    Task<IEnumerable<ErrorCodeModel>> GetErrorCode(string ErrorType, CancellationToken cancellationToken);
    Task<StateCitybyPincodeModel> GetStateCitybyPincode(string Pincode, CancellationToken cancellationToken);
    Task<ResetPasswordResponseModel> ResetPassword(string emailId,string env, CancellationToken cancellationToken);
    Task<LogoutResponseModel> Logout(string userId, CancellationToken cancellationToken);
    Task<bool> ResetPasswordVerification(string GuId, string userId);
    Task<IEnumerable<GetUserListForDepartmentTaggingModel>> GetUserListForDepartmentTagging(string TaggingType, CancellationToken cancellationToken);
    Task<IEnumerable<GetPOSPSourceTypeVm>> GetPOSPSourceType(CancellationToken cancellationToken);
    Task<GetSourcedByUserListResponseModel> GetSourcedByUserList(string? BUId, CancellationToken cancellationToken);
    Task<IEnumerable<GetAllRelationshipManagerVM>> GetAllRelationshipManager(string? UserId, CancellationToken cancellationToken);
    Task<bool> SendCompletedRegisterationMail(string UserId, CancellationToken cancellationToken);
}
