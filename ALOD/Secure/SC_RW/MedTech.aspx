<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SC_RW/SC_RW.master" MaintainScrollPositionOnPostback="true" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.RW.Secure_sc_rw_MedTech" EnableEventValidation="false" Codebehind="MedTech.aspx.vb" %>

<%@ MasterType VirtualPath="~/Secure/SC_RW/SC_RW.master" %>

<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="~/secure/Shared/UserControls/FindingsControl.ascx" TagName="Findings" TagPrefix="uc" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck" TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/ICDCodeControl.ascx" TagName="ICDCodeControl" TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/ICD7thCharacterControl.ascx" TagName="ICD7thCharacterControl" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    <asp:Panel runat="server" ID="pnlMedTechControls" CssClass="dataBlock">
        <div class="dataBlock-header">
            RW Case Info:
        </div>
        <div class="dataBlock-body">
            <table class="tableStyle">
                <tr>
                    <td class="number">
                        A
                    </td>
                    <td class="label labelRequired">
                        *Renewal Due Date:
                    </td>
                    <td class="value">
                        <asp:TextBox runat="server" ID="txtRenewalDate" MaxLength="10" onchange="DateCheck(this);" Width="80" />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        B
                    </td>
                    <td class="label labelRequired">
                        Med Group Name:
                    </td>
                    <td class="value">
                        <asp:DropDownList runat="server" ID="ddlMedGroups" />
                    </td>
                </tr>

                <tr>
                    <td class="number">
                        C
                    </td>
                    <td class="">
                         <asp:Label runat="server" CssClass="label labelRequired" ID="lblRMU">RMU Name:</asp:Label>
                    </td>
                    <td class="value">
                        <asp:DropDownList runat="server" ID="ddlRMUs" />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        D
                    </td>
                    <td class="label labelRequired">
                        *Diagnosis:
                    </td>
                    <td class="value">
                        <asp:UpdatePanel runat="server" ID="upnlMedTechICDControl">
                            <ContentTemplate>
                                <uc1:ICDCodeControl runat="server" ID="ucICDCodeControl" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        E
                    </td>
                    <td class="label">
                        7th Character:
                    </td>
                    <td class="value">
                        <asp:UpdatePanel runat="server" ID="upnlMedTech7thCharacterControl">
                            <ContentTemplate>
                                <uc1:ICD7thCharacterControl runat="server" ID="ucICD7thCharacterControl" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        F
                    </td>
                    <td class="label labelRequired">
                        Diagnosis Text:
                    </td>
                    <td class="value">
                        <asp:Label runat="server" ID="lblDiagnosis" CssClass="lblDisableText" />
                        <asp:TextBox runat="server" ID="txtDiagnosis" Width="500px" TextMode="MultiLine" Rows="4" MaxLength="250" />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        G
                    </td>
                    <td class="label labelRequired">
                        Patient Name:
                    </td>
                    <td class="value">
                        <asp:TextBox runat="server" Enabled="false" ID="txtMemberName" />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        H
                    </td>
                    <td class="label labelRequired">
                        Rank:
                    </td>
                    <td class="value">
                        <asp:TextBox runat="server" Enabled="false" ID="txtMemberRank" />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        I
                    </td>
                    <td class="label labelRequired">
                        SSN:
                    </td>
                    <td class="value">
                        <asp:TextBox runat="server" Enabled="false" ID="txtMemberProtectedSSN" />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        J
                    </td>
                    <td class="label">
                        DAFSC:
                    </td>
                    <td class="value">
                        <asp:TextBox runat="server" ID="txtDAFSC"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        K
                    </td>
                    <td class="label">
                        Years Satisfactory Service:
                    </td>
                    <td class="value">
                        <asp:DropDownList runat="server" ID="ddlYearsSatisfactoryService" AutoPostBack="True" />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        L
                    </td>
                    <td class="label">
                        BMI:
                    </td>
                    <td class="value">
                        <asp:TextBox runat="server" ID="txtBodyMassIndex" class="numericOnly" />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        M
                    </td>
                    <td class="label labelRequired">
                        Missed Work Days (in the last year - due to condition):
                    </td>
                    <td class="value">
                        <asp:DropDownList runat="server" ID="ddlMissedWorkDays" />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        N
                    </td>
                    <td class="label labelRequired">
                        Specialist Required for Management:
                    </td>
                    <td class="value">
                        <asp:DropDownList runat="server" ID="ddlSpecialistRequired" />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        O
                    </td>
                    <td class="label labelRequired">
                        ER or Urgent Care Visits:
                    </td>
                    <td class="value">
                        <asp:RadioButtonList runat="server" ID="rblEROrUrgentCareVisits" RepeatDirection="Horizontal">
                            <asp:ListItem Value="1" Text="Yes" />
                            <asp:ListItem Value="0" Text="No" />
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        P
                    </td>
                    <td runat="server" id="tdERorUrgentCareDetails" class="label">
                        ER or Urgent Care Visit (details):
                    </td>
                    <td class="value">
                        <asp:TextBox runat="server" ID="txtERorUrgentCareDetails" Rows="5" TextMode="MultiLine" Width="80%" MaxLength="250" />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        Q
                    </td>
                    <td class="label labelRequired">
                        Hospitalizations:
                    </td>
                    <td class="value">
                        <asp:RadioButtonList runat="server" ID="rblHospitalizations" RepeatDirection="Horizontal">
                            <asp:ListItem Value="1" Text="Yes" />
                            <asp:ListItem Value="0" Text="No" />
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        R
                    </td>
                    <td runat="server" id="tdHospitalizationsDetails" class="label">
                        <asp:Label runat="server" ID="lblHospitalizationsDetails">Hospitalizations (details):</asp:Label>
                        
                    </td>
                    <td class="value">
                        <asp:TextBox runat="server" ID="txtHospitalizationDetails" Text="" Rows="5" TextMode="MultiLine" Width="80%" MaxLength="250" />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        S
                    </td>
                    <td class="label labelRequired">
                        Risk for Sudden Incapacitation:
                    </td>
                    <td class="value">
                        <asp:DropDownList runat="server" ID="ddlIncapacitationRisk" />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        T
                    </td>
                    <td class="label labelRequired">
                        Recommended Follow Up Interval:
                    </td>
                    <td class="value">
                        <asp:DropDownList runat="server" ID="ddlFollowUpInterval" />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        U
                    </td>
                    <td class="label">
                        DAWG Recommendation:
                    </td>
                    <td class="value">
                        <asp:DropDownList runat="server" ID="ddlDAWGRecommendation" />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        V
                    </td>
                    <td class="label">
                        Dx interfers with in-garrison duties:
                    </td>
                    <td class="value">
                        <asp:RadioButtonList runat="server" ID="rblDutyInterference" RepeatDirection="Horizontal">
                            <asp:ListItem Value="1" Text="Yes" />
                            <asp:ListItem Value="0" Text="No" />
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        W
                    </td>
                    <td class="label labelRequired">
                        Prognosis:
                    </td>
                    <td class="value">
                        <asp:TextBox runat="server" ID="txtPrognosis" Rows="5" TextMode="MultiLine" Width="450" MaxLength="250" />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        X
                    </td>
                    <td class="label labelRequired">
                        Treatment:
                    </td>
                    <td class="value">
                        <asp:TextBox runat="server" ID="txtTreatment" Rows="5" TextMode="MultiLine" Width="450" MaxLength="250" />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        Y
                    </td>
                    <td class="label labelRequired">
                        Modifications/Dosage:
                    </td>
                    <td class="value">
                        <asp:TextBox runat="server" ID="txtMedicationDosages" Rows="5" TextMode="MultiLine" Width="450" MaxLength="250" />
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

            $('.datePickerFuture').datepicker(calendarPick("Future", "<%=CalendarImage %>"));


            $(".numericOnly").keypress(function (e) {
                if (String.fromCharCode(e.keyCode).match(/^[-+]?[0-9]*\.?[0-9]*([eE][-+]?[0-9]+)?$/)) return true;
                return false;
            });
            
            updateCSSClassForRadioButtonListDetailsCheckboxes("<%=rblEROrUrgentCareVisits.UniqueID%>", "<%=tdERorUrgentCareDetails.ClientID%>");
            updateCSSClassForRadioButtonListDetailsCheckboxes("<%=rblHospitalizations.UniqueID%>", "<%=tdHospitalizationsDetails.ClientID%>");
        });

        function updateCSSClassForRadioButtonListDetailsCheckboxes(rblUniqueId, tdClientId) {
            var radioButtons = document.getElementsByName(rblUniqueId);

            for (var i = 0; i < radioButtons.length; i++) {
                if (radioButtons[i].value === "1" && radioButtons[i].checked) {
                    $('#' + tdClientId).addClass('labelRequired');
                }
                else if (radioButtons[i].value === "1" && !radioButtons[i].checked) {
                    $('#' + tdClientId).removeClass('labelRequired');
                }
            }
        }
    </script>
</asp:Content>