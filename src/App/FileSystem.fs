module App.FileSystem

type Directory = System.IO.Directory
type DirectoryInfo = System.IO.DirectoryInfo
type Path = System.IO.Path

let createDir (path: string) : Option<string> =
    try Directory.CreateDirectory path |> ignore; None
    with | ex -> Some ex.Message

let ensureDir (path: string) : Option<string> =
    if Directory.Exists path
    then None
    else createDir path

let buildFullPath (baseDir: string) (path: string) : Result<string, string> =
    try
        Ok <| DirectoryInfo(Path.Join(baseDir, path)).FullName
    with | ex -> Result.Error ex.Message
    