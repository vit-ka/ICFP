function write_to_um_console(text) {
    var con = document.getElementById("output_console");

    if (con) {
        con.innerHTML += '<pre class="console_text console_log">' + text + '</pre>';
    }
    else {
        console.log("Couldn't find element by id console_output.");
    }
}

function write_to_um_console_as_char(charCode) {
    console.log("OUTPUT: 0x" + charCode.toString(16));
    var con = document.getElementById("output_console");

    if (con) {
        var lastPre = con.lastChild;

        if (lastPre.className.indexOf("console_log") >= 0)
            con.innerHTML += '<pre class="console_text">' + String.fromCharCode(charCode) + '</pre>';
        else {
            con.removeChild(lastPre);
            con.innerHTML += '<pre class="console_text">' + lastPre.innerHTML + String.fromCharCode(charCode) + '</pre>';
        }
    }
    else {
        console.log("Couldn't find element by id console_output.");
    }
}

function load_file_from_server(url, callback) {
    var fileToTest = new XMLHttpRequest();
    fileToTest.open("GET", url, true);
    fileToTest.overrideMimeType("text/plain; charset=x-user-defined");

    fileToTest.onreadystatechange = function () {
        if (fileToTest.readyState == 4 && fileToTest.status == 200) {

            var resp = fileToTest.responseText;
            write_to_um_console('INFO: The file has been downloaded from the server.');
            write_to_um_console('INFO: dataLength: ' + Math.floor(resp.length / 1024) + ' kBi.');

            callback(resp);
        }
        else {
            if (fileToTest.readyState == 4) {
                write_to_um_console('ERROR: Could not load the file from the server: '
                    + fileToTest.status)
            }
            else if (fileToTest.readyState == 2) {
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