<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SC_PEPP/SC_PEPP.master"
    MaintainScrollPositionOnPostback="true" AutoEventWireup="false"
     EnableEventValidation="false" Inherits="ALOD.Web.Special_Case.PEPP.Secure_sc_pepp_HQTech" Codebehind="HQTech.aspx.vb" %>

<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Register Src="~/secure/Shared/UserControls/FindingsControl.ascx" TagName="Findings"
    TagPrefix="uc" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_PEPP/SC_PEPP.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck"
    TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/ICDCodeControl.ascx" TagName="ICDCodeControl" TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/ICD7thCharacterControl.ascx" TagName="ICD7thCharacterControl" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    <div class="dataBlock">
        <div class="dataBlock-header">
            PEPP Case Info
        </div>
    </div>
        <div class="dataBlock">
        <div class="dataBlock-header">
            Case Data
        </div>
        <asp:UpdatePanel ID="upnlCaseData" runat="server" ChildrenAsTriggers="True">
            <ContentTemplate>
                <table>
                    <tr>
                        <td class="number">
                            A
                        </td>
                        <td class="label  labelRequired">
                            *Type:
                        </td>
                        <td>
                            <%--<asp:DropDownList runat="server" AutoPostBack="true" ID="ddlType"></asp:DropDownList>--%>
                            <asp:CheckBoxList runat="server" AutoPostBack="true" ID="chklType" RepeatDirection="Horizontal" RepeatLayout="Table" CellSpacing="10" />
                        </td>
                    </tr>
                    <tr runat="server" id="OtherTypeRow" visible="false">
                        <td class="number">
                            A.1
                        </td>
                        <td class="label  labelRequired">
                            *Enter Type Name:
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtEnterType" Width="175px" MaxLength="50"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            B
                        </td>
                        <td class="label  labelRequired">
                            *Rating:
                        </td>
                        <td>
                            <asp:DropDownList runat="server" AutoPostBack="true" ID="ddlRating"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr runat="server" id="OtherRatingRow" visible="false">
                        <td class="number">
                            B.1
                        </td>
                        <td class="label  labelRequired">
                            *Enter Rating Name:
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtEnterRating" Width="175px" MaxLength="50"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            C
                        </td>
                        <td class="label  labelRequired">
                            *Disposition:
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlDisposition"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            D
                        </td>
                        <td class="label  labelRequired">
                            *Renewal:
                        </td>
                        <td>
                            <asp:RadioButton runat="server" ID="rdbRenewalYes" GroupName="RenewalDecision" Text="Yes" AutoPostBack="false" />
                            <asp:RadioButton runat="server" ID="rdbRenewalNo" GroupName="RenewalDecision" Text="No" AutoPostBack="false" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            E
                        </td>
                        <td class="label  labelRequired">
                            *Base Assign:
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlBaseAssign"></asp:DropDownList>
                        </td>
                    </tr>
        <%--            <tr>
                        <td class="number">
                            F
                        </td>
                        <td class="label  labelRequired">
                            *Completed By:
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlCompletedByUnit"></asp:DropDownList>
                        </td>
                    </tr>--%>
                    <tr>
                        <td class="number">
                            F
                        </td>
                        <td class="label  labelRequired">
                            *Date Received:
                        </td>
                        <td><asp:TextBox ID="txtDateReceived" MaxLength="10"
                                onchange="DateCheck(this);" runat="server" Width="80"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td class="number">
                            G
                        </td>
                        <td class="label  labelRequired">
                            *Is Waiver Required?:
                        </td>
                        <td>
                            <asp:RadioButton runat="server" ID="rdbWaiverRequiredYes" GroupName="WaiverRequired" Text="Yes" AutoPostBack="true " />
                            <asp:RadioButton runat="server" ID="rdbWaiverRequiredNo" GroupName="WaiverRequired" Text="No" AutoPostBack="true" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            H
                        </td>
                        <td class="label">
                            <asp:Label runat="server" ID="lblExpireDateRowText" Text="Waiver Expiration Date:" />
                        </td>
                        <td>
                            <asp:TextBox ID="txtExpireDate" MaxLength="10"
                                onchange="DateCheck(this);" runat="server" Width="80"></asp:TextBox>
                            <asp:CheckBox ID="chbIndefinite" runat="server" Text="Indefinite" CssClass="boxPosition" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            I
                        </td>
                        <td class="label">
                            Assignment Limitation Code:
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlALC" Width="150px">
                                <asp:ListItem Value="0" Selected="True">No Limitations</asp:ListItem>
                                <asp:ListItem Value="1">C1</asp:ListItem>
                                <asp:ListItem Value="2">C2</asp:ListItem>
                                <asp:ListItem Value="3">C3</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div class="dataBlock">
        <div class="dataBlock-header">
            Select an ICD code
        </div>
        <asp:UpdatePanel ID="UpdatePanel4" runat="server">
            <ContentTemplate>
                <table>
                    <tr>
                        <td class="number">
                            K
                        </td>
                        <td class="label">
                            <asp:Label runat="server" ID="lblICDRowText" Text="Diagnosis:" />
                        </td>
                        <td class="value">
                            <uc1:ICDCodeControl runat="server" ID="ucICDCodeControl" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            L
                        </td>
                        <td class="label">
                            7th Character:
                        </td>
                        <td class="value">
                            <uc1:ICD7thCharacterControl runat="server" ID="ucICD7thCharacterControl" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            M
                        </td>
                        <td class="label">
                            <asp:Label runat="server" ID="lblDiagnosisRowText" Text="Diagnosis Text:" />
                        </td>
                        <td class="value">
                            <asp:Label ID="DiagnosisLabel" runat="server" CssClass="lblDisableText"></asp:Label>
                            <asp:TextBox ID="DiagnosisTextBox" Width="500px" TextMode="MultiLine" runat="server" Rows="4" MaxLength="250"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">
    <script type="text/javascript">

        function pageLoad() {


            $('.datePickerPast').datepicker(calendarPick("Past", "<%=CalendarImage %>"));

            $('.datePicker').datepicker(calendarPick("All", "<%=CalendarImage %>"));

            $('.datePickerFuture').datepicker(calendarPick("Future", "<%=CalendarImage %>"));

            initMultiLines();

        }

        // Toggles on/off Expiration Date to Indefinite
        function SetIndefinite() {

            var tx, cb;
            tx = document.getElementById('ctl00_ctl00_ContentMain_ContentNested_txtExpireDate');
            cb = document.getElementById('ctl00_ctl00_ContentMain_ContentNested_chbIndefinite');

            if (tx.value != "12/31/2100" || tx.value.length == 0) {
                tx.value = "12/31/2100";

            }

            else if (tx.value == "12/31/2100") {
                tx.value = "";
                tx.disabled = false;

            }
        };

        function ToggleOtherRatingTextbox() {
            var element = document.getElementById("OtherRatingRow");
            var val = $("[id*='ddlRating'] :selected").text();
            
            if (val == "Other")
                element.style.display = "";
            else
                element.style.display = "none";
        };

    </script>
</asp:Content>