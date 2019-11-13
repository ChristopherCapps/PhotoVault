﻿// Learn more about F# at http://fsharp.org

open System
open Fake.Core
open Fake.Core.CreateProcess
open FSharp.Data

let (?) (s:string) propertyName = s + propertyName

[<EntryPoint>]
let main argv =
    let results = 
        fromRawCommand "exiftool" ["/Users/ccapps/Pictures"; "-json"]
        |> redirectOutput
        |> Proc.run

    let x = "Test" ? test ? help
    printfn "Result: %A" x
    let output = JsonValue.Parse results.Result.Output
    for photo in output do        
        printfn "File: %A DTO: %A" photo?FileName photo?FileModifyDate
    0 // return an integer exit code