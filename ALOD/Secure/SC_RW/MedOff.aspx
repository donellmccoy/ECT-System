<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SC_RW/SC_RW.master" MaintainScrollPositionOnPostback="true" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.RW.Secure_sc_rw_MedOff" EnableViewState="True" EnableEventValidation="false" Codebehind="MedOff.aspx.vb" %>

<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_RW/SC_RW.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="../Shared/UserControls/ICDCodeControl.ascx" TagName="ICDCodeControl" TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/ICD7thCharacterControl.ascx" TagName="ICD7thCharacterControl" TagPrefix="uc1" %>

<asp:Content runat="Server" ID="Content1" ContentPlaceHolderID="HeaderNested">
    <style type="text/css">
        .FindingsIndent 
        {
            margin-left: 15px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    <asp:Panel runat="server" ID="pnlBoardMedicalInformation" CssClass="dataBlock">
        <div class="dataBlock-header">
            Medical Officer
        </div>
        <div class="dataBlock-body">
            <table>
                <tr>
                    <td class="number">
                        A
                    </td>
                    <td class="label  labelRequired" >
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
                        <asp:Label runat="server" ID="lblDiagnosisLabel" CssClass="lblDisableText" />
                        <asp:TextBox runat="server" ID="txtDiagnosisTextBox" Width="500px" TextMode="MultiLine" Multiline="True" Rows="4" MaxLength="250" />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        D
                    </td>
                    <td class="label labelRequired">
                        Decision:
                    </td>
                    <td class="value">
                        <asp:UpdatePanel runat="server" ID="upnlDecision">
                            <ContentTemplate>
                                <asp:RadioButtonList runat="server" ID="rblDecision" RepeatDirection="Vertical" AutoPostBack="true" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        E
                    </td>
                    <td id="tdDecisionExplanation" class="label">
                        Decision Explanation:
                    </td>
                    <td class="value">
                        <asp:TextBox runat="server" ID="txtDecisionComment" Height="50" Width="500px" TextMode="MultiLine" MaxLength="250" />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        F
                    </td>
                    <td id="tdDQParagraph" class="label">
                        DQ Paragraph:
                    </td>
                    <td class="value">
                        <asp:TextBox runat="server" ID="txtDQParagraph" Height="40px" Width="500px" TextMode="MultiLine" MaxLength="250" />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        G
                    </td>
                    <td class="label">
                        Expiration/Renewal Date:
                    </td>
                    <td class="value">
                        <asp:TextBox runat="server" ID="txtExpirationDate" MaxLength="10" onchange="DateCheck(this);" Text="" />
                        <asp:CheckBox runat="server" ID="chbIndefinite" CssClass="boxPosition" Text="Indefinite" />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        H
                    </td>
                    <td id="tdReturnToDutyDate" class="label">
                        Return To Duty Date:
                    </td>
                    <td class="value">
                        <asp:TextBox runat="server" ID="txtReturnToDutyDate" MaxLength="10" onchange="DateCheck(this);" Text="" />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        I
                    </td>
                    <td id="tdProcessAs" class="label">
                        Process As:
                    </td>
                    <td class="value">
                        <asp:UpdatePanel runat="server" ID="upnlProcessAs">
                            <ContentTemplate>
                                <asp:DropDownList runat="server" ID="ddlProcessAs" Width="200px" AutoPostBack="true">
                                    <asp:ListItem Value="0">-- Select How to Process --</asp:ListItem>
                                    <asp:ListItem Value="1">Full WWD</asp:ListItem>
                                    <asp:ListItem Value="2">Full MEB</asp:ListItem>
                                    <asp:ListItem Value="3">Full Case Directed (TBD)</asp:ListItem>
                                </asp:DropDownList>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        J
                    </td>
                    <td class="label">
                        Assignment Limitation Code:
                    </td>
                    <td class="value">
                        <asp:DropDownList runat="server" ID="ddlAssignmentLimitationCode" Width="200px">
                            <asp:ListItem Value="0" Selected="True">No Limitations</asp:ListItem>
                            <asp:ListItem Value="1">C1</asp:ListItem>
                            <asp:ListItem Value="2">C2</asp:ListItem>
                            <asp:ListItem Value="3">C3</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        K
                    </td>
                    <td class="label labelRequired">
                        Memo/Letter:
                    </td>
                    <td class="value">
                        <asp:UpdatePanel runat="server" ID="upnlMemos">
                            <ContentTemplate>
                                <asp:DropDownList runat="server" ID="ddlMemos" Width="200px" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    
    <asp:Panel runat="server" Id="pnlSeniorMedialReviewerInformation" CssClass="dataBlock">
        <div class="dataBlock-header">
            Senior Medical Reviewer RW Decision
        </div>
        <div class="dataBlock-body">
            <table>
                <tr>
                    <td class="number">
                        A
                    </td>
                    <td class="label labelRequired">
                        Decision/Finding:
                    </td>
                    <td class="value">
                        <asp:UpdatePanel runat="server" ID="upnlSeniorMedicalReviewerFindings">
                            <ContentTemplate>
                                <div>
                                    <asp:RadioButtonList runat="server" ID="rblSeniorMedicalReviewerDecision" RepeatDirection="Vertical" AutoPostBack="True" Visible="False">
                                        <asp:ListItem Value="Y">Concur with the action of Board Medical</asp:ListItem>
                                        <asp:ListItem Value="N">Non-Concur with the action of Board Medical</asp:ListItem>
                                        <asp:ListItem Value="R">Return Without Action</asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                                <asp:Panel runat="server" ID="pnlSeniorMedicalReviewerFindings" CssClass="FindingsIndent" Visible="False">
                                    <asp:RadioButtonList runat="server" ID="rblSeniorMedicalReviewerFindings" RepeatDirection="Vertical" AutoPostBack="True"/>
                                </asp:Panel>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        B
                    </td>
                    <td class="label labelRequired">
                        Decision Explanation
                    </td>
                    <td class="value">
                        <asp:TextBox Height="50" Width="500px" runat="server" ID="txtSeniorMedicalReviewerDecisionComment" TextMode="MultiLine" MaxLength="250" />
                    </td>
                </tr>
            </table>
            
            <asp:UpdatePanel runat="server" ID="upnlSeniorMedicalReviewerAdditionalData">
                <ContentTemplate>
                    <asp:Panel runat="server" ID="pnlSeniorMedicalReviewerAdditionalData" Visible="False">
                        <table>
                            <tr>
                                <td class="number">
                                    C
                                </td>
                                <td class="label  labelRequired" >
                                    *Diagnosis:
                                </td>
                                <td class="value">
                                    <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                        <ContentTemplate>
                                            <uc1:ICDCodeControl runat="server" ID="ucSeniorMedicalReviewerICDCodeControl" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                                <td class="number">
                                    D
                                </td>
                                <td class="label">
                                    7th Character:
                                </td>
                                <td class="value">
                                    <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                                        <ContentTemplate>
                                            <uc1:ICD7thCharacterControl runat="server" ID="ucSeniorMedicalReviewerICD7thCharacterControl" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                                <td class="number">
                                    E
                                </td>
                                <td class="label labelRequired">
                                    *Diagnosis Text:
                                </td>
                                <td class="value">
                                    <asp:Label runat="server" ID="lblSeniorMedicalReviewerDiagnosisText" CssClass="lblDisableText" />
                                    <asp:TextBox runat="server" ID="txtSeniorMedicalReviewerDiagnosisText" Width="500px" TextMode="MultiLine" Multiline="True" Rows="4" MaxLength="250" />
                                </td>
                            </tr>
                            <tr>
                                <td class="number">
                                    F
                                </td>
                                <td id="tdSeniorMedicalReviewerDQParagraph" class="label">
                                    DQ Paragraph:
                                </td>
                                <td class="value">
                                    <asp:TextBox runat="server" ID="txtSeniorMedicalReviewerDQParagraph" Height="40px" Width="500px" TextMode="MultiLine" MaxLength="250" />
                                </td>
                            </tr>
                            <tr>
                                <td class="number">
                                    G
                                </td>
                                <td class="label">
                                    Expiration/Renewal Date:
                                </td>
                                <td class="value">
                                    <asp:TextBox runat="server" ID="txtSeniorMedicalReviewerExpirationDate" MaxLength="10" onchange="DateCheck(this);" Text="" />
                                    <asp:CheckBox runat="server" ID="chbSeniorMedicalReviewerIndefinite" CssClass="boxPosition" Text="Indefinite" />
                                </td>
                            </tr>
                            <tr>
                                <td class="number">
                                    H
                                </td>
                                <td id="tdSeniorMedicalReviewerReturnToDutyDate" class="label">
                                    Return To Duty Date:
                                </td>
                                <td class="value">
                                    <asp:TextBox runat="server" ID="txtSeniorMedicalReviewerReturnToDutyDate" MaxLength="10" onchange="DateCheck(this);" Text="" />
                                </td>
                            </tr>
                            <tr>
                                <td class="number">
                                    I
                                </td>
                                <td id="tdSeniorMedicalReviewerProcessAs" class="label">
                                    Process As:
                                </td>
                                <td class="value">
                                    <asp:UpdatePanel runat="server" ID="upnlSeniorMedicalReviewerProcessAs">
                                        <ContentTemplate>
                                            <asp:DropDownList runat="server" ID="ddlSeniorMedicalReviewerProcessAs" Width="200px" AutoPostBack="true">
                                                <asp:ListItem Value="0">-- Select How to Process --</asp:ListItem>
                                                <asp:ListItem Value="1">Full WWD</asp:ListItem>
                                                <asp:ListItem Value="2">Full MEB</asp:ListItem>
                                                <asp:ListItem Value="3">Full Case Directed (TBD)</asp:ListItem>
                                            </asp:DropDownList>
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                                <td class="number">
                                    J
                                </td>
                                <td class="label">
                                    Assignment Limitation Code:
                                </td>
                                <td class="value">
                                    <asp:DropDownList runat="server" ID="ddlSeniorMedicalReviewerAssignmentLimitationCode" Width="200px">
                                        <asp:ListItem Value="0" Selected="True">No Limitations</asp:ListItem>
                                        <asp:ListItem Value="1">C1</asp:ListItem>
                                        <asp:ListItem Value="2">C2</asp:ListItem>
                                        <asp:ListItem Value="3">C3</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td class="number">
                                    K
                                </td>
                                <td class="label labelRequired">
                                    Memo/Letter:
                                </td>
                                <td class="value">
                                    <asp:UpdatePanel runat="server" ID="upnlSeniorMedicalReviewerMemos">
                                        <ContentTemplate>
                                            <asp:DropDownList runat="server" ID="ddlSeniorMedicalReviewerMemos" Width="200px" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </asp:Panel>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">
    <script type="text/javascript">
        $(function () {
            bindDatePickers();
            updateCSSClassForDecisionDependentLabels("<%=rblDecision.UniqueID%>", '');
            updateCSSClassForDecisionDependentLabels("<%=rblSeniorMedicalReviewerFindings.UniqueID%>", 'SeniorMedicalReviewer');
        });

        function pageLoad() {
            bindDatePickers();
            updateCSSClassForDecisionDependentLabels("<%=rblDecision.UniqueID%>");
            updateCSSClassForDecisionDependentLabels("<%=rblSeniorMedicalReviewerFindings.UniqueID%>", 'SeniorMedicalReviewer');
        }

        function bindDatePickers() {
            $('.datePicker').datepicker(calendarPick("Past", "<%=CalendarImage %>"));

            $('.datePickerAll').datepicker(calendarPick("All", "<%=CalendarImage %>"));

            $('.datePickerPlusFuture').datepicker(calendarPick("TomorrowFuture", "<%=CalendarImage %>"));

            $('.datePickerFuture').datepicker(calendarPick("Future", "<%=CalendarImage %>"));
        }

        function updateCSSClassForDecisionDependentLabels(rblUniqueId, additionalIdText) {
            var radioButtons = document.getElementsByName(rblUniqueId);
            var disqualify = '<%= DisqualifyDecisionValue%>';
            var qualify = '<%= QualifyDecisionValue%>';
            var adminDisqualify = '<%= AdminDisqualifiedDecisionValue%>';
            var adminQualify = '<%= AdminQualifiedDecisionValue%>';
            var isDisqualifyOptionSelected = false;
            var isQualifyOptionSelected = false;

            var tableDataDecisionExplanationId = 'td' + additionalIdText + 'DecisionExplanation';
            var tableDataDQParagraphId = 'td' + additionalIdText + 'DQParagraph';
            var tableDataProcessAsId = 'td' + additionalIdText + 'ProcessAs';
            var tableDataReturnToDutyDateId = 'td' + additionalIdText + 'ReturnToDutyDate';

            for (var i = 0; i < radioButtons.length; i++) {
                if (radioButtons[i].value === disqualify || radioButtons[i].value === adminDisqualify) {
                    if (radioButtons[i].checked) {
                        isDisqualifyOptionSelected = true;
                    }
                }

                if (radioButtons[i].value === qualify || radioButtons[i].value === adminQualify) {
                    if (radioButtons[i].checked) {
                        isQualifyOptionSelected = true;
                    }
                }
            }

            if (isDisqualifyOptionSelected) {
                $('#' + tableDataDecisionExplanationId).addClass('labelRequired');
                $('#' + tableDataDQParagraphId).addClass('labelRequired');
                $('#' + tableDataProcessAsId).addClass('labelRequired');
            }
            else {
                $('#' + tableDataDecisionExplanationId).removeClass('labelRequired');
                $('#' + tableDataDQParagraphId).removeClass('labelRequired');
                $('#' + tableDataProcessAsId).removeClass('labelRequired');
            }

            if (isQualifyOptionSelected) {
                $('#' + tableDataReturnToDutyDateId).addClass('labelRequired');
            }
            else {
                $('#' + tableDataReturnToDutyDateId).removeClass('labelRequired');
            }
        }

        function SetIndefinite(textboxUniqueId) {
            var tx = document.getElementsByName(textboxUniqueId)[0];
            var infiniteDate = '<%= InfiniteDateString%>';

            if (tx.value != infiniteDate || tx.value.length == 0) {
                tx.value = infiniteDate;
            }
            else if (tx.value == infiniteDate) {
                tx.value = "";
            }
        }
    </script>
</asp:Content>
