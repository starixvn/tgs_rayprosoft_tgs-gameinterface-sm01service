using System.Diagnostics;

namespace Tgs.GameInterface.SM01Service;

public class DiagnosticsInterceptorOptions
{
    public ActivitySource InterceptorActivitySource { get; set; } =
        new ActivitySource($"Tgs.GameInterface.SM01Service.DiagnosticsInterceptor");
}
