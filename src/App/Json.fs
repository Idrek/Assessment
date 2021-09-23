module App.Json

type JsonElement = System.Text.Json.JsonElement
type JsonProperty = System.Text.Json.JsonProperty
type JsonValueKind = System.Text.Json.JsonValueKind

let arrayChilds (property: JsonProperty) : Result<seq<JsonElement>, string> =
    try
        property.Value.EnumerateArray() |> Seq.cast<JsonElement> |> Ok
    with | ex -> Result.Error ex.Message

let objectChilds (item: JsonElement) : Result<seq<JsonProperty>, string> =
    try
        item.EnumerateObject() |> Seq.cast<JsonProperty> |> Ok
    with | ex -> Result.Error ex.Message

let isDir (item: JsonElement) : bool =
    match objectChilds item with
    | Error e -> false
    | Ok properties -> 
        properties |> Seq.head |> fun property -> property.Value.ValueKind = JsonValueKind.Array
        