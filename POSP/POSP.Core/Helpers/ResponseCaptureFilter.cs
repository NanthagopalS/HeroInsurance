using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using POSP.Core.Contracts.Common;
using POSP.Core.Contracts.Persistence;
using POSP.Domain.APILogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace POSP.Core.Helpers;


public class ResponseCaptureFilter : IAsyncResultFilter
{

	private readonly ILogger<ResponseCaptureFilter> _logger;
	private readonly IHttpContextAccessor _httpContextAccessor;
	private readonly IApplicationClaims _applicationClaims;
	private readonly ILogsRepository _logsRepository;

	public ResponseCaptureFilter(ILogger<ResponseCaptureFilter> logger, IHttpContextAccessor httpContextAccessor,
		 IApplicationClaims applicationClaims, ILogsRepository logsRepository)
	{
		_logger = logger ?? throw new ArgumentNullException(nameof(logger));
		_httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
		_applicationClaims = applicationClaims ?? throw new ArgumentNullException(nameof(applicationClaims));
		_logsRepository = logsRepository ?? throw new ArgumentNullException(nameof(_logsRepository));
	}

	public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
	{
		var originalBody = _httpContextAccessor.HttpContext.Response.Body;
		var responseBody = new MemoryStream();

		try
		{
			string reqbody = "";
			long contentLength = _httpContextAccessor.HttpContext.Request.ContentLength.GetValueOrDefault();
			if (contentLength > 0)
			{
				_httpContextAccessor.HttpContext.Request.Body.Seek(0, SeekOrigin.Begin);
				using (StreamReader reader = new StreamReader(_httpContextAccessor.HttpContext.Request.Body))
				{
					reqbody = await reader.ReadToEndAsync();
					// Use body string
				}
			}

			_httpContextAccessor.HttpContext.Response.Body = responseBody;

			await next();

			responseBody.Seek(0, SeekOrigin.Begin);
			var response = await new StreamReader(responseBody).ReadToEndAsync();

			// Log or process the 'response' data here
			_logger.LogInformation($"Response captured: {response}");

			responseBody.Seek(0, SeekOrigin.Begin);
			await responseBody.CopyToAsync(originalBody);

			string resp = response.Length > 2000 ? response.Substring(0, 2000) : response;
			var reqUrl = ExtractURL(_httpContextAccessor.HttpContext.Request);
			var requestMethod = _httpContextAccessor.HttpContext.Request.Method;
			var requestPath = _httpContextAccessor.HttpContext.Request.Path;
			var queryString = await ExtractRequestBodyFromQueryString(_httpContextAccessor.HttpContext.Request);
			var body = "";
			if (!string.IsNullOrEmpty(queryString) && !string.IsNullOrEmpty(reqbody))
			{
				var reqbodyDict = JsonSerializer.Deserialize<Dictionary<string, object>>(reqbody);
				var queryStringDict = JsonSerializer.Deserialize<Dictionary<string, object>>(queryString);

				// Merge dictionaries
				var mergedDict = queryStringDict.Concat(reqbodyDict)
												.ToDictionary(x => x.Key, x => x.Value);

				// Serialize merged dictionary back to JSON string
				body = JsonSerializer.Serialize(mergedDict);
			}
			var ress = !string.IsNullOrEmpty(queryString) ? queryString : reqbody;
			var responseStatusCode = _httpContextAccessor.HttpContext.Response.StatusCode.ToString();
			var reqqbody = !string.IsNullOrEmpty(body) ? body : ress;
			var userId = _applicationClaims.GetUserId();
			var apiLogs = new APILogsModel()
			{
				RequestMethod = requestMethod,
				RequestBody = reqqbody,
				RequestUrl = reqUrl,
				RequestPath = requestPath,
				ResponseStatusCode = responseStatusCode,
				ResponseBody = resp,
				UserId = userId
			};
			await InsertICLogs(apiLogs);
		}
		catch (Exception ex)
		{
			_logger.LogInformation(ex, "Exception occurred in OnResultExecutionAsync");
			// Handle the exception here or rethrow it if necessary
			throw;
		}
		finally
		{
			_httpContextAccessor.HttpContext.Response.Body = originalBody;
		}
	}
	private async Task<string> ExtractRequestBodyFromQueryString(HttpRequest request)
	{
		var uri = request.GetEncodedUrl(); // Get the full URL

		// Separate the query string part from the URL
		var queryStringStart = uri.IndexOf('?');
		if (queryStringStart >= 0 && queryStringStart < uri.Length - 1)
		{
			var fullQueryString = uri.Substring(queryStringStart + 1); // Get the query string part

			// Separate the regular query parameters from the appended body content
			var queryParts = fullQueryString.Split('&');
			// Splitting the query string by '&' to get individual key-value pairs
			var keyValuePairs = fullQueryString.Split('&')
				.Select(part => part.Split('='))
				.ToDictionary(split => split[0], split => split[1]);

			// Serialize the dictionary to JSON
			var json = JsonSerializer.Serialize(keyValuePairs);

			return json;
		}


		return null; // Return null if no body content was found in the query string
	}
	private async Task<string> ReadRequestBody(HttpRequest request)
	{
		request.EnableBuffering();
		using (StreamReader reader = new StreamReader(request.Body, Encoding.UTF8, true, 1024, true))
		{
			string bodyContent = await reader.ReadToEndAsync();
			request.Body.Position = 0; // Reset the position of the request body stream
			return bodyContent;
		}
	}
	private async Task<int> InsertICLogs(APILogsModel logsModel)
	{
		var id = await _logsRepository.InsertAPILogs(logsModel);
		return id;
	}
	private string ExtractURL(HttpRequest request)
	{
		var url = request.GetEncodedUrl();
		return url;
	}
}

