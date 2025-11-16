using MinimalHelpers.Routing;
using OperationResults.AspNetCore.Http;
using MacOsSampleApi.Shared.Models;
using MacOsSampleApi.Shared.Models.Requests;
using MacOsSampleApi.BusinessLayer.Services.Interfaces;
using MinimalHelpers.FluentValidation;
using OperationResults;

namespace MacOsSampleApi.Endpoints;

public class CitiesEndpoint : IEndpointRouteHandlerBuilder
{
    public static void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var citiesApiGroup = endpoints.MapGroup("/api/cities").WithTags("Cities");

        citiesApiGroup.MapPost(string.Empty, CreateAsync)
            .Produces<City>(StatusCodes.Status201Created)
            .WithValidation<SaveCityRequest>()
            .WithName("CreateCity");

        citiesApiGroup.MapGet("{id:guid}", GetAsync)
            .Produces<City>()
            .Produces(StatusCodes.Status404NotFound)
            .WithName("GetCity");

        citiesApiGroup.MapGet(string.Empty, GetListAsync)
            .Produces<PaginatedList<City>>()
            .Produces(StatusCodes.Status400BadRequest)
            .WithName("GetCities");

        citiesApiGroup.MapPut("{id:guid}", UpdateAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithValidation<SaveCityRequest>()
            .WithName("UpdateCity");

        citiesApiGroup.MapDelete("{id:guid}", DeleteAsync)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithName("DeleteCity");
    }

    private static async Task<IResult> CreateAsync(SaveCityRequest request, ICityService cityService, HttpContext httpContext)
    {
        var result = await cityService.CreateAsync(request, httpContext.RequestAborted);
        return httpContext.CreateResponse(result, "GetCity", new { id = result.Content?.Id });
    }

    private static async Task<IResult> GetAsync(Guid id, ICityService cityService, HttpContext httpContext)
    {
        var result = await cityService.GetAsync(id, httpContext.RequestAborted);
        return httpContext.CreateResponse(result);
    }

    private static async Task<IResult> GetListAsync(ICityService cityService, HttpContext httpContext, string? name = null)
    {
        var result = await cityService.GetListAsync(name, httpContext.RequestAborted);
        return httpContext.CreateResponse(result);
    }

    private static async Task<IResult> UpdateAsync(Guid id, SaveCityRequest request, ICityService cityService, HttpContext httpContext)
    {
        var result = await cityService.UpdateAsync(id, request, httpContext.RequestAborted);
        return httpContext.CreateResponse(result);
    }

    private static async Task<IResult> DeleteAsync(Guid id, ICityService cityService, HttpContext httpContext)
    {
        var result = await cityService.DeleteAsync(id, httpContext.RequestAborted);
        return httpContext.CreateResponse(result);
    }
}