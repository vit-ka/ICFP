function Cpu(memory) {
    this.operationFinger = 0;
    this.memory = memory;
    this.startInterpretation = startInterpretation;
    this.register = [0, 0, 0, 0, 0, 0, 0, 0];
}

function startInterpretation() {
    while (this.operationFinger < this.memory.getArrayLength(0) && this.operationFinger < 100) {
        var operation = this.memory.getArrayElem(0, this.operationFinger);
        var operationType = (operation & 0xf0000000) >>> 28;
        var a = (operation & 0x000001c0) >>> 6;
        var b = (operation & 0x00000038) >>> 3;
        var c = (operation & 0x00000007);

        console.log("Operation finger points at " + this.operationFinger);
        console.log("Operation is 0x" + (operation + 0x100000000).toString(16).substr(-8));
        console.log("Operation type is 0x" + (operationType + 0x10).toString(16).substr(-1));
        console.log("Operation arguments are: a = 0x" + a + ', b = 0x' + b + ', c = 0x' + c);

        switch (operationType) {
            case 0x00:
                if (this.register[c] != 0)
                    this.register[a] == this.register[b];
                break;
            case 0x01:
                this.register[a] = this.memory.getArrayElem(this.register[b], this.register[c]);
                break;
            case 0x02:
                this.memory.setArrayElem(this.register[a], this.register[b], this.register[c]);
                break;
            case 0x03:
                this.register[a] = (this.register[b] + this.register[c]) & 0xffffffff;
                break;
            case 0x04:
                this.register[a] = (this.register[b] * this.register[c]) & 0xffffffff;
                break;
            case 0x05:
                // TODO: Possible incorrect division in case of negative numbers.
                this.register[a] = (this.register[b] / this.register[c]) & 0xffffffff;
                break;
            case 0x06:
                this.register[a] = (~(this.register[b] & this.register[c])) & 0xffffffff;
                break;
            case 0x07:
                write_to_um_console("INFO: Halt command has been received.");
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
                this.register[c] = 0;
                break;
            case 0x0c:
                this.memory.moveArrayToZeroIndex(this.register[b]);
                this.operationFinger = this.register[c];
                break;
            case 0x0d:
                a = (operation & 0x0e000000) >> 25;
                var value = operation & 0x01ffffff;

                this.register[a] = value;
                break;
            default:
                var message = "ERROR: Incorrect operator " + (operation + 0x100000000).toString(16).substr(-8);
                console.log(message);
                write_to_um_console(message);
        }

        ++this.operationFinger;
    }
}