using MediatR;

namespace C4Generator.Application.Commands.Repositories;

public record DeleteRepositoryCommand(Guid RepositoryId) : IRequest;
