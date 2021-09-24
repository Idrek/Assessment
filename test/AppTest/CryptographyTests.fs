module AppTest.CryptographyTests

open App.Cryptography
open Xunit

type File = System.IO.File
type Path = System.IO.Path

[<Fact>]
let ``Tests for checkMd5 function`` () =
    let checkSum : string = "edfc80bdc5c752b6a73543dbba07d3ff"
    let contents : string = 
        """monday
tuesday
wednesday
thursday
friday
saturday
sunday"""
    let fileName : string = Path.GetTempFileName ()
    File.WriteAllText(fileName, contents)
    Assert.True(checkMd5 checkSum fileName)

    