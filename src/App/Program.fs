module App.Program

open CommandLine
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
    printfn "Hello World from F#!"
    0 // return an integer exit code
