<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Sys.Secure_Shared_System_UserPermissions" Codebehind="UserPermissions.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <div class="indent-small">
        <div style="width: 68%; float: left;">
            <strong>Permissions for:</strong> <asp:label runat="server" ID="UserName" />
        </div>
        <div style="width: 30%; float: right; text-align: right;">
            <asp:LinkButton ID="lnkManageUsers" runat="server">
                <asp:Image runat="server" ID="ReturnImage" AlternateText="Return to manage users"
                    ImageAlign="AbsMiddle" ImageUrl="~/images/arrow_left.gif" />
                Return to Manage Users
            </asp:LinkButton>
            <asp:LinkButton ID="lnkPermReport" runat="server">
                <asp:Image runat="server" ID="Image1" AlternateText="Return to permission report"
                    ImageAlign="AbsMiddle" ImageUrl="~/images/arrow_left.gif" />
                Return to Permission Report
            </asp:LinkButton>
        </div>
        <br /><br />
        <asp:GridView ID="gvPerms" runat="server" AllowSorting="True" AutoGenerateColumns="False"
            DataKeyNames="id" DataSourceID="dataPerms" HorizontalAlign="Center" Width="100%">
            <Columns>
                <asp:TemplateField HeaderText="Name">
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("name") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblName" runat="server" Text='<%# Bind("name") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Width="200px" />
                </asp:TemplateField>
                <asp:BoundField DataField="description" HeaderText="Description">
                    <ItemStyle Width="300px" />
                </asp:BoundField>
                <asp:TemplateField HeaderText="Allowed">
                    <ItemTemplate>
                        <asp:RadioButton ID="rbDefault" runat="server" Checked="True" GroupName="status"
                            Text="Default" />&nbsp;<asp:RadioButton ID="rbGrant" runat="server" GroupName="status"
                                Text="Grant" />&nbsp;<asp:RadioButton ID="rbRevoke" runat="server" GroupName="status"
                                    Text="Revoke" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <br />
        <br />
        <div style="text-align: right;">
            <asp:Button ID="btnUpdate" runat="server" Text="Update" />&nbsp;
            <asp:Button ID="CancelButton" runat="server" Text="Cancel" />
        </div>
    </div>
    <asp:ObjectDataSource ID="dataPerms" runat="server" SelectMethod="GetUserAssignable" TypeName="ALODWebUtility.Permission.PermissionList">
    </asp:ObjectDataSource>
</asp:Content>
