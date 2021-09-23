module App.FileSystem

type Directory = System.IO.Directory
type DirectoryInfo = System.IO.DirectoryInfo
type Path = System.IO.Path
type Stack<'t> = System.Collections.Generic.Stack<'t>

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

let toPath (segments: Stack<string>) (fileNameO: Option<string>) : string =
    let fileSegment : array<string> =
        match fileNameO with
        | None -> Array.empty
        | Some fileName -> Array.singleton fileName
    let path : array<string> = Array.append (segments.ToArray() |> Array.rev) fileSegment
    Path.Combine(path)
    