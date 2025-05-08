using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FF.AI.Common.Models.Gemini;
//https://ai.google.dev/api/caching#Content
internal class GeminiRequest
{
    public Content[] Contents { get; set; } = [];
    public GenerationConfig? GenerationConfig { get; set; }
}

internal class GenerationConfig
{
    [JsonPropertyName("responseMimeType")]
    public string ResponseMimeType { get; set; } = "application/json";

    [JsonPropertyName("responseSchema")]
    public object? ResponseSchema { get; set; }

    [JsonPropertyName("temperature")]
    public float? Temperature { get; set; }
}

internal static class GeminiRequestExtensions
{
    public static GeminiRequest ToGeminiRequest(this ChatRequest request)
    {
        var geminiRequest = new GeminiRequest
        {
            Contents = request.Messages.Select(m => m.ToContent()).ToArray()
        };

        if (request.Options != null)
        {
            geminiRequest.GenerationConfig = new GenerationConfig();

            // if (request.Options.ResponseFormat != null)
            // {
            //     geminiRequest.GenerationConfig.ResponseSchema = ConvertToGeminiSchema(request.Options.ResponseFormat);
            // }

            if (request.Options.Temperature.HasValue)
            {
                geminiRequest.GenerationConfig.Temperature = (float)request.Options.Temperature.Value;
            }
        }

        return geminiRequest;
    }

}


