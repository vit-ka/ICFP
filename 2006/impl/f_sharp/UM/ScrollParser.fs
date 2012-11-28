namespace Scroll

type ScrollItem =
    | ConditionalMove of uint32 * uint32 * uint32
    | ArrayIndex of uint32 * uint32 * uint32
    | ArrayAmendment of uint32 * uint32 * uint32
    | Addition of uint32 * uint32 * uint32
    | Multiplication of uint32 * uint32 * uint32
    | Division of uint32 * uint32 * uint32

type IScrollParser =
    interface 
        abstract member ParseScrollItem : uint32 -> ScrollItem
    end

type ScrollParser() =
    do printfn "Scroll parser has been initialized."

    let (|StandartOperatorExtract|) (value : uint32) = (value >>> 28,
                                                        value &&& 0b00000000000000000000000111000000u,
                                                        value &&& 0b00000000000000000000000000111000u,
                                                        value &&& 0b00000000000000000000000000000111u)

    interface IScrollParser with
        member this.ParseScrollItem(item) =
            match item with
            | StandartOperatorExtract (0u, a, b, c) -> ConditionalMove(a, b, c)
            | StandartOperatorExtract (1u, a, b, c) -> ArrayIndex(a, b, c)
            | StandartOperatorExtract (2u, a, b, c) -> ArrayAmendment(a, b, c)
            | StandartOperatorExtract (3u, a, b, c) -> Addition(a, b, c)
            | StandartOperatorExtract (4u, a, b, c) -> Multiplication(a, b, c)
            | StandartOperatorExtract (5u, a, b, c) -> Division(a, b, c)
            | _ -> failwithf "Incorrect operator received %x" item