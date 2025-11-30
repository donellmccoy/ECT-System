
/* scripts for showing changelog */

function showChangeSet(logId, userId)
{    

    //start with a fresh dom
    $('#changeSet').remove();      
    
    //create the main div
    var parent = $.create(
        "DIV", 
        {   
            "id" : "changeSet" 
            //,"class" : "modalPanel jqmDialog"
            ,"style" : "height: 400px; width: 600px;"
        }, 
        [
            "DIV", 
            {
                "id" : "dvLoading", 
                "class" : "labelWait"
                //,"style" : "width: 596px; text-align: center; margin-top: 140px; border-width: 0px;"
            },
            [
                "IMG", {"src" : $_HOSTNAME + "/images/busy.gif", "alt" : "Loading"}, [],
                "span", {}, [" Loading..."]
            ]
        ]
    );

    $('body').append(parent);
 
    //show our dialog
    $.showDialog('#changeSet', { modal: true, position: 'center', width: 600, height: 400, title: 'Change Log' });
    
    //now get the contents from the webservice
    $.ajax ({
        type: 'POST',
        url: $_HOSTNAME + '/Secure/Utility/DataService.asmx/GetChangeSet',
        data: 'logId=' + logId + "&userId=" + userId ,
        dataType: 'xml',
        success: showResults 
    });
}


function showResults(response)
{
    $('#dvLoading').remove();
    
    //process the response from the webservice call
    $(response).find('ChangeSet').each(function() {

    //height: 380px; width: 596px; 
        //create the header info
        var header = $.create(
            "DIV", {"id" : "dvBody", "style" : "overflow: auto; padding-left: 4px;"},
            [
                "TABLE", {"cellpadding" : "2"},
                [
                    "TBODY", {},
                    [
                        "TR", {},
                        [
                            "TD", {"style" : "text-align: right; font-weight: bold;"}, ["Date:"],
                            "TD", {}, [$(this).attr('ActionDate')],
                            "TD", {"style" : "text-align: right; font-weight: bold;"}, ["User:"],
                            "TD", {}, 
                            [
                                $(this).attr('Rank') + ' ' + $(this).attr('LastName') + ", " + $(this).attr('FirstName')
                            ]
                        ]
                    ]
                ]
            ]
        );
        
        $('#changeSet').append(header);
        
        //create the table
        var table = $.create(
            
                "TABLE", {"cellpadding" : "0", "cellspacing" : "0"}, 
                [
                    "THEAD", {}, 
                    [
                        "TR", {"class" : "gridViewHeader", "style" : "height: 20px;"}, 
                        [
                            "TD", {"style" : "width: 200px;"}, ["Section"],
                            "TD", {"style" : "width: 120px;"}, ["Field"],
                            "TD", {"style" : "width: 140px;"}, ["Old Value"],
                            "TD", {"style" : "width: 140px;"}, ["New Value"]
                        ]
                    ],
                    "TBODY", {"id" : "changeRows"}, 
                    [
                    ]
                ]
            
        );
        
        $('#dvBody').append(table);
        
        //add the rows
        var i = 0;
        $(this).find("ChangeRow").each(function() {
        
            var row = $.create(
                "TR", {"class" : i++ % 2 === 0 ? "gridViewRow" : "gridViewAlternateRow"},
                [
                    "TD", {}, [ $(this).attr("Section") ],
                    "TD", {}, [ $(this).attr("Field") ],
                    "TD", {}, [ $(this).attr("OldValue") ],
                    "TD", {}, [ $(this).attr("NewValue") ]
                ]
            );
            
            $('#changeRows').append(row);
        });
    });
    
    
}
