namespace Memory

open System.Collections.Generic

type IMemory =
    interface
        abstract member GetValueFromBlock : uint32 * uint32 -> uint32
        abstract member WriteValueToBlock : uint32 * uint32 * uint32 -> unit
    end

type MemoryManager() =
    
    do printfn "Memory manager has been initialized."

    member private this.Platters : Dictionary<uint32, uint32 array> = new Dictionary<_,_>()

    interface IMemory with
        member this.GetValueFromBlock(arrayAddress, address) =
            if not <| this.Platters.ContainsKey(arrayAddress) then
                failwithf "There is no array at address %d" arrayAddress
            if this.Platters.[arrayAddress].Length <= int address then
                failwithf "There is no element %d in the array %d" address arrayAddress

            this.Platters.[arrayAddress].[int address]

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

        let mergeTuple (a : byte, b : byte, c : byte, d : byte) : uint32 =
            (uint32 a <<< 24) ||| (uint32 b <<< 16) ||| (uint32 c <<< 8) ||| (uint32 d)

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

        if not <| this.Platters.ContainsKey(0u) then
            this.Platters.Add(0u, null)

        this.Platters.[0u] <- intArray

        printfn "Scroll with %d commands has been loaded to 0 array" <| intArray.Length

    