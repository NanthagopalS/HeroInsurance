using Dapper;
using Identity.Core.Contracts.Persistence;
using Identity.Core.Features.Authenticate.Commands.ResetPasswordAdmin;
using Identity.Domain.Authentication;
using Identity.Domain.UserLogin;
using Identity.Persistence.Configuration;
using Identity.Persistence.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ThirdPartyUtilities.Abstraction;
using ThirdPartyUtilities.Helpers;
using ThirdPartyUtilities.Models.JWT;
using AuthenticationResponse = Identity.Domain.Authentication.AuthenticationResponse;

namespace Identity.Persistence.Repository;
public class AuthenticateRepository : IAuthenticateRepository
{
    private readonly ApplicationDBContext _context;
    private readonly TokenSettings _tokenSettings;
    private readonly ILogger<AuthenticateRepository> _logger;
    private readonly ISmsService _sMSService;
    private readonly IConfiguration _config;
    private readonly IEmailService _emailService;

    public AuthenticateRepository(ApplicationDBContext context,
                                  IOptions<TokenSettings> tokenSettings,
                                  ILogger<AuthenticateRepository> logger, IEmailService emailService,
                                  ISmsService sMSService, IConfiguration config)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _tokenSettings = tokenSettings.Value;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _sMSService = sMSService ?? throw new ArgumentNullException(nameof(sMSService));
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    }

    /// <summary>
    /// Authenticate User
    /// </summary>
    /// <param name="authenticationRequest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<AuthenticationResponse> AuthenticateUser(string userId, string OTP, CancellationToken cancellationToken)
    {
        _logger.LogInformation("AuthenticateUser");


        var verifyOTPResponse = await VerifyOTP(userId, OTP);
        var authenticationResponse = new AuthenticationResponse();
        if (verifyOTPResponse != null && verifyOTPResponse.IsValidOTP)
        {
            POSPDetailsModel pospDetails = await GetPOSPDetails(userId);
            authenticationResponse = new AuthenticationResponse
            {
                Token = CreateJWTTokenForPOSP(pospDetails),
                UserId = userId,
                UserProfileStage = verifyOTPResponse.UserProfileStage,
                WrongOtpCount = int.Parse(verifyOTPResponse.wrongOtpCount),
                IsAccountLock = false
            };

            return authenticationResponse;
        }
        authenticationResponse = new AuthenticationResponse
        {
            Token = null,
            UserId = userId,
            UserProfileStage = verifyOTPResponse.UserProfileStage,
            WrongOtpCount = int.Parse(verifyOTPResponse.wrongOtpCount),
            IsAccountLock = int.Parse(verifyOTPResponse.wrongOtpCount) >= 4
        };

        return authenticationResponse;
    }

    /// <summary>
    /// VerifyOTP
    /// </summary>
    /// <param name="mobileNo"></param>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<VerifyOTPResponse> VerifyOTP(string userId, string otp)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("OTPNumber", otp, DbType.String, ParameterDirection.Input);
        parameters.Add("UserId", userId, DbType.String, ParameterDirection.Input);
        parameters.Add("Condition", "VERIFYOTP", DbType.String, ParameterDirection.Input);

        var result = await connection.QueryAsync<VerifyOTPResponse>("[dbo].[Identity_InsertUserOTPDetail]", parameters, commandType: CommandType.StoredProcedure);

        if (result.Any())
        {
            return result.Single();
        }
        return default;
    }

    public async Task<UserLoginResponseModel> SendSMS(string mobileNo)
    {
        using var connection = _context.CreateConnection();
        var user = await CheckUserByMobileNo(mobileNo);
        if (user.UserId == "0")
        {
            return new UserLoginResponseModel
            {
                UserId = user.UserId,
                IsUserExists = false,
                IsAccountLock = false,
                Message = "User Already exists"
            };
        }
        if (user.UserId != null)
        {
            string defaultOtp = _config.GetSection("OTPConfig").GetSection("IsDefaultOtp").Value;
            var DBOTPCount = await GetdbOtpCount(user.UserId, mobileNo);
            var IsActive = await GetActiveStatus(user.UserId);
            if (int.Parse(DBOTPCount) < 3)
            {
                if (!string.IsNullOrWhiteSpace(user.UserId))
                {
                    var parameters = new DynamicParameters();
                    if (defaultOtp.ToLower() == "true")
                    {
                        string otpId = Guid.NewGuid().ToString();
                        parameters.Add("MobileNo", mobileNo, DbType.String, ParameterDirection.Input);
                        parameters.Add("OTPId", otpId, DbType.String, ParameterDirection.Input);
                        parameters.Add("OTPNumber", "1001", DbType.Int64, ParameterDirection.Input);
                    }
                    else
                    {
                        var sMSResponse = await _sMSService.SendSMS(mobileNo, CancellationToken.None).ConfigureAwait(false);
                        if (sMSResponse != null)
                        {
                            parameters.Add("MobileNo", mobileNo, DbType.String, ParameterDirection.Input);
                            parameters.Add("OTPId", sMSResponse.response.id, DbType.String, ParameterDirection.Input);
                            parameters.Add("OTPNumber", sMSResponse.response.OTP, DbType.Int64, ParameterDirection.Input);
                        }

                    }
                    string OTPMessage = "";
                    if (user.IsRegistrationVerified)
                    {
                        OTPMessage = "Login OTP Sent Successfull";
                    }
                    else
                    {
                        OTPMessage = "Registration OTP Sent Successfully";
                    }
                    var result = await connection.ExecuteAsync("[dbo].[Identity_InsertUserOTPDetail]", parameters, commandType: CommandType.StoredProcedure);
                    // Skip to send OTP on mobile number
                    return new UserLoginResponseModel
                    {
                        UserId = user.UserId,
                        IsUserExists = true,
                        WrongOtpCount = int.Parse(DBOTPCount),
                        IsAccountLock = false,
                        IsActive = IsActive,
                        Message = OTPMessage
                    };
                }
            }
            else
            {
                return new UserLoginResponseModel
                {
                    UserId = user.UserId,
                    IsUserExists = true,
                    WrongOtpCount = int.Parse(DBOTPCount),
                    IsAccountLock = true,
                    IsActive = IsActive,
                    Message = "Maximum limit exceeded, kindly try after (3 minutes)"
                };
            }
        }
        else
        {
            return new UserLoginResponseModel
            {
                UserId = null,
                IsUserExists = false,
                WrongOtpCount = 0,
                IsAccountLock = true,
                Message = "User does not exist"

            };
        }



        return default;
    }





    /// <summary>
    /// GetdbOtpCount
    /// </summary>
    /// <param name="mobileNo"></param>
    /// <returns></returns>
    public async Task<string> GetdbOtpCount(string userId, string mobileNo)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", userId, DbType.String, ParameterDirection.Input);
        parameters.Add("MobileNo", mobileNo, DbType.String, ParameterDirection.Input);

        var result = await connection.QueryAsync<string>("[dbo].[Identity_GetOTPCount]", parameters, commandType: CommandType.StoredProcedure);

        if (result.Any())
        {
            return result.FirstOrDefault();
        }
        return default;
    }

    public async Task<string> GetActiveStatus(string userId)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("UserId", userId, DbType.String, ParameterDirection.Input);

        var result = await connection.QueryAsync<string>("[dbo].[Identity_GetActiveStatus]", parameters, commandType: CommandType.StoredProcedure);

        if (result.Any())
        {
            return result.FirstOrDefault();
        }
        return default;
    }






    /// <summary>
    /// CheckUserByMobileNo
    /// </summary>
    /// <param name="mobileNo"></param>
    /// <returns></returns>
    public async Task<CheckUserByMobileNoResponce> CheckUserByMobileNo(string mobileNo)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("MobileNo", mobileNo, DbType.String, ParameterDirection.Input);

        var result = await connection.QueryAsync<CheckUserByMobileNoResponce>("[dbo].[Identity_CheckUserByMobileNo]", parameters, commandType: CommandType.StoredProcedure);

        if (result.Any())
        {
            return result.FirstOrDefault();
        }
        return default;
    }

    /// <summary>
    /// Authenticate User
    /// </summary>
    /// <param name="authenticationRequest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public async Task<AuthenticationAdminResponse> AuthenticateAdminUser(string emailId, string Password, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        _logger.LogInformation("AuthenticateUser");

        var parameters = new DynamicParameters();
        parameters.Add("EmailId", emailId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<AuthenticationAdminResponse>("[dbo].[Identity_GetAdminUser]", parameters, commandType: CommandType.StoredProcedure);
        if (result is not null && result.Any())
        {
            var authResponse = result.FirstOrDefault();
            if (Hash256.ValidateSH256Password(Password, authResponse.Password))
            {
                authResponse.Token = CreateJWTToken(authResponse.UserId);
                return authResponse;
            }
        }
        return default;
    }

    /// <summary>
    /// VerifyOTP
    /// </summary>
    /// <param name="mobileNo"></param>
    /// <param name="emailId"></param>
    /// <returns></returns>
    private async Task<VerifyAdminUserResponse> GetUserByEmailId(string emailId)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("EmailId", emailId, DbType.String, ParameterDirection.Input);

        var result = await connection.QueryAsync<VerifyAdminUserResponse>("[dbo].[Identity_GetAdminUser]", parameters, commandType: CommandType.StoredProcedure);

        if (result.Any())
        {
            return result.FirstOrDefault();
        }
        return default;
    }

    /// <summary>
    /// Create JWT Token
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    private string CreateJWTToken(string userId)
    {
        DateTime issuedAt = DateTime.UtcNow;
        DateTime expires = DateTime.UtcNow.AddDays(1);

        var tokenHandler = new JwtSecurityTokenHandler();

        ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
        {
                new Claim(ApiClaimTypes.UserId, userId)
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
    public string CreateJWTTokenForPOSP(POSPDetailsModel pOSPDetailsModel)
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
    private async Task<POSPDetailsModel> GetPOSPDetails(string userId)
    {
        using var connection = _context.CreateConnection();
        _logger.LogInformation("AuthenticateUser");

        var parameters = new DynamicParameters();
        parameters.Add("UserIdInput", userId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<POSPDetailsModel>("[dbo].[Identity_GetPOSPDetails]", parameters,
            commandType: CommandType.StoredProcedure);

        if (result.Any())
        {
            return result.FirstOrDefault();
        }
        return default;
    }
    public async Task<ResetPasswordAdminVm> ResetPasswordAdmin(string UserId, CancellationToken cancellationToken)
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        ResetPasswordGenerator resetPasswordGenerator = new ResetPasswordGenerator();
        var Password = resetPasswordGenerator.GenerateRandomStrongPassword(10);
        var newpassword = Hash256.Hash256Password(Password);
        parameters.Add("UserId", UserId, DbType.String, ParameterDirection.Input);
        parameters.Add("Password", newpassword, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<ResetPasswordAdminVm>("[dbo].[Identity_ResetPasswordAdminSendCredentials]", parameters,commandType: CommandType.StoredProcedure);
        if (result.Any())
        {
            string guId = await _emailService.SendPasswordResetEmail(UserId, result.FirstOrDefault().Email, Password, CancellationToken.None).ConfigureAwait(false);
        }
        return result.FirstOrDefault();
    }
}
