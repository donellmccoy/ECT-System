<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SC_MEB/SC_MEB.master" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.MEB.Secure_sc_meb_Tracking" Codebehind="Tracking.aspx.vb" %>

<%@ Import Namespace="ALODWebUtility.Common" %>
<%@ Register Src="../Shared/UserControls/CaseTracking.ascx" TagName="CaseTracking" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:CaseTracking runat="server" ID="CaseTracking" />
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
