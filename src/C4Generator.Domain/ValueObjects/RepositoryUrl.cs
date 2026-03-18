namespace C4Generator.Domain.ValueObjects;

public sealed class RepositoryUrl
{
    public string Value { get; }

    private RepositoryUrl(string value) => Value = value;

    public static RepositoryUrl Create(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("Repository URL cannot be empty.", nameof(url));

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp))
            throw new ArgumentException("Repository URL must be a valid absolute HTTP/HTTPS URL.", nameof(url));

        return new RepositoryUrl(url.TrimEnd('/'));
    }

    public override string ToString() => Value;

    public override bool Equals(object? obj) => obj is RepositoryUrl other && Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode(StringComparison.OrdinalIgnoreCase);
}
