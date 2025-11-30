<%@ Page Title="" Language="C#" MasterPageFile="~/Secure/SC_Incap/SC_Incap.master" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.IN.Secure_sc__in_lod_Documents" MaintainScrollPositionOnPostback="true" CodeBehind="LODDocuments.aspx.cs" %>

<%@ Import Namespace="ALODWebUtility.Common" %>

<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ Register Src="../Shared/UserControls/LODDocuments.ascx" TagName="LODDocuments" TagPrefix="uc1" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:LODDocuments runat="server" ID="ucLODDocuments" />
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
