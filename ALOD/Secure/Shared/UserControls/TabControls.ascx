<%@ Control Language="VB" AutoEventWireup="false" Inherits="ALOD.Web.UserControls.TabControls" Codebehind="TabControls.ascx.vb" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>

<div class="tabControls">
    <div class="hidden">
        <asp:Button runat="server" ID="btnCommand" CssClass="hidden" />
        <asp:TextBox runat="server" ID="txtCommand" CssClass="hidden" />
    </div>
    <div style="float: left; width: 75%; text-align: left">
        <asp:Button runat="server" ID="btnSave" Width="62px" Text="Save" />&nbsp;&nbsp;&nbsp;
        <asp:Button runat="server" ID="btnPrint" Width="62px" Text="Print" />&nbsp;&nbsp;&nbsp;
        <asp:Button runat="server" ID="btnDelete" Width="62px" Text="Delete" />
    </div>
    <div style="float: right; width: 24%; text-align: right">
        <asp:Button runat="server" ID="btnBack" Width="75px" Text="Previous" />&nbsp;&nbsp;&nbsp;
        <asp:Button runat="server" ID="btnNext" Width="75px" Text="Next" />&nbsp;&nbsp;&nbsp;
    </div>
</div>
