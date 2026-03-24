using C4Generator.Domain.Enums;

namespace C4Generator.Application.Abstractions;

public interface ISourceControlProviderFactory
{
    ISourceControlProvider GetProvider(SourceControlProvider provider);
}
