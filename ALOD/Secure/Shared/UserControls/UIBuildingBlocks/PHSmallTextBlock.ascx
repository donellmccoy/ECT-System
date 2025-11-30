<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="PHSmallTextBlock.ascx.vb" Inherits="ALOD.Web.UserControls.UIBuildingBlocks.PHSmallTextBlock" %>

<asp:Label runat="server" ID="lblScreenReaderText" AssociatedControlID="txtShortText" CssClass="visuallyHidden" />
<asp:TextBox runat="server" ID="txtShortText" />
<asp:Label runat="server" ID="lblShortText" CssClass="fieldWordWrapLabel" Visible="false" />