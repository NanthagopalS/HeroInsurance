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
    public class TATARepository : ITATARepository
    {
        private readonly ILogger<TATARepository> _logger;
        private readonly HttpClient _httpClient;
        private readonly HeroConfig _heroConfig;
        private readonly TokenSettings _tokenSettings;
        private readonly ApplicationDBContext _context;

        public TATARepository(ILogger<TATARepository> logger,
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

		public async Task GetBreakInStatus()
		{

			try
			{
				var lstTATABreaInStatus = await TATABreakInAndPaymentStatus(false);
				if (lstTATABreaInStatus != null && lstTATABreaInStatus.Any())
				{
					_logger.LogInformation("GetBreakInStatus count {count}", lstTATABreaInStatus.Count());
					foreach (var item in lstTATABreaInStatus)
					{
						var token = CreateJWTToken(Guid.NewGuid().ToString());
						_httpClient.DefaultRequestHeaders.Clear();
						_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
						var tataURL = $"{_heroConfig.BaseURL}{_heroConfig.TATABreakInURL}?VehicleTypeId={item.VehicleTypeId}&LeadId={item.LeadId}&ProposalNo={item.ProposalNo}&TicketId={item.TicketId}&QuoteTransactionId={item.QuoteTransactionId}";
						var breakInStatus = await _httpClient.GetAsync(tataURL);
						_logger.LogInformation("GetBreakInStatus Response {breakInStatus}", JsonConvert.SerializeObject(breakInStatus));
					}
				}

			}
			catch (Exception ex)
			{
				_logger.LogError("TATA BreakIn Status exception {error}", ex.Message);
			}
		}

        public async Task GetPaymentStatus()
        {

            try
            {
                var lstTATAPaymentStatus = await TATABreakInAndPaymentStatus(true);
                if (lstTATAPaymentStatus != null && lstTATAPaymentStatus.Any())
                {
                    _logger.LogInformation("GetPayment count {count}", lstTATAPaymentStatus.Count());
                    foreach (var item in lstTATAPaymentStatus)
                    {
                        var token = CreateJWTToken(Guid.NewGuid().ToString());
                        _httpClient.DefaultRequestHeaders.Clear();
                        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                        var tataURL = $"{_heroConfig.BaseURL}{_heroConfig.TATAVerifyPaymentURL}?VehicleTypeId={item.VehicleTypeId}&LeadId={item.LeadId}&PaymentId={item.PaymentId}&ProposalNo={item.ProposalNo}";
                        var paymentStatus = await _httpClient.GetAsync(tataURL);
                        _logger.LogInformation("GetPaymentStatus Response {paymentStatus}", JsonConvert.SerializeObject(paymentStatus));
                    }
                }

            }
            catch (Exception ex)
            {
                _logger.LogError("TATA Payment Status exception {error}", ex.Message);
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
		private async Task<IEnumerable<TATABreakInStatusModel>> TATABreakInAndPaymentStatus(bool isPayment)
		{
			using var connection = _context.CreateConnection();
			var parameters = new DynamicParameters();
			parameters.Add("InsurerId", _heroConfig.TATAInsurerId, DbType.String, ParameterDirection.Input);
			parameters.Add("IsPayment", isPayment, DbType.Boolean, ParameterDirection.Input);
			var result = await connection.QueryAsync<TATABreakInStatusModel>("[dbo].[Insurance_Job_GetTATABreakinAndPaymentStatus]", parameters, commandType: CommandType.StoredProcedure).ConfigureAwait(false);
			return result;
		}
	}
}
#endregion