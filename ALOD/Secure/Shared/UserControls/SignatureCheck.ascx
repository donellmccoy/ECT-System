<%@ Control Language="VB" AutoEventWireup="false"
    Inherits="ALOD.Web.UserControls.Secure_Shared_UserControls_SigntureCheck" Codebehind="SignatureCheck.ascx.vb" %>
    <asp:ScriptManagerProxy runat="server" ID="proxy1">
        <Scripts>
            <asp:ScriptReference Path="~/Script/DBSign.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
<asp:Panel runat="server" ID="WrapperPanel">
    <asp:Image runat="server" ID="StatusImage" AlternateText="Signature Status" ImageAlign="AbsMiddle"
        ImageUrl="~/images/sig_check.gif" />
    <asp:Label runat="server" ID="StatusLabel" Text="No signature found" />
</asp:Panel>
