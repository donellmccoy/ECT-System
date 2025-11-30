<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SC_PEPP/SC_PEPP.master" MaintainScrollPositionOnPostback="true" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.PEPP.Secure_sc_pepp_MedOff" EnableViewState="True" EnableEventValidation="false" Codebehind="MedOff.aspx.vb" %>

<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_PEPP/SC_PEPP.master" %>
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
                    <td class="label" >
                        Diagnosis:
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
                    <td class="label">
                        Diagnosis Text:
                    </td>
                    <td class="value">
                        <asp:Label ID="DiagnosisLabel" runat="server" CssClass="lblDisableText"></asp:Label>
                        <asp:TextBox ID="DiagnosisTextBox" Width="500px" TextMode="MultiLine" runat="server" Multiline="True" Rows="4" MaxLength="250"></asp:TextBox>
                    </td>
                </tr>
            </table>
            <asp:UpdatePanel ID="upnlCaseData" runat="server" ChildrenAsTriggers="True">
                <ContentTemplate>
                    <table>
                        <tr>
                            <td>
                                <asp:RadioButton runat="server" ID="DecisionY" GroupName="Decision" Text="Qualify" AutoPostBack="true" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:RadioButton runat="server" ID="DecisionN" GroupName="Decision" Text="Disqualify" AutoPostBack="true" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                &nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td>Decision Explanation:</td>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox Height="50" Width="500px" runat="server" ID="DecisionComment" TextMode="MultiLine" MaxLength="250"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>DQ Paragraph:</td>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox Height="40px" Width="500px" runat="server" ID="DQParagraph" TextMode="MultiLine" MaxLength="250"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <table>
                        <tr>
                            <td>
                                <asp:Label ID="Label15" runat="server">Expiration/Renewal Date: </asp:Label>
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="ExpirationDate" MaxLength="10" onchange="DateCheck(this);" Text="" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:Label ID="Label3" runat="server">Certification Date:</asp:Label>
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="txtCertificationDate" MaxLength="10" onchange="DateCheck(this);" Text="" />
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </asp:Panel>
    
    <asp:Panel runat="server" Id="pnlSeniorMedialReviewerInformation" CssClass="dataBlock">
        <div class="dataBlock-header">
            Senior Medical Reviewer PEPP Decision
        </div>
        <div class="dataBlock-body">
            <table>
                <tr>
                    <td>
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
                                    <asp:RadioButtonList runat="server" ID="rblSeniorMedicalReviewerFindings" RepeatDirection="Vertical" AutoPostBack="True">
                                        <asp:ListItem Value="1">Qualify</asp:ListItem>
                                        <asp:ListItem Value="0">Disqualify</asp:ListItem>
                                    </asp:RadioButtonList>
                                </asp:Panel>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td>
                        <br/>
                    </td>
                </tr>
                <tr>
                    <td>
                        Decision Explanation
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:TextBox Height="50" Width="500px" runat="server" ID="txtSeniorMedicalReviewerDecisionComment" TextMode="MultiLine" MaxLength="250" />
                    </td>
                </tr>
            </table>
            <asp:UpdatePanel runat="server" ID="upnlSeniorMedicalReviewerAdditionalData">
                <ContentTemplate>
                    <asp:Panel runat="server" ID="pnlSeniorMedicalReviewerAdditionalData" Visible="False">
                        <table>
                            <tr>
                                <td>
                                    Diagnosis:
                                </td>
                                <td>
                                    <asp:UpdatePanel runat="server" ID="UpdatePanel4">
                                        <ContentTemplate>
                                            <uc1:ICDCodeControl runat="server" ID="ucSeniorMedicalReviewerICDCodeControl" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    7th Character:
                                </td>
                                <td>
                                    <asp:UpdatePanel runat="server" ID="UpdatePanel3">
                                        <ContentTemplate>
                                            <uc1:ICD7thCharacterControl runat="server" ID="ucSeniorMedicalReviewerICD7thCharacterControl" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    Diagnosis Text:
                                </td>
                                <td>
                                    <asp:Label runat="server" ID="lblSeniorMedicalReviewerDiagnosisText" CssClass="lblDisableText"/>
                                    <asp:TextBox runat="server" ID="txtSeniorMedicalReviewerDiagnosisText" Width="500px" TextMode="MultiLine" Multiline="True" Rows="4" MaxLength="250"/>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    DQ Paragraph:
                                </td>
                                <td>
                                    <asp:TextBox Height="40px" Width="500px" runat="server" ID="txtSeniorMedicalReviewerDQParagraph" TextMode="MultiLine" MaxLength="250" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label1" runat="server">Expiration/Renewal Date: </asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtSeniorMedicalReviewerExpirationDate" MaxLength="10" onchange="DateCheck(this);" Text="" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label2" runat="server">Certification Date:</asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtSeniorMedicalReviewerCertificationDate" MaxLength="10" onchange="DateCheck(this);" Text="" />
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

        function pageLoad() {
            $('.datePicker').datepicker(calendarPick("Past", "<%=CalendarImage %>"));

            $('.datePickerAll').datepicker(calendarPick("All", "<%=CalendarImage %>"));

            $('.datePickerPlusFuture').datepicker(calendarPick("TomorrowFuture", "<%=CalendarImage %>"));

            $('.datePickerFuture').datepicker(calendarPick("Future", "<%=CalendarImage %>"));

            initMultiLines();
        }

    </script>
</asp:Content>
