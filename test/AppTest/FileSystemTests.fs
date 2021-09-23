module AppTest.FileSystemTests

open App.FileSystem
open Xunit

type Stack<'t> = System.Collections.Generic.Stack<'t>
type String = System.String

[<Fact>]
let ``Tests for toPath function`` () =
    let segments : Stack<string> = new Stack<String>(Array.empty)
    Assert.Equal(String.Empty, toPath segments None)
    Assert.Equal("file1", toPath segments (Some "file1"))

    let segments : Stack<string> = new Stack<String>([|"dir1"|])
    Assert.Equal("dir1/file1", toPath segments (Some "file1"))

    let segments : Stack<string> = new Stack<String>([|"dir1"; "dir2"|])
    Assert.Equal("dir1/dir2/file1", toPath segments (Some "file1"))
    Assert.Equal("dir1/dir2", toPath segments None)

    