<%@ Page Title="Edit MangedBy Settings" Language="VB" MasterPageFile="~/Secure/Secure.master"
    AutoEventWireup="false" Inherits="ALOD.Web.Sys.Secure_Shared_System_ManagedBy" Codebehind="ManagedBy.aspx.vb" %>

<%@ Register Src="../UserControls/FeedbackPanel.ascx" TagName="FeedbackPanel" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    Role:
    <asp:DropDownList ID="GroupSelect" runat="server" AutoPostBack="True" DataTextField="Description" DataValueField="Id">
    </asp:DropDownList>
     manages the following User Roles:
    <br />
    <br />
    <asp:GridView ID="GroupsGrid" runat="server" AutoGenerateColumns="False"
        DataKeyNames="groupId" AllowSorting="False">
        <Columns>
            <asp:BoundField DataField="Name" HeaderText="Group Name" SortExpression="Name">
                <ItemStyle Width="200px" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Managed" SortExpression="Manages">
                <EditItemTemplate>
                    <asp:CheckBox ID="cbManaged" runat="server" Checked='<%# Bind("Manages") %>' />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="cbManaged" runat="server" Checked='<%# Bind("Manages") %>' />
                </ItemTemplate>
                <ItemStyle Width="100px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Notify">
                <EditItemTemplate>
                    <asp:CheckBox ID="cbNotify" runat="server" Checked='<%# Bind("Notify") %>' />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="cbNotify" runat="server" Checked='<%# Bind("Notify") %>' />
                </ItemTemplate>
                <ItemStyle Width="100px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="View" >
                <EditItemTemplate>
                    <asp:CheckBox ID="cbViewBy" runat="server" Checked='<%# Bind("ViewBy") %>' />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="cbViewBy" runat="server" Checked='<%# Bind("ViewBy") %>' />
                </ItemTemplate>
                <ItemStyle Width="100px" />
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <%--end--%>

    <%--<asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False"
        DataKeyNames="groupId" AllowSorting="False">
        <Columns>
            <asp:TemplateField HeaderText="View" >
                <EditItemTemplate>
                    <asp:CheckBox ID="cbViewBy" runat="server" Checked='<%# Bind("ViewBy") %>' />
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:CheckBox ID="cbViewBy" runat="server" Checked='<%# Bind("ViewBy") %>' />
                </ItemTemplate>
                <ItemStyle Width="100px" />
            </asp:TemplateField>

             </Columns>
    </asp:GridView>--%>
  <%--  end--%>
    <br />
    <p>
        <asp:Button ID="btnUpdate" runat="server" Text="Update" />
    </p>
</asp:Content>
