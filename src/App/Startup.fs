module App.Startup

open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting

type App = App.App
type Assets = App.Assets.Assets
type IDownloable = App.Assets.IDownloable

let createHostBuilder (args: array<string>) : IHostBuilder =
    Host.CreateDefaultBuilder(args)
        .ConfigureServices(fun (context: HostBuilderContext) (services: IServiceCollection) ->
            services.AddHttpClient()
                .AddScoped<App>()
                .AddScoped<IDownloable, Assets>() |> ignore)

                