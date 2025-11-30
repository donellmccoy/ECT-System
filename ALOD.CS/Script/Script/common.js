

//function to validate a text box is not empty
$.fn.validateTextBox = function (options) {
    if (this === null || this.length === 0) {
        return false;
    }

    if (this[0].value.length === 0) {
        $(this).addClass('fieldRequired');
        return false;
    }

    $(this).removeClass('fieldRequired');
    return true;
};

$.fn.validateDropDown = function (options) {
    if (this === null || this.length === 0) {
        return false;
    }

    if (this[0].selectedIndex === 0) {
        $(this).addClass('fieldRequired');
        return false;
    }

    $(this).removeClass('fieldRequired');
    return true;
};

// Add some stuff to jquery
jQuery.extend({

    // Common function to show dialog windows
    showDialog: function (dialog, options) {

       // $.ui.dialog({ bgiframe: true });
        $(dialog).dialog(options);
    },

    hideDialog: function (dialog) {
        //   $(dialog).jqm().jqmHide();
    }

});


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

function confirmAction(action) {
    return confirm('Are you sure you want to ' + action + ' this case?');
}


function showWhois(xml) {
    $('#dvLoading').remove();

    var node = $(xml).find('User');

    var Address = node.attr('Address') !== null ? node.attr('Address') : new String();
    var city = node.attr('City') !== null ? node.attr('City') : new String();
    var state = node.attr('State') !== null ? node.attr('State') : new String();
    var zip = node.attr('Zip') !== null ? node.attr('Zip') : new String();

    var table = $.create(
        "p", { "style": "padding: 6px; font-size: 12px;" },
        [
            "TABLE", { "cellpadding": "2", "cellspacing": "2" },
            [
                "TBODY", {},
                [
                    "TR", {},
                    [
                        "TD", { "style": "font-weight: bold; text-align: right;" }, ["Name:"],
                        "TD", {},
                        [
                            node.attr('Rank'), ' ', node.attr('LastName'), ', ', node.attr('FirstName')
                        ]
                    ],
                    "TR", {},
                    [
                        "TD", { "style": "font-weight: bold; text-align: right;" }, ["Role:"],
                        "TD", {},
                        [
                            node.attr('Role')
                        ]
                    ],
                    "TR", {},
                    [
                        "TD", { "style": "font-weight: bold; text-align: right;" }, ["Email:"],
                        "TD", {},
                        [
                            "A", { "href": "mailto:" + node.attr('Email') },
                            [
                                node.attr('Email')
                            ]
                        ]
                    ],
                    "TR", {},
                    [
                        "TD", { "style": "font-weight: bold; text-align: right;" }, ["DSN:"],
                        "TD", {},
                        [
                            node.attr('DSN')
                        ]
                    ],
                    "TR", {},
                    [
                        "TD", { "style": "font-weight: bold; text-align: right;" }, ["Phone:"],
                        "TD", {},
                        [
                            node.attr('Phone')
                        ]
                    ],
                    "TR", {},
                    [
                        "TD", { "style": "font-weight: bold; text-align: right; vertical-align: top;" }, ["Address:"],
                        "TD", {},
                        [
                            "SPAN", {}, [Address],
                            "BR", {}, [],
                            "SPAN", {}, [city, ', ', state, ' ', zip]

                        ]
                    ]

                ]
            ]
        ]
    );

    $('#whois').append(table);
}

function hideDialog() {
    $('#whois').jqm().jqmHide();
    $('#whois').remove();
}

// Whois popup 
function getWhois(id) {
    //start with a clean popup
    $('#whois').remove();

    //create the main div
    var parent = $.create(

        "DIV",
        {
            "id": "whois",
            "title": "User Information"
        },
        [
            "P",
            {
                "id": "dvLoading",
                "class": "labelWait"
                //"style": "width: 296px; text-align: center; margin-top: 10px; margin-bottom: 10px; border-width: 0px;"
            },
            [
                "IMG", { "src": $_HOSTNAME + "/images/busy.gif", "alt": "Loading" }, [],
                "span", {}, ["Loading..."]
            ]
        ]

    );

    $('body').append(parent);

    $.showDialog('#whois', { draggable: true, resizable: true, modal: true });

    $.ajax({
        type: 'POST',
        url: $_HOSTNAME + '/Secure/Utility/DataService.asmx/GetWhois',
        data: 'userId=' + id,
        dataType: 'xml',
        success: showWhois
    });
}

// Removes any HTML tags from a string
function removeHTMLTags(input) {
    return input.replace(/<\/?[^>]+(>|$)/mig, "");
}

function Trim(sValue) {
    return sValue.replace(/^\s*|\s*$/g, "");
}

function initMultiLines() {

    $('.textLimit:not(.tladded)').each(function () {

        $(this).addClass('tladded');

        var width = $(this).width();

        var box = $.create("div", {
            'class': 'multilineDisplay',
            'style': 'width:auto;'
        },
            [
                "span", { 'id': this.id + '_label' }, ["Characters Remaining: "],
                "span", { 'id': this.id + '_output', 'class': 'number' }, []
            ]
        );

        $(box).insertAfter($(this));

        //set the initial count
        var max = parseInt(this.attributes.maxLength.value, 10);
        var cur = parseInt(this.value.length, 10);
        var count = (this.value.match(new RegExp("\\r|\\n", "g")) || []).length;
        var left = (max - cur) - count;

        $('#' + this.id + '_output').html(String(left));

        $(this).on("keyup", function (e) {
            cur = parseInt(this.value.length, 10);
            count = (this.value.match(new RegExp("\\r|\\n", "g")) || []).length;
            left = Math.max(0, (max - cur) - count);

            $('#' + this.id + '_output').html(String(left));

            if (((e.key >= ' ' && e.key <= '~') || (e.key > '�')) && left <= 0 || (e.key === 'Enter') && left <= 0) {
                // Cut it back to the max 
                this.value = this.value.slice(0, max - count);
                return false;
            }
        });




       
    });
}

// shortcut to getElementById 
//works with pages with and without a master page
function element(item) {
    var obj = document.getElementById(item);

    if (obj == null) {
        //ctl00_ContentMain_btnOptionGo
        obj = document.getElementById('ctl00_ContentMain_' + item);
    }

    if (obj === null) {
        obj = document.getElementById('ctl00_ctl00_ContentMain_ContentNested_' + item);
    }

    return obj;
}


function radioElement(item) {

    obj = document.getElementsByName('ctl00$ctl00$ContentMain$ContentNested$' + item);
    return obj;
}

function radioElementById(item) {

    obj = document.getElementById('ctl00_ctl00_ContentMain_ContentNested_' + item);
    return obj;
}

function unCheckRadioList(radioName) {
    obj = document.getElementsByName(radioName);
    for (var i = 0; i < obj.length; i++) {
        obj[i].checked = false;
    }
    return obj;
}

function enableRadioList(radioName, enable) {
    obj = document.getElementsByName(radioName);
    for (var i = 0; i < obj.length; i++) {
        obj[i].disabled = !enable;
    }
    return obj;
}

function disableRadioElement(item) {
    obj = element(item);
    for (var i = 0; i < obj.length; i++) {
        obj[i].disbled = true;
    }
    return obj;
}

function enableRadioElement(item) {
    obj = element(item);
    for (var i = 0; i < obj.length; i++) {
        obj[i].disbled = false;
    }
    return obj;
}

function unCheckRadioElement(item) {
    obj = element(item);
    for (var i = 0; i < obj.length; i++) {
        obj[i].checked = false;
    }
    return obj;
}

function setNoRadioElement(item) {
    obj = document.getElementsByName('ctl00$ctl00$ContentMain$ContentNested$' + item);
    obj[0].checked = false; //yes option 
    obj[1].checked = true; //no option 

    return obj;
}

// this will get an element from the parent window
//first checking the opener (for popups)
//and then the parent frame (for iframes)
function parentElement(item) {
    var obj = window.opener.parent.document.getElementById(item);

    if (obj === null) {
        obj = window.opener.parent.document.getElementById('ctl00_ctl00_ContentMain_ContentNested_' + item);
    }

    return obj;
}

function enableControl(controlId, enabled) {
    if (enabled) {
        $(element(controlId)).removeAttr('disabled');
    } else {
        $(element(controlId)).attr('disabled', 'disabled');
    }
}

// Limit type of input in text boxes 
function checkFormat(control, e, type, optional) {
    var key;

    if (e) {
        key = e.keyCode || e.which;
    } else {
        return true;
    }

    if (key === null || key === 0 || key === 8 || key === 9 || key === 13 || key === 27) {
        return true;
    }

    var charKey = String.fromCharCode(key).toLowerCase();

    if (type == 'Numeric') {
        if ((optional + '0123456789').indexOf(charKey) != -1) {
            return true;
        }
        return false;
    }
    else if (type == 'Alpha') {
        if ((optional + 'abcdefghijklmnopqrstuvwxyz').indexOf(charKey) != -1) {
            return true;
        }
        return false;
    }
    else if (type == 'Money') {
        // Allow digits
        if (('0123456789').indexOf(charKey) != -1) {
            return true;
        }

        if (('.').indexOf(charKey) != -1) {
            // Allow one (and only one) .
            if (control.value.indexOf('.') == -1) {
                // This is the first . so allow it
                return true;
            }
        }
        return false;
    }
    else if (type == 'UIC') {
        if (control.value.length === 0) {
            // Make sure the first character has to be a 'W'
            if (charKey != 'w') {
                return false;
            }
            else {
                return true;
            }
        }
        else {
            if (control.value.length >= 1 && control.value.length <= 3) {
                if (('0123456789abcdefghijklmnopqrstuvwxyz').indexOf(charKey) != -1) {
                    return true;
                }
                else {
                    return false;
                }
            }

            if (control.value.length >= 4) {
                if (('*0123456789abcdefghijklmnopqrstuvwxyz').indexOf(charKey) != -1) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
    }
    else {
        if ((optional + '0123456789abcdefghijklmnopqrstuvwxyz').indexOf(charKey) != -1) {
            return true;
        }
        return false;
    }

    return true;
}


//Function to get a value from the query string 
//returns defValue if the key is not found 

function getQueryStringValue(key, defValue) {
    //if a default value is not supplied this will return null rather then undefined
    //yes, it looks silly, blame javascript	
    if (defValue === null) {
        defValue = null;
    }

    var query = window.location.search.substring(1);
    var vars = query.split("&");

    for (var i = 0; i < vars.length; i++) {
        var pair = vars[i].split("=");

        if (pair[0] == key) {
            return pair[1];
        }
    }

    return defValue;
}


// Default button handler (for enter keys)
function defaultButton(button, e) {
    var key;

    if (e) {
        key = e.keyCode || e.which;
    } else {
        return true;
    }

    if (key == 13) {
        var ctrl = element(button);

        if (typeof ctrl != 'undefined' && ctrl !== null) {
            ctrl.click();
            e.returnValue = false;
            e.cancel = true;
        }
    }
}


//event handling
function getEventTarget(e) {
    if (e) {
        if (e.target) {
            return e.target;
        } else if (e.srcElement) {
            return e.srcElement;
        }
    }
    return null;
}



// Alert Message for Page_Expired
function alertUser(msg) {
    alert(msg);
}

// select box helper methods
//-------------------------------------------------------------------------
// gets the currently selected value from a drop down box

function getSelectedText(id) {
    var box = element(id);

    if (box.options.length === 0) {
        return '';
    }

    return box.options[box.selectedIndex].text;
}


function getSelectedValue(id) {
    var box = element(id);

    if (box.options.length === 0) {
        return '';
    }

    return box.options[box.selectedIndex].value;
}

// sets the currently selected value for a drop down box

function setSelectedValue(id, value) {
    var box = element(id);

    if (!box) {
        return;
    }

    for (var i = 0; i < box.options.length; i++) {
        if (box.options[i].value == value) {
            box.selectedIndex = i;
            return;
        }
    }
}

// Gets the Text value from a select control based on value
function getSelectTextByValue(id, value) {
    var box = element(id);

    if (!box) {
        return "";
    }

    var scope = "0";

    $(box).children().each(function () {
        if (this.value == value) {
            scope = this.text;
        }
    });

    return scope;
}

//Shows or hides a selectionbox
function setSelectionVisible(sel) {
    var lst = element(sel);

    if (lst !== null) {
        if (lst.options.length < 1) {
            $(lst).hide();
        } else {
            $(lst).show();
        }
    }
}

// -------------------------------------------------------------------
// hasOptions(obj)
//  Utility function to determine if a select object has an options array
// -------------------------------------------------------------------
function hasOptions(obj) {
    if (obj !== null && obj.options !== null) { return true; }
    return false;
}

function swapOptions(obj, i, j) {
    var o = obj.options;
    var i_selected = o[i].selected;
    var j_selected = o[j].selected;
    var temp = new Option(o[i].text, o[i].value, o[i].defaultSelected, o[i].selected);
    var temp2 = new Option(o[j].text, o[j].value, o[j].defaultSelected, o[j].selected);
    o[i] = temp2;
    o[j] = temp;
    o[i].selected = j_selected;
    o[j].selected = i_selected;
}

// -------------------------------------------------------------------
// moveOptionUp(select_object)
//  Move selected option in a select list up one
// -------------------------------------------------------------------
function moveOptionUp(obj) {
    if (!hasOptions(obj)) { return; }
    for (var i = 0; i < obj.options.length; i++) {
        if (obj.options[i].selected) {
            if (i !== 0 && !obj.options[i - 1].selected) {
                swapOptions(obj, i, i - 1);
                obj.options[i - 1].selected = true;
            }
        }
    }
}

// -------------------------------------------------------------------
// copySelectedOptions(select_object,select_object[,autosort(true/false)])
//  This function copies options between select boxes instead of 
//  moving items. Duplicates in the target list are not allowed.
// -------------------------------------------------------------------
function copySelectedOptions(from, to) {
    var options = {};
    var i;

    if (hasOptions(to)) {
        for (i = 0; i < to.options.length; i++) {
            options[to.options[i].value] = to.options[i].text;
        }
    }

    if (!hasOptions(from)) { return; }

    for (i = 0; i < from.options.length; i++) {
        var o = from.options[i];
        if (o.selected) {
            if (options[o.value] === null || options[o.value] == "undefined" || options[o.value] != o.text) {
                var index = hasOptions(to) ? to.options.length : 0;
                //if (!hasOptions(to)) { var index = 0; } else { var index=to.options.length; }
                to.options[index] = new Option(o.text, o.value, false, false);
            }
        }
    }

}


// -------------------------------------------------------------------
// copyAllSelectedOptions(select_object,select_object[,autosort(true/false)])
//  This function copies all between select boxes instead of 
//  moving items. Duplicates in the target list are not allowed.
// -------------------------------------------------------------------
function copyAllSelectedOptions(from, to) {
    var options = {};
    var i;

    if (hasOptions(to)) {
        for (i = 0; i < to.options.length; i++) {
            options[to.options[i].value] = to.options[i].text;
        }
    }

    if (!hasOptions(from)) { return; }

    for (i = 0; i < from.options.length; i++) {
        var o = from.options[i];
        if (options[o.value] === null || options[o.value] == "undefined" || options[o.value] != o.text) {
            var index = hasOptions(to) ? to.options.length : 0;
            //if (!hasOptions(to)) { var index = 0; } else { var index=to.options.length; }
            to.options[index] = new Option(o.text, o.value, false, false);
        }
    }

}


// -------------------------------------------------------------------
// moveSelectedOptions(select_object,select_object[,autosort(true/false)])
// This function moves options between select boxes. 
// Duplicates in the target list are not allowed.
// -------------------------------------------------------------------
function moveSelectedOptions(from, to) {
    var options = {};
    var i;

    if (hasOptions(to)) {
        for (i = 0; i < to.options.length; i++) {
            options[to.options[i].value] = to.options[i].text;
        }
    }

    if (!hasOptions(from)) { return; }

    var rem = [];

    for (i = 0; i < from.options.length; i++) {
        var o = from.options[i];
        if (o.selected) {
            if (options[o.value] === null || options[o.value] == "undefined" || options[o.value] != o.text) {
                var index = hasOptions(to) ? to.options.length : 0;
                //if (!hasOptions(to)) { var index = 0; } else { var index=to.options.length; }
                to.options[index] = new Option(o.text, o.value, false, false);
                rem.push(i);
            }
        }
    }

    for (i = rem.length; i >= 0; i--) {
        from.options[rem[i]] = null;
    }
}

// -------------------------------------------------------------------
// moveOptionDown(select_object)
//  Move selected option in a select list down one
// -------------------------------------------------------------------
function moveOptionDown(obj) {
    if (!hasOptions(obj)) { return; }
    for (i = obj.options.length - 1; i >= 0; i--) {
        if (obj.options[i].selected) {
            if (i != (obj.options.length - 1) && !obj.options[i + 1].selected) {
                swapOptions(obj, i, i + 1);
                obj.options[i + 1].selected = true;
            }
        }
    }
}

// -------------------------------------------------------------------
// removeSelectedOptions(select_object)
//  Remove all selected options from a list
//  (Thanks to Gene Ninestein)
// -------------------------------------------------------------------
function removeSelectedOptions(from) {
    if (!hasOptions(from)) { return; }
    if (from.type == "select-one") {
        from.options[from.selectedIndex] = null;
    }
    else {
        for (var i = (from.options.length - 1); i >= 0; i--) {
            var o = from.options[i];
            if (o.selected) {
                from.options[i] = null;
            }
        }
    }
    from.selectedIndex = -1;
}

// -------------------------------------------------------------------
// removeAllOptions(select_object)
//  Remove all options from a list
// -------------------------------------------------------------------
function removeAllOptions(from) {
    if (!hasOptions(from)) { return; }
    for (var i = (from.options.length - 1); i >= 0; i--) {
        from.options[i] = null;
    }
    from.selectedIndex = -1;
}



//-------------------------------------------------------------------------
//Pop up a window 
function PopUp(URL, W, H, SB, RS) {
    W = Math.min(window.screen.availWidth, W);
    H = Math.min(window.screen.availHeight, H);
    Y = Math.round((window.screen.availHeight - H) / 2);
    X = Math.round((window.screen.availWidth - W) / 2);
    if (SB === null) { SB = 0; }
    if (RS === null) { RS = 0; }
    var Cmd = "toolbar=0,status=0,menubar=0,";
    Cmd += "resizable=" + RS + ",scrollbars=" + SB + ",";
    Cmd += "width=" + W + ",height=" + H + ",top=" + Y + ",left=" + X;
    return window.open(URL, "_blank", Cmd);
}


//Validates Integer  value
function IntCheck(oTextBox, DefaultValue) {
    var Value = oTextBox.value;
    if (isNaN(Value) || Value.indexOf(".") > -1) {
        alert("Please enter an integer value.");
        var IntValue = parseInt(Value, 10);
        oTextBox.value = isNaN(IntValue) ? DefaultValue : IntValue;
        oTextBox.select();
    }
}

function TimeCheck(oTextBox) {
    //Validate military time	
    var Time = oTextBox.value;

    if (Time.length === 0) {
        return;
    }

    var Time1 = parseInt(Time.substr(0, 2), 10);
    var Time2 = parseInt(Time.substr(2, 2), 10);
    if (Time.length < 4 || isNaN(Time) || Time1 < 0 || Time1 > 23 || Time2 < 0 || Time2 > 59) {
        alert("Invalid time.\nValid range: 0000-2359");
        oTextBox.value = "";
        oTextBox.focus();
    }
}

//Function to test for a radio button checked 
function IsChecked(btn) {
    var cnt = -1;
    for (var i = btn.length - 1; i > -1; i--) {
        if (btn[i].checked) {
            cnt = i; i = -1;
        }
    }

    if (cnt > -1) {
        return true;
    } else {
        return false;
    }
}

function daysInFebruary(year) {
    // February has 29 days in any year evenly divisible by four,
    // EXCEPT for centurial years which are not also divisible by 400.
    return (((year % 4 === 0) && ((!(year % 100 === 0)) || (year % 400 === 0))) ? 29 : 28);
}

function dayInMonth(n) {
    if (n == 4 || n == 6 || n == 9 || n == 11) { return 30; }
    if (n == 2) { return 29; }
    return 31;
}

function IsValidDate(strDate) {

    //Validate military time
    var minYear = 1900;
    var maxYear = 2100;

    var parts = strDate.split("/");

    if (parts.length != 3) {
        return false;
    }

    var month = parseInt(parts[0], 10);
    var day = parseInt(parts[1], 10);
    var year = parseInt(parts[2], 10);

    if (month < 1 || month > 12) {
        return false;
    }

    if (day < 1 || day > 31 || (month == 2 && day > daysInFebruary(year)) || day > dayInMonth(month)) {
        return false;
    }

    if (year === 0 || year < minYear || year > maxYear) {
        return false;
    }

    return true;

}


function DateCheck(oTextBox) {
    //Validate date

    var minYear = 1900;
    var maxYear = 2100;
    var strDate = oTextBox.value;

    if (strDate.length === 0) {
        return;
    }

    if (strDate.length < 8 || strDate.length > 10 || (IsValidDate(strDate) === false)) {
        alert("Invalid Date.\nDate format is: MM/DD/YYYY");
        oTextBox.value = "";
        oTextBox.focus();
    }
}





// Convert an XML date to a military formatted date 
function xmlDateToMilitaryDate(input) {
    var year = input.substr(0, 4);
    var month = input.substr(5, 2);
    var day = input.substr(8, 2);

    return String(year) + String(month) + String(day);
}


// -------------------------------------------------------------------
// compareDates(date1,date1format,date2,date2format)
//   Compare two date strings to see which is greater.
//   Returns:
//   1 if date1 is greater than date2
//   0 if date2 is greater than date1 of if they are the same
//  -1 if either of the dates is in an invalid format
// -------------------------------------------------------------------
function compareDates(date1, dateformat1, date2, dateformat2) {
    var d1 = getDateFromFormat(date1, dateformat1);
    var d2 = getDateFromFormat(date2, dateformat2);
    if (d1 === 0 || d2 === 0) {
        return -1;
    }
    else if (d1 > d2) {
        return 1;
    }
    return 0;
}


//Helper to check if a list box has selection 
function checkList(listId) {
    var list = element(listId);

    if (list.selectedIndex === 0) {
        $(list).addClass('fieldRequired');
        return false;
    }

    $(list).removeClass('fieldRequired');
    return true;
}

var LastBackgroundColor = null;
function ListMouseOver(oTR) {
    LastBackgroundColor = oTR.style.backgroundColor;
    oTR.style.backgroundColor = "#ffff99";
    //oTR.style.color = "black";
}
function ListMouseOut(oTR) {
    oTR.style.backgroundColor = LastBackgroundColor;
    //oTR.style.color = "black";
}



// Print / view forms and documents

function PrintForm348() {
    var refId = getQueryStringValue('refId', 0);
    var url = "";
    var moduleId = getQueryStringValue('moduleId', 2);

    // handle reinvestigations
    var requestId = getQueryStringValue('requestId', 0);
    if ((requestId > 0) && (refId == 0)) {
        refId = requestId;
        moduleId = 5;
    }

    if (refId === 0) {
        return;
    }

    switch (moduleId) {
        case 2:
            url = $_HOSTNAME + "/secure/lod/Print.aspx?refId=" + refId;
            break;
        case 5:
            url = $_HOSTNAME + "/secure/ReinvestigationRequests/Print.aspx?moduleId=5&refId=" + refId;
            break;
        case 6:
            url = $_HOSTNAME + "/secure/SC_Incap/Print.aspx?refId=" + refId;
            break;
        case 7:
            url = $_HOSTNAME + "/secure/SC_Congress/Print.aspx?refId=" + refId;
            break;
        case 8:
            url = $_HOSTNAME + "/secure/SC_BMT/Print.aspx?refId=" + refId;
            break;
        case 9:
            url = $_HOSTNAME + "/secure/SC_WWD/Print.aspx?refId=" + refId;
            break;
        case 10:
            url = $_HOSTNAME + "/secure/SC_PWaivers/Print.aspx?refId=" + refId;
            break;
        case 11:
            url = $_HOSTNAME + "/secure/SC_MEB/Print.aspx?refId=" + refId;
            break;
        case 12:
            url = $_HOSTNAME + "/secure/SC_BCMR/Print.aspx?refId=" + refId;
            break;
        case 13:
            url = $_HOSTNAME + "/secure/SC_FastTrack/Print.aspx?refId=" + refId;
            break;
        case 14:
            url = $_HOSTNAME + "/secure/SC_CMAS/Print.aspx?refId=" + refId;
            break;
        case 15:
            url = $_HOSTNAME + "/secure/SC_MEPS/Print.aspx?refId=" + refId;
            break;
        case 16:
            url = $_HOSTNAME + "/secure/SC_MMSO/Print.aspx?refId=" + refId;
            break;
        case 17:
            url = $_HOSTNAME + "/secure/SC_MH/Print.aspx?refId=" + refId;
            break;
        case 24:
            url = $_HOSTNAME + "/secure/AppelRequest/Print.aspx?refId=" + refId;
        default:  // case 2
            url = $_HOSTNAME + "/secure/lod/Print.aspx?refId=" + refId;
    }

    // if (form !== null) 
    //url += "&form=" + form;

    showPopup({
        'Url': url,
        'Center': true,
        'Width': 900,
        'Height': 800,
        'Resizable': true
    });

}

function PrintForm348R() {
    var refId = getQueryStringValue('refId', 0);
    if (refId === 0) {
        return;
    }
    var url = $_HOSTNAME + "/secure/SARC/Print.aspx?refId=" + refId;
    showPopup({ 'Url': url, 'Center': true, 'Width': 900, 'Height': 800, 'Resizable': true });
}

function PrintPHForm() {
    var refId = getQueryStringValue('refId', 0);
    if (refId === 0) {
        return;
    }
    var url = $_HOSTNAME + "/secure/SC_PH/Print.aspx?refId=" + refId;
    showPopup({ 'Url': url, 'Center': true, 'Width': 900, 'Height': 800, 'Resizable': true });
}

function PrintWWD() {
    var refId = getQueryStringValue('refId', 0);
    if (refId === 0) {
        return;
    }
    var url = $_HOSTNAME + "/secure/SC_WWD/Print.aspx?refId=" + refId;
    showPopup({'Url': url,'Center': true,'Width': 900, 'Height': 800,'Resizable': true});
}

function PrintPSID() {
    var refId = getQueryStringValue('refId', 0);
    if (refId === 0) {
        return;
    }
    var url = $_HOSTNAME + "/secure/SC_PSID/Print.aspx?refId=" + refId;
    showPopup({ 'Url': url, 'Center': true, 'Width': 900, 'Height': 800, 'Resizeable': true });
}

function PrintFT() {
    var refId = getQueryStringValue('refId', 0);
    if (refId === 0) {
        return;
    }
    var url = $_HOSTNAME + "/secure/SC_FastTrack/Print.aspx?refId=" + refId;
    showPopup({ 'Url': url, 'Center': true, 'Width': 900, 'Height': 800, 'Resizable': true });
}

function PrintRRForm348() {
    var refId = getQueryStringValue('refId', 0);
    if (refId === 0) {
        return;
    }
    var url = $_HOSTNAME + "/secure/ReinvestigationRequests/Print.aspx?moduleId=5&refId=" + refId;
    showPopup({ 'Url': url, 'Center': true, 'Width': 900, 'Height': 800, 'Resizable': true });
}

function PrintMMSO() {
    var refId = getQueryStringValue('refId', 0);
    if (refId === 0) {
        return;
    }
    var url = $_HOSTNAME + "/secure/SC_MMSO/Print.aspx?refId=" + refId;
    showPopup({ 'Url': url, 'Center': true, 'Width': 900, 'Height': 800, 'Resizable': true });
}

function PrintAppeal() {
    var refId = getQueryStringValue('requestId', 0);
    if (refId === 0) {
        return;
    }
    var url = $_HOSTNAME + "/secure/AppealRequests/Print.aspx?moduleId=24&refId=" + refId;
    showPopup({ 'Url': url, 'Center': true, 'Width': 900, 'Height': 800, 'Resizable': true });
}

function PrintSpecCase(moduleId) {
    var refId = getQueryStringValue('refId', 0);
    if (refId === 0) {
        return;
    }
    var url = "";
    switch (moduleId)
    {
        case 6:
            url = $_HOSTNAME + "/secure/SC_Incap/Print.aspx?refId=" + refId;
            break;
        case 7:
            url = $_HOSTNAME + "/secure/SC_Congress/Print.aspx?refId=" + refId;
            break;
        case 8:
            url = $_HOSTNAME + "/secure/SC_BMT/Print.aspx?refId=" + refId;
            break;
        case 9:
            url = $_HOSTNAME + "/secure/SC_WWD/Print.aspx?refId=" + refId;
            break;
        case 10:
            url = $_HOSTNAME + "/secure/SC_PWaivers/Print.aspx?refId=" + refId;
            break;
        case 11:
            url = $_HOSTNAME + "/secure/SC_MEB/Print.aspx?refId=" + refId;
            break;
        case 12:
            url = $_HOSTNAME + "/secure/SC_BCMR/Print.aspx?refId=" + refId;
            break;
        case 13:
            url = $_HOSTNAME + "/secure/SC_FastTrack/Print.aspx?refId=" + refId;
            break;
        case 14:
            url = $_HOSTNAME + "/secure/SC_CMAS/Print.aspx?refId=" + refId;
            break;
        case 15:
            url = $_HOSTNAME + "/secure/SC_MEPS/Print.aspx?refId=" + refId;
            break;
        case 16:
            url = $_HOSTNAME + "/secure/SC_MMSO/Print.aspx?refId=" + refId;
            break;
        case 17:
            url = $_HOSTNAME + "/secure/SC_MH/Print.aspx?refId=" + refId;
            break;
    }
    showPopup({ 'Url': url, 'Center': true, 'Width': 900, 'Height': 800, 'Resizable': true });
}






function viewDoc(url) {
    showPopup({
        'Url': url,
        'Width': 642,
        'Height': 668,
        'Center': true,
        'Resizable': true,
        'ScrollBars': true
        });
    }



function pascodeValidation(pascode) {
    $.ajax({
        type: 'POST',
        url: $_HOSTNAME + '/Secure/Utility/DataService.asmx/ValidPascode',
        data: 'pascode=' + pascode,
        dataType: 'xml',
        success: displayMessage
    });
}

function displayMessage(xml) {
    return (xml.documentElement.firstChild.nodeValue);
}


function confirmPascode(pascode) {
    if (pascodeValidation(pascode)) {
        //alert("test");
        return true;
    }
    else {
        alert("Invalid pascode");
        return false;
    }
}

(function () {

    //set blockUi defaults
    $.blockUI.defaults.overlayCSS.backgroundColor = '#C0C0C0';
    $.blockUI.defaults.overlayCSS.opacity = '0.75';
    $.blockUI.defaults.message = '<div class="labelWait"><img src="' + $_HOSTNAME + '/images/busy.gif" /> Please Wait...</div>';

    initMultiLines();
});

function showBusy(targetId) {
    $('#' + targetId).html("<img src='../App_Themes/DefaultBlue/images/busy-tiny.gif'/>");
}


function onLodLoadComplete(result, userContext, methodName) {
    $('#' + userContext).innerHTML(result);
}

function onLodLoadFailed(result, userContext, methodName) {
    $('#' + userContext).html('');
}


function onUnitLookupComplete(result, userContext, methodName) {
    var ddl = $('#' + userContext).get(0);
    ddl.options.length = 0;
    ddl.options[0] = new Option("-- All --", '0');
    $.each(result, function (index, item) {
        ddl.options[ddl.options.length] = new Option(item.Name, item.Value);
    });
}

function onUnitLookupFailed(error) {
    var ddl = $('#' + userContext).get(0);
    ddl.options.length = 0;
}

function onInBoxLookUpComplete(result, userContext, methodName) {
      $('#' + userContext).html(result);
}

function onInBoxLookUpFailed(result, userContext, methodName) {
     $('#' + userContext).html('');
 }

 function searchStart() {
     $('#spWait').show();
 }
 function searchEnd() {
     $('#spWait').hide();
 } 
          
 function isStringNullOrEmpty(str) {
     return (!str || str.length === 0);
 }

 function calendarPick(pick, calendar) {
     

     switch (pick) {
         //1
         case "All":
             return {
                 showOn: 'both',
                 buttonImage: calendar,
                 buttonImageOnly: true,
                 changeMonth: true,
                 changeYear: true
             }
         //2
         case "Past":
             return {
                 showOn: 'both',
                 buttonImage: calendar,
                 buttonImageOnly: true,
                 maxDate: "d",
                 changeMonth: true,
                 changeYear: true
             }
        //3
         case "TomorrowFuture":
             return {
                 showOn: 'both',
                 buttonImage: calendar,
                 buttonImageOnly: true,
                 changeMonth: true,
                 changeYear: true,
                 minDate: '+1d'
             }
        //4
         case "Future":
             return {
                 showOn: 'both',
                 buttonImage: calendar,
                 buttonImageOnly: true,
                 changeMonth: true,
                 changeYear: true,
                 minDate: 'd'
             }
        //5
         case "DocPast":
             return {
                 showOn: 'both',
                 buttonImage: calendar,
                 buttonImageOnly: true,
                 changeMonth: true,
                 changeYear: true,
                 maxDate: 'd',
                 beforeShow: function () {
                     $("#ui-datepicker-div").css("z-index", '2006'); // The z-index property specifies the stack order of an element. An element with greater stack order is always in front of an element with a lower stack order.
                 }
             }
        //6
         case "DocAll":
             return {
                 showOn: 'both',
                 buttonImage: calendar,
                 buttonImageOnly: true,
                 changeMonth: true,
                 changeYear: true,
                 beforeShow: function () {
                     $("#ui-datepicker-div").css("z-index", '2006');
                 }
             }
     }
     
 }

//used to display a form 348/261

 function printForms(id, casetype) {
     var url;

     switch (casetype) {
         case "lod":
             url = $_HOSTNAME + "/secure/lod/Print.aspx?id=" + id;
             break;
         case "SC_PH":
             url = $_HOSTNAME + "/Secure/SC_PH/Print.aspx?refId=" + id;
             break;
         case "SARC":
             url = $_HOSTNAME + "/Secure/SARC/Print.aspx?refId=" + id;
             break;
         case "SC_FT":
             url = $_HOSTNAME + "/secure/SC_FastTrack/Print.aspx?refId=" + id;
             break;
         case "SC_WD":
             url = $_HOSTNAME + "/secure/SC_WWD/Print.aspx?refId=" + id;
             break;
     }

     showPopup({
         Url: url,
         Center: true,
         Width: 900,
         Height: 800,
         Resizable: true
     });
 }

function printForm(casetype, id, form) {

    var url = $_HOSTNAME;

    switch(casetype){
        case "lod":
            if (arguments.length == 2) {
                url = url + "/Secure/lod/print.aspx?id=" + id;
            }
            else if (arguments.length == 3) {
                url = url + "/Secure/lod/print.aspx?id=" + id + "&form=" + form;
            }
            break;
        case "reinvestigation":
            if (arguments.length == 2) {
                url = url + "/Secure/ReinvestigationRequests/print.aspx?id=" + id;
            }
            else if (arguments.length == 3) {
                url = url + "/Secure/ReinvestigationRequests/print.aspx?id=" + id + "&form=" + form;
            }
    }

    showPopup({
        'Url': url,
        'Width': '690',
        'Height': '700',
        'Resizable': true,
        'Center': true,
        'Reload': false
    });

}
 