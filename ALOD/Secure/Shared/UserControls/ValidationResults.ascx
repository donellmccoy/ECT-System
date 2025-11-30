<%@ Control Language="VB" AutoEventWireup="false"
    Inherits="ALOD.Web.UserControls.Secure_Controls_ValidationResults" Codebehind="ValidationResults.ascx.vb" %>
<asp:Panel runat="server" ID="pnlWarning" CssClass="emptyItem" Visible="false">
<br />
This case cannot be sent forward until it is complete with no validation errors.
</asp:Panel>
<asp:Repeater ID="rptItems" runat="server">

    <ItemTemplate>
        <table>
            <tr>
                <td class="number">&nbsp;</td>
                <td class="label">
                    <asp:Label runat="server" ID="lblSection" Text="" />
                </td>
                <td class="value">
                    <asp:Repeater    OnItemDataBound ="rptMessage_ItemDataBound" ID="rptMessage" runat="server">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblItem"  CssClass="valFailure" Text='<%# Eval("Message", "&bull;&nbsp;{0}") %>' />
                        </ItemTemplate>
                        <SeparatorTemplate>
                       <br />
                        </SeparatorTemplate>
                    </asp:Repeater>
                </td>
            </tr>
        </table>
    </ItemTemplate>
</asp:Repeater>
<asp:Panel runat="server" ID="pnlEmpty" CssClass="emptyItem" Visible="true">
    <br />
    No Validation Errors
</asp:Panel>

