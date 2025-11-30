<%@ Page Title="" Language="C#" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Help.Secure_help_Documentation" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" CodeBehind="Documentation.aspx.cs" %>

<%@ Register Src="~/Secure/Shared/UserControls/EditKeyValDialog.ascx" TagName="EditKeyVal" TagPrefix="uc1" %>
<%@ Register Src="~/Secure/Shared/UserControls/AddKeyValDialog.ascx" TagName="AddKeyVal" TagPrefix="uc1" %>

<%@ Import Namespace="ALODWebUtility.Common" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
	<style type="text/css">
		#docs li
		{
			margin-bottom: 6px;
		}
		.iconImg
		{
			padding: 2px;
			margin-right: 6px;
		}
	</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="Server">
	<div id="wizard" style="border-top-width: 1px;">
		<div id="EditDocument" class="dataBlock" title="Edit Document Details" style="display: none;
			margin-bottom: 0px; background-color: #EEE;">
			<table>
				<tr>
					<td class="number">
						1
					</td>
					<td class="label-small">
						Category:
					</td>
					<td class="value">
						<asp:DropDownList runat="server" ID="DocumentTypeSelect" DataTextField="CategoryDescription" DataValueField="DocCatId" style="max-width:300px;">
						</asp:DropDownList>
					</td>
				</tr>
				<tr>
					<td class="number">
						2
					</td>
					<td class="label-small">
						Date:
					</td>
					<td class="value">
						<asp:TextBox runat="server" ID="DocumentDate" MaxLength="12" Width="100px" CssClass="datePickerToToday" />
					</td>
				</tr>
				<tr>
					<td class="number">
						3
					</td>
					<td class="label-small">
						Description:
					</td>
					<td class="value">
						<asp:TextBox runat="server" ID="DocumentDescription" Width="200px" MaxLength="50" onkeypress="return checkFormat(this,event,&#39;AlphaNumeric&#39;,&#39;`,~,!,@,$,%,*,(,),-,_,+,\&#39;,&quot;,.,,,?,/,\\,|,:, &#39;);" />
					</td>
				</tr>
			</table>
		</div>
		<asp:Button runat="server" ID="UpdateDocumentButton" CssClass="hidden" />
		<asp:TextBox runat="server" ID="DocumentEditValues" CssClass="hidden" />
		<asp:Button runat="server" ID="RefreshButton" CssClass="hidden" />
		<div>
			<uc1:EditKeyVal ID="ucEditKeyVal" runat="server" />
		</div>
		<br />
		<div>
			<uc1:AddKeyVal ID="ucAddKeyVal" runat="server" />
		</div>
		<asp:Repeater runat="server" ID="CategoryRepeater">
			<SeparatorTemplate>
				<p>
					&nbsp;
				</p>
			</SeparatorTemplate>
			<ItemTemplate>
				<div id='parentRow' runat="Server">
					<!--Start of Parent rows Table -->
					<table class="docHeader">
						<tr>
							<td style="width: 596px;">
								<%#Container.ItemIndex + 1%> .&nbsp;<%#Server.HtmlEncode(Eval("CategoryDescription"))%>
							</td>
							<td style="width: 196px;">
								<asp:Label runat="server" ID="lblStatus" />
							</td>
							<td style="width: 196px;">
								<%--<asp:Label runat="server" ID="lblReqd" CssClass="labelRequired" Font-Bold="true" />--%>
							</td>
							<td style="width: 52px; text-align: right;">
								<asp:Image runat="server" ID="UploadImage" ImageAlign="AbsMiddle" ImageUrl="~/images/upload.gif"
									CssClass="iconUpload" />
							</td>
						</tr>
					</table>
				
					<!--Start of ChildRows Table -->
					<asp:Repeater ID="ChildDocRepeater" runat="server" OnItemDataBound="ChildDataBound" OnItemCommand="Document_ItemCommand">
						<ItemTemplate>
							<table class="docDetails">
								<tr class="docRow">
									<td style="text-align: left; width: 440px;">
										<asp:HyperLink runat="server" ID="ViewDocLink" NavigateUrl="#">
											<asp:Image runat="server" ID="Icon" ImageUrl='<%# Eval("IconUrl") %>' CssClass="iconImg"
												ImageAlign="AbsMiddle" />
										
											<%# IIf(String.IsNullOrEmpty(DataBinder.Eval(Container.DataItem, "Description")), Server.HtmlEncode(Eval("DocTypeName")) + "_" + Server.HtmlEncode(Eval("UploadedBy")) + "_" + Server.HtmlEncode(Eval("DocDate", "{0:" + DATE_FORMAT + "}")), Server.HtmlDecode(Eval("Description")))%>
										</asp:HyperLink>
									</td>
									<td style="text-align: center; width: 100px;">
										<%#Server.HtmlEncode(Eval("DocDate", "{0:" + DATE_FORMAT + "}"))%>
									</td>
									<td style="text-align: left; width: 160px;">
										<%#Server.HtmlEncode(Eval("UploadedBy"))%>
									</td>
									<td style="text-align: right; width: 200px;">
										<asp:Image runat="server" ID="EditDocument" ImageAlign="AbsMiddle" AlternateText="Edit Document"
											SkinID="imgEditSmall" CssClass="pointer" />
										&nbsp; <asp:ImageButton runat="server" ID="DeleteDocument" ImageAlign="AbsMiddle" AlternateText="Delete Document"
											SkinID="buttonDelete" CssClass="pointer" CommandName="DeleteDocument" CommandArgument='<%# Eval("Id").ToString() + "|" + Eval("Description") %>' />
										<ajax:ConfirmButtonExtender runat="server" ID="ConfirmDelete" TargetControlID="DeleteDocument"
											ConfirmText="Are you sure you want to delete this document?" />
										<asp:Image runat="server" ID="LockedDocument" SkinID="imgLocked" AlternateText="Document is Read-Only"
											Visible="false" />
									</td>
								</tr>
							</table>
						</ItemTemplate>
					</asp:Repeater>
					<!--End of ChildRows Table -->
				</div>
			</ItemTemplate>
		</asp:Repeater>
		<p>
			&nbsp;
		</p>
		<div id="docs">
			<!-- COMPUTER BASED TRAINING LINKS TABLE -->
			<table class="docHeader">
				<tr>
					<td style="width: 596px;">
						<%--<%#Container.ItemIndex + 1%> .&nbsp;<%#Server.HtmlEncode(Eval("CategoryDescription"))%>--%>
						4 . Computer Based Training (CBTs)
					</td>
					<td style="width: 196px;">
					</td>
					<td style="width: 196px;">
					</td>
					<td style="width: 52px; text-align: right;">
						<asp:Image runat="server" ID="imgUploadVidKeyVal" ImageAlign="AbsMiddle" ImageUrl="~/images/upload.gif" CssClass="iconUpload" />
					</td>
				</tr>
			</table>
			<asp:Repeater ID="VideoRepeater" runat="server" OnItemDataBound="VideoDataBound" OnItemCommand="KeyVal_ItemCommand">
				<ItemTemplate>
					<table class="docDetails">
						<tr class="docRow">
							<td style="text-align: left; width: 440px;">
								<asp:HyperLink runat="server" ID="ViewVidLink" NavigateUrl='<%# Eval("Value")%>'>
									<asp:Image runat="server" ID="Icon" ImageUrl="~/images/wmv_icon.gif" CssClass="iconImg" ImageAlign="AbsMiddle" />

									<%# Eval("ValueDescription")%>
								</asp:HyperLink>
							</td>
							<td style="text-align: center; width: 100px;">
							</td>
							<td style="text-align: left; width: 160px;">
							</td>
							<td style="text-align: right; width: 200px;">
								<asp:Image runat="server" ID="EditVideo" ImageAlign="AbsMiddle" AlternateText="Edit Video"
									SkinID="imgEditSmall" CssClass="pointer" />
								&nbsp; 
								<asp:ImageButton runat="server" ID="DeleteVideo" ImageAlign="AbsMiddle" AlternateText="Delete Video"
									SkinID="buttonDelete" CssClass="pointer" CommandName="DeleteKeyVal" CommandArgument='<%# Eval("Id").ToString() + "|" + Eval("ValueDescription")%>' />
								<ajax:ConfirmButtonExtender runat="server" ID="ConfirmDelete" TargetControlID="DeleteVideo"
									ConfirmText="Are you sure you want to delete this video link? NOTE: This will only delete the link to the video, but not the video file itself." />
							</td>
						</tr>
					</table>
				</ItemTemplate>
			</asp:Repeater>
			<p>
				&nbsp;
			</p>
			<!-- MISCELLANEOUS LINKS TABLE -->
			<table class="docHeader">
				<tr>
					<td style="width: 596px;">
						<%--<%#Container.ItemIndex + 1%> .&nbsp;<%#Server.HtmlEncode(Eval("CategoryDescription"))%>--%>
						5 . Miscellaneous Links
					</td>
					<td style="width: 196px;">
					</td>
					<td style="width: 196px;">
					</td>
					<td style="width: 52px; text-align: right;">
						<asp:Image runat="server" ID="imgUploadMiscLinkValue" ImageAlign="AbsMiddle" ImageUrl="~/images/upload.gif" CssClass="iconUpload" />
					</td>
				</tr>
			</table>
			<asp:Repeater ID="MiscLinksRepeater" runat="server" OnItemDataBound="MiscLinksDataBound" OnItemCommand="KeyVal_ItemCommand">
				<ItemTemplate>
					<table class="docDetails">
						<tr class="docRow">
							<td style="text-align: left; width: 440px;">
								<asp:HyperLink runat="server" ID="ViewMiscLink" Target="_blank" NavigateUrl='<%# Eval("Value")%>'>
									<asp:Image runat="server" ID="Icon" ImageUrl="~/images/hyperlink.gif" CssClass="iconImg" ImageAlign="AbsMiddle" />

									<%# Eval("ValueDescription")%>
								</asp:HyperLink>
							</td>
							<td style="text-align: center; width: 100px;">
							</td>
							<td style="text-align: left; width: 160px;">
							</td>
							<td style="text-align: right; width: 200px;">
								<asp:Image runat="server" ID="EditMiscLink" ImageAlign="AbsMiddle" AlternateText="Edit Misc Link"
									SkinID="imgEditSmall" CssClass="pointer" />
								&nbsp; 
								<asp:ImageButton runat="server" ID="DeleteMiscLink" ImageAlign="AbsMiddle" AlternateText="Delete Video"
									SkinID="buttonDelete" CssClass="pointer" CommandName="DeleteKeyVal" CommandArgument='<%# Eval("Id").ToString() + "|" + Eval("ValueDescription")%>' />
								<ajax:ConfirmButtonExtender runat="server" ID="ConfirmDelete" TargetControlID="DeleteMiscLink"
									ConfirmText="Are you sure you want to delete this link?" />
							</td>
						</tr>
					</table>
				</ItemTemplate>
			</asp:Repeater>
			<p>
				&nbsp;
			</p>
		</div>
	</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFooter" runat="Server">
	<script type="text/javascript">

		var activeDocId = 0;

		function editDocument(docId, category, docDate, description) {

			activeDocId = docId;
	   
			setSelectedValue('<%= DocumentTypeSelect.ClientId %>', category);
			element('<%= DocumentDate.ClientId %>').value = docDate;
			element('<%= DocumentDescription.ClientId %>').value = description;

			$('#EditDocument').dialog('open');
		}

		function serializeEditor() {

			var values = String(activeDocId) + "|"
				+ getSelectedValue('<%= DocumentTypeSelect.ClientId %>') + "|"
				+ element('<%= DocumentDate.ClientId %>').value + "|"
				+ element('<%= DocumentDescription.ClientId %>').value;

			element('<%= DocumentEditValues.ClientId %>').value = values;
		}
        /* $(document).ready(function () {Deprecated ready Syntax: -- Diamante Lawson 10/12/2023 */

		(function() {


			//prepare the edit document dialog
			$('#EditDocument').dialog({
				autoOpen: false,
				modal: false,
				resizable: false,
				width: 500,
				height: 250,
				autoResize: true,
				buttons: {
					'Cancel': function() {
						$(this).dialog('close');

					},
					'Update': function() {
						serializeEditor();
						$(this).dialog('close');
						element('<%= UpdateDocumentButton.ClientId %>').click();
					}
				}

			});

			//date pickers
		    $('.datePickerToToday').datepicker(calendarPick("DocPast", "<%=CalendarImage %>"));
		    $('.datePicker').datepicker(calendarPick("DocAll", "<%=CalendarImage %>"));

		});

		//Opens up the  document URL  in a dailog window
		function uploadDoc(url) {
			showPopup({
				'Url': url,
				'Width': 500,
				'Height': 300,
				'Center': true,
				'Reload': true,
				'Resizable': true,
				'ScrollBars': true,
				ReloadButton: element('<%= RefreshButton.ClientId %>')
			});
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

		function confirmDelete() {
			return confirm("This will permanently delete this document.  Are you sure you want to proceed?");
		}

		function confirmKeyValDelete() {
			return confirm("This will permanently delete this Key Value.  Are you sure you want to proceed?");
		}
	
    </script>
</asp:Content>
