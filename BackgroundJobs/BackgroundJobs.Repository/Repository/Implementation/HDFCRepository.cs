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
using System.Transactions;

namespace BackgroundJobs.Repository.Repository.Implementation;

public class HDFCRepository : IHDFCRepository
{
    private readonly ILogger<HDFCRepository> _logger;
    private readonly HttpClient _httpClient;
    private readonly HeroConfig _heroConfig;
    private readonly TokenSettings _tokenSettings;
    private readonly ApplicationDBContext _context;

    public HDFCRepository(ILogger<HDFCRepository> logger,
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

    public async Task GetCKYCStatus()
    {
        try
        {
            var lstHDFCCKYCPOAStatus = await HDFCCKYCPOAStatus().ConfigureAwait(false);
            if (lstHDFCCKYCPOAStatus != null && lstHDFCCKYCPOAStatus.Any())
            {
                _logger.LogInformation("GetCKYCStatus count {count}", lstHDFCCKYCPOAStatus.Count());
                foreach (var item in lstHDFCCKYCPOAStatus)
                {
                    var token = CreateJWTTokenForPOSP(Guid.NewGuid().ToString());
                    _httpClient.DefaultRequestHeaders.Clear();
                    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    var hdfcURL = $"{_heroConfig.BaseURL}{_heroConfig.HdfcCkycPoaStatusURL}/{item.QuoteTransactionId}/{item.KYCId}";
                    var ckycPaymentStatus = await _httpClient.GetAsync(hdfcURL);
                    _logger.LogInformation("GetCKYCStatus Response {response}", JsonConvert.SerializeObject(ckycPaymentStatus));
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("HDFC CKYC POA Status exception {error}", ex.Message);
        }
    }

    public async Task CreatePOSP()
    {
        try
        {
            var pospDetail = await GetPospDetails().ConfigureAwait(false);
            if (pospDetail != null && pospDetail.Any())
            {
                _logger.LogInformation("POSP Details Count {count}", pospDetail.Count());
                foreach (var pospCreateModel in pospDetail)
                {
                    var token = CreateJWTTokenForPOSP(Guid.NewGuid().ToString());
                    _httpClient.DefaultRequestHeaders.Clear();
                    _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                    var request = JsonConvert.SerializeObject(pospCreateModel);
                    var createPOSPURL = $"{_heroConfig.BaseURL}{_heroConfig.HdfcCreatePOSPURL}";
                    var pospStatus = await _httpClient.PostAsync(createPOSPURL, new StringContent(request, Encoding.UTF8, "application/json"));
                    var message = await pospStatus.Content.ReadAsStringAsync();
                    _logger.LogInformation("Create POSP Response {response}", JsonConvert.SerializeObject(message));
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("HDFC Create POSP exception {error}", ex.Message);
        }
    }

    #region Private Method

    private string CreateJWTTokenForPOSP(string userId)
    {
        DateTime issuedAt = DateTime.UtcNow;
        DateTime expires = DateTime.UtcNow.AddDays(1);

        var tokenHandler = new JwtSecurityTokenHandler();

        ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[]
        {
                new Claim("uid", userId)
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

    private async Task<IEnumerable<HDFCCKYCPOAStatusModel>> HDFCCKYCPOAStatus()
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("InsurerId", _heroConfig.HDFCInsurerId, DbType.String, ParameterDirection.Input);
        var result = await connection.QueryAsync<HDFCCKYCPOAStatusModel>("[dbo].[Insurance_Job_GetCKYCPOAStatus]", parameters,
                commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result;
    }

    private async Task<IEnumerable<HDFCCreatePOSPModel>> GetPospDetails()
    {
        using var connection = _context.CreateConnection();
        var parameters = new DynamicParameters();
        var result = await connection.QueryAsync<HDFCCreatePOSPModel>("[dbo].[Insurance_Job_GetPospId]", 
                commandType: CommandType.StoredProcedure).ConfigureAwait(false);
        return result;
    }
    #endregion
}
