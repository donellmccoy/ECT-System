<%@ Page Title="" Language="C#" AutoEventWireup="false" MasterPageFile="~/Secure/Secure.master" CodeBehind="RFAByGroup.aspx.cs" Inherits="ALOD.Web.Reports.Secure_Reports_RFAByGroup" %>

<%@ Register Src="~/Secure/Shared/UserControls/ReportNav.ascx" TagName="ReportNav" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="server">
    <div class="indent">
        <div>
            <uc1:ReportNav ID="rptNav" runat="server" />
        </div>
        <br />
        <asp:GridView ID="rfaByGrpRptGrid" runat="server"  Width="100%" AllowSorting="True" AutoGenerateColumns="False" PageSize="20" EmptyDataText="No record found" AllowPaging="True"> 
            <Columns>
                <asp:BoundField DataField="Unit" SortExpression="Unit" HeaderText="Unit" HtmlEncode="false" >
                    <ItemStyle Width="120px"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="PAS" HeaderText="PAS" SortExpression="PAS">
                    <ItemStyle Width="10px"></ItemStyle>
                </asp:BoundField>  
                <asp:BoundField DataField="CaseId" SortExpression="Case Id" HeaderText="Case Id" HtmlEncode="false" >
                    <ItemStyle Width="30px"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="DateSent" HeaderText="Date Sent" SortExpression="Date Sent" HtmlEncode="false" DataFormatString="{0:dd-MMMM-yy }">
                    <ItemStyle Width="30px"></ItemStyle>
                </asp:BoundField>
                <asp:BoundField DataField="ReasonSentBack" HeaderText="Reason Sent Back" SortExpression="Reason Sent Back" HtmlEncode="false" >
                    <ItemStyle Width="120px"></ItemStyle>
                </asp:BoundField>
                    <asp:BoundField DataField="ExplanationForSendingBack" HeaderText="Explanation For Sending Back"  SortExpression="Explanation For Sending Back" HtmlEncode="false">
                    <ItemStyle Width="120px"></ItemStyle>
                </asp:BoundField>
            </Columns>
        </asp:GridView>
        <br />
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFooter" runat="server">
</asp:Content>
