<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Admin.Secure_Shared_Admin_AppAuth" Codebehind="AppAuth.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .InfoText
        {
            color: orangered;
            vertical-align:top;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="Server">
    <div class="indent">
        <asp:GridView ID="gvValues" runat="server" AutoGenerateColumns="False" DataSourceID="odsUsersTitle" DataKeyNames="userID" Width="100%">
            <Columns>
                <asp:TemplateField HeaderText="Username">
                    <ItemTemplate>
                        <asp:Label ID="lblId" runat="server" CssClass="hidden" Text='<%# Bind("userID")%>'></asp:Label>
                        <asp:Label ID="lblUsername" runat="server" Text='<%# Eval("username")%>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Name">
                    <ItemTemplate>
                        <asp:Label ID="lblName" runat="server" Text='<%# Eval("Name")%>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Title">
                    <EditItemTemplate>
                        <asp:TextBox ID="txtTitle" runat="server" Text='<%# Bind("Title")%>' Width="90%"></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblTitle" runat="server" Text='<%# Bind("Title")%>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField ShowHeader="False">
                    <EditItemTemplate>
                        <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="True" CommandName="Update" ValidationGroup="edit" Text="Update"></asp:LinkButton>
                        <asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit" ValidationGroup="edit"></asp:LinkButton>
                    </ItemTemplate>
                    <ItemStyle Width="10%" />
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

    <asp:ObjectDataSource ID="odsUsersTitle" runat="server" TypeName="ALOD.Data.Services.UserService" SelectMethod="GetUsersAlternateTitleByGroup" UpdateMethod="UpdateUserAlternateTitle">
        <SelectParameters>
            <asp:Parameter Name="groupId" Type="Int32" DefaultValue="0" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="userId" Type="Int32" />
            <asp:Parameter Name="groupId" Type="Int32" DefaultValue="0" />
            <asp:Parameter Name="title" Type="String" />
        </UpdateParameters>
    </asp:ObjectDataSource>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFooter" runat="Server">
</asp:Content>
