namespace C4Generator.Worker;

public sealed class WorkerSettings
{
    public string TempDirectory { get; set; } = Path.Combine(Path.GetTempPath(), "c4generator", "repos");
    public string GitHubToken { get; set; } = string.Empty;
}

public sealed class GeminiSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gemini-2.0-flash";
    public string BaseUrl { get; set; } = "https://generativelanguage.googleapis.com";
}
