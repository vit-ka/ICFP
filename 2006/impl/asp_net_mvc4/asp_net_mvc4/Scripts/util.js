function write_to_um_console(text) {
    var con = document.getElementById("output_console");

    if (con) {
        con.innerHTML += '<pre class="console_text console_log"> * ' + text + '</pre>';
        con.scrollTop = con.scrollHeight;
    } else {
        console.log("Couldn't find element by id console_output.");
    }
}

function write_to_notifier(text, showImage) {
    var notifier = document.getElementById("notifier_text");
    var notifierImage = document.getElementById("notifier_image");

    if (notifier)
        notifier.innerText = text;

    if (notifierImage)
        notifierImage.style.visibility = showImage ? "visible" : "hidden";
}

function write_to_um_console_inside_machine(text) {
    var con = document.getElementById("output_console");

    if (con) {
        var lastPre = con.lastChild;

        if (lastPre.className.indexOf("console_log") >= 0)
            con.innerHTML += '<pre class="console_text">' + text + '</pre>';
        else {
            con.removeChild(lastPre);
            con.innerHTML += '<pre class="console_text">' + lastPre.innerHTML + text + '</pre>';
        }

        con.scrollTop = con.scrollHeight;
//        console.log("UM Output: " + text);
    } else {
        console.log("Couldn't find element by id console_output.");
    }
}

function load_file_from_server(url, callback) {
    var fileToTest = new XMLHttpRequest();
    fileToTest.open("GET", url, true);
    fileToTest.overrideMimeType("text/plain; charset=x-user-defined");

    fileToTest.onreadystatechange = function() {
        if (fileToTest.readyState == 4 && fileToTest.status == 200) {

            var resp = fileToTest.responseText;
            write_to_um_console('INFO: The file has been downloaded from the server.');
            write_to_um_console('INFO: dataLength: ' + Math.floor(resp.length / 1024) + ' kBi.');

            callback(resp);
        } else {
            if (fileToTest.readyState == 4) {
                write_to_um_console('ERROR: Could not load the file from the server: '
                    + fileToTest.status);
            } else if (fileToTest.readyState == 2) {
                write_to_um_console('INFO: Downloading program file...');
            }
        }
    };

    fileToTest.send(null);
}

function convert_to_array_of_commands(text) {
    var result = [];

    for (var i = 0; i < text.length; i += 4) {
        var command = 0;
        command |= (text.charCodeAt(i) & 0xff) << 24;
        command |= (text.charCodeAt(i + 1) & 0xff) << 16;
        command |= (text.charCodeAt(i + 2) & 0xff) << 8;
        command |= (text.charCodeAt(i + 3) & 0xff);

//        console.log('0x' + (command + 0x100000000).toString(16).substr(-8)
//            + ' = 0x' + text.charCodeAt(i).toString(16)
//            + ' | 0x' + text.charCodeAt(i + 1).toString(16)
//            + ' | 0x' + text.charCodeAt(i + 2).toString(16)
//            + ' | 0x' + text.charCodeAt(i + 3).toString(16));

        result[i / 4] = command;
    }

    write_to_um_console('INFO: ' + result.length + ' commands have been read from file.');

    return result;
}

function Queue() {

    // initialise the queue and offset
    var queue = [];
    var offset = 0;

    /* Returns the length of the queue.
    */
    this.getLength = function() {

        // return the length of the queue
        return (queue.length - offset);

    };

    /* Returns true if the queue is empty, and false otherwise.
    */
    this.isEmpty = function() {

        // return whether the queue is empty
        return (queue.length == 0);

    };

    /* Enqueues the specified item. The parameter is:
    *
    * item - the item to enqueue
    */
    this.enqueue = function(item) {

        // enqueue the item
        queue.push(item);

    };

    /* Dequeues an item and returns it. If the queue is empty then undefined is
    * returned.
    */
    this.dequeue = function() {

        // if the queue is empty, return undefined
        if (queue.length == 0) return undefined;

        // store the item at the front of the queue
        var item = queue[offset];

        // increment the offset and remove the free space if necessary
        if (++offset * 2 >= queue.length) {
            queue = queue.slice(offset);
            offset = 0;
        }

        // return the dequeued item
        return item;

    };

    /* Returns the item at the front of the queue (without dequeuing it). If the
    * queue is empty then undefined is returned.
    */
    this.peek = function() {

        // return the item at the front of the queue
        return (queue.length > 0 ? queue[offset] : undefined);
    };

}


function InputProcessor() {
    this.queue = new Queue();

    InputProcessor.prototype.instance = this;
}

InputProcessor.prototype.process_keypress = function (event) {
    var code = event.charCode;

    if (code == 16 || code == 17 || code == 18)
        return false;

    this.queue.enqueue(code);

    // Adds a new line after carriage return.
    if (code == 13) {
        this.queue.enqueue(10);
    }


    if (this.cpu) {
        this.cpu.is_waiting_for_input = false;
        startInterpetation(this.cpu);
    }

    this.update_char_counter();

//    console.log(code + ": " + String.fromCharCode(code));

    return false;
};

InputProcessor.prototype.set_in_wait_mode = function(cpu) {
    this.cpu = cpu;
    this.cpu.is_waiting_for_input = true;
};

InputProcessor.prototype.is_queue_empty = function() {
    return this.queue.isEmpty();
};

InputProcessor.prototype.get_next_char = function() {
    if (!this.queue.isEmpty()) {
        var ch = this.queue.dequeue();
        this.update_char_counter();
        return ch;
    } else {
        write_to_um_console("ERROR: Empty Input Queue");
        return 0xffffffff;
    }
};

InputProcessor.prototype.update_char_counter = function() {
    var counter = document.getElementById("input_field_counter");
    if (counter)
        counter.innerHTML = "[" + this.queue.getLength() + "]";
};