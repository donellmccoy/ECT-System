<%@ Page Title="" Language="C#" MasterPageFile="~/Public/Public.master" AutoEventWireup="false" Inherits="ALOD.Web.login_SelectAccount" CodeBehind="SelectAccount.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentHeader" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="Server">
    <div class="indent">
        <br />
        <h1>
            Select Account</h1>
        <p>
            You have multiple accounts currently available. Please select the account you would
            like to log into</p>
        <asp:Repeater runat="server" ID="AccountRepeater">
            <ItemTemplate>
                &nbsp;&nbsp;&nbsp;<strong>&bull;</strong>
                <asp:LinkButton runat="server" ID="AccountLink" CommandName="AccountSelect" /><br /><br />
            </ItemTemplate>
        </asp:Repeater>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFooter" runat="Server">
</asp:Content>
