using System.Reflection;
using Tgs.GameInterface.SM01Service.Core;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSM01ServiceCore(
        this IServiceCollection services)
        => services
            .AddHttpClient<IGameApiClient, GameApiClient>()
            .Services
            .RegisterAllServiceUtilityTypes(
                Assembly.GetExecutingAssembly(),
                ServiceLifetime.Transient);
}
