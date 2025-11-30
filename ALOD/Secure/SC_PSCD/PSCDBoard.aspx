<%@ Page Language="VB" MasterPageFile="~/Secure/SC_PSCD/SC_PSCD.master" AutoEventWireup="false" Async="true" Inherits="ALOD.Web.Special_Case.PSCD.Secure_PSCD_PSCDBoard" Title="Untitled Page" Codebehind="PSCDBoard.aspx.vb" %>

<%@ Import Namespace="ALODWebUtility.Common" %>
<%@ Import Namespace="ALOD.Core.Domain.Users" %>

<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_PSCD/SC_PSCD.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Reference Control="~/Secure/Shared/UserControls/ModuleHeader.ascx" %>
<%@ Register Src="~/secure/Shared/UserControls/FindingsControl.ascx" TagName="Findings"
    TagPrefix="uc" %>
<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck"
    TagPrefix="uc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentNested" runat="Server">

    <asp:Panel runat="server" ID="pnlBoardFindings" CssClass="dataBlock">
        <div class="dataBlock" runat="server">
            <div class="dataBlock-header">
                Board Tech Review
            </div>
            <div >
            
                </div>
            
                <div class="dataBlock-body">
                <uc:Findings ID="ucHQMedTechFindings"  FindingsOnly="true" ShowFormText="True" FormFindingsText="" SetReadOnly="True" runat="Server" ShownOnText="(Shown on PSC - D Findings Memo)"
                    RemarksLableText="Findings: (Shown on PSC - D Commentary WorkSheet)" FindingsRequired="true" FindingsLableText="* Findings:"></uc:Findings>
            </div>
        </div>
        

        <div class="dataBlock" runat="server">
            <div class="dataBlock-header">
                Board Medical Review
            </div>
            <div class="dataBlock-body">
                <uc:Findings ID="ucBoardMedicalFindings"  FindingsOnly="true" ShowFormText="True" FormFindingsText="" SetReadOnly="True" runat="Server" ShownOnText="(Shown on PSC - D Findings Memo)"
                    RemarksLableText="Medical Review and Recommendations: (Shown on PSC - D Commentary WorkSheet)" FindingsRequired="true" FindingsLableText="* Findings:"></uc:Findings>
            </div>
        </div>

        <div class="dataBlock" runat="server">
            <div class="dataBlock-header">
                Senior Medical Review
            </div>
            <div class="dataBlock-body">
                <uc:Findings ID="ucSeniorMedicalFindings"  FindingsOnly="True" ShowFormText="True" FormFindingsText="" SetReadOnly="True" runat="Server" ShownOnText="(Shown on PSC - D Findings Memo)"
                    RemarksLableText="Decision Explanation:" ConcurWith="Board Medical" ShowRemarks="True" FindingsRequired="true" FindingsLableText="* Findings:" ShowAdditionalRemarksText="true" 
                    AdditionalRemarksLableText="Senior Medical Review and Recommendations: (Shown on PSC - D Commentary WorkSheet)" ></uc:Findings>
            </div>
        </div>

        <div class="dataBlock" runat="server">
            <div class="dataBlock-header">
                Board Legal Review
            </div>
            <div class="dataBlock-body">
                <uc:Findings ID="ucBoardLegalFindings"  FindingsOnly="true" ShowFormText="True" FormFindingsText="" SetReadOnly="True" runat="Server" ShownOnText="(Shown on PSC - D Findings Memo)"
                    RemarksLableText="Legal Review and Recommendations:" ShowRemarks="True" FindingsRequired="true" FindingsLableText="* Findings:"></uc:Findings>
            </div>
        </div>

        <div class="dataBlock" runat="server">
            <div class="dataBlock-header">
                Board Personnal Review
            </div>
            <div class="dataBlock-body">
                <uc:Findings ID="ucPersonnelFindings"   SetDecisionToggle="True" ShowFormText="False" SetReadOnly="True" runat="Server" FindingsOnly="true"
                    RemarksLableText="Personnel Review and Recommendations:" FindingsRequired="true" FindingsLableText="* Findings:"></uc:Findings>
                <asp:UpdatePanel runat="server" ID="upnlPersonnelFindings_v2">
                    <ContentTemplate>
                        <table id="bdPersonnel_v2" runat="server">
                            <tr>
                                <td class="number">
                                    <asp:Label runat="server" ID="Label1_v2">&nbsp;</asp:Label>
                                </td>
                                <td class="label">
                                    <asp:Label runat="server" ID="bdPersonnellbl_v2" Text="Refer member to DES for processing: " />
                                </td>
                                <td class="value">
                                    <asp:CheckBox ID="BPR_DESCheckBox" runat="server"/>
                                    <asp:Label runat="server" ID="bdPersonnelChecklbl_v2"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            
        </div>
                            
                                
        <div class="dataBlock" runat="server">
            <div class="dataBlock-header">
                Approving Authority Review
            </div>
            <div class="dataBlock-body">
                <uc:Findings ID="ucApprovingAuthorityFindings"  FindingsOnly="True" ShowFormText="True" FormFindingsText="" SetReadOnly="True" runat="Server" ShownOnText="(Shown on PSC - D Findings Memo)"
                    RemarksLableText="Approving Authority Comments:" FindingsLableText="* Final Approval" ShowRemarks="True" FindingsRequired="true"></uc:Findings>
                <table id="bdfrmlPersonnel_v2" runat="server">
                            <tr>
                                <td class="number">
                                    <asp:Label runat="server" ID="FRMLPersonnel_v2">&nbsp;</asp:Label>
                                </td>
                                <td class="label">
                                    <asp:Label runat="server" ID="Label1" Text="Refer member to DES for processing: " />
                                </td>
                                <td class="value">
                                    <asp:CheckBox ID="AAR_DESCheckBox" runat="server"/>
                            
                                    <asp:Label runat="server" ID="bdFrmlPersonnelChecklbl_v2"></asp:Label>
                                </td>
                            </tr>
                        </table>
                              
            </div>
        </div>



    </asp:Panel>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="FooterNested" runat="Server">
</asp:Content>