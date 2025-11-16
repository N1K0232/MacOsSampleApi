using System.Security.Authentication;
using System;
namespace MacOsSampleApi.DataAccessLayer.Entities;

public class Person
{
    public Guid Id { get; set; }

    public Guid CityId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public virtual City City { get; set; } = null!;
}