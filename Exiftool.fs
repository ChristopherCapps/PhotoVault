module Exiftool

open Fake.Core
open Fake.Core.CreateProcess
open FSharp.Data
open FSharp.Data.JsonExtensions


let results = 
    fromRawCommand "exiftool" ["/Users/capps/Downloads/Photos"; "-json"]
    |> redirectOutput
    |> Proc.run

let x= results.