<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/AppealRequests/AppealRequest.master" AutoEventWireup="false" Inherits="ALOD.Web.AP.Secure_ap_lodBoard" Codebehind="lodBoard.aspx.vb" %>

<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/AppealRequests/AppealRequest.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Reference Control="~/Secure/Shared/UserControls/ModuleHeader.ascx" %>
<%@ Register Src="~/secure/Shared/UserControls/FindingsControl.ascx" TagName="Findings" TagPrefix="uc" %>
<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    <div class="dataBlock" id="BoardMedical" runat="server">
        <div class="dataBlock-header">
            Board Medical Appeal Decision
        </div>
        <div class="dataBlock-body indent-small">
            <uc:Findings runat="Server" ID="ucBoardMedicalFindings" SetDecisionToggle="False" FindingsOnly="True" ShowFormText="True" SetReadOnly="True" ReasonsLabelText="Decision Explanation:"
                    ShowRemarks="false" ShownOnText="" Adjustlimit="2400" />
            <uc1:SignatureCheck runat="server" ID="ucSigCheckBoardMedical" Template="Form348APFindings" CssClass="sigcheck-form no-border" />
        </div>
    </div>
    <div class="dataBlock" id="BoardLegal" runat="server">
        <div class="dataBlock-header">
            Board Legal Appeal Decision
        </div>
        <div class="dataBlock-body indent-small">
            <uc:Findings runat="Server" ID="ucBoardJAFindings" SetDecisionToggle="False" FindingsOnly="True" ShowFormText="True" SetReadOnly="True" ReasonsLabelText="Decision Explanation:" 
                    ShowRemarks="false" ShownOnText="" Adjustlimit="2400" />
            <uc1:SignatureCheck runat="server" ID="ucSigCheckBoardJA" Template="Form348APFindings" CssClass="sigcheck-form no-border" />
        </div>
    </div>
    <div class="dataBlock" id="BoardAdmin" runat="server">
        <div class="dataBlock-header">
            Board Administrator Appeal Decision
        </div>
        <div class="dataBlock-body indent-small">
            <uc:Findings runat="Server" ID="ucBoardAdminFindings" SetDecisionToggle="False" FindingsOnly="True" ShowFormText="True" SetReadOnly="True" ReasonsLabelText="Decision Explanation:"
                    ShowRemarks="false" ShownOnText="" Adjustlimit="2400" />
            <uc1:SignatureCheck runat="server" ID="ucSigCheckBoardAdmin" Template="Form348APFindings" CssClass="sigcheck-form no-border" />
        </div>
    </div>
    <div class="dataBlock" id="ApprovingAuthority" runat="server">
        <div class="dataBlock-header">
            Approving Authority Appeal Decision
        </div>
        <div class="dataBlock-body indent-small">
            <uc:Findings runat="Server" ID="ucApprovingAuthorityFindings" SetDecisionToggle="False" FindingsOnly="True" ShowFormText="True" SetReadOnly="True" ReasonsLabelText="Decision Explanation:"
                    ShowRemarks="false" ShownOnText="" Adjustlimit="2400" />
            <uc1:SignatureCheck runat="server" ID="ucSigCheckApprovingAuthority" Template="Form348APFindings" CssClass="sigcheck-form no-border" />
        </div>
    </div>
    <div class="dataBlock" id="AppellateAuth" runat="server">
        <div class="dataBlock-header">
            Appellate Authority Appeal Decision
        </div>
        <div class="dataBlock-body indent-small">
            <uc:Findings runat="Server" ID="ucAppellateAuthorityFindings" SetDecisionToggle="False" FindingsOnly="True" ShowFormText="True" SetReadOnly="True" ReasonsLabelText="Decision Explanation:"
                    ShowRemarks="false" ShownOnText="" Adjustlimit="2400" />
            <uc1:SignatureCheck runat="server" ID="ucSigCheckAppellateAuthority" Template="Form348APFindings" CssClass="sigcheck-form no-border" />
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">
  
</asp:Content>