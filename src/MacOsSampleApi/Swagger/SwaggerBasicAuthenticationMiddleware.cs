using System.Net.NetworkInformation;
using System.Xml.Linq;
using System.Net.Http.Headers;
using System.Text;
using MacOsSampleApi.BusinessLayer.Settings;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using TinyHelpers.Extensions;

namespace MacOsSampleApi.Swagger;
    
public class SwaggerBasicAuthenticationMiddleware(RequestDelegate next, IOptions<SwaggerSettings> swaggerOptions)
{
    private readonly SwaggerSettings swagger = swaggerOptions.Value;

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.StartsWithSegments("/swagger") && swagger.UserName.HasValue() && swagger.Password.HasValue())
        {
            string? authenticationHeader = context.Request.Headers[HeaderNames.Authorization];
            if(authenticationHeader?.StartsWith("Basic ") ?? false)
            {
                var header = AuthenticationHeaderValue.Parse(authenticationHeader);
                var credentials = Encoding.UTF8.GetString(Convert.FromBase64String(header.Parameter!)).Split(':', count: 2);

                var userName = credentials.ElementAtOrDefault(0);
                var password = credentials.ElementAtOrDefault(1);

                if (userName == swagger.UserName && password == swagger.Password)
                {
                    await next.Invoke(context).ConfigureAwait(false);
                    return;
                }
            }

            context.Response.Headers.WWWAuthenticate = new StringValues("Basic");
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        }
        else
        {
            await next.Invoke(context).ConfigureAwait(false);
        }
    }
}