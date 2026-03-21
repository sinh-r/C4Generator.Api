using System.ComponentModel.DataAnnotations;

namespace C4Generator.Infrastructure.Auth;

public sealed class JwtSettings
{
    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;

    [Required, MinLength(32, ErrorMessage = "SecretKey must be at least 32 characters (256-bit key for HMAC-SHA256).")]
    public string SecretKey { get; set; } = string.Empty;

    public int TokenExpiryMinutes { get; set; } = 60;
}
