


/*=======================================
Shows 'Who is working this case' info
=======================================*/

function showWorkflowUsers (refId, moduleId, wsStatus, MemberUnitId) {

     
          //start with a clean popup
    $('#whoisWorking').remove();
    
     //create the main div
    var parent = $.create(

        "DIV", 
        {   
            "id" : "whoisWorking", 
            //"class" : "modalPanel jqmDialog",
            //"style": "width: 300px; height: 300px; z-index: 200;",
            "title" : "Current Users"
            
        }, 
        [
            "P", 
            {
                "id" : "dvLoading", 
                "class" : "labelWait"
                //"style" : "width: 296px; text-align: center; margin-top: 100px; margin-bottom: 10px; border-width: 0px;"
            },
            [
                "IMG", {"src" : $_HOSTNAME + "/images/busy.gif", "alt" : "Loading"}, [],
                "span", {}, [" Please Wait..."]
            ]
        ]
        
    );

    $('body').append(parent);
    
    $.showDialog('#whoisWorking', { maxHeight: 366 });
    //$.showDialog('#whoisWorking', false, 200);
        
    $.ajax ({
        type: 'POST',
        url: $_HOSTNAME + '/Secure/Utility/DataService.asmx/GetWorkflowUsers',
        data: 'refId=' + refId + "&moduleId=" + moduleId + "&wsStatus=" + wsStatus + "&MemberUnitId=" + MemberUnitId,
        dataType: 'xml',
        success: showWhoisWorking
    });
}


function showWhoisWorking (xml)
{
     
    $('#dvLoading').remove();
    
    var body = $.create(
        "P", 
        {   
            "id" : "whoisBody"
            //"class" : "modalBody",
            //"style" : "width: 292px; overflow: auto; height: 268px;"
        },
        [
            "P", {}, //{"style" : "width: 274px; font-weight: bold; border-bottom: 1px solid #C0C0C0;"}, 
            ['This case is awaiting action by one of:']
        ]
    );
        var nMembers = 0;
    $('#whoisWorking').append(body);

    $(xml).find('User').each(function() {
        var node = $(this);

        nMembers = nMembers + 1;

        var table = $.create(
            "P", { "style": "padding: 6px; font-size: 12px; border-bottom: 1px solid #C0C0C0;" },
            [
                "TABLE", { "cellpadding": "2", "cellspacing": "0" },
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
                        ]

                    ]
                ]
            ]
        );

        $('#whoisBody').append(table);



    });      //end .each function


    if (nMembers == 0) {
        
        $('#whoisBody').append(
            $.create("P" ,{},
                      [
                         "P", {},   
                         ['No users found for the current status']
                      ]
                )
          );

    }
    
}

function hideDialog()
{
 //   $('#whoisWorking').jqm().jqmHide();
 //   $('#whoisWorking').remove();
}



