<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Sys.Secure_Shared_Admin_EmailTest" Codebehind="EmailTest.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" Runat="Server">
<div class="indent">
    <table>
        <tr>
            <td class="number">A</td>
            <td class="label">To:</td>
            <td class="value">
                <asp:TextBox runat="server" ID="AddressBox"></asp:TextBox>
            </td>
        </tr>
         <tr>
            <td class="number">B</td>
            <td class="label">Subject:</td>
            <td class="value">
                <asp:TextBox Runat="server" ID="SubjectBox"></asp:TextBox>
            </td>
        </tr>
         <tr>
            <td class="number">C</td>
            <td class="label">Message:</td>
            <td class="value">
                <asp:TextBox runat="server" ID="MessageBox" TextMode="MultiLine" Rows="6" Columns="40"></asp:TextBox>
            </td>
        </tr>
         <tr>
            <td class="number">D</td>
            <td class="label">Action:</td>
            <td class="value">
                <asp:Button runat="server" ID="SendButton" Text="Send" />
            </td>
        </tr>
         <tr>
            <td class="number">E</td>
            <td class="label">Result:</td>
            <td class="value">
                <asp:Label runat="server" ID="Result"></asp:Label>
            </td>
        </tr>
    </table>
</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFooter" Runat="Server">
</asp:Content>

