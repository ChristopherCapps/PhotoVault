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

    type TagCollection =
        | AllTags
        | NamedTags of string seq
    
    type MediaResult = {
        fileName: string
        sourceFile: System.IO.FileInfo
        tags: Map<string,string>
    }

    type MediaQueryResult =
        | MediaQueryFailure of exitCode: int * errorMessage: string
        | MediaQuerySuccess of MediaResult seq

    let private run args mediaPath = 
        let result = 
            seq { yield mediaPath; yield JsonOutputFlag; yield! args }
            |> run 

        match result with 
        | ExiftoolFailure (exitCode, errorMessage) -> 
            MediaQueryFailure (exitCode, errorMessage)
        | ExiftoolSuccess (out, err) -> 
            let results = JsonExtensions.Properties(JsonValue.Parse out)
            // we need to treat the out json as a sequence of objects!
            let record = 
                results 
                |> Seq.fold (fun (map:Map<string,string>) (property, value) -> 
                    map.Add(property, JsonExtensions.AsString(value))) Map.empty
            MediaQuerySuccess (JsonValue.Parse out)

    let queryMediaTags tags mediaPath =
        let tagArguments =
            match tags with
            | AllTags -> []
            | NamedTags tags -> Seq.toList tags |> List.map ((+) "-")

        run tagArguments mediaPath