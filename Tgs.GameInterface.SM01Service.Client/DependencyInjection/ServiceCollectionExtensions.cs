using Microsoft.Extensions.Options;
using Tgs.GameInterface.SM01Service.Client;
using Tgs.GameInterface.SM01Service.Protos;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGameInterfaceSM01ServiceClient(
        this IServiceCollection services,
        Action<GameInterfaceSM01ClientOptions, IServiceProvider> configure)
        => services
            .AddOptions<GameInterfaceSM01ClientOptions>()
            .Configure(configure)
            .Services
            .AddGrpcClient<GameInterfaceSM01Service.GameInterfaceSM01ServiceClient>((sp, configureClient) =>
            {
                var options = sp.GetRequiredService<IOptions<GameInterfaceSM01ClientOptions>>().Value;
                configureClient.Address = options.BaseUrl;
            })
            .ConfigureChannel((sp, configureChannel) =>
            {
                var handler = new SocketsHttpHandler
                {
                    PooledConnectionIdleTimeout = Timeout.InfiniteTimeSpan,
                    KeepAlivePingDelay = TimeSpan.FromSeconds(60),
                    KeepAlivePingTimeout = TimeSpan.FromSeconds(30),
                    EnableMultipleHttp2Connections = true
                };
                configureChannel.HttpHandler = handler;
            })
            .AddInterceptor<LoggingInterceptor>()
            .Services
            .AddSingleton<DiagnosticsInterceptorOptions>()
            .AddSingleton<LoggingInterceptor>()
            .AddTransient<IGameInterfaceSM01Client, GameInterfaceSM01Client>();
}
