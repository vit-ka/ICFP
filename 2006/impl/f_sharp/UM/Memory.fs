namespace Memory

open System.Collections.Generic

type IMemory =
    interface
        abstract member GetValueFromBlock : int * int -> int
        abstract member WriteValueToBlock : int * int * int -> unit
    end

type MemoryManager() =
    
    do printfn "Memory manager has been initialized."

    member this.Platters : Dictionary<int, int array> = new Dictionary<_,_>()

    interface IMemory with
        member this.GetValueFromBlock(arrayAddress, address) =
            0
        member this.WriteValueToBlock(arrayAddress, address, value) =
            ()

    member private this.convertByteArrayToIntArray data =
        let rec zipDataToTuplesOfFourBytes ar acc =
            match ar with
            | [] -> acc
            | [a] -> (a, 0uy, 0uy, 0uy) :: acc
            | [a; b] -> (a, b, 0uy, 0uy) :: acc 
            | [a; b; c] -> (a, b, c, 0uy) :: acc
            | [a; b; c; d] -> (a, b, c, d) :: acc
            | a :: b :: c :: d :: xs -> zipDataToTuplesOfFourBytes xs ((a, b, c, d) :: acc)

        let mergeTuple (a : byte, b : byte, c : byte, d : byte) : int =
            int a <<< 24 ||| int b <<< 16 ||| int c <<< 8 ||| int d

        let rec mergeTuplesOfFourBytesToInt ar acc =
            match ar with
            | [] -> acc
            | [x] -> mergeTuple x :: acc
            | x :: xs -> mergeTuplesOfFourBytesToInt xs (mergeTuple x :: acc)

        let zippedTuples = zipDataToTuplesOfFourBytes <| Array.toList data <| []

        []
        |> mergeTuplesOfFourBytesToInt zippedTuples
        |> List.toArray


    member this.LoadScrollToZeroArray(data : byte array) =
        let intArray = this.convertByteArrayToIntArray data

        if not <| this.Platters.ContainsKey(0) then
            this.Platters.Add(0, null)

        printfn "Scroll with %d commands has been loaded to 0 array" <| intArray.Length

        this.Platters.[0] <- intArray


    