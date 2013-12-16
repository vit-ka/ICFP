namespace ICFP2009.VirtualMachine.Core

type IInstructionRunner = interface
    abstract member RunNextStep : unit -> unit
end

