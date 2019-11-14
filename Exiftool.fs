module Exiftool

open Fake.Core
open FSharp.Data
open FSharp.Data.JsonExtensions

[<Literal>]
let EnvExiftoolPath = "EXIFTOOL_PATH"
[<Literal>]
let DefaultExiftoolPath = "exiftool"

[<Literal>]
let private JsonOutputFlag = "-json"

let private ExiftoolPath = 
    Environment.environVarOrDefault EnvExiftoolPath DefaultExiftoolPath

type ExiftoolArgs = string seq

type private ExiftoolResult = 
    | Failure of exitCode: int * errorMessage: string
    | Success of output: string * error: string

type TagCollection =
    | AllTags
    | NamedTags of string seq

let private runExiftool (args:ExiftoolArgs) =
    let result = 
        CreateProcess.fromRawCommand ExiftoolPath args
        |> CreateProcess.redirectOutput
        |> Proc.run

    match result.ExitCode with
    | 0 -> 
        Success(result.Result.Output, result.Result.Error)
    | error -> 
        Failure(error, result.Result.Error)

type MediaQueryResult =
    | Failure of exitCode: int * errorMessage: string
    | Success of JsonValue

let private mediaQuery args mediaPath = 
    seq { yield mediaPath; yield JsonOutputFlag; yield! args }
    |> runExiftool 

let getMediaTags tags mediaPath =
    let tagArguments =
        match tags with
        | AllTags -> []
        | NamedTags tags -> Seq.toList tags

    let result = runExiftool (List.append [mediaPath; JsonOutputFlag] tagArguments)

    match result with 
    | ExiftoolResult.Failure (exitCode, errorMessage) -> Failure (exitCode, errorMessage)
    | ExiftoolResult.Success (out, err) ->  Success (JsonValue.Parse out)