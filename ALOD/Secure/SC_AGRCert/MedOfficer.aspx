<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SC_AGRCert/SC_AGRCert.master" MaintainScrollPositionOnPostback="true" AutoEventWireup="false" EnableEventValidation="false" Inherits="ALOD.Web.Special_Case.AGR.Secure_sc_agr_MedOff" Codebehind="MedOfficer.aspx.vb" %>

<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_AGRCert/SC_AGRCert.master" %>
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
    <asp:Panel runat="server" ID="pnlMedicalOfficerInformation" CssClass="dataBlock">
        <div class="dataBlock-header">
            <% If (SpecCase.ALC.HasValue And SpecCase.ALC = 0 And SpecCase.MAJCOM.HasValue And SpecCase.MAJCOM = 0) Then %>
                    Medical Officer (Wing) AGR Decision
            <% Else %>
                    Medical Officer AGR Decision
            <% End If %>
        </div>
        <div class="dataBlock-body">
            <table>
                <tr>
                    <td>
                        <asp:RadioButton runat="server" ID="DecisionY" GroupName="Decision1" Text="Approve AGR Medical Certification" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:RadioButton runat="server" ID="DecisionN" GroupName="Decision1" Text="Deny AGR Medical Certification" />
                    </td>
                </tr>
                <tr>
	                <td> 
		                <asp:RadioButton runat="server" ID="DecisionRFA" GroupName="Decision1" Text="Return For Action" />
	                </td>
                </tr>
                <tr>
                    <td>
                        <br />
                    </td>
                </tr>
                <tr>
                    <td>
                        <span style="color: red">* Determination Explanation</span> 
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
                        AGR Medical Certification Date
                        <asp:Label ID="AGRDateLabel" runat="server" CssClass="lblDisableText"/>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:TextBox ID="AGRDateTextBox" MaxLength="10" onchange="DateCheck(this);" runat="server" Width="80"/>
                    </td>
                </tr>
                <tr>
                    <td>
                        <br />
                    </td>
                </tr>
                
            </table>
        </div>
    </asp:Panel>

    <asp:Panel runat="server" Id="pnlBoardMedialReviewerInformation" CssClass="dataBlock">
        <div class="dataBlock-header">
            Medical Officer (HQ) AGR Decision
        </div>
        <div class="dataBlock-body">
            <table>
                <tr>
                    <td>
                        <asp:RadioButton runat="server" ID="DecisionBMedY" GroupName="Decision2" Text="Approve AGR Medical Certification" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:RadioButton runat="server" ID="DecisionBMedN" GroupName="Decision2" Text="Deny AGR Medical Certification" />
                    </td>
                </tr>
                <tr>
	                <td> 
		                <asp:RadioButton runat="server" ID="DecisionRFABMed" GroupName="Decision2" Text="Return For Action" />
	                </td>
                </tr>
                <tr>
                    <td>
                        <br />
                    </td>
                </tr>
                <tr>
                    <td>
                        <span style="color: red">* Determination Explanation</span>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:TextBox Height="50" Width="500px" runat="server" ID="DecisionCommentBMed" TextMode="MultiLine" MaxLength="250"/>
                    </td>
                </tr>
                <tr>
                    <td>
                        <br />
                    </td>
                </tr>
                <tr>
                    <td>
                        AGR Medical Certification Date
                        <asp:Label ID="AGRAltDateLabel" runat="server" CssClass="lblDisableText"/>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:TextBox ID="AGRAltDateTextBox" MaxLength="10" onchange="DateCheck(this);" runat="server" Width="80" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <br />
                    </td>
                </tr>
     
            </table>
        </div>
    </asp:Panel>
    <asp:Panel runat="server" Id="pnlSeniorMedialReviewerInformation" CssClass="dataBlock">
        <div class="dataBlock-header">
            Medical Officer (HQ SMR) AGR Decision
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
                                        <asp:ListItem Value="1">Approve AGR Medical Certification</asp:ListItem>
                                        <asp:ListItem Value="0">Deny AGR Medical Certification</asp:ListItem>
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
                    <td >
                        <span style="color: red">*  Decision Explanation</span>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:TextBox Height="50" Width="500px" runat="server" ID="txtSeniorMedicalReviewerDecisionComment" TextMode="MultiLine" MaxLength="250"  />
                    </td>
                </tr>
                <tr>
                    <td>
                        <br />
                    </td>
                </tr>
                <tr>
	                    <td>
		                    AGR Medical Certification Date
		                    <asp:Label ID="AGRAlt2DateLabel" runat="server" CssClass="lblDisableText"/>
	                    </td>
                    </tr>
                    <tr>
	                    <td>
		                    <asp:TextBox ID="txtSeniorMedicalReviewerRenewalDate" MaxLength="10" onchange="DateCheck(this);" runat="server" Width="80"/>
	                    </td>
                    </tr>
                <tr>
                    <td>
                        <br />
                    </td>
                </tr>
            </table>
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
