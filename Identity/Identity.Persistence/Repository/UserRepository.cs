using Dapper;
using Identity.Core.Contracts.Persistence;
using Identity.Core.Features.User.Commands.UpdateUserPasswordFromUserLinkCommand;
using Identity.Core.Features.User.Queries.GetAllRelationshipManager;
using Identity.Core.Features.User.Queries.GetPOSPSourceType;
using Identity.Domain.Authentication;
using Identity.Domain.Password;
using Identity.Domain.Roles;
using Identity.Domain.User;
using Identity.Domain.UserAddressDetail;
using Identity.Domain.UserBankDetail;
using Identity.Domain.UserCreation;
using Identity.Domain.UserInquiryDetail;
using Identity.Domain.UserPersonalDetail;
using Identity.Persistence.Configuration;
using Identity.Persistence.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Transactions;
using ThirdPartyUtilities.Abstraction;
using ThirdPartyUtilities.Models.JWT;
using ThirdPartyUtilities.Models.Signzy;

namespace Identity.Persistence.Repository;
public class UserRepository : IUserRepository
{
    private readonly ApplicationDBContext _context;
    private readonly ISignzyService _signzyService;
    private readonly ISmsService _sMSService;
    private readonly IEmailService _emailService;
    private readonly IMongoDBService _mongodbService;
    private readonly TokenSettings _tokenSettings;
    private readonly IConfiguration _config;

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="context"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public UserRepository(ApplicationDBContext context, ISignzyService signzyService, ISmsService sMSService, IEmailService emailService, IMongoDBService mongodbService, IOptions<TokenSettings> tokenSettings, IConfiguration config)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _sMSService = sMSService ?? throw new ArgumentNullException(nameof(sMSService));
        _signzyService = signzyService ?? throw new ArgumentNullException(nameof(signzyService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _mongodbService = mongodbService ?? throw new ArgumentNullException(nameof(mongodbService));
        _tokenSettings = tokenSettings.Value;
        _config = config;
    }

    /// <summary>
    /// InsertUserCreationDetail
    /// </summary>
    /// <param name="userCreationModel"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<UserCreateResponseModel> InsertUserCreationDetail(UserCreationModel userCreationModel)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserName", userCreationModel.UserName, DbType.String, ParameterDirection.Input);
        parameters.Add("EmailId", userCreationModel.EmailId, DbType.String, ParameterDirection.Input);
        parameters.Add("MobileNo", userCreationModel.MobileNo, DbType.String, ParameterDirection.Input);
        parameters.Add("BackOfficeUserId", userCreationModel.BackOfficeUserId, DbType.String, ParameterDirection.Input);
        parameters.Add("ReferralUserId", userCreationModel.ReferralUserId, DbType.String, ParameterDirection.Input);
        parameters.Add("Password", Hash256.Hash256Password(_config.GetSection("Password").GetSection("DefaultPassword").Value.ToString()));

        //BackOfficeUserId NULL or Values...
        var result = await connection.QueryAsync<UserCreateResponseModel>("[dbo].[Identity_InsertUser]", parameters, commandType: CommandType.StoredProcedure);
        if (result.FirstOrDefault().IsUserExists)
        {
            return default;
        }
        var userDetail = result.FirstOrDefault();
        if (userDetail.IsUserExists == true)
        {
            return default;
        }
        else
        {
            userDetail = result.FirstOrDefault();
            await SendSMS(userCreationModel.MobileNo, userDetail.UserId);
            await SendEmail(userCreationModel.EmailId, userDetail.UserId, userCreationModel.Environment);
            return result.FirstOrDefault();
        }
        //if (result.Any())
        //{
        //    //var userDetail = result.FirstOrDefault();
        //    await SendSMS(userCreationModel.MobileNo, userDetail.UserId).ConfigureAwait(false);
        //    await SendEmail(userCreationModel.EmailId, userDetail.UserId).ConfigureAwait(false);

        //    return result.FirstOrDefault();
        //}

        //return default;
    }


    /// <summary>
    /// UpdateUserPersonalDetail
    /// </summary>
    /// <param name="userCreationModel"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> UpdateUserPersonalDetail(UserPersonalDetailModel userPersonalDetailModel)
    {

        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", userPersonalDetailModel.UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("Gender", userPersonalDetailModel.Gender, DbType.String, ParameterDirection.Input);
        parameters.Add("FatherName", userPersonalDetailModel.FatherName, DbType.String, ParameterDirection.Input);
        parameters.Add("DOB", userPersonalDetailModel.DOB, DbType.String, ParameterDirection.Input);
        parameters.Add("Email", userPersonalDetailModel.Email, DbType.String, ParameterDirection.Input);
        parameters.Add("AlternateContactNo", userPersonalDetailModel.AlternateContactNo, DbType.String, ParameterDirection.Input);
        parameters.Add("AadhaarNumber", userPersonalDetailModel.AadhaarNumber, DbType.String, ParameterDirection.Input);
        parameters.Add("IsNameDifferentOnDocument", userPersonalDetailModel.IsNameDifferentOnDocument, DbType.Boolean, ParameterDirection.Input);
        parameters.Add("NameDifferentOnDocument", userPersonalDetailModel.NameDifferentOnDocument, DbType.String, ParameterDirection.Input);
        parameters.Add("NameDifferentOnDocumentFilePath", userPersonalDetailModel.NameDifferentOnDocumentFilePath, DbType.String, ParameterDirection.Input);
        parameters.Add("AliasName", userPersonalDetailModel.AliasName, DbType.String, ParameterDirection.Input);
        parameters.Add("AddressLine1", userPersonalDetailModel.AddressLine1, DbType.String, ParameterDirection.Input);
        parameters.Add("AddressLine2", userPersonalDetailModel.AddressLine2, DbType.String, ParameterDirection.Input);
        parameters.Add("Pincode", userPersonalDetailModel.Pincode, DbType.Int64, ParameterDirection.Input);
        parameters.Add("CityId", userPersonalDetailModel.CityId, DbType.String, ParameterDirection.Input);
        parameters.Add("StateId", userPersonalDetailModel.StateId, DbType.String, ParameterDirection.Input);
        parameters.Add("EducationQualificationTypeId", userPersonalDetailModel.EducationQualificationTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("InsuranceSellingExperienceRangeId", userPersonalDetailModel.InsuranceSellingExperienceRangeId, DbType.String, ParameterDirection.Input);
        parameters.Add("InsuranceProductsofInterestTypeId", userPersonalDetailModel.InsuranceProductsofInterestTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("POSPSourceMode", userPersonalDetailModel.POSPSourceMode, DbType.Boolean, ParameterDirection.Input);
        parameters.Add("POSPSourceTypeId", userPersonalDetailModel.POSPSourceTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("SourcedByUserId", userPersonalDetailModel.SourcedByUserId, DbType.String, ParameterDirection.Input);
        parameters.Add("ServicedByUserId", userPersonalDetailModel.ServicedByUserId, DbType.String, ParameterDirection.Input);
        parameters.Add("NOCAvailable", userPersonalDetailModel.NOCAvailable.ToUpper(), DbType.String, ParameterDirection.Input);
        parameters.Add("IsSelling", userPersonalDetailModel.IsSelling, DbType.String, ParameterDirection.Input);
        parameters.Add("IsDraft", userPersonalDetailModel.IsDraft, DbType.Boolean, ParameterDirection.Input);
        parameters.Add("InsuranceCompanyofInterestTypeId", userPersonalDetailModel.InsuranceCompanyofInterestTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("AssistedBUId", userPersonalDetailModel.AssistedBUId, DbType.String, ParameterDirection.Input);
        parameters.Add("CreatedBy", userPersonalDetailModel.CreatedBy, DbType.String, ParameterDirection.Input);
        parameters.Add("IsAdminUpdating", userPersonalDetailModel.IsAdminUpdating, DbType.Boolean, ParameterDirection.Input);
        var result = await connection.ExecuteAsync("[dbo].[Identity_UpdateUserPersonalDetail]", parameters, commandType: CommandType.StoredProcedure);

        return (result > 0);
    }



    /// <summary>
    /// UpdateUserBankDetail
    /// </summary>
    /// <param name="userBankDetailModel"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<UserBankDetailUpdateResponce> UpdateUserBankDetail(UserBankDetailModel userBankDetailModel, CancellationToken cancellation)
    {
        var responce = new UserBankDetailUpdateResponce();
        using var connection = _context.CreateConnection();
        int res = await InsertBankResponse(userBankDetailModel, string.Empty, cancellation);
        if (res == 1)
        {
            var parameters = new DynamicParameters();
            parameters.Add("UserId", userBankDetailModel.UserId, DbType.String, ParameterDirection.Input);
            parameters.Add("BankId", userBankDetailModel.BankId, DbType.String, ParameterDirection.Input);
            parameters.Add("IFSC", userBankDetailModel.IFSC, DbType.String, ParameterDirection.Input);
            parameters.Add("AccountHolderName", userBankDetailModel.AccountHolderName, DbType.String, ParameterDirection.Input);
            parameters.Add("AccountNumber", userBankDetailModel.AccountNumber, DbType.String, ParameterDirection.Input);
            parameters.Add("IsDraft", userBankDetailModel.IsDraft, DbType.Boolean, ParameterDirection.Input);
            parameters.Add("IsAdminUpdating", userBankDetailModel.IsAdminUpdating, DbType.Boolean, ParameterDirection.Input);
            var result = await connection.ExecuteAsync("[dbo].[Identity_UpdateUserBankDetail]", parameters, commandType: CommandType.StoredProcedure);
            if (result > 0)
            {
                responce.Status = true;
                responce.Message = userBankDetailModel.IsDraft ? "Bank details saved as Draft" : "Bank details saved";
            }
            else
            {
                responce.Status = false;
                responce.Message = "We can't validate your bank details for now, Please try again later";
            }
        }
        else
        {
            string messageForBankFailure = "";
            if (res == 0)
            {
                messageForBankFailure = "We can't validate your bank details for now, Please try again later";
            }
            else if (res == 2)
            {
                messageForBankFailure = "Bank account details not valid";
            }
            else if (res == 3)
            {
                messageForBankFailure = "Name did not match with Account Details";
            }
            else
            {
                messageForBankFailure = "We can't validate your bank details for now, Please try again later";
            }
            responce.Status = false;
            responce.Message = messageForBankFailure;
        }
        return responce;
    }

    public async Task<bool> UpdateUserAddressDetail(UserAddressDetailModel userAddressDetailModel, CancellationToken cancellation)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", userAddressDetailModel.UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("AddressLine1", userAddressDetailModel.AddressLine1, DbType.String, ParameterDirection.Input);
        parameters.Add("AddressLine2", userAddressDetailModel.AddressLine2, DbType.String, ParameterDirection.Input);
        parameters.Add("Pincode", userAddressDetailModel.Pincode, DbType.String, ParameterDirection.Input);
        parameters.Add("CityId", userAddressDetailModel.CityId, DbType.String, ParameterDirection.Input);
        parameters.Add("StateId", userAddressDetailModel.StateId, DbType.String, ParameterDirection.Input);
        var result = await connection.ExecuteAsync("[dbo].[Identity_UpdateUserAddressDetail]", parameters, commandType: CommandType.StoredProcedure);

        return (result > 0);

    }

    // 0 for sygny API failure case
    // 1 for success responce
    // 2 for Invalid bank account details
    // 3 for name mismatch
    private async Task<int> InsertBankResponse(UserBankDetailModel userBankDetailModel, string mobileNumber, CancellationToken cancellationToken)
    {
        if (userBankDetailModel.IsDraft)
        {
            return userBankDetailModel.IsDraft ? 1 : 0;
        }
        BankVerificationResponse response = await _signzyService.GetBankVerification(mobileNumber, userBankDetailModel.AccountNumber, userBankDetailModel.AccountHolderName, userBankDetailModel.IFSC, cancellationToken);
        if (response == null)
        {
            return 0;
        }
        var nameMatchScorePercentage = _config.GetSection("NameMatchScore").GetSection("NameMatchScorePercentage").Value;
        if (response.result.reason.Equals("Invalid Bank Account Details"))
            return 2;
        if (!string.IsNullOrEmpty(nameMatchScorePercentage) && response.result.nameMatchScore < decimal.Parse(nameMatchScorePercentage))
            return 3;

        if (response != null)
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("Task", response.task, DbType.String, ParameterDirection.Input);
            parameters.Add("Id", response.id, DbType.String, ParameterDirection.Input);
            parameters.Add("PatronId", response.patronId, DbType.String, ParameterDirection.Input);
            parameters.Add("Active", response.result.active, DbType.String, ParameterDirection.Input);
            parameters.Add("Reason", response.result.reason, DbType.String, ParameterDirection.Input);
            parameters.Add("NameMatch", response.result.nameMatch, DbType.String, ParameterDirection.Input);
            parameters.Add("MobileMatch", response.result.mobileMatch, DbType.String, ParameterDirection.Input);
            parameters.Add("SignzyReferenceId", response.result.signzyReferenceId, DbType.String, ParameterDirection.Input);
            parameters.Add("NameMatchScore ", response.result.nameMatch, DbType.String, ParameterDirection.Input);
            parameters.Add("Nature", response.result.auditTrail.nature, DbType.String, ParameterDirection.Input);
            parameters.Add("Value", response.result.auditTrail.value, DbType.String, ParameterDirection.Input);
            parameters.Add("Timestamp", response.result.auditTrail.timestamp, DbType.String, ParameterDirection.Input);
            parameters.Add("Response", response.result.bankTransfer.response, DbType.String, ParameterDirection.Input);
            parameters.Add("BankRRN", response.result.bankTransfer.bankRRN, DbType.String, ParameterDirection.Input);
            parameters.Add("BeneName", response.result.bankTransfer.beneName, DbType.String, ParameterDirection.Input);
            parameters.Add("BeneMMID", response.result.bankTransfer.beneMMID, DbType.String, ParameterDirection.Input);
            parameters.Add("BeneMobile", response.result.bankTransfer.beneMobile, DbType.String, ParameterDirection.Input);
            parameters.Add("BeneIFSC", response.result.bankTransfer.beneIFSC, DbType.String, ParameterDirection.Input);

            var result = await connection.ExecuteAsync("[dbo].[Identity_InsertBankVerificationDetails]", parameters, commandType: CommandType.StoredProcedure);
            //return (result > 0);
            //if (response.result.reason.Equals("success"))
            //{
            return 1;
            //}
        }
        return 0;
    }

    /// <summary>
    /// InsertUserInquiryDetail
    /// </summary>
    /// <param name="userCreationModel"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> InsertUserInquiryDetail(UserInquiryDetailModel userInquiryDetail)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserName", userInquiryDetail.UserName, DbType.String, ParameterDirection.Input);
        parameters.Add("PhoneNumber", userInquiryDetail.PhoneNumber, DbType.String, ParameterDirection.Input);
        parameters.Add("InquiryDescription", userInquiryDetail.InquiryDescription, DbType.String, ParameterDirection.Input);

        var result = await connection.ExecuteAsync("[dbo].[Identity_InsertUserInquiryDetail]", parameters, commandType: CommandType.StoredProcedure);

        return (result > 0);
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
        var mobileParams = new DynamicParameters();
        mobileParams.Add("PANNumber", panNumber, DbType.String, ParameterDirection.Input);
        var mobileExistResult = await connection.QueryAsync<PanDetailsModel>("[dbo].[Identity_CheckPANExists]", mobileParams, commandType: CommandType.StoredProcedure);
        var parameters = new DynamicParameters();
        if (mobileExistResult.Any())
        {
            parameters.Add("PanNumber", panNumber, DbType.String, ParameterDirection.Input);
            parameters.Add("Name", mobileExistResult.FirstOrDefault().Name, DbType.String, ParameterDirection.Input);
            parameters.Add("FatherName", mobileExistResult.FirstOrDefault().FatherName, DbType.String, ParameterDirection.Input);
            parameters.Add("DOB", mobileExistResult.FirstOrDefault().DOB, DbType.String, ParameterDirection.Input);
            parameters.Add("InstanceId", "", DbType.String, ParameterDirection.Input);
            parameters.Add("InstanceCallbackUrl", "", DbType.String, ParameterDirection.Input);
            parameters.Add("UserId", userId, DbType.String, ParameterDirection.Input);
            var result = await connection.QueryAsync<PanDetailsModel>("[dbo].[Identity_InsertPanDetails]", parameters,
             commandType: CommandType.StoredProcedure);
            return mobileExistResult.FirstOrDefault();
        } // Code End For Static PAN Number

        var vehicleDetails = await CheckPanDetailsFromDB(userId, panNumber, cancellationToken);

        if (vehicleDetails == null)
        {
            //Code To Check PAN Number is Exists or Not in TestPAN table


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
                var result = await connection.QueryAsync<PanDetailsModel>("[dbo].[Identity_InsertPanDetails]", parameters,
                 commandType: CommandType.StoredProcedure);
                return result.FirstOrDefault();
            }
            else
            {
                vehicleDetails = new PanDetailsModel() { InvalidPanMsg = "The PAN you have entered is not valid", IsUserExists = true };
            }

        }
        else
        {
            vehicleDetails = new PanDetailsModel() { InvalidPanMsg = "The PAN you have entered is already exist", IsUserExists = true };
        }
        return vehicleDetails;
    }

    /// <summary>
    /// VerifyOTP
    /// </summary>
    /// <param name="mobileNo"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<bool> VerifyOTP(string otp, string userId)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("OTPNumber", otp, DbType.String, ParameterDirection.Input);
        parameters.Add("UserId", userId, DbType.String, ParameterDirection.Input);
        parameters.Add("Condition", "VERIFYOTP", DbType.String, ParameterDirection.Input);

        var result = await connection.QueryAsync<bool>("[dbo].[Identity_InsertUserOTPDetail] ", parameters, commandType: CommandType.StoredProcedure);
        if (result.Any())
        {
            return result.FirstOrDefault();
        }
        return default;
    }

    /// <summary>
    /// VerifyEmail
    /// </summary>
    /// <param name="guId"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<bool> VerifyEmail(string guId, string userId)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("GuId", guId, DbType.String, ParameterDirection.Input);
        parameters.Add("UserId", userId, DbType.String, ParameterDirection.Input);
        parameters.Add("Condition", "VERIFYEMAIL", DbType.String, ParameterDirection.Input);

        var result = await connection.QueryAsync<bool>("[dbo].[Identity_InsertUserEmailVerificationLinkDetail]", parameters, commandType: CommandType.StoredProcedure);

        return result.FirstOrDefault();
    }

    /// <summary>
    /// Check PAN details from DB
    /// </summary>
    /// <param name="panNumber"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>

    public async Task<IEnumerable<UserDocumentTypeModel>> GetUserDocumentType(string UserId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@UserId", UserId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<UserDocumentTypeModel>("[dbo].[Identity_GetDocumentType]", parameters, commandType: CommandType.StoredProcedure);

        return result;
    }

    public async Task<bool> UserDocumentUpload(UserDocumentDetailModel userdocumentdetailmodel)
    {
        Stream stream = new MemoryStream(userdocumentdetailmodel.ImageStream);
        var id = await _mongodbService.MongoUpload(userdocumentdetailmodel.DocumentFileName, stream, userdocumentdetailmodel.ImageStream);
        //var image = await _mongodbService.MongoDownload(id);
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", userdocumentdetailmodel.UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("DocumentTypeId", userdocumentdetailmodel.DocumentTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("DocumentFileName", userdocumentdetailmodel.DocumentFileName, DbType.String, ParameterDirection.Input);
        parameters.Add("FileSize", userdocumentdetailmodel.FileSize, DbType.String, ParameterDirection.Input);
        parameters.Add("DocumentId", id, DbType.String, ParameterDirection.Input);
        parameters.Add("IsAdminUpdating", userdocumentdetailmodel.IsAdminUpdating, DbType.Boolean, ParameterDirection.Input);
        var result = await connection.ExecuteAsync("[dbo].[Identity_InsertUserDocumentDetail]", parameters, commandType: CommandType.StoredProcedure);
        return (result > 0);
    }

    /// <summary>
    /// Get GetPOSPUserMaster List
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns> 
    public async Task<POSPUserMasterModel> GetPOSPUserMaster(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();


        var result = await connection.QueryMultipleAsync("[dbo].[Identity_GetPOSPUserMaster]",

           commandType: CommandType.StoredProcedure);

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
        var result = await connection.QueryAsync<PanDetailsModel>("[dbo].[Identity_InsertPanDetails]", parameters,
                     commandType: CommandType.StoredProcedure);
        var panDetails = result.FirstOrDefault();

        return panDetails;
    }

    private async Task SendSMS(string mobileNo, string userId)
    {
        using var connection = _context.CreateConnection();
        //Code To Check Mobile Number is Exists or Not in TestMobile table
        /* var mobileParams = new DynamicParameters();
         mobileParams.Add("MobileNo", mobileNo, DbType.String, ParameterDirection.Input);
         var mobileExistResult = await connection.QueryAsync<TestMobilePAN>("[dbo].[Identity_CheckMobileExists]", mobileParams, commandType: CommandType.StoredProcedure).ConfigureAwait(false);*/
        var parameters = new DynamicParameters();
        string defaultOtp = _config.GetSection("OTPConfig").GetSection("IsDefaultOtp").Value;
        if (defaultOtp.ToLower() == "true")
        {
            string otpId = Guid.NewGuid().ToString();
            parameters.Add("MobileNo", mobileNo, DbType.String, ParameterDirection.Input);
            parameters.Add("OTPId", otpId, DbType.String, ParameterDirection.Input);
            parameters.Add("OTPNumber", "1001", DbType.Int64, ParameterDirection.Input);
        }
        /*else if (mobileExistResult.FirstOrDefault().IsExists)
        {
            string otpId = Guid.NewGuid().ToString().ToUpper();
            parameters.Add("MobileNo", mobileNo, DbType.String, ParameterDirection.Input);
            parameters.Add("OTPId", otpId, DbType.String, ParameterDirection.Input);
            parameters.Add("OTPNumber", "1001", DbType.Int64, ParameterDirection.Input);
            parameters.Add("UserId", userId, DbType.String, ParameterDirection.Input);
        }*/
        else
        {
            var sMSResponse = await _sMSService.SendSMS(mobileNo, CancellationToken.None).ConfigureAwait(false);

            if (sMSResponse != null)
            {
                parameters.Add("MobileNo", mobileNo, DbType.String, ParameterDirection.Input);
                parameters.Add("OTPId", sMSResponse.response.id, DbType.String, ParameterDirection.Input);
                parameters.Add("OTPNumber", sMSResponse.response.OTP, DbType.Int64, ParameterDirection.Input);
                parameters.Add("UserId", userId, DbType.String, ParameterDirection.Input);
            }
        }
        var result = await connection.ExecuteAsync("[dbo].[Identity_InsertUserOTPDetail]", parameters, commandType: CommandType.StoredProcedure);
    }

    private async Task SendEmail(string emailId, string userId, string env)
    {
        string guId = await _emailService.SendVerificationEmail(emailId, userId, CancellationToken.None).ConfigureAwait(false);

        if (!string.IsNullOrEmpty(guId))
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("EmailId", emailId, DbType.String, ParameterDirection.Input);
            parameters.Add("GuId", guId, DbType.String, ParameterDirection.Input);
            parameters.Add("UserId", userId, DbType.String, ParameterDirection.Input);

            var result = await connection.ExecuteAsync("[dbo].[Identity_InsertUserEmailVerificationLinkDetail]", parameters, commandType: CommandType.StoredProcedure);
        }
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
        var result = await connection.QueryAsync<UserDetailModel>("[dbo].[Identity_GetUserDetail]", parameters,
                     commandType: CommandType.StoredProcedure);

        return result;
    }
    //Benefits Task
    public async Task<IEnumerable<BenefitDetailModel>> GetBenefitsDetail(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        var result = await connection.QueryAsync<BenefitDetailModel>("[dbo].[Identity_GetBenefitsDetail]", parameters,
                     commandType: CommandType.StoredProcedure);

        return result;
    }
    public async Task<bool> InsertBenefitsDetailCreationDetail(BenefitDetailModel benefitsDetailModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("BenefitsTitle", benefitsDetailModel.BenefitsTitle, DbType.String, ParameterDirection.Input);
        parameters.Add("BenefitsDescription", benefitsDetailModel.BenefitsDescription, DbType.String, ParameterDirection.Input);
        parameters.Add("IsActive", true, DbType.Boolean, ParameterDirection.Input);

        var result = await connection.ExecuteAsync("[dbo].[Identity_InsertBenefitsDetail]", parameters, commandType: CommandType.StoredProcedure);

        return (result > 0);
    }
    public async Task<bool> UpdateBenefitsDetail(BenefitDetailModel benefitsDetailModel, CancellationToken cancellationToken)
    {

        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("Id", benefitsDetailModel.Id, DbType.String, ParameterDirection.Input);
            parameters.Add("BenefitsTitle", benefitsDetailModel.BenefitsTitle, DbType.String, ParameterDirection.Input);
            parameters.Add("BenefitsDescription", benefitsDetailModel.BenefitsDescription, DbType.String, ParameterDirection.Input);
            var result = await connection.ExecuteAsync("[dbo].[Identity_UpdateBenefitsDetail]", parameters, commandType: CommandType.StoredProcedure);
            return (result > 0);

        }
    }
    public async Task<bool> DeleteBenefitsDetail(string Id, CancellationToken cancellationToken)
    {

        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("Id", Id, DbType.String, ParameterDirection.Input);

            var result = await connection.ExecuteAsync("[dbo].[Identity_DeleteBenefitsDetail]", parameters, commandType: CommandType.StoredProcedure);

            return (result > 0);

        }
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

        var result = await connection.ExecuteAsync("[dbo].[Identity_UpdateUserEmailIdDetail]", parameters,
            commandType: CommandType.StoredProcedure);

        await SendEmail(userModel.EmailId, userModel.UserId, userModel.Environment).ConfigureAwait(false);

        return result > 0;
    }

    // API Login 
    public async Task<AdminUserValidationModel> ValidateAdminLogin(AdminUserModel loginModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("EmailId", loginModel.EmailId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<AdminUserValidationModel>("[dbo].[Identity_ValidateAdminLogin]", parameters, commandType: CommandType.StoredProcedure);
        if (result is not null && result.Any())
        {
            var userDetails = result.FirstOrDefault();
            if (userDetails.IsUserExists)
            {
                string temp = Hash256.Hash256Password(userDetails.Password);
                if (Hash256.ValidateSH256Password(loginModel.PassWord, userDetails.Password))
                {
                    userDetails.AuthToken = CreateJWTToken(userDetails.UserId, userDetails.RoleName);
                    userDetails.Message = "Login Successfull";
                    return userDetails;
                }
            }
        }
        return default;
    }

    public async Task<UpdateUserPasswordFromUserLinkResponceModel> UpdateUserPasswordFromUserLink(UpdateUserPasswordFromUserLinkCommand updateUserPasswordModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", updateUserPasswordModel.UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("Guid", updateUserPasswordModel.Guid, DbType.String, ParameterDirection.Input);

        var result = await connection.QueryAsync<ValidateUserPasswordFromUserLinkResponceModel>("[dbo].[Identity_ValidateUserPasswordFromUserLink]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        if (result is not null && result.Any())
        {
            var data = result.FirstOrDefault();
            if (data is not null && data.IsValid)
            {
                var resetParams = new DynamicParameters();
                resetParams.Add("UserId", updateUserPasswordModel.UserId, DbType.String, ParameterDirection.Input);
                resetParams.Add("Password", Hash256.Hash256Password(updateUserPasswordModel.ConfirmPassWord), DbType.String, ParameterDirection.Input);
                await connection.QueryAsync<ResetPasswordAdminResponseModel>("[dbo].[Identity_ResetPasswordAdmin]", resetParams, commandType: CommandType.StoredProcedure);
                return new UpdateUserPasswordFromUserLinkResponceModel()
                {
                    IsValid = data.IsValid,
                    Message = "Passsword reset successfull",
                    IsPasswordUpdated = true
                };
            }
            else
            {
                return new UpdateUserPasswordFromUserLinkResponceModel()
                {
                    IsValid = data.IsValid,
                    Message = data.Message,
                    IsPasswordUpdated = false
                };
            }
        }

        return default;

    }

    public async Task<AdminUpdateUserResponseModel> UpdateAdminUserPasswordSelf(AdminUpdateUserModel updateLoginModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", updateLoginModel.UserId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<GetUserPasswordFromUserId>("[dbo].[Identity_GetAdminPasswordByUserId]", parameters, commandType: CommandType.StoredProcedure);

        if (result.Any())
        {
            var userOldPasswords = result.FirstOrDefault();
            if (userOldPasswords.UserId != null && userOldPasswords.Password != null && Hash256.ValidateSH256Password(updateLoginModel.OldPassWord, userOldPasswords.Password))
            {
                var updatePasswordParameters = new DynamicParameters();
                updatePasswordParameters.Add("UserId", updateLoginModel.UserId, DbType.String, ParameterDirection.Input);
                updatePasswordParameters.Add("NewPassWord", Hash256.Hash256Password(updateLoginModel.NewPassWord), DbType.String, ParameterDirection.Input);
                var updatePasswordRes = await connection.QueryAsync<AdminUpdateUserResponseModel>("[dbo].[Identity_UpdateAdminUserPassword]", updatePasswordParameters, commandType: CommandType.StoredProcedure);
                if (updatePasswordRes.Any())
                {
                    return updatePasswordRes.FirstOrDefault();
                }
            }
        }
        var defResponce = new AdminUpdateUserResponseModel()
        {
            IsPasswordUpdated = false,
            UserId = null,
            Message = "Failed to change password"
        };
        return defResponce;

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
        var result = await connection.QueryAsync<UserBreadcrumStatusDetailModel>("[dbo].[Identity_GetUserBreadcrumStatusDetail]", parameters,
                     commandType: CommandType.StoredProcedure);

        return result;
    }

    public async Task<IEnumerable<RoleTypeResponseModel>> GetRoleTypeDetails(CancellationToken cancellationToken)
    {

        using var connection = _context.CreateConnection();

        var result = await connection.QueryAsync<RoleTypeResponseModel>("[dbo].[Identity_GetRoleTypeDetails]", commandType: CommandType.StoredProcedure);

        return result;

    }
    public async Task<IEnumerable<ModuleResponseModel>> GetModuleDetails(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();

        var result = await connection.QueryAsync<ModuleResponseModel>("[dbo].[Identity_GetModuleDetails]", commandType: CommandType.StoredProcedure);

        return result;
    }
    public async Task<VerifyEmailResponseModel> VerfyIdentityAdminEmail(string UserId, string EmailId, CancellationToken cancellationToken)
    {

        using var connection = _context.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("UserId", UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("EmailId", EmailId, DbType.String, ParameterDirection.Input);

        var result = await connection.QueryAsync<VerifyEmailResponseModel>("[dbo].[Identity_VerfyUserEmail]", parameters, commandType: CommandType.StoredProcedure);

        if (result.FirstOrDefault().IsValidEmail.Equals(true))
        {
            Guid objguid = Guid.NewGuid();

            using var connection1 = _context.CreateConnection();
            var parameters1 = new DynamicParameters();
            parameters1.Add("EmailId", EmailId, DbType.String, ParameterDirection.Input);
            parameters1.Add("GuId", objguid.ToString(), DbType.String, ParameterDirection.Input);
            parameters1.Add("UserId", UserId, DbType.String, ParameterDirection.Input);

            var resultInsert = await connection.QueryAsync<VerifyEmailResponseModel>("[dbo].[Identity_AdminInsertUserEmailVerificationLinkDetail]", parameters1, commandType: CommandType.StoredProcedure);

        }
        return default;
    }

    public async Task<bool> RoleModulePermissionMapping(RoleModulePermissionModel objPermissionModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        int Result = 0;
        List<RoleModulePermissionCommandInsertModel> lstRoleModulePermissionCommandInsert = new List<RoleModulePermissionCommandInsertModel>(); ;
        var parameters = new DynamicParameters();
        try
        {
            parameters.Add("RoleTypeID", objPermissionModel.RoleTypeID, DbType.Int64, ParameterDirection.Input);
            parameters.Add("RoleTitleName", objPermissionModel.RoleTitleName, DbType.String, ParameterDirection.Input);
            parameters.Add("BUID", objPermissionModel.BUID, DbType.Int64, ParameterDirection.Input);
            parameters.Add("RoleLevelID", objPermissionModel.BUID, DbType.Int64, ParameterDirection.Input);
            parameters.Add("CreatedBy", objPermissionModel.CreatedBy, DbType.String, ParameterDirection.Input);
            var resultIdentityRoleId = await connection.QueryAsync<UserRoleIdentity>("[dbo].[Identity_InsertUserRole]", parameters, commandType: CommandType.StoredProcedure);
            if (resultIdentityRoleId.Any())
            {
                {
                    var IdentityRoleId = resultIdentityRoleId.FirstOrDefault().IdentityRoleId;
                    lstRoleModulePermissionCommandInsert = objPermissionModel.RoleModulePermissionCommandInsert.ToList();
                    foreach (RoleModulePermissionCommandInsertModel rolepermission in lstRoleModulePermissionCommandInsert)
                    {
                        var parametersMapping = new DynamicParameters();
                        parametersMapping.Add("ModuleID", rolepermission.@ModuleID, DbType.Int64, ParameterDirection.Input);
                        //parametersMapping.Add("RoletypeID", rolepermission.RoletypeID, DbType.Int64, ParameterDirection.Input);
                        parametersMapping.Add("IdentityRoleId", IdentityRoleId, DbType.Int64, ParameterDirection.Input);
                        parametersMapping.Add("AddPermission", rolepermission.AddPermission, DbType.Boolean, ParameterDirection.Input);
                        parametersMapping.Add("EditPermission", rolepermission.EditPermission, DbType.Boolean, ParameterDirection.Input);
                        parametersMapping.Add("ViewPermission", rolepermission.ViewPermission, DbType.Boolean, ParameterDirection.Input);
                        parametersMapping.Add("DeletePermission", rolepermission.DeletePermission, DbType.Boolean, ParameterDirection.Input);
                        parametersMapping.Add("DownloadPermission", rolepermission.DownloadPermission, DbType.Boolean, ParameterDirection.Input);
                        parametersMapping.Add("CreatedBy", rolepermission.CreatedBy, DbType.String, ParameterDirection.Input);
                        Result = await connection.ExecuteAsync("[dbo].[Identity_InsertRoleModulePermissionMapping]", parametersMapping, commandType: CommandType.StoredProcedure);
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
        parameters.Add("RoleID", objPermissionModel.RoleID, DbType.String, ParameterDirection.Input);
        parameters.Add("ModuleID", objPermissionModel.ModuleID, DbType.Int64, ParameterDirection.Input);
        parameters.Add("Roletypeid", objPermissionModel.RoletypeID, DbType.Int64, ParameterDirection.Input);
        parameters.Add("AddPermission", objPermissionModel.AddPermission, DbType.Boolean, ParameterDirection.Input);
        parameters.Add("EditPermission", objPermissionModel.EditPermission, DbType.Boolean, ParameterDirection.Input);
        parameters.Add("ViewPermission", objPermissionModel.ViewPermission, DbType.Boolean, ParameterDirection.Input);
        parameters.Add("DeletePermission", objPermissionModel.DeletePermission, DbType.Boolean, ParameterDirection.Input);
        parameters.Add("DownloadPermission", objPermissionModel.DownloadPermission, DbType.Boolean, ParameterDirection.Input);
        parameters.Add("UpdatedBy", objPermissionModel.UpdatedBy, DbType.String, ParameterDirection.Input);
        parameters.Add("UpdatedOn", System.DateTime.Now, DbType.DateTime, ParameterDirection.Input);
        parameters.Add("isActive", objPermissionModel.isActive, DbType.Boolean, ParameterDirection.Input);

        var result = await connection.ExecuteAsync("[dbo].[Identity_UpdateRoleModulePermissionMapping]", parameters, commandType: CommandType.StoredProcedure);

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
        var result = await connection.QueryAsync<RoleSearchResponseModel>("[dbo].[Identity_GetRoleModulePermission]", parameters,
                    commandType: CommandType.StoredProcedure);
        return result;
    }
    public async Task<IEnumerable<RoleSearchResponseAllModel>> GetPermissionMappingAll(CancellationToken cancellationToken)
    {

        using var connection = _context.CreateConnection();

        var result = await connection.QueryAsync<RoleSearchResponseAllModel>("[dbo].[Identity_GetRoleModulePermissionALL]",
                  commandType: CommandType.StoredProcedure);
        return result;
    }


    public async Task<IEnumerable<RoleBULevelResponseModel>> GetRoleBULevelDetails(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();

        var result = await connection.QueryAsync<RoleBULevelResponseModel>("[dbo].[Identity_GetIBULevelDetails]", commandType: CommandType.StoredProcedure);

        return result;
    }

    /// <summary>
    /// GetPOSPConfigurationDetail
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns> 
    public async Task<IEnumerable<POSPConfigurationDetailModel>> GetPOSPConfigurationDetail(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var result = await connection.QueryAsync<POSPConfigurationDetailModel>("[dbo].[Identity_GetPOSPConfigurationDetail]",
                     commandType: CommandType.StoredProcedure);

        return result;
    }

    public async Task<IEnumerable<UserDocumentDetailModel>> GetUserDocumentDetail(string UserId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", UserId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<UserDocumentDetailModel>("[dbo].[Identity_GetUserDocumentDetail]", parameters,
                     commandType: CommandType.StoredProcedure);
        foreach (var item in result)
        {
            //item.Image64 = await _mongodbService.MongoDownload("63b670462b67c64123803a07");
            item.Image64 = await _mongodbService.MongoDownload(item.DocumentId);
        }
        return result;
    }
    /// <summary>
    /// UserProfilePictureUpload
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns> 

    public async Task<bool> UserProfilePictureUpload(UserProfilePictureModel userProfilePictureModel)
    {
        Stream stream = new MemoryStream(userProfilePictureModel.ImageStream);
        var id = await _mongodbService.MongoUpload(userProfilePictureModel.ProfilePictureFileName, stream, userProfilePictureModel.ImageStream);
        //var image = await _mongodbService.MongoDownload(id);
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", userProfilePictureModel.UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("ProfilePictureID", userProfilePictureModel.ProfilePictureID, DbType.String, ParameterDirection.Input);
        parameters.Add("ProfilePictureFileName", userProfilePictureModel.ProfilePictureFileName, DbType.String, ParameterDirection.Input);
        parameters.Add("ProfilePictureStoragePath", userProfilePictureModel.ProfilePictureStoragePath, DbType.String, ParameterDirection.Input);
        parameters.Add("DocumentId", id, DbType.String, ParameterDirection.Input);
        var result = await connection.ExecuteAsync("[dbo].[Identity_InsertUploadProfilePicture]", parameters,
                        commandType: CommandType.StoredProcedure);
        return (result > 0);
    }

    /// <summary>
    /// GetUserProfilePictureDetail
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns> 
    public async Task<IEnumerable<UserProfilePictureModel>> GetUserProfilePictureDetail(string UserId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", UserId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<UserProfilePictureModel>("[dbo].[Identity_GetUserProfilePictureDetail]", parameters,
                     commandType: CommandType.StoredProcedure);
        foreach (var item in result)
        {
            //item.Image64 = await _mongodbService.MongoDownload("63b670462b67c64123803a07");
            item.Image64 = await _mongodbService.MongoDownload(item.DocumentId);
        }
        return result;
    }

    /// <summary>
    /// Get GetUserProfileDetail List
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns> 
    public async Task<UserProfileDetailModel> GetUserProfileDetail(string UserId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", UserId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryMultipleAsync("[dbo].[Identity_GetUserDetail]", parameters,
           commandType: CommandType.StoredProcedure);

        UserProfileDetailModel response = new()
        {
            UserList = result.Read<UserModel>(),
            UserDetailList = result.Read<UserDetailModel>(),
            InsuranceProductModel = result.Read<InsuranceProductModel>(),
            InsuranceCompanyModel = result.Read<InsuranceCompanyModel>(),
            UserBankDetailList = result.Read<UserBankDetailModel>(),
            UserAddressDetailList = result.Read<UserAddressDetailModel>(),
            DocumentTypeList = result.Read<DocumentTypeModel>(),
            POSPAccountDetailModel = result.Read<POSPAccountDetailModel>()
        };

        return response;
    }

    public async Task<bool> ReUploadDocument(UserDocumentDetailModel userdocumentdetailmodel)
    {
        Stream stream = new MemoryStream(userdocumentdetailmodel.ImageStream);
        var id = await _mongodbService.MongoUpload(userdocumentdetailmodel.DocumentFileName, stream, userdocumentdetailmodel.ImageStream);
        //var image = await _mongodbService.MongoDownload(id);
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", userdocumentdetailmodel.UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("DocumentTypeId", userdocumentdetailmodel.DocumentTypeId, DbType.String, ParameterDirection.Input);
        parameters.Add("DocumentFileName", userdocumentdetailmodel.DocumentFileName, DbType.String, ParameterDirection.Input);
        parameters.Add("DocumentId", id, DbType.String, ParameterDirection.Input);
        parameters.Add("IsAdminUpdating", userdocumentdetailmodel.IsAdminUpdating, DbType.Boolean, ParameterDirection.Input);
        var result = await connection.ExecuteAsync("[dbo].[Identity_InsertReUplaodDocument]", parameters, commandType: CommandType.StoredProcedure);
        return (result > 0);
    }
    public async Task<IEnumerable<BUSearchResponseModel>> GetBUDetails(BUSearchInputModel objBUInputModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("BUName", objBUInputModel?.BUName, DbType.String, ParameterDirection.Input);
        parameters.Add("CreatedFrom", objBUInputModel?.CreatedFrom, DbType.String, ParameterDirection.Input);
        parameters.Add("CreatedTo", objBUInputModel?.CreatedTo, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<BUSearchResponseModel>("[dbo].[Identity_GetBUDetails]", parameters,
                    commandType: CommandType.StoredProcedure);
        return result;
    }

    public async Task<bool> BUUpdateDetails(BUUpdateInputModel objBUUpdateModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("BUID", objBUUpdateModel?.BUID, DbType.String, ParameterDirection.Input);
        parameters.Add("IsActive", objBUUpdateModel?.IsActive, DbType.Boolean, ParameterDirection.Input);
        var result = await connection.ExecuteAsync("[dbo].[Identity_UpdateBUDetails]", parameters,
                    commandType: CommandType.StoredProcedure);
        return (result > 0);
    }
    public async Task<bool> BUInsertDetails(BUInsertInputModel objBUInputModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add("Roletypeid", objBUInputModel?.Roletypeid, DbType.Int64, ParameterDirection.Input);
        parameters.Add("BULevelID", objBUInputModel?.BULevelID, DbType.Int64, ParameterDirection.Input);
        parameters.Add("BUName", objBUInputModel?.BUName, DbType.String, ParameterDirection.Input);
        parameters.Add("IsActive", objBUInputModel?.IsActive, DbType.Boolean, ParameterDirection.Input);
        parameters.Add("RoleId", objBUInputModel?.RoleId, DbType.String, ParameterDirection.Input);
        parameters.Add("CreatedBy", objBUInputModel?.CreatedBy, DbType.String, ParameterDirection.Input);


        var result = await connection.ExecuteAsync("[dbo].[Identity_InsertBUDetails]", parameters, commandType: CommandType.StoredProcedure);

        return (result > 0);
    }
    public async Task<bool> CategoryInsert(CategoryInputModel objCategoryModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add("UserCategoryName", objCategoryModel?.UserCategoryName, DbType.String, ParameterDirection.Input);
        parameters.Add("CreatedBy", objCategoryModel?.CreatedBy, DbType.String, ParameterDirection.Input);

        var result = await connection.ExecuteAsync("[dbo].[Identity_InsertUserCategory]", parameters, commandType: CommandType.StoredProcedure);

        return (result > 0);
    }

    public async Task<bool> UserRoleMappingInsert(UserMappingInsertInputModel objMapInputModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add("UserID", objMapInputModel?.UserID, DbType.String, ParameterDirection.Input);
        parameters.Add("RoleID", objMapInputModel?.RoleID, DbType.String, ParameterDirection.Input);
        parameters.Add("ReportingUserID", objMapInputModel?.ReportingUserID, DbType.String, ParameterDirection.Input);
        parameters.Add("CategoryID", objMapInputModel?.CategoryID, DbType.Int64, ParameterDirection.Input);
        parameters.Add("BUID", objMapInputModel?.BUID, DbType.Int64, ParameterDirection.Input);
        parameters.Add("RoleTypeID", objMapInputModel?.RoleTypeID, DbType.Int64, ParameterDirection.Input);
        parameters.Add("IsActive", objMapInputModel?.IsActive, DbType.Boolean, ParameterDirection.Input);

        var result = await connection.ExecuteAsync("[dbo].[Identity_InsertUserRoleMapping]", parameters, commandType: CommandType.StoredProcedure);

        return (result > 0);
    }
    public async Task<IEnumerable<UserRoleSearchResponseModel>> GetUserRoleMapping(UserRoleSearchInputModel objSearchModel, CancellationToken cancellationToken)
    {

        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        //if (!string.IsNullOrEmpty(objPermissionModel.ToString()))
        if ((objSearchModel.EMPID != "") || (objSearchModel.RoleTypeName != "") || ((objSearchModel.Status.ToString() != "") || (objSearchModel.CreatedFrom != "" && objSearchModel.CreatedTo != "")))
        {
            parameters.Add("EMPID", objSearchModel?.EMPID, DbType.String, ParameterDirection.Input);
            parameters.Add("RoleTypeName", objSearchModel?.RoleTypeName, DbType.String, ParameterDirection.Input);
            if (objSearchModel.Status.ToString() != "")
            {
                parameters.Add("isActive", Convert.ToBoolean(objSearchModel?.Status), DbType.Boolean, ParameterDirection.Input);
            }
            parameters.Add("CreatedFrom", objSearchModel?.CreatedFrom, DbType.String, ParameterDirection.Input);
            parameters.Add("CreatedTo", objSearchModel?.CreatedTo, DbType.String, ParameterDirection.Input);
            var result = await connection.QueryAsync<UserRoleSearchResponseModel>("[dbo].[Identity_GetUserRoleMappingSearch]", parameters,
                        commandType: CommandType.StoredProcedure);
            return result;
        }
        else
        {
            var result = await connection.QueryAsync<UserRoleSearchResponseModel>("[dbo].[Identity_GetUserRoleMapping]",
                      commandType: CommandType.StoredProcedure);
            return result;
        }


    }
    public async Task<bool> InsertUserAndRoleMapping(UserRoleModel objUserRoleModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();

        var parameters = new DynamicParameters();
        try
        {
            parameters.Add("UserName", objUserRoleModel?.UserName, DbType.String, ParameterDirection.Input);
            parameters.Add("EmpID", objUserRoleModel?.EmpID, DbType.String, ParameterDirection.Input);
            parameters.Add("MobileNo", objUserRoleModel?.MobileNo, DbType.String, ParameterDirection.Input);
            parameters.Add("EmailId", objUserRoleModel?.@EmailId, DbType.String, ParameterDirection.Input);
            parameters.Add("Gender", objUserRoleModel?.Gender, DbType.String, ParameterDirection.Input);
            parameters.Add("DOB", objUserRoleModel?.DOB, DbType.String, ParameterDirection.Input);
            parameters.Add("RoleId", objUserRoleModel?.RoleId, DbType.String, ParameterDirection.Input);
            parameters.Add("CreatedBy", objUserRoleModel?.CreatedBy, DbType.String, ParameterDirection.Input);
            parameters.Add("StatusUser", objUserRoleModel?.StatusUser, DbType.Boolean, ParameterDirection.Input);


            parameters.Add("RoleTypeID", objUserRoleModel?.RoleTypeID, DbType.Int64, ParameterDirection.Input);
            parameters.Add("IdentityRoleId", objUserRoleModel?.IdentityRoleId, DbType.Int64, ParameterDirection.Input);
            parameters.Add("ReportingIdentityRoleId", objUserRoleModel?.ReportingIdentityRoleId, DbType.Int64, ParameterDirection.Input);
            // parameters.Add("UserID", objUserRoleModel?.UserID, DbType.String, ParameterDirection.Input);
            parameters.Add("ReportingUserID", objUserRoleModel?.ReportingUserID, DbType.String, ParameterDirection.Input);
            parameters.Add("CategoryID", objUserRoleModel?.CategoryID, DbType.Int64, ParameterDirection.Input);
            parameters.Add("StatusRoleUser", objUserRoleModel?.StatusRoleUser, DbType.Boolean, ParameterDirection.Input);

            var result = await connection.ExecuteAsync("[dbo].[Identity_InsertUserandRoleMapping]", parameters, commandType: CommandType.StoredProcedure);
            return (result > 0);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<bool> UpdateUserAndRoleMapping(UserRoleUpdateModels objUserRoleModel, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();

        var parameters = new DynamicParameters();
        try
        {
            parameters.Add("UserID", objUserRoleModel?.UserID, DbType.String, ParameterDirection.Input);
            parameters.Add("UserName", objUserRoleModel?.UserName, DbType.String, ParameterDirection.Input);
            parameters.Add("EmpID", objUserRoleModel?.EmpID, DbType.String, ParameterDirection.Input);
            parameters.Add("MobileNo", objUserRoleModel?.MobileNo, DbType.String, ParameterDirection.Input);
            parameters.Add("EmailId", objUserRoleModel?.@EmailId, DbType.String, ParameterDirection.Input);
            parameters.Add("Gender", objUserRoleModel?.Gender, DbType.String, ParameterDirection.Input);
            parameters.Add("DOB", objUserRoleModel?.DOB, DbType.String, ParameterDirection.Input);
            parameters.Add("RoleId", objUserRoleModel?.RoleId, DbType.String, ParameterDirection.Input);
            parameters.Add("UpdatedBy", objUserRoleModel?.UpdatedBy, DbType.String, ParameterDirection.Input);
            parameters.Add("isActive", objUserRoleModel?.IsActive, DbType.Boolean, ParameterDirection.Input);

            parameters.Add("UserRoleID", objUserRoleModel?.UserRoleID, DbType.Int64, ParameterDirection.Input);
            parameters.Add("RoleTypeID", objUserRoleModel?.RoleTypeID, DbType.Int64, ParameterDirection.Input);
            parameters.Add("IdentityRoleId", objUserRoleModel?.IdentityRoleId, DbType.Int64, ParameterDirection.Input);
            parameters.Add("ReportingIdentityRoleId", objUserRoleModel?.ReportingIdentityRoleId, DbType.Int64, ParameterDirection.Input);
            parameters.Add("ReportingUserID", objUserRoleModel?.ReportingUserID, DbType.String, ParameterDirection.Input);
            parameters.Add("CategoryID", objUserRoleModel?.CategoryID, DbType.Int64, ParameterDirection.Input);
            //   parameters.Add("StatusRoleUser", objUserRoleModel?.StatusRoleUser, DbType.Boolean, ParameterDirection.Input);

            var result = await connection.ExecuteAsync("[dbo].[Identity_UpdateUserandRoleMapping]", parameters, commandType: CommandType.StoredProcedure);
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
        parameters.Add("@EMPID", objMappingInputModel.EMPID, DbType.String, ParameterDirection.Input);
        parameters.Add("@RoleTypeName", objMappingInputModel.RoleTypeName, DbType.String, ParameterDirection.Input);
        if (objMappingInputModel.isActive != "")
        {
            bool isActive = Convert.ToBoolean(objMappingInputModel.isActive);
            parameters.Add("@isActive", isActive, DbType.Boolean, ParameterDirection.Input);
        }

        parameters.Add("@CreatedFrom", objMappingInputModel.CreatedFrom, DbType.String, ParameterDirection.Input);
        parameters.Add("@CreatedTo", objMappingInputModel.CreatedTo, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<RoleMappingResponseModel>("[dbo].[Identity_GetUserAndRoleMappingSearch]", parameters,
                             commandType: CommandType.StoredProcedure);
        return result;
    }
    public async Task<IEnumerable<RoleMappingResponseModel>> GetUserandRoleMappingAll(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var result = await connection.QueryAsync<RoleMappingResponseModel>("[dbo].[Identity_GetUserandRoleMapping]",
                     commandType: CommandType.StoredProcedure);

        return result;
    }

    public async Task<bool> ResetUserAccountDetail(string MobileNo, CancellationToken cancellationToken)
    {
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("MobileNo", MobileNo, DbType.String, ParameterDirection.Input);
            var result = await connection.ExecuteAsync("[dbo].[Identity_ResetDatabaseAccount]", parameters,
                commandType: CommandType.StoredProcedure);

            return (result > 0);

        }
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
        var result = await connection.QueryMultipleAsync("[dbo].[Identity_GetUserPersonalVerificationDetail]", parameters,
           commandType: CommandType.StoredProcedure);

        PanDetailMasterModel response = new()
        {
            UserList = result.Read<UserModel>(),
            UserDetailList = result.Read<UserDetailModel>(),
            InsuranceProductModel = result.Read<InsuranceProductModel>(),
            InsuranceCompanyModel = result.Read<InsuranceCompanyModel>(),
            PanDetailsList = result.Read<PanDetailsModel>(),
            UserAddressDetailList = result.Read<UserAddressDetailModel>()
        };

        return response;
    }

    public async Task<IEnumerable<RoleLevelResponseModel>> GetRoleLevelDetails(CancellationToken cancellationToken)
    {

        using var connection = _context.CreateConnection();

        var result = await connection.QueryAsync<RoleLevelResponseModel>("[dbo].[Identity_GetRoleTypeDetails]", commandType: CommandType.StoredProcedure);

        return result;

    }

    public async Task<IEnumerable<ErrorCodeModel>> GetErrorCode(string ErrorType, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("ErrorType", ErrorType, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<ErrorCodeModel>("[dbo].[Identity_GetErrorCode]", parameters,
                     commandType: CommandType.StoredProcedure);
        return result;
    }

    public async Task<StateCitybyPincodeModel> GetStateCitybyPincode(string Pincode, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("Pincode", Pincode, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryMultipleAsync("[dbo].[Identity_GetStateCitybyPincode]", parameters,
           commandType: CommandType.StoredProcedure);

        StateCitybyPincodeModel response = new()
        {
            StateList = result.Read<StateModel>(),
            CityList = result.Read<CityModel>(),

        };

        return response;
    }

    public async Task<ResetPasswordResponseModel> ResetPassword(string emailId, string env, CancellationToken cancellationToken)
    {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("EmailId", emailId, DbType.String, ParameterDirection.Input);
            var userDetails = await connection.QueryAsync<ResetPasswordResponseModel>("[dbo].[Identity_ResetPassword]", parameters,
                         commandType: CommandType.StoredProcedure);

            var detail = userDetails.FirstOrDefault();
            bool isMailSent = true;
            if (!string.IsNullOrWhiteSpace(detail.UserId) && detail.IsValidate)
            {
                var guId = await _emailService.ResetPasswordEmail(emailId, detail.UserId, CancellationToken.None);

                var paramResetPwd = new DynamicParameters();
                paramResetPwd.Add("GuId", guId, DbType.String, ParameterDirection.Input);
                paramResetPwd.Add("EmailId", emailId, DbType.String, ParameterDirection.Input);
                paramResetPwd.Add("UserId", detail.UserId, DbType.String, ParameterDirection.Input);
                var updateDB = await connection.ExecuteAsync("[dbo].[Identity_InsertResetPasswordVerificationLinkDetail]", paramResetPwd, commandType: CommandType.StoredProcedure);

                detail.Message = "Reset link sent successfully";
            }
            return detail;   
    }

    public async Task<LogoutResponseModel> Logout(string userId, CancellationToken cancellationToken)
    {

        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", userId, DbType.String, ParameterDirection.Input);
        var userDetails = await connection.QueryAsync<LogoutResponseModel>("[dbo].[Identity_Logout]", parameters,
                     commandType: CommandType.StoredProcedure);
        var detail = userDetails.FirstOrDefault();
        return detail;
    }

    /// <summary>
    /// Create JWT Token
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    private string CreateJWTToken(string userId, string roleName)
    {
        DateTime issuedAt = DateTime.UtcNow;
        DateTime expires = DateTime.UtcNow.AddDays(1);

        var tokenHandler = new JwtSecurityTokenHandler();
        ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
        {
                new Claim(ApiClaimTypes.UserId, userId),
                new Claim(ApiClaimTypes.RoleName, roleName)
        });

        var securityKey = new SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(_tokenSettings.SigningKey));

        var signingCredentials = new SigningCredentials(securityKey,
                                                        SecurityAlgorithms.HmacSha256Signature);
        var token =
                tokenHandler.CreateJwtSecurityToken(issuer: _tokenSettings.Issuer,
                                                    audience: _tokenSettings.Audience,
                                                    subject: claimsIdentity,
                                                    notBefore: issuedAt,
                                                    expires: expires,
                                                    signingCredentials: signingCredentials);
        var tokenString = tokenHandler.WriteToken(token);

        return tokenString;
    }

    public async Task<bool> ResetPasswordVerification(string guId, string userId)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("GuId", guId, DbType.String, ParameterDirection.Input);
        parameters.Add("UserId", userId, DbType.String, ParameterDirection.Input);
        parameters.Add("Condition", "VERIFY", DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<bool>("[dbo].[Identity_GetResetPasswordVerification]", parameters, commandType: CommandType.StoredProcedure);
        return result.FirstOrDefault();
    }

    public async Task<IEnumerable<GetUserListForDepartmentTaggingModel>> GetUserListForDepartmentTagging(string TaggingType, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("TaggingType", TaggingType, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<GetUserListForDepartmentTaggingModel>("[dbo].[Identity_GetUserListForDepartmentTagging]", parameters,
                     commandType: CommandType.StoredProcedure);
        return result;
    }

    public async Task<IEnumerable<GetPOSPSourceTypeVm>> GetPOSPSourceType(CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var result = await connection.QueryAsync<GetPOSPSourceTypeVm>("[dbo].[Identity_GetPOSPSourceType]", commandType: CommandType.StoredProcedure);
        return result;
    }

    public async Task<GetSourcedByUserListResponseModel> GetSourcedByUserList(string? BUId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("BUId", BUId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryMultipleAsync("[dbo].[Identity_GetSourcedByUserList]", parameters,
                     commandType: CommandType.StoredProcedure);

        GetSourcedByUserListResponseModel response = new()
        {
            SourcedByUser = result.Read<SourcedByUser>(),
            ServicedByUser = result.Read<ServicedByUser>(),
        };
        return response;
    }


    public async Task<IEnumerable<GetAllRelationshipManagerVM>> GetAllRelationshipManager(string? UserId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", UserId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<GetAllRelationshipManagerVM>("[dbo].[Identity_GetAllRelationshipManager]",
                     parameters, commandType: CommandType.StoredProcedure);

        return result;
    }

    public async Task<bool> SendCompletedRegisterationMail(string UserId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", UserId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<SendCompletedRegisterationMailResponseModel>("[dbo].[Identity_SendEmailForAssistedPOSP]",
                     parameters, commandType: CommandType.StoredProcedure);

        if (result != null)
        {
            await _emailService.SendEmailForCompletedPOSP(result.FirstOrDefault().Username, result.FirstOrDefault().EmailId, result.FirstOrDefault().MobileNo, result.FirstOrDefault().POSPId, cancellationToken).ConfigureAwait(false);
        }

        return default;
    }
}



