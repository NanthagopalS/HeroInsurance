using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Admin.Core.Helpers;
public static class ApiHelper<T>
{
    public static async Task<HttpResponseMessage> HttpClientHelper(int requestType, string accessToken, string baseURL, string urlPath, T request)
    {
        using var client = new HttpClient();

        client.BaseAddress = new Uri(baseURL);
        client.DefaultRequestHeaders.Clear();

        var defaultRequestHeaders = client.DefaultRequestHeaders;
        if (defaultRequestHeaders.Accept == null || !defaultRequestHeaders.Accept.Any(m => m.MediaType == "application/json"))
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        if (requestType == 1)
        {
            var authenticationString = $"25860711:digit123";
            var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(authenticationString));
            defaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
        }
        else
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        HttpResponseMessage Res = await client.PostAsJsonAsync(urlPath, request);
        return Res;
    }
}
