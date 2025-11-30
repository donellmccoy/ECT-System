<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SC_RS/SC_RS.master" MaintainScrollPositionOnPostback="true" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.RS.Secure_sc_rs_MedOff" Codebehind="MedOff.aspx.vb" %>

<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Register Src="~/secure/Shared/UserControls/FindingsControl.ascx" TagName="Findings"
    TagPrefix="uc" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_RS/SC_RS.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck"
    TagPrefix="uc1" %>

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
            Medical Officer RS Decision
        </div>
        <div class="dataBlock-body">
            <table>
                <tr>
                    <td class="number">
                        A
                    </td>
                    <td class="label  labelRequired">
                        *Determination:
                    </td>
                    <td>
                        <asp:RadioButton runat="server" ID="DecisionY" GroupName="Decision" Text="Qualify" />
                        <br />
                        <asp:RadioButton runat="server" ID="DecisionN" GroupName="Decision" Text="Disqualify" />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        B
                    </td>
                    <td class="label  labelRequired">
                        *Decision Explanation:
                    </td>
                    <td>
                        <asp:TextBox Height="50" Width="500px" runat="server" ID="DecisionComment" TextMode="MultiLine" MaxLength="500" />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        C
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
                <tr>
                    <td class="number">
                        D
                    </td>
                    <td class="label">
                        Renewal Date:
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="txtRenewalDate" MaxLength="10" onchange="DateCheck(this);" Text="" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    
    <asp:Panel runat="server" Id="pnlSeniorMedialReviewerInformation" CssClass="dataBlock">
        <div class="dataBlock-header">
            Senior Medical Reviewer RS Decision
        </div>
        <div class="dataBlock-body">
            <table>
                <tr>
                    <td class="number">
                        A
                    </td>
                    <td class="label">
                        *Decision/Determination:
                    </td>
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
                                    <asp:RadioButtonList runat="server" ID="rblSeniorMedicalReviewerFindings" RepeatDirection="Vertical">
                                        <asp:ListItem Value="1">Qualify</asp:ListItem>
                                        <asp:ListItem Value="0">Disqualify</asp:ListItem>
                                    </asp:RadioButtonList>
                                </asp:Panel>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        B
                    </td>
                    <td class="label  labelRequired">
                        *Decision Explanation:
                    </td>
                    <td>
                        <asp:TextBox Height="50" Width="500px" runat="server" ID="txtSeniorMedicalReviewerDecisionComment" TextMode="MultiLine" MaxLength="250" />
                    </td>
                </tr>
            </table>
            <asp:UpdatePanel runat="server" ID="upnlSeniorMedicalReviewerAdditionalData">
                <ContentTemplate>
                    <asp:Panel runat="server" ID="pnlSeniorMedicalReviewerAdditionalData" Visible="False">
                        <br/>
                        <table>
                            <tr>
                                <td class="number">
                                    C
                                </td>
                                <td class="label">
                                    Assignment Limitation Code:
                                </td>
                                <td>
                                    <asp:DropDownList runat="server" ID="ddlSeniorMedicalReviewerALC" Width="150px">
                                        <asp:ListItem Value="0" Selected="True">No Limitations</asp:ListItem>
                                        <asp:ListItem Value="1">C1</asp:ListItem>
                                        <asp:ListItem Value="2">C2</asp:ListItem>
                                        <asp:ListItem Value="3">C3</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td class="number">
                                    D
                                </td>
                                <td class="label">
                                    Renewal Date:
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtSeniorMedicalReviewerRenewalDate" MaxLength="10" onchange="DateCheck(this);" Text="" />
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
            $('.datePickerPlusFuture').datepicker(calendarPick("TomorrowFuture", "<%=CalendarImage %>"));
        });

        function pageLoad() {
            $('.datePickerPlusFuture').datepicker(calendarPick("TomorrowFuture", "<%=CalendarImage %>"));
        }

    </script>
</asp:Content>
