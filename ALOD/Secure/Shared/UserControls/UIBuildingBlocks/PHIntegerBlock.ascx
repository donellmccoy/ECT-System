<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="PHIntegerBlock.ascx.vb" Inherits="ALOD.Web.UserControls.UIBuildingBlocks.PHIntegerBlock" %>

<asp:Label runat="server" ID="lblScreenReaderText" AssociatedControlID="txtInteger" CssClass="visuallyHidden" />
<asp:TextBox runat="server" ID="txtInteger" />