<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SC_MEB/SC_MEB.master" MaintainScrollPositionOnPostback="true" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.MEB.Secure_sc_meb_MedOff" EnableEventValidation="false" CodeBehind="MedOff.aspx.vb" %>


<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_MEB/SC_MEB.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="../Shared/UserControls/ICDCodeControl.ascx" TagName="ICDCodeControl" TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/ICD7thCharacterControl.ascx" TagName="ICD7thCharacterControl" TagPrefix="uc1" %>

<asp:Content runat="Server" ID="Content1" ContentPlaceHolderID="HeaderNested">
    <style type="text/css">
        .FindingsIndent {
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
                        <asp:UpdatePanel runat="server" ID="UpdatePanel5">
                            <ContentTemplate>
                                <div>
                                    <asp:RadioButtonList runat="server" ID="rblDecision" RepeatDirection="Vertical" AutoPostBack="True">
                                        <asp:ListItem Value="1">Concur with DAWG Recommendation</asp:ListItem>
                                        <asp:ListItem Value="0">Non-Concur with DAWG Recommendation</asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td>
                        <br />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lblDecisionExplanation" runat="server" LabelFor="DecisionComment">Decision Explanation</asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:TextBox Height="50" Width="500px" runat="server" ID="txtDecisionComment" TextMode="MultiLine" MaxLength="250"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <br />
                    </td>
                </tr>
            </table>
            <table style="border-spacing: 5px">
                <tr>
                    <td>
                        <asp:Label ID="Label1" runat="server">*Diagnosis:</asp:Label>
                    </td>
                    <td class="value">
                        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                            <ContentTemplate>
                                <uc1:ICDCodeControl runat="server" ID="ucICDCodeControl" />
                                <br />
                                &nbsp;&nbsp; * - Changing this selection requires moving to a new tab and returning to synchronize data
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label2" runat="server">7th Character:</asp:Label>
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
                    <td>
                        <asp:Label ID="Label55" runat="server">Diagnosis Text:</asp:Label>
                    </td>
                    <td class="value">
                        <asp:Label ID="DiagnosisLabel" runat="server" CssClass="lblDisableText"></asp:Label>
                        <asp:TextBox ID="txtDiagnosis" Width="500px" TextMode="MultiLine" runat="server"
                            Rows="4" MaxLength="250"></asp:TextBox>
                    </td>
                </tr>
                <br />
                <asp:RadioButtonList runat="server" ID="rblDetermination" GroupName="Determination">
                    <asp:ListItem Value="1" Text="Return To Duty" />
                    <asp:ListItem Value="0" Text="Disqualify" />
                </asp:RadioButtonList>
                <tr>
                    <br />
                    <td>
                        <asp:Label runat="server" ID="Label5" LabelFor="ddlAssignmentLimitation">Assignment Limitation: </asp:Label>
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlAssignmentLimitation" runat="server">
                            <asp:ListItem Value="0" Text="--- Select One ---" Selected="True"></asp:ListItem>
                            <asp:ListItem Value="1" Text="ALC1"></asp:ListItem>
                            <asp:ListItem Value="2" Text="ALC2"></asp:ListItem>
                            <asp:ListItem Value="3" Text="ALC3"></asp:ListItem>
                            <asp:ListItem Value="4" Text="No ALC"></asp:ListItem>
                            <asp:ListItem Value="5" Text="No ALC (indefinite)"></asp:ListItem>
                            <asp:ListItem Value="6" Text="Discharged"></asp:ListItem>
                            <asp:ListItem Value="7" Text="Retired"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <br />
                <br />
                <tr>
                    <td>Memo/Letter:
                    </td>
                    <td>
                        <asp:DropDownList ID="ddlMemos" runat="server" DataSourceID="ObjectDataSource1"
                            DataTextField="Title" DataValueField="Id">
                        </asp:DropDownList>
                        <asp:ObjectDataSource ID="ObjectDataSource1" runat="server"
                            OldValuesParameterFormatString="original_{0}" SelectMethod="GetTemplatesByModule"
                            TypeName="ALOD.Data.MemoDao2">
                            <SelectParameters>
                                <asp:Parameter DefaultValue="11" Name="module" Type="Byte" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>

    <asp:Panel runat="server" ID="pnlSeniorMedialReviewerInformation" CssClass="dataBlock">
        <div class="dataBlock-header">
            Senior Medical Reviewer Determination
        </div>
        <div class="dataBlock-body">
            <table>
                <tr>
                    <td>
                        <asp:UpdatePanel runat="server" ID="upnlSeniorMedicalReviewerFindings" Visible="true">
                            <ContentTemplate>
                                <div>
                                    <asp:RadioButtonList runat="server" ID="rblSeniorMedicalReviewerConcur" RepeatDirection="Vertical" AutoPostBack="True" Visible="False">
                                        <asp:ListItem Value="Y">Concur with the action of Board Medical</asp:ListItem>
                                        <asp:ListItem Value="N">Non-Concur with the action of Board Medical</asp:ListItem>
                                        <asp:ListItem Value="R">Return Without Action</asp:ListItem>
                                    </asp:RadioButtonList>
                                </div>
                                <asp:Panel runat="server" ID="pnlSeniorMedicalReviewerFindings" CssClass="FindingsIndent" Visible="False">
                                    <asp:RadioButtonList runat="server" ID="rblSeniorMedicalReviewerFindings" RepeatDirection="Vertical" AutoPostBack="True">
                                        <asp:ListItem Value="1">Return To Duty</asp:ListItem>
                                        <asp:ListItem Value="0">Disqualify</asp:ListItem>
                                    </asp:RadioButtonList>
                                </asp:Panel>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td>
                        <br />
                    </td>
                </tr>
                <tr>
                    <td>Decision Explanation
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:TextBox Height="50" Width="500px" runat="server" ID="txtSeniorMedicalReviewerDecisionComment" TextMode="MultiLine" MaxLength="250" />
                    </td>
                </tr>
            </table>
            <br />
            <asp:UpdatePanel runat="server" ID="upnlSeniorMedicalReviewerAdditionalData">
                <ContentTemplate>
                    <asp:Panel runat="server" ID="pnlSeniorMedicalReviewerAdditionalData" Visible="False">
                        <table>
                            <tr>
                                <td>
                                    <asp:RadioButtonList runat="server" ID="rblSeniorMedicalReviewerDecision" RepeatDirection="Vertical" AutoPostBack="True">
                                        <asp:ListItem Value="1">Concur with DAWG Recommendation</asp:ListItem>
                                        <asp:ListItem Value="0">Non-Concur with DAWG Recommendation</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                        </table>
                        <table style="border-spacing: 5px">
                            <tr>
                                <td>
                                    <asp:Label ID="Label3" runat="server">*Diagnosis:</asp:Label>
                                </td>
                                <td class="value">
                                    <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                        <ContentTemplate>
                                            <uc1:ICDCodeControl runat="server" ID="ucSeniorMedicalReviewerICDCodeControl" />
                                            <br />
                                            &nbsp;&nbsp; * - Changing this selection requires moving to a new tab and returning to synchronize data
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label ID="Label4" runat="server">7th Character:</asp:Label>
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
                                <td>
                                    <asp:Label ID="Label6" runat="server">Diagnosis Text:</asp:Label>
                                </td>
                                <td class="value">
                                    <asp:Label ID="Label7" runat="server" CssClass="lblDisableText"></asp:Label>
                                    <asp:TextBox ID="txtSeniorMedicalReviewerDiagnosis" Width="500px" TextMode="MultiLine" runat="server"
                                        Rows="4" MaxLength="250"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Label runat="server" ID="Label8" LabelFor="ddlAssignmentLimitation">Assignment Limitation: </asp:Label>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlSeniorMedicalReviewerAssignmentLimitation" runat="server">
                                        <asp:ListItem Value="0" Text="--- Select One ---" Selected="True"></asp:ListItem>
                                        <asp:ListItem Value="1" Text="ALC1"></asp:ListItem>
                                        <asp:ListItem Value="2" Text="ALC2"></asp:ListItem>
                                        <asp:ListItem Value="3" Text="ALC3"></asp:ListItem>
                                        <asp:ListItem Value="4" Text="No ALC"></asp:ListItem>
                                        <asp:ListItem Value="5" Text="No ALC (indefinite)"></asp:ListItem>
                                        <asp:ListItem Value="6" Text="Discharged"></asp:ListItem>
                                        <asp:ListItem Value="7" Text="Retired"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td>Memo/Letter:
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlSeniorMedicalReviewerMemos" runat="server" DataSourceID="ObjectDataSource1"
                                        DataTextField="Title" DataValueField="Id">
                                    </asp:DropDownList>
                                    <asp:ObjectDataSource ID="ObjectDataSource2" runat="server"
                                        OldValuesParameterFormatString="original_{0}" SelectMethod="GetTemplatesByModule"
                                        TypeName="ALOD.Data.MemoDao2">
                                        <SelectParameters>
                                            <asp:Parameter DefaultValue="11" Name="module" Type="Byte" />
                                        </SelectParameters>
                                    </asp:ObjectDataSource>
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
</asp:Content>
