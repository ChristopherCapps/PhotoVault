// Learn more about F# at http://fsharp.org

open System
open Fake.Core
open Fake.Core.CreateProcess
open FSharp.Data
open FSharp.Data.JsonExtensions

open Exiftool

[<EntryPoint>]
let main argv =
    let results = getMediaTags AllTags "/Users/capps/Downloads/Photos"
    match results with
    | Failure (exitCode, errorMessage) ->
        printfn "Error: %d; %s" exitCode errorMessage
        exitCode
    | Success json -> 
        for photo in json do        
            printfn "File: %A DTO: %A" photo?FileName photo?FileModifyDate
        0
