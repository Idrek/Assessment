module AppTest.JsonTests

open App.Json
open System.Text.Json
open Xunit

[<Fact>]
let ``Tests for objectChilds function`` () =
    let json : string = """
        {
            "en_GB": [
                {
                  "md5": "fd632797a9adb10d068868f86a2b6951", 
                  "name": "keno.html", 
                  "required": true
                }
            ],
            "skin": "black"
        }
    """
    let root : JsonElement = JsonDocument.Parse(json).RootElement
    match objectChilds root |> Result.map Seq.toArray with
    | Error e -> failwith "Invalid branch"
    | Ok props ->
        Assert.Equal("en_GB", props.[0].Name)
        Assert.Equal(JsonValueKind.Array, props.[0].Value.ValueKind)
        Assert.Equal("skin", props.[1].Name)
        Assert.Equal(JsonValueKind.String, props.[1].Value.ValueKind)
        Assert.Equal("black", props.[1].Value.GetString())

        