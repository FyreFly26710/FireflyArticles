using System;
using System.Text.Json.Serialization;

namespace FF.Articles.Backend.AI.API.Models.AiDtos;
public class ArticlesAIResponseDto
{
    public List<AIGenArticleDto> Articles { get; set; } = new();

    public string AIMessage { get; set; } = "";

    public long TopicId { get; set; }

}
public class AIGenArticleDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Abstract { get; set; } = "";
    public List<string> Tags { get; set; } = new();
}