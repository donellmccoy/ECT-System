<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="PHLargeTextBlock.ascx.vb" Inherits="ALOD.Web.UserControls.UIBuildingBlocks.PHLargeTextBlock" %>

<asp:Label runat="server" ID="lblScreenReaderText" AssociatedControlID="txtLongText" CssClass="visuallyHidden" />
<asp:TextBox runat="server" ID="txtLongText" TextMode="MultiLine" Rows="3" />
<asp:Label runat="server" ID="lblLongText" CssClass="fieldWordWrapLabel" Visible="false" />