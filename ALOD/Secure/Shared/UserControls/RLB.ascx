<%@ Control Language="VB" AutoEventWireup="false" Inherits="ALOD.Web.UserControls.Secure_Shared_UserControls_RLB" Codebehind="RLB.ascx.vb" %>
<table>
    <tr>
        <td class="number">
            A
        </td>
        <td class="label">
            Reason for return:
        </td>
        <td class="value">
            <asp:Label ID="lblRwoaReason" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="number">
            B
        </td>
        <td class="label">
            <asp:Label ID="lblComments" runat="server" Text="Comments from Board:"></asp:Label>
            
        </td>
        <td class="value">
            <asp:Label ID="RLBExplantionLbl" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="number">
            C
        </td>
        <td class="label">
            Reply Comments:
        </td>
        <td class="value">
            <asp:Label ID="UnitCommentsLbl" Visible="false" runat="server"></asp:Label>
            <asp:TextBox ID="txtMedTechComments" runat="server" Width="500px" TextMode="MultiLine"
                Rows="5" MaxLength="250"></asp:TextBox>
        </td>
    </tr>
</table>
