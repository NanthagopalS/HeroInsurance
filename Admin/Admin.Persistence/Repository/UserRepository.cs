using Admin.Core.Contracts.Common;
using Admin.Core.Contracts.Persistence;
using Admin.Core.Features.User.Commands.DeactivateUserById;
using Admin.Core.Features.User.Commands.EditNotification;
using Admin.Core.Features.User.Commands.InsertNotification;
using Admin.Core.Features.User.Commands.PublishNotification;
using Admin.Core.Features.User.Commands.ResetUserDetail;
using Admin.Core.Features.User.Commands.UpdateActivePOSPAccountDetail;
using Admin.Core.Features.User.Commands.UpdateDocumentStatus;
using Admin.Core.Features.User.Commands.UpdatePersonalDetails;
using Admin.Core.Features.User.Queries.CheckForRole;
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
//using NETCore.MailKit.Core;
using Admin.Domain.Roles;
using Admin.Domain.User;
using Admin.Domain.UserAddressDetail;
using Admin.Persistence.Configuration;
using Admin.Persistence.Utilities;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Transactions;
using ThirdPartyUtilities.Abstraction;
using ThirdPartyUtilities.Helpers;
using Org.BouncyCastle.Asn1.Ocsp;
using MediatR;
using Admin.Core.Features.User.Queries.GetAllBUDetailsByUserIDQuery;
using Admin.Core.Features.User.Queries.GetTotalSalesDetail;

namespace Admin.Persistence.Repository;
public class UserRepository : IUserRepository
{
    private readonly ApplicationDBContext _context;
    private readonly IdentityApplicationDBContext _identityDbContext;
    private readonly ISignzyService _signzyService;
    private readonly ISmsService _sMSService;
    private readonly IEmailService _emailService;
    private readonly IMongoDBService _mongodbService;
    private readonly IConfiguration _config;
    private readonly IApplicationClaims _applicationClaims;


    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="context"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public UserRepository(ApplicationDBContext context, ISignzyService signzyService, ISmsService sMSService, IEmailService emailService, IMongoDBService mongodbService, IConfiguration configuration, IApplicationClaims applicationClaims, IdentityApplicationDBContext identityDbContext)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _sMSService = sMSService ?? throw new ArgumentNullException(nameof(sMSService));
        _signzyService = signzyService ?? throw new ArgumentNullException(nameof(signzyService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _mongodbService = mongodbService ?? throw new ArgumentNullException(nameof(mongodbService));
        _config = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _applicationClaims = applicationClaims ?? throw new ArgumentNullException(nameof(applicationClaims));
        _identityDbContext = identityDbContext ?? throw new ArgumentNullException(nameof(identityDbContext));
    }


    /// <summary>
    /// Verify Pan
    /// </summary>
    /// <param name="panNumber"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<PanDetailsModel> VerifyPanDetails(string userId, string panNumber, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var vehicleDetails = await CheckPanDetailsFromDB(userId, panNumber, cancellationToken).ConfigureAwait(false);
        if (vehicleDetails == null)
        {
            //Code To Check PAN Number is Exists or Not in TestPAN table
            var mobileParams = new DynamicParameters();
            mobileParams.Add("PANNumber", panNumber, DbType.String, ParameterDirection.Input);
            var mobileExistResult = await connection.QueryAsync<PanDetailsModel>("[dbo].[Identity_CheckPANExists]", mobileParams, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            var parameters = new DynamicParameters();
            if (mobileExistResult.Any())
            {
                return mobileExistResult.FirstOrDefault();
            } // Code End For Static PAN Number
            else
            {
                var panResponse = await _signzyService.GetPANDetails(panNumber, cancellationToken);
                if (panResponse != null)
                {
                    parameters.Add("PanNumber", panResponse.response.result.number, DbType.String, ParameterDirection.Input);
                    parameters.Add("Name", panResponse.response.result.name, DbType.String, ParameterDirection.Input);
                    parameters.Add("FatherName", panResponse.response.result.fatherName, DbType.String, ParameterDirection.Input);
                    parameters.Add("DOB", panResponse.response.result.dob, DbType.String, ParameterDirection.Input);
                    parameters.Add("InstanceId", panResponse.response.instance.id, DbType.String, ParameterDirection.Input);
                    parameters.Add("InstanceCallbackUrl", panResponse.response.instance.callbackUrl, DbType.String, ParameterDirection.Input);
                    parameters.Add("UserId", userId, DbType.String, ParameterDirection.Input);
                    var result = await connection.QueryAsync<PanDetailsModel>("[dbo].[Admin_InsertPanDetails]", parameters,
                                 commandType: CommandType.StoredProcedure).ConfigureAwait(false);
                    return result.FirstOrDefault();
                }
            }
        }
        return vehicleDetails;
    }



    /// <summary>
    /// Check PAN details from DB
    /// </summary>
    /// <param name="panNumber"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>

    public async Task<IEnumerable<UserDocumentTypeModel>> GetUserDocumentType(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();

        var result = await connection.QueryAsync<UserDocumentTypeModel>("[dbo].[Admin_GetDocumentType]", commandType: CommandType.StoredProcedure);

        return result;
    }


    /// <summary>
    /// Get GetPOSPUserMaster List
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns> 
    public async Task<POSPUserMasterModel> GetPOSPUserMaster(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();


        var result = await connection.QueryMultipleAsync("[dbo].[Admin_GetPOSPUserMaster]",

           commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        POSPUserMasterModel response = new()
        {
            BackgroundTypeMasterList = result.Read<BackgroundTypeMasterModel>(),
            InsurerCompanyMasterList = result.Read<InsurerCompanyMasterModel>(),
            POSPSourceTypeMasterList = result.Read<POSPSourceTypeMasterModel>(),
            PremiumRangeTypeMasterList = result.Read<PremiumRangeTypeMasterModel>(),
            CityList = result.Read<CityModel>(),
            StateList = result.Read<StateModel>(),
            BankNameMasterList = result.Read<BankNameMasterModel>(),
            EducationQualificationMasterList = result.Read<EducationQualificationMasterModel>(),
            InsuranceProductsOfInterestMasterList = result.Read<InsuranceProductsOfInterestModel>(),
        };

        return response;
    }

    private async Task<PanDetailsModel> CheckPanDetailsFromDB(string userId, string panNumber, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("PanNumber", panNumber, DbType.String, ParameterDirection.Input);
        parameters.Add("UserId", userId, DbType.String, ParameterDirection.Input);
        parameters.Add("Condition", "CHECKPANDETAILS", DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<PanDetailsModel>("[dbo].[Admin_InsertPanDetails]", parameters,
                     commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        var panDetails = result.FirstOrDefault();

        return panDetails;
    }

    /// <summary>
    /// Get UserDetail List
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns> 
    public async Task<IEnumerable<UserDetailModel>> GetUserDetail(string UserId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", UserId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<UserDetailModel>("[dbo].[Admin_GetUserDetail]", parameters,
                     commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return result;
    }


    /// <summary>
    /// UpdateUserBankDetail
    /// </summary>
    /// <param name="userEmailIdModel"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> UpdateUserEmailIdDetail(UserModel userModel, CancellationToken cancellation)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", userModel.UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("EmailId", userModel.EmailId, DbType.String, ParameterDirection.Input);


        var result = await connection.ExecuteAsync("[dbo].[Admin_UpdateUserEmailIdDetail]", parameters,
            commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return result > 0;
    }

    /// <summary>
    /// GetUserBreadcrumStatusDetail
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns> 
    public async Task<IEnumerable<UserBreadcrumStatusDetailModel>> GetUserBreadcrumStatusDetail(string UserId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", UserId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<UserBreadcrumStatusDetailModel>("[dbo].[Admin_GetUserBreadcrumStatusDetail]", parameters,
                     commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return result;
    }

    public async Task<IEnumerable<RoleTypeResponseModel>> GetRoleTypeDetails(CancellationToken cancellationToken)
    {

        using var connection = _context.CreateConnection();

        var result = await connection.QueryAsync<RoleTypeResponseModel>("[dbo].[Admin_GetRoleTypeDetails]", commandType: CommandType.StoredProcedure);

        return result;

    }
    public async Task<IEnumerable<ModuleResponseModel>> GetModuleDetails(string moduleGroupName, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("ModuleGroupName", moduleGroupName, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<ModuleResponseModel>("[dbo].[Admin_GetModuleDetails]", parameters, commandType: CommandType.StoredProcedure);
        return result;
    }

    public async Task<bool> RoleModulePermissionMapping(RoleModulePermissionModel objPermissionModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        int Result = 0;
        List<RoleModulePermissionCommandInsertModel> lstRoleModulePermissionCommandInsert = new List<RoleModulePermissionCommandInsertModel>(); ;
        var parameters = new DynamicParameters();
        try
        {
            parameters.Add("RoleTypeID", objPermissionModel.RoleTypeId, DbType.String, ParameterDirection.Input);
            parameters.Add("RoleTitleName", objPermissionModel.RoleTitleName, DbType.String, ParameterDirection.Input);
            parameters.Add("BUID", objPermissionModel.BUId, DbType.String, ParameterDirection.Input);
            parameters.Add("RoleLevelID", objPermissionModel.RoleLevelId, DbType.String, ParameterDirection.Input);
            parameters.Add("CreatedBy", objPermissionModel.CreatedBy, DbType.String, ParameterDirection.Input);
            var resultIdentityRoleId = await connection.QueryAsync<UserRoleIdentity>("[dbo].[Admin_InsertUserRole]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            if (resultIdentityRoleId.Any())
            {
                {
                    var IdentityRoleId = resultIdentityRoleId.FirstOrDefault().IdentityRoleId;
                    lstRoleModulePermissionCommandInsert = objPermissionModel.RoleModulePermissionCommandInsert.ToList();
                    foreach (RoleModulePermissionCommandInsertModel rolepermission in lstRoleModulePermissionCommandInsert)
                    {
                        var parametersMapping = new DynamicParameters();
                        parametersMapping.Add("ModuleID", rolepermission.ModuleId, DbType.String, ParameterDirection.Input);
                        parametersMapping.Add("RoletypeID", objPermissionModel.RoleTypeId, DbType.String, ParameterDirection.Input);
                        parametersMapping.Add("IdentityRoleId", IdentityRoleId, DbType.Int64, ParameterDirection.Input);
                        parametersMapping.Add("AddPermission", rolepermission.AddPermission, DbType.Boolean, ParameterDirection.Input);
                        parametersMapping.Add("EditPermission", rolepermission.EditPermission, DbType.Boolean, ParameterDirection.Input);
                        parametersMapping.Add("ViewPermission", rolepermission.ViewPermission, DbType.Boolean, ParameterDirection.Input);
                        parametersMapping.Add("DeletePermission", rolepermission.DeletePermission, DbType.Boolean, ParameterDirection.Input);
                        parametersMapping.Add("DownloadPermission", rolepermission.DownloadPermission, DbType.Boolean, ParameterDirection.Input);
                        parametersMapping.Add("CreatedBy", rolepermission.CreatedBy, DbType.String, ParameterDirection.Input);
                        Result = await connection.ExecuteAsync("[dbo].[Admin_InsertRoleModulePermissionMapping]", parametersMapping, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
                        Result++;
                    }
                }
            }


            return Result > 0;
        }
        catch (TransactionException ex)
        {
            throw new Exception(ex.Message, ex);

        }
    }

    public async Task<bool> UpdateRoleModulePermissionMapping(RoleModuleUpdatePermissionModel objPermissionModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("RoleID", objPermissionModel.RoleId, DbType.String, ParameterDirection.Input);
        parameters.Add("ModuleID", objPermissionModel.ModuleId, DbType.String, ParameterDirection.Input);
        parameters.Add("Roletypeid", objPermissionModel.RoletypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("AddPermission", objPermissionModel.AddPermission, DbType.Boolean, ParameterDirection.Input);
        parameters.Add("EditPermission", objPermissionModel.EditPermission, DbType.Boolean, ParameterDirection.Input);
        parameters.Add("ViewPermission", objPermissionModel.ViewPermission, DbType.Boolean, ParameterDirection.Input);
        parameters.Add("DeletePermission", objPermissionModel.DeletePermission, DbType.Boolean, ParameterDirection.Input);
        parameters.Add("DownloadPermission", objPermissionModel.DownloadPermission, DbType.Boolean, ParameterDirection.Input);
        parameters.Add("UpdatedBy", objPermissionModel.UpdatedBy, DbType.String, ParameterDirection.Input);
        parameters.Add("UpdatedOn", System.DateTime.Now, DbType.DateTime, ParameterDirection.Input);
        parameters.Add("isActive", objPermissionModel.isActive, DbType.Boolean, ParameterDirection.Input);

        var result = await connection.ExecuteAsync("[dbo].[Admin_UpdateRoleModulePermissionMapping]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return (result > 0);
    }
    public async Task<IEnumerable<RoleSearchResponseModel>> GetPermissionMapping(RoleSearchInputModel objPermissionModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();

        parameters.Add("RoleTitleName", objPermissionModel?.RoleTitleName, DbType.String, ParameterDirection.Input);
        parameters.Add("RoleTypeName", objPermissionModel?.RoleTypeName, DbType.String, ParameterDirection.Input);
        parameters.Add("CreatedFrom", objPermissionModel?.CreatedFrom, DbType.String, ParameterDirection.Input);
        parameters.Add("CreatedTo", objPermissionModel?.CreatedTo, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<RoleSearchResponseModel>("[dbo].[Admin_GetRoleModulePermission]", parameters,
                    commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result;
    }
    public async Task<IEnumerable<RoleSearchResponseAllModel>> GetPermissionMappingAll(CancellationToken cancellationToken)
    {

        using var connection = _context.CreateConnection();

        var result = await connection.QueryAsync<RoleSearchResponseAllModel>("[dbo].[Admin_GetRoleModulePermissionALL]",
                  commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result;
    }


    public async Task<IEnumerable<RoleBULevelResponseModel>> GetRoleBULevelDetails(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();

        var result = await connection.QueryAsync<RoleBULevelResponseModel>("[dbo].[Admin_GetIBULevelDetails]", commandType: CommandType.StoredProcedure);

        return result;
    }

    public async Task<IEnumerable<BUSearchModel>> GetBUDetail(string? RoleTypeId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("RoleTypeId", RoleTypeId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<BUSearchModel>("[dbo].[Admin_GetBUDetails]", parameters,
                    commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result;
    }

    public async Task<bool> BUUpdateDetails(BUUpdateInputModel objBUUpdateModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("BUID", objBUUpdateModel?.BUId, DbType.String, ParameterDirection.Input);
        parameters.Add("IsActive", objBUUpdateModel?.IsActive, DbType.Boolean, ParameterDirection.Input);
        var result = await connection.ExecuteAsync("[dbo].[Admin_UpdateBUDetails]", parameters,
                    commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return (result > 0);
    }
    public async Task<bool> BUInsertDetails(BUInsertInputModel objBUInputModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add("Roletypeid", objBUInputModel?.RoleTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("BULevelID", objBUInputModel?.BULevelId, DbType.String, ParameterDirection.Input);
        parameters.Add("BUName", objBUInputModel?.BUName, DbType.String, ParameterDirection.Input);
        parameters.Add("IsActive", objBUInputModel?.IsActive, DbType.Boolean, ParameterDirection.Input);
        parameters.Add("RoleId", objBUInputModel?.RoleId, DbType.String, ParameterDirection.Input);
        parameters.Add("CreatedBy", objBUInputModel?.CreatedBy, DbType.String, ParameterDirection.Input);


        var result = await connection.ExecuteAsync("[dbo].[Admin_InsertBUDetails]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return (result > 0);
    }
    public async Task<bool> CategoryInsert(CategoryInputModel objCategoryModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add("UserCategoryName", objCategoryModel?.UserCategoryName, DbType.String, ParameterDirection.Input);
        parameters.Add("CreatedBy", objCategoryModel?.CreatedBy, DbType.String, ParameterDirection.Input);

        var result = await connection.ExecuteAsync("[dbo].[Admin_InsertUserCategory]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return (result > 0);
    }

    public async Task<bool> UserRoleMappingInsert(UserMappingInsertInputModel objMapInputModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add("UserID", objMapInputModel?.UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("RoleID", objMapInputModel?.RoleId, DbType.String, ParameterDirection.Input);
        parameters.Add("ReportingUserID", objMapInputModel?.ReportingUserId, DbType.String, ParameterDirection.Input);
        parameters.Add("CategoryID", objMapInputModel?.CategoryId, DbType.String, ParameterDirection.Input);
        parameters.Add("BUID", objMapInputModel?.BUId, DbType.String, ParameterDirection.Input);
        parameters.Add("RoleTypeID", objMapInputModel?.RoleTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("IsActive", objMapInputModel?.IsActive, DbType.Boolean, ParameterDirection.Input);

        var result = await connection.ExecuteAsync("[dbo].[Admin_InsertUserRoleMapping]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return (result > 0);
    }
    public async Task<IEnumerable<UserRoleSearchResponseModel>> GetUserRoleMapping(UserRoleSearchInputModel objSearchModel, CancellationToken cancellationToken)
    {

        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        //if (!string.IsNullOrEmpty(objPermissionModel.ToString()))
        if ((objSearchModel.EMPId != "") || (objSearchModel.RoleTypeName != "") || ((objSearchModel.Status.ToString() != "") || (objSearchModel.CreatedFrom != "" && objSearchModel.CreatedTo != "")))
        {
            parameters.Add("EMPID", objSearchModel?.EMPId, DbType.String, ParameterDirection.Input);
            parameters.Add("RoleTypeName", objSearchModel?.RoleTypeName, DbType.String, ParameterDirection.Input);
            if (objSearchModel.Status.ToString() != "")
            {
                parameters.Add("isActive", Convert.ToBoolean(objSearchModel?.Status), DbType.Boolean, ParameterDirection.Input);
            }
            parameters.Add("CreatedFrom", objSearchModel?.CreatedFrom, DbType.String, ParameterDirection.Input);
            parameters.Add("CreatedTo", objSearchModel?.CreatedTo, DbType.String, ParameterDirection.Input);
            var result = await connection.QueryAsync<UserRoleSearchResponseModel>("[dbo].[Admin_GetUserRoleMappingSearch]", parameters,
                        commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            return result;
        }
        else
        {
            var result = await connection.QueryAsync<UserRoleSearchResponseModel>("[dbo].[Admin_GetUserRoleMapping]",
                      commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            return result;
        }


    }
    //public async Task<bool> InsertUserAndRoleMapping(UserRoleModel objUserRoleModel, CancellationToken cancellationToken)
    //{
    //    using var connection = _context.CreateConnection();

    //    var parameters = new DynamicParameters();
    //    try
    //    {
    //        parameters.Add("UserName", objUserRoleModel?.UserName, DbType.String, ParameterDirection.Input);
    //        parameters.Add("EmpID", objUserRoleModel?.EmpID, DbType.String, ParameterDirection.Input);
    //        parameters.Add("MobileNo", objUserRoleModel?.MobileNo, DbType.String, ParameterDirection.Input);
    //        parameters.Add("EmailId", objUserRoleModel?.@EmailId, DbType.String, ParameterDirection.Input);
    //        parameters.Add("Gender", objUserRoleModel?.Gender, DbType.String, ParameterDirection.Input);
    //        parameters.Add("DOB", objUserRoleModel?.DOB, DbType.String, ParameterDirection.Input);
    //        parameters.Add("RoleId", objUserRoleModel?.RoleId, DbType.String, ParameterDirection.Input);
    //        parameters.Add("CreatedBy", objUserRoleModel?.CreatedBy, DbType.String, ParameterDirection.Input);
    //        parameters.Add("StatusUser", objUserRoleModel?.StatusUser, DbType.Boolean, ParameterDirection.Input);
    //        parameters.Add("RoleTypeID", objUserRoleModel?.RoleTypeId, DbType.String, ParameterDirection.Input);
    //        parameters.Add("IdentityRoleId", objUserRoleModel?.IdentityRoleId, DbType.String, ParameterDirection.Input);
    //        parameters.Add("ReportingIdentityRoleId", objUserRoleModel?.ReportingIdentityRoleId, DbType.String, ParameterDirection.Input);
    //        // parameters.Add("UserID", objUserRoleModel?.UserID, DbType.String, ParameterDirection.Input);
    //        parameters.Add("ReportingUserID", objUserRoleModel?.ReportingUserId, DbType.String, ParameterDirection.Input);
    //        parameters.Add("CategoryID", objUserRoleModel?.CategoryId, DbType.String, ParameterDirection.Input);
    //        parameters.Add("StatusRoleUser", objUserRoleModel?.StatusRoleUser, DbType.Boolean, ParameterDirection.Input);
    //        var result = await connection.ExecuteAsync("[dbo].[Admin_InsertUserandRoleMapping]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
    //        return (result > 0);
    //    }
    //    catch (Exception ex)
    //    {
    //        throw new Exception(ex.Message);
    //    }
    //}

    /*public async Task<bool> InsertUserRoleMappingDetail(UserRoleMappingDetailPermissionModel userRoleMappingDetailPermissionModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        int Result = 0;
        List<UserRoleMappingDetailCommandInsertModel> lstBuModulePermissionCommandInsert = new List<UserRoleMappingDetailCommandInsertModel>(); ;
        var parameters = new DynamicParameters();
        try
        {
            parameters.Add("RoleTypeId", userRoleMappingDetailPermissionModel.RoleTypeId, DbType.String, ParameterDirection.Input);
            parameters.Add("RoleName", userRoleMappingDetailPermissionModel.RoleName, DbType.String, ParameterDirection.Input);
            parameters.Add("BUId", userRoleMappingDetailPermissionModel.BUId, DbType.String, ParameterDirection.Input);
            parameters.Add("RoleLevelId", userRoleMappingDetailPermissionModel.RoleLevelId, DbType.String, ParameterDirection.Input);
            var resultIdentityRoleId = await connection.QueryAsync<UserRoleIdentity>("[dbo].[Admin_InsertUserRole]", parameters,
                                commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            if (resultIdentityRoleId.Any())
            {
                {
                    var IdentityRoleId = resultIdentityRoleId.FirstOrDefault().IdentityRoleId;
                    lstBuModulePermissionCommandInsert = userRoleMappingDetailPermissionModel.UserRoleMappingDetailCommandInsert.ToList();
                    foreach (UserRoleMappingDetailCommandInsertModel rolepermission in lstBuModulePermissionCommandInsert)
                    {
                        var parametersMapping = new DynamicParameters();
                        parametersMapping.Add("RoleTypeId", userRoleMappingDetailPermissionModel.RoleTypeId, DbType.String, ParameterDirection.Input);
                        parametersMapping.Add("IdentityRoleId", IdentityRoleId, DbType.Int64, ParameterDirection.Input);
                        parametersMapping.Add("AddPermission", rolepermission.AddPermission, DbType.Boolean, ParameterDirection.Input);
                        parametersMapping.Add("EditPermission", rolepermission.EditPermission, DbType.Boolean, ParameterDirection.Input);
                        parametersMapping.Add("ViewPermission", rolepermission.ViewPermission, DbType.Boolean, ParameterDirection.Input);
                        parametersMapping.Add("DeletePermission", rolepermission.DeletePermission, DbType.Boolean, ParameterDirection.Input);
                        parametersMapping.Add("DownloadPermission", rolepermission.DownloadPermission, DbType.Boolean, ParameterDirection.Input);
                        Result = await connection.ExecuteAsync("[dbo].[Admin_InsertRoleModulePermissionMapping]", parametersMapping,
                                            commandType: CommandType.StoredProcedure).ConfigureAwait(false);
                        Result++;
                    }
                }
            }
            return Result > 0;

        }
        catch (TransactionException ex)
        {
            throw new Exception(ex.Message, ex);

        }
    }*/
    public async Task<string> InsertUserRoleMappingDetail(UserRoleModel objUserRoleModel, CancellationToken cancellationToken)
    {
        Stream stream = new MemoryStream(objUserRoleModel.ImageStream);
        var mogoDocumentId = string.Empty;
        if (objUserRoleModel.ImageStream != null && objUserRoleModel.ImageStream.Length > 0)
        {
            mogoDocumentId = await _mongodbService.MongoUpload(objUserRoleModel.ProfilePictureID, stream, objUserRoleModel.ImageStream);
        }
        using var connection = _identityDbContext.CreateConnection();
        var parameters = new DynamicParameters();
        try
        {
            ResetPasswordGenerator passwordGen = new ResetPasswordGenerator();
            string newPassword = passwordGen.GenerateRandomStrongPassword(10);
            parameters.Add("UserName", objUserRoleModel?.UserName, DbType.String, ParameterDirection.Input);
            parameters.Add("EmpId", objUserRoleModel?.EmpID, DbType.String, ParameterDirection.Input);
            parameters.Add("MobileNo", objUserRoleModel?.MobileNo, DbType.String, ParameterDirection.Input);
            parameters.Add("EmailId", objUserRoleModel?.@EmailId, DbType.String, ParameterDirection.Input);
            parameters.Add("Gender", objUserRoleModel?.Gender, DbType.String, ParameterDirection.Input);
            parameters.Add("DOB", objUserRoleModel?.DOB, DbType.String, ParameterDirection.Input);
            parameters.Add("ProfilePictureId", objUserRoleModel?.ProfilePictureID, DbType.String, ParameterDirection.Input);
            parameters.Add("RoleTypeId", objUserRoleModel?.RoleTypeId, DbType.String, ParameterDirection.Input);
            parameters.Add("BUId", objUserRoleModel?.BUId, DbType.String, ParameterDirection.Input);
            parameters.Add("RoleId", objUserRoleModel?.RoleId, DbType.String, ParameterDirection.Input);
            parameters.Add("ReportingIdentityRoleId", objUserRoleModel?.ReportingIdentityRoleId, DbType.String, ParameterDirection.Input);
            parameters.Add("ReportingUserId", objUserRoleModel?.ReportingUserId, DbType.String, ParameterDirection.Input);
            parameters.Add("CategoryId", objUserRoleModel?.CategoryId, DbType.String, ParameterDirection.Input);
            parameters.Add("CreatedBy", objUserRoleModel?.CreatedBy, DbType.String, ParameterDirection.Input);
            parameters.Add("DocumentId", mogoDocumentId, DbType.String, ParameterDirection.Input);
            parameters.Add("Password", Hash256.Hash256Password(newPassword), DbType.String, ParameterDirection.Input);
            var result = await connection.ExecuteAsync("[dbo].[Identity_InsertUserWithRoleMapping]", parameters,
                         commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            if (result > 0)
            {
                return newPassword;
            }
            else
            {
                return string.Empty;
            }
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    public async Task<bool> UpdateUserRoleMappingDetail(UpdateUserRoleMappingDetailModel updateUserRoleMappingDetailModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        int Result = 0;
        List<UserRoleMappingDetailCommandUpdateModel> lstUserRoleMappingDetailCommandUpdate = new List<UserRoleMappingDetailCommandUpdateModel>(); ;
        var parameters = new DynamicParameters();
        try
        {
            parameters.Add("RoleId", updateUserRoleMappingDetailModel.RoleId, DbType.String, ParameterDirection.Input);
            parameters.Add("RoleTypeId", updateUserRoleMappingDetailModel.RoleTypeId, DbType.String, ParameterDirection.Input);
            parameters.Add("RoleName", updateUserRoleMappingDetailModel.RoleName, DbType.String, ParameterDirection.Input);
            parameters.Add("BUId", updateUserRoleMappingDetailModel.BUId, DbType.String, ParameterDirection.Input);
            parameters.Add("RoleLevelId", updateUserRoleMappingDetailModel.RoleLevelId, DbType.String, ParameterDirection.Input);
            var resultIdentityRoleId = await connection.QueryAsync<UserRoleIdentity>("[dbo].[Admin_InsertUserRole]", parameters,
                                commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            if (resultIdentityRoleId.Any())
            {
                {
                    var IdentityRoleId = resultIdentityRoleId.FirstOrDefault().IdentityRoleId;
                    lstUserRoleMappingDetailCommandUpdate = updateUserRoleMappingDetailModel.UserRoleMappingDetailCommandUpdate.ToList();
                    foreach (UserRoleMappingDetailCommandUpdateModel rolepermission in lstUserRoleMappingDetailCommandUpdate)
                    {
                        var parametersMapping = new DynamicParameters();
                        parametersMapping.Add("RoleTypeId", updateUserRoleMappingDetailModel.RoleTypeId, DbType.String, ParameterDirection.Input);
                        parametersMapping.Add("IdentityRoleId", IdentityRoleId, DbType.Int64, ParameterDirection.Input);
                        parametersMapping.Add("AddPermission", rolepermission.AddPermission, DbType.Boolean, ParameterDirection.Input);
                        parametersMapping.Add("EditPermission", rolepermission.EditPermission, DbType.Boolean, ParameterDirection.Input);
                        parametersMapping.Add("ViewPermission", rolepermission.ViewPermission, DbType.Boolean, ParameterDirection.Input);
                        parametersMapping.Add("DeletePermission", rolepermission.DeletePermission, DbType.Boolean, ParameterDirection.Input);
                        parametersMapping.Add("DownloadPermission", rolepermission.DownloadPermission, DbType.Boolean, ParameterDirection.Input);
                        Result = await connection.ExecuteAsync("[dbo].[Admin_InsertRoleModulePermissionMapping]", parametersMapping,
                                            commandType: CommandType.StoredProcedure).ConfigureAwait(false);
                        Result++;
                    }
                }
            }
            return Result > 0;

        }
        catch (TransactionException ex)
        {
            throw new Exception(ex.Message, ex);

        }
    }

    public async Task<bool> UpdateUserAndRoleMapping(UserRoleUpdateModels objUserRoleModel, CancellationToken cancellationToken)
    {
        var mogoDocumentId = string.Empty;
        if (objUserRoleModel.IsProfilePictureChange && objUserRoleModel.ImageStream != null)
        {
            Stream stream = new MemoryStream(objUserRoleModel.ImageStream);
            mogoDocumentId = await _mongodbService.MongoUpload(objUserRoleModel.ProfilePictureID, stream, objUserRoleModel.ImageStream);
        }
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        try
        {
            parameters.Add("UserRoleMappingId", objUserRoleModel?.UserRoleMappingId, DbType.String, ParameterDirection.Input);
            parameters.Add("UserId", objUserRoleModel?.UserId, DbType.String, ParameterDirection.Input);
            parameters.Add("UserName", objUserRoleModel?.UserName, DbType.String, ParameterDirection.Input);
            parameters.Add("EmpId", objUserRoleModel?.EmpID, DbType.String, ParameterDirection.Input);
            parameters.Add("MobileNo", objUserRoleModel?.MobileNo, DbType.String, ParameterDirection.Input);
            parameters.Add("EmailId", objUserRoleModel?.@EmailId, DbType.String, ParameterDirection.Input);
            parameters.Add("Gender", objUserRoleModel?.Gender, DbType.String, ParameterDirection.Input);
            parameters.Add("DOB", objUserRoleModel?.DOB, DbType.String, ParameterDirection.Input);
            parameters.Add("ProfilePictureId", objUserRoleModel?.ProfilePictureID, DbType.String, ParameterDirection.Input);
            parameters.Add("RoleTypeId", objUserRoleModel?.RoleTypeId, DbType.String, ParameterDirection.Input);
            parameters.Add("BUId", objUserRoleModel?.BUId, DbType.String, ParameterDirection.Input);
            parameters.Add("RoleId", objUserRoleModel?.RoleId, DbType.String, ParameterDirection.Input);
            parameters.Add("ReportingIdentityRoleId", objUserRoleModel?.ReportingIdentityRoleId, DbType.String, ParameterDirection.Input);
            parameters.Add("ReportingUserId", objUserRoleModel?.ReportingUserId, DbType.String, ParameterDirection.Input);
            parameters.Add("CategoryId", objUserRoleModel?.CategoryId, DbType.String, ParameterDirection.Input);
            parameters.Add("CreatedBy", objUserRoleModel?.CreatedBy, DbType.String, ParameterDirection.Input);
            parameters.Add("DocumentId", mogoDocumentId, DbType.String, ParameterDirection.Input);
            parameters.Add("IsActive", objUserRoleModel?.IsActive, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("IsProfilePictureChange", objUserRoleModel?.IsProfilePictureChange, DbType.Boolean, ParameterDirection.Input);
            var result = await connection.ExecuteAsync("[dbo].[Admin_UpdateUserandRoleMapping]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            return (result > 0);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }
    public async Task<IEnumerable<RoleMappingResponseModel>> GetUserandRoleMapping(RoleMappingInputModel objMappingInputModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@EMPID", objMappingInputModel.EMPId, DbType.String, ParameterDirection.Input);
        parameters.Add("@RoleTypeName", objMappingInputModel.RoleTypeName, DbType.String, ParameterDirection.Input);
        if (objMappingInputModel.isActive != "")
        {
            bool isActive = Convert.ToBoolean(objMappingInputModel.isActive);
            parameters.Add("@isActive", isActive, DbType.Boolean, ParameterDirection.Input);
        }

        parameters.Add("@CreatedFrom", objMappingInputModel.CreatedFrom, DbType.String, ParameterDirection.Input);
        parameters.Add("@CreatedTo", objMappingInputModel.CreatedTo, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<RoleMappingResponseModel>("[dbo].[Admin_GetUserAndRoleMappingSearch]", parameters,
                             commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result;
    }
    public async Task<IEnumerable<RoleMappingResponseModel>> GetUserandRoleMappingAll(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var result = await connection.QueryAsync<RoleMappingResponseModel>("[dbo].[Admin_GetUserandRoleMapping]",
                     commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return result;
    }

    /// <summary>
    /// GetUserPersonalVerificationDetail
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns> 
    public async Task<PanDetailMasterModel> GetUserPersonalVerificationDetail(string UserId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", UserId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryMultipleAsync("[dbo].[Admin_GetUserPersonalVerificationDetail]", parameters,
           commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        PanDetailMasterModel response = new()
        {
            UserList = result.Read<UserModel>(),
            UserDetailList = result.Read<UserDetailModel>(),
            PanDetailsList = result.Read<PanDetailsModel>(),
            UserAddressDetailList = result.Read<UserAddressDetailModel>()
        };

        return response;
    }

    public async Task<IEnumerable<RoleLevelResponseModel>> GetRoleLevelDetails(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var result = await connection.QueryAsync<RoleLevelResponseModel>("[dbo].[Admin_GetRoleLevelDetails]", commandType: CommandType.StoredProcedure);
        return result;
    }

    public async Task<RoleDetailResponseModel> GetRoleDetail(RoleDetailInputModel roleDetailInputModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@RoleName", roleDetailInputModel.RoleName, DbType.String, ParameterDirection.Input);
        parameters.Add("@RoleTypeId", roleDetailInputModel.RoleTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("@StartDate", roleDetailInputModel.StartDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@EndDate", roleDetailInputModel.EndDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@IsActive", roleDetailInputModel.IsActive, DbType.Boolean, ParameterDirection.Input);
        parameters.Add("@CurrentPageIndex", roleDetailInputModel.CurrentPageIndex, DbType.Int64, ParameterDirection.Input);
        parameters.Add("@CurrentPageSize", roleDetailInputModel.CurrentPageSize, DbType.Int64, ParameterDirection.Input);
        var result = await connection.QueryMultipleAsync("[dbo].[Admin_GetRoleDetail]", parameters,
            commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        RoleDetailResponseModel response = new()
        {
            RoleDetailModel = result.Read<RoleDetailModel>(),
            RoleDetailPagingModel = result.Read<RoleDetailPagingModel>()

        };
        return response;
    }

    public async Task<IEnumerable<RoleTypeDetailResponseModel>> GetRoleTypeDetail(RoleTypeDetailInputModel roleDetailInputModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@RoleName", roleDetailInputModel.RoleName, DbType.String, ParameterDirection.Input);
        parameters.Add("@RoleTypeId", roleDetailInputModel.RoleTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("@StartDate", roleDetailInputModel.StartDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@EndDate", roleDetailInputModel.EndDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@PageIndex", roleDetailInputModel.PageIndex, DbType.Int64, ParameterDirection.Input);
        var result = await connection.QueryAsync<RoleTypeDetailResponseModel>("[dbo].[Admin_GetRoleTypeDetail]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result;
    }

    public async Task<GetAllUserRoleMappingResponseModel> GetAllUserRoleMappingDetail(GetAllUserRoleMappingInputModel GetAllUserRoleMappingInputModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@EmployeeIdorName", GetAllUserRoleMappingInputModel.EmployeeIdorName, DbType.String, ParameterDirection.Input);
        parameters.Add("@RoleTypeId", GetAllUserRoleMappingInputModel.RoleTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("@StatusId", GetAllUserRoleMappingInputModel.StatusId, DbType.Boolean, ParameterDirection.Input);
        parameters.Add("@StartDate", GetAllUserRoleMappingInputModel.StartDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@EndDate", GetAllUserRoleMappingInputModel.EndDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@CurrentPageIndex", GetAllUserRoleMappingInputModel.CurrentPageIndex, DbType.Int64, ParameterDirection.Input);
        parameters.Add("@CurrentPageSize", GetAllUserRoleMappingInputModel.CurrentPageSize, DbType.Int64, ParameterDirection.Input);
        var result = await connection.QueryMultipleAsync("[dbo].[Admin_GetAllUserRoleMappingDetail]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        GetAllUserRoleMappingResponseModel response = new()
        {
            UserRoleMappingDataModel = result.Read<UserRoleMappingDataModel>(),
            UserRoleMappingPaginationModel = result.Read<UserRoleMappingPaginationModel>()

        };
        return response;
    }

    public async Task<AllBUDetailResponseModel> GetAllBuDetail(AllBUDetailModel allBUDetailModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@BuName", allBUDetailModel.BUName, DbType.String, ParameterDirection.Input);
        parameters.Add("@StartDate", allBUDetailModel.StartDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@EndDate", allBUDetailModel.EndDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@CurrentPageIndex", allBUDetailModel.CurrentPageIndex, DbType.Int64, ParameterDirection.Input);
        parameters.Add("@CurrentPageSize", allBUDetailModel.CurrentPageSize, DbType.Int64, ParameterDirection.Input);
        parameters.Add("@IsActive", allBUDetailModel.IsActive, DbType.Boolean, ParameterDirection.Input);
        var result = await connection.QueryMultipleAsync("[dbo].[Admin_GetAllBUDetail]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        AllBUDetailResponseModel response = new()
        {
            BUDetailsModel = result.Read<BUDetailsModel>(),
            BUDetailsPagingModel = result.Read<BUDetailsPagingModel>()
        };
        return response;
    }


    public async Task<bool> InsertBUDetail(BuResponsePermissionModel bUModelPermission, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("BUName", bUModelPermission.BUName, DbType.String, ParameterDirection.Input);
        parameters.Add("BUHeadId", bUModelPermission.BUHeadId, DbType.String, ParameterDirection.Input);
        parameters.Add("HierarchyLevelId", bUModelPermission.HierarchyLevelId, DbType.String, ParameterDirection.Input);
        parameters.Add("IsSales", bUModelPermission.IsSales, DbType.Boolean, ParameterDirection.Input);
        var result = await connection.ExecuteAsync("[dbo].[Admin_InsertBUDetails]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return (result > 0);
    }


    public async Task<bool> UpdateBUDetail(UpdateBUStatusResonse bUModelPermission, CancellationToken cancellation)
    {
        using var connection = _context.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("BUId", bUModelPermission.BUId, DbType.String, ParameterDirection.Input);
        parameters.Add("BUName", bUModelPermission.BUName, DbType.String, ParameterDirection.Input);
        parameters.Add("BUHeadId", bUModelPermission.BUHeadId, DbType.String, ParameterDirection.Input);
        parameters.Add("HierarchyLevelId", bUModelPermission.HierarchyLevelId, DbType.String, ParameterDirection.Input);
        parameters.Add("IsSales", bUModelPermission.IsSales, DbType.Boolean, ParameterDirection.Input);
        var result = await connection.ExecuteAsync("[dbo].[Admin_UpdateBUDetails]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return (result > 0);

    }

    public async Task<IEnumerable<ParticularBUDetailModel>> GetParticularBUDetail(string BUId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@BUId", BUId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<ParticularBUDetailModel>("[dbo].[Admin_GetParticularBUDetail]", parameters,
                     commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result;
    }

    public async Task<bool> InsertRoleDetails(RoleDetailInsertInputModel roleDetailInsertInputModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        int Result = 0;
        List<RoleDetailPermissionInsertModel> lstRoleModulePermissionCommandInsert = new List<RoleDetailPermissionInsertModel>();
        var parameters = new DynamicParameters();
        try
        {
            parameters.Add("RoleTypeId", roleDetailInsertInputModel?.RoleTypeId, DbType.String, ParameterDirection.Input);
            parameters.Add("RoleName", roleDetailInsertInputModel?.RoleName, DbType.String, ParameterDirection.Input);
            parameters.Add("BUId", roleDetailInsertInputModel?.BUId, DbType.String, ParameterDirection.Input);
            parameters.Add("RoleLevelId", roleDetailInsertInputModel?.RoleLevelId, DbType.String, ParameterDirection.Input);
            parameters.Add("IsActive", roleDetailInsertInputModel?.IsActive, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("CreatedBy", roleDetailInsertInputModel?.CreatedBy, DbType.String, ParameterDirection.Input);
            var resultIdentityRoleId = await connection.QueryAsync<UserRoleIdentity>("[dbo].[Admin_InsertRoleDetail]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            if (resultIdentityRoleId.Any())
            {
                {
                    var IdentityRoleId = resultIdentityRoleId?.FirstOrDefault()?.IdentityRoleId;
                    lstRoleModulePermissionCommandInsert = roleDetailInsertInputModel.RoleDetailPermissionInsert.ToList();
                    foreach (RoleDetailPermissionInsertModel rolepermission in lstRoleModulePermissionCommandInsert)
                    {
                        if (!string.IsNullOrWhiteSpace(rolepermission.ModuleId) && rolepermission.ModuleId != "string")
                        {
                            var parametersMapping = new DynamicParameters();
                            parametersMapping.Add("ModuleID", rolepermission.ModuleId, DbType.String, ParameterDirection.Input);
                            parametersMapping.Add("RoleTypeId", roleDetailInsertInputModel?.RoleTypeId, DbType.String, ParameterDirection.Input);
                            parametersMapping.Add("RoleId", IdentityRoleId, DbType.String, ParameterDirection.Input);
                            parametersMapping.Add("AddPermission", rolepermission.AddPermission, DbType.Boolean, ParameterDirection.Input);
                            parametersMapping.Add("EditPermission", rolepermission.EditPermission, DbType.Boolean, ParameterDirection.Input);
                            parametersMapping.Add("ViewPermission", rolepermission.ViewPermission, DbType.Boolean, ParameterDirection.Input);
                            parametersMapping.Add("DeletePermission", rolepermission.DeletePermission, DbType.Boolean, ParameterDirection.Input);
                            parametersMapping.Add("DownloadPermission", rolepermission.DownloadPermission, DbType.Boolean, ParameterDirection.Input);
                            parametersMapping.Add("CreatedBy", rolepermission.CreatedBy, DbType.String, ParameterDirection.Input);
                            Result = await connection.ExecuteAsync("[dbo].[Admin_InsertRoleDetailModulePermissionMapping]", parametersMapping, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
                            Result++;
                        }
                        else
                        {
                            Result++;
                        }
                    }
                }
            }


            return Result > 0;
        }
        catch (TransactionException ex)
        {
            throw new Exception(ex.Message, ex);

        }
    }

    public async Task<bool> UpdateRoleDetails(RoleDetailUpdateInputModel roleDetailUpdateInputModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        int Result = 0, i = 0;

        List<RoleDetailPermissionUpdateModel> lstRoleModulePermissionCommandUpdate = new List<RoleDetailPermissionUpdateModel>();
        var parameters = new DynamicParameters();
        try
        {
            parameters.Add("RoleId", roleDetailUpdateInputModel?.RoleId, DbType.String, ParameterDirection.Input);
            parameters.Add("RoleTypeId", roleDetailUpdateInputModel?.RoleTypeId, DbType.String, ParameterDirection.Input);
            parameters.Add("RoleName", roleDetailUpdateInputModel?.RoleName, DbType.String, ParameterDirection.Input);
            parameters.Add("BUId", roleDetailUpdateInputModel?.BUId, DbType.String, ParameterDirection.Input);
            parameters.Add("RoleLevelId", roleDetailUpdateInputModel?.RoleLevelId, DbType.String, ParameterDirection.Input);
            parameters.Add("IsActive", roleDetailUpdateInputModel?.IsActive, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("UpdatedBy", roleDetailUpdateInputModel?.UpdatedBy, DbType.String, ParameterDirection.Input);
            var resultIdentityRoleId = await connection.QueryAsync<UserRoleIdentity>("[dbo].[Admin_UpdateRoleDetail]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            if (roleDetailUpdateInputModel?.RoleDetailPermissionUpdate != null && roleDetailUpdateInputModel?.RoleDetailPermissionUpdate?.Count > 0)
            {
                {
                    lstRoleModulePermissionCommandUpdate = roleDetailUpdateInputModel.RoleDetailPermissionUpdate.ToList();
                    foreach (RoleDetailPermissionUpdateModel rolepermission in lstRoleModulePermissionCommandUpdate)
                    {
                        i++;
                        var parametersMapping = new DynamicParameters();
                        parametersMapping.Add("ItemNo", i, DbType.Int64, ParameterDirection.Input);
                        parametersMapping.Add("ModuleId", rolepermission.ModuleId, DbType.String, ParameterDirection.Input);
                        parametersMapping.Add("RoleId", roleDetailUpdateInputModel?.RoleId, DbType.String, ParameterDirection.Input);
                        parametersMapping.Add("RoleTypeId", roleDetailUpdateInputModel?.RoleTypeId, DbType.String, ParameterDirection.Input);
                        parametersMapping.Add("AddPermission", rolepermission.AddPermission, DbType.Boolean, ParameterDirection.Input);
                        parametersMapping.Add("EditPermission", rolepermission.EditPermission, DbType.Boolean, ParameterDirection.Input);
                        parametersMapping.Add("ViewPermission", rolepermission.ViewPermission, DbType.Boolean, ParameterDirection.Input);
                        parametersMapping.Add("DeletePermission", rolepermission.DeletePermission, DbType.Boolean, ParameterDirection.Input);
                        parametersMapping.Add("DownloadPermission", rolepermission.DownloadPermission, DbType.Boolean, ParameterDirection.Input);
                        parametersMapping.Add("UpdatedBy", rolepermission.CreatedBy, DbType.String, ParameterDirection.Input);
                        Result = await connection.ExecuteAsync("[dbo].[Admin_UpdateRoleDetailModulePermissionMapping]", parametersMapping, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
                        Result++;
                    }
                }
            }
            return Result > 0;
        }
        catch (TransactionException ex)
        {
            throw new Exception(ex.Message, ex);

        }
    }

    public async Task<IEnumerable<ParticularRoleDetailResponseModel>> GetParticularRoleDetail(string roleTypeId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@RoleId", roleTypeId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<ParticularRoleDetailResponseModel>("[dbo].[Admin_GetParticularRoleDetail]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        // Binding Code Start : For Binding List Of Permisssion Need to Put Database Call
        var permissionList = await connection.QueryAsync<ParticularPermissionRoleDetailResponseModel>("[dbo].[Admin_GetParticularRolePermissionDetail]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        foreach (var item in result)
        {
            item.ParticularPermissions = permissionList;
        }
        // Binding Code End
        return result;
    }

    public async Task<AllPOSPDetailForIIBDashboardModel> GetAllPOSPDetailForIIBDashboard(AllPOSPDetailForIIBInputModel allPOSPDetailForIIBInputModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@SearchText", allPOSPDetailForIIBInputModel.Searchtext, DbType.String, ParameterDirection.Input);
        parameters.Add("@Createdby", allPOSPDetailForIIBInputModel.CreatedBy, DbType.String, ParameterDirection.Input);
        parameters.Add("@Statustype", allPOSPDetailForIIBInputModel.StatusType, DbType.String, ParameterDirection.Input);
        parameters.Add("@StartDate", allPOSPDetailForIIBInputModel.StartDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@EndDate", allPOSPDetailForIIBInputModel.EndDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@CurrentPageIndex", allPOSPDetailForIIBInputModel.PageIndex, DbType.Int64, ParameterDirection.Input);
        parameters.Add("@CurrentPageSize", allPOSPDetailForIIBInputModel.PageSize, DbType.Int64, ParameterDirection.Input);
        var result = await connection.QueryMultipleAsync("[dbo].[Admin_GetAllPOSPDetailForIIBDashboard]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        AllPOSPDetailForIIBDashboardModel response = new()
        {
            AllPOSPDetailDataModel = result.Read<AllPOSPDetailDataModel>(),
            AllPOSPDetailDataPaginationModel = result.Read<AllPOSPDetailDataPaginationModel>()
        };
        return response;
    }

    public async Task<IEnumerable<GetProductCategoryModel>> GetProductCategory(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();

        var result = await connection.QueryAsync<GetProductCategoryModel>("[dbo].[Admin_GetProductCategory]",
            commandType: CommandType.StoredProcedure);

        return result;
    }

    public async Task<HierarchyManagementDetailResponseModel> GetHierarchyManagementDetail(string roleId, string roleTypeId, string parentUserId, string parentUserRoleId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@RoleId", roleId, DbType.String, ParameterDirection.Input);
        parameters.Add("@RoleTypeId", roleTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("@ParentUserId", parentUserId, DbType.String, ParameterDirection.Input);
        parameters.Add("@ParentUserRoleId", parentUserRoleId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryMultipleAsync("[dbo].[Admin_GetHierarchyManagementDetail]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        HierarchyManagementDetailResponseModel response = new()
        {
            UserList = result.Read<UserListModel>(),
            ParentList = result.Read<ParentListModel>()

        };
        return response;
    }

    public async Task<IEnumerable<UserByBUIdResponseModel>> GetUserByBUId(GetUserByBUIdQuery request, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@BUId", request.BUId, DbType.String, ParameterDirection.Input);
        parameters.Add("@UserId", _applicationClaims.GetUserId(), DbType.String, ParameterDirection.Input);
        parameters.Add("@SearchText", request.SearchText, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<UserByBUIdResponseModel>("[dbo].[Admin_GetUserByBUId]", parameters, commandType: CommandType.StoredProcedure);
        return result;
    }

    public async Task<IEnumerable<GetParticularPOSPDetailForIIBDashboardVm>> GetParticularPOSPDetailForIIBDashboard(string? UserId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@UserId", UserId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<GetParticularPOSPDetailForIIBDashboardVm>("[dbo].[Admin_GetParticularPOSPDetailForIIBDashboard]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result;
    }

    public async Task<POSPManagementResponseModel> GetPOSPManagementDetail(POSPManagementInputModel pOSPManagementInputModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@POSPStatus", pOSPManagementInputModel.POSPStatus, DbType.Int64, ParameterDirection.Input);
        parameters.Add("@Searchtext", pOSPManagementInputModel.SearchText, DbType.String, ParameterDirection.Input);
        parameters.Add("@Stage", pOSPManagementInputModel.StageId, DbType.String, ParameterDirection.Input);
        parameters.Add("@CreatedBy", pOSPManagementInputModel.CreatedBy, DbType.String, ParameterDirection.Input);
        parameters.Add("@RelationManagerId", pOSPManagementInputModel.RelationManagerId, DbType.String, ParameterDirection.Input);
        parameters.Add("@CurrentPageIndex", pOSPManagementInputModel.PageIndex, DbType.Int64, ParameterDirection.Input);
        parameters.Add("@CurrentPageSize", pOSPManagementInputModel.PageSize, DbType.Int64, ParameterDirection.Input);
        var result = await connection.QueryMultipleAsync("[dbo].[Admin_GetPOSPManagementDetailList]", parameters,
                     commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        POSPManagementResponseModel response = new()
        {
            POSPManagementDataModel = result.Read<POSPManagementDataModel>(),
            POSPManagementPaginationModel = result.Read<POSPManagementPaginationModel>()
        };
        return response;
    }

    public async Task<IEnumerable<POSPInActiveDetailsModel>> GetInActivePOSPDetail(string? CriteriaType, string? FromDate, string? ToDate, int? PageIndex, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@CriteriaType", CriteriaType, DbType.String, ParameterDirection.Input);
        parameters.Add("@FromDate", FromDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@ToDate", ToDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@PageIndex", PageIndex, DbType.Int64, ParameterDirection.Input);
        var result = await connection.QueryAsync<POSPInActiveDetailsModel>("[dbo].[Admin_GetInActivePOSPDetail]", parameters,
                        commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result;
    }

    public async Task<bool> UpdateParticularPOSPDetailForIIBDashboard(string? UserId, string? IIBStatus, string? IIBUploadStatus, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("@UserId", UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("@IIBStatus", IIBStatus, DbType.String, ParameterDirection.Input);
        parameters.Add("@IIBUploadStatus", IIBUploadStatus, DbType.String, ParameterDirection.Input);
        var result = await connection.ExecuteAsync("[dbo].[Admin_UpdateParticularPOSPDetailForIIBDashboard]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return (result > 0);
    }

    public async Task<IEnumerable<LeadOverviewModel>> GetLeadOverview(string? LeadType, string? UserId, string? StartDate, string? EndDate, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@LeadType", LeadType, DbType.String, ParameterDirection.Input);
        parameters.Add("@UserId", UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("@StartDate", StartDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@EndDate", EndDate, DbType.String, ParameterDirection.Input);

        var result = await connection.QueryAsync<LeadOverviewModel>("[dbo].[Admin_GetLeadOverview]", parameters,
            commandType: CommandType.StoredProcedure);

        return result;
    }

    public async Task<IEnumerable<FunnelChartModel>> GetFunnelChart(string? StartDate, string? EndDate, string? UserId, CancellationToken cancellationToken)
    {
        UserId = !string.IsNullOrEmpty(UserId) ? UserId : _applicationClaims.GetUserId();

        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@StartDate", StartDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@EndDate", EndDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@UserId", UserId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<FunnelChartModel>("[dbo].[Admin_GetFunnelChart]", parameters,
            commandType: CommandType.StoredProcedure);

        return result;
    }

    public async Task<IEnumerable<SalesOverviewModel>> GetSalesOverview(string? UserId, string? StartDate, string? EndDate, CancellationToken cancellationToken)
    {
        UserId = (UserId != null) ? UserId : _applicationClaims.GetUserId();

        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@UserId", UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("@Startdate", StartDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@Enddate", EndDate, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<SalesOverviewModel>("[dbo].[Admin_GetSalesOverview]", parameters,
            commandType: CommandType.StoredProcedure);

        return result;
    }

    public async Task<GetLeadManagementDetailModel> GetLeadManagementDetail(GetLeadManagementDetailQuery leadDetailQuery, CancellationToken cancellationToken)
    {
        try
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("@ViewLeadsType", !string.IsNullOrWhiteSpace(leadDetailQuery.ViewLeadsType) ? leadDetailQuery.ViewLeadsType : "Lead", DbType.String, ParameterDirection.Input);
            parameters.Add("@UserId", leadDetailQuery.UserId, DbType.String, ParameterDirection.Input);
            parameters.Add("@SearchText", leadDetailQuery.SearchText, DbType.String, ParameterDirection.Input);
            parameters.Add("@LeadType", leadDetailQuery.LeadType, DbType.String, ParameterDirection.Input);
            parameters.Add("@POSPId", leadDetailQuery.POSPId, DbType.String, ParameterDirection.Input);
            parameters.Add("@PolicyType", leadDetailQuery.PolicyType, DbType.String, ParameterDirection.Input);
            parameters.Add("@PreQuote", leadDetailQuery.PreQuote, DbType.String, ParameterDirection.Input);
            parameters.Add("@AllStatus", leadDetailQuery.AllStatus, DbType.String, ParameterDirection.Input);
            parameters.Add("@StartDate", leadDetailQuery.StartDate, DbType.String, ParameterDirection.Input);
            parameters.Add("@EndDate", leadDetailQuery.EndDate, DbType.String, ParameterDirection.Input);
            parameters.Add("@CurrentPageIndex", leadDetailQuery.PageIndex, DbType.Int64, ParameterDirection.Input);
            parameters.Add("@CurrentPageSize", leadDetailQuery.PageSize, DbType.Int64, ParameterDirection.Input);
            parameters.Add("@isMask", leadDetailQuery.isMask, DbType.Boolean, ParameterDirection.Input);
            var result = await connection.QueryMultipleAsync("[dbo].[Admin_GetLeadManagementDetail]", parameters,
                commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            GetLeadManagementDetailModel response = new()
            {
                LeadDetailModelList = result.Read<LeadDetailModelList>(),
                LeadDetailPagingModel = result.Read<LeadDetailPagingModel>()

            };
            return response;
        }
        catch (Exception)
        {

            throw;
        }
    }


    public async Task<IEnumerable<GetRenewalDetailModel>> GetRenewalDetail(string? POSPId, string? PolicyNo, string? CustomerName, string? PolicyType, string? StartDate, string? EndDate, int PageIndex, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@POSPId", POSPId, DbType.String, ParameterDirection.Input);
        parameters.Add("@PolicyNo", PolicyNo, DbType.String, ParameterDirection.Input);
        parameters.Add("@CustomerName", CustomerName, DbType.String, ParameterDirection.Input);
        parameters.Add("@PolicyType", PolicyType, DbType.String, ParameterDirection.Input);
        parameters.Add("@StartDate", StartDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@EndDate", EndDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@PageIndex", PageIndex, DbType.Int64, ParameterDirection.Input);

        var result = await connection.QueryAsync<GetRenewalDetailModel>("[dbo].[Admin_GetRenewalDetail]", parameters,
            commandType: CommandType.StoredProcedure);

        return result;


    }

    public async Task<IEnumerable<GetPoliciesDetailModel>> GetPoliciesDetail(string? POSPId, string? PolicyNo, string? CustomerName, string? Mobile, string? PolicyMode, string? PolicyType, string? StartDate, string? EndDate, int PageIndex, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@POSPId", POSPId, DbType.String, ParameterDirection.Input);
        parameters.Add("@PolicyNo", PolicyNo, DbType.String, ParameterDirection.Input);
        parameters.Add("@CustomerName", CustomerName, DbType.String, ParameterDirection.Input);
        parameters.Add("@Mobile", Mobile, DbType.String, ParameterDirection.Input);
        parameters.Add("@PolicyMode", PolicyMode, DbType.String, ParameterDirection.Input);
        parameters.Add("@PolicyType", PolicyType, DbType.String, ParameterDirection.Input);
        parameters.Add("@StartDate", StartDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@EndDate", EndDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@PageIndex", PageIndex, DbType.Int64, ParameterDirection.Input);

        var result = await connection.QueryAsync<GetPoliciesDetailModel>("[dbo].[Admin_GetPoliciesDetail]", parameters,
            commandType: CommandType.StoredProcedure);

        return result;

    }
    public async Task<IEnumerable<POSPOnboardingDetailModel>> GetPOSPOnboardingDetail(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        var result = await connection.QueryAsync<POSPOnboardingDetailModel>("[dbo].[Admin_GetPOSPCountOnboardingDetail]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result;
    }

    public async Task<bool> BulkUploadIIBDocument(IIBBulkUploadDocument iIB, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("@UserId", iIB.UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("@IIBStatus", iIB.IIBStatus, DbType.String, ParameterDirection.Input);
        parameters.Add("@IIBUploadStatus", iIB.IIBUploadStatus, DbType.String, ParameterDirection.Input);

        var result = await connection.ExecuteAsync("[dbo].[Admin_UpdateParticularPOSPDetailForIIBDashboard]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return (result > 0);
    }

    public async Task<GetAllExamManagementDetailModel> GetAllExamManagementDetail(string? Searchtext, string? Category, string? StartDate, string? EndDate, int? PageIndex, int? pageSize, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@SearchText", Searchtext, DbType.String, ParameterDirection.Input);
        parameters.Add("@Category", Category, DbType.String, ParameterDirection.Input);
        parameters.Add("@StartDate", StartDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@EndDate", EndDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@CurrentPageIndex", PageIndex, DbType.Int64, ParameterDirection.Input);
        parameters.Add("@CurrentPageSize", pageSize, DbType.Int64, ParameterDirection.Input);
        var result = await connection.QueryMultipleAsync("[dbo].[Admin_GetAllExamManagementDetail]", parameters,
           commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        GetAllExamManagementDetailModel response = new()
        {
            ExamManagementDetailModel = result.Read<ExamManagementDetailModel>(),
            ExamManagementDetailPagingModel = result.Read<ExamManagementDetailPagingModel>()
        };
        return response;
    }

    public async Task<GetAgreementManagementDetailModel> GetAgreementManagementDetail(string? searchText, string? statusId, string? startDate, string? endDate, int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@SearchText", searchText, DbType.String, ParameterDirection.Input);
        parameters.Add("@StatusId", statusId, DbType.String, ParameterDirection.Input);
        parameters.Add("@StartDate", startDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@EndDate", endDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@CurrentPageIndex", pageIndex, DbType.Int64, ParameterDirection.Input);
        parameters.Add("@CurrentPageSize", pageSize, DbType.Int64, ParameterDirection.Input);
        var result = await connection.QueryMultipleAsync("[dbo].[Admin_GetAgreementManagementDetail]", parameters,
            commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        GetAgreementManagementDetailModel response = new()
        {
            AgreementDetailListModel = result.Read<AgreementDetailListModel>(),
            AgreementDetailPagingModel = result.Read<AgreementDetailPagingModel>()

        };
        return response;
    }



    public async Task<AllTrainingManagementDetailsModel> GetAllTrainingManagementDetails(string? searchtext, string? category, string? startDate, string? endDate, int? pageIndex, int? pageSize, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@SearchText", searchtext, DbType.String, ParameterDirection.Input);
        parameters.Add("@Category", category, DbType.String, ParameterDirection.Input);
        parameters.Add("@StartDate", startDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@EndDate", endDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@CurrentPageIndex", pageIndex, DbType.Int64, ParameterDirection.Input);
        parameters.Add("@CurrentPageSize", pageSize, DbType.Int64, ParameterDirection.Input);
        var result = await connection.QueryMultipleAsync("[dbo].[Admin_GetAllTrainingManagementDetails]", parameters,
                commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        AllTrainingManagementDetailsModel response = new()
        {
            AllTrainingManagementDetails = result.Read<AllTrainingManagementDetails>(),
            TrainingManagementDetailsPagingModel = result.Read<TrainingManagementDetailsPagingModel>()

        };
        return response;
    }

    public async Task<IEnumerable<GetUserListVm>> GetUserList(string? roleId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@RoleId", roleId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<GetUserListVm>("[dbo].[Admin_GetUserList]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result;
    }

    public async Task<IEnumerable<GetBUHierarchyVm>> GetBUHierarchy(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        var result = await connection.QueryAsync<GetBUHierarchyVm>("[dbo].[Admin_GetBUHierarchy]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result;
    }

    public async Task<bool> UpdateBUStatus(string? BUId, bool? IsActive, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("@BUId", BUId, DbType.String, ParameterDirection.Input);
        parameters.Add("@IsActive", IsActive, DbType.Boolean, ParameterDirection.Input);
        var result = await connection.ExecuteAsync("[dbo].[Admin_UpdateBUStatus]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return (result > 0);
    }

    public async Task<bool> UpdateActivateDeActivateRole(UpdateActivateDeActivateRoleModel UpdateActivateDeActivateRoleModel, CancellationToken cancellation)
    {
        using var connection = _context.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("RoleId", UpdateActivateDeActivateRoleModel.RoleId, DbType.String, ParameterDirection.Input);
        parameters.Add("IsActive", UpdateActivateDeActivateRoleModel.IsActive, DbType.Boolean, ParameterDirection.Input);

        var result = await connection.ExecuteAsync("[dbo].[Admin_UpdateActivateDeActivateRole]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return (result > 0);

    }

    public async Task<bool> UpdateActivateDeActivateBU(UpdateActivateDeActivateBUModel UpdateActivateDeActivateBUModel, CancellationToken cancellation)
    {
        using var connection = _context.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("BUId", UpdateActivateDeActivateBUModel.BUId, DbType.String, ParameterDirection.Input);
        parameters.Add("IsActive", UpdateActivateDeActivateBUModel.IsActive, DbType.Boolean, ParameterDirection.Input);

        var result = await connection.ExecuteAsync("[dbo].[Admin_UpdateActivateDeActivateBU]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return (result > 0);

    }

    public async Task<LeadDetail> GetParticularLeadDetail(string? LeadId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@LeadId", LeadId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryMultipleAsync("[dbo].[Admin_GetParticularLeadDetail]", parameters,
            commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        LeadDetail response = new()
        {
            PersonalDetail = result.Read<PersonalDetailModel>(),
            VehicleDetail = result.Read<VehicleDetailModel>(),
            policyDetails = result.Read<PolicyDetails>()
        };

        foreach (var proposalRequest in response.PersonalDetail)
        {
            if (!string.IsNullOrEmpty(proposalRequest.ProposalRequestBody))
            {
                var proposalReq = Newtonsoft.Json.JsonConvert.DeserializeObject<ProposaDetails>(proposalRequest.ProposalRequestBody);
                if (proposalReq != null)
                {
                    var nomineeName = string.Empty;
                    if (!string.IsNullOrEmpty(proposalReq.nomineeDetails.nomineeName))
                    {
                        nomineeName = proposalReq.nomineeDetails.nomineeName;
                    }
                    else if (!string.IsNullOrEmpty(proposalReq.nomineeDetails.nomineeFirstName))
                    {
                        nomineeName = proposalReq.nomineeDetails.nomineeFirstName + " " + proposalReq.nomineeDetails.nomineeLastName;
                    }
                    proposalRequest.NomineeName = nomineeName;
                    proposalRequest.NomineeAge = proposalReq.nomineeDetails.nomineeAge;
                    proposalRequest.NomineeRelation = proposalReq.nomineeDetails.nomineeRelation;
                }
            }
        }

        return response;
    }

    public async Task<bool> ResetAdminUserAccountDetail(string UserId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", UserId, DbType.String, ParameterDirection.Input);
        var result = await connection.ExecuteAsync("[dbo].[Admin_ResetAdminDatabaseAccount]", parameters,
            commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return (result > 0);
    }

    public async Task<IEnumerable<GetParticularLeadUploadedDocumentVm>> GetParticularLeadUploadedDocument(string? UserId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", UserId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<GetParticularLeadUploadedDocumentVm>("[dbo].[Admin_GetParticularLeadUploadedDocument]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result;
    }

    public async Task<bool> UpdateDocumentStatus(UpdateDocumentStatusCommand updateDocumentStatus, CancellationToken cancellation)
    {
        using var connection = _context.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("DocumentId", updateDocumentStatus.DocumentId, DbType.String, ParameterDirection.Input);
        parameters.Add("IsApprove", updateDocumentStatus.IsApprove, DbType.Boolean, ParameterDirection.Input);
        parameters.Add("BackOfficeRemark", updateDocumentStatus.BackOfficeRemark, DbType.String, ParameterDirection.Input);

        var result = await connection.ExecuteAsync("[dbo].[Admin_UpdateDocumentStatus]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return (result > 0);

    }
    public async Task<bool> UpdateActivateDeActivateExamManagement(UpdateActivateDeActivateExamManagementModel UpdateActivateDeActivateExamManagementModel, CancellationToken cancellation)
    {
        using var connection = _context.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("QuestionId", UpdateActivateDeActivateExamManagementModel.QuestionId, DbType.String, ParameterDirection.Input);
        parameters.Add("IsActive", UpdateActivateDeActivateExamManagementModel.IsActive, DbType.Boolean, ParameterDirection.Input);

        var result = await connection.ExecuteAsync("[dbo].[Admin_UpdateActivateDeActivateExamManagement]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return (result > 0);

    }

    public async Task<bool> UpdateActivateDeActivateTrainingManagement(UpdateActivateDeActivateTrainingManagementModel UpdateActivateDeActivateTrainingManagementModel, CancellationToken cancellation)
    {
        using var connection = _context.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("TrainingMaterialId", UpdateActivateDeActivateTrainingManagementModel.TrainingMaterialId, DbType.String, ParameterDirection.Input);
        parameters.Add("IsActive", UpdateActivateDeActivateTrainingManagementModel.IsActive, DbType.Boolean, ParameterDirection.Input);

        var result = await connection.ExecuteAsync("[dbo].[Admin_UpdateActivateDeActivateTrainingManagement]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return (result > 0);

    }

    public async Task<GetPOSPDocumentByIdVm> GetPOSPDocumentById(string? documentId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@DocumentId", documentId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<GetPOSPDocumentByIdVm>("[dbo].[Admin_GetFileTypeByDocumentId]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        if (result.Any())
        {
            result.FirstOrDefault().ImageStream = await _mongodbService.MongoDownload(documentId);
        }
        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<GetParticularUserRoleMappingDetailModel>> GetParticularUserRoleMappingDetail(string UserRoleMappingId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@UserRoleMappingId", UserRoleMappingId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<GetParticularUserRoleMappingDetailModel>("[dbo].[Admin_GetParticularUserRoleMappingDetail]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        if (result.Any() && !string.IsNullOrWhiteSpace(result.FirstOrDefault()?.DocumentId))
        {
            result.FirstOrDefault().ImageStream = await _mongodbService.MongoDownload(result.FirstOrDefault()?.DocumentId);
        }
        return result;
    }

    public async Task<bool> InsertStampInstruction(string? SrNo, string? Instruction, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("@SrNo", SrNo, DbType.String, ParameterDirection.Input);
        parameters.Add("@Instruction", Instruction, DbType.String, ParameterDirection.Input);
        var result = await connection.ExecuteAsync("[dbo].[Admin_InsertStampInstruction]", parameters,
                commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return (result > 0);
    }

    public async Task<bool> InsertStampData(string? SrNo, string? StampData, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("@SrNo", SrNo, DbType.String, ParameterDirection.Input);
        parameters.Add("@StampData", StampData, DbType.String, ParameterDirection.Input);
        var result = await connection.ExecuteAsync("[dbo].[Admin_InsertStampData]", parameters,
                commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return (result > 0);
    }

    public async Task<bool> ExampBulkUpload(ExamBulkUploadModel bulkUploadModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        int Result = 0;
        List<ExamBulkUploadCommandInsertModel> lstBulkUploadCmd = new List<ExamBulkUploadCommandInsertModel>(); ;
        var parameters = new DynamicParameters();
        try
        {
            parameters.Add("ExamModuleType", !string.IsNullOrWhiteSpace(bulkUploadModel.ExamModuleType) ? bulkUploadModel.ExamModuleType : "", DbType.String, ParameterDirection.Input);
            parameters.Add("QuestionValue", !string.IsNullOrWhiteSpace(bulkUploadModel.QuestionValue) ? bulkUploadModel.QuestionValue : "", DbType.String, ParameterDirection.Input);
            parameters.Add("IsActive", bulkUploadModel.IsActive, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("CreatedBy", !string.IsNullOrWhiteSpace(bulkUploadModel.CreatedBy) ? bulkUploadModel.CreatedBy : "", DbType.String, ParameterDirection.Input);
            var resultIdentityRoleId = await connection.QueryAsync<UserRoleIdentity>("[dbo].[Admin_InsertExamBulkUploadQuestion]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            if (resultIdentityRoleId.Any())
            {
                {
                    var questionId = resultIdentityRoleId.FirstOrDefault().IdentityRoleId;
                    lstBulkUploadCmd = bulkUploadModel.ExamBulkUploadCommandInsertModel.ToList();
                    foreach (ExamBulkUploadCommandInsertModel command in lstBulkUploadCmd)
                    {
                        var parametersMapping = new DynamicParameters();
                        parametersMapping.Add("QuestionId", questionId, DbType.String, ParameterDirection.Input);
                        parametersMapping.Add("OptionValue", command.OptionValue, DbType.String, ParameterDirection.Input);
                        parametersMapping.Add("IsCorrectAnswer", command.IsCorrectAnswer, DbType.Boolean, ParameterDirection.Input);
                        Result = await connection.ExecuteAsync("[dbo].[Admin_InsertExamBulkUploadOption]", parametersMapping, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
                        Result++;
                    }
                }
            }
            return Result > 0;
        }
        catch (TransactionException ex)
        {
            throw new Exception(ex.Message, ex);

        }
    }

    public async Task<IEnumerable<GetRoleLevelByBUIdforRoleCreationResponseModel>> GetRoleLevelByBUIdforRoleCreation(GetRoleLevelByBUIdforRoleCreationInputModel inputModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@BUId", inputModel.BUId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<GetRoleLevelByBUIdforRoleCreationResponseModel>("[dbo].[Admin_GetRoleLevelByBUIdforRoleCreation]", parameters,
                     commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result;
    }

    public async Task<bool> UpdatePersonalDetails(UpdatePersonalDetailsCommand updatePersonalDetails, CancellationToken cancellation)
    {

        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();

        parameters.Add("UserId", updatePersonalDetails.UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("AlternateNumber", updatePersonalDetails.AlternateNumber, DbType.String, ParameterDirection.Input);
        parameters.Add("AddressLine1", updatePersonalDetails.AddressLine1, DbType.String, ParameterDirection.Input);
        parameters.Add("AddressLine2", updatePersonalDetails.AddressLine2, DbType.String, ParameterDirection.Input);
        parameters.Add("Pincode", updatePersonalDetails.Pincode, DbType.Int64, ParameterDirection.Input);
        parameters.Add("City", updatePersonalDetails.City, DbType.String, ParameterDirection.Input);
        parameters.Add("State", updatePersonalDetails.State, DbType.String, ParameterDirection.Input);
        parameters.Add("EducationalQualification", updatePersonalDetails.EducationalQualification, DbType.String, ParameterDirection.Input);
        parameters.Add("InsuranceProductsofInterestTypeId", updatePersonalDetails.ICName, DbType.String, ParameterDirection.Input);
        parameters.Add("SellingExperience", updatePersonalDetails.InsuranceSellingExperience, DbType.String, ParameterDirection.Input);
        parameters.Add("ProductCategoryId", updatePersonalDetails.ProductCategoryId, DbType.String, ParameterDirection.Input);
        parameters.Add("POSPSource", updatePersonalDetails.POSPSourceMode, DbType.Boolean, ParameterDirection.Input);
        parameters.Add("ICName", updatePersonalDetails.ICName, DbType.String, ParameterDirection.Input);
        parameters.Add("PremiumSold", updatePersonalDetails.PremiumSold, DbType.String, ParameterDirection.Input);
        parameters.Add("IsSelling", updatePersonalDetails.IsSelling, DbType.String, ParameterDirection.Input);
        parameters.Add("NOCAvailable", updatePersonalDetails.NOCAvailable, DbType.String, ParameterDirection.Input);

        var result = await connection.ExecuteAsync("[dbo].[Admin_UpdatePersonalDetail]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return (result > 0);
    }

    public async Task<bool> DeleteTrainingManagementDetail(string TrainingMaterialId, CancellationToken cancellationToken)
    {
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("TrainingMaterialId", TrainingMaterialId, DbType.String, ParameterDirection.Input);
            var result = await connection.ExecuteAsync("[dbo].[Admin_DeleteTrainingManagementDetail]", parameters,
                commandType: CommandType.StoredProcedure).ConfigureAwait(false);

            return (result > 0);

        }
    }

    public async Task<bool> DeleteExamManagementDetail(string? QuestionId, CancellationToken cancellationToken)
    {
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("QuestionId", QuestionId, DbType.String, ParameterDirection.Input);
            var result = await connection.ExecuteAsync("[dbo].[Admin_DeleteExamManagementDetail]", parameters,
                commandType: CommandType.StoredProcedure).ConfigureAwait(false);

            return (result > 0);

        }
    }

    public async Task<bool> DeletePoliciesDetail(string POSPId, CancellationToken cancellationToken)
    {
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("POSPId", POSPId, DbType.String, ParameterDirection.Input);
            var result = await connection.ExecuteAsync("[dbo].[Admin_DeletePoliciesDetail]", parameters,
                commandType: CommandType.StoredProcedure).ConfigureAwait(false);

            return (result > 0);

        }
    }

    public async Task<StampCountResponseModel> GetStampCountQuery(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var result = await connection.QueryAsync<StampCountResponseModel>("[dbo].[Admin_GetStampCountForAgreementManagement]", commandType: CommandType.StoredProcedure);
        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<CheckUserExistOrNotModel>> CheckUserExistOrNot(string? UserId, string? EmpId, string? MobileNo, string? EmailId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@UserId", UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("@EmpId", EmpId, DbType.String, ParameterDirection.Input);
        parameters.Add("@MobileNo", MobileNo, DbType.String, ParameterDirection.Input);
        parameters.Add("@EmailId", EmailId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<CheckUserExistOrNotModel>("[dbo].[Admin_CheckUserExistOrNot]", parameters,
            commandType: CommandType.StoredProcedure);

        return result;
    }

    public async Task<IEnumerable<GetPOSPStageDetailModel>> GetPOSPStageDetail(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var result = await connection.QueryAsync<GetPOSPStageDetailModel>("[dbo].[Admin_GetPOSPStageDetail]", commandType: CommandType.StoredProcedure);
        return result;
    }


    public async Task<bool> UploadTrainingFile(string? TrainingModuleType, string? LessionTitle, string? DocumentId, string? fileName, string MaterialFormatType, string VideoDuration, string LessonNumber, string CreatedBy, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("TrainingModuleType", !string.IsNullOrWhiteSpace(TrainingModuleType) ? TrainingModuleType : "", DbType.String, ParameterDirection.Input);
        parameters.Add("LessionTitle", !string.IsNullOrWhiteSpace(LessionTitle) ? LessionTitle : "", DbType.String, ParameterDirection.Input);
        parameters.Add("FileName", !string.IsNullOrWhiteSpace(fileName) ? fileName : "", DbType.String, ParameterDirection.Input);
        parameters.Add("DocumentId", !string.IsNullOrWhiteSpace(DocumentId) ? DocumentId : "", DbType.String, ParameterDirection.Input);
        parameters.Add("MaterialFormatType", !string.IsNullOrWhiteSpace(MaterialFormatType) ? MaterialFormatType : "", DbType.String, ParameterDirection.Input);
        parameters.Add("VideoDuration", !string.IsNullOrWhiteSpace(VideoDuration) ? VideoDuration : "", DbType.String, ParameterDirection.Input);
        parameters.Add("LessonNumber", !string.IsNullOrWhiteSpace(LessonNumber) ? LessonNumber : "", DbType.String, ParameterDirection.Input);
        parameters.Add("CreatedBy", CreatedBy, DbType.String, ParameterDirection.Input);
        var result = await connection.ExecuteAsync("[dbo].[Admin_InsertTariningMaterialDetail]", parameters,
            commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return (result > 0);
    }

    public async Task<bool> UpdateAgreementStatusByUserId(UpdateAgreementStatusByUserIdModel UpdateAgreementStatusByUserIdModel, CancellationToken cancellation)
    {
        using var connection = _context.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("UserId", UpdateAgreementStatusByUserIdModel.UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("ProcessType", UpdateAgreementStatusByUserIdModel.ProcessType, DbType.String, ParameterDirection.Input);

        var result = await connection.ExecuteAsync("[dbo].[Admin_UpdateAgreementStatusByUserId]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return (result > 0);

    }

    public async Task<IEnumerable<GetExamParticularQuestionDetailModel>> GetExamParticularQuestionDetail(string? questionId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@QuestionId", questionId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<GetExamParticularQuestionDetailModel>("[dbo].[Admin_GetExamParticularQuestionDetail]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result;
    }

    public async Task<bool> UpdateExamParticularQuestion(UpdateExamParticularQuestionModel model, CancellationToken cancellation)
    {
        try
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("QuestionId", model.QuestionId, DbType.String, ParameterDirection.Input);
            parameters.Add("OptionId1", model.OptionId1, DbType.String, ParameterDirection.Input);
            parameters.Add("OptionValue1", model.OptionValue1, DbType.String, ParameterDirection.Input);
            parameters.Add("OptionId2", model.OptionId2, DbType.String, ParameterDirection.Input);
            parameters.Add("OptionValue2", model.OptionValue2, DbType.String, ParameterDirection.Input);
            parameters.Add("OptionId3", model.OptionId3, DbType.String, ParameterDirection.Input);
            parameters.Add("OptionValue3", model.OptionValue3, DbType.String, ParameterDirection.Input);
            parameters.Add("OptionId4", model.OptionId4, DbType.String, ParameterDirection.Input);
            parameters.Add("OptionValue4", model.OptionValue4, DbType.String, ParameterDirection.Input);
            parameters.Add("CorrectAnswerIndex", model.CorrectAnswerIndex, DbType.Int64, ParameterDirection.Input);
            var result = await connection.ExecuteAsync("[dbo].[Admin_UpdateExamParticularQuestionDetail]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            return (result > 0);
        }
        catch (Exception)
        {
            throw;
        }

    }
    public async Task<IEnumerable<TotalSalesResponseModel>> GetTotalSalesDetail(GetTotalSalesDetailQuery request, CancellationToken cancellationToken)
    {
        request.UserId = !string.IsNullOrEmpty(request.UserId) ? request.UserId : _applicationClaims.GetUserId();
        var ImageLogoUrl = _config.GetSection("WebUrl").GetSection("prefixUrl").Value + "images/insurance/";
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();

        parameters.Add("@UserId", request.UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("@StartDate", request.StartDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@EndDate", request.EndDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@ImageLogoUrl", ImageLogoUrl, DbType.String, ParameterDirection.Input);

        var result = await connection.QueryAsync<TotalSalesResponseModel>("[dbo].[Admin_GetTotalSales]", parameters, commandType: CommandType.StoredProcedure);
        return result;
    }

    public async Task<CardsDetailResponseModel> GetCardsDetail(string? userId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();

        parameters.Add("@UserId", userId, DbType.String, ParameterDirection.Input);

        var result = await connection.QueryMultipleAsync("[dbo].[Admin_GetCardsDetail]", parameters,
            commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        CardsDetailResponseModel response = new()
        {
            //userCardModel = result.Read<UserCardModel>(),
            contactCardModel = result.Read<ContactCardModel>(),
            supportSectionModel = result.Read<SupportSectionModel>()
        };
        return response;
    }

    public async Task<IEnumerable<RelationshipManagerModel>> GetRelationshipManager(string? userId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();

        parameters.Add("@UserId", userId, DbType.String, ParameterDirection.Input);

        var result = await connection.QueryAsync<RelationshipManagerModel>("[dbo].[Admin_GetRelationshipManager]", parameters,
            commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result;
    }


    public async Task<bool> RequestForEditProfile(RequestForEditProfileInputModel model, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        if (model.CreatedBy == "string")
            model.CreatedBy = "";
        try
        {
            parameters.Add("UserId", model?.UserId, DbType.String, ParameterDirection.Input);
            parameters.Add("RequestType", model?.RequestType, DbType.String, ParameterDirection.Input);
            parameters.Add("NewRequestTypeContent", model?.NewRequestTypeContent, DbType.String, ParameterDirection.Input);
            parameters.Add("CreatedBy", !string.IsNullOrWhiteSpace(model?.CreatedBy) ? model?.CreatedBy : "", DbType.String, ParameterDirection.Input);
            var result = await connection.ExecuteAsync("[dbo].[Admin_InsertRequestForEditProfile]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            return (result > 0);
        }
        catch (TransactionException ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }




    public async Task<GetCustomersDetailResponseModel> GetCustomersDetail(GetCustomersDetailInputModel GetCustomersDetailInputModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@CustomerType", GetCustomersDetailInputModel.CustomerType, DbType.String, ParameterDirection.Input);
        parameters.Add("@SearchText", GetCustomersDetailInputModel.SearchText, DbType.String, ParameterDirection.Input);
        parameters.Add("@PolicyType", GetCustomersDetailInputModel.PolicyType, DbType.String, ParameterDirection.Input);
        parameters.Add("@StartDate", GetCustomersDetailInputModel.StartDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@EndDate", GetCustomersDetailInputModel.EndDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@CurrentPageIndex", GetCustomersDetailInputModel.CurrentPageIndex, DbType.Int64, ParameterDirection.Input);
        parameters.Add("@CurrentPageSize", GetCustomersDetailInputModel.CurrentPageSize, DbType.Int64, ParameterDirection.Input);

        var result = await connection.QueryMultipleAsync("[dbo].[Admin_GetCustomerDetail]", parameters,
            commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        GetCustomersDetailResponseModel response = new()
        {
            GetCustomersDetailModel = result.Read<GetCustomersDetailModel>(),
            CustomersDetailPagingModel = result.Read<CustomersDetailPagingModel>()

        };
        return response;
    }

    public async Task<bool> UpdateActivePOSPAccountDetail(UpdateActivePOSPAccountDetailCommand updateActivePOSPAccountDetail, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("POSPUserId", updateActivePOSPAccountDetail.POSPUserId, DbType.String, ParameterDirection.Input);
        parameters.Add("PreSaleUserId", updateActivePOSPAccountDetail.PreSaleUserId, DbType.String, ParameterDirection.Input);
        parameters.Add("PostSaleUserId", updateActivePOSPAccountDetail.PostSaleUserId, DbType.String, ParameterDirection.Input);
        parameters.Add("MarketingUserId", updateActivePOSPAccountDetail.MarketingUserId, DbType.String, ParameterDirection.Input);
        parameters.Add("ClaimUserId", updateActivePOSPAccountDetail.ClaimUserId, DbType.String, ParameterDirection.Input);
        parameters.Add("SourcedBy", updateActivePOSPAccountDetail.SourcedBy, DbType.String, ParameterDirection.Input);
        parameters.Add("CreatedBy", updateActivePOSPAccountDetail.CreatedBy, DbType.String, ParameterDirection.Input);
        parameters.Add("ServicedBy", updateActivePOSPAccountDetail.ServicedBy, DbType.String, ParameterDirection.Input);

        var result = await connection.ExecuteAsync("[dbo].[Admin_UpdateActivePOSPAccountDetail]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return (result > 0);
    }

    public async Task<IEnumerable<GetInsuranceTypeVm>> GetInsuranceType(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var result = await connection.QueryAsync<GetInsuranceTypeVm>("[dbo].[Admin_GetInsuranceType]", commandType: CommandType.StoredProcedure);
        return result;
    }

    public async Task<IEnumerable<GetLeadStageVm>> GetLeadStage(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var result = await connection.QueryAsync<GetLeadStageVm>("[dbo].[Admin_GetLeadStage]", commandType: CommandType.StoredProcedure);
        return result;
    }

    public async Task<IEnumerable<GetRecipientListVm>> GetRecipientList(string? searchtext, string? RecipientType, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@SearchText", searchtext, DbType.String, ParameterDirection.Input);
        parameters.Add("@RecipientType", RecipientType, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<GetRecipientListVm>("[dbo].[Admin_GetRecipientList]", parameters, commandType: CommandType.StoredProcedure);
        return result;
    }

    public async Task<GetNotificationListResponseModel> GetNotificationList(string? SearchText, string? RecipientTypeId, string? StartDate, string? EndDate, int? CurrentPageIndex, int? CurrentPageSize, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@SearchText", SearchText, DbType.String, ParameterDirection.Input);
        parameters.Add("@RecipientTypeId", RecipientTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("@StartDate", StartDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@EndDate", EndDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@CurrentPageIndex", CurrentPageIndex, DbType.String, ParameterDirection.Input);
        parameters.Add("@CurrentPageSize", CurrentPageSize, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryMultipleAsync("[dbo].[Admin_GetNotificationList]",
            parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        GetNotificationListResponseModel response = new()
        {
            NotificationListModel = result.Read<NotificationListModel>(),
            NotificationPagingModel = result.Read<NotificationPagingModel>()

        };
        return response;
    }

    public async Task<bool> InsertNotification(InsertNotificationCommand insertNotificationCommand, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("AlertTypeId", insertNotificationCommand.AlertTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("RecipientId", insertNotificationCommand.RecipientId, DbType.String, ParameterDirection.Input);
        parameters.Add("RecipientUserids", insertNotificationCommand.RecipientUserids, DbType.String, ParameterDirection.Input);
        parameters.Add("NotificationCategory", insertNotificationCommand.NotificationCategory, DbType.String, ParameterDirection.Input);
        parameters.Add("NotificationOrigin", insertNotificationCommand.NotificationOrigin, DbType.String, ParameterDirection.Input);
        parameters.Add("NotificationTitle", insertNotificationCommand.NotificationTitle, DbType.String, ParameterDirection.Input);
        parameters.Add("Description", insertNotificationCommand.Description, DbType.String, ParameterDirection.Input);
        parameters.Add("NotificationEventId", insertNotificationCommand.NotificationEventId, DbType.String, ParameterDirection.Input);
        parameters.Add("IsPublished", insertNotificationCommand.IsPublished, DbType.Boolean, ParameterDirection.Input);

        var result = await connection.ExecuteAsync("[dbo].[Admin_InsertNotification]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return (result > 0);
    }

    public async Task<bool> EditNotification(EditNotificationCommand editNotification, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("@NotificationId", editNotification.NotificationId, DbType.String, ParameterDirection.Input);
        parameters.Add("@AlertTypeId", editNotification.AlertTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("@RecipientId", editNotification.RecipientId, DbType.String, ParameterDirection.Input);
        parameters.Add("@UserIds", editNotification.UserIds, DbType.String, ParameterDirection.Input);
        parameters.Add("@Title", editNotification.Title, DbType.String, ParameterDirection.Input);
        parameters.Add("@Description", editNotification.Description, DbType.String, ParameterDirection.Input);

        var result = await connection.ExecuteAsync("[dbo].[Admin_EditNotification]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return (result > 0);
    }

    public async Task<InsertLeadDetailsModel> InsertLeadDetails(string? LeadName, string? LeadPhoneNumber, string? LeadEmailId, string? UserId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("LeadName", LeadName, DbType.String, ParameterDirection.Input);
        parameters.Add("LeadPhoneNumber", LeadPhoneNumber, DbType.String, ParameterDirection.Input);
        parameters.Add("LeadEmailId", LeadEmailId, DbType.String, ParameterDirection.Input);
        parameters.Add("UserId", UserId, DbType.String, ParameterDirection.Input);

        var result = await connection.QueryAsync<InsertLeadDetailsModel>("[dbo].[Admin_InsertLeadDetails]", parameters,
                  commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        if (result.Any())
        {

            return result.FirstOrDefault();
        }

        return default;
    }

    public async Task<bool> PublishNotification(PublishNotificationCommand publish, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();

        parameters.Add("NotificationId", publish.NotificationId, DbType.String, ParameterDirection.Input);

        var result = await connection.ExecuteAsync("[dbo].[Admin_PublishNotification]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return (result > 0);
    }

    public async Task<IEnumerable<DownloadAgreementResponse>> DownloadAgreement(string? POSPId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("POSPId", POSPId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<DownloadAgreementResponse>("[dbo].[Admin_DownloadAgreement]", parameters,
                     commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        result.FirstOrDefault().Image64 = await _mongodbService.MongoAgreementDownload(result.FirstOrDefault().DocumentId);

        return result;
    }

    public async Task<bool> DeactivateUserById(DeactivateUserByIdCommand deactivateUserByIdCommand, CancellationToken cancellation)
    {
        using var connection = _context.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("UserId", deactivateUserByIdCommand.UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("IsActive", deactivateUserByIdCommand.IsActive, DbType.Boolean, ParameterDirection.Input);
        parameters.Add("DeactivatePOSPId", deactivateUserByIdCommand.DeactivatePOSPId, DbType.String, ParameterDirection.Input);


        var result = await connection.ExecuteAsync("[dbo].[Admin_DeactivatePOSPUserById]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        return (result > 0);

    }

    public async Task<IEnumerable<GetUserCategoryVm>> GetUserCategory(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();

        var result = await connection.QueryAsync<GetUserCategoryVm>("[dbo].[Admin_GetUserCategory]", commandType: CommandType.StoredProcedure);
        return result;
    }

    public async Task<GetAssistedBUDetailsResponseModel> GetAssistedBUDetails(string? RoleId, string? UserId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("RoleId", RoleId, DbType.String, ParameterDirection.Input);
        parameters.Add("UserId", UserId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryMultipleAsync("[dbo].[Admin_GetAssistedBUDetails]",
            parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        GetAssistedBUDetailsResponseModel response = new()
        {
            AssistedBUDetails = result.Read<AssistedBUDetails>(),
            AssistedSelectedBUDetails = result.Read<AssistedSelectedBUDetails>()

        };
        return response;
    }
    public async Task<GetDeactivatedUserReponseModel> GetDeactivatedUser(GetDeactivatedUserQuery getDeactivatedUserQuery, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@SearchText", getDeactivatedUserQuery.SearchText, DbType.String, ParameterDirection.Input);
        parameters.Add("@RelationManagerId", getDeactivatedUserQuery.RelationManagerId, DbType.String, ParameterDirection.Input);
        parameters.Add("@StartDate", getDeactivatedUserQuery.StartDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@EndDate", getDeactivatedUserQuery.EndDate, DbType.String, ParameterDirection.Input);
        parameters.Add("@CurrentPageIndex", getDeactivatedUserQuery.CurrentPageIndex, DbType.String, ParameterDirection.Input);
        parameters.Add("@CurrentPageSize", getDeactivatedUserQuery.CurrentPageSize, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<POSPUserDetail>("[dbo].[Admin_GetDeactivatedUser]", parameters, commandType: CommandType.StoredProcedure);
        if (result.Any())
        {
            int totalRecords = result.FirstOrDefault().TotalRecord;
            GetDeactivatedUserReponseModel resp = new()
            {
                POSPUserDetail = result,
                TotalRecord = totalRecords,
            };
            return resp;
        }

        return default;
    }

    public async Task<IEnumerable<GetAllBUUserRoleVm>> GetAllBUUserRole(GetAllBUUserRoleQuery bUUserQuery, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@BUId", bUUserQuery.BUId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<GetAllBUUserRoleVm>("[dbo].[Admin_GetAllBUUser]", parameters,
                     commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result;
    }

    public async Task<IEnumerable<GetAllSharedReportingRoleVm>> GetAllSharedReportingRole(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();

        var result = await connection.QueryAsync<GetAllSharedReportingRoleVm>("[dbo].[Admin_GetSharedWithCentralUser]", commandType: CommandType.StoredProcedure);
        return result;
    }

    public async Task<IEnumerable<CheckForRoleVm>> CheckForRole(CheckForRoleQuery checkForRoleQuery, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@UserId", checkForRoleQuery.UserId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<CheckForRoleVm>("[dbo].[Admin_CheckForRole]", parameters,
                     commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result;
    }
    /// <summary>
    /// get all bu as per heirarchy of user
    /// </summary>
    /// <param name="allBUDetailModel"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<AllBUDetaiByUserIdModel>> GetAllBuDetailByUserID(GetAllBUDetailsByUserIDQuery allBUDetailModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@UserID", allBUDetailModel.UserID, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryMultipleAsync("[dbo].[Admin_GetAllBUDetailBYUserID]", parameters, commandType: CommandType.StoredProcedure);
        if (result is not null)
        {
            IEnumerable<AllBUDetaiByUserIdModel> response = result.Read<AllBUDetaiByUserIdModel>();
            return response;
        }
        return default;
    }

    /// <summary>
    /// All Posp stage count
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<GetAllPOSPCountDetailModel>> GetAllPOSPCountDetail(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        var result = await connection.QueryAsync<GetAllPOSPCountDetailModel>("[dbo].[Admin_GetAllPOSPStageCount]", parameters, commandType: CommandType.StoredProcedure);
        return result;
    }
    /// <summary>
    /// All Posp Back office pending report
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<GetAllPOSPBackOficePendingReportModel>> GetAllPOSPBackOficePendingReport(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        var result = await connection.QueryAsync<GetAllPOSPBackOficePendingReportModel>("[dbo].[Admin_AllPOSPBackOfficePendingReport]", parameters, commandType: CommandType.StoredProcedure);
        return result;
    }

    public async Task<ResetUserDetailByIdModel> ResetUserIdDetail(ResetUserAccountDetailQuery resetUserAccountDetailQuery, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", resetUserAccountDetailQuery.UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("RejectionReasonId", resetUserAccountDetailQuery.PanRejectionReasonsCSV, DbType.String, ParameterDirection.Input);
        parameters.Add("RejectedBy", _applicationClaims.GetUserId(), DbType.String, ParameterDirection.Input);
        var result = await connection.QueryMultipleAsync("[dbo].[Admin_ResetDatabaseAccountByUserId]", parameters,commandType: CommandType.StoredProcedure);
        if (result is not null)
        {
            IEnumerable<DocumentDetail> documentDetail = result.Read<DocumentDetail>();
            IEnumerable<ResetUserDetailsForPanVerification> userDetails = result.Read<ResetUserDetailsForPanVerification>();
            var responce = new ResetUserDetailByIdModel()
            {
                documentDetail = documentDetail,
                userDetail = userDetails
            };
            return responce;
        }
        return default;
    }
}



