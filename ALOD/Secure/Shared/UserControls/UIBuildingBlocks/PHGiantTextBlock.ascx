<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="PHGiantTextBlock.ascx.vb" Inherits="ALOD.Web.UserControls.UIBuildingBlocks.PHGiantTextBlock" %>

<asp:Label runat="server" ID="lblScreenReaderText" AssociatedControlID="txtGiantText" CssClass="visuallyHidden" />
<asp:TextBox runat="server" ID="txtGiantText" TextMode="MultiLine" Rows="5" />
<asp:Label runat="server" ID="lblGiantText" CssClass="fieldWordWrapLabel" Visible="false" />