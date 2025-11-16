using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MacOsSampleApi.Shared.Models.Requests;

namespace MacOsSampleApi.BusinessLayer.Validations;

public class SaveCityRequestValidator : AbstractValidator<SaveCityRequest>
{
    public SaveCityRequestValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty()
            .WithMessage("Name is required")
            .MaximumLength(255)
            .WithMessage("Max length is 255");
    }
}