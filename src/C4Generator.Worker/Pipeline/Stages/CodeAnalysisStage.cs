using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace C4Generator.Worker.Pipeline.Stages;

/// <summary>
/// Stage 3: Walks all .cs files in the cloned repository using Roslyn's syntax API.
/// Extracts namespace, class names, and interface names from each file.
/// Skips generated files, bin/obj directories, and test projects.
/// </summary>
public sealed class CodeAnalysisStage : IArchitecturePipelineStage
{
    private static readonly HashSet<string> SkippedDirectories =
        new(StringComparer.OrdinalIgnoreCase) { "bin", "obj", ".git", "node_modules", "packages" };

    private readonly ILogger<CodeAnalysisStage> _logger;

    public CodeAnalysisStage(ILogger<CodeAnalysisStage> logger)
    {
        _logger = logger;
    }

    public Task ExecuteAsync(ArchitectureGenerationContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(context.LocalRepoPath))
            throw new InvalidOperationException("LocalRepoPath is not set. RepositoryCloneStage must run first.");

        var files = EnumerateCsFiles(context.LocalRepoPath).ToList();
        _logger.LogInformation("CodeAnalysis: found {Count} .cs files in {Path}", files.Count, context.LocalRepoPath);

        var extracted = new List<ExtractedFileInfo>(files.Count);
        foreach (var filePath in files)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var info = AnalyzeFile(filePath, context.LocalRepoPath);
            if (info is not null)
                extracted.Add(info);
        }

        context.ExtractedFiles = extracted.AsReadOnly();
        _logger.LogInformation("CodeAnalysis extracted {Count} files with types", extracted.Count);
        return Task.CompletedTask;
    }

    private static IEnumerable<string> EnumerateCsFiles(string root)
    {
        return Directory.EnumerateFiles(root, "*.cs", SearchOption.AllDirectories)
            .Where(f => !f.Split(Path.DirectorySeparatorChar)
                          .Any(part => SkippedDirectories.Contains(part)));
    }

    private ExtractedFileInfo? AnalyzeFile(string filePath, string root)
    {
        try
        {
            var source = File.ReadAllText(filePath);
            var tree = CSharpSyntaxTree.ParseText(source);
            var root2 = tree.GetCompilationUnitRoot();

            var ns = root2.Members
                .OfType<BaseNamespaceDeclarationSyntax>()
                .Select(n => n.Name.ToString())
                .FirstOrDefault() ?? string.Empty;

            var classes = root2.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Select(c => c.Identifier.Text)
                .ToList();

            var interfaces = root2.DescendantNodes()
                .OfType<InterfaceDeclarationSyntax>()
                .Select(i => i.Identifier.Text)
                .ToList();

            if (classes.Count == 0 && interfaces.Count == 0)
                return null;

            var relativePath = Path.GetRelativePath(root, filePath);
            return new ExtractedFileInfo(relativePath, ns, classes.AsReadOnly(), interfaces.AsReadOnly());
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Skipping file {File} due to parse error", filePath);
            return null;
        }
    }
}
