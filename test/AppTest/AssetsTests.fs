module AppTest.AssetsTests

open App.Assets
open System.Text.Json
open Xunit

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

