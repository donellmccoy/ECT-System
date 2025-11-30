<%@ Page Title="Edit Memorandum" Language="VB" MasterPageFile="~/Secure/Popup.master"
    AutoEventWireup="false" Inherits="ALOD.Web.Memos.Secure_Shared_Memos_EditMemo" ValidateRequest="false" Codebehind="EditMemo.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
<style type="text/css">
    .popupBody { background-color: #EEE; } 
</style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="Server">
    <table>
        <tr>
            <td class="number">
                A
            </td>
            <td class="label-small">
                Date:
            </td>
            <td class="value">
                <asp:TextBox ID="MemoDate" runat="server" CssClass="datePicker"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="number">
                B
            </td>
            <td class="label-small">
                Memo:
            </td>
            <td class="value">
                <asp:TextBox ID="MemoBody" runat="server" Columns="60" Rows="26" TextMode="MultiLine"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="number">
                C
            </td>
            <td class="label-small">
                Signature Block:
            </td>
            <td class="value">
                <asp:TextBox ID="MemoSignature" runat="server" Columns="60" Rows="3" TextMode="MultiLine"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="number">
                D
            </td>
            <td class="label-small">
                Attachments:
            </td>
            <td class="value">
                <asp:TextBox ID="MemoAttachments" runat="server" Columns="60" Rows="3" TextMode="MultiLine"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="number">
                E
            </td>
            <td class="label-small">
                Action
            </td>
            <td class="value">
                <asp:Button runat="server" ID="SaveButton" Text="Save and Close" Width="125px" 
                    Style="margin-left: 0px" />
                &nbsp;
                <input type="button" value="Cancel" onclick="window.close();" />
            </td>
        </tr>
    </table>
</asp:Content>
