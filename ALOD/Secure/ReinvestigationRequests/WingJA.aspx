<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/ReinvestigationRequests/ReinvestigationRequest.master" AutoEventWireup="false" Inherits="ALOD.Web.RR.Secure_rr_WingJA" Codebehind="WingJA.aspx.vb" %>

<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Register Src="~/secure/Shared/UserControls/FindingsControl.ascx" TagName="Findings" TagPrefix="uc" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/ReinvestigationRequests/ReinvestigationRequest.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderNested" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    <div class="dataBlock">
        <div class="dataBlock-header">
            Wing JA Reinvestigation Request Decision
        </div>
        <div class="dataBlock-body">
            <uc:Findings runat="Server" ID="ucWingJAFindings" SetDecisionToggle="False" FindingsOnly="True" ShowFormText="True" SetReadOnly="True" ReasonsLabelText="Recommendation Explanation:"
		            ShowRemarks="false" ShownOnText="" Adjustlimit="1000" />
            <uc1:SignatureCheck runat="server" ID="ucSigCheckWingJA" Template="Form348RRFindings" CssClass="sigcheck-form no-border" />
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">
</asp:Content>
