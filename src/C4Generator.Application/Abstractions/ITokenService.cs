using C4Generator.Domain.Entities;

namespace C4Generator.Application.Abstractions;

public record TokenResult(string Token, DateTime ExpiresAt);

public interface ITokenService
{
    TokenResult GenerateToken(User user);
}
