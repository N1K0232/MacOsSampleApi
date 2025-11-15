using FluentValidation;
using MacOsSampleApi.Shared.Models.Requests;

namespace MacOsSampleApi.BusinessLayer.Validations;

public class SavePersonRequestValidator : AbstractValidator<SavePersonRequest>
{
    public SavePersonRequestValidator()
    {
        RuleFor(p => p.FirstName)
            .NotEmpty()
            .WithMessage("FirstName is required")
            .MaximumLength(255)
            .WithMessage("Max length is 255");

        RuleFor(p => p.LastName)
            .NotEmpty()
            .WithMessage("LastName is required")
            .MaximumLength(255)
            .WithMessage("Max length is 255");
    }
}