module App.Program

open CommandLine
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting

module Startup = App.Startup

type CLIResult = 
    | Success = 0
    | CommandLineParseError = 1
    | CommandLineNotParsed = 2

let parseCommandLine (argv: array<string>) : Result<Parsed<Options>, CLIResult> =
    let commandLine = CommandLine.Parser.Default.ParseArguments<Options>(argv)
    match commandLine with
    | :? CommandLine.NotParsed<Options> -> Result.Error CLIResult.CommandLineNotParsed
    | :? CommandLine.Parsed<Options> as parsed -> Ok parsed
    | _ -> Result.Error CLIResult.CommandLineParseError

[<EntryPoint>]
let main argv =
    async {
        use host : IHost = Startup.createHostBuilder(argv).Build()
        let scope : IServiceScope = host.Services.CreateScope()
        let app : App = scope.ServiceProvider.GetRequiredService<App>()
        match parseCommandLine argv with
        | Error e -> return int e
        | Ok parsed -> 
            do! app.Run(parsed.Value)
            return int CLIResult.Success
    } |> Async.RunSynchronously

