<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="PHCSVTextBlock.ascx.vb" Inherits="ALOD.Web.UserControls.UIBuildingBlocks.PHCSVTextBlock" %>

<asp:Label runat="server" ID="lblScreenReaderText" AssociatedControlID="txtCSV" CssClass="visuallyHidden" />
<asp:TextBox runat="server" ID="txtCSV" TextMode="MultiLine" Rows="3" />
<asp:Label runat="server" ID="lblCSV" Visible="false"  />
