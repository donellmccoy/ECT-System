<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Admin.Secure_Shared_Admin_PermissionReport" Codebehind="PermissionReport.aspx.vb" %>

 <asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
     <div class="indent">
        <div class="border-thin">
            <div class="searchTitle">
                Permission Search</div>
            <div class="searchBody">
                <p style="text-align: left">   
                  <asp:Label runat="server" AssociatedControlID="PermissionsSelect" Text="Permission:" />
                    <asp:DropDownList ID="PermissionsSelect"  DataTextField="Description" DataValueField="Id" Width="250px" runat="server">
                    </asp:DropDownList>
                
                    <asp:Button ID="SearchButton" runat="server" Text="View in Browser"   CausesValidation="False">
                    </asp:Button>&nbsp;
                  
                    <asp:Button runat="server" ID="ExportButton" Text="Export to Excel" Visible="True"
                        OnClick="btnExport_Click" />
              
            </div>
        </div>
        <br />
        <br />
        <asp:Panel runat="server" ID="SearchMessage" CssClass="emptyItem" Visible="true">
            Please select a permission to search by
        </asp:Panel>
        <asp:GridView ID="UsersGrid" runat="server" Width="920px" HorizontalAlign="Center"
            AllowSorting="True" AllowPaging="True" PageSize="30" AutoGenerateColumns="False"
            DataSourceId="Permissions" datakeynames="SSN" >
            <Columns>
               
                <asp:TemplateField HeaderText="Name" SortExpression="Name">
                    <ItemTemplate>
                        <%#Eval("Name")%>
                    </ItemTemplate>
                    <ItemStyle Width="150px"></ItemStyle>
                </asp:TemplateField>
                <asp:BoundField HeaderText="SSN" SortExpression="LastFour" DataField="LastFour">
                    <ItemStyle Width="60px" />
                </asp:BoundField>
                <asp:TemplateField HeaderText="Role" SortExpression="RoleName">
                    <ItemTemplate>
                        <%#Eval("RoleName")%>
                    </ItemTemplate>
                    <ItemStyle Width="220px"></ItemStyle>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Unit" SortExpression="CurrentUnitName">
                    <ItemTemplate>
                        <%#Eval("CurrentUnitName")%>
                    </ItemTemplate>
                </asp:TemplateField> 
                 <asp:TemplateField HeaderText="Status" SortExpression="AccessStatusDescr">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="StatusLabel"><%#Eval("AccessStatusDescr")%></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Width="100px" />
                </asp:TemplateField>
<%--                  <asp:TemplateField>
                    <ItemTemplate>
                        <asp:ImageButton ImageAlign="AbsMiddle" SkinID="imgUserEdit" ID="lnk_account" CommandArgument='<%# Eval("Id") %>'
                            ToolTip="EditUser" runat="server" CommandName="EditInfo" />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                </asp:TemplateField>--%>
            </Columns>
            <EmptyDataRowStyle Font-Bold="true" CssClass="emptyItem" />
            <EmptyDataTemplate>
                No users found</EmptyDataTemplate>
            <PagerStyle HorizontalAlign="Center" VerticalAlign="Bottom"></PagerStyle>
        </asp:GridView>
    </div>
    <asp:ObjectDataSource ID="UserData" runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetUsersWithPermission" TypeName="ALOD.Data.Services.UserService">
        <SelectParameters>       
            <asp:ControlParameter ControlID="PermissionsSelect" Name="permissionId" PropertyName="SelectedValue"
                Type="Int32"/>
        </SelectParameters>
    </asp:ObjectDataSource>
     <asp:ObjectDataSource ID="Permissions" runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetUsersWithPermission" TypeName="ALOD.Data.Services.UserService">
        <SelectParameters>       
            <asp:ControlParameter ControlID="PermissionsSelect" Name="permissionId" PropertyName="SelectedValue"
                Type="Int16"/>
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>