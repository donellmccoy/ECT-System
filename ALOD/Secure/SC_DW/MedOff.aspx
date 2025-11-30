<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SC_DW/SC_DW.master" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.DW.Secure_SC_DW_MedOff" Codebehind="MedOff.aspx.vb" %>


<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_DW/SC_DW.master" %>
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
            Medical Officer DW Decision
        </div>
        <div class="dataBlock-body">
            <table>
                <tr>
                    <td>
                        <asp:RadioButton runat="server" ID="DecisionY" GroupName="Decision" Text="Approve Deployment Waiver" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:RadioButton runat="server" ID="DecisionN" GroupName="Decision" Text="Deny Deployment Waiver" />
                    </td>
                </tr>
                <tr>
                    <td>
                        Decision Explanation
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:TextBox Height="50" Width="500px" runat="server" ID="DecisionComment" TextMode="MultiLine" MaxLength="250"/>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    
    <asp:Panel runat="server" Id="pnlSeniorMedialReviewerInformation" CssClass="dataBlock">
        <div class="dataBlock-header">
            Senior Medical Reviewer DW Decision
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
                                        <asp:ListItem Value="1">Approve Deployment Waiver</asp:ListItem>
                                        <asp:ListItem Value="0">Deny Deployment Waiver</asp:ListItem>
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
        </div>
    </asp:Panel>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">
</asp:Content>

