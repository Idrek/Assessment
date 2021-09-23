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

[<Fact>]
let ``Tests for arrayChilds function`` () =
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
    let propsR : Result<array<JsonProperty>, string> = objectChilds root |> Result.map Seq.toArray
    let firstChildsR : Result<array<JsonElement>, string> = 
        propsR 
        |> Result.bind (fun (props: array<JsonProperty>) -> arrayChilds props.[0]) 
        |> Result.map Seq.toArray
    match firstChildsR with
    | Error e -> failwith "Invalid branch"
    | Ok elems ->
        Assert.Equal("fd632797a9adb10d068868f86a2b6951", elems.[0].GetProperty("md5").GetString())
        Assert.Equal("keno.html", elems.[0].GetProperty("name").GetString())
        Assert.True(elems.[0].GetProperty("required").GetBoolean())

[<Fact>]
let ``Tests for isDir function`` () =
    let json : string = """
        {
            "level1": [
                {
                  "md5": "fd632797a9adb10d068868f86a2b6951", 
                  "name": "keno.html", 
                  "required": true
                },
                {
                    "level2": []
                }
            ]
        }
    """
    let root : JsonElement = JsonDocument.Parse(json).RootElement
    let propsR : Result<array<JsonProperty>, string> = objectChilds root |> Result.map Seq.toArray
    let childsR : Result<array<JsonElement>, string> = 
        propsR 
        |> Result.bind (fun (props: array<JsonProperty>) -> arrayChilds props.[0]) 
        |> Result.map Seq.toArray
    match childsR with
    | Error e -> failwith "Invalid branch"
    | Ok childs ->
        Assert.False(isDir childs.[0])
        Assert.True(isDir childs.[1])

        