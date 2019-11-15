module Exiftool

open Fake.Core

[<Literal>]
let ExiftoolPathEnvVar = "EXIFTOOL_PATH"
[<Literal>]
let DefaultExiftoolExePath = "exiftool"

[<Literal>]
let private JsonOutputFlag = "-json"

let private exePath = 
    Environment.environVarOrDefault ExiftoolPathEnvVar DefaultExiftoolExePath

type Args = string seq

type private ExiftoolResult = 
    | ExiftoolFailure of exitCode: int * errorMessage: string
    | ExiftoolSuccess of output: string * error: string

let private run (args:Args) =
    let result = 
        CreateProcess.fromRawCommand exePath args
        |> CreateProcess.redirectOutput
        |> Proc.run

    match result.ExitCode with
    | 0 -> 
        ExiftoolSuccess(result.Result.Output, result.Result.Error)
    | error -> 
        ExiftoolFailure(error, result.Result.Error)

module MediaQuery =
    open FSharp.Data
    open FSharp.Data.Runtime

    type TagCollection =
        | AllTags
        | NamedTags of string seq
    
    type Tags = Map<string,string>

    [<Literal>]
    let TagSourceFile = "SourceFile"

    type MediaResult = {
        sourceFile: System.IO.FileInfo
        tags: Tags
    }

    type MediaQueryResult =
        | MediaQueryFailure of exitCode: int * errorMessage: string
        | MediaQuerySuccess of MediaResult seq

    let private jsonValueToString jsonValue =
        let useNoneForNullOrEmpty = true        
        jsonValue 
        |> JsonConversions.AsString useNoneForNullOrEmpty System.Globalization.CultureInfo.InvariantCulture
        |> function
            | Some str -> str
            | None -> System.String.Empty

    let private jsonValueToTags jsonValue =
        jsonValue
        |> JsonExtensions.Properties
        |> Seq.fold (fun (tags:Tags) (tagName, value) -> 
            tags |> Map.add tagName (value |> jsonValueToString)) Map.empty

    let private tagsToMediaResult (tags: Tags) = {
        sourceFile = tags.[TagSourceFile] |> System.IO.FileInfo
        tags = tags
    }                

    let private run args mediaPath = 
        let result = 
            seq { yield mediaPath; yield JsonOutputFlag; yield! args }
            |> run 

        match result with 
        | ExiftoolFailure (exitCode, errorMessage) -> 
            MediaQueryFailure (exitCode, errorMessage)
        | ExiftoolSuccess (out, err) -> 
            out
            |> JsonValue.Parse
            |> JsonExtensions.AsArray
            |> Seq.map (jsonValueToTags >> tagsToMediaResult)
            |> MediaQuerySuccess

    let queryMediaTags tags mediaPath =
        let tagArguments =
            match tags with
            | AllTags -> []
            | NamedTags tags -> Seq.toList tags |> List.map ((+) "-")

        run tagArguments mediaPath