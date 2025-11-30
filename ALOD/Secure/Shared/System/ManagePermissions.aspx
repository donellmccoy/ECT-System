<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Sys.Secure_Shared_System_ManagePermissions" Codebehind="ManagePermissions.aspx.vb" %>

<%@ Register Src="../UserControls/FeedbackPanel.ascx" TagName="FeedbackPanel" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    Select a Group:&nbsp;<asp:DropDownList ID="GroupSelect" runat="server" AutoPostBack="True" DataTextField="Description" DataValueField="Id">
    </asp:DropDownList>
    <br />
    <br />
    <asp:GridView ID="PermissionGrid" runat="server" AllowSorting="True" AutoGenerateColumns="False"
        DataKeyNames="id" DataSourceID="dataPerms" HorizontalAlign="Center" Width="90%">
        <Columns>
            <asp:BoundField DataField="name" HeaderText="Name">
                <ItemStyle Width="200px" />
            </asp:BoundField>
            <asp:BoundField DataField="description" HeaderText="Description">
                <ItemStyle Width="300px" />
            </asp:BoundField>
            <asp:TemplateField HeaderText="Allowed">
                <ItemTemplate>
                    <asp:CheckBox ID="cbPerms" runat="server" Checked='<%# Bind("allowed") %>' />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <br />
    <p style="text-align: right; padding-right: 50px;">
        <asp:Button ID="btnUpdate" runat="server" Text="Update" />
    </p>
    <asp:ObjectDataSource ID="dataPerms" runat="server" SelectMethod="GetByGroupId" TypeName="ALODWebUtility.Permission.PermissionList">
        <SelectParameters>
            <asp:ControlParameter Name="groupId" ControlID="GroupSelect" PropertyName="SelectedValue" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>
