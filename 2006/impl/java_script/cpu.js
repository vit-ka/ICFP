function Cpu(memory) {
    this.operationFinger = 0;
    this.memory = memory;
    this.startInterpretation = startInterpretation;
    this.register = [0, 0, 0, 0, 0, 0, 0, 0];
}

function startInterpretation() {
    var debug = "";

    while (this.operationFinger < this.memory.getArrayLength(0)) {
        var operation = this.memory.getArrayElem(0, this.operationFinger);

        ++this.operationFinger;

        var operationType = (operation & 0xf0000000) >>> 28;
        var a = (operation & 0x000001c0) >>> 6;
        var b = (operation & 0x00000038) >>> 3;
        var c = (operation & 0x00000007);

//        debug += "0x" + this.operationFinger.toString(16)
//            + ": 0x" + (operation + 0x100000000).toString(16).substr(-8)
//            + " as 0x" + operationType.toString(16)
//            + " a=" + a
//            + " b=" + b
//            + " c=" + c
//            + "["
//            + "0x" + (this.register[0] + 0x100000000).toString(16).substr(-8)
//            + ",0x" + (this.register[1] + 0x100000000).toString(16).substr(-8)
//            + ",0x" + (this.register[2] + 0x100000000).toString(16).substr(-8)
//            + ",0x" + (this.register[3] + 0x100000000).toString(16).substr(-8)
//            + ",0x" + (this.register[4] + 0x100000000).toString(16).substr(-8)
//            + ",0x" + (this.register[5] + 0x100000000).toString(16).substr(-8)
//            + ",0x" + (this.register[6] + 0x100000000).toString(16).substr(-8)
//            + ",0x" + (this.register[7] + 0x100000000).toString(16).substr(-8)
//            + "]\n";

//        console.log("========================================================================");
//        console.log("Operation finger points at " + this.operationFinger.toString(16));
//        console.log("Operation is 0x" + (operation + 0x100000000).toString(16).substr(-8));
//        console.log("Operation type is 0x" + operationType.toString(16));
//
//        if (operationType != 0x0d) {
//            console.log("Operation arguments are: a = 0x" + a + ', b = 0x' + b + ', c = 0x' + c);
//            console.log("Registers before operation were: a = 0x" + this.register[a].toString(16)
//                + ', b = 0x' + this.register[b].toString(16)
//                + ', c = 0x' + this.register[c].toString(16));
//        }

        switch (operationType) {
            case 0x00:
                if (this.register[c] != 0)
                    this.register[a] = this.register[b];
                break;
            case 0x01:
                this.register[a] = this.memory.getArrayElem(this.register[b], this.register[c]);
                break;
            case 0x02:
                this.memory.setArrayElem(this.register[a], this.register[b], this.register[c]);
                break;
            case 0x03:
                this.register[a] = ((this.register[b] + this.register[c]) & 0xffffffff) >>> 0;
                break;
            case 0x04:
                this.register[a] = ((this.register[b] * this.register[c]) & 0xffffffff) >>> 0;
                break;
            case 0x05:
                // TODO: Possible incorrect division in case of negative numbers.
                this.register[a] = (((this.register[b] >>> 0) / (this.register[c] >>> 0)) & 0xffffffff) >>> 0;
                break;
            case 0x06:
                this.register[a] = ((~(this.register[b] & this.register[c])) & 0xffffffff) >>> 0;
                break;
            case 0x07:
                write_to_um_console("INFO: Halt command has been received.");
                write_to_um_console(debug);
                return;
            case 0x08:
                var newArrayIndex = this.memory.createNewArray(this.register[c]);
                this.register[b] = newArrayIndex;
                break;
            case 0x09:
                this.memory.abandonArray(this.register[c]);
                break;
            case 0x0a:
                write_to_um_console_as_char(this.register[c]);
                break;
            case 0x0b:
                // TODO: Implement input queue.
                this.register[c] = 'a'.charCodeAt(0);
                break;
            case 0x0c:
                this.memory.copyArrayToZeroIndex(this.register[b]);
                this.operationFinger = this.register[c];
                break;
            case 0x0d:
                a = (operation & 0x0e000000) >>> 25;
                var value = operation & 0x01ffffff;

//                console.log("Argument is: a = 0x" + a.toString(16));
//                console.log("Register before operation was: a = 0x" + this.register[a].toString(16));
//                console.log("Value before operation was: a = 0x" + value.toString(16));

                this.register[a] = value;

//                console.log("Register after operation was: a = 0x" + this.register[a].toString(16));
                break;
            default:
                var message = "ERROR: Incorrect operator " + (operation + 0x100000000).toString(16).substr(-8);
                console.log(message);
                write_to_um_console(message);
        }

//        if (operationType != 0x0d)
//            console.log("Registers after operation were: a = 0x" + this.register[a].toString(16)
//                + ', b = 0x' + this.register[b].toString(16)
//                + ', c = 0x' + this.register[c].toString(16));


    }
}