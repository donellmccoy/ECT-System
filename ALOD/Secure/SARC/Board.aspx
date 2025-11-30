<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SARC/SARCMaster.master" AutoEventWireup="false" Inherits="ALOD.Web.SARC.Board" CodeBehind="Board.aspx.vb" MaintainScrollPositionOnPostback="true"%>

<%@ Import Namespace="ALODWebUtility.Common" %>

<%@ MasterType VirtualPath="~/Secure/SARC/SARCMaster.master" %>

<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>

<%@ Register Src="~/Secure/Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck" TagPrefix="uc1" %>
<%@ Register Src="~/Secure/Shared/UserControls/FindingsControl.ascx" TagName="Findings" TagPrefix="uc" %>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    <asp:Panel runat="server" ID="pnlBoardFindings">
        <asp:Panel runat="server" ID="pnlApprovingAuthorityFindings" CssClass="dataBlock">
            <div class="dataBlock-header">
                Approving Authority Review:
            </div>
            <div class="dataBlock-body">
                <asp:UpdatePanel runat="server" ID="upnlApprovingAuthorityFindings">
                    <ContentTemplate>
                        <uc:Findings runat="Server" ID="ucApprovingAuthorityFindings" SetDecisionToggle="False" FindingsOnly="True" ShowFormText="False" SetReadOnly="True" RemarksLableText="Approving Authority Remarks:<br />(Not Shown on Form 348-R)" DoFindingsAutoPostBack="true" />
                        <table runat="server" id="tblConsultation" visible="false">
                            <tr>
                                <td class="number">
                            
                                </td>
                                <td class="label">
                                    Consultation From:&nbsp;
                                </td>
                                <td>
                                    <asp:DropDownList runat="server" ID="ddlBoardMembers" Visible="false" />
                                    <asp:Label runat="server" ID="lblBoardMember" Visible="false" />
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>

                <uc1:SignatureCheck runat="server" ID="ucSigCheckApprovingAuth" Template="Form348SARCFindings" CssClass="sigcheck-form no-border" />
            </div>
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlBoardAdminFindings" CssClass="dataBlock">
            <div class="dataBlock-header">
                Board Administrator Review:
            </div>
            <div class="dataBlock-body">
                <uc:Findings runat="Server" ID="ucBoardAdminFindings" SetDecisionToggle="False" FindingsOnly="True" ShowFormText="False" SetReadOnly="True" RemarksLableText="Board Administrator Remarks:<br />(Not Shown on Form 348-R)" />
                <uc1:SignatureCheck runat="server" ID="ucSigCheckBoardAdmin" Template="Form348SARCFindings" CssClass="sigcheck-form no-border" />
            </div>
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlBoardMedicalFindings" CssClass="dataBlock">
            <div class="dataBlock-header">
                Board Medical Review:
            </div>
            <div class="dataBlock-body">
                <uc:Findings runat="Server" ID="ucBoardMedicalFindings" SetDecisionToggle="False" FindingsOnly="True" ShowFormText="False" SetReadOnly="True" RemarksLableText="Board Medical Remarks:<br />(Not Shown on Form 348-R)" />
                <uc1:SignatureCheck runat="server" ID="ucSigCheckBoardMedical" Template="Form348SARCFindings" CssClass="sigcheck-form no-border" />
            </div>
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlBoardJAFindings" CssClass="dataBlock">
            <div class="dataBlock-header">
                Board JA Review:
            </div>
            <div class="dataBlock-body">
                <uc:Findings runat="Server" ID="ucBoardJAFindings" SetDecisionToggle="False" FindingsOnly="True" ShowFormText="False" SetReadOnly="True" RemarksLableText="Board JA Remarks:<br />(Not Shown on Form 348-R)" />
                <uc1:SignatureCheck runat="server" ID="ucSigCheckBoardJA" Template="Form348SARCFindings" CssClass="sigcheck-form no-border" />
            </div>
        </asp:Panel>
    </asp:Panel>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">
  
</asp:Content>