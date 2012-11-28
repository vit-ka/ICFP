namespace CPU

open Memory
open IO
open Scroll

type CPU(memory : IMemory, io : IIO, parser : IScrollParser) =
    
    let mutable executionFinger = 0u

    do printfn "CPU has been initialized."

    member private this.Memory = memory
    member private this.IO = io
    member private this.Parser = parser
    
    member private this.Registers : uint32 array = Array.zeroCreate 8

    // Process operator
    member private this.doOperator operator =
        match operator with
        | ConditionalMove(a,b,c) -> if this.Registers.[int c] <> 0u then this.Registers.[int a] <- this.Registers.[int b]
        | ArrayIndex(a,b,c) -> this.Registers.[int a] <- this.Memory.GetValueFromBlock (this.Registers.[int b], this.Registers.[int c])
        | ArrayAmendment(a,b,c) ->  this.Memory.WriteValueToBlock (this.Registers.[int a], this.Registers.[int b], this.Registers.[int c])
        | Addition(a,b,c) ->  this.Registers.[int a] <- (this.Registers.[int b] + this.Registers.[int c]) % 0b00000000000000000000000100000000u
        | Multiplication(a,b,c) ->  this.Registers.[int a] <- (this.Registers.[int b] * this.Registers.[int c]) % 0b00000000000000000000000100000000u
        | Division(a,b,c) ->  this.Registers.[int a] <- (this.Registers.[int b] / this.Registers.[int c])

    member this.StartInterpretation() =
        printfn "Starting interpretation of scroll..."
        
        while true do
            let operatorInt = this.Memory.GetValueFromBlock(0u, executionFinger)
            let operatorToDo = parser.ParseScrollItem operatorInt
            executionFinger <- executionFinger + 1u

            this.doOperator operatorToDo

            printfn "Execution finger: %d. %x. Next operator to do is %A" executionFinger operatorInt operatorToDo

        ()
