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
using System.Text;

namespace BackgroundJobs.Repository.Repository.Implementation;

public class ICICIRepository : IICICIRepository
{
    private readonly ILogger<ICICIRepository> _logger;
    private readonly HttpClient _httpClient;
    private readonly HeroConfig _heroConfig;
    private readonly TokenSettings _tokenSettings;
    private readonly ApplicationDBContext _context;
    private readonly IdentityApplicationDBContext _identityApplicationDBContext;

    public ICICIRepository(ILogger<ICICIRepository> logger,
                             HttpClient httpClient,
                             IOptions<HeroConfig> options,
                             IOptions<TokenSettings> tokenSetting,
                             ApplicationDBContext applicationDBContext,
                             IdentityApplicationDBContext identityApplicationDBContext
        )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClient = httpClient;
        _heroConfig = options.Value;
        _tokenSettings = tokenSetting.Value;
        _context = applicationDBContext;
        _identityApplicationDBContext = identityApplicationDBContext;
    }

    public async Task GetPolicyStatus()
    {
        try
        {
            var token = CreateJWTTokenForPOSP(Guid.NewGuid().ToString());
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var breakInURL = $"{_heroConfig.BaseURL}{_heroConfig.ICICIBreakInURL}";
            var policyStatus = await _httpClient.GetAsync(breakInURL);
            var message = await policyStatus.Content.ReadAsStringAsync();
            _logger.LogInformation("PolicyStatus Response {response}", JsonConvert.SerializeObject(message));
        }
        catch (Exception ex)
        {
            _logger.LogError("GetKYCStatus exception {error}", ex.Message);
        }
    }



    public async Task GetPaymentStatus()
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
                    var statusURL = $"{_heroConfig.BaseURL}{_heroConfig.ICICICKYCPaymentStatusURL}{item.ApplicationId}";
                    var paymentStatus = await _httpClient.GetAsync(statusURL);
                    var message = await paymentStatus.Content.ReadAsStringAsync();
                    _logger.LogInformation("GetPaymentStatus Response {response}", JsonConvert.SerializeObject(message));
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("GetCKYCPaymentStatus exception {error}", ex.Message);
        }
    }

    public async Task CreateIMBroker()
    {
        try
        {
            var pospDetails = await GetPOSPDetails().ConfigureAwait(false);
            if (pospDetails != null && pospDetails.Any())
            {
                _logger.LogInformation("Create IM Broker count {count}", pospDetails.Count());
                foreach (var item in pospDetails)
                {
                    var token = CreateJWTTokenForPOSP(Guid.NewGuid().ToString());
                    _httpClient.DefaultRequestHeaders.Clear();
                    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    var request = JsonConvert.SerializeObject(item);
                    var createBrokerURL = $"{_heroConfig.BaseURL}{_heroConfig.ICICIIMBrokerURL}";
                    var paymentStatus = await _httpClient.PostAsync(createBrokerURL, new StringContent(request, Encoding.UTF8, "application/json"));
                    var message = await paymentStatus.Content.ReadAsStringAsync();
                    _logger.LogInformation("Create IM Broker Response {response}", JsonConvert.SerializeObject(message));
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Create IM Broker exception {error}", ex.Message);
        }
    }

    #region Private Method

    private async Task<IEnumerable<BreakInModel>> GetBreakInDataAsync()
    {
        using var connection = _context.CreateConnection();
        var result = await connection.QueryAsync<BreakInModel>("[dbo].[Insurance_Job_GetICICIBreakinDetails]",
                                                              commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result;
    }
    private async Task<IEnumerable<CKYCPaymentStatusModel>> GetCKYCPaymentStatusDB()
    {
        using var connection = _context.CreateConnection();
        var result = await connection.QueryAsync<CKYCPaymentStatusModel>("[dbo].[Insurance_Job_GetICICIPaymentStatus]",
                                                              commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result;
    }
    private async Task<IEnumerable<ICICICreateIMBrokerModel>> GetPOSPDetails()
    {
        using var connection = _identityApplicationDBContext.CreateConnection();
        var result = await connection.QueryAsync<ICICICreateIMBrokerModel>("[dbo].[Identity_Job_GetICICIPOSPOnboardDetails]",
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
            new Claim("uid", userId)
        }); ;

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
    #endregion
}
