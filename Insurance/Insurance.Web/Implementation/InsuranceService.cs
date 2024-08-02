using Insurance.Web.Abstraction;
using Insurance.Web.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Insurance.Web.Implementation;

public class InsuranceService : IInsuranceService
{
    private readonly HttpClient _httpClient;
    private readonly HeroConfig _heroConfig;
    private readonly ILogger<InsuranceService> _logger;
    private readonly TokenSettings _tokenSettings;
    public InsuranceService(HttpClient httpClient, IOptions<HeroConfig> options, ILogger<InsuranceService> logger, IOptions<TokenSettings> tokensettings)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _heroConfig = options.Value;
        _logger = logger;
        _tokenSettings = tokensettings.Value;
    }

    public async Task<PaymentDetailsVm> SavePaymentStatus(string applicationId, string userId, string transactionNumber, CancellationToken cancellationToken)
    {
        _logger.LogInformation("GoDigit Transaction Number {transactionNumber}", transactionNumber);
        var token = CreateJWTTokenForPOSP(userId);
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        var paymentResponse = await _httpClient.GetStringAsync($"{_heroConfig.GoDigitURL}{applicationId}", cancellationToken);
        var result = JsonConvert.DeserializeObject<HeroResult>(paymentResponse);
        _logger.LogInformation("GoDigit Payment {Response}", JsonConvert.SerializeObject(result));
        if (result.data.succeeded)
        {
            var paymentDetails = JsonConvert.DeserializeObject<PaymentDetailsVm>(result.data.result.ToString());
            return paymentDetails;
        }
        return default;
    }

    public async Task<PaymentDetailsVm> SaveBajajPaymentStatus(BajajPaymentResponseModel bajajPaymentResponseModel, CancellationToken cancellationToken)
    {
        var token = CreateJWTTokenForPOSP(bajajPaymentResponseModel.userId);
        var requestBody = JsonConvert.SerializeObject(bajajPaymentResponseModel);
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        var url = $"{_heroConfig.BaseURL}{_heroConfig.BajajURL}{bajajPaymentResponseModel.id}";

        var paymentResponse = await _httpClient.PostAsync(url, new StringContent(requestBody, Encoding.UTF8, "application/json")).ConfigureAwait(false);
        var stream = await paymentResponse.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<HeroResult>(stream);
        _logger.LogInformation("Bajaj Payment {Response}", JsonConvert.SerializeObject(result));
        if (result.data.succeeded)
        {
            var paymentDetails = JsonConvert.DeserializeObject<PaymentDetailsVm>(result.data.result.ToString());
            return paymentDetails;
        }
        return default;
    }

    public async Task<PaymentDetailsVm> SaveBajajTPPaymentStatus(BajajPaymentResponseModel bajajPaymentResponseModel,
        CancellationToken cancellationToken)
    {
        var token = CreateJWTTokenForPOSP(bajajPaymentResponseModel.userId);
        var requestBody = JsonConvert.SerializeObject(bajajPaymentResponseModel);
        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        var url = $"{_heroConfig.BaseURL}{_heroConfig.BajajTPURL}{bajajPaymentResponseModel.id}";
        var paymentResponse = await _httpClient.PostAsync(url, new StringContent(requestBody, Encoding.UTF8, "application/json")).ConfigureAwait(false);

        var stream = await paymentResponse.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<HeroResult>(stream);
        _logger.LogInformation("Bajaj Payment {Response}", JsonConvert.SerializeObject(result));
        if (result.data.succeeded)
        {
            var paymentDetails = JsonConvert.DeserializeObject<PaymentDetailsVm>(result.data.result.ToString());
            return paymentDetails;
        }
        return default;
    }
    public async Task<PaymentDetailsVm> SaveICICIPaymentStatus(string userId, string correlationId, string transactionId)
    {
        try
        {
            PaymentDetailsVm paymentDetailsVm = new PaymentDetailsVm();
            if (!string.IsNullOrEmpty(transactionId))
            {
                var token = CreateJWTTokenForPOSP(userId);

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var url = $"{_heroConfig.BaseURL}{_heroConfig.ICICIURL}{correlationId}/{transactionId}";
                var paymentResponse = await _httpClient.GetStringAsync(url);
                var result = JsonConvert.DeserializeObject<ICICIHeroResult>(paymentResponse);
                _logger.LogInformation("ICICI Payment {Response}", JsonConvert.SerializeObject(result));
                if (result.statusCode == "200")
                {
                    return result.data;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation("ICICI Payment Exception", ex);
            return default;
        }
        return default;
    }

    public async Task<CKYCPOAResponseModel> GetHDFCPOAStatus(HDFCPOAResponse hdfcPOAResponse, CancellationToken cancellationToken)
    {
        try
        {
            var token = CreateJWTTokenForPOSP(hdfcPOAResponse.userId);
            var requestBody = JsonConvert.SerializeObject(hdfcPOAResponse);
            _logger.LogInformation("HDFC CKYC POA Request {ckycRequest}", requestBody);


            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var url = _heroConfig.BaseURL + _heroConfig.HDFCPOAStatusURL + hdfcPOAResponse.id;
            var ckycResponse = await _httpClient.PostAsync(url, new StringContent(requestBody, Encoding.UTF8, "application/json"), cancellationToken).ConfigureAwait(false);

            if (ckycResponse.IsSuccessStatusCode)
            {
                var resContent = ckycResponse.Content.ReadAsStringAsync().Result;
                var res = JsonConvert.DeserializeObject<CKYCPOAResponseModel>(resContent);
                _logger.LogInformation("HDFC CKYC POA Response {ckycResponse}", resContent);
                return res;
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation("HDFC CKYC POA Exception", ex);
        }
        return default;
    }
    private string CreateJWTTokenForPOSP(string userId)
    {
        DateTime issuedAt = DateTime.UtcNow;
        DateTime expires = DateTime.UtcNow.AddDays(1);

        _logger.LogInformation("JWT Token Starter {userid}", userId);
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
        _logger.LogInformation("JWT Token token {token}", tokenString);
        return tokenString;
    }

    public async Task<HeroResult> SaveHDFCPaymentStatus(string quoteTransactionId, string userId, HDFCPaymentResponseModel hdfcPaymentModel, CancellationToken cancellationToken)
    {
        HeroResult paymentDetailsVm = new HeroResult();
        try
        {
            var token = CreateJWTTokenForPOSP(userId);
            var requestBody = JsonConvert.SerializeObject(hdfcPaymentModel);
            _logger.LogInformation("HDFC Payment Response {PaymentRedirectionResponse}", requestBody);

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var url = _heroConfig.BaseURL + _heroConfig.HDFCPGStatusURL + quoteTransactionId;
            var paymentResponse = await _httpClient.PostAsync(url, new StringContent(requestBody, Encoding.UTF8, "application/json"), cancellationToken).ConfigureAwait(false);

            if (paymentResponse.IsSuccessStatusCode)
            {
                var resContent = paymentResponse.Content.ReadAsStringAsync().Result;
                paymentDetailsVm = JsonConvert.DeserializeObject<HeroResult>(resContent);
                _logger.LogInformation("HDFC Payment Status Response {PaymentStatusResponse}", paymentDetailsVm);
                return paymentDetailsVm;
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation("HDFC CKYC POA Exception", ex);
        }
        return default;
    }

    public async Task<ICICIHeroResult> SaveTATAPaymentStatus(string userId, TATAPaymentResponseModel tataPaymentModel, CancellationToken cancellationToken)
    {
        ICICIHeroResult paymentDetailsVm = new ICICIHeroResult();
        try
        {
            var token = CreateJWTTokenForPOSP(userId);
            var requestBody = JsonConvert.SerializeObject(tataPaymentModel);
            _logger.LogInformation("TATA Payment Response {PaymentRedirectionResponse}", requestBody);

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var url = _heroConfig.BaseURL + _heroConfig.TATAPGStatusRedirectionURL;
            var paymentResponse = await _httpClient.PostAsync(url, new StringContent(requestBody, Encoding.UTF8, "application/json"), cancellationToken).ConfigureAwait(false);

            if (paymentResponse.IsSuccessStatusCode)
            {
                var resContent = paymentResponse.Content.ReadAsStringAsync().Result;
                paymentDetailsVm = JsonConvert.DeserializeObject<ICICIHeroResult>(resContent);
                _logger.LogInformation("TATA Payment Status Response {PaymentStatusResponse}", paymentDetailsVm);
                return paymentDetailsVm;
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation("TATA CKYC POA Exception", ex);
        }
        return default;
    }
    public async Task<PaymentDetailsVm> SaveCholaPaymentStatus(string quoteTransactionId, string userId, CholaPaymentResponseModel cholaPaymentModel, CancellationToken cancellationToken)
    {
        try
        {
            var token = CreateJWTTokenForPOSP(userId);
            var requestBody = JsonConvert.SerializeObject(cholaPaymentModel);
            _logger.LogInformation("Chola Payment Response {PaymentRedirectionResponse}", requestBody);

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var url = _heroConfig.BaseURL + _heroConfig.CholaPGStatusURL + quoteTransactionId;
            var paymentResponse = await _httpClient.PostAsync(url, new StringContent(requestBody, Encoding.UTF8, "application/json"), cancellationToken).ConfigureAwait(false);

            if (paymentResponse.IsSuccessStatusCode)
            {
                var resContent = paymentResponse.Content.ReadAsStringAsync().Result;
                var paymentResponseData = JsonConvert.DeserializeObject<ICICIHeroResult>(resContent);
                _logger.LogInformation("Chola Payment Status Response {PaymentStatusResponse}", resContent);
                if (paymentResponseData != null && paymentResponseData.data.InsurerStatusCode.Equals(200))
                {
                    return paymentResponseData.data;
                }
                return default;
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Chola Payment Status Exception {Response}", ex);
        }
        return default;
    }
    public async Task<CKYCPOAResponseModel> GetCholaCKYCDetails(CholaKYCResponse cholaKYCResponse, CancellationToken cancellationToken)
    {
        try
        {
            var token = CreateJWTTokenForPOSP(cholaKYCResponse.userId);
            var requestBody = JsonConvert.SerializeObject(cholaKYCResponse);
            _logger.LogInformation("Chola GetCholaCKYCDetails Response {requestBody}", requestBody);


            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var url = _heroConfig.BaseURL + _heroConfig.CholaPOAStatusURL + cholaKYCResponse.quoteTransactionId;
            var ckycResponse = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);

            if (ckycResponse.IsSuccessStatusCode)
            {
                var resContent = ckycResponse.Content.ReadAsStringAsync().Result;
                var res = JsonConvert.DeserializeObject<CKYCPOAResponseModel>(resContent);
                _logger.LogInformation("Chola GetCholaCKYCDetails Response {resContent}", resContent);
                return res;
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Chola CKYC POA Exception", ex);
        }
        return default;
    }

    public async Task<ICICIHeroResult> SaveIFFCOPaymentStatus(IFFCOPaymentResponseModel iFFCOPaymentResponseModel, CancellationToken cancellationToken)
    {
        PaymentDetailsVm paymentDetailsVm = new PaymentDetailsVm();
        try
        {
            if (iFFCOPaymentResponseModel != null)
            {
                var token = CreateJWTTokenForPOSP(iFFCOPaymentResponseModel.UserId);

                var requestBody = JsonConvert.SerializeObject(iFFCOPaymentResponseModel);
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                var url = _heroConfig.BaseURL + _heroConfig.IFFCOSavePaymentStatus;
                var paymentResponse = await _httpClient.PostAsync(url, new StringContent(requestBody, Encoding.UTF8, "application/json"), cancellationToken).ConfigureAwait(false);

                if (paymentResponse.IsSuccessStatusCode)
                {
                    var resContent = paymentResponse.Content.ReadAsStringAsync().Result;
                    var paymentResponseData = JsonConvert.DeserializeObject<ICICIHeroResult>(resContent);
                    _logger.LogInformation("IFFCO Payment Status Response {PaymentStatusResponse}", resContent);
                    if (paymentResponseData != null)
                    {
                        return paymentResponseData;
                    }
                    return default;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation("ICICI Payment Exception", ex);
        }
        return default;
    }

    public async Task<CKYCPOAResponseModel> GetRelianceCKYCDetails(RelianceKYCResponse relianceKYCResponse, CancellationToken cancellationToken)
    {
        try
        {
            var token = CreateJWTTokenForPOSP(relianceKYCResponse.UserId);
            var requestBody = JsonConvert.SerializeObject(relianceKYCResponse);
            _logger.LogInformation("Reliance CKYC POA Request {ckycRequest}", requestBody);


            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var url = _heroConfig.BaseURL + _heroConfig.ReliancePOAStatusURL + relianceKYCResponse.QuoteTransactionId;
            var ckycResponse = await _httpClient.PostAsync(url, new StringContent(requestBody, Encoding.UTF8, "application/json"), cancellationToken).ConfigureAwait(false);

            if (ckycResponse.IsSuccessStatusCode)
            {
                var resContent = ckycResponse.Content.ReadAsStringAsync().Result;
                var res = JsonConvert.DeserializeObject<CKYCPOAResponseModel>(resContent);
                _logger.LogInformation("HDFC CKYC POA Response {ckycResponse}", resContent);
                return res;
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Reliance CKYC POA Exception", ex);
        }
        return default;
    }

    public async Task<PaymentDetailsVm> SaveReliancePaymentStatus(string quoteTransactionId, string userId, ReliancePaymentResponseModel reliancePaymentModel, CancellationToken cancellationToken)
    {
        try
        {

            var token = CreateJWTTokenForPOSP(Guid.NewGuid().ToString());
            var requestBody = JsonConvert.SerializeObject(reliancePaymentModel);
            _logger.LogInformation("Reliance Payment Response {PaymentRedirectionResponse}", requestBody);

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var url = _heroConfig.BaseURL + _heroConfig.ReliancePGStatusURL + quoteTransactionId;
            var paymentResponse = await _httpClient.PostAsync(url, new StringContent(requestBody, Encoding.UTF8, "application/json"), cancellationToken).ConfigureAwait(false);

            if (paymentResponse.IsSuccessStatusCode)
            {
                var resContent = paymentResponse.Content.ReadAsStringAsync().Result;
                var paymentResponseData = JsonConvert.DeserializeObject<ICICIHeroResult>(resContent);
                _logger.LogInformation("Reliance Payment Status Response {PaymentStatusResponse}", resContent);
                if (paymentResponseData != null && paymentResponseData.data.InsurerStatusCode.Equals(200))
                {
                    return paymentResponseData.data;
                }
                return default;
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Reliance Payment Status Exception {message}", ex);
        }
        return default;
    }

    public async Task<ICICIHeroResult> SaveUnitedIndiaPaymentStatus(string userId, UnitedIndiaPaymentResponseModel UnitedIndiaPaymentModel, CancellationToken cancellationToken)
    {
        ICICIHeroResult paymentDetailsVm = new ICICIHeroResult();
        try
        {
            var token = CreateJWTTokenForPOSP(userId);
            var requestBody = JsonConvert.SerializeObject(UnitedIndiaPaymentModel);
            _logger.LogInformation("UnitedIndia Payment Response {PaymentRedirectionResponse}", requestBody);

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var url = _heroConfig.BaseURL + _heroConfig.UnitedIndiaPGStatusRedirectionURL;
            var paymentResponse = await _httpClient.PostAsync(url, new StringContent(requestBody, Encoding.UTF8, "application/json"), cancellationToken).ConfigureAwait(false);

            if (paymentResponse.IsSuccessStatusCode)
            {
                var resContent = paymentResponse.Content.ReadAsStringAsync().Result;
                paymentDetailsVm = JsonConvert.DeserializeObject<ICICIHeroResult>(resContent);
                _logger.LogInformation("UnitedIndia Payment Status Response {PaymentStatusResponse}", paymentDetailsVm);
                return paymentDetailsVm;
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation("UnitedIndia Save Payment Status Exception {Message}", ex.Message);
        }
        return default;
    }

    public async Task<LeadDetails> SaveUnitedIndiaCKYCStatus(UnitedIndiaCKYCResponseModel unitedIndiaCKYCResponseModel, CancellationToken cancellationToken)
    {
        UIICHeroResult leadDetails = new UIICHeroResult();
        try
        {
            var token = CreateJWTTokenForPOSP(unitedIndiaCKYCResponseModel.UserId);
            var requestBody = JsonConvert.SerializeObject(unitedIndiaCKYCResponseModel);
            _logger.LogInformation("UnitedIndia CKYC Response {PaymentRedirectionResponse}", requestBody);

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var url = _heroConfig.BaseURL + _heroConfig.UnitedIndiaCKYCStatusRedirectionURL;
            var paymentResponse = await _httpClient.PostAsync(url, new StringContent(requestBody, Encoding.UTF8, "application/json"), cancellationToken).ConfigureAwait(false);

            if (paymentResponse.IsSuccessStatusCode)
            {
                var resContent = paymentResponse.Content.ReadAsStringAsync().Result;
                leadDetails = JsonConvert.DeserializeObject<UIICHeroResult>(resContent);
                _logger.LogInformation("UnitedIndia Save CKYC POA Status Response {leadInformation}", leadDetails);
                return leadDetails?.data;
            }
            return default;
        }
        catch (Exception ex)
        {
            _logger.LogInformation("UnitedIndia Save CKYC POA Status Exception {message}", ex);
        }
        return default;
    }

    public async Task<HeroResult> GetProposalDetails(string insurerId, string quotransactionId, string userId, CancellationToken cancellationToken)
    {
        _logger.LogInformation("IFFCO GetProposalDetails Started {insurer} {user} {quoite}", insurerId, userId, quotransactionId);

        var token = CreateJWTTokenForPOSP(userId);

        _httpClient.DefaultRequestHeaders.Clear();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        var url = $"{_heroConfig.BaseURL}{_heroConfig.GetProposalDetails}{insurerId}/{quotransactionId}";
        var proposalRequest = await _httpClient.GetAsync(url, cancellationToken);

        _logger.LogInformation("IFFCO GetProposalDetails {status}", proposalRequest.IsSuccessStatusCode);

        if (proposalRequest.IsSuccessStatusCode)
        {
            var request = await proposalRequest.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonConvert.DeserializeObject<HeroResult>(request);
            _logger.LogInformation("IFFCO GetProposalDetails Response {proposalResponse}", result);
            return result;
        }
        return default;
    }
    public async Task<string> GetUserIdDetails(string proposalNumber, CancellationToken cancellationToken)
    {
        try
        {
            var token = CreateJWTTokenForPOSP(Guid.NewGuid().ToString());

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            var url = $"{_heroConfig.BaseURL}{_heroConfig.GetUserIdDetails}{_heroConfig.IFFCOInsurerId}/{proposalNumber}";
            var userId = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);

            if (userId.IsSuccessStatusCode)
            {
                var request = userId.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<GetDatailsHeroResult>(request);
                _logger.LogInformation("Get UserId Details Response {proposalResponse}", result);
                return result.data[0];
            }
        }
        catch (Exception ex)
        {
            _logger.LogInformation("Get UserId Details Exception", ex);
        }
        return default;
    }
}
