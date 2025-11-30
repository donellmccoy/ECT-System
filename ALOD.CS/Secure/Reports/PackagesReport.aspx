<%@ Page Title="" Language="C#" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Reports.Secure_Reports_PackagesReport" CodeBehind="PackagesReport.aspx.cs" %>

<%@ Register Src="~/Secure/Shared/UserControls/ReportNav.ascx" TagName="ReportNav" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" Runat="Server">
     <div class="indent">
        <div>
            <uc1:ReportNav ID="rptNav" runat="server" />
        </div>
        <br />
        <asp:GridView ID="pkgRptGrid" runat="server"  Width="100%" AllowSorting="True" AutoGenerateColumns="False" PageSize="20" EmptyDataText="No record found" AllowPaging="True"> 
            <Columns>
                <asp:BoundField DataField="pkgName" SortExpression="pkgName" HeaderText="Package Name" HtmlEncode="false" ItemStyle-Width="60px">
                    <ItemStyle Width="120px"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="startTime" SortExpression="startTime" HeaderText="Date" DataFormatString="{0:dd-MMMM-yy }" HtmlEncode="false" ItemStyle-Width="60px">
                    <ItemStyle Width="120px"></ItemStyle>
                </asp:BoundField>  
                <asp:BoundField DataField="nRowRawInserted" SortExpression="nRowRawInserted" HeaderText="Raw Records" HtmlEncode="false" ItemStyle-Width="60px">
                    <ItemStyle Width="120px"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="nRowInserted" HeaderText="Inserted" SortExpression="nRowInserted" HtmlEncode="false" ItemStyle-Width="60px">
                    <ItemStyle Width="120px"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="nDeletedMembers" HeaderText="Deleted" SortExpression="nDeletedMembers" HtmlEncode="false" ItemStyle-Width="60px">
                    <ItemStyle Width="120px"></ItemStyle>
                </asp:BoundField>
                    <asp:BoundField DataField="nModifiedRecords" HeaderText="Modified"  SortExpression="nModifiedRecords" HtmlEncode="false" ItemStyle-Width="60px">
                    <ItemStyle Width="120px"></ItemStyle>
                </asp:BoundField>
            </Columns>
        </asp:GridView>
        <br />
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFooter" Runat="Server">
</asp:Content>

