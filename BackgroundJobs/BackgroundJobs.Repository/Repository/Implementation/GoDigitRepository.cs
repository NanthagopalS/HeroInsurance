using BackgroundJobs.Configuration;
using BackgroundJobs.Models;
using BackgroundJobs.Repository.Models;
using BackgroundJobs.Repository.Repository.Abstraction;
using Dapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BackgroundJobs.Repository.Repository.Implementation;

public class GoDigitRepository : IGoDigitRepository
{
    private readonly ILogger<GoDigitRepository> _logger;
    private readonly HttpClient _httpClient;
    private readonly HeroConfig _heroConfig;
    private readonly TokenSettings _tokenSettings;
    private readonly ApplicationDBContext _context;

    public GoDigitRepository(ILogger<GoDigitRepository> logger,
                             HttpClient httpClient,
                             IOptions<HeroConfig> options,
                             IOptions<TokenSettings> tokenSetting,
                             ApplicationDBContext applicationDBContext)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClient = httpClient;
        _heroConfig = options.Value;
        _tokenSettings = tokenSetting.Value;
        _context = applicationDBContext;
    }

    public async Task GetPolicyStatus()
    {
        try
        {
            var lstBreakIn = await GetBreakInDataAsync().ConfigureAwait(false);
            if (lstBreakIn != null && lstBreakIn.Any())
            {
                _logger.LogInformation("GetPolicyStatus count {count}", lstBreakIn.Count());
                foreach (var item in lstBreakIn)
                {
                    var token = CreateJWTTokenForPOSP(item.QuoteTransactionId);
                    _httpClient.DefaultRequestHeaders.Clear();
                    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    var breakInURL = $"{_heroConfig.BaseURL}{_heroConfig.GoDigitBreakInURL}{item.QuoteTransactionId}/{item.PolicyNumber}?LeadId={item.LeadId}";
                    var policyStatus = await _httpClient.GetAsync(breakInURL);
                    _logger.LogInformation("PolicyStatus Response {response}", JsonConvert.SerializeObject(policyStatus));
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("GetKYCStatus exception {error}", ex.Message);
        }
    }

    private async Task<IEnumerable<BreakInModel>> GetBreakInDataAsync()
    {
        using var connection = _context.CreateConnection();
        var result = await connection.QueryAsync<BreakInModel>("[dbo].[Insurance_Job_GetBreakinDetails]",
                                                              commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result;
    }

    public async Task GetCKYCPaymentStatus()
    {
        try
        {
            var lstCKYCPaymentStatus = await GetCKYCPaymentStatusDB().ConfigureAwait(false);
            if (lstCKYCPaymentStatus != null && lstCKYCPaymentStatus.Any())
            {
                _logger.LogInformation("GetCKYCPaymentStatus count {count}", lstCKYCPaymentStatus.Count());
                foreach (var item in lstCKYCPaymentStatus)
                {
                    var token = CreateJWTTokenForPOSP(item.QuoteTransactionId);
                    _httpClient.DefaultRequestHeaders.Clear();
                    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    var statusURL = $"{_heroConfig.BaseURL}{_heroConfig.GoDigitCKYCPaymentStatusURL}{item.ApplicationId}?LeadId={item.LeadId}";
                    var ckycPaymentStatus = await _httpClient.GetAsync(statusURL);
                    _logger.LogInformation("GetCKYCPaymentStatus Response {response}", JsonConvert.SerializeObject(ckycPaymentStatus));
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("GetCKYCPaymentStatus exception {error}", ex.Message);
        }
    }

    private async Task<IEnumerable<CKYCPaymentStatusModel>> GetCKYCPaymentStatusDB()
    {
        using var connection = _context.CreateConnection();
        var result = await connection.QueryAsync<CKYCPaymentStatusModel>("[dbo].[Insurance_Job_GetCKYCPaymentStatus]",
                                                              commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result;
    }
    private string CreateJWTTokenForPOSP(string userId)
    {
        DateTime issuedAt = DateTime.UtcNow;
        DateTime expires = DateTime.UtcNow.AddDays(1);

        var tokenHandler = new JwtSecurityTokenHandler();

        ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
        {
                new Claim("UserId", userId)
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
}
