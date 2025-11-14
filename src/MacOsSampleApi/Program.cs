using System.Net;
using TinyHelpers.AspNetCore.Extensions;
using MacOsSampleApi.BusinessLayer.Settings;
using TinyHelpers.AspNetCore.OpenApi;
using MacOsSampleApi.Swagger;

var builder = WebApplication.CreateBuilder(args);

var settings = builder.Services.ConfigureAndGet<AppSettings>(builder.Configuration, nameof(AppSettings)) ?? new AppSettings();
var swagger = builder.Services.ConfigureAndGet<SwaggerSettings>(builder.Configuration, nameof(SwaggerSettings)) ?? new SwaggerSettings();

builder.Services.AddHttpContextAccessor();
builder.Services.AddRequestLocalization(settings.SupportedCultures);

builder.Services.AddDefaultProblemDetails();
builder.Services.AddDefaultExceptionHandler();

if(swagger.IsEnabled)
{
    builder.Services.AddOpenApi(options => 
    {
        options.RemoveServerList();
        options.AddAcceptLanguageHeader();
        options.AddDefaultProblemDetailsResponse();
    });
}

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

app.MapPost("/api/ping", () => TypedResults.Ok(new { message = "pong"}));

app.Run();
