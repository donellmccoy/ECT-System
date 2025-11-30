<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SARC/SARCMaster.master" AutoEventWireup="false" Inherits="ALOD.Web.SARC.Admin" CodeBehind="Admin.aspx.vb" %>

<%@ Import Namespace="ALODWebUtility.Common" %>

<%@ MasterType VirtualPath="~/Secure/SARC/SARCMaster.master" %>

<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>

<%@ Register Src="~/Secure/Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck" TagPrefix="uc1" %>
<%@ Register Src="~/Secure/Shared/UserControls/FindingsControl.ascx" TagName="Findings" TagPrefix="uc" %>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    <asp:Panel runat="server" ID="pnlSARCAdminFindings" CssClass="dataBlock">
        <div class="dataBlock-header">
            SARC Administrator Review:
        </div>
        <div class="dataBlock-body">
            <uc:Findings runat="Server" ID="ucSARCAdminFindings" SetDecisionToggle="False" FindingsOnly="True" ShowFormText="False" SetReadOnly="True" RemarksMaxLength="250" RemarksLableText="Remarks:<br />(Shown on Form 348-R)" />
            <uc1:SignatureCheck runat="server" ID="ucSigCheckSARCAdmin" Template="Form348SARCFindings" CssClass="sigcheck-form no-border" />
        </div>
    </asp:Panel>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">
  
</asp:Content>