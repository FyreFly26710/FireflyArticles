using System;
using System.Text.Json.Serialization;

namespace FF.Articles.Backend.AI.API.Models.AiDtos;
public class ArticlesAIResponseDto
{
    [JsonPropertyName("Articles")]
    public List<AIGenArticleDto> Articles { get; set; } = new();

    [JsonPropertyName("AIMessage")]
    public string AIMessage { get; set; } = "";
    [JsonPropertyName("Tags")]
    public List<string> Tags { get; set; } = new();
}
public class AIGenArticleDto
{
    [JsonPropertyName("Id")]
    public int Id { get; set; }

    [JsonPropertyName("Title")]
    public string Title { get; set; } = "";

    [JsonPropertyName("Abstract")]
    public string Abstract { get; set; } = "";
}