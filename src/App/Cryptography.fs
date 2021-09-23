module App.Cryptography

module Hex = App.Hexadecimal

type File = System.IO.File
type MD5 = System.Security.Cryptography.MD5

let checkMd5 (md5hash: string) (path: string) : bool =
    if File.Exists path then
        let bytes : array<byte> = File.ReadAllBytes path
        use md5 : MD5 = MD5.Create()
        let res = md5.ComputeHash bytes |> Hex.fromBytes Hex.Case.Lower
        res = md5hash
    else false
    
