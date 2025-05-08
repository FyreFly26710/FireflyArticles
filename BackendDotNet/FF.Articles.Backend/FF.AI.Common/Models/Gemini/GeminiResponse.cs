namespace FF.AI.Common.Models.Gemini;

internal class GeminiResponse
{
    public List<Candidate> Candidates { get; set; } = [];

    public UsageMetadata UsageMetadata { get; set; } = default!;

    public string ModelVersion { get; set; } = default!;
}

internal class Candidate
{
    public Content Content { get; set; } = default!;

    public string FinishReason { get; set; } = default!;

    public double AvgLogprobs { get; set; }
}

internal class UsageMetadata
{
    public int PromptTokenCount { get; set; }

    public int CandidatesTokenCount { get; set; }

    public int TotalTokenCount { get; set; }

    public List<TokenDetails> PromptTokensDetails { get; set; } = [];

    public List<TokenDetails> CandidatesTokensDetails { get; set; } = [];
}

internal class TokenDetails
{
    public string Modality { get; set; } = default!;

    public int TokenCount { get; set; }
}

internal static class GeminiResponseExtensions
{
    public static ChatResponse ToChatResponse(this GeminiResponse response)
    {
        var chatResponse = new ChatResponse();
        chatResponse.Message = response.Candidates.First().Content.ToMessage();
        chatResponse.ExtraInfo = new ExtraInfo
        {
            CreatedAt = DateTime.UtcNow,
            Model = response.ModelVersion,
            InputTokens = response.UsageMetadata.PromptTokenCount,
            OutputTokens = response.UsageMetadata.CandidatesTokenCount,
        };
        return chatResponse;
    }
}
