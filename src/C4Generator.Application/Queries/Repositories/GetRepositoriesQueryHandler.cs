using C4Generator.Application.Abstractions;
using C4Generator.Application.DTOs;
using C4Generator.Domain.Entities;
using C4Generator.Domain.Interfaces;
using MediatR;

namespace C4Generator.Application.Queries.Repositories;

public sealed class GetRepositoriesQueryHandler
    : IRequestHandler<GetRepositoriesQuery, PagedResult<RepositoryDto>>
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(10);

    private readonly IUnitOfWork _unitOfWork;
    private readonly ISourceControlProviderFactory _providerFactory;

    public GetRepositoriesQueryHandler(
        IUnitOfWork unitOfWork,
        ISourceControlProviderFactory providerFactory)
    {
        _unitOfWork = unitOfWork;
        _providerFactory = providerFactory;
    }

    public async Task<PagedResult<RepositoryDto>> Handle(
        GetRepositoriesQuery request,
        CancellationToken cancellationToken)
    {
        var cached = await _unitOfWork.Repositories
            .GetByProviderAndOwnerAsync(request.Provider, request.Name, cancellationToken);

        var isFresh = cached.Count > 0 &&
                      cached.All(r => r.LastSyncedAt >= DateTime.UtcNow - CacheTtl);

        if (!isFresh)
        {
            var provider = _providerFactory.GetProvider(request.Provider);

            var remoteRepos = request.Scope == Domain.Enums.RepositoryScope.Organization
                ? await provider.GetOrganizationRepositoriesAsync(
                    request.Name, request.Token, request.PageNumber, request.PageSize, cancellationToken)
                : await provider.GetUserRepositoriesAsync(
                    request.Name, request.Token, request.PageNumber, request.PageSize, cancellationToken);

            foreach (var remote in remoteRepos)
            {
                var existing = await _unitOfWork.Repositories
                    .GetByExternalIdAsync(request.Provider, remote.ExternalId, cancellationToken);

                if (existing is not null)
                {
                    existing.Sync(remote.Description, remote.DefaultBranch, remote.Language, remote.Url);
                    _unitOfWork.Repositories.Update(existing);
                }
                else
                {
                    var newRepo = Repository.CreateFromSourceControl(
                        remote.Name,
                        remote.Owner,
                        remote.Url,
                        remote.ExternalId,
                        request.Provider,
                        remote.Description,
                        remote.DefaultBranch,
                        remote.Language);

                    await _unitOfWork.Repositories.AddAsync(newRepo, cancellationToken);
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            cached = await _unitOfWork.Repositories
                .GetByProviderAndOwnerAsync(request.Provider, request.Name, cancellationToken);
        }

        var totalCount = cached.Count;
        var items = cached
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(MapToDto)
            .ToList();

        return new PagedResult<RepositoryDto>(items, totalCount, request.PageNumber, request.PageSize);
    }

    private static RepositoryDto MapToDto(Domain.Entities.Repository r) =>
        new(r.Id, r.Name, r.Owner, r.Url, r.Description, r.DefaultBranch,
            r.Language, r.ArchitectureStatus, r.Provider, r.ExternalId,
            r.LastSyncedAt, r.CreatedAt, r.UpdatedAt);
}
