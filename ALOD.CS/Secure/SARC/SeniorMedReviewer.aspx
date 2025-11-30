<%@ Page Title="" Language="C#" MasterPageFile="~/Secure/SARC/SARCMaster.master" AutoEventWireup="false" Inherits="ALOD.Web.SARC.SeniorMedReviewer" CodeBehind="SeniorMedReviewer.aspx.cs" MaintainScrollPositionOnPostback="true"%>

<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SARC/SARCMaster.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Reference Control="~/Secure/Shared/UserControls/ModuleHeader.ascx" %>
<%@ Register Src="~/secure/Shared/UserControls/FindingsControl.ascx" TagName="Findings" TagPrefix="uc" %>
<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="true">
        <ContentTemplate>
            <div class="dataBlock" id="SeniorMedical" runat="server">
                <div class="dataBlock-header">
                    Senior Medical Reviewer SARC Decision
                </div>
                <div class="dataBlock-body indent-small">
                    <uc:Findings runat="Server" ID="ucSeniorMedicalFindings" SetDecisionToggle="False" ShowFormText="False" SetReadOnly="True" RemarksLableText="Senior Medical Reviewer Remarks:<br />(Not Shown on Form 348-R)"
                            Adjustlimit="250" ConcurWith="Board Medical" />
                    <uc1:SignatureCheck runat="server" ID="ucSigCheckSeniorMedical" Template="Form348APFindings" CssClass="sigcheck-form no-border" />
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">
    <script type="text/javascript">

$(function () {
            CheckDecision("ucSeniorMedicalFindings_rblDecison_1", "ucSeniorMedicalFindings_rblFindings");
        });

    </script>
</asp:Content>