namespace Scroll

type ScrollItem =
    | A

type IScrollParser =
    interface 
        abstract member ParseScrollItem : int -> ScrollItem
    end

type ScrollParser() =
    do printfn "Scroll parser has been initialized."

    interface IScrollParser with
        member this.ParseScrollItem(item) =
            A