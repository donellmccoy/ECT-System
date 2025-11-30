<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="PHMultipleOptionsBlock.ascx.vb" Inherits="ALOD.Web.UserControls.UIBuildingBlocks.PHMultipleOptionsBlock" %>

<asp:Label runat="server" ID="lblScreenReaderText" AssociatedControlID="lsbMultipleOptions" CssClass="visuallyHidden" />
<asp:ListBox runat="server" ID="lsbMultipleOptions" SelectionMode="Multiple" Rows="3" />
<asp:Label runat="server" ID="lblMultipleOptions" Visible="false" />