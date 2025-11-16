using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Linq.Expressions;
using System.ComponentModel;
using System;
using System.Data;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.Exceptions;
using System.Net.Cache;
using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Data.Common;
using System.Reflection.PortableExecutable;
using MacOsSampleApi.DataAccessLayer;
using MacOsSampleApi.Shared.Models;
using MacOsSampleApi.Shared.Models.Requests;
using Microsoft.EntityFrameworkCore;
using TinyHelpers.Extensions;
using MacOsSampleApi.BusinessLayer.Services.Interfaces;
using OperationResults;
using Entities = MacOsSampleApi.DataAccessLayer.Entities;

namespace MacOsSampleApi.BusinessLayer.Services;
    
public class PeopleService(ApplicationDbContext db) : IPeopleService
{
    public async Task<Result<Person>> CreateAsync(SavePersonRequest request, CancellationToken cancellationToken)
    {
        var dbPerson = new Entities.Person
        {
            CityId = request.CityId,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        await db.People.AddAsync(dbPerson, cancellationToken);
        await db.SaveChangesAsync(true, cancellationToken);

        var createdPerson = new Person(dbPerson.Id, dbPerson.FirstName, dbPerson.LastName, dbPerson.City?.Name);
        return createdPerson;
    }

    public async Task<Result<Person>> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var dbPerson = await db.People.AsNoTracking()
            .Include(p => p.City)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        
        if (dbPerson is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "Person not found", $"Person not found with id {id}");
        }

        var person = new Person(dbPerson.Id, dbPerson.FirstName, dbPerson.LastName, dbPerson.City.Name);
        return person;
    }
    
    public async Task<Result<PaginatedList<Person>>> GetListAsync(string? searchText, int pageIndex, int itemsPerPage, string orderBy, CancellationToken cancellationToken)
    {
        var query = db.People.AsNoTracking()
            .Include(p => p.City)
            .WhereIf(searchText.HasValue(), p => p.FirstName.Contains(searchText!) || p.LastName.Contains(searchText!));
        
        var totalCount = await query.CountAsync(cancellationToken);

        try
        {
            query = query.OrderBy(orderBy);
        }
        catch (ParseException ex)
        {
            return Result.Fail(FailureReasons.ClientError, "Unable to order", ex.Message);
        }

        var dbPeople = await query.Skip(pageIndex * itemsPerPage).Take(itemsPerPage + 1).ToListAsync(cancellationToken);
        var people = dbPeople.Take(itemsPerPage).Select(p => new Person(p.Id, p.FirstName, p.LastName, p.City.Name));

        var list = new PaginatedList<Person>(people, totalCount, dbPeople.Count > itemsPerPage);
        return list;
    }

    public async Task<Result> UpdateAsync(Guid id, SavePersonRequest request, CancellationToken cancellationToken)
    {
        var person = await db.People.FindAsync([id], cancellationToken);
        if (person is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "Person not found", $"Person not found with id {id}");
        }

        person.CityId = request.CityId;
        person.FirstName = request.FirstName;
        person.LastName = request.LastName;

        await db.SaveChangesAsync(true, cancellationToken);
        return Result.Ok();
    }

    public async Task<Result> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var person = await db.People.FindAsync([id], cancellationToken);
        if (person is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "Person not found", $"Person not found with id {id}");
        }

        db.People.Remove(person);

        await db.SaveChangesAsync(true, cancellationToken);
        return Result.Ok();
    }
}