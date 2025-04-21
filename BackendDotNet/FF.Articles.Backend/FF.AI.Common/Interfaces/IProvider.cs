using System;

namespace FF.AI.Common.Interfaces;

public interface IProvider
{
    string ProviderName { get; }
    string? ApiKey { get; }
    string ChatEndpoint { get; }
    string? ListModelsEndpoint { get; }
}
