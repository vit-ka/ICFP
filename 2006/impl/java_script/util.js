function write_to_um_console(text) {
    var con = document.getElementById("output_console");

    if (con) {
        con.innerHTML += '<pre class="console_text">' + text + '</pre>';
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
            write_to_um_console('INFO: The file has been downloaded from the server.\n' +
                'INFO: dataLength: ' + Math.floor(resp.length / 1024) + ' kBi.');

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
        command |= text[i] << 24;
        command |= text[i + 1] << 16;
        command |= text[i + 2] << 8;
        command |= text[i + 3];

        result[i / 4] = command;
    }

    write_to_um_console('INFO: ' + result.length + ' commands have been read from file.');

    return result;
}