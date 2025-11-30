<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Admin.Secure_Shared_Admin_UserTracking" Codebehind="UserTracking.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <div class="indent-small">
        <div style="float: left;">
            <asp:CheckBox ID="chkShowAll" runat="server" Visible="false" AutoPostBack="True"
                Text="Show All" />
        </div>
        <div style="float: right; text-align: right;">
            <asp:LinkButton ID="lnkManageUsers" runat="server">
                <asp:Image runat="server" ID="ReturnImage" AlternateText="Return to manage users"
                    ImageAlign="AbsMiddle" ImageUrl="~/images/arrow_left.gif" />
                Return to Manage Users
            </asp:LinkButton>
        </div>
        <br />
        <br />
        <asp:GridView ID="gvTracking" runat="server" PageSize="30" DataSourceID="TrackingData"
            AllowPaging="True" AllowSorting="True" AutoGenerateColumns="False" Width="100%">
            <Columns>
                <asp:BoundField DataField="actionDate" DataFormatString="{0:MM/dd/yyyy HHmm}" HeaderText="Action Date"
                    SortExpression="actiondate">
                    <ItemStyle Wrap="False" />
                </asp:BoundField>
                <asp:BoundField DataField="caseId" HeaderText="CaseId" SortExpression="description" />
                <asp:TemplateField HeaderText="Action Name" SortExpression="actionName">
                    <ItemTemplate>
                        <asp:HyperLink ID="lnkAction" runat="server" Text='<%# Eval("actionName") %>'></asp:HyperLink>
                    </ItemTemplate>
                    <ItemStyle Wrap="False" />
                </asp:TemplateField>
                <asp:BoundField DataField="notes" HeaderText="Comments" SortExpression="notes" />
                <asp:BoundField DataField="moduleName" HeaderText="Module Type" SortExpression="moduleName" />
            </Columns>
            <EmptyDataRowStyle Font-Bold="true" CssClass="emptyItem" />
            <EmptyDataTemplate>
                No activity found</EmptyDataTemplate>
            <PagerStyle HorizontalAlign="Center" VerticalAlign="Bottom"></PagerStyle>
        </asp:GridView>
    </div>
    <asp:ObjectDataSource ID="TrackingData" runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetByUserId" TypeName="ALOD.Data.Services.TrackingService">
        <SelectParameters>
            <asp:SessionParameter Name="userId" SessionField="EditId" Type="Int32" />
            <asp:ControlParameter ControlID="chkShowAll" Name="showAll" PropertyName="Checked"
                Type="Boolean" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>
