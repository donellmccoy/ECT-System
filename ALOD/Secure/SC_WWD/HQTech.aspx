<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SC_WWD/SC_WWD.master" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.WWD.Secure_sc_WWD_HQTech" Codebehind="HQTech.aspx.vb" %>

<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Register Src="~/secure/Shared/UserControls/FindingsControl.ascx" TagName="Findings"
    TagPrefix="uc" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_WWD/SC_WWD.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck"
    TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    <div class="dataBlock">
        <div class="dataBlock-header">
            <asp:Label runat="server" ID="lblHeader" Text="HQ AFRC Technician Decision" />
        </div>        
        <br />
        &nbsp;&nbsp;&nbsp;<asp:RadioButton runat="server" ID="DecisionY" GroupName="Decision"
            Text="Accept Package" AutoPostBack="True" />
        <br />
        &nbsp;&nbsp;&nbsp;<asp:RadioButton runat="server" ID="DecisionN" GroupName="Decision"
            Text="Deny Package" AutoPostBack="True" />
        <br />
        <br />
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">
    <script type="text/javascript">

        $(function () {

            $('.datePicker').datepicker(calendarPick("Past", "<%=CalendarImage %>"));

            $('.datePickerAll').datepicker(calendarPick("All", "<%=CalendarImage %>"));

            $('.datePickerPlusFuture').datepicker(calendarPick("TomorrowFuture", "<%=CalendarImage %>"));

        });

    </script>
</asp:Content>
