function Cpu(memory, inputProcessor) {
    this.operationFinger = 0;
    this.memory = memory;
    this.inputProcessor = inputProcessor;
    this.register = [0, 0, 0, 0, 0, 0, 0, 0];

    this.debug = "";
    this.buffer = "";
    this.lastWriteToBufferCounter = 0;
    this.counter = 0;
    this.halted = false;
    this.is_waiting_for_input = false;
    this.intervalId = -1;
}

function continueInterpretation(cpu) {

    //console.log("At continue interpretation " + cpu.operationFinger);

    var counterAtStart = cpu.counter;
    while (cpu.operationFinger < cpu.memory.getArrayLength(0) && cpu.counter - counterAtStart < 10000) {
        var operation = cpu.memory.getArrayElem(0, cpu.operationFinger);

        ++cpu.operationFinger;
        ++cpu.counter;

        var operationType = (operation & 0xf0000000) >>> 28;
        var a = (operation & 0x000001c0) >>> 6;
        var b = (operation & 0x00000038) >>> 3;
        var c = (operation & 0x00000007);

        if (cpu.buffer != "" && (operationType == 0x0b || (operationType != 0x0a && cpu.counter - cpu.lastWriteToBufferCounter > 10))) {
            write_to_um_console_inside_machine(cpu.buffer);
            cpu.buffer = "";
        }

//        debug += "0x" + cpu.operationFinger.toString(16)
//            + ": 0x" + (operation + 0x100000000).toString(16).substr(-8)
//            + " as 0x" + operationType.toString(16)
//            + " a=" + a
//            + " b=" + b
//            + " c=" + c
//            + "["
//            + "0x" + (cpu.register[0] + 0x100000000).toString(16).substr(-8)
//            + ",0x" + (cpu.register[1] + 0x100000000).toString(16).substr(-8)
//            + ",0x" + (cpu.register[2] + 0x100000000).toString(16).substr(-8)
//            + ",0x" + (cpu.register[3] + 0x100000000).toString(16).substr(-8)
//            + ",0x" + (cpu.register[4] + 0x100000000).toString(16).substr(-8)
//            + ",0x" + (cpu.register[5] + 0x100000000).toString(16).substr(-8)
//            + ",0x" + (cpu.register[6] + 0x100000000).toString(16).substr(-8)
//            + ",0x" + (cpu.register[7] + 0x100000000).toString(16).substr(-8)
//            + "]\n";

//        console.log("========================================================================");
//        console.log("Operation finger points at " + cpu.operationFinger.toString(16));
//        console.log("Operation is 0x" + (operation + 0x100000000).toString(16).substr(-8));
//        console.log("Operation type is 0x" + operationType.toString(16));
//
//        if (operationType != 0x0d) {
//            console.log("Operation arguments are: a = 0x" + a + ', b = 0x' + b + ', c = 0x' + c);
//            console.log("Registers before operation were: a = 0x" + cpu.register[a].toString(16)
//                + ', b = 0x' + cpu.register[b].toString(16)
//                + ', c = 0x' + cpu.register[c].toString(16));
//        }

        switch (operationType) {
        case 0x00:
            if (cpu.register[c] != 0)
                cpu.register[a] = cpu.register[b];
            break;
        case 0x01:
            cpu.register[a] = cpu.memory.getArrayElem(cpu.register[b], cpu.register[c]);
            break;
        case 0x02:
            cpu.memory.setArrayElem(cpu.register[a], cpu.register[b], cpu.register[c]);
            break;
        case 0x03:
            cpu.register[a] = ((cpu.register[b] + cpu.register[c]) & 0xffffffff) >>> 0;
            break;
        case 0x04:
            cpu.register[a] = ((cpu.register[b] * cpu.register[c]) & 0xffffffff) >>> 0;
            break;
        case 0x05:
            cpu.register[a] = (((cpu.register[b] >>> 0) / (cpu.register[c] >>> 0)) & 0xffffffff) >>> 0;
            break;
        case 0x06:
            cpu.register[a] = ((~(cpu.register[b] & cpu.register[c])) & 0xffffffff) >>> 0;
            break;
        case 0x07:
            write_to_um_console("INFO: Halt command has been received.");
            write_to_um_console(cpu.debug);
            cpu.halted = true;
            return;
        case 0x08:
            var newArrayIndex = cpu.memory.createNewArray(cpu.register[c]);
            cpu.register[b] = newArrayIndex;
            break;
        case 0x09:
            cpu.memory.abandonArray(cpu.register[c]);
            break;
        case 0x0a:
            cpu.buffer += String.fromCharCode(cpu.register[c]);
            cpu.lastWriteToBufferCounter = cpu.counter;
            break;
        case 0x0b:
            if (cpu.inputProcessor.is_queue_empty()) {
                // Moves counters back in order to be in the same place next time a symbol occurred.
                --cpu.operationFinger;
                --cpu.counter;
                cpu.inputProcessor.set_in_wait_mode(cpu);
                //write_to_um_console("INFO: Queue is empty. Entering in waiting mode.");
                return;
            } else {
                var ch = cpu.inputProcessor.get_next_char();
                console.log(ch);
                cpu.register[c] = ch & 0x000000ff;
            }
            break;
        case 0x0c:
            cpu.memory.copyArrayToZeroIndex(cpu.register[b]);
            cpu.operationFinger = cpu.register[c];
            break;
        case 0x0d:
            a = (operation & 0x0e000000) >>> 25;
            var value = operation & 0x01ffffff;

//                console.log("Argument is: a = 0x" + a.toString(16));
//                console.log("Register before operation was: a = 0x" + cpu.register[a].toString(16));
//                console.log("Value before operation was: a = 0x" + value.toString(16));

            cpu.register[a] = value;

//                console.log("Register after operation was: a = 0x" + cpu.register[a].toString(16));
            break;
        default:
            var message = "ERROR: Incorrect operator " + (operation + 0x100000000).toString(16).substr(-8);
            console.log(message);
            write_to_um_console(message);
        }

//        if (operationType != 0x0d)
//            console.log("Registers after operation were: a = 0x" + cpu.register[a].toString(16)
//                + ', b = 0x' + cpu.register[b].toString(16)
        //                + ', c = 0x' + cpu.register[c].toString(16));
    }
}

function startInterpetation(cpu) {
    cpu.intervalId = setInterval(function() {
        if (!cpu.halted && !cpu.is_waiting_for_input) {
            write_to_notifier("Interpretation is in progress.", true);
            continueInterpretation(cpu);
        } else if (cpu.halted) {
            write_to_notifier("Interpretation has stopped.", false);
            //write_to_um_console("DEBUG: Interval " + cpu.intervalId + " has been cleared.");
            clearInterval(cpu.intervalId);
            cpu.intervalId = -1;
        } else if (cpu.is_waiting_for_input && cpu.intervalId > 0) {
            //write_to_um_console("DEBUG: Interval " + cpu.intervalId + " has been cleared.");
            write_to_notifier("Interpretation is waiting for input.", true);

            clearInterval(cpu.intervalId);
            cpu.intervalId = -1;
        }
    }, 0);

    //write_to_um_console("DEBUG: Interval set to: " + cpu.intervalId);
}