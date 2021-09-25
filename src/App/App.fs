namespace App

open CommandLine

module Cryptography = App.Cryptography
module FS = App.FileSystem
module Http = App.Http
module Json = App.Json

type File = System.IO.File
type IDownloable = App.Assets.IDownloable
type JsonDocument = System.Text.Json.JsonDocument
type Path = System.IO.Path
type Tree = App.Assets.Tree
type Uri = System.Uri

type Options = {
    [<Option("uri", Required=true, HelpText="Uri from where download files")>]
    Uri: string
    [<Option("dir", Required=true, HelpText="Directory to save files")>]
    Dir: string
}

type App (asset: IDownloable) =

    let downloadFiles (baseDir: string) (baseUri: string) (paths: array<string * string>) =
        async {
            for (md5hash, path) in paths do
                match md5hash, FS.buildFullPath baseDir path with
                | _, Error message -> failwith message
                | md5hash, Ok fullPath when String.isEmpty md5hash ->
                    FS.ensureDir fullPath |> ignore
                | md5hash, Ok fullPath ->
                    let dir : string = Path.GetDirectoryName fullPath
                    match FS.ensureDir dir with
                    | Some errorMessage -> printfn "Problem with %A directory: %A" dir errorMessage
                    | None -> 
                        let fileUri : string = Path.Join(baseUri, path)
                        match Cryptography.checkMd5 md5hash fullPath, File.Exists fullPath with
                        | false, true ->
                            printfn "Resuming file %A ..." fullPath
                            let! _ = asset.Resume fileUri fullPath
                            ()
                        | false, false ->
                            printfn "Downloading file %A ..." fullPath 
                            let! _ = asset.Download fileUri fullPath
                            ()
                        | _ -> ()
        }

    member this.Run (options: Options) =
        async {
            let { Uri = baseUri; Dir = baseDir } : Options = options
            let assetsFileName : string = "assetsTree.json"
            let assetsUri : string = Path.Join(baseUri, assetsFileName)
            let! (assetsStructure : Result<string, string>) = Http.download assetsUri
            let (assetsR : Result<JsonDocument, string>) = Result.bind Json.ofString assetsStructure
            match assetsR with
            | Error message -> failwith message
            | Ok assets ->
                let pathsR : Result<array<string * string>, string> = 
                    Tree.FromJson assets |> Result.map Tree.ToPaths
                match pathsR with
                | Error message -> failwith message
                | Ok paths -> do! downloadFiles baseDir baseUri paths
        }

