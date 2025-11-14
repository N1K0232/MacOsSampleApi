using System.Runtime.Intrinsics.X86;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MacOsSampleApi.BusinessLayer.Settings;
    
public class SwaggerSettings
{
    public bool IsEnabled { get; init; } = true;

    public string? UserName { get; init; }

    public string? Password { get; init; }
}