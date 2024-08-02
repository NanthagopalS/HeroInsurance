using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;
using ThirdPartyUtilities.Abstraction;
using ThirdPartyUtilities.Helpers;
using ThirdPartyUtilities.Models.JWT;
using ThirdPartyUtilities.Models.Log;
using ThirdPartyUtilities.Models.Signzy;

namespace ThirdPartyUtilities.Implementation;
public class SignzyService : ISignzyService
{
    private readonly ILogger<SignzyService> _logger;
    private readonly HttpClient _client;
    private readonly SignzyConfig _signzyConfig;
    private readonly ILogService _logService;

    private readonly string VehicleTask = "detailedSearch";
    private readonly string PANTask = "fetch";
    private readonly string PANType = "individualPan";
    private readonly string BankTask = "bankTransfer";
    private readonly string Service = "Identity";
    public IHttpContextAccessor HttpContextAccessor { get; }
    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="logger"></param>
    public SignzyService(ILogger<SignzyService> logger,
                         HttpClient client,
                         IOptions<SignzyConfig> options,
                         ILogService logService,
                         IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _signzyConfig = options.Value;
        _logService = logService;
        HttpContextAccessor = httpContextAccessor;
    }

    public async Task<AuthenticationResponse> AuthenticateSignzy(CancellationToken cancellationToken)
    {
        var id = 0;
        var responseBody = string.Empty;
        var authenticationModel = new AuthenticationRequest
        {
            username = _signzyConfig.Username,
            password = _signzyConfig.Password
        };
        var requestBody = JsonConvert.SerializeObject(authenticationModel);
        id = await InsertICLog(requestBody, string.Empty, _signzyConfig.BaseURL + _signzyConfig.LoginPath, string.Empty, string.Empty);
        try
        {
            var tokenResponse = await _client.PostAsync("api/v2/patrons/login", new StringContent(requestBody, Encoding.UTF8, "application/json"),
                cancellationToken)
                .ConfigureAwait(false);

            if (!tokenResponse.IsSuccessStatusCode)
            {
                _logger.LogError("Product not found");
                responseBody = await tokenResponse.Content.ReadAsStringAsync();
            }
            else
            {
                var stream = await tokenResponse.Content.ReadAsStreamAsync(cancellationToken);
                var authenticationResponse = stream.DeserializeFromJson<AuthenticationResponse>();
                responseBody = JsonConvert.SerializeObject(authenticationResponse);
                await UpdateICLog(id, string.Empty, responseBody);
                _logger.LogInformation("Signzy Authenticate details {response}", authenticationResponse);
                if (authenticationResponse != null)
                {
                    return authenticationResponse;
                }
            }
            await UpdateICLog(id, string.Empty, responseBody);
            return default;
        }
        catch(Exception ex)
        {
            _logger.LogError("AuthenticateSignzy Error {exception}", ex.Message);
            await UpdateICLog(id, string.Empty, ex.Message);
            return default;
        }
    }

    public async Task<VehicleRegistrationResponse> GetVehicleRegistrationDetails(string vehicleNumber, CancellationToken cancellationToken)
    {
        var id = 0;
        var responseBody = string.Empty;
        var authResponse = await AuthenticateSignzy(cancellationToken);
        if (authResponse != null)
        {
            var vehicleRegistrationRequest = new VehicleRegistrationRequest
            {
                task = VehicleTask,
                essentials = new Essentials
                {
                    vehicleNumber = vehicleNumber,
                    blacklistCheck = true,
                    signzyID = "HEROSZY"
                }
            };

            _client.DefaultRequestHeaders.Add("Authorization", authResponse.id);
            id = await InsertICLog(JsonConvert.SerializeObject(vehicleRegistrationRequest), string.Empty, _signzyConfig.BaseURL + $"api/v2/patrons/{authResponse.userId}/vehicleregistrations",
                authResponse.id, JsonConvert.SerializeObject(_client.DefaultRequestHeaders));
            try
            {
                var vehicleResponse = await _client.PostAsJsonAsync($"api/v2/patrons/{authResponse.userId}/vehicleregistrations", vehicleRegistrationRequest,
                cancellationToken)
                .ConfigureAwait(false);

                if (!vehicleResponse.IsSuccessStatusCode)
                {
                    responseBody = await vehicleResponse.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogError("Vehicle not found");
                }
                else
                {
                    var stream = await vehicleResponse.Content.ReadAsStreamAsync(cancellationToken);
                    var vehicleRegistrationResponse = stream.DeserializeFromJson<VehicleRegistrationResponse>();
                    responseBody = JsonConvert.SerializeObject(vehicleRegistrationResponse);
                    await UpdateICLog(id, string.Empty, responseBody);
                    _logger.LogInformation("Signzy Vehicle details {response}", vehicleRegistrationResponse);
                    if (vehicleRegistrationResponse != null)
                    {
                        _logger.LogInformation("Vehicle Registration {vehiclernumber} {response} ", vehicleNumber, JsonConvert.SerializeObject(vehicleRegistrationResponse));
                        return vehicleRegistrationResponse;
                    }
                }
                await UpdateICLog(id, string.Empty, responseBody);
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError("GetVehicleRegistrationDetails Error {exception}", ex.Message);
                await UpdateICLog(id, string.Empty, ex.Message);
                return default;
            }
        }
        return default;
    }

    public async Task<IdentitiesResponseModel> GetIdentities(string identityType, CancellationToken cancellationToken)
    {
        var id = 0;
        var responseBody = string.Empty;
        var authResponse = await AuthenticateSignzy(cancellationToken);
        if (authResponse != null)
        {
            var identitiesRequest = new IdentitiesRequestModel
            {
                type = identityType,
                email = _signzyConfig.Email,
                callbackUrl = _signzyConfig.CallBackURL,
                images = new List<byte[]>()
            };

            _client.DefaultRequestHeaders.Add("Authorization", authResponse.id);
            id = await InsertICLog(JsonConvert.SerializeObject(identitiesRequest), string.Empty, _signzyConfig.BaseURL + $"api/v2/patrons/{authResponse.userId}/identities",
                authResponse.id, JsonConvert.SerializeObject(_client.DefaultRequestHeaders));
            try
            {
                var identityResponse = await _client.PostAsJsonAsync($"api/v2/patrons/{authResponse.userId}/identities", identitiesRequest,
                        cancellationToken)
                        .ConfigureAwait(false);

                if (!identityResponse.IsSuccessStatusCode)
                {
                    responseBody = await identityResponse.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogError("Failed to verify Identity");
                }
                else
                {
                    var stream = await identityResponse.Content.ReadAsStreamAsync(cancellationToken);
                    var identitiesResponse = stream.DeserializeFromJson<IdentitiesResponseModel>();
                    responseBody = JsonConvert.SerializeObject(identitiesResponse);
                    await UpdateICLog(id, string.Empty, responseBody);
                    _logger.LogInformation("Signzy GetIdentities details {response}", identitiesResponse);
                    if (identitiesResponse != null)
                    {
                        return identitiesResponse;
                    }
                }
                await UpdateICLog(id, string.Empty, responseBody);
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError("GetIdentities Error {exception}", ex.Message);
                await UpdateICLog(id, string.Empty, ex.Message);
                return default;
            }
        }
        return default;
    }

    public async Task<PANVerificationResponse> GetPANDetails(string panNumber, CancellationToken cancellationToken)
    {
        var id = 0;
        var responseBody = string.Empty;
        var identities = await GetIdentities(PANType, cancellationToken);
        if (identities != null)
        {
            var pANVerificationRequest = new PANVerificationRequestModel
            {
                service = Service,
                itemId = identities.id,
                accessToken = identities.accessToken,
                task = PANTask,
                essentials = new PANVerificationEssentials
                {
                    number = panNumber
                }
            };
            id = await InsertICLog(JsonConvert.SerializeObject(pANVerificationRequest), string.Empty, _signzyConfig.BaseURL + $"api/v2/snoops",
                string.Empty, string.Empty);
            try
            {
                var responseMessage = await _client.PostAsJsonAsync($"api/v2/snoops", pANVerificationRequest,
                        cancellationToken)
                        .ConfigureAwait(false);

                if (!responseMessage.IsSuccessStatusCode)
                {
                    responseBody = await responseMessage.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogError("Failed to fetch PAN");
                    await UpdateICLog(id, string.Empty, responseBody);
                    return null;
                }
                else
                {
                    var stream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
                    var pANVerificationResponse = stream.DeserializeFromJson<PANVerificationResponse>();
                    responseBody = JsonConvert.SerializeObject(pANVerificationResponse);
                    await UpdateICLog(id, string.Empty, responseBody);
                    _logger.LogInformation("Signzy PAN details {response}", pANVerificationResponse);
                    if (pANVerificationResponse != null)
                    {
                        return pANVerificationResponse;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("GetPANDetails Error {exception}", ex.Message);
                await UpdateICLog(id, string.Empty, ex.Message);
                return default;
            }
        }
        return default;
    }

    public async Task<BankVerificationResponse> GetBankVerification(string beneficiaryMobile,
                                                                   string beneficiaryAccount,
                                                                   string beneficiaryName,
                                                                   string beneficiaryIFSC,
                                                                   CancellationToken cancellationToken)
    {
        var id = 0;
        var responseBody = string.Empty;
        var authenticationResponse = await AuthenticateSignzy(cancellationToken);
        if (authenticationResponse != null)
        {
            var bankVerificationRequest = new BankVerificationRequestModel
            {
                task = BankTask,
                essentials = new BankEssentials
                {
                    beneficiaryAccount = beneficiaryAccount,
                    beneficiaryIFSC = beneficiaryIFSC,
                    beneficiaryMobile = beneficiaryMobile,
                    beneficiaryName = beneficiaryName,
                    nameFuzzy = "true"
                }
            };

            _client.DefaultRequestHeaders.Add("Authorization", authenticationResponse.id);
            id = await InsertICLog(JsonConvert.SerializeObject(bankVerificationRequest), string.Empty, _signzyConfig.BaseURL + $"api/v2/patrons/{authenticationResponse.userId}/bankaccountverifications",
                authenticationResponse.id, JsonConvert.SerializeObject(_client.DefaultRequestHeaders));
            try
            {
                var responseMessage = await _client.PostAsJsonAsync($"api/v2/patrons/{authenticationResponse.userId}/bankaccountverifications", bankVerificationRequest,
                        cancellationToken)
                        .ConfigureAwait(false);

                if (!responseMessage.IsSuccessStatusCode)
                {
                    responseBody = await responseMessage.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogError("Failed to verify Bank details");
                }
                else
                {
                    try
                    {
                        var stream = await responseMessage.Content.ReadAsStreamAsync(cancellationToken);
                        var bankVerificationResponse = stream.DeserializeFromJson<BankVerificationResponse>();
                        responseBody = JsonConvert.SerializeObject(bankVerificationResponse);
                        await UpdateICLog(id, string.Empty, responseBody);
                        _logger.LogInformation("Signzy Bank details {response}", bankVerificationResponse);
                        if (bankVerificationResponse != null)
                        {
                            return bankVerificationResponse;
                        }
                    }
                    catch (Exception exx)
                    {
                        BankVerificationResponse respo = new BankVerificationResponse();
                        BankResult res = new BankResult();
                        res.reason = "Invalid Bank Account Details";
                        respo.result = res;
                        return respo;
                    }
                }
                await UpdateICLog(id, string.Empty, responseBody);
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError("GetBankVerification Error {exception}", ex.Message);
                await UpdateICLog(id, string.Empty, ex.Message);
                return default;
            }
        }
        return default;
    }

    private async Task UpdateICLog(int id, string applicationId, string data)
    {
        var logsModel = new LogModel
        {
            Id = id,
            ResponseBody = data,
            ResponseTime = DateTime.Now,
            ApplicationId = applicationId
        };
        await _logService.UpdateLog(logsModel);
    }
    private async Task<int> InsertICLog(string requestBody, string leadId, string api, string token, string header)
    {
        var userId = Convert.ToString(HttpContextAccessor.HttpContext.User.FindFirst(ApiClaimTypes.UserId).Value);
        var logsModel = new LogModel
        {
            InsurerId = string.Empty,
            RequestBody = requestBody,
            API = api,
            UserId = userId,
            Token = token,
            Headers = header,
            LeadId = leadId
        };

        var id = await _logService.InsertLog(logsModel);
        return id;
    }
}
