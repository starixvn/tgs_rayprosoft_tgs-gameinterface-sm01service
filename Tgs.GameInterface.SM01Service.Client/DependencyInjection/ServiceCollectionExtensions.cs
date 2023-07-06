using Grpc.Core;
using Grpc.Net.Client.Balancer;
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
                configureChannel.Credentials = ChannelCredentials.Insecure;
            })
            .AddInterceptor<LoggingInterceptor>()
            .Services
            //負載平衡時，效能很重要。 藉由快取位址，解析位址的延遲會從 gRPC 呼叫中排除。 進行第一個 gRPC 呼叫時，將會叫用解析程式，而後續的呼叫會使用快取。
            .AddSingleton<ResolverFactory>(sp => new DnsResolverFactory(refreshInterval: TimeSpan.FromSeconds(30)))
            .AddSingleton<DiagnosticsInterceptorOptions>()
            .AddSingleton<LoggingInterceptor>()
            .AddTransient<IGameInterfaceSM01Client, GameInterfaceSM01Client>();
}
