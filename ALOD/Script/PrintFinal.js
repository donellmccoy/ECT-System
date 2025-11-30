// Opens 1 or 2 windows for Form 348/261 documents
// File reference must be placed in <body> section of Print.aspx file in order to work
// Values for viewDoc() function comes from Print.aspx.vb as public vabiables.


//Pop up a window
var $_popup = null;

function showPopup(options) {
    //reuse is currently broken, so it's turned off for now
    options.Reuse = false;

    var values = "";
    var width = Math.min(window.screen.availWidth, options.Width);
    var height = Math.min(window.screen.availHeight, options.Height);

    values += "toolbar=" + (options.ToolBar !== null ? (options.ToolBar ? "1" : "0") : "0");
    values += ",status=" + (options.StatusBar !== null ? (options.StatusBar ? "1" : "0") : "0");
    values += ",menubar=" + (options.MenuBar !== null ? (options.MenuBar ? "1" : "0") : "0");
    values += ",resizable=" + (options.Resizable !== null ? (options.Resizable ? "1" : "0") : "0");
    values += ",scrollbars=" + (options.ScrollBars !== null ? (options.ScrollBars ? "1" : "0") : "0");
    values += ",width=" + width;
    values += ",height=" + height;

    if (options.Center !== null && options.Center === true) {

        var top = Math.round((window.screen.availHeight - height) / 2);
        var left = Math.round((window.screen.availWidth - width) / 2);

        values += ",top=" + top;
        values += ",left=" + left;
    }

    var popup = null;

    if (options.Reuse) {
        if ($_popup === null) {
            //open a new window
            popup = window.open(options.Url, "_blank", values);
            $_popup = popup;
        } else {
            //reuse the existing window
            $_popup.window.location.href = options.Url;
            $_popup.window.resizeTo(width + 10, height + 36);
        }
    } else {
        popup = window.open(options.Url, "_blank", values);
    }

    if ((options.Reload !== null && options.Reload === true) || options.OnClose !== null) {

        var interval = setInterval(function () {

            if (!popup) {
                clearInterval(interval);
                $_popup = null;
                return; // No window created
            }

            try {
                if (popup.closed) {

                    if (typeof options.OnClose != 'undefined' && options.OnClose !== null) {
                        options.OnClose();
                    }

                    if (options.Reload) {
                        if (typeof options.ReloadButton != 'undefined' && options.ReloadButton !== null) {
                            options.ReloadButton.click();
                        } else {
                            window.location.reload(true);
                        }

                        $.blockUI();
                    }

                    $_popup = null;
                    clearInterval(interval);
                }
            } catch (e) {
                $_popup = null;
                clearInterval(interval);
            }

        }, 200);
    }
}


// View final forms 348, 261 if case is complete/final
function viewDocs(path, path2) {
    var url = path;

    showPopup({
        'Url': url,
        'Width': 642,
        'Height': 668,
        'Center': true,
        'Resizable': true,
        'ScrollBars': true
    });

    // checks for Form 261
    var url2 = path2;

    if (url2 != null && url2 != "") {
        showPopup({
            'Url': url2,
            'Width': 642,
            'Height': 668,
            'Center': false,
            'Resizable': true,
            'ScrollBars': true
        });
    }

// close Print browser
self.window.close();
}

