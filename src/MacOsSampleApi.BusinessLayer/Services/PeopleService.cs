using System.Reflection;
using System.Runtime.Intrinsics.X86;
using System.Linq.Expressions;
using System.ComponentModel;
using System;
using System.Data;
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
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        await db.People.AddAsync(dbPerson, cancellationToken);
        await db.SaveChangesAsync(true, cancellationToken);

        var createdPerson = new Person(dbPerson.Id, dbPerson.FirstName, dbPerson.LastName);
        return createdPerson;
    }

    public async Task<Result<Person>> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var dbPerson = await db.People.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        if (dbPerson is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "Person not found", $"Person not found with id {id}");
        }

        var person = new Person(dbPerson.Id, dbPerson.FirstName, dbPerson.LastName);
        return person;
    }
    
    public async Task<Result<IEnumerable<Person>>> GetListAsync(string? searchText, CancellationToken cancellationToken)
    {
        var people = await db.People.AsNoTracking().WhereIf(searchText.HasValue(), p => p.FirstName.Contains(searchText!) || p.LastName.Contains(searchText!))
            .Select(p => new Person(p.Id, p.FirstName, p.LastName))
            .ToListAsync(cancellationToken);

        return people;
    }

    public async Task<Result> UpdateAsync(Guid id, SavePersonRequest request, CancellationToken cancellationToken)
    {
        var person = await db.People.FindAsync([id], cancellationToken);
        if (person is null)
        {
            return Result.Fail(FailureReasons.ItemNotFound, "Person not found", $"Person not found with id {id}");
        }

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