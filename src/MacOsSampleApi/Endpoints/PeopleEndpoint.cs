using MinimalHelpers.Routing;
using OperationResults.AspNetCore.Http;
using MacOsSampleApi.Shared.Models;
using MacOsSampleApi.Shared.Models.Requests;
using MacOsSampleApi.BusinessLayer.Services.Interfaces;
using MinimalHelpers.FluentValidation;
using OperationResults;

namespace MacOsSampleApi.Endpoints;

public class PeopleEndpoint : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var peopleApiGroup = endpoints.MapGroup("/api/people").WithTags("People");

        peopleApiGroup.MapPost(string.Empty, CreateAsync)
            .Produces<Person>(StatusCodes.Status201Created)
            .WithValidation<SavePersonRequest>()
            .WithName("CreatePerson");

        peopleApiGroup.MapGet("{id:guid}", GetAsync)
            .Produces<Person>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetPerson");

        peopleApiGroup.MapGet(string.Empty, GetListAsync)
            .Produces<PaginatedList<Person>>()
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("GetPeople");

        peopleApiGroup.MapPut("{id:guid}", UpdateAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithValidation<SavePersonRequest>()
            .WithName("UpdatePerson");

        peopleApiGroup.MapDelete("{id:guid}", DeleteAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("DeletePerson");
    }

    private static async Task<IResult> CreateAsync(SavePersonRequest request, IPeopleService peopleService, HttpContext httpContext)
    {
        var result = await peopleService.CreateAsync(request, httpContext.RequestAborted);
        return httpContext.CreateResponse(result, "GetPerson", new { id = result.Content?.Id });
    }

    private static async Task<IResult> GetAsync(Guid id, IPeopleService peopleService, HttpContext httpContext)
    {
        var result = await peopleService.GetAsync(id, httpContext.RequestAborted);
        return httpContext.CreateResponse(result);
    }

    private static async Task<IResult> GetListAsync(IPeopleService peopleService, HttpContext httpContext, string? searchText = null, int pageIndex = 0, int itemsPerPage = 10, string orderBy = "FirstName, LastName")
    {
        var result = await peopleService.GetListAsync(searchText, pageIndex, itemsPerPage, orderBy, httpContext.RequestAborted);
        return httpContext.CreateResponse(result);
    }

    private static async Task<IResult> UpdateAsync(Guid id, SavePersonRequest request, IPeopleService peopleService, HttpContext httpContext)
    {
        var result = await peopleService.UpdateAsync(id, request, httpContext.RequestAborted);
        return httpContext.CreateResponse(result);
    }

    private static async Task<IResult> DeleteAsync(Guid id, IPeopleService peopleService, HttpContext httpContext)
    {
        var result = await peopleService.DeleteAsync(id, httpContext.RequestAborted);
        return httpContext.CreateResponse(result);
    }
}