using C4Generator.Application.Abstractions;
using C4Generator.Domain.Enums;

namespace C4Generator.Infrastructure.SourceControl;

internal sealed class SourceControlProviderFactory : ISourceControlProviderFactory
{
    private readonly Dictionary<SourceControlProvider, ISourceControlProvider> _providers;

    public SourceControlProviderFactory(IEnumerable<ISourceControlProvider> providers)
    {
        _providers = providers.ToDictionary(p => p.ProviderType);
    }

    public ISourceControlProvider GetProvider(SourceControlProvider provider)
    {
        if (!_providers.TryGetValue(provider, out var instance))
            throw new NotSupportedException($"Source control provider '{provider}' is not registered.");

        return instance;
    }
}
