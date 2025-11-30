<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SARC/SARCMaster.master" AutoEventWireup="false" Inherits="ALOD.Web.SARC.Tracking" CodeBehind="Tracking.aspx.vb" %>

<%@ Import Namespace="ALODWebUtility.Common" %>
<%@ Register Src="../Shared/UserControls/CaseTracking.ascx" TagName="CaseTracking" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:CaseTracking runat="server" ID="CaseTracking" />
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
