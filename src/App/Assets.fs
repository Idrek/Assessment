namespace App.Assets

module FS = App.FileSystem
module Json = App.Json

type JsonDocument = System.Text.Json.JsonDocument
type JsonElement = System.Text.Json.JsonElement
type JsonProperty = System.Text.Json.JsonProperty
type Queue<'t> = System.Collections.Generic.Queue<'t>
type Stack<'t> = System.Collections.Generic.Stack<'t>
type String = System.String

type FileAttributes = {
    Md5: string
    Name: string
    Required: bool
} 

type Tree = 
    | Dir of string * array<Tree>
    | File of FileAttributes
with
    static member FromJson (assets: JsonDocument) : Result<Tree, string> =
            let mutable isError : bool = false
            let rec loop (dir: JsonProperty) : array<Tree> =
                let objects : seq<JsonElement> = 
                    match Json.arrayChilds dir with
                    | Error e -> isError <- true; Seq.empty
                    | Ok objects -> objects
                if Seq.isEmpty objects
                then Array.empty
                else
                    [|
                        for object in objects do
                            if Json.isDir object
                            then
                                let subDirs : seq<JsonProperty> = 
                                    match Json.objectChilds object with
                                    | Error e -> isError <- true; Seq.empty
                                    | Ok subDirs -> subDirs
                                for subDir in subDirs do yield (Dir (subDir.Name, loop subDir))
                            else
                                match object.TryGetProperty("md5"), 
                                      object.TryGetProperty("name"), 
                                      object.TryGetProperty("required") with
                                | (true, md5), (true, name), (true, required) ->
                                    yield File (
                                        let f : FileAttributes = {
                                            Md5 = md5.GetString()
                                            Name = name.GetString()
                                            Required = required.GetBoolean()
                                        }
                                        f)
                                | _ -> isError <- true; ()
                    |] 
            Json.objectChilds assets.RootElement
            |> Result.bind (fun topDirs ->
                let assets : array<Tree> = [| for dir in topDirs do yield Dir(dir.Name, loop dir) |]
                if isError 
                then Result.Error "Error: Parse JSON to Assets type"
                else Ok <| Dir (".", assets))

    static member ToPaths (assets: Tree) : array<string * string> =
            let pathSegments : Stack<string> = new Stack<string>()
            let filePaths : Queue<string * string> = new Queue<string * string>()
            let rec loop assets =
                match assets with
                | Dir (dir, childs) ->
                    pathSegments.Push dir
                    if Array.isEmpty childs
                    then 
                        let dirPath : string = FS.toPath pathSegments None
                        filePaths.Enqueue (String.Empty, dirPath)
                    else ()
                    childs |> Array.iter loop
                    pathSegments.Pop() |> ignore
                | File { Md5 = md5; Name = name; Required = true } ->
                    let filePath : string = FS.toPath pathSegments (Some name)
                    filePaths.Enqueue (md5, filePath)
                | _ -> ()
            loop assets
            filePaths.ToArray()


