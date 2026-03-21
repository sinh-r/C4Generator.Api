using C4Generator.Domain.Enums;

namespace C4Generator.Application.DTOs;

public sealed record AuthResponse(
    string Token,
    DateTime ExpiresAt,
    string Username,
    UserRole Role
);
