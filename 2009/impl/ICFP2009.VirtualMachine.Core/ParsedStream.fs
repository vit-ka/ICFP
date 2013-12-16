namespace ICFP2009.VirtualMachine.Core

type ParsedStream() =
    member this.Instructions : int32 list = []
    member this.MemoryData : double list = []

    member this.PushToInstuctions (newInstuction : int32) =
        this.Instructions = this.Instructions @ [newInstuction]

    member this.PushToMemoryData (newData : double) =
        this.MemoryData = this.MemoryData @ [newData]


