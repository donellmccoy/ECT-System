// Session timeout dialog

var timerID;
var timerMoveID;
var sec;
var $_TD_CONTINUE_BUTTON = "your_button_id"; // Replace 'your_button_id' with the actual ID of the button element
var $_HOSTNAME; // Reference the hostname variable
var $_TD_DELAY = 60; // Replace 60 with the desired value in seconds
var $_TD_MS_TIMEOUT = 60000; // Replace 60000 with the desired timeout duration in milliseconds

// ... rest of your JavaScript code ...

$(function () {
    $("#timeoutDialog").dialog({
        autoOpen: false,
        modal: true,
        resizable: true,
        width: 400,
        height: 220,
        closeOnEscape: false,
        center: true,
        title: "Session Expiration",
        close: function () {
            $("#" + $_TD_CONTINUE_BUTTON).on("click", function () {
                // Your click event handler code here
            });
        },
        buttons: {
            'Continue': function () {
                $(this).dialog('close');
            }
        }

    });

    startTimer();
});
function TimerCheck() {
    sec -= 1;
    if (sec <= 0) {
        // Timed out
        $("#timeoutDialog").dialog("destroy");
        clearInterval(timerID);
        window.location.href = $_HOSTNAME + "/public/logout.aspx";
    } else {
        $("#TimeLeft").html("You have " + Math.floor(sec / 60) + " minutes and " + (sec % 60) + " seconds until expiration.");

    }
}

function startTimer() {
    if (timerID > 0) {
        window.clearInterval(timerID);
    }
    if (timerMoveID > 0) {
        window.clearInterval(timerMoveID);
    }
    window.setTimeout(function () {
        $("#timeoutDialog").dialog("open");
        sec = $_TD_DELAY; // Replace with your desired delay value in seconds
        timerID = window.setInterval(TimerCheck, 1000);
    }, $_TD_MS_TIMEOUT); // Set to your desired timeout duration in milliseconds
}
