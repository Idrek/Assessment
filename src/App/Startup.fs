module App.Startup

open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging

type App = App.App
type Assets = App.Assets.Assets
type IDownloable = App.Assets.IDownloable

let createHostBuilder (args: array<string>) : IHostBuilder =
    Host.CreateDefaultBuilder(args)
        .ConfigureServices(fun (context: HostBuilderContext) (services: IServiceCollection) ->
            services.AddHttpClient()
                .AddScoped<App>()
                .AddScoped<IDownloable, Assets>() |> ignore)
        .ConfigureLogging(fun (context: HostBuilderContext) (logging: ILoggingBuilder) -> 
            logging.ClearProviders() |> ignore)

                