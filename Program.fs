// Learn more about F# at http://fsharp.org

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
    if Array.length argv < 1 then failwith "A path or filename must be provided."
    queryMediaTags (NamedTags (Array.tail argv)) argv.[0]
    |> printResults
    0