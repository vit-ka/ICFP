module UM

open System.IO

//let loadScrollFromDisk filename =

let MAX_FILE_SIZE = 1024L * 1024L * 10L

let readProgramFromFile filename =
    let fileInfo = new FileInfo(filename)
    use fileStream = fileInfo.OpenRead()
    let fileSize = fileInfo.Length
    if fileSize > MAX_FILE_SIZE then
        failwith "The size of the file \"%s\" is too large. A file can't be greater than %d bytes" filename MAX_FILE_SIZE
    let buffer = Array.create <| int fileSize <| 0uy
    fileStream.Read(buffer, 0, buffer.Length) |> ignore
    buffer

[<EntryPoint>]    
let main (args: string[]) =
    printfn "Interpretator started."

    if args.Length < 1 then
        failwith "You have to pass a parameter to the program. It should be a name of image to interpretate."

    let filename = args.[0]

    printfn "I am going to read program from the file \"%s\"" filename

    let file = new FileInfo(filename);

    if not file.Exists then
        failwith "There is no file at the given path. Are you kidding me?"

    let programBytes = readProgramFromFile <| file.FullName

    printfn "The %d bytes have been read from the file." <| programBytes.Length

    printfn "Starting file interpretation..."

    0

