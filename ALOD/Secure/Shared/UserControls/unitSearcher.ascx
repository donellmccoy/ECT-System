<%@ Control Language="VB" AutoEventWireup="false"
    Inherits="ALOD.Web.UserControls.Secure_Shared_UserControls_unitSearcher" Codebehind="unitSearcher.ascx.vb" %>

<script type="text/javascript">
 
    function initializeUnitSearcher() {
        element('<%= lblMessage.ClientId %>').innerHTML = '';
        $('#lstResults > option').remove();
    }

    function setSelection(val, name) {
        //Intermittently save the selected unit Id and name
        //If the client cancels the dialog these values are ignored
        //If the client accepts then these values update the client values      
        element('<%=hdnSelectedUnit.ClientId %>').value = val;
        element('<%=hdnSelectedUnitName.ClientId %>').value = name;
    }


    function getData() {

        var description = element('txtDescription');
        var pascode = element('txtPasCode');
        var activeOnly = element('<%=hdnActiveOnly.ClientId %>').value; 

        if (pascode.value.length < 1 && description.value.length < 1) {
            pascode.focus();

            $(description).addClass('ui-state-error');
            $(pascode).addClass('ui-state-error');

            return false;
        }

        $(description).removeClass('ui-state-error');
        $(pascode).removeClass('ui-state-error');

        //block the list box		
        //$('#modalBody').block();

        //clear our existing options
        $('#lstResults > option').remove();

        //get search results
        if ($_HOSTNAME == '') $_HOSTNAME = 'http://localhost:44300';
        $.ajax({
            type: 'POST',
            url: $_HOSTNAME + '/Public/PublicDataService.asmx/GetUnits',
            data: 'p=' + pascode.value + '&descr=' + description.value + '&active=' + activeOnly,
            dataType: 'xml',
            error: function(obj, errorText, exc) {
                element('<%= lblMessage.ClientId %>').innerHTML = 'No match found';
                $('div#modalBody').unblock();
            },
            success: setResults
        });

    }

    //ajax results come back as xml responseXML is sent to the setResults function         
    function setResults(response) {
        //loop over each Diagnosis element
        element('<%= lblMessage.ClientId %>').innerHTML = ' ';

        $(response).find('Unit').each(function() {
            //output it to the table

           // var child = $.create("option", { "value": $('CS_ID', this).text().trim(), "innerHTML": $('LONG_NAME', this).text().trim() }, []);

           // $('#lstResults').append(child);
            $('#lstResults').append($("<option></option>").attr("value", $('CS_ID', this).text().trim()).text($('LONG_NAME', this).text().trim()));

        });  //End each function 

        // $('div#modalBody').unblock();

    }  
</script>


    <fieldset>
        <label for="txtPasCode">
            PAS Code</label>
        <input type="text" id="txtPasCode" onkeypress="return checkFormat(this,event,&#39;AlphaNumeric&#39;,&#39;`,~,!,@,$,%,*,(,),-,_,+,\&#39;,&quot;,.,,,?,/,\\,|,:, &#39;);" style="width: 70px;" />
        
        <label for="txtDescription">
            Description</label>
        <input type="text" id="txtDescription" onkeypress="return checkFormat(this,event,&#39;AlphaNumeric&#39;,&#39;`,~,!,@,$,%,*,(,),-,_,+,\&#39;,&quot;,.,,,?,/,\\,|,:, &#39;);" style="width: 150px;" />&nbsp;
        <button id="cmdSearch" onclick="getData();">
            Search</button>
    </fieldset>
    <p>
        <asp:Label ID="lblMessage" runat="Server"></asp:Label><br />
        <select id="lstResults" class="srchlist" size="10"
            onchange="setSelection(this.options[this.selectedIndex].value,this.options[this.selectedIndex].innerHTML);">
        </select>
    </p>
    <input type="hidden" id="hdnSelectedUnit" runat="Server" />
    <input type="hidden" id="hdnSelectedUnitName" runat="Server" />
     <input type="hidden" id="hdnActiveOnly" runat="Server" />

      