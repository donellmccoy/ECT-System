<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Admin.Secure_Shared_Admin_RoleRequests" Codebehind="RoleRequests.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="Server">
    <asp:GridView runat="server" ID="RequestGrid" Width="100%" AutoGenerateColumns="false"
        AllowSorting="true">
        <Columns>
            <asp:TemplateField HeaderText="User" SortExpression="LastName">
                <ItemTemplate>
                    <%#Eval("LastName")%>,
                    <%#Eval("FirstName")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField HeaderText="SSN" DataField="SSN" SortExpression="SSN" />
            <asp:BoundField HeaderText="Requested Role" DataField="Group Name" SortExpression="Group Name" />
            <asp:TemplateField HeaderText="Type" SortExpression="NewRole">
                <ItemTemplate>
                    <%#IIf(Eval("NewRole").ToString = "True", "New Role", "Role Change")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Unit" SortExpression="Unit Name">
                <ItemTemplate>
                    <%#Eval("Unit Name") %>
                    (<%#Eval("Pas Code")%>)
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Date" SortExpression="Date Requested">
                <ItemTemplate>
                    <%#Eval("Date Requested", "{0:MM/dd/yyyy}")%>
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:ImageButton runat="server" Visible="true" ID="ViewImage" SkinID="imgUserEdit" CommandArgument='<%#Eval("userId") %>' CommandName="ProcessRequest" AlternateText="Process Request" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
        <EmptyDataRowStyle CssClass="emptyItem" />
        <EmptyDataTemplate>
            No Requests Found
        </EmptyDataTemplate>
    </asp:GridView>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFooter" runat="Server">
</asp:Content>
