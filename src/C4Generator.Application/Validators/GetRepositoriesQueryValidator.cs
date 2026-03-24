using C4Generator.Application.Queries.Repositories;
using C4Generator.Domain.Enums;
using FluentValidation;

namespace C4Generator.Application.Validators;

public sealed class GetRepositoriesQueryValidator : AbstractValidator<GetRepositoriesQuery>
{
    public GetRepositoriesQueryValidator()
    {
        RuleFor(x => x.Provider)
            .IsInEnum().WithMessage("Provider must be a valid source control provider.");

        RuleFor(x => x.Scope)
            .IsInEnum().WithMessage("Scope must be either Organization or User.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Organization or user name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("A source control access token is required.");

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("Page number must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100).WithMessage("Page size must be between 1 and 100.");
    }
}
