<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/ReinvestigationRequests/ReinvestigationRequest.master" AutoEventWireup="false" Inherits="ALOD.Web.RR.Secure_rr_lodBoard" Codebehind="lodBoard.aspx.vb" %>

<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/ReinvestigationRequests/ReinvestigationRequest.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Reference Control="~/Secure/Shared/UserControls/ModuleHeader.ascx" %>
<%@ Register Src="~/secure/Shared/UserControls/FindingsControl.ascx" TagName="Findings" TagPrefix="uc" %>
<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    <div class="dataBlock" id="WingJA" runat="server">
        <div class="dataBlock-header">
            Wing JA Reinvestigation Request Decision
        </div>
        <div class="dataBlock-body indent-small">
            <uc:Findings runat="Server" ID="ucWingJAFindings" SetDecisionToggle="False" FindingsOnly="True" ShowFormText="True" SetReadOnly="True" ReasonsLabelText="Recommendation Explanation:"
		            ShowRemarks="false" ShownOnText="" Adjustlimit="1000" />
            <uc1:SignatureCheck runat="server" ID="ucSigCheckWingJA" Template="Form348RRFindings" CssClass="sigcheck-form no-border" />
        </div>
    </div>
    <div class="dataBlock" id="WingCC" runat="server">
        <div class="dataBlock-header">
            Wing CC Reinvestigation Request Decision
        </div>
        <div class="dataBlock-body indent-small">
            <uc:Findings runat="Server" ID="ucWingCCFindings" SetDecisionToggle="False" FindingsOnly="True" ShowFormText="True" SetReadOnly="True" ReasonsLabelText="Recommendation Explanation:"
		            ShowRemarks="false" ShownOnText="" Adjustlimit="1000" />
            <uc1:SignatureCheck runat="server" ID="ucSigCheckWingCC" Template="Form348RRFindings" CssClass="sigcheck-form no-border" />
        </div>
    </div>
    <div class="dataBlock" id="BoardA1" runat="server">
        <div class="dataBlock-header">
            Board Admin Reinvestigation Request Decision
        </div>
        <div class="dataBlock-body indent-small">
            <uc:Findings runat="Server" ID="ucBoardAdminFindings" SetDecisionToggle="False" FindingsOnly="True" ShowFormText="True" SetReadOnly="True" ReasonsLabelText="Recommendation Explanation:"
                    ShowRemarks="false" ShownOnText="" Adjustlimit="1000" />
            <uc1:SignatureCheck runat="server" ID="ucSigCheckBoardAdmin" Template="Form348RRFindings" CssClass="sigcheck-form no-border" />
        </div>
    </div>
    <div class="dataBlock" id="BoardMedical" runat="server">
        <div class="dataBlock-header">
            Board Medical Reinvestigation Request Decision
        </div>
        <div class="dataBlock-body indent-small">
            <uc:Findings runat="Server" ID="ucBoardMedicalFindings" SetDecisionToggle="False" FindingsOnly="True" ShowFormText="True" SetReadOnly="True" ReasonsLabelText="Recommendation Explanation:"
                    ShowRemarks="false" ShownOnText="" Adjustlimit="1000" />
            <uc1:SignatureCheck runat="server" ID="ucSigCheckBoardMedical" Template="Form348RRFindings" CssClass="sigcheck-form no-border" />
        </div>
    </div>
    <div class="dataBlock" id="BoardLegal" runat="server">
        <div class="dataBlock-header">
            Board Legal Reinvestigation Request Decision
        </div>
        <div class="dataBlock-body indent-small">
            <uc:Findings runat="Server" ID="ucBoardJAFindings" SetDecisionToggle="False" FindingsOnly="True" ShowFormText="True" SetReadOnly="True" ReasonsLabelText="Recommendation Explanation:" 
                    ShowRemarks="false" ShownOnText="" Adjustlimit="1000" />
            <uc1:SignatureCheck runat="server" ID="ucSigCheckBoardJA" Template="Form348RRFindings" CssClass="sigcheck-form no-border" />
        </div>
    </div>
    <div class="dataBlock" id="ApprovingAuthority" runat="server">
        <div class="dataBlock-header">
            Approving Authority Reinvestigation Request Decision
        </div>
        <div class="dataBlock-body indent-small">
            <uc:Findings runat="Server" ID="ucApprovingAuthorityFindings" SetDecisionToggle="False" FindingsOnly="True" ShowFormText="True" SetReadOnly="True" ReasonsLabelText="Decision Explanation:"
                    ShowRemarks="false" ShownOnText="" Adjustlimit="1000" />
            <uc1:SignatureCheck runat="server" ID="ucSigCheckApprovingAuthority" Template="Form348RRFindings" CssClass="sigcheck-form no-border" />
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">
  
</asp:Content>
