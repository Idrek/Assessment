module AppTest.AssetsTests

open App.Assets
open Microsoft.Extensions.DependencyInjection
open System.Text.Json
open System.Threading.Tasks
open Xunit

type Thread = System.Threading.Thread
type IServiceProvider = System.IServiceProvider
type Path = System.IO.Path

let provider () : IServiceProvider =
    let services : ServiceCollection = ServiceCollection()
    services.AddHttpClient()
        .AddScoped<IDownloable, Assets>() |> ignore
    services.BuildServiceProvider() :> IServiceProvider

[<Fact>]
let ``Tests for Tree.FromJson method`` () =
    let json : string = """
        {
            "level1": [
                {
                  "md5": "fd632797a9adb10d068868f86a2b6951", 
                  "name": "keno.html", 
                  "required": true
                },
                {
                  "md5": "4262186789c88657ddeaef6acbaaa45f", 
                  "name": "roulette.html", 
                  "required": true
                },
                {
                    "level2": [
                        {
                          "md5": "2e480334d198ce3be300cbaa796d8dcf", 
                          "name": "index.html", 
                          "required": true
                        }
                    ]
                }
            ]
        }
    """
    let document : JsonDocument = JsonDocument.Parse(json)
    let treeR : Result<Tree, string> = Tree.FromJson document
    match treeR with
    | Error e -> failwith "Invalid branch"
    | Ok tree ->
        let output : Tree = 
            Dir (".", [|
                Dir ("level1", [|
                    File { 
                        Md5 = "fd632797a9adb10d068868f86a2b6951"
                        Name = "keno.html"
                        Required = true 
                    }
                    File { 
                        Md5 = "4262186789c88657ddeaef6acbaaa45f"
                        Name = "roulette.html"
                        Required = true 
                    }
                    Dir ("level2", [|
                        File { 
                            Md5 = "2e480334d198ce3be300cbaa796d8dcf"
                            Name = "index.html"
                            Required = true 
                        }
                    |])
                |])
            |])
        Assert.Equal(output, tree)

[<Fact>]
let ``Tests for Tree.ToPaths method`` () =
    let assets : Tree = 
        Dir (".", [|
                Dir ("level1", [|
                    File { 
                        Md5 = "fd632797a9adb10d068868f86a2b6951"
                        Name = "keno.html"
                        Required = true 
                    }
                    File { 
                        Md5 = "4262186789c88657ddeaef6acbaaa45f"
                        Name = "roulette.html"
                        Required = true 
                    }
                    Dir ("level2", [|
                        File { 
                            Md5 = "2e480334d198ce3be300cbaa796d8dcf"
                            Name = "index.html"
                            Required = true 
                        }
                    |])
                |])
            |])
    let result : array<string * string> = [|
        ("fd632797a9adb10d068868f86a2b6951", "./level1/keno.html")
        ("4262186789c88657ddeaef6acbaaa45f", "./level1/roulette.html")
        ("2e480334d198ce3be300cbaa796d8dcf", "./level1/level2/index.html")
    |]
    Assert.Equal<array<string * string>>(result, Tree.ToPaths assets)

[<Fact>]
let ``Tests for Assets.Download method`` () =
    let t = new Task(fun () -> WebServer.App.main Array.empty |> ignore)
    t.Start()
    // Some time for the web server to start.
    Thread.Sleep 1000
    async {
        let httpClientFactory : IHttpClientFactory = provider().GetRequiredService<IHttpClientFactory>()
        let assets : IDownloable = Assets(httpClientFactory) :> IDownloable
        let fileName : string = Path.GetTempFileName ()
        let! result = assets.Download "http://localhost:5000/partial.txt" fileName
        match result with
        | Error e -> failwithf "Invalid branch: %A" e
        | Ok () ->
            let fileData : string = File.ReadAllText fileName
            let output : string = 
                """monday
tuesday
wednesday
thursday
fri"""
            Assert.Equal(output, fileData)
            File.Delete fileName
    }

[<Fact>]
let ``Tests for Assets.Resume method`` () =
    let t = new Task(fun () -> WebServer.App.main Array.empty |> ignore)
    t.Start()
    // Some time for the web server to start.
    Thread.Sleep 1000
    async {
        let httpClientFactory : IHttpClientFactory = provider().GetRequiredService<IHttpClientFactory>()
        let assets : IDownloable = Assets(httpClientFactory) :> IDownloable
        let fileName : string = Path.GetTempFileName ()
        let! result = assets.Download "http://localhost:5000/partial.txt" fileName
        match result with
        | Error e -> failwithf "Invalid branch (download): %A" e
        | Ok () ->
            let! result = assets.Resume "http://localhost:5000/complete.txt" fileName
            match result with
            | Error e -> failwithf "Invalid branch (restart): %A" e
            | Ok () ->
                let fileData : string = File.ReadAllText fileName
                let output : string = 
                    """monday
tuesday
wednesday
thursday
friday
saturday
sunday"""
                Assert.Equal(output, fileData)
                File.Delete fileName
    }


