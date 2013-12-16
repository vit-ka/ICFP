namespace ICFP2009.VirtualMachine.Core

type Loader(fileName : string) =
    member private this.FileName = fileName
    member this.LoadFromFile() =
        use streamReader = new System.IO.BinaryReader(new System.IO.FileStream(this.FileName, System.IO.FileMode.Open))
        let b = streamReader.ReadBytes(12)
        System.Console.WriteLine(b.[1])
