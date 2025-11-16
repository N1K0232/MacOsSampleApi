using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MacOsSampleApi.Shared.Models;
using MacOsSampleApi.Shared.Models.Requests;
using OperationResults;

namespace MacOsSampleApi.BusinessLayer.Services.Interfaces;

public interface ICityService
{
    Task<Result<City>> CreateAsync(SaveCityRequest request, CancellationToken cancellationToken);

    Task<Result<City>> GetAsync(Guid id, CancellationToken cancellationToken);

    Task<Result<IEnumerable<City>>> GetListAsync(string? name, CancellationToken cancellationToken);

    Task<Result> UpdateAsync(Guid id, SaveCityRequest request, CancellationToken cancellationToken);

    Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken);  
}