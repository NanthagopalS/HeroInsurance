using Admin.Core.Features.Banners.Queries.GetBannerDetail;
using Admin.Core.Features.HelpAndSupport.Queries.GetAllHelpAndSupport;
using Admin.Core.Features.HelpAndSupport.Queries.GetConcernType;
using Admin.Core.Features.HelpAndSupport.Queries.GetSubConcernType;
using Admin.Core.Features.Mmv.GetHeroVariantLists;
using Admin.Core.Features.Mmv.UpdateVariantsMapping;
using Admin.Core.Features.Mmv.VariantMappingStatus;
using Admin.Core.Features.Notification.Queries.GetAdminAlertType;
using Admin.Core.Features.Notification.Queries.GetAdminRecipientType;
using Admin.Core.Features.Notification.Queries.GetNotificationByIdAndType;
using Admin.Core.Features.Notification.Queries.GetNotificationDetailById;
using Admin.Core.Features.Notification.Queries.GetNotificationRecordById;
using Admin.Core.Features.TicketManagement.Command;
using Admin.Core.Features.TicketManagement.Queries.GetDeactivationTicketManagementDetail;
using Admin.Core.Features.TicketManagement.Queries.GetPOSPDetailsByDeactiveTicketId;
using Admin.Core.Features.TicketManagement.Queries.GetPOSPDetailsByIDToDeActivate;
using Admin.Core.Features.TicketManagement.Queries.GetTicketManagementDetail;
using Admin.Core.Features.TicketManagement.Queries.GetTicketManagementDetailById;
using Admin.Core.Features.User.Commands.BUInsert;
using Admin.Core.Features.User.Commands.BUUpdate;
using Admin.Core.Features.User.Commands.CategoryInsert;
using Admin.Core.Features.User.Commands.DeleteExamManagementDetail;
using Admin.Core.Features.User.Commands.DeletePoliciesDetail;
using Admin.Core.Features.User.Commands.DeleteTrainingManagementDetail;
using Admin.Core.Features.User.Commands.DownloadAgreement;
using Admin.Core.Features.User.Commands.EditNotification;
using Admin.Core.Features.User.Commands.InsertBUDetail;
using Admin.Core.Features.User.Commands.InsertLeadDetails;
using Admin.Core.Features.User.Commands.InsertNotification;
using Admin.Core.Features.User.Commands.InsertStampData;
using Admin.Core.Features.User.Commands.InsertStampInstruction;
using Admin.Core.Features.User.Commands.InsertUserRoleMappingDetaill;
using Admin.Core.Features.User.Commands.PublishNotification;
using Admin.Core.Features.User.Commands.ResetUserDetail;
using Admin.Core.Features.User.Commands.RoleDetailInsert;
using Admin.Core.Features.User.Commands.RoleModulePermission;
using Admin.Core.Features.User.Commands.RoleModuleUpdatePermission;
using Admin.Core.Features.User.Commands.UpdateActivateDeActivateBU;
using Admin.Core.Features.User.Commands.UpdateActivateDeActivateExamManagement;
using Admin.Core.Features.User.Commands.UpdateActivateDeActivateRole;
using Admin.Core.Features.User.Commands.UpdateActivateDeActivateTrainingManagement;
using Admin.Core.Features.User.Commands.UpdateActivePOSPAccountDetail;
using Admin.Core.Features.User.Commands.UpdateAgreementStatusByUserId;
using Admin.Core.Features.User.Commands.UpdateBUDetail;
using Admin.Core.Features.User.Commands.UpdateBUStatus;
using Admin.Core.Features.User.Commands.UpdateDocumentStatus;
using Admin.Core.Features.User.Commands.UpdateParticularPOSPDetailForIIBDashboard;
using Admin.Core.Features.User.Commands.UpdatePersonalDetails;
using Admin.Core.Features.User.Commands.UpdateUserRoleMappingDetail;
using Admin.Core.Features.User.Commands.UserEmailId;
using Admin.Core.Features.User.Commands.UserMappingInsert;
using Admin.Core.Features.User.Commands.UserRoleGetAllMapping;
using Admin.Core.Features.User.Commands.UserRoleGetMapping;
using Admin.Core.Features.User.Commands.UserRoleSearch;
using Admin.Core.Features.User.Commands.UserRoleUpdateModel;
using Admin.Core.Features.User.Queries.CheckForRole;
using Admin.Core.Features.User.Queries.CheckUserExistOrNot;
using Admin.Core.Features.User.Queries.GetAgreementManagementDetail;
using Admin.Core.Features.User.Queries.GetAllBUDetail;
using Admin.Core.Features.User.Queries.GetAllBUDetailsByUserIDQuery;
using Admin.Core.Features.User.Queries.GetAllBUUser;
using Admin.Core.Features.User.Queries.GetAllExamManagementDetail;
using Admin.Core.Features.User.Queries.GetAllPOSPBackOficePendingReport;
using Admin.Core.Features.User.Queries.GetAllPOSPCountDetail;
using Admin.Core.Features.User.Queries.GetAllPOSPDetailForIIBDashboard;
using Admin.Core.Features.User.Queries.GetAllSharedReportingRole;
using Admin.Core.Features.User.Queries.GetAllTrainingManagementDetails;
using Admin.Core.Features.User.Queries.GetAllUserRoleMappingDetailModel;
using Admin.Core.Features.User.Queries.GetAssistedBUDetails;
using Admin.Core.Features.User.Queries.GetBuDetail;
using Admin.Core.Features.User.Queries.GetBUHeadDetail;
using Admin.Core.Features.User.Queries.GetBUHierarchy;
using Admin.Core.Features.User.Queries.GetCardsDetail;
using Admin.Core.Features.User.Queries.GetCustomersDetail;
using Admin.Core.Features.User.Queries.GetDeactivatedUser;
using Admin.Core.Features.User.Queries.GetFunnelChart;
using Admin.Core.Features.User.Queries.GetInActivePOSPDetail;
using Admin.Core.Features.User.Queries.GetInsuranceType;
using Admin.Core.Features.User.Queries.GetLeadManagementDetail;
using Admin.Core.Features.User.Queries.GetLeadOverview;
using Admin.Core.Features.User.Queries.GetLeadStage;
using Admin.Core.Features.User.Queries.GetModel;
using Admin.Core.Features.User.Queries.GetNotificationList;
using Admin.Core.Features.User.Queries.GetParticularBUDetail;
using Admin.Core.Features.User.Queries.GetParticularLeadDetail;
using Admin.Core.Features.User.Queries.GetParticularLeadUploadedDocument;
using Admin.Core.Features.User.Queries.GetParticularPOSPDetailForIIBDashboard;
using Admin.Core.Features.User.Queries.GetParticularRoleDetail;
using Admin.Core.Features.User.Queries.GetParticularUserRoleMappingDetail;
using Admin.Core.Features.User.Queries.GetPoliciesDetail;
using Admin.Core.Features.User.Queries.GetPOSPOnboardingDetail;
using Admin.Core.Features.User.Queries.GetPOSPStageDetail;
using Admin.Core.Features.User.Queries.GetProductCategory;
using Admin.Core.Features.User.Queries.GetRecipientList;
using Admin.Core.Features.User.Queries.GetRelationshipManager;
using Admin.Core.Features.User.Queries.GetRenewalDetail;
using Admin.Core.Features.User.Queries.GetRoleBULevel;
using Admin.Core.Features.User.Queries.GetRoleLevel;
using Admin.Core.Features.User.Queries.GetRolePermission;
using Admin.Core.Features.User.Queries.GetRolePermissionAll;
using Admin.Core.Features.User.Queries.GetRoleType;
using Admin.Core.Features.User.Queries.GetSalesOverview;
using Admin.Core.Features.User.Queries.GetTotalSalesDetail;
using Admin.Core.Features.User.Queries.GetUserBreadcrumStatusDetail;
using Admin.Core.Features.User.Queries.GetUserByBUId;
using Admin.Core.Features.User.Queries.GetUserCategory;
using Admin.Core.Features.User.Queries.GetUserDetail;
using Admin.Core.Features.User.Queries.GetUserPersonalVerificationDetail;
using Admin.Core.Features.User.Queries.PanVerificationDetails;
using Admin.Core.Features.User.Querries.GetMasterType;
using Admin.Domain.Banners;
using Admin.Domain.HelpAndSupport;
using Admin.Domain.Mmv;
using Admin.Domain.Notification;
using Admin.Domain.Roles;
using Admin.Domain.TicketManagement;
using Admin.Domain.User;
using AutoMapper;

namespace Admin.Core.Profiles;
public class MappingProfile : Profile
{
    public MappingProfile()
    {

        CreateMap<PanDetailsModel, PanVerificationVM>();
        CreateMap<POSPUserMasterModel, MasterTypeVm>();
        CreateMap<UserDetailModel, GetUserDetailVm>();
        CreateMap<UserEmailIdCommand, UserModel>();
        CreateMap<UserBreadcrumStatusDetailModel, GetUserBreadcrumStatusDetailVm>();
        CreateMap<RoleTypeResponseModel, RoleTypeVm>();
        CreateMap<ModuleResponseModel, ModelVm>();
        CreateMap<RoleModulePermissionCommand, RoleModulePermissionModel>();
        CreateMap<RoleModulePermissionCommandInsert, RoleModulePermissionCommandInsertModel>();
        CreateMap<UpdateRoleModulePermissionCommand, RoleModuleUpdatePermissionModel>();
        CreateMap<GetRoleQueryCommand, RoleSearchInputModel>();
        CreateMap<RoleSearchResponseModel, RoleSearchVM>();
        CreateMap<RoleSearchResponseAllModel, RoleSearchAllVM>();
        CreateMap<RoleBULevelResponseModel, BULevelVm>();
        CreateMap<BUUpdateCommand, BUUpdateInputModel>();
        CreateMap<BUInsertCommand, BUInsertInputModel>();
        CreateMap<CategoryInsertCommand, CategoryInputModel>();
        CreateMap<UserMappingInsertCommand, UserMappingInsertInputModel>();
        CreateMap<UserRoleSearchCommand, UserRoleSearchInputModel>();
        CreateMap<UserRoleSearchResponseModel, UserRoleVM>();
        CreateMap<UserRoleMappingUpdateCommand, UserRoleUpdateModels>();
        CreateMap<RoleMappingResponseModel, UserRoleGetVM>();
        CreateMap<UserRoleMappingGetCommand, RoleMappingInputModel>();
        CreateMap<RoleMappingResponseModel, UserRoleGetAllVM>();
        CreateMap<PanDetailMasterModel, GetUserPersonalVerificationDetailVm>();
        CreateMap<RoleMappingResponseModel, UserRoleGetAllVM>();
        CreateMap<RoleLevelResponseModel, RoleLevelVM>();
        // CreateMap<RoleDetailResponseModel, RoleDetailVM>(); //For RoleDetail
        //CreateMap<GetRoleDetail, RoleDetailInputModel>(); // For RoleDetail
        CreateMap<RoleTypeDetailResponseModel, RoleTypeDetailVM>();
        CreateMap<GetRoleTypeDetail, RoleTypeDetailInputModel>();

        //CreateMap<GetAllUserRoleMappingResponseModel, GetAllUserRoleMappingDetailVm>();
        //CreateMap<GetAllUserRoleMappingDetailQuery, GetAllUserRoleMappingInputModel>();


        CreateMap<BUSearchModel, GetBUDetailVm>();
        //CreateMap<AllBUDetailResponseModel, GetAllBUDetailVm>();
        //CreateMap<GetAllBUDetailQuery, AllBUDetailModel>();
        CreateMap<InsertBUDetailCommand, BuResponsePermissionModel>();
        CreateMap<UpdateBUDetailCommand, UpdateBUStatusResonse>();
        CreateMap<ParticularBUDetailModel, GetParticularBUDetailVm>();
        CreateMap<GetParticularBUDetailQuery, ParticularBUDetailModel>();

        CreateMap<InsertBUDetailCommand, BUDetailModel>();
        CreateMap<UpdateBUDetailCommand, BUDetailModel>();
        CreateMap<RoleDetailInsertCommand, RoleDetailInsertInputModel>();
        CreateMap<RoleDetailPermissionInsertCmd, RoleDetailPermissionInsertModel>();
        CreateMap<RoleDetailUpdateCommand, RoleDetailUpdateInputModel>();
        CreateMap<RoleDetailPermissionUpdateCmd, RoleDetailPermissionUpdateModel>();
        CreateMap<ParticularRoleDetailResponseModel, ParticularRoleDetailVM>(); //For Particular RoleDetail
        CreateMap<GetParticularRoleDetail, ParticularRoleDetailInputModel>(); // For Particular RoleDetail


        CreateMap<InsertUserRoleMappingDetaillCommand, UserRoleModel>(); // For insert user and role mapping
        // CreateMap<InsertUserRoleMappingDetaillCommand, UserRoleMappingDetailPermissionModel>();
        CreateMap<UpdateUserRoleMappingDetailCommand, UpdateUserRoleMappingDetailModel>(); // For update user and role mapping
        CreateMap<GetParticularRoleDetailResponseModel, GetParticularRoleDetailQuery>(); // For particular role detail

        //CreateMap<AllPOSPDetailForIIBDashboardModel, GetAllPOSPDetailForIIBDashboardVm>();
        CreateMap<UserByBUIdResponseModel, GetUserByBUIdVm>();
        CreateMap<GetProductCategoryModel, GetProductCategoryVm>();
        CreateMap<ParticularPOSPDetailForIIBDashboardModel, GetParticularPOSPDetailForIIBDashboardVm>();
        CreateMap<UpdateParticularPOSPDetailForIIBDashboardCommand, ParticularPOSPIIBDasboardStatusUpdate>();


        CreateMap<HierarchyManagementDetailResponseModel, HierarchyManagementDetailVM>(); //For Hierarchy Management Detail
        CreateMap<GetHierarchyManagementDetail, HierarchyManagementDetailInputModel>(); // For Hierarchy Management Detail
        CreateMap<GetHierarchyManagementDetail, HierarchyManagementDetailResponseModel>(); // For Hierarchy Management Detail

        CreateMap<POSPInActiveDetailsModel, GetInActivePOSPDetailVm>();

        CreateMap<POSPOnboardingDetailModel, GetPOSPOnboardingDetailVm>();

        CreateMap<GetLeadManagementDetailModel, GetLeadManagementDetailVm>(); // For Lead Management detail
        CreateMap<GetRenewalDetailModel, GetRenewalDetailVm>(); // For Renewal detail
        CreateMap<GetPoliciesDetailModel, GetPoliciesDetailVm>(); // For Policies detail


        CreateMap<LeadOverviewModel, GetLeadOverviewVm>(); // For lead overview
        CreateMap<FunnelChartModel, GetFunnelChartVm>(); // For funnel chart
        CreateMap<SalesOverviewModel, GetSalesOverviewVm>(); // For sales overview

        CreateMap<AllTrainingManagementDetailsModel, GetAllTrainingManagementDetailsVm>();

        CreateMap<GetAllExamManagementDetailModel, GetAllExamManagementDetailVm>(); // For exam management detail
        CreateMap<GetAgreementManagementDetailModel, GetAgreementManagementDetailVm>(); // For agreement management detail
        CreateMap<GetUserListResponseModel, GetUserListVm>();

        CreateMap<BUHierarchyList, GetBUHierarchyVm>();
        CreateMap<UpdateBUStatusComand, UpdateBUStatusResponse>();

        CreateMap<RoleDetailResponseModel, RoleDetailVM>(); //For Role Detail
        CreateMap<GetRoleDetail, RoleDetailInputModel>(); // For Role Detail
        CreateMap<GetRoleDetail, RoleDetailResponseModel>(); // For Role Detail

        CreateMap<AllBUDetailResponseModel, GetAllBUDetailVm>(); //For ALL BU Detail
        CreateMap<GetAllBUDetailQuery, AllBUDetailModel>(); // For ALL BU Detail
        CreateMap<GetAllBUDetailQuery, AllBUDetailResponseModel>(); // For ALL BU Detail

        CreateMap<UpdateActivateDeActivateRoleCommand, UpdateActivateDeActivateRoleModel>(); // For activate and deactivate role
        CreateMap<UpdateActivateDeActivateBUCommand, UpdateActivateDeActivateBUModel>(); // For activate and deactivate BU 

        CreateMap<UpdateDocumentStatusCommand, ParticularLeadUploadedDocumentModel>();

        CreateMap<LeadDetail, GetParticularLeadDetailVm>();
        CreateMap<ParticularLeadUploadedDocumentModel, GetParticularLeadUploadedDocumentVm>();


        CreateMap<GetAllUserRoleMappingResponseModel, GetAllUserRoleMappingDetailVm>(); // For User Role Mapping
        CreateMap<GetAllUserRoleMappingDetailQuery, GetAllUserRoleMappingInputModel>(); // For User Role Mapping
        CreateMap<GetAllUserRoleMappingDetailQuery, GetAllUserRoleMappingResponseModel>(); // For User Role Mapping

        CreateMap<AllPOSPDetailForIIBDashboardModel, GetAllPOSPDetailForIIBDashboardVm>(); //All POSP Detail For IIB Dashboard 
        CreateMap<GetAllPOSPDetailForIIBDashboardQuery, AllPOSPDetailForIIBInputModel>(); // All POSP Detail For IIB Dashboard 
        CreateMap<GetAllPOSPDetailForIIBDashboardQuery, AllPOSPDetailForIIBDashboardModel>(); // All POSP Detail For IIB Dashboard 

        CreateMap<POSPManagementResponseModel, POSPManagementDetailVM>(); // For POS Management Details
        CreateMap<GetPOSPManagementDetail, POSPManagementInputModel>(); // For POS Management Details
        CreateMap<GetPOSPManagementDetail, POSPManagementResponseModel>(); // For POS Management Details

        CreateMap<UpdateActivateDeActivateExamManagementCommand, UpdateActivateDeActivateExamManagementModel>(); // For activate and deactivate exam management
        CreateMap<UpdateActivateDeActivateTrainingManagementCommand, UpdateActivateDeActivateTrainingManagementModel>(); // For activate and deactivate training management

        CreateMap<GetPOSPDocumentByIdQuery, POSPDocumentByIdDetail>(); // For POS Document By ID
        CreateMap<POSPDocumentByIdModel, GetPOSPDocumentByIdVm>(); // For POS Document By ID


        CreateMap<GetParticularUserRoleMappingDetailModel, GetParticularUserRoleMappingDetailVm>(); // For particular user and role mapping detail
        CreateMap<GetParticularUserRoleMappingDetailQuery, GetParticularUserRoleMappingDetailModel>(); // For particular user and role mapping detail

        CreateMap<InsertStampInstructionCommand, InsertStampInstructionModel>(); // For insert stamp instruction
        CreateMap<InsertStampDataCommand, InsertStampDataModel>(); // For insert stamp data

        CreateMap<ExamBulkUploadCommand, ExamBulkUploadModel>(); // For Bulk Upload Exam 
        CreateMap<GetRoleLevelByBUIdforRoleCreationResponseModel, GetRoleLevelByBUIdforRoleCreationVM>(); //For Role Level By BU Id For Role Creation
        CreateMap<GetRoleLevelByBUIdforRoleCreation, GetRoleLevelByBUIdforRoleCreationInputModel>(); // For Role Level By BU Id For Role Creation
        CreateMap<GetRoleLevelByBUIdforRoleCreation, GetRoleLevelByBUIdforRoleCreationResponseModel>(); // For Role Level By BU Id For Role Creation


        CreateMap<UpdatePersonalDetailsCommand, UpdateUserPersonalDetailResponseModel>();

        CreateMap<DeleteTrainingManagementDetailCommand, AllTrainingManagementDetailsModel>();
        CreateMap<DeleteExamManagementDetailCommand, GetAllExamManagementDetailModel>();
        CreateMap<DeletePoliciesDetailCommand, GetPoliciesDetailModel>();
        CreateMap<DeleteTrainingManagementDetailCommand, AllTrainingManagementDetailsModel>(); // For delete Training management detail
        CreateMap<DeleteExamManagementDetailCommand, GetAllExamManagementDetailModel>(); // For delete exam management detail
        CreateMap<DeletePoliciesDetailCommand, GetPoliciesDetailModel>(); // For delete leads policies detail

        CreateMap<StampCountResponseModel, StampCountVm>(); // For Stamp Count in Agreement Management 

        CreateMap<CheckUserExistOrNotModel, CheckUserExistOrNotVm>(); // For check user exists or not
        CreateMap<GetPOSPStageDetailModel, GetPOSPStageDetailVm>(); // For POSP stage detail

        CreateMap<UpdateAgreementStatusByUserIdCommand, UpdateAgreementStatusByUserIdModel>(); // For Update Agreement Status By UserId

        CreateMap<GetExamParticularQuestionDetailModel, GetExamParticularQuestionDetailVm>(); // For GetExamParticularQuestionDetailModel
        CreateMap<GetExamParticularQuestionDetailQuery, GetExamParticularQuestionDetailModel>(); // For GetExamParticularQuestionDetailModel

        CreateMap<UpdateExamParticularQuestionCommand, UpdateExamParticularQuestionModel>(); // For Update Particular Exam Option 

        CreateMap<TotalSalesResponseModel, GetTotalSalesDetailVm>(); //Getting Total Sales list for all IC's
        //CreateMap<GetTotalSalesDetailQuery, TotalSalesResponseModel>(); //Getting Total Sales list for all IC's

        CreateMap<CardsDetailResponseModel, GetCardsDetailVm>(); //Getting card details for contact section in leads dasboard
        CreateMap<RelationshipManagerModel, GetRelationshipManagerVm>(); //Getting card details for contact section in leads dasboard


        CreateMap<GetCustomersDetailResponseModel, GetCustomersDetailVm>(); //For Customer Detail
        CreateMap<GetCustomersDetailQuery, GetCustomersDetailInputModel>(); // For Customer Detail
        CreateMap<GetCustomersDetailQuery, GetCustomersDetailResponseModel>(); // For Customer Detail
        CreateMap<RequestForEditProfileCommand, RequestForEditProfileInputModel>(); // Request For Edit Profile

        CreateMap<GetBannerDetailModel, GetBannerDetailVm>(); // For Banner detail
        CreateMap<InsuranceTypeModel, GetInsuranceTypeVm>();
        CreateMap<LeadStageModel, GetLeadStageVm>();

        CreateMap<GetRecipientListResponseModel, GetRecipientListVm>();
        CreateMap<GetNotificationListResponseModel, GetNotificationListVm>();

        CreateMap<UpdateActivePOSPAccountDetailCommand, UpdateActivePOSPAccountDetailModel>();
        CreateMap<UpdateActivePOSPAccountDetailCommand, UpdateActivePOSPAccountDetailModel>();
        CreateMap<EditNotificationCommand, EditNotificationResponseModel>();

        CreateMap<InsertLeadDetailsCommand, InsertLeadDetailsModel>();
        CreateMap<GetAdminAlertTypeModel, GetAdminAlertTypeVm>(); // For Alert Type
        CreateMap<GetAdminRecipientTypeModel, GetAdminRecipientTypeVm>(); // For Recipient Type
        CreateMap<GetNotificationDetailByIdModel, GetNotificationDetailByIdVm>(); // For Get Notification Detail By Id
        CreateMap<GetNotificationRecordByIdModel, GetNotificationRecordByIdVm>(); // For Get Notification Detail By Id
        CreateMap<GetConcernTypeResponseModel, GetConcernTypeVm>();
        CreateMap<GetSubConcernTypeResponseModel, GetSubConcernTypeVm>();

        CreateMap<GetNotificationByIdAndTypeModel, GetNotificationByIdAndTypeVm>(); // For Get Notification Detail By Id And Type

        CreateMap<GetConcernTypeResponseModel, GetConcernTypeVm>();
        CreateMap<GetSubConcernTypeResponseModel, GetSubConcernTypeVm>();

        CreateMap<GetAllHelpAndSupportResponseModel, GetAllHelpAndSupportVm>();
        CreateMap<InsertNotificationCommand, InsertNotificationResponseModel>();

        CreateMap<ParticularHelpAndSupportModel, GetParticularHelpAndSupportVm>(); // Get Particular Help and Support 
        CreateMap<GetParticularHelpAndSupportQuery, ParticularHelpAndSupportModel>();
        CreateMap<PublishNotificationCommand, PublishNotificationResponseModel>();

        CreateMap<GetTicketManagementDetailResponseModel, GetTicketManagementDetailVm>();
        CreateMap<GetTicketManagementDetailByIdModel, GetTicketManagementDetailByIdVm>();
        CreateMap<GetTicketManagementDetailByIdVm, GetTicketManagementDetailByIdModel>();

        CreateMap<UpdateTicketManagementDetailByIdCommand, UpdateTicketManagementDetailByIdResponseModel>();

        CreateMap<GetDeactivationTicketManagementDetailResponseModel, GetDeactivationTicketManagementDetailVm>();
        CreateMap<GetPOSPDetailsByIDToDeActivateResponseModel, GetPOSPDetailsByIDToDeActivateVm>();

        CreateMap<DownloadAgreementCommand, DownloadAgreementResponse>();
        CreateMap<GetUserCategoryResponseModel, GetUserCategoryVm>();

        CreateMap<GetPOSPDetailsByDeactiveTicketIdResponceModel, GetPOSPDetailsByDeactiveTicketIdVm>();
        CreateMap<GetAssistedBUDetailsResponseModel, GetAssistedBUDetailsVm>();

        CreateMap<GetDeactivatedUserReponseModel, GetDeactivatedUserVm>();

        CreateMap<AllBUUserResponseModel, GetAllBUUserRoleVm>();
        CreateMap<GetAllSharedReportingRoleResponseModel, GetAllSharedReportingRoleVm>();
        CreateMap<CheckForRoleResponseModel, CheckForRoleQuery>();
        CreateMap<RoleDetailInsertResponceModel, RoleDetailInsertVm>();

        CreateMap<AllBUDetaiByUserIdModel, GetAllBUDetailsByUserIDVm>();
        CreateMap<GetAllPOSPCountDetailModel, GetAllPOSPCountDetailVM>();
        CreateMap<GetAllPOSPBackOficePendingReportModel, GetAllPOSPBackOficePendingReportVM>();
        CreateMap<ResetUserDetailByIdModel, ResetUserAccountDetailVM>();
        CreateMap<GetHeroVariantListsResponceModel, GetHeroVariantListsQueryVm>();
        CreateMap<UpdateVariantsMappingResponceModel, UpdateVariantsMappingCommandHandlerVm>();
		CreateMap<GetCustomMmvSearchResponseModel, GetCustomMmvSearchVm>();
	}

}
