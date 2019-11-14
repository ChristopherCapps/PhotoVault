module Exiftool

open Fake.Core
open FSharp.Data
open FSharp.Data.JsonExtensions

[<Literal>]
let DefaultExiftoolPath = "exiftool"

[<Literal>]
let private JsonOutputFlag = "-json"

type ExiftoolPath = 
    | DefaultToolPath
    | AbsoluteToolPath of string

type ExiftoolArgs = string seq

type private ExiftoolResult = 
    | Failure of exitCode: int
    | Success of output: string * error: string

type TagCollection =
    | AllTags
    | NamedTags of string seq

let private runExiftool exiftoolPath (args:ExiftoolArgs) =
    let exiftoolPath = 
        match exiftoolPath with
        | DefaultToolPath -> DefaultExiftoolPath
        | AbsoluteToolPath path -> path

    let result = 
        CreateProcess.fromRawCommand exiftoolPath args
        |> CreateProcess.redirectOutput
        |> Proc.run

    match result.ExitCode with
    | 0 -> 
        Success(result.Result.Output, result.Result.Error)
    | error -> 
        Failure(error)

let getMediaTags tags mediaPath =
    let tagArguments =
        match tags with
        | AllTags -> System.String.Empty
        | NamedTags tags -> tags |> Seq.fold (sprintf "%s -%s") System.String.Empty

    let result = runExiftool DefaultToolPath [mediaPath; tagArguments; JsonOutputFlag]

    match result with 
    | Failure exitCode -> 