<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Popup.master" AutoEventWireup="false" Inherits="ALOD.Web.Reports.Secure_Reports_AdhocResults" Codebehind="AdhocResults.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" Runat="Server">
    <asp:Panel runat="server" ID="pnlErrors" Visible="false">
        Error(s) have occured while attempting to execute this Ad-Hoc report. Please contact the Help Desk for assistance and provide them with the following error information:
        <br />
        <br />
        <b>Query Id:</b>&nbsp;<asp:Label runat="server" ID="lblQueryId" />
        <br />
        <b>Query Title:</b>&nbsp;<asp:Label runat="server" ID="lblQueryTitle" />
        <br />
        <asp:BulletedList runat="server" ID="bllErrors" CssClass="labelRequired"/>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlResults" Visible="false">
        Records Found: <strong><asp:Label runat="server" ID="CountLabel" /></strong>
        <br /><br />
        <asp:gridView runat="server" ID="ResultsGrid" />
    </asp:Panel>
</asp:Content>

