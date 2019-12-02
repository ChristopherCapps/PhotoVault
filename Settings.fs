module Settings

open Newtonsoft.Json

type Repository = {
    path: string
    indexFile: string
}

let serialize settings =
    JsonConvert.SerializeObject settings