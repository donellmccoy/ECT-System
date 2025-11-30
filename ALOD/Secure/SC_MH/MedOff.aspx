<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SC_MH/SC_MH.master" MaintainScrollPositionOnPostback="true"
    AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.MH.Secure_sc_mh_MedOff" EnableEventValidation="false" Codebehind="MedOff.aspx.vb" %>

<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_MH/SC_MH.master" %>
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
            Medical Officer Determination
        </div>

        <div class="dataBlock-body">
            <table>
                <tr>
                    <td>
                        <asp:RadioButton runat="server" ID="DecisionY" GroupName="Decision" Text="Approve Med Hold Request" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:RadioButton runat="server" ID="DecisionN" GroupName="Decision" Text="Disapprove Med Hold Request" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <br />
                    </td>
                </tr>
                <tr>
                    <td>
                         Decision Explanation
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:TextBox Height="50" Width="500px" runat="server" ID="DecisionComment" TextMode="MultiLine" MaxLength="250" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <br />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblExpirationDateTitle" runat="server" CssClass="label labelRequired" Text="* Expiration Date" />
                        <asp:TextBox ID="txtExpirationDate" MaxLength="10" onchange="DateCheck(this);" runat="server" Width="80" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Label ID="lblExperationDate" runat="server" CssClass="lblDisableText" />
                        <br />
                        <br />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>

    <asp:Panel runat="server" Id="pnlSeniorMedialReviewerInformation" CssClass="dataBlock">
        <div class="dataBlock-header">
            Senior Medical Reviewer Determination
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
                                    <asp:RadioButtonList runat="server" ID="rblSeniorMedicalReviewerFindings" RepeatDirection="Vertical">
                                        <asp:ListItem Value="1">Approve Med Hold Request</asp:ListItem>
                                        <asp:ListItem Value="0">Disapprove Med Hold Request</asp:ListItem>
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
                        <asp:TextBox Height="50" Width="500px" runat="server" ID="SeniorMedicalReviewerDecisionComment" TextMode="MultiLine" MaxLength="250" />
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
                                    <asp:Label ID="lblSeniorMedicalReviewerExperationDateTitle" runat="server" CssClass="label labelRequired" Text="* Expiration Date" />
                                    <asp:TextBox ID="txtSeniorMedialReviewExperationDate" MaxLength="10" onchange="DateCheck(this);" runat="server" Width="80" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <asp:Label ID="lblSeniorMedicalReviewerExperationDate" runat="server" CssClass="lblDisableText" />
                                    <br />
                                    <br />
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

            $('.datePicker').datepicker(calendarPick("All", "<%=CalendarImage %>"));

        });        

        function pageLoad() {

            $('.datePicker').datepicker(calendarPick("All", "<%=CalendarImage %>"));
        }
    
    </script>
</asp:Content>
