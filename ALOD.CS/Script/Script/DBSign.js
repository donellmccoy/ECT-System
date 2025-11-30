
function showDbSignInfo(xml) {

    $('#dvDbSignLoading').remove();

    var node = $(xml).find('User');

    var name = node.attr('Name') !== null ? node.attr('Name') : new String();
    var date = node.attr('DateSigned') !== null ? node.attr('DateSigned') : new String();

    var table = $.create(
        "p", { "style": "padding: 6px; font-size: 12px;" },
        [
            "TABLE", { "cellpadding": "2", "cellspacing": "2" },
            [
                "TBODY", {},
                [
                    "TR", {},
                    [
                        "TD", { "style": "font-weight: bold; text-align: right;" }, ["Signed By:"],
                        "TD", {},
                        [
                            name
                        ]
                    ],
                    "TR", {},
                    [
                        "TD", { "style": "font-weight: bold; text-align: right;" }, ["Signed On:"],
                        "TD", {},
                        [
                            date
                        ]
                    ]
                ]
            ]
        ]
    );

    $('#dbsigninfo').append(table);
}

function hideDialog() {
    $('#dbsigninfo').jqm().jqmHide();
    $('#dbsigninfo').remove();
}

/* Whois popup */
function getDbSignInfo(refId, secId, template) {
    //start with a clean popup
    $('#dbsigninfo').remove();

    //create the main div
    var parent = $.create(

        "DIV",
        {
            "id": "dbsigninfo",
            "title": "Signature Information"
        },
        [
            "P",
            {
                "id": "dvDbSignLoading",
                "class": "labelWait"
            },
            [
                "IMG", { "src": $_HOSTNAME + "/images/busy.gif", "alt": "Loading" }, [],
                "span", {}, ["Loading..."]
            ]
        ]

    );

    $('body').append(parent);

    $.showDialog('#dbsigninfo', { draggable: true, resizable: true, position: 'center', modal: true });

    $.ajax({
        type: 'POST',
        url: $_HOSTNAME + '/Secure/Utility/DataService.asmx/GetDbSignInfo',
        data: 'refId=' + refId + "&secId=" + secId + "&template=" + template,
        dataType: 'xml',
        success: showDbSignInfo
    });
}





