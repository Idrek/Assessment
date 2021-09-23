namespace App.Assets

type File = {
    Md5: string
    Name: string
    Required: bool
} 

type Tree = 
    | Dir of string * array<Tree>
    | File of File
    