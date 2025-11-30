<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="PHOptionBlock.ascx.vb" Inherits="ALOD.Web.UserControls.UIBuildingBlocks.PHOptionBlock" %>

<asp:Label runat="server" ID="lblScreenReaderText" AssociatedControlID="ddlOptions" CssClass="visuallyHidden" />
<asp:DropDownList runat="server" ID="ddlOptions" />
<asp:Label runat="server" ID="lblOptions" Visible="false" />