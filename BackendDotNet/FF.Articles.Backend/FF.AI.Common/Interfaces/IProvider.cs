using System;

namespace FF.AI.Common.Interfaces;

public interface IProvider
{
    string ChatEndpoint { get; }
    string? ApiKey { get; }
    string? ListModelsEndpoint { get; }
}
