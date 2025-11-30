<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Secure/SC_WWD/SC_WWD.master" CodeBehind="CaseDialogue.aspx.vb" Inherits="ALOD.Web.Special_Case.WWD.Secure_sc_CaseDialogue" %>

<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_WWD/SC_WWD.master" %>
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