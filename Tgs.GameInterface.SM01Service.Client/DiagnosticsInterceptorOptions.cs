using System.Diagnostics;

namespace Tgs.GameInterface.SM01Service.Client;

public class DiagnosticsInterceptorOptions
{
    public ActivitySource InterceptorActivitySource { get; set; } =
        new ActivitySource($"Tgs.GameInterface.SM01Service.Client.DiagnosticsInterceptor");
}
