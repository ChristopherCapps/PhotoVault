// Learn more about F# at http://fsharp.org

open Exiftool.Media
open Settings

let printResults results =
    match results with
    | MediaQueryFailure (exitCode, errorMessage) ->
        printfn "Error: %d; %s" exitCode errorMessage
    | MediaQuerySuccess json -> 
        for result in json do        
            printfn "%A" result

[<EntryPoint>]
let main argv =
    let results = CLI.parser.Parse argv
    printfn "%A" (CLI.parser.PrintUsage())
    printfn "%A" results
    let settings = { 
        path = "/tmp/"
        indexFile = "/tmp/repository.idx" 
    }
    //printfn "%s" (serialize [settings; settings; settings])
    // if Array.length argv < 1 then failwith "A path or filename must be provided."
    // queryMediaTags (NamedTags (Array.tail argv)) argv.[0]
    // |> printResults
    0