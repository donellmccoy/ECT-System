<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Lod/Lod.master" AutoEventWireup="false" Inherits="ALOD.Web.LOD.Secure_lod_WingJA" Codebehind="WingJA.aspx.vb" %>

<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Register Src="~/secure/Shared/UserControls/FindingsControl.ascx" TagName="Findings"
    TagPrefix="uc" %>
<%--<%@ Register Src="~/secure/Shared/UserControls/v1_FindingsControl.ascx" TagName="v1_Findings"
    TagPrefix="v1_uc" %>--%>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/Lod/LOD.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck"
    TagPrefix="uc1" %>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    <asp:Panel runat="server" ID="OriginalLOD" Visible="false">
    <div class="dataBlock">
        <div class="dataBlock-header">
            1 - Informal Wing JA Review
        </div>
        <div class="dataBlock-body">
            <uc:Findings ID="InformalFindings" ShowPrevFindings="True" ConcurWith="Unit Commander"
                SetDecisionToggle="True" SetReadOnly="False" runat="Server"></uc:Findings>
            <uc1:SignatureCheck ID="SigCheckInformal" runat="server" Template="Form348Findings" CssClass="sigcheck-form" />
        </div>
    </div>
    <asp:Panel runat="server" ID="FormalPanel" CssClass="dataBlock">
        <div class="dataBlock-header">
            2 - Formal Action by Wing JA
        </div>
        <div class="dataBlock-body">
            <uc:Findings ID="FormalFindings" ShowPrevFindings="True" ConcurWith="Investigating Officer"
                SetDecisionToggle="True" SetReadOnly="False" runat="Server"></uc:Findings>
            <uc1:SignatureCheck ID="SigCheckFormal" runat="server" Template="Form348Findings" CssClass="sigcheck-form" />
        </div>
        </asp:Panel>
    </asp:Panel>

    <asp:Panel runat="server" ID="LOD_v2" Visible="false">
        <div class="dataBlock">
            <div class="dataBlock-header">
                1 - Informal Wing JA Review
            </div>
            <div class="dataBlock-body">
                <uc:Findings ID="InformalFindings_v2" ShowPrevFindings="True" ConcurWith="Unit Commander"
                    SetDecisionToggle="True" SetReadOnly="False" runat="Server"></uc:Findings>
                <uc1:SignatureCheck ID="SigCheckInformal_v2" runat="server" Template="Form348Findings" CssClass="sigcheck-form" />
            </div>
        </div>
        <asp:Panel runat="server" ID="FormalPanel_v2" CssClass="dataBlock">
            <div class="dataBlock-header">
                2 - Formal Action by Wing JA
            </div>
            <div class="dataBlock-body">
                <uc:Findings ID="FormalFindings_v2" ShowPrevFindings="True" ConcurWith="Investigating Officer"
                    SetDecisionToggle="True" SetReadOnly="False" runat="Server"></uc:Findings>
                <uc1:SignatureCheck ID="SigCheckFormal_v2" runat="server" Template="Form348Findings" CssClass="sigcheck-form" />
            </div>
        </asp:Panel>
    </asp:Panel>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">
</asp:Content>
