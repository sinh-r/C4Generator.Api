using C4Generator.Application.DTOs;
using MediatR;

namespace C4Generator.Application.Commands.Auth;

public record RegisterUserCommand(
    string Username,
    string Email,
    string Password
) : IRequest<AuthResponse>;
