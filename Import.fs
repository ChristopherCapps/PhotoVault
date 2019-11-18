module Import

open Exiftool.Media

[<Literal>]
let TagDateTimeOriginal = "DateTimeOriginal"
[<Literal>]
let TagCreateDate = "CreateDate"
[<Literal>]
let TagCreationDate = "CreationDate"
[<Literal>]
let TagFileModifyDate = "FileModifyDate"

let timestampTags = [ 
    TagDateTimeOriginal
    TagCreateDate
    TagCreationDate
    TagFileModifyDate 
]

let getPreferredTimestampTag media =
    match media.mediaFormat with
    | Image _ -> TagDateTimeOriginal
    | Video ext -> 
        match ext with
        | "avi" -> TagDateTimeOriginal
        | "mov" | "mp4" -> TagCreateDate
        | "mpg" -> TagFileModifyDate
        | _ -> failwithf "Unrecognized media type: %A" media.sourceFile
    | Unknown ext -> failwithf "Unrecognized file type: %A" media.sourceFile

let private calculateMD5Hash media =
    use md5 = System.Security.Cryptography.MD5.Create()
    use stream = System.IO.File.OpenRead(media.sourceFile.FullName)
    System.BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", System.String.Empty);

