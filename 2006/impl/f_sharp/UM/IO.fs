namespace IO

type IIO =
    interface
        abstract member WriteToEnvironment : char -> unit
        abstract member ReadFromEnvironment: unit -> char
    end

type IOManager() =
    
    do printfn "IO manager has been initialized."

    interface IIO with
        member this.WriteToEnvironment(value) =
            ()
        member this.ReadFromEnvironment() =
            ' '