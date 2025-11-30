<%@ Page Title="" Language="C#" MasterPageFile="~/Secure/ReinvestigationRequests/ReinvestigationRequest.master" AutoEventWireup="false" Inherits="ALOD.Web.RR.Secure_rr_Tracking" CodeBehind="Tracking.aspx.cs" %>

<%@ Import Namespace="ALODWebUtility.Common" %>
<%@ Register Src="../Shared/UserControls/CaseTracking.ascx" TagName="CaseTracking" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:CaseTracking runat="server" ID="CaseTracking" />
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
