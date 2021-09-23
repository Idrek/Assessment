module AppTest.HexadecimalTests

open App.Hexadecimal
open Xunit

[<Fact>]
let ``Test fromByte function`` () =
    Assert.Equal("00", fromByte Case.Upper 0uy)
    Assert.Equal("09", fromByte Case.Upper 9uy)
    Assert.Equal("0A", fromByte Case.Upper 10uy)
    Assert.Equal("0F", fromByte Case.Upper 15uy)
    Assert.Equal("FF", fromByte Case.Upper 255uy)

    