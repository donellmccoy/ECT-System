<%@ Page Title="" Language="C#" MasterPageFile="~/Secure/SC_DW/SC_DW.master" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.DW.Secure_SC_DW_MedTech" EnableEventValidation = "false" CodeBehind="MedTech.aspx.cs" %>


<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Register Src="~/secure/Shared/UserControls/FindingsControl.ascx" TagName="Findings" TagPrefix="uc" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_DW/SC_DW.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck"
    TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    
    <div class="dataBlock">
        <div class="dataBlock-header">
            Point of Contact Info
        </div>
        <table>
            <tr>
                <td>POC Name:</td>
                <td><asp:TextBox ID="txtPOCNameLabel" MaxLength="180" runat="server" Width="400"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>POC DSN/Phone:</td>
                <td><asp:TextBox ID="txtPOCPhoneLabel" MaxLength="180" runat="server" Width="400"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>POC DSN/Phone:</td>
                <td><asp:TextBox ID="txtPOCEmailLabel" MaxLength="180" runat="server" Width="400"></asp:TextBox>
                </td>
            </tr>
        </table>
    </div>
    <div class="dataBlock">
        <div class="dataBlock-header">Medical Technician - DW</div>
            <br />
              
        <table style="border-spacing: 5px">
            <tr>
                <td>Deployment Start:</td>
                <td><asp:Label ID="Label10" runat="server" CssClass="lblDisableText"></asp:Label>
                    <asp:TextBox ID="txtDeployStart" MaxLength="10" onchange="DateCheck(this);compareStartDate(this,ctl00_ctl00_ContentMain_ContentNested_txtDeployEnd);" 
                        runat="server" Width="80"></asp:TextBox>
                </td>
            </tr>

            <tr>
                <td>Deployment End:</td>
                <td><asp:Label ID="Label2" runat="server" CssClass="lblDisableText"></asp:Label>
                    <asp:TextBox ID="txtDeployEnd" MaxLength="10" onchange="DateCheck(this);compareEndDate(ctl00_ctl00_ContentMain_ContentNested_txtDeployStart,this);" 
                        runat="server" Width="80"></asp:TextBox>
                </td>
            </tr>

            <tr>    
                <td>&nbsp;</td>
                <td><asp:Label ID="Label1" runat="server" CssClass="labelRequired">Do not include classified information.</asp:Label></td>
            </tr>

            <tr>
                <td>Deployment Location:</td>
                <td><asp:TextBox ID="txtDeployLocation" MaxLength="250" runat="server" Width="400"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>Line Number:</td>
                <td><asp:TextBox ID="txtLineNumber" MaxLength="30" runat="server" Width="400"></asp:TextBox>
                </td>
            </tr>

            <tr>
                <td style="vertical-align: text-top">Line Remarks:</td>
                <td class="value">
                    <asp:TextBox ID="txtLineRemarks" Width="400px" TextMode="MultiLine" runat="server"
                        Rows="4" MaxLength="250"></asp:TextBox>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">
    <script type="text/javascript">

       $(function () {

            $('.datePicker').datepicker(calendarPick("Past", "<%=CalendarImage %>"));
            $('.datePickerAll').datepicker(calendarPick("All", "<%=CalendarImage %>"));

        });

        function compareStartDate(date1, date2) {
            var d1 = new Date(date1.value);
            var d2 = new Date(date2.value);
               

            if (d1 >= d2) {
                alert("Start Date must be before End Date.");
                d1.focus();
                return false;
               
            } else {
                return true;
            }
        }

        function compareEndDate(date1, date2) {
            var d1 = new Date(date1.value);
            var d2 = new Date(date2.value);

            if (d2 <= d1) {
                alert("End Date must be after Start Date.");
                d2.focus();
                return false;
            } else {
                return true;
            }
        }
    
    </script>
</asp:Content>


