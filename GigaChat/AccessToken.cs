using System.Net.Http.Headers;
using System.Text.Json;
using GigaChat.DTO;
using Microsoft.Extensions.Configuration;

namespace GigaChat.GigaChat;

public class AccessToken
{
    private readonly IConfiguration config;
    private readonly JsonSerializerOptions jsonOpt = new(JsonSerializerDefaults.Web);
    public AccessToken(IConfiguration _config)
    {
        config = _config;
    }
    public AccessTokenResponseDTO GetAccessToken()
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, config["AuthURL"]);
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", config["AuthorizationKey"]);
        request.Headers.Add("RqUID", Guid.NewGuid().ToString());
        request.Content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["scope"] = "GIGACHAT_API_PERS"
        });
        var httpClient = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        });
        using var response = httpClient.Send(request);
        response.EnsureSuccessStatusCode();
        return JsonSerializer.Deserialize<AccessTokenResponseDTO>(ReadBody(response), jsonOpt);

    }

    private string ReadBody(HttpResponseMessage response)
    {
        using var stream = response.Content.ReadAsStream();
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}