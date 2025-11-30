<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SC_FastTrack/SC_FastTrack.master" MaintainScrollPositionOnPostback="true" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.IRILO.Secure_sc_FT_MedOff" EnableViewState="True" EnableEventValidation="false" Codebehind="MedOff.aspx.vb" %>

<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_FastTrack/SC_FastTrack.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="../Shared/UserControls/ICDCodeControl.ascx" TagName="ICDCodeControl" TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/ICD7thCharacterControl.ascx" TagName="ICD7thCharacterControl" TagPrefix="uc1" %>

<asp:Content runat="Server" ID="Content1" ContentPlaceHolderID="HeaderNested">
    <style type="text/css">
        .cellpadding td 
        {
            padding: 5px;
        }

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
                        <asp:TextBox ID="DiagnosisTextBox" Width="500px" TextMode="MultiLine" runat="server" Rows="4" MaxLength="250"></asp:TextBox>
                    </td>
                </tr>
            </table>
            <table class="cellpadding">
                <tr>
                    <td>
                        <asp:Label CssClass="label labelRequired" runat="server">Decision:</asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:RadioButtonList runat="server" ID="rblDecision" RepeatDirection="Vertical" AutoPostBack="true" />
                    </td>
                </tr>
                <tr>
                    <td>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="decisionlbl" runat="server">Decision Explanation:</asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:TextBox Height="50" Width="500px" runat="server" ID="DecisionComment" TextMode="MultiLine" MaxLength="250"/>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="DQlbl" CssClass="label labelRequired" runat="server">DQ Paragraph:</asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:TextBox Height="40px" Width="500px" runat="server" ID="DQParagraph" TextMode="MultiLine" MaxLength="250"/>
                    </td>
                </tr>
            </table>

            <table class="cellpadding">
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
                        <asp:Label ID="lblReturnToDutyDateRow" runat="server">Return To Duty Date: </asp:Label>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="ReturnToDutyDate" MaxLength="10" onchange="DateCheck(this);" Text="" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="ProcessRow" runat="server">Process As: </asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlProcess" AutoPostBack="true" runat="server" Width="200px" />
                    </td>
                </tr>
                <tr>
                    <td>
                        Assignment Limitation Code:
                    </td>
                    <td> 
                        <asp:DropDownList ID="ddlAssignmentLimitationCode"
                                          runat="server" Width="150px">
                            <asp:ListItem Value="0" Selected="True">No Limitations</asp:ListItem>
                            <asp:ListItem Value="1">C1</asp:ListItem>
                            <asp:ListItem Value="2">C2</asp:ListItem>
                            <asp:ListItem Value="3">C3</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label CssClass="label labelRequired" runat="server">Memo/Letter:</asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlMemos" runat="server" Width="200px" DataTextField="Title" DataValueField="Id"/>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    
    <asp:Panel runat="server" Id="pnlSeniorMedialReviewerInformation" CssClass="dataBlock">
        <div class="dataBlock-header">
            Senior Medical Reviewer IRILO Decision
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
                    <td class="label">
                        <asp:Label runat="server" ID="lblSeniorMedicalReviewerDecisionExplanationRow" Text="Decision Explanation"/>
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
                                <td class="label">
                                    <asp:Label runat="server" ID="lblSeniorMedicalReviewerDqParagraphRow" Text="DQ Paragraph:"/>
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
                                </td>
                            </tr>
                            <tr>
                                <td class="number">
                                    H
                                </td>
                                <td class="label">
                                    <asp:Label runat="server" ID="lblSeniorMedicalReviewerReturnToDutyDateRow" Text="Return To Duty Date:"/>
                                </td>
                                <td class="value">
                                    <asp:TextBox runat="server" ID="txtSeniorMedicalReviewerReturnToDutyDate" MaxLength="10" onchange="DateCheck(this);" Text="" />
                                </td>
                            </tr>
                            <tr>
                                <td class="number">
                                    I
                                </td>
                                <td class="label">
                                    <asp:Label runat="server" ID="lblSeniorMedicalReviewerProcessAsRow" Text="Process As:"/>
                                </td>
                                <td class="value">
                                    <asp:DropDownList runat="server" ID="ddlSeniorMedicalReviewerProcessAs" Width="200px" AutoPostBack="true"/>
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
            bindDatepickers();
        });

        function pageLoad() {
            bindDatepickers();
        }

        function bindDatepickers() {
            $('.datePicker').datepicker(calendarPick("Past", "<%=CalendarImage %>"));
            
            $('.datePickerAll').datepicker(calendarPick("All", "<%=CalendarImage %>"));
            
            $('.datePickerPlusFuture').datepicker(calendarPick("TomorrowFuture", "<%=CalendarImage %>"));

            $('.datePickerFuture').datepicker(calendarPick("Future", "<%=CalendarImage %>"));
        }
    </script>
</asp:Content>
