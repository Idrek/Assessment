module App.Hexadecimal

type String = System.String

type Case = Lower | Upper

let fromByte (case: Case) (b: byte) : string =
    let hexFormat : string =
        match case with
        | Lower -> "{0:x2}"
        | Upper -> "{0:X2}"
    String.Format(hexFormat, b)
