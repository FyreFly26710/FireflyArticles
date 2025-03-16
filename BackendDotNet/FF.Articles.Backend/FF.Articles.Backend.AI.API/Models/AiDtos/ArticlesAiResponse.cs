using System;
using System.Text.Json.Serialization;

namespace FF.Articles.Backend.AI.API.Models.AiDtos;
public class ArticlesAIResponse
{
    [JsonPropertyName("Articles")]
    public List<AIGenArticle> Articles { get; set; } = new();

    [JsonPropertyName("AIMessage")]
    public string AIMessage { get; set; } = "";
}
