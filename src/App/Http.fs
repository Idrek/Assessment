module App.Http

open Hopac

module C = HttpFs.Client
module MRequest = C.Request
module MResponse = C.Response

type TRequest = C.Request

let createRequest (uri: string) : TRequest =
    MRequest.createUrl (C.HttpMethod.Get) uri

let runRequest (request: TRequest) : Async<Result<string, string>> =
    job {
        try
            use! response = C.getResponse request
            let! body = MResponse.readBodyAsString response
            return Ok body
        with 
            | ex -> return Result.Error ex.Message
    } |> Job.toAsync

let download (uri: string) : Async<Result<string, string>> =
    uri |> createRequest |> runRequest

    