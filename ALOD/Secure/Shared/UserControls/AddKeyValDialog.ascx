<%@ Control Language="VB" AutoEventWireup="false" Inherits="ALOD.Web.UserControls.AddKeyValDialog" CodeBehind="AddKeyValDialog.ascx.vb" %>

<script type="text/javascript">

	function addKeyVal(keyId) {

		setSelectedValue('<%= ddlKeySelect.ClientID%>', keyId);

		$('#AddKeyVal').dialog('open');
	}

	function serializeKeyValAddEditor() {

		var values = getSelectedValue('<%= ddlKeySelect.ClientID%>') + "|"
			+ element('<%= txtValueDescription.ClientID%>').value + "|"
			+ element('<%= txtValue.ClientID%>').value;

		element('<%= txtKeyValAddValues.ClientID%>').value = values;
	}

	function isAddInputValid() {
		var isValid = true;

		if (isStringNullOrEmpty(element('<%= txtValueDescription.ClientID%>').value) == true) {

			$('#<%= txtValueDescription.ClientID%>').addClass('fieldRequired');
			isValid = false;
		}
		else {

			$('#<%= txtValueDescription.ClientID%>').removeClass('fieldRequired');
		}

		if (isStringNullOrEmpty(element('<%= txtValue.ClientID%>').value) == true) {

			$('#<%= txtValue.ClientID%>').addClass('fieldRequired');
			isValid = false;
		}
		else {

			$('#<%= txtValue.ClientID%>').removeClass('fieldRequired');
		}

		return isValid;
	}


    $(function () {
		
		//prepare the add keyval dialog
		$('#AddKeyVal').dialog({
			autoOpen: false,
			modal: false,
			resizable: false,
			width: 400,
			autoResize: true,
			buttons: {
				'Cancel': function () {
					$(this).dialog('close');

				},
				'Add': function () {
					if (isAddInputValid()) {
						serializeKeyValAddEditor();
						$(this).dialog('close');
						element('<%= btnAddKeyVal.ClientID%>').click();
					}
				}
			}

		});

	});

</script>

<div id="AddKeyVal" class="dataBlock" title="Add KeyVal Details" style="display: none; margin-bottom: 0px; background-color: #EEE;">
	<table>
		<tr>
			<td class="number">
				1
			</td>
			<td class="label-small">
				Key:
			</td>
			<td class="value">
				<asp:DropDownList runat="server" ID="ddlKeySelect">
				</asp:DropDownList>
			</td>
		</tr>
		<tr>
			<td class="number">
				2
			</td>
			<td class="label-small">
				Value Description:
			</td>
			<td class="value">
				<asp:TextBox runat="server" ID="txtValueDescription" Width="200px" MaxLength="50" onkeypress="return checkFormat(this,event,&#39;AlphaNumeric&#39;,&#39;`,~,!,@,$,%,*,(,),-,_,+,\&#39;,&quot;,.,,,?,/,\\,|,:, &#39;);" />
			</td>
		</tr>
		<tr>
			<td class="number">
				3
			</td>
			<td class="label-small">
				Value:
			</td>
			<td class="value">
				<asp:TextBox runat="server" ID="txtValue" Width="200px" MaxLength="8000" onkeypress="return checkFormat(this,event,&#39;AlphaNumeric&#39;,&#39;`,~,!,@,$,%,*,(,),-,_,+,\&#39;,&quot;,.,,,?,/,\\,|,:, &#39;);" />
			</td>
		</tr>
	</table>
</div>
<asp:Button runat="server" ID="btnAddKeyVal" CssClass="hidden" />
<asp:TextBox runat="server" ID="txtKeyValAddValues" CssClass="hidden" />