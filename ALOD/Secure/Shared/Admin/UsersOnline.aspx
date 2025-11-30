<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Admin.Secure_Shared_Admin_UsersOnline" Codebehind="UsersOnline.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" DataSourceID="dataUsers">
        <Columns>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:Label runat="server" ID="lblRowNum" Text='<%# String.Format("{0}.", Container.DataItemIndex + 1) %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="userName" HeaderText="User" SortExpression="userName">
                <ItemStyle Width="140px" />
            </asp:BoundField>
            <asp:BoundField DataField="role" HeaderText="Role" SortExpression="roleName">
                <ItemStyle Width="230px" />
            </asp:BoundField>
            <asp:BoundField DataField="unitName" HeaderText="Unit" SortExpression="regionName">
                <ItemStyle Width="280px" />
            </asp:BoundField>
            <asp:BoundField DataField="loginTime" DataFormatString="{0:yyyyMMdd HHmm}" HeaderText="Login Time"
                HtmlEncode="False" SortExpression="loginTime">
                <ItemStyle Width="120px" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Time Online">
                <ItemTemplate>
                    <asp:Label runat="server" ID="TimeLabel" />
                </ItemTemplate>
                <ItemStyle Width="100px" />
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton runat="server" ID="ClearUserButton" CommandName="ClearUser" CommandArgument='<%# Eval("userId") %>'
                        Text="Clear" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <asp:ObjectDataSource ID="dataUsers" runat="server" SelectMethod="GetOnLineUsers"
        TypeName="ALOD.Data.Services.UserService"></asp:ObjectDataSource>
</asp:Content>
