using C4Generator.Application.DTOs;
using MediatR;

namespace C4Generator.Application.Commands.Repositories;

public record CreateRepositoryCommand(
    string Name,
    string Owner,
    string Url,
    string? Description
) : IRequest<RepositoryDto>;
