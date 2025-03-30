
using System.Text.Json.Serialization;

namespace FF.Articles.Backend.AI.API.Models.AiDtos;
public class AIGenArticle
{
    [JsonPropertyName("Id")]
    public int Id { get; set; }

    [JsonPropertyName("Title")]
    public string Title { get; set; } = "";

    [JsonPropertyName("Abstract")]
    public string Abstract { get; set; } = "";
    [JsonPropertyName("Tags")]
    public List<string> Tags { get; set; } = new();
}