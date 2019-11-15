// Learn more about F# at http://fsharp.org

open FSharp.Data
open FSharp.Data.JsonExtensions
open Exiftool.MediaQuery

let printResults results =
    match results with
    | MediaQueryFailure (exitCode, errorMessage) ->
        printfn "Error: %d; %s" exitCode errorMessage
    | MediaQuerySuccess json -> 
        for result in json do        
            printfn "%A" result

[<EntryPoint>]
let main argv =    
    queryMediaTags AllTags "Testing/data"
    |> printResults
    queryMediaTags (NamedTags ["FileName"; "FileModifyDate"]) "Testing/data"
    |> printResults
    0