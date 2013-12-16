namespace ICFP2009.VirtualMachine.Core

type IBlobReader = interface
    abstract member ParseStream : System.IO.StreamReader -> ParsedStream
end