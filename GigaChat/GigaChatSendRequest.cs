using System.Text;
using System.Text.Json;
using GigaChat.DTO;
using Microsoft.Extensions.Configuration;

namespace GigaChat.GigaChat;

public class GigaChatSendRequest
{
    private readonly JsonSerializerOptions jsonOpt = new(JsonSerializerDefaults.Web);
    private readonly IConfiguration config;
    private readonly AccessToken accessTokenService;
    public GigaChatSendRequest(IConfiguration _config, AccessToken _accessTokenService)
    {
        config = _config;
        accessTokenService = _accessTokenService;
    }
    public string AskGigaChat(AccessTokenResponseDTO accessToken, List<ChatMessage> history)
    {
        //проверка срока действия выданного токена        
        //если до истечения срока токена меньше 10 секунд - запросить новый
        if ((DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - accessToken.ExpiresAt) > (1800000 - 10000))
        {
            accessToken = accessTokenService.GetAccessToken();
        }

        var token = accessToken.AccessToken;
        var jsonHistory = JsonSerializer.Serialize(new ChatRequest("GigaChat-2", history), jsonOpt);
        using var request = new HttpRequestMessage(HttpMethod.Post, config["ChatURL"])
        {
            Content = new StringContent(jsonHistory, Encoding.UTF8, "application/json")
        };
        var http = new HttpClient(new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        });
        http.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        using var response = http.Send(request);
        response.EnsureSuccessStatusCode();
        var result = JsonSerializer.Deserialize<ChatResponse>(ReadBody(response), jsonOpt);
        return result.Choices[0].Message.Content;
    }

    private string ReadBody(HttpResponseMessage response)
    {
        using var stream = response.Content.ReadAsStream();
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

}