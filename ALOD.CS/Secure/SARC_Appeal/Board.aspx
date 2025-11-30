<%@ Page Title="" Language="C#" MasterPageFile="~/Secure/SARC_Appeal/SARC_Appeal.master" AutoEventWireup="false" Inherits="ALOD.Web.APSA.Secure_apsa_Board" CodeBehind="Board.aspx.cs" %>

<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SARC_Appeal/SARC_Appeal.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Reference Control="~/Secure/Shared/UserControls/ModuleHeader.ascx" %>
<%@ Register Src="~/secure/Shared/UserControls/FindingsControl.ascx" TagName="Findings"
    TagPrefix="uc" %>
<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck"
    TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    <asp:Panel runat="server" ID="pnlBoardFindings">
        <asp:Panel runat="server" ID="pnlAppellateAuthorityFindings" CssClass="dataBlock">
            <div class="dataBlock-header">
                Appellate Authority Review:
            </div>
            <div class="dataBlock-body">
                <asp:UpdatePanel runat="server" ID="upnlAppellateAuthorityFindings">
                    <ContentTemplate>
                        <uc:Findings runat="Server" ID="ucAppellateAuthorityFindings" SetDecisionToggle="False" FindingsOnly="True" ShowFormText="True" SetReadOnly="True" ReasonsLabelText="Appellate Authority Remarks:" 
                            DoFindingsAutoPostBack="true" ShownOnText="" Adjustlimit="250" ShowRemarks="false"/>
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

                <uc1:SignatureCheck runat="server" ID="ucSigCheckAppellateAuth" Template="Form348APSARCFindings" CssClass="sigcheck-form no-border" />
            </div>
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlBoardAdminFindings" CssClass="dataBlock">
            <div class="dataBlock-header">
                Board Administrator Review:
            </div>
            <div class="dataBlock-body">
                <uc:Findings runat="Server" ID="ucBoardAdminFindings" SetDecisionToggle="False" FindingsOnly="True" ShowFormText="True" SetReadOnly="True" ReasonsLabelText="Board Administrator Remarks:"
                    ShowRemarks="false" ShownOnText="" Adjustlimit="250" />
                <uc1:SignatureCheck runat="server" ID="ucSigCheckBoardAdmin" Template="Form348APSARCFindings" CssClass="sigcheck-form no-border" />
            </div>
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlBoardMedicalFindings" CssClass="dataBlock">
            <div class="dataBlock-header">
                Board Medical Review:
            </div>
            <div class="dataBlock-body">
                <uc:Findings runat="Server" ID="ucBoardMedicalFindings" SetDecisionToggle="False" FindingsOnly="True" ShowFormText="True" SetReadOnly="True" RemarksLableText="Board Medical Remarks:" 
                    ShowRemarks="false" ShownOnText="" Adjustlimit="250" />
                <uc1:SignatureCheck runat="server" ID="ucSigCheckBoardMedical" Template="Form348APSARCFindings" CssClass="sigcheck-form no-border" />
            </div>
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlBoardJAFindings" CssClass="dataBlock">
            <div class="dataBlock-header">
                Board JA Review:
            </div>
            <div class="dataBlock-body">
                <uc:Findings runat="Server" ID="ucBoardJAFindings" SetDecisionToggle="False" FindingsOnly="True" ShowFormText="True" SetReadOnly="True" RemarksLableText="Board JA Remarks:" 
                    ShowRemarks="false" ShownOnText="" Adjustlimit="250" />
                <uc1:SignatureCheck runat="server" ID="ucSigCheckBoardJA" Template="Form348APSARCFindings" CssClass="sigcheck-form no-border" />
            </div>
        </asp:Panel>
    </asp:Panel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">
  
</asp:Content>
