namespace FF.Articles.Backend.Identity.API.Models.OAuth;

public class UserInfo
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("verified_email")]
    public bool VerifiedEmail { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("given_name")]
    public string GivenName { get; set; } = string.Empty;

    [JsonPropertyName("family_name")]
    public string FamilyName { get; set; } = string.Empty;

    [JsonPropertyName("picture")]
    public string Picture { get; set; } = string.Empty;

    [JsonPropertyName("locale")]
    public string Locale { get; set; } = string.Empty;
}
