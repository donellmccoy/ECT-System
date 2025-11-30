<%@ Page Title="" Language="C#" MasterPageFile="~/Secure/SC_Incap/SC_Incap.master"
    AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.IN.Secure_sc_in_HQTech" CodeBehind="HQTech.aspx.cs" %>

<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Register Src="~/secure/Shared/UserControls/FindingsControl.ascx" TagName="Findings"
    TagPrefix="uc" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_Incap/SC_Incap.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck"
    TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
 <asp:Panel runat="server" ID="pnlOPRInformation" CssClass="dataBlock">
        <div class="dataBlock-header">
            IN OPR Extension � HR Review
        </div>
        <div class="dataBlock-body">
            <table>
                <tr runat="server" id="OPRExtSection" visible="false">
                    <td class="number">
                        </td>
                        <td class="label labelRequired">INCAP Pay Extention:
                        </td>
                    <td>
                    <asp:RadioButtonList runat="server" ID="rblOPRExtQuestion" RepeatDirection="Horizontal" Enabled="false" >
                                        <asp:ListItem Value="1">Approve</asp:ListItem>
                                        <asp:ListItem Value="0">Disspprove</asp:ListItem>
                                    </asp:RadioButtonList>
                    </td>
                    </tr>
                <tr runat="server" id="OPRIntAppealSection" visible="false">
                    <td class="number">
                        </td>
                        <td class="label labelRequired">Initial INCAP Pay Appeal:
                        </td>
                    <td>
                    <asp:RadioButtonList runat="server" ID="rblOPRAppealQuestion" RepeatDirection="Horizontal" Enabled="false" >
                                        <asp:ListItem Value="1">Approve</asp:ListItem>
                                        <asp:ListItem Value="0">Disspprove</asp:ListItem>
                                    </asp:RadioButtonList>
                    </td>
                    </tr>
                <tr runat="server" id="OPRExtAppealSection" visible="false">
                    <td class="number">
                        </td>
                        <td class="label labelRequired">INCAP Pay Extention Appeal:
                        </td>
                    <td>
                    <asp:RadioButtonList runat="server" ID="rblOPRExtAppealQuestion" RepeatDirection="Horizontal" Enabled="false" >
                                        <asp:ListItem Value="1">Approve</asp:ListItem>
                                        <asp:ListItem Value="0">Disspprove</asp:ListItem>
                                    </asp:RadioButtonList>
                    </td>
                    </tr>
                <tr>
                    <td>
                        <br />
                    </td>
                </tr>
            </table>  
        </div>      
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlOCRInformation" CssClass="dataBlock">
        <div class="dataBlock-header">
            IN OCR Extension � HR Review
        </div>
        <div class="dataBlock-body">
            <table>
                <tr runat="server" id="OCRExtSection" visible="false">
                    <td class="number">
                        </td>
                        <td class="label labelRequired">INCAP Pay Extention:
                        </td>
                    <td>
                    <asp:RadioButtonList runat="server" ID="rblOCRExtQuestion" RepeatDirection="Horizontal" Enabled="false" >
                                        <asp:ListItem Value="1">Approve</asp:ListItem>
                                        <asp:ListItem Value="0">Disspprove</asp:ListItem>
                                    </asp:RadioButtonList>
                    </td>
                    </tr>
                <tr runat="server" id="OCRIntAppealSection" visible="false">
                    <td class="number">
                        </td>
                        <td class="label labelRequired">Initial INCAP Pay Appeal:
                        </td>
                    <td>
                    <asp:RadioButtonList runat="server" ID="rblOCRAppealQuestion" RepeatDirection="Horizontal" Enabled="false" >
                                        <asp:ListItem Value="1">Approve</asp:ListItem>
                                        <asp:ListItem Value="0">Disspprove</asp:ListItem>
                                    </asp:RadioButtonList>
                    </td>
                    </tr>
                <tr runat="server" id="OCRExtAppealSection" visible="false">
                    <td class="number">
                        </td>
                        <td class="label labelRequired">INCAP Pay Extention Appeal:
                        </td>
                    <td>
                    <asp:RadioButtonList runat="server" ID="rblOCRExtAppealQuestion" RepeatDirection="Horizontal" Enabled="false" >
                                        <asp:ListItem Value="1">Approve</asp:ListItem>
                                        <asp:ListItem Value="0">Disspprove</asp:ListItem>
                                    </asp:RadioButtonList>
                    </td>
                    </tr>
                <tr>
                    <td>
                        <br />
                    </td>
                </tr>
            </table>  
        </div>      
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlDOSInformation" CssClass="dataBlock">
        <div class="dataBlock-header">
            IN Director Of Staff Review
        </div>
        <div class="dataBlock-body">
            <table>
                <tr runat="server" id="DOSExtSection" visible="false">
                    <td class="number">
                        </td>
                        <td class="label labelRequired">INCAP Pay Extention:
                        </td>
                    <td>
                    <asp:RadioButtonList runat="server" ID="rblDOSExtQuestion" RepeatDirection="Horizontal" Enabled="false" >
                                        <asp:ListItem Value="1">Approve</asp:ListItem>
                                        <asp:ListItem Value="0">Disspprove</asp:ListItem>
                                    </asp:RadioButtonList>
                    </td>
                    </tr>
                <tr runat="server" id="DOSAppealSection" visible="false">
                    <td class="number">
                        </td>
                        <td class="label labelRequired">Initial INCAP Pay Appeal:
                        </td>
                    <td>
                    <asp:RadioButtonList runat="server" ID="rblDOSAppealQuestion" RepeatDirection="Horizontal" Enabled="false" >
                                        <asp:ListItem Value="1">Approve</asp:ListItem>
                                        <asp:ListItem Value="0">Disspprove</asp:ListItem>
                                    </asp:RadioButtonList>
                    </td>
                    </tr>
                <tr runat="server" id="DOSExtAppealSection" visible="false">
                    <td class="number">
                        </td>
                        <td class="label labelRequired">INCAP Pay Extention Appeal:
                        </td>
                    <td>
                    <asp:RadioButtonList runat="server" ID="rblDOSExtAppealQuestion" RepeatDirection="Horizontal" Enabled="false" >
                                        <asp:ListItem Value="1">Approve</asp:ListItem>
                                        <asp:ListItem Value="0">Disspprove</asp:ListItem>
                                    </asp:RadioButtonList>
                    </td>
                    </tr>
                <tr>
                    <td>
                        <br />
                    </td>
                </tr>
            </table>  
        </div>      
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlCCRInformation" CssClass="dataBlock">
        <div class="dataBlock-header">
            IN Command Chief Review
        </div>
        <div class="dataBlock-body">
            <table>
                <tr runat="server" id="CCRExtSection" visible="false">
                    <td class="number">
                        </td>
                        <td class="label labelRequired">INCAP Pay Extention:
                        </td>
                    <td>
                    <asp:RadioButtonList runat="server" ID="rblCCRExtQuestion" RepeatDirection="Horizontal" Enabled="false" >
                                        <asp:ListItem Value="1">Approve</asp:ListItem>
                                        <asp:ListItem Value="0">Disspprove</asp:ListItem>
                                    </asp:RadioButtonList>
                    </td>
                    </tr>
                <tr runat="server" id="CCRAppealSection" visible="false">
                    <td class="number">
                        </td>
                        <td class="label labelRequired">Initial INCAP Pay Appeal:
                        </td>
                    <td>
                    <asp:RadioButtonList runat="server" ID="rblCCRAppealQuestion" RepeatDirection="Horizontal" Enabled="false" >
                                        <asp:ListItem Value="1">Approve</asp:ListItem>
                                        <asp:ListItem Value="0">Disspprove</asp:ListItem>
                                    </asp:RadioButtonList>
                    </td>
                    </tr>
                <tr runat="server" id="CCRExtAppealSection" visible="false">
                    <td class="number">
                        </td>
                        <td class="label labelRequired">INCAP Pay Extention Appeal:
                        </td>
                    <td>
                    <asp:RadioButtonList runat="server" ID="rblCCRExtAppealQuestion" RepeatDirection="Horizontal" Enabled="false" >
                                        <asp:ListItem Value="1">Approve</asp:ListItem>
                                        <asp:ListItem Value="0">Disspprove</asp:ListItem>
                                    </asp:RadioButtonList>
                    </td>
                    </tr>
                <tr>
                    <td>
                        <br />
                    </td>
                </tr>
            </table>  
        </div>      
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlVCRInformation" CssClass="dataBlock">
        <div class="dataBlock-header">
            IN Vice Commander Review
        </div>
        <div class="dataBlock-body">
            <table>
                <tr runat="server" id="VCRExtSection" visible="false">
                    <td class="number">
                        </td>
                        <td class="label labelRequired">INCAP Pay Extention:
                        </td>
                    <td>
                    <asp:RadioButtonList runat="server" ID="rblVCRExtQuestion" RepeatDirection="Horizontal" Enabled="false" >
                                        <asp:ListItem Value="1">Approve</asp:ListItem>
                                        <asp:ListItem Value="0">Disspprove</asp:ListItem>
                                    </asp:RadioButtonList>
                    </td>
                    </tr>
                <tr runat="server" id="VCRAppealSection" visible="false">
                    <td class="number">
                        </td>
                        <td class="label labelRequired">Initial INCAP Pay Appeal:
                        </td>
                    <td>
                    <asp:RadioButtonList runat="server" ID="rblVCRAppealQuestion" RepeatDirection="Horizontal" Enabled="false" >
                                        <asp:ListItem Value="1">Approve</asp:ListItem>
                                        <asp:ListItem Value="0">Disspprove</asp:ListItem>
                                    </asp:RadioButtonList>
                    </td>
                    </tr>
                <tr runat="server" id="VCRExtAppealSection" visible="false">
                    <td class="number">
                        </td>
                        <td class="label labelRequired">INCAP Pay Extention Appeal:
                        </td>
                    <td>
                    <asp:RadioButtonList runat="server" ID="rblVCRExtAppealQuestion" RepeatDirection="Horizontal" Enabled="false" >
                                        <asp:ListItem Value="1">Approve</asp:ListItem>
                                        <asp:ListItem Value="0">Disspprove</asp:ListItem>
                                    </asp:RadioButtonList>
                    </td>
                    </tr>
                <tr>
                    <td>
                        <br />
                    </td>
                </tr>
            </table>  
        </div>      
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlDOPInformation" CssClass="dataBlock">
        <div class="dataBlock-header">
            IN Director of Personnel Review
        </div>
        <div class="dataBlock-body">
            <table>
                <tr runat="server" id="DOPExtSection" visible="false">
                    <td class="number">
                        </td>
                        <td class="label labelRequired">INCAP Pay Extention:
                        </td>
                    <td>
                    <asp:RadioButtonList runat="server" ID="rblDOPExtQuestion" RepeatDirection="Horizontal" Enabled="false" >
                                        <asp:ListItem Value="1">Approve</asp:ListItem>
                                        <asp:ListItem Value="0">Disspprove</asp:ListItem>
                                    </asp:RadioButtonList>
                    </td>
                    </tr>
                <tr runat="server" id="DOPAppealSection" visible="false">
                    <td class="number">
                        </td>
                        <td class="label labelRequired">Initial INCAP Pay Appeal:
                        </td>
                    <td>
                    <asp:RadioButtonList runat="server" ID="rblDOPAppealQuestion" RepeatDirection="Horizontal" Enabled="false" >
                                        <asp:ListItem Value="1">Approve</asp:ListItem>
                                        <asp:ListItem Value="0">Disspprove</asp:ListItem>
                                    </asp:RadioButtonList>
                    </td>
                    </tr>
                <tr runat="server" id="DOPExtAppealSection" visible="false">
                    <td class="number">
                        </td>
                        <td class="label labelRequired">INCAP Pay Extention Appeal:
                        </td>
                    <td>
                    <asp:RadioButtonList runat="server" ID="rblDOPExtAppealQuestion" RepeatDirection="Horizontal" Enabled="false" >
                                        <asp:ListItem Value="1">Approve</asp:ListItem>
                                        <asp:ListItem Value="0">Disspprove</asp:ListItem>
                                    </asp:RadioButtonList>
                    </td>
                    </tr>
                <tr>
                    <td>
                        <br />
                    </td>
                </tr>
            </table>  
        </div>      
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlCAFRInformation" CssClass="dataBlock">
        <div class="dataBlock-header">
            IN CAFR Action
        </div>
        <div class="dataBlock-body">
            <table>
                <tr runat="server" id="CAFRExtSection" visible="false">
                    <td class="number">
                        </td>
                        <td class="label labelRequired">INCAP Pay Extention:
                        </td>
                    <td>
                    <asp:RadioButtonList runat="server" ID="rblCAFRExtQuestion" RepeatDirection="Horizontal" Enabled="false" >
                                        <asp:ListItem Value="1">Approve</asp:ListItem>
                                        <asp:ListItem Value="0">Disspprove</asp:ListItem>
                                    </asp:RadioButtonList>
                    </td>
                    </tr>
                <tr runat="server" id="CAFRAppealSection" visible="false">
                    <td class="number">
                        </td>
                        <td class="label labelRequired">Initial INCAP Pay Appeal:
                        </td>
                    <td>
                    <asp:RadioButtonList runat="server" ID="rblCAFRAppealQuestion" RepeatDirection="Horizontal" Enabled="false" >
                                        <asp:ListItem Value="1">Approve</asp:ListItem>
                                        <asp:ListItem Value="0">Disspprove</asp:ListItem>
                                    </asp:RadioButtonList>
                    </td>
                    </tr>
                <tr runat="server" id="CAFRExtAppealSection" visible="false">
                    <td class="number">
                        </td>
                        <td class="label labelRequired">INCAP Pay Extention Appeal:
                        </td>
                    <td>
                    <asp:RadioButtonList runat="server" ID="rblCAFRExtAppealQuestion" RepeatDirection="Horizontal" Enabled="false" >
                                        <asp:ListItem Value="1">Approve</asp:ListItem>
                                        <asp:ListItem Value="0">Disspprove</asp:ListItem>
                                    </asp:RadioButtonList>
                    </td>
                    </tr>
                <tr>
                    <td>
                        <br />
                    </td>
                </tr>
            </table>  
        </div>      
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">
    <script type="text/javascript">


        $(function () {

            $('.datePicker').datepicker(calendarPick("Past", "<%=CalendarImage %>"));
            $('.datePickerAll').datepicker(calendarPick("All", "<%=CalendarImage %>"));

        });

            
    </script>
</asp:Content>
