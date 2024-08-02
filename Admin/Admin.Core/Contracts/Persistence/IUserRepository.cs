using Admin.Core.Features.User.Commands.DeactivateUserById;
using Admin.Core.Features.User.Commands.EditNotification;
using Admin.Core.Features.User.Commands.InsertNotification;
using Admin.Core.Features.User.Commands.PublishNotification;
using Admin.Core.Features.User.Commands.ResetUserDetail;
using Admin.Core.Features.User.Commands.UpdateActivePOSPAccountDetail;
using Admin.Core.Features.User.Commands.UpdateDocumentStatus;
using Admin.Core.Features.User.Commands.UpdatePersonalDetails;
using Admin.Core.Features.User.Queries.CheckForRole;
using Admin.Core.Features.User.Queries.GetAllBUDetailsByUserIDQuery;
using Admin.Core.Features.User.Queries.GetAllBUUser;
using Admin.Core.Features.User.Queries.GetAllSharedReportingRole;
using Admin.Core.Features.User.Queries.GetBUHeadDetail;
using Admin.Core.Features.User.Queries.GetBUHierarchy;
using Admin.Core.Features.User.Queries.GetDeactivatedUser;
using Admin.Core.Features.User.Queries.GetInsuranceType;
using Admin.Core.Features.User.Queries.GetLeadManagementDetail;
using Admin.Core.Features.User.Queries.GetLeadStage;
using Admin.Core.Features.User.Queries.GetParticularLeadUploadedDocument;
using Admin.Core.Features.User.Queries.GetParticularPOSPDetailForIIBDashboard;
using Admin.Core.Features.User.Queries.GetRecipientList;
using Admin.Core.Features.User.Queries.GetTotalSalesDetail;
using Admin.Core.Features.User.Queries.GetUserByBUId;
using Admin.Core.Features.User.Queries.GetUserCategory;
using Admin.Domain.Roles;
using Admin.Domain.User;

namespace Admin.Core.Contracts.Persistence;
public interface IUserRepository
{
    Task<POSPUserMasterModel> GetPOSPUserMaster(CancellationToken cancellationToken);
    Task<PanDetailsModel> VerifyPanDetails(string userId, string panNumber, CancellationToken cancellationToken);
    Task<IEnumerable<UserDocumentTypeModel>> GetUserDocumentType(CancellationToken cancellationToken);
    Task<IEnumerable<UserDetailModel>> GetUserDetail(string userId, CancellationToken cancellationToken);
    Task<bool> UpdateUserEmailIdDetail(UserModel userModel, CancellationToken cancellation);
    Task<IEnumerable<UserBreadcrumStatusDetailModel>> GetUserBreadcrumStatusDetail(string UserId, CancellationToken cancellationToken);
    Task<IEnumerable<RoleTypeResponseModel>> GetRoleTypeDetails(CancellationToken cancellationToken);
    Task<IEnumerable<ModuleResponseModel>> GetModuleDetails(string moduleGroupName, CancellationToken cancellationToken);
    Task<bool> RoleModulePermissionMapping(RoleModulePermissionModel objPermissionModel, CancellationToken cancellationToken);
    Task<bool> UpdateRoleModulePermissionMapping(RoleModuleUpdatePermissionModel objPermissionModel, CancellationToken cancellationToken);
    Task<IEnumerable<RoleSearchResponseModel>> GetPermissionMapping(RoleSearchInputModel request, CancellationToken cancellationToken);
    Task<IEnumerable<RoleSearchResponseAllModel>> GetPermissionMappingAll(CancellationToken cancellationToken);
    Task<IEnumerable<RoleBULevelResponseModel>> GetRoleBULevelDetails(CancellationToken cancellationToken);
    Task<IEnumerable<BUSearchModel>> GetBUDetail(string? RoleTypeId, CancellationToken cancellationToken);
    Task<bool> BUUpdateDetails(BUUpdateInputModel objBUUpdateModel, CancellationToken cancellationToken);
    Task<bool> BUInsertDetails(BUInsertInputModel objBUInputModel, CancellationToken cancellationToken);
    Task<bool> CategoryInsert(CategoryInputModel objCategoryModel, CancellationToken cancellationToken);
    Task<bool> UserRoleMappingInsert(UserMappingInsertInputModel objMapInputModel, CancellationToken cancellationToken);
    Task<IEnumerable<UserRoleSearchResponseModel>> GetUserRoleMapping(UserRoleSearchInputModel objSearchModel, CancellationToken cancellationToken);
    Task<bool> UpdateUserAndRoleMapping(UserRoleUpdateModels objUserRoleModel, CancellationToken cancellationToken);
    Task<IEnumerable<RoleMappingResponseModel>> GetUserandRoleMapping(RoleMappingInputModel objMappingInputModel, CancellationToken cancellationToken);
    Task<IEnumerable<RoleMappingResponseModel>> GetUserandRoleMappingAll(CancellationToken cancellationToken);
    Task<PanDetailMasterModel> GetUserPersonalVerificationDetail(string UserId, CancellationToken cancellationToken);
    Task<IEnumerable<RoleLevelResponseModel>> GetRoleLevelDetails(CancellationToken cancellationToken);

    Task<RoleDetailResponseModel> GetRoleDetail(RoleDetailInputModel roleDetailInputModel, CancellationToken cancellationToken);
    Task<IEnumerable<RoleTypeDetailResponseModel>> GetRoleTypeDetail(RoleTypeDetailInputModel roleTypeDetailInputModel, CancellationToken cancellationToken);
    Task<GetAllUserRoleMappingResponseModel> GetAllUserRoleMappingDetail(GetAllUserRoleMappingInputModel GetAllUserRoleMappingInputModel, CancellationToken cancellationToken);
    Task<AllBUDetailResponseModel> GetAllBuDetail(AllBUDetailModel allBUDetailModel, CancellationToken cancellationToken);
    Task<bool> InsertBUDetail(BuResponsePermissionModel bUInsertModel, CancellationToken cancellationToken);
    Task<bool> UpdateBUDetail(UpdateBUStatusResonse bUUpdateModel, CancellationToken cancellationToken);
    Task<IEnumerable<ParticularBUDetailModel>> GetParticularBUDetail(string BUId, CancellationToken cancellationToken);

    Task<bool> InsertRoleDetails(RoleDetailInsertInputModel roleDetailInsertInputModel, CancellationToken cancellationToken);
    Task<bool> UpdateRoleDetails(RoleDetailUpdateInputModel roleDetailUpdateInputModel, CancellationToken cancellationToken);

    Task<string> InsertUserRoleMappingDetail(UserRoleModel objUserRoleModel, CancellationToken cancellationToken);
    //Task<bool> InsertUserRoleMappingDetail(UserRoleMappingDetailPermissionModel userRoleMappingDetailPermissionModel, CancellationToken cancellationToken);
    Task<bool> UpdateUserRoleMappingDetail(UpdateUserRoleMappingDetailModel updateUserRoleMappingDetailModel, CancellationToken cancellationToken);
    Task<IEnumerable<ParticularRoleDetailResponseModel>> GetParticularRoleDetail(string roleTypeId, CancellationToken cancellationToken);
    Task<AllPOSPDetailForIIBDashboardModel> GetAllPOSPDetailForIIBDashboard(AllPOSPDetailForIIBInputModel allPOSPDetailForIIBInputModel, CancellationToken cancellationToken);
    Task<IEnumerable<UserByBUIdResponseModel>> GetUserByBUId(GetUserByBUIdQuery request, CancellationToken cancellationToken);

    Task<IEnumerable<GetProductCategoryModel>> GetProductCategory(CancellationToken cancellationToken);
    Task<IEnumerable<GetParticularPOSPDetailForIIBDashboardVm>> GetParticularPOSPDetailForIIBDashboard(string? POSPId, CancellationToken cancellationToken);
    Task<bool> UpdateParticularPOSPDetailForIIBDashboard(string? UserId, string? IIBStatus, string? IIBUploadStatus, CancellationToken cancellationToken);
    Task<bool> BulkUploadIIBDocument(IIBBulkUploadDocument iIBBulkUploadDocument, CancellationToken cancellationToken);

    Task<HierarchyManagementDetailResponseModel> GetHierarchyManagementDetail(string roleId, string roleTypeId, string ParentUserId, string ParentUserRoleId, CancellationToken cancellationToken);
    Task<IEnumerable<POSPInActiveDetailsModel>> GetInActivePOSPDetail(string? CriteriaType, string? FromDate, string? ToDate, int? PageIndex, CancellationToken cancellationToken);

    Task<IEnumerable<POSPOnboardingDetailModel>> GetPOSPOnboardingDetail(CancellationToken cancellationToken);
    Task<GetLeadManagementDetailModel> GetLeadManagementDetail(GetLeadManagementDetailQuery getLeadManagementDetailQuery, CancellationToken cancellationToken);
    Task<IEnumerable<GetRenewalDetailModel>> GetRenewalDetail(string? POSPId, string? PolicyNo, string? CustomerName, string? PolicyType, string? StartDate, string? EndDate, int PageIndex, CancellationToken cancellationToken);
    Task<IEnumerable<GetPoliciesDetailModel>> GetPoliciesDetail(string? POSPId, string? PolicyNo, string? CustomerName, string? Mobile, string? PolicyMode, string? PolicyType, string? StartDate, string? EndDate, int PageIndex, CancellationToken cancellationToken);
    Task<IEnumerable<LeadOverviewModel>> GetLeadOverview(string? LeadType, string? UserId, string? StartDate, string? EndDate, CancellationToken cancellationToken);
    Task<IEnumerable<FunnelChartModel>> GetFunnelChart(string? UserId, string? StartDate, string? EndDate, CancellationToken cancellationToken);
    Task<IEnumerable<SalesOverviewModel>> GetSalesOverview(string? UserId, string? StartDate, string? EndDate, CancellationToken cancellationToken);
    Task<POSPManagementResponseModel> GetPOSPManagementDetail(POSPManagementInputModel roleDetailInputModel, CancellationToken cancellationToken);
    Task<AllTrainingManagementDetailsModel> GetAllTrainingManagementDetails(string? searchtext, string? category, string? startDate, string? endDate, int? pageIndex, int? pageSize, CancellationToken cancellationToken);

    Task<GetAllExamManagementDetailModel> GetAllExamManagementDetail(string? Searchtext, string? Category, string? StartDate, string? EndDate, int? PageIndex, int? pageSize, CancellationToken cancellationToken);
    Task<GetAgreementManagementDetailModel> GetAgreementManagementDetail(string? searchText, string? statusId, string? startDate, string? endDate, int pageIndex, int pageSize, CancellationToken cancellationToken);
    Task<IEnumerable<GetUserListVm>> GetUserList(string? roleId, CancellationToken cancellationToken);
    Task<IEnumerable<GetBUHierarchyVm>> GetBUHierarchy(CancellationToken cancellationToken);
    Task<bool> UpdateBUStatus(string? BUId, bool? IsActive, CancellationToken cancellationToken);
    Task<bool> UpdateActivateDeActivateRole(UpdateActivateDeActivateRoleModel UpdateActivateDeActivateRoleModel, CancellationToken cancellation);
    Task<bool> UpdateActivateDeActivateBU(UpdateActivateDeActivateBUModel UpdateActivateDeActivateBUModel, CancellationToken cancellation);
    Task<bool> UpdateDocumentStatus(UpdateDocumentStatusCommand updateDocumentStatus, CancellationToken cancellation);
    Task<bool> UpdatePersonalDetails(UpdatePersonalDetailsCommand updatePersonalDetails, CancellationToken cancellation);

    Task<LeadDetail> GetParticularLeadDetail(string? LeadId, CancellationToken cancellationToken);
    Task<bool> ResetAdminUserAccountDetail(string? UserId, CancellationToken cancellationToken);
    Task<IEnumerable<GetParticularLeadUploadedDocumentVm>> GetParticularLeadUploadedDocument(string? UserId, CancellationToken cancellationToken);
    Task<bool> UpdateActivateDeActivateExamManagement(UpdateActivateDeActivateExamManagementModel UpdateActivateDeActivateExamManagementModel, CancellationToken cancellation);
    Task<bool> UpdateActivateDeActivateTrainingManagement(UpdateActivateDeActivateTrainingManagementModel UpdateActivateDeActivateTrainingManagementModel, CancellationToken cancellation);

    Task<GetPOSPDocumentByIdVm> GetPOSPDocumentById(string? DocumentId, CancellationToken cancellationToken);
    Task<IEnumerable<GetParticularUserRoleMappingDetailModel>> GetParticularUserRoleMappingDetail(string UserRoleMappingId, CancellationToken cancellationToken);
    Task<bool> InsertStampInstruction(string? SrNo, string? Instruction, CancellationToken cancellationToken);
    Task<bool> InsertStampData(string? SrNo, string? StampData, CancellationToken cancellationToken);
    Task<bool> ExampBulkUpload(ExamBulkUploadModel bulkUploadModel, CancellationToken cancellationToken);
    Task<IEnumerable<GetRoleLevelByBUIdforRoleCreationResponseModel>> GetRoleLevelByBUIdforRoleCreation(GetRoleLevelByBUIdforRoleCreationInputModel inputModel, CancellationToken cancellationToken);
    Task<bool> DeleteTrainingManagementDetail(string TrainingMaterialId, CancellationToken cancellationToken);
    Task<bool> DeleteExamManagementDetail(string? QuestionId, CancellationToken cancellationToken);
    Task<bool> DeletePoliciesDetail(string POSPId, CancellationToken cancellationToken);
    Task<StampCountResponseModel> GetStampCountQuery(CancellationToken cancellationToken);
    Task<IEnumerable<CheckUserExistOrNotModel>> CheckUserExistOrNot(string? UserId, string? EmpId, string? MobileNo, string? EmailId, CancellationToken cancellationToken);
    Task<IEnumerable<GetPOSPStageDetailModel>> GetPOSPStageDetail(CancellationToken cancellationToken);
    Task<bool> UploadTrainingFile(string? TrainingModuleType, string? LessionTitle, string? DocumentId, string? fileName, string MaterialFormatType, string VideoDuration, string LessonNumber, string CreatedBy, CancellationToken cancellationToken);
    Task<bool> UpdateAgreementStatusByUserId(UpdateAgreementStatusByUserIdModel UpdateAgreementStatusByUserIdModel, CancellationToken cancellation);
    Task<IEnumerable<GetExamParticularQuestionDetailModel>> GetExamParticularQuestionDetail(string? questionId, CancellationToken cancellationToken);
    Task<bool> UpdateExamParticularQuestion(UpdateExamParticularQuestionModel UpdateAgreementStatusByUserIdModel, CancellationToken cancellation);

    Task<GetCustomersDetailResponseModel> GetCustomersDetail(GetCustomersDetailInputModel GetCustomersDetailInputModel, CancellationToken cancellationToken);
    Task<bool> RequestForEditProfile(RequestForEditProfileInputModel roleDetailInsertInputModel, CancellationToken cancellationToken);
    Task<IEnumerable<TotalSalesResponseModel>> GetTotalSalesDetail(GetTotalSalesDetailQuery req, CancellationToken cancellation);
    Task<CardsDetailResponseModel> GetCardsDetail(string? userId, CancellationToken cancellationToken);
    Task<IEnumerable<RelationshipManagerModel>> GetRelationshipManager(string? userId, CancellationToken cancellationToken);
    Task<IEnumerable<GetInsuranceTypeVm>> GetInsuranceType(CancellationToken cancellationToken);
    Task<IEnumerable<GetLeadStageVm>> GetLeadStage(CancellationToken cancellationToken);
    Task<IEnumerable<GetRecipientListVm>> GetRecipientList(string? searchtext, string? RecipientType, CancellationToken cancellationToken);
    Task<bool> UpdateActivePOSPAccountDetail(UpdateActivePOSPAccountDetailCommand updateActivePOSPAccount, CancellationToken cancellationToken);
    Task<bool> InsertNotification(InsertNotificationCommand insertNotificationCommand, CancellationToken cancellationToken);
    Task<bool> EditNotification(EditNotificationCommand editNotificationCommand, CancellationToken cancellationToken);
    Task<bool> PublishNotification(PublishNotificationCommand publishNotification, CancellationToken cancellationToken);
    Task<InsertLeadDetailsModel> InsertLeadDetails(string? LeadName, string? LeadPhoneNumber, string? LeadEmailId, string? UserId, CancellationToken cancellationToken);
    Task<GetNotificationListResponseModel> GetNotificationList(string? SearchText, string? RecipientTypeId, string? StartDate, string? EndDate, int? CurrentPageIndex, int? CurrentPageSize, CancellationToken cancellationToken);
    Task<IEnumerable<DownloadAgreementResponse>> DownloadAgreement(string? POSPId, CancellationToken cancellationToken);
    Task<IEnumerable<GetAllBUUserRoleVm>> GetAllBUUserRole(GetAllBUUserRoleQuery bUUserQuery, CancellationToken cancellationToken);
    Task<bool> DeactivateUserById(DeactivateUserByIdCommand deactivateCommand, CancellationToken cancellation);
    Task<IEnumerable<GetUserCategoryVm>> GetUserCategory(CancellationToken cancellationToken);
    Task<GetDeactivatedUserReponseModel> GetDeactivatedUser(GetDeactivatedUserQuery getDeactivatedUserQuery, CancellationToken cancellationToken);
    Task<GetAssistedBUDetailsResponseModel> GetAssistedBUDetails(string? RoleId, string? UserId, CancellationToken cancellationToken);

    Task<IEnumerable<GetAllSharedReportingRoleVm>> GetAllSharedReportingRole(CancellationToken cancellationToken);
    Task<IEnumerable<CheckForRoleVm>> CheckForRole(CheckForRoleQuery checkForRoleQuery, CancellationToken cancellationToken);
    Task<IEnumerable<AllBUDetaiByUserIdModel>> GetAllBuDetailByUserID(GetAllBUDetailsByUserIDQuery allBUDetailModel, CancellationToken cancellationToken);

    Task<IEnumerable<GetAllPOSPCountDetailModel>> GetAllPOSPCountDetail(CancellationToken cancellationToken);
    Task<IEnumerable<GetAllPOSPBackOficePendingReportModel>> GetAllPOSPBackOficePendingReport(CancellationToken cancellationToken);
    Task<ResetUserDetailByIdModel> ResetUserIdDetail(ResetUserAccountDetailQuery resetUserAccountDetailQuery, CancellationToken cancellationToken);
}
