using System.Linq;
using System.Threading;
using FluentValidation;
using MacOsSampleApi.Shared.Models.Requests;
using MacOsSampleApi.DataAccessLayer;
using Microsoft.EntityFrameworkCore;

namespace MacOsSampleApi.BusinessLayer.Validations;

public class SavePersonRequestValidator : AbstractValidator<SavePersonRequest>
{
    private readonly ApplicationDbContext db;

    public SavePersonRequestValidator(ApplicationDbContext db)
    {
        this.db = db;

        RuleFor(p => p.CityId)
            .NotEmpty()
            .WithMessage("The city is required")
            .MustAsync(ExistAsync)
            .WithMessage("City not found");

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

    private async Task<bool> ExistAsync(Guid cityId, CancellationToken cancellationToken)
        => await db.Cities.AnyAsync(c => c.Id == cityId, cancellationToken);
}