using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using TinyHelpers.AspNetCore.Extensions;
using MacOsSampleApi.BusinessLayer.Settings;
using TinyHelpers.AspNetCore.OpenApi;
using MacOsSampleApi.Swagger;
using Microsoft.EntityFrameworkCore;
using MacOsSampleApi.DataAccessLayer;
using MacOsSampleApi.BusinessLayer.Services;
using MacOsSampleApi.BusinessLayer.Services.Interfaces;
using MacOsSampleApi.BusinessLayer.Validations;
using MinimalHelpers.Routing;
using MinimalHelpers.Validation;
using OperationResults.AspNetCore.Http;
using ResultErrorResponseFormat = OperationResults.AspNetCore.Http.ErrorResponseFormat;
using ValidationErrorResponseFormat = MinimalHelpers.Validation.ErrorResponseFormat;

var builder = WebApplication.CreateBuilder(args);

var settings = builder.Services.ConfigureAndGet<AppSettings>(builder.Configuration, nameof(AppSettings)) ?? new AppSettings();
var swagger = builder.Services.ConfigureAndGet<SwaggerSettings>(builder.Configuration, nameof(SwaggerSettings)) ?? new SwaggerSettings();

builder.Services.AddHttpContextAccessor();
builder.Services.AddRequestLocalization(settings.SupportedCultures);

builder.Services.AddDefaultProblemDetails();
builder.Services.AddDefaultExceptionHandler();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddOperationResult(options => 
{
    options.ErrorResponseFormat = ResultErrorResponseFormat.List;
});

builder.Services.ConfigureValidation(options =>
{
    options.ErrorResponseFormat = ValidationErrorResponseFormat.List;
});

builder.Services.AddValidatorsFromAssemblyContaining<SavePersonRequestValidator>();

if(swagger.IsEnabled)
{
    builder.Services.AddOpenApi(options => 
    {
        options.RemoveServerList();
        options.AddAcceptLanguageHeader();
        options.AddDefaultProblemDetailsResponse();
    });
}

builder.Services.AddDbContext<ApplicationDbContext>(options => 
{
    options.UseInMemoryDatabase("application");
});

builder.Services.AddScoped<IPeopleService, PeopleService>();

var app = builder.Build();
app.UseHttpsRedirection();

app.UseRouting();
app.UseRequestLocalization();

if(swagger.IsEnabled)
{
    app.UseMiddleware<SwaggerBasicAuthenticationMiddleware>();
    app.MapOpenApi();
    app.UseSwaggerUI(options => 
    {
        options.SwaggerEndpoint("/openapi/v1.json", $"{settings.ApplicationName} v1");
    });
}

app.MapEndpoints();
app.Run();