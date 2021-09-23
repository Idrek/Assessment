module App.Http

module C = HttpFs.Client
module MRequest = C.Request

type TRequest = C.Request

let createRequest (uri: string) : TRequest =
    MRequest.createUrl (C.HttpMethod.Get) uri
    