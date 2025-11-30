<%@ Page Title="" Language="C#" MasterPageFile="~/Secure/SC_DW/SC_DW.master" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.DW.Secure_SC_DW_Tracking" CodeBehind="Tracking.aspx.cs" %>

<%@ Import Namespace="ALODWebUtility.Common" %>
<%@ Register Src="../Shared/UserControls/CaseTracking.ascx" TagName="CaseTracking" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:CaseTracking runat="server" ID="CaseTracking" />
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>


