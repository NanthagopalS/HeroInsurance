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
using System.Net.Http;
using System.Security.Claims;

namespace BackgroundJobs.Repository.Repository.Implementation;

public class IFFCORepository : IIFFCORepository
{
    private readonly ILogger<IFFCORepository> _logger;
    private readonly HttpClient _httpClient;
    private readonly HeroConfig _heroConfig;
    private readonly TokenSettings _tokenSettings;
    private readonly ApplicationDBContext _context;
    public  IFFCORepository(ILogger<IFFCORepository> logger,
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
    public async Task GetBreakinPinStatus()
    {
        try
        {
            var breakinDetails = await GetBreakinDetails().ConfigureAwait(false);
            if (breakinDetails != null && breakinDetails.Any())
            {
                _logger.LogInformation("GetPolicyStatus count {count}", breakinDetails.Count());
                foreach (var item in breakinDetails)
                {
                    var token = CreateJWTToken(item.UserId);
                    _httpClient.DefaultRequestHeaders.Clear();
                    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    var breakInURL = $"{_heroConfig.BaseURL}{_heroConfig.IFFCOBreakInURL}/{item.QuoteTransactionId}/{item.BreakinId}/{item.ProposalNumber}";
                    var policyStatus = await _httpClient.PostAsync(breakInURL, null);
                    var message = await policyStatus.Content.ReadAsStringAsync();
                    _logger.LogInformation("PinStatus Response {response}", JsonConvert.SerializeObject(message));
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Get IFFCO Breakin status exception {error}", ex.Message);
        }
    }

    #region Private Method

    private string CreateJWTToken(string userId)
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
    #endregion
    private async Task<IEnumerable<IFFCOBreakinStatusModel>> GetBreakinDetails()
    {
        using var connection = _context.CreateConnection();
        var result = await connection.QueryAsync<IFFCOBreakinStatusModel>("[dbo].[Insurance_Job_GetIFFCOBreakinStatus]",
                                                              commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result;
    }
}
