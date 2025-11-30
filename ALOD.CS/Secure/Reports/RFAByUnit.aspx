<%@ Page Title="" Language="C#" AutoEventWireup="false" MasterPageFile="~/Secure/Secure.master" CodeBehind="RFAByUnit.aspx.cs" Inherits="ALOD.Web.Reports.Secure_Reports_RFAByUnit" %>

<%@ Register Src="~/Secure/Shared/UserControls/ReportNav.ascx" TagName="ReportNav" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="server">
    <div class="indent">
        <div>
            <uc1:ReportNav ID="rptNav" runat="server" />
        </div>
        <br />
        <asp:GridView ID="rfaRptGrid" runat="server"  Width="100%" AllowSorting="True" AutoGenerateColumns="False" PageSize="20" EmptyDataText="No record found" AllowPaging="True"> 
            <Columns>
                <asp:BoundField DataField="Unit" SortExpression="Unit" HeaderText="Unit" HtmlEncode="false" >
                    <ItemStyle Width="120px"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="PAS" HeaderText="PAS" SortExpression="PAS">
                    <ItemStyle Width="60px"></ItemStyle>
                </asp:BoundField>  
                <asp:BoundField DataField="Total Cases" SortExpression="Total Cases" HeaderText="Total Cases" HtmlEncode="false" >
                    <ItemStyle Width="60px"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="Total Cases With No RWOAs" HeaderText="Total Cases With No RWOAs" SortExpression="Total Cases With No RWOAs" HtmlEncode="false" >
                    <ItemStyle Width="60px"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="Total Cases With RWOAs" HeaderText="Total Cases With RWOAs" SortExpression="Total Cases With RWOAs" HtmlEncode="false" >
                    <ItemStyle Width="60px"></ItemStyle>
                </asp:BoundField>
                    <asp:BoundField DataField="Total Cases With One RWOA" HeaderText="Total Cases With One RWOA"  SortExpression="Total Cases With One RWOA" HtmlEncode="false">
                    <ItemStyle Width="60px"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="Total Cases With More Than One RWOA" HeaderText="Total Cases With More Than One RWOA" SortExpression="Total Cases With More Than One RWOA" ItemStyle-Width="60px"/>
                <asp:BoundField DataField="Total RWOAs" HeaderText="Total RWOAs" SortExpression="Total RWOAs" ItemStyle-Width="60px"/>
                <asp:BoundField DataField="Reasons" HeaderText="Reasons" SortExpression="Reasons" ItemStyle-Width="120px"/>
            </Columns>
        </asp:GridView>
        <br />
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFooter" runat="server">
</asp:Content>
