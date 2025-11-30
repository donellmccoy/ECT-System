<%@ Page Title="" Language="C#" AutoEventWireup="false" MasterPageFile="~/Secure/SARC_Appeal/SARC_Appeal.Master" CodeBehind="CaseDialogue.aspx.cs" Inherits="ALOD.Web.APSA.Secure_sarcp_CaseDialogue" %>


<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SARC_Appeal/SARC_Appeal.Master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck" TagPrefix="uc1" %>
<%@ Register TagPrefix="uc1" Namespace="ALOD.Secure.Shared.UserControls" Assembly="ALOD" %>
<%@ Register TagPrefix="uc1" TagName="CaseDialogue" Src="~/Secure/Shared/UserControls/CaseDialogue.ascx" %>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" Runat="Server">
    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
        <ContentTemplate>
            <uc1:CaseDialogue runat="server" ID="CaseDialogue" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>