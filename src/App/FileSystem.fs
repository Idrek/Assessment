module App.FileSystem

type Directory = System.IO.Directory

let createDir (path: string) : Option<string> =
    try Directory.CreateDirectory path |> ignore; None
    with | ex -> Some ex.Message
    