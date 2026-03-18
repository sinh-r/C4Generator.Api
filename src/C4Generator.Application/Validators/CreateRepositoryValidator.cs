using C4Generator.Application.Commands.Repositories;
using FluentValidation;

namespace C4Generator.Application.Validators;

public sealed class CreateRepositoryValidator : AbstractValidator<CreateRepositoryCommand>
{
    public CreateRepositoryValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Repository name is required.")
            .MaximumLength(200).WithMessage("Repository name must not exceed 200 characters.");

        RuleFor(x => x.Owner)
            .NotEmpty().WithMessage("Repository owner is required.")
            .MaximumLength(100).WithMessage("Owner must not exceed 100 characters.");

        RuleFor(x => x.Url)
            .NotEmpty().WithMessage("Repository URL is required.")
            .Must(BeAValidUrl).WithMessage("Repository URL must be a valid absolute HTTP/HTTPS URL.");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
            .When(x => x.Description is not null);
    }

    private static bool BeAValidUrl(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
               (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp);
    }
}
