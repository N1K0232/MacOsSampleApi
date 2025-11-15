namespace MacOsSampleApi.BusinessLayer.Settings;

public class AppSettings
{
    public string ApplicationName { get; init; } = "MacOsSampleApi";

    public string ApplicationDescription { get; init; } = "My first web api project on MacOS";

    public string[] SupportedCultures { get; init; } = [ "en", "it" ];
}