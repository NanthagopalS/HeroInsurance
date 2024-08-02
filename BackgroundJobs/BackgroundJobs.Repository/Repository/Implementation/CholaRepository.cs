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

namespace BackgroundJobs.Repository.Repository.Implementation
{
    public class CholaRepository : ICholaRepository
    {
        private readonly ILogger<CholaRepository> _logger;
        private readonly HttpClient _httpClient;
        private readonly HeroConfig _heroConfig;
        private readonly TokenSettings _tokenSettings;
        private readonly ApplicationDBContext _context;

        public CholaRepository(ILogger<CholaRepository> logger, HttpClient httpClient, IOptions<HeroConfig> options, IOptions<TokenSettings> tokenSetting, ApplicationDBContext applicationDBContext)
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
                var lstCholaKYCStatus = await CholaCKYCStatus().ConfigureAwait(false);
                if (lstCholaKYCStatus != null && lstCholaKYCStatus.Any())
                {
                    _logger.LogInformation("GetCKYCStatus count {count}", lstCholaKYCStatus.Count());
                    foreach (var item in lstCholaKYCStatus)
                    {
                        var token = CreateJWTToken(Guid.NewGuid().ToString());
                        _httpClient.DefaultRequestHeaders.Clear();
                        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                        var cholaURL = $"{_heroConfig.BaseURL}{_heroConfig.CholaCkycStatusURL}/{item.TransactionId}/{item.appRefNo}/{item.QuoteTransactionId}";
                        var ckycStatus = await _httpClient.GetAsync(cholaURL);
                        _logger.LogInformation("GetCKYCStatus Response {ckycStatus}", JsonConvert.SerializeObject(ckycStatus));
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Chola CKYC Status exception {error}", ex.Message);
            }
        }

        public async Task GetBreakInStatus()
        {
            try
            {
                var lstCholaBreaInStatus = await CholaBreakInStatus();
                if (lstCholaBreaInStatus != null && lstCholaBreaInStatus.Any())
                {
                    _logger.LogInformation("GetBreakInStatus count {count}", lstCholaBreaInStatus.Count());
                    foreach (var item in lstCholaBreaInStatus)
                    {
                        var token = CreateJWTToken(Guid.NewGuid().ToString());
                        _httpClient.DefaultRequestHeaders.Clear();
                        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                        var cholaURL = $"{_heroConfig.BaseURL}{_heroConfig.CholaBreakinStatusURL}/{item.BreakinId}?LeadId={item.LeadId}";
                        var breakInStatus = await _httpClient.GetAsync(cholaURL);
                        _logger.LogInformation("GetBreakInStatus Response {breakInStatus}", JsonConvert.SerializeObject(breakInStatus));
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Chola BreakIn Status exception {error}", ex.Message);
            }
        }

        public async Task GetPaymentStatus()
        {
            try
            {
                var lstCholaPaymentStatus = await CholaPaymentStatus();
                if (lstCholaPaymentStatus != null && lstCholaPaymentStatus.Any())
                {
                    _logger.LogInformation("GetPaymentStatus count {count}", lstCholaPaymentStatus.Count());
                    foreach (var item in lstCholaPaymentStatus)
                    {
                        var token = CreateJWTToken(Guid.NewGuid().ToString());
                        _httpClient.DefaultRequestHeaders.Clear();
                        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                        var cholaURL = $"{_heroConfig.BaseURL}{_heroConfig.CholaPaymentStatusURL}/{item.ApplicationId}";
                        var paymentStatus = await _httpClient.GetAsync(cholaURL);
                        _logger.LogInformation($"GetPaymentStatus Response {paymentStatus}", JsonConvert.SerializeObject(paymentStatus));
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("Chola CKYC Status exception {error}", ex.Message);
            }
        }

        #region Private Method
        private async Task<IEnumerable<CholaCKYCStatusModel>> CholaCKYCStatus()
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("InsurerId", _heroConfig.CholaInsurerId, DbType.String, ParameterDirection.Input);
            var result = await connection.QueryAsync<CholaCKYCStatusModel>("[dbo].[Insurance_Job_GetCholaCKYCStatus]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            return result;
        }
        private async Task<IEnumerable<CholaBreakInStatusModel>> CholaBreakInStatus()
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("InsurerId", _heroConfig.CholaInsurerId, DbType.String, ParameterDirection.Input);
            var result = await connection.QueryAsync<CholaBreakInStatusModel>("[dbo].[Insurance_Job_GetCholaBreakinStatus]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            return result;
        }

        private async Task<IEnumerable<CholaPaymentStatusModel>> CholaPaymentStatus()
        {
            using var connection = _context.CreateConnection();
            var parameters = new DynamicParameters();
            parameters.Add("InsurerId", _heroConfig.CholaInsurerId, DbType.String, ParameterDirection.Input);
            var result = await connection.QueryAsync<CholaPaymentStatusModel>("[dbo].[Insurance_Job_GetCholaPaymentStatus]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
            return result;
        }

        private string CreateJWTToken(string userId)
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
        #endregion
    }
}
