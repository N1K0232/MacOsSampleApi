var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var app = builder.Build();
app.UseHttpsRedirection();

app.MapOpenApi();
app.UseSwaggerUI(options => 
{
    options.SwaggerEndpoint("/openapi/v1.json", "MacOsSampleApi v1");
});

app.MapPost("/api/ping", () => TypedResults.Ok(new { message = "pong"}));

app.Run();
