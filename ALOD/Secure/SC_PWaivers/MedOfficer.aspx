<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SC_PWaivers/SC_Pwaiver.master" MaintainScrollPositionOnPostback="true" AutoEventWireup="false" EnableEventValidation="false" Inherits="ALOD.Web.Special_Case.PW.Secure_sc_pw_MedOff" Codebehind="MedOfficer.aspx.vb" %>

<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_PWaivers/SC_Pwaiver.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>

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
            Medical Officer Participation Waiver Determination
        </div>
        <div class="dataBlock-body">
            <table>
                <tr>
                    <td>
                        <asp:RadioButton runat="server" ID="DecisionY" GroupName="Decision" Text="Approve Participation Waiver" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:RadioButton runat="server" ID="DecisionN" GroupName="Decision" Text="Deny Participation Waiver" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <br />
                    </td>
                </tr>
                <tr>
                    <td>
                        Determination Explanation
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:TextBox Height="50" Width="500px" runat="server" ID="DecisionComment" TextMode="MultiLine" MaxLength="250"/>
                    </td>
                </tr>
                <tr>
                    <td>
                        <br />
                    </td>
                </tr>
                <tr>
                    <td>
                        Participation Waiver Date
                        <asp:Label ID="PWaiverDateLabel" runat="server" CssClass="lblDisableText"/>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:TextBox ID="PWaiverDateTextBox" MaxLength="10" onchange="DateCheck(this);" runat="server" Width="80"/>
                    </td>
                </tr>
                <tr>
                    <td>
                        <br />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="PWaiverExpirationDateLabel" runat="server" CssClass="lblDisableText" Text="Participation Waiver Expiration Date"/>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:TextBox ID="PWaiverExpirationDateTextBox" MaxLength="10" onchange="DateCheck(this);" runat="server" Width="80"/>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    
    <asp:Panel runat="server" Id="pnlSeniorMedialReviewerInformation" CssClass="dataBlock">
        <div class="dataBlock-header">
            Senior Medical Reviewer Participation Waiver Decision
        </div>
        <div class="dataBlock-body">
            <table>
                <tr>
                    <td>
                        <asp:UpdatePanel runat="server" ID="upnlSeniorMedicalReviewerFindings">
                            <ContentTemplate>
                                <div>
                                    <asp:RadioButton runat="server" ID="rblSeniorMedicalReviewerDecisionY" GroupName="SeniorMedicalReviewerDecision" Value="Y" AutoPostBack="True" Visible="False" Text="Concur with the action of Board Medical" />
                                    <br />
                                    <asp:RadioButton runat="server" ID="rblSeniorMedicalReviewerDecisionN" GroupName="SeniorMedicalReviewerDecision" Value="N" AutoPostBack="True" Visible="False" Text="Non-Concur with the action of Board Medical" />
                                    <asp:Panel runat="server" ID="pnlSeniorMedicalReviewerFindings" CssClass="FindingsIndent" Visible="False">
                                        <asp:RadioButtonList runat="server" ID="rblSeniorMedicalReviewerFindings" RepeatDirection="Vertical">
                                            <asp:ListItem Value="1">Approve Participation Waiver</asp:ListItem>
                                            <asp:ListItem Value="0">Deny Participation Waiver</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </asp:Panel>
                                    <br />
                                    <asp:RadioButton runat="server" ID="rblSeniorMedicalReviewerDecisionR" GroupName="SeniorMedicalReviewerDecision" Value="R" AutoPostBack="True" Visible="False" Text="Return Without Action" />
                                </div>
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
                        <br/>
                        <table>
                            <tr>
                                <td>
                                    Participation Waiver Date
                                    <asp:Label ID="lblSeniorMedicalReviewerPWaiverDate" runat="server" CssClass="lblDisableText"/>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtSeniorMedicalReviewerPWaiverDate" MaxLength="10" onchange="DateCheck(this);" runat="server" Width="80"/>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="lblSeniorMedicalReviewerExpirationDate" runat="server" CssClass="lblDisableText" Text="Participation Waiver Expiration Date"/>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:TextBox ID="txtSeniorMedicalReviewerExpirationDate" MaxLength="10" onchange="DateCheck(this);" runat="server" Width="80"/>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </asp:Panel>
    
    <asp:ObjectDataSource ID="PWCategoriesObjectDataSource" runat="server" SelectMethod="GetPWCategories" TypeName="ALOD.Data.Services.LookupService"></asp:ObjectDataSource>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">
    <script type="text/javascript">

        $(function () {

            $('.datePicker').datepicker(calendarPick("All", "<%=CalendarImage %>"));

        });

        function pageLoad() {
            $('.datePicker').datepicker(calendarPick("All", "<%=CalendarImage %>"));
        }
    
    </script>
</asp:Content>
