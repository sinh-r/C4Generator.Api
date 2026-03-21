using C4Generator.Application.Abstractions;
using C4Generator.Application.DTOs;
using C4Generator.Application.Exceptions;
using C4Generator.Domain.Interfaces;
using MediatR;

namespace C4Generator.Application.Queries.Auth;

public sealed class LoginQueryHandler : IRequestHandler<LoginQuery, AuthResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;

    public LoginQueryHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ITokenService tokenService)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> Handle(LoginQuery request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.ToLowerInvariant();
        var user = await _unitOfWork.Users.GetByEmailAsync(normalizedEmail, cancellationToken);
        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password.");

        var tokenResult = _tokenService.GenerateToken(user);

        return new AuthResponse(tokenResult.Token, tokenResult.ExpiresAt, user.Username, user.Role);
    }
}
