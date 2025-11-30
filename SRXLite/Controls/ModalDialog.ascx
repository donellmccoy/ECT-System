<%@ Control Language="VB" AutoEventWireup="false" Inherits="SRXLite.Web.Controls.Controls_ModalDialog" Codebehind="ModalDialog.ascx.vb" %>
<input id="hTargetCtrl" runat="server" type="hidden" />
<asp:Button ID="btnShowModalDialog" runat="server" style="display:none" />
<asp:Panel id="pnlContent" runat="server" CssClass="dialogContent" style="display:none; overflow:auto">
	<div id="divContent" runat="server">
		<table width="100%" cellpadding="0" cellspacing="0" style="margin-bottom:10px">
			<tr>
				<td><h1 id="ModalDialogTitle" runat="server" class="title" style="color:white"></h1></td>
				<td align="right"><asp:Image ID="imgClose" runat="server" ImageUrl="~/images/close.jpg" AlternateText="Close" /></td>
			</tr>
		</table>
		<asp:PlaceHolder id="phContent" runat="server">
		</asp:PlaceHolder>
	</div>
</asp:Panel>
<ajaxToolkit:DropShadowExtender ID="DropShadowExtender1" runat="server" TargetControlID="pnlContent" Opacity=".3" Rounded="false" TrackPosition="true" />
<ajaxToolkit:ModalPopupExtender ID="ModalPopupExtender1" runat="server" PopupControlID="pnlContent" TargetControlID="hTargetCtrl" BackgroundCssClass="modalBackground" RepositionMode="RepositionOnWindowResizeAndScroll" />