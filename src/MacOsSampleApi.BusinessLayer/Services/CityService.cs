using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MacOsSampleApi.DataAccessLayer;
using MacOsSampleApi.Shared.Models;
using MacOsSampleApi.Shared.Models.Requests;
using Microsoft.EntityFrameworkCore;
using TinyHelpers.Extensions;
using MacOsSampleApi.BusinessLayer.Services.Interfaces;
using OperationResults;
using Entities = MacOsSampleApi.DataAccessLayer.Entities;

namespace MacOsSampleApi.BusinessLayer.Services;

public class CityService(ApplicationDbContext db) : ICityService
{
    public async Task<Result<City>> CreateAsync(SaveCityRequest request, CancellationToken cancellationToken)
    {
        var dbCity = new Entities.City
        {
            Name = request.Name
        };

        await db.Cities.AddAsync(dbCity, cancellationToken);
        await db.SaveChangesAsync(true, cancellationToken);

        var createdCity = new City(dbCity.Id, dbCity.Name);
        return createdCity;
    }

    public async Task<Result<City>> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var dbCity = await db.Cities.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (dbCity is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "City not found", $"City not found with id {id}");
        }

        var city = new City(dbCity.Id, dbCity.Name);
        return city;
    }
    
    public async Task<Result<IEnumerable<City>>> GetListAsync(string? name, CancellationToken cancellationToken)
    {
        var cities = await db.Cities.WhereIf(name.HasValue(), c => c.Name.Contains(name!))
            .Select(c => new City(c.Id, c.Name))
            .ToListAsync(cancellationToken);

        return cities;
    }

    public async Task<Result> UpdateAsync(Guid id, SaveCityRequest request, CancellationToken cancellationToken)
    {
        var city = await db.Cities.FindAsync([id], cancellationToken);
        if (city is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "City not found", $"City not found with id {id}");
        }

        city.Name = request.Name;

        await db.SaveChangesAsync(true, cancellationToken);
        return Result.Ok();
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var city = await db.Cities.FindAsync([id], cancellationToken);
        if (city is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "City not found", $"City not found with id {id}");
        }

        db.Cities.Remove(city);

        await db.SaveChangesAsync(true, cancellationToken);
        return Result.Ok();
    }
}