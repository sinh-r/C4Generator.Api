using LibGit2Sharp;
using Microsoft.Extensions.Options;

namespace C4Generator.Worker.Pipeline.Stages;

/// <summary>
/// Stage 2: Clones the repository into a local temp directory.
/// If the directory already exists it performs a fast-forward pull instead.
/// </summary>
public sealed class RepositoryCloneStage : IArchitecturePipelineStage
{
    private readonly WorkerSettings _settings;
    private readonly ILogger<RepositoryCloneStage> _logger;

    public RepositoryCloneStage(IOptions<WorkerSettings> settings, ILogger<RepositoryCloneStage> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public Task ExecuteAsync(ArchitectureGenerationContext context, CancellationToken cancellationToken)
    {
        var localPath = Path.Combine(_settings.TempDirectory, context.RepositoryId.ToString());
        Directory.CreateDirectory(_settings.TempDirectory);

        if (Directory.Exists(Path.Combine(localPath, ".git")))
        {
            _logger.LogInformation("Repository already cloned at {Path}, pulling latest.", localPath);
            using var repo = new LibGit2Sharp.Repository(localPath);
            var remote = repo.Network.Remotes["origin"];
            var refSpecs = remote.FetchRefSpecs.Select(r => r.Specification);
            Commands.Fetch(repo, remote.Name, refSpecs, null, null);

            var branch = repo.Branches[context.Branch]
                ?? repo.Branches[$"origin/{context.Branch}"];

            if (branch is not null)
                Commands.Checkout(repo, branch);
        }
        else
        {
            _logger.LogInformation("Cloning {Url} → {Path}", context.RepositoryUrl, localPath);
            var cloneOptions = new CloneOptions { BranchName = context.Branch };
            LibGit2Sharp.Repository.Clone(context.RepositoryUrl, localPath, cloneOptions);
        }

        context.LocalRepoPath = localPath;
        _logger.LogInformation("Repository ready at {Path}", localPath);
        return Task.CompletedTask;
    }
}
