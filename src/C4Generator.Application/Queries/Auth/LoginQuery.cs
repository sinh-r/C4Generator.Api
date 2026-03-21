using C4Generator.Application.DTOs;
using MediatR;

namespace C4Generator.Application.Queries.Auth;

public record LoginQuery(
    string Email,
    string Password
) : IRequest<AuthResponse>;
