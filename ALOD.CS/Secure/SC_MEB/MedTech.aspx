<%@ Page Title="" Language="C#" MasterPageFile="~/Secure/SC_MEB/SC_MEB.master" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.MEB.Secure_sc_meb_MedTech" CodeBehind="MedTech.aspx.cs" %>

<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Register Src="~/secure/Shared/UserControls/FindingsControl.ascx" TagName="Findings"
    TagPrefix="uc" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_MEB/SC_MEB.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck"
    TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    <div class="dataBlock">
        <div class="dataBlock-header">
            Medical Technician - Notification
        </div>
        <div id="divMemberNotificationDateSection">
            <br />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Member Notification Date
            <asp:Label ID="MEBNotificationDateLabel" runat="server" CssClass="lblDisableText"></asp:Label>
            <br />
            <br />
            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:TextBox ID="MEBNotificationDateTextBox"
                MaxLength="10" onchange="DateCheck(this);" runat="server" Width="80"></asp:TextBox>
            &nbsp;<br />
            <br />
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">
    <script type="text/javascript">

    $(function () {

            $('.datePicker').datepicker(calendarPick("Past", "<%=CalendarImage %>"));
        });        
    
    </script>
</asp:Content>
