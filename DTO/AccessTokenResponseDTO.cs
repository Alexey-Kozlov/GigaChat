using System.Text.Json.Serialization;

namespace GigaChat.DTO;

public record AccessTokenResponseDTO(
    [property: JsonPropertyName("access_token")] string AccessToken,
    [property: JsonPropertyName("expires_at")] long ExpiresAt
);
