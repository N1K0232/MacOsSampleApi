using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MacOsSampleApi.Shared.Models;
using MacOsSampleApi.Shared.Models.Requests;
using OperationResults;

namespace MacOsSampleApi.BusinessLayer.Services.Interfaces;

public interface IPeopleService
{
    Task<Result<Person>> CreateAsync(SavePersonRequest request, CancellationToken cancellationToken);

    Task<Result<Person>> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<IEnumerable<Person>>> GetListAsync(string? searchText, CancellationToken cancellationToken);

    Task<Result> UpdateAsync(Guid id, SavePersonRequest request, CancellationToken cancellationToken);

    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);
}