namespace CPU

open Memory
open IO
open Scroll

type CPU(memory : IMemory, io : IIO) =
    
    let executionFinger = 0

    do printfn "CPU has been initialized."

    member this.Memory = memory
    member this.IO = io
    
    member this.Registers : int array = Array.zeroCreate 8

    member this.StartInterpretation() =
        printfn "Starting interpretation of scroll..."
        ()
