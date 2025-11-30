<%@ Control Language="VB" AutoEventWireup="false" Inherits="ALOD.Web.UserControls.EditKeyValDialog" CodeBehind="EditKeyValDialog.ascx.vb" %>

<script type="text/javascript">

	var activeValId = 0;

	function editKeyVal(valId, keyId, valDescription, value) {

		activeValId = valId;

		setSelectedValue('<%= ddlKeySelect.ClientID%>', keyId);
		element('<%= txtValueDescription.ClientID%>').value = valDescription;
		element('<%= txtValue.ClientID%>').value = value;

		$('#EditKeyVal').dialog('open');
	}

	function serializeKeyValEditEditor() {

		var values = String(activeValId) + "|"
			+ getSelectedValue('<%= ddlKeySelect.ClientID%>') + "|"
			+ element('<%= txtValueDescription.ClientID%>').value + "|"
			+ element('<%= txtValue.ClientID%>').value;

		element('<%= txtKeyValEditValues.ClientID%>').value = values;
	}

	function isEditInputValid() {
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

	(function () {

		//prepare the edit keyval dialog
		$('#EditKeyVal').dialog({
			autoOpen: false,
			modal: false,
			resizable: false,
			width: 400,
			autoResize: true,
			buttons: {
				'Cancel': function () {
					$(this).dialog('close');

				},
				'Update': function () {
					if (isEditInputValid()) {
						serializeKeyValEditEditor();
						$(this).dialog('close');
						element('<%= btnUpdateKeyVal.ClientID%>').click();
					}
				}
			}

		});

	});

</script>

<div id="EditKeyVal" class="dataBlock" title="Edit KeyVal Details" style="display: none; margin-bottom: 0px; background-color: #EEE;">
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
<asp:Button runat="server" ID="btnUpdateKeyVal" CssClass="hidden" />
<asp:TextBox runat="server" ID="txtKeyValEditValues" CssClass="hidden" />