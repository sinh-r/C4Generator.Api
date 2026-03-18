using C4Generator.Application.Commands.Architecture;
using FluentValidation;

namespace C4Generator.Application.Validators;

public sealed class GenerateArchitectureValidator : AbstractValidator<GenerateArchitectureCommand>
{
    public GenerateArchitectureValidator()
    {
        RuleFor(x => x.RepositoryId)
            .NotEmpty().WithMessage("Repository ID is required.");

        RuleFor(x => x.Branch)
            .MaximumLength(200).WithMessage("Branch name must not exceed 200 characters.")
            .When(x => x.Branch is not null);
    }
}
