<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/lod/Lod.master" AutoEventWireup="false" Inherits="ALOD.Web.LOD.SeniorMedReviewer" Codebehind="SeniorMedReviewer.aspx.vb"%>

<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/lod/Lod.master" %>
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
                    <asp:label id="FindingsTitle" runat="server" Text="Senior Medical Reviewer LOD Decision" />
                </div>
                <div class="dataBlock-body indent-small">
                    <uc:Findings runat="Server" ID="ucSeniorMedicalFindings" SetDecisionToggle="False" ShowFormText="True" SetReadOnly="True" ReasonsLabelText="Decision Explanation:"
                            ShowRemarks="false" ShownOnText="" Adjustlimit="250" ConcurWith="Board Medical" />
                    <uc1:SignatureCheck runat="server" ID="ucSigCheckSeniorMedical" Template="Form348APFindings" CssClass="sigcheck-form no-border" />
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">
    <script type="text/javascript">

        $(document).ready(function () {
            CheckDecision("ucSeniorMedicalFindings_rblDecison_1", "ucSeniorMedicalFindings_rblFindings");
        });

    </script>
</asp:Content>