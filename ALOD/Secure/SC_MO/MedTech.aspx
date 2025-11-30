<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SC_MO/SC_MO.master"
    MaintainScrollPositionOnPostback="true" AutoEventWireup="false"
     EnableEventValidation="false" Inherits="ALOD.Web.Special_Case.MO.Secure_sc_mo_MedTech" Codebehind="MedTech.aspx.vb" %>

<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Register Src="~/secure/Shared/UserControls/FindingsControl.ascx" TagName="Findings"
    TagPrefix="uc" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_MO/SC_MO.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck"
    TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/ICDCodeControl.ascx" TagName="ICDCodeControl" TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/ICD7thCharacterControl.ascx" TagName="ICD7thCharacterControl" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    <div class="dataBlock">
        <div class="dataBlock-header">
            MO Case Info
        </div>
    </div>
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
        <div class="dataBlock-header">
            Select an ICD code
        </div>
        <table>
            <tr>
                <td class="number">
                    A
                </td>
                <td class="label  labelRequired">
                    *Diagnosis:
                </td>
                <td class="value">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <uc1:ICDCodeControl runat="server" ID="ucICDCodeControl" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td class="number">
                    B
                </td>
                <td class="label">
                    7th Character:
                </td>
                <td class="value">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <uc1:ICD7thCharacterControl runat="server" ID="ucICD7thCharacterControl" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td class="number">
                    C
                </td>
                <td class="label labelRequired">
                    *Diagnosis Text:
                </td>
                <td class="value">
                    <asp:Label ID="DiagnosisLabel" runat="server" CssClass="lblDisableText"></asp:Label>
                    <asp:TextBox ID="DiagnosisTextBox" Width="500px" TextMode="MultiLine" runat="server"
                        Rows="4" MaxLength="250"></asp:TextBox>
                </td>
            </tr>
        </table>
    </div>
    <div class="dataBlock">
        <div class="dataBlock-header">
            Additional Info
        </div>
        <table>
            <tr>
                <td class="number">
                    A
                </td>
                <td class="label">
                    Case Type
                </td>
                <td>
                    <asp:DropDownList runat="server" ID="ddlCaseType"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="number">
                    B
                </td>
                <td class="label labelRequired">
                    *Justification
                </td>
                <td>
                    <asp:TextBox Height="50" Width="500px" runat="server" ID="txtJustification" TextMode="MultiLine" MaxLength="250"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="number">
                    C
                </td>
                <td class="label">
                    HYTD/MSD:
                </td>
                <td><asp:TextBox ID="HYTDTextBox" MaxLength="10"
                        onchange="DateCheck(this);" runat="server" Width="80"></asp:TextBox></td>
            </tr>
        </table>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">
    <script type="text/javascript">

        $(function () {

            $('.datePicker').datepicker(calendarPick("Past", "<%=CalendarImage %>"));

            $('.datePickerAll').datepicker(calendarPick("All", "<%=CalendarImage %>"));

            $('.datePickerFuture').datepicker(calendarPick("All", "<%=CalendarImage %>"));

        });

    </script>
</asp:Content>