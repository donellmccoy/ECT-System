<%@ Page Title="" Language="C#" MasterPageFile="~/Secure/SARC_Appeal/SARC_Appeal.master" AutoEventWireup="false" Inherits="ALOD.Web.APSA.Admin" CodeBehind="Admin.aspx.cs" %>

<%@ Import Namespace="ALODWebUtility.Common" %>

<%@ MasterType VirtualPath="~/Secure/SARC_Appeal/SARC_Appeal.master" %>

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
            <uc:Findings runat="Server" ID="ucSARCAdminFindings" SetDecisionToggle="False" FindingsOnly="True" ShowFormText="True" SetReadOnly="True" ReasonsLabelText="Remarks:" 
                ShownOnText="" Adjustlimit="250" ShowRemarks="false"/>
            <uc1:SignatureCheck runat="server" ID="ucSigCheckSARCAdmin" Template="Form348APSARCFindings" CssClass="sigcheck-form no-border" />
        </div>
    </asp:Panel>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">
  
</asp:Content>
