using System;

namespace FF.AI.Common.Models;

public class ChatProvider
{
    public string ProviderName { get; set; } = string.Empty;
    public List<string>? Models { get; set; } = [];
    public bool IsAvailable { get; set; } = true;

}
