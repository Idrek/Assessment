namespace App


module Cryptography = App.Cryptography
module FS = App.FileSystem
type IDownloable = App.Assets.IDownloable
type Path = System.IO.Path
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
                            let! _ = asset.Resume fileUri fullPath
                            ()
                        | false, false ->
                            let! _ = asset.Download fileUri fullPath
                            ()
                        | _ -> ()
        }

