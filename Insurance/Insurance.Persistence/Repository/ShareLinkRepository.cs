using Dapper;
using DnsClient.Internal;
using Insurance.Core.Contracts.Common;
using Insurance.Core.Contracts.Persistence;
using Insurance.Core.Features.ShareLink.Command.AuthenticateUser;
using Insurance.Core.Features.ShareLink.Command.SendNotification;
using Insurance.Core.Features.ShareLink.Command.SendOTP;
using Insurance.Core.Features.ShareLink.Command.VerifyOTP;
using Insurance.Domain.Customer;
using Insurance.Domain.InsuranceMaster;
using Insurance.Domain.ShareLink;
using Insurance.Persistence.Configuration;
using Insurance.Persistence.ICIntegration.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver.Linq;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ThirdPartyUtilities.Abstraction;
using ThirdPartyUtilities.Models.JWT;
using ThirdPartyUtilities.Models.SMS;

namespace Insurance.Persistence.Repository;

public class ShareLinkRepository : IShareLinkRepository
{
    private readonly ApplicationDBContext _context;
    private readonly ISmsService _smsService;
    private readonly IEmailService _emailService;
    private readonly IApplicationClaims _applicationClaims;
    private readonly IMongoDBService _mongoDBService;
    private readonly ShareLinkFrontEndURL _shareLinkFrontEndURL;
    private readonly TokenSettings _tokenSettings;
    private readonly IConfiguration _configuration;
    private readonly IdentityApplicationDBContext _identityApplicationDBContext;

    public ShareLinkRepository(ApplicationDBContext context,
        ISmsService smsService,
        IEmailService emailService,
        IApplicationClaims applicationClaims,
        IMongoDBService mongoDBService,
        IOptions<ShareLinkFrontEndURL> options,
        IOptions<TokenSettings> tokenSettings,
        IConfiguration configuration,
        IdentityApplicationDBContext identityApplicationDBContext)
    {
        _context = context;
        _smsService = smsService;
        _emailService = emailService;
        _applicationClaims = applicationClaims;
        _configuration = configuration;
        _mongoDBService = mongoDBService;
        _shareLinkFrontEndURL = options.Value;
        _tokenSettings = tokenSettings.Value;
        _identityApplicationDBContext = identityApplicationDBContext;
    }
    public async Task<string> SendNotification(SendNotificationCommand request, CancellationToken cancellationToken)
    {
        var emailResponse = false;
        var smsResponse = (dynamic)null;
        using var connections = _context.CreateConnection();
        var parameter = new DynamicParameters();
        var url = string.Empty;

        var stage = request.Type.Equals("BreakInSuccess", StringComparison.OrdinalIgnoreCase)
            || request.Type.Equals("BreakInFailure", StringComparison.OrdinalIgnoreCase)
            || request.Type.Equals("BreakInInitiation", StringComparison.OrdinalIgnoreCase) ? "BreakIn" : request.Type;

        url = $"{_shareLinkFrontEndURL.BaseURL}{_applicationClaims.GetUserId()}/";

        parameter.Add("LeadId", request.LeadId, DbType.String, ParameterDirection.Input);
        parameter.Add("UserId", _applicationClaims.GetUserId(), DbType.String, ParameterDirection.Input);
        parameter.Add("EmailId", request.EmailId, DbType.String, ParameterDirection.Input);
        parameter.Add("MobileNumber", request.MobileNumber, DbType.String, ParameterDirection.Input);
        parameter.Add("Link", url, DbType.String, ParameterDirection.Input);
        parameter.Add("Type", stage, DbType.String, ParameterDirection.Input);
        parameter.Add("Insurer", request.InsurerId, DbType.String, ParameterDirection.Input);

        var getLeaddetais = await connections.QueryMultipleAsync("[dbo].[Insurance_GetSharePlanLeadDetails]",
        parameter,
            commandType: CommandType.StoredProcedure);
        var leadDetails = getLeaddetais.Read<CreateLeadModel>().FirstOrDefault();
        var sharePlanDetails = getLeaddetais.Read<SendNotificationRequestModel>().FirstOrDefault();
        if (leadDetails != null && sharePlanDetails != null)
        {
            sharePlanDetails.GrossPremium = leadDetails.GrossPremium;
            sharePlanDetails.BreakinId = leadDetails.BreakinId;
            sharePlanDetails.ManufacturingYear = leadDetails.MakeMonthYear;
            sharePlanDetails.CustomerName = leadDetails.LeadName;
            sharePlanDetails.SMSPhoneNumber = request.MobileNumber;
            sharePlanDetails.EmailId = request.EmailId;
            sharePlanDetails.Type = request.Type;
            sharePlanDetails.POSPName = _applicationClaims?.GetUserName();
            sharePlanDetails.POSPMobile = _applicationClaims?.GetMobileNo();
            sharePlanDetails.PaymentLink = leadDetails.PaymentLink;
            sharePlanDetails.QuoteTransactionId = leadDetails.QuoteTransactionID;
            sharePlanDetails.InsurerId = leadDetails.InsurerId;
            if (request.Type.Equals("Payment", StringComparison.OrdinalIgnoreCase))
            {
                sharePlanDetails.PolicyPDF = Convert.FromBase64String(await _mongoDBService.MongoDownload(sharePlanDetails.DocumentId));
            }

            if (stage.Equals("BreakIn"))
            {
                smsResponse = await _smsService.SendICNotification(sharePlanDetails, cancellationToken);
                if (smsResponse != null && smsResponse.response.status.Equals("success"))
                {
                    return "success";
                }
            }
            else
            {
                smsResponse = await _smsService.SendICNotification(sharePlanDetails, cancellationToken);
                emailResponse = await _emailService.SendBuyJourneyNotification(sharePlanDetails, cancellationToken);
                if (smsResponse != null && smsResponse.response.status.Equals("success") && emailResponse)
                {
                    return "success";
                }
            }
            return default;
        }
        return default;
    }
    public async Task<string> SendSMS(SendOTPCommand request, CancellationToken cancellationToken)
    {
        var LeadDetails = await GetParticularLeadDetailById(request.LeadId, cancellationToken);
        if (LeadDetails != null && !string.IsNullOrEmpty(LeadDetails.PhoneNumber))
        {
            var connection = _context.CreateConnection();
            string defaultOtp = _configuration.GetSection("OTPConfig").GetSection("IsDefaultOtp").Value;
            var parameters = new DynamicParameters();
            SMSResponse sMSResponse = default;
            if (!Convert.ToBoolean(defaultOtp))
            {
                sMSResponse = await _smsService.SendSMS(LeadDetails.PhoneNumber, cancellationToken);
            }
            parameters.Add("MobileNo", LeadDetails.PhoneNumber, DbType.String, ParameterDirection.Input);
            parameters.Add("OTPId", sMSResponse != null ? sMSResponse?.response?.id : Guid.NewGuid().ToString(), DbType.String, ParameterDirection.Input);
            parameters.Add("OTPNumber", sMSResponse != null ? sMSResponse?.response?.OTP : "1001", DbType.Int64, ParameterDirection.Input);
            parameters.Add("LeadID", request.LeadId, DbType.String, ParameterDirection.Input);
            parameters.Add("UserId", _applicationClaims.GetUserId(), DbType.String, ParameterDirection.Input);
            var result = await connection.ExecuteAsync("[dbo].[Insurance_InsertOTPDetail]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);

            return "success";
        }
        return default;
    }
    public async Task<string> VerifyOTP(VerifyOTPCommand request, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("OTPNumber", request.OTP, DbType.String, ParameterDirection.Input);
        parameters.Add("UserId", _applicationClaims.GetUserId(), DbType.String, ParameterDirection.Input);
        parameters.Add("LeadID", request.LeadId, DbType.String, ParameterDirection.Input);
        parameters.Add("Condition", "VERIFYOTP", DbType.String, ParameterDirection.Input);

        var result = await connection.QueryAsync<VerifyOTPResponse>("[dbo].[Insurance_InsertOTPDetail]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        if (result.Any())
        {
            if (result.FirstOrDefault() != null && result.FirstOrDefault().IsValidOTP)
            {
                return "success";
            }
            else
            {
                return "failed";
            }
        }
        return default;
    }
    public async Task<AuthenticateUserVm> AuthenticateUser(AuthenticateUserCommand request, CancellationToken cancellationToken)
    {
        AuthenticateUserVm authenticateUserVm = new AuthenticateUserVm()
        {
            IsLinkExpired = true
        };
        var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("TransactionId", request.TransactionId, DbType.String, ParameterDirection.Input);
        parameters.Add("UserId", request.UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("LeadId", request.LeadId, DbType.String, ParameterDirection.Input);
        parameters.Add("StageId", request.StageId, DbType.String, ParameterDirection.Input);

        var result = await connection.QueryAsync("[dbo].[Insurance_AuthenticateUser]", parameters, commandType: CommandType.StoredProcedure);
        var response = result.FirstOrDefault().Result;
        if (response)
        {
            var token = await GetPOSPDetails(request.UserId);
            authenticateUserVm.Data = token;
            authenticateUserVm.IsLinkExpired = false;
            return authenticateUserVm;
        }
        return authenticateUserVm;
    }
    public async Task<PersonalDetailModel> GetParticularLeadDetailById(string LeadId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("LeadId", LeadId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<PersonalDetailModel>("[dbo].[Insurance_GetParticularLeadDetailById]", parameters,
            commandType: CommandType.StoredProcedure);
        return result.FirstOrDefault();
    }

    private string CreateJWTToken(POSPDetailsModel pOSPDetailsModel)
    {
        DateTime issuedAt = DateTime.UtcNow;
        DateTime expires = DateTime.UtcNow.AddDays(1);

        var tokenHandler = new JwtSecurityTokenHandler();

        ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
               {
            new Claim(ApiClaimTypes.UserId, pOSPDetailsModel.UserId),
            new Claim(ApiClaimTypes.POSPId, pOSPDetailsModel.POSPId),
            new Claim(ApiClaimTypes.UserName, pOSPDetailsModel.UserName),
            new Claim(ApiClaimTypes.DOB,pOSPDetailsModel.DateofBirth),
            new Claim(ApiClaimTypes.EmailId, pOSPDetailsModel.EmailId),
            new Claim(ApiClaimTypes.MobileNo,pOSPDetailsModel.MobileNo),
            new Claim(ApiClaimTypes.AadhaarNumber,pOSPDetailsModel.AadhaarNumber),
            new Claim(ApiClaimTypes.PAN,pOSPDetailsModel.PAN),
            new Claim(ApiClaimTypes.CityName,pOSPDetailsModel.CityName),
            new Claim(ApiClaimTypes.RoleName,pOSPDetailsModel.RoleName)
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

    private async Task<string> GetPOSPDetails(string userId)
    {
        var token = string.Empty;
        using var connection = _identityApplicationDBContext.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("UserIdInput", userId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<POSPDetailsModel>("[dbo].[Identity_GetPOSPDetails]", parameters,
            commandType: CommandType.StoredProcedure).ConfigureAwait(false);

        if (result.Any())
        {
            token = CreateJWTToken(result.FirstOrDefault());
            return token;
        }
        return default;
    }
}

