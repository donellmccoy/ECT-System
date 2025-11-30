<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SC_MEB/SC_MEB.master" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.MEB.Secure_sc__meb_lod_Documents" MaintainScrollPositionOnPostback="true" Codebehind="LODDocuments.aspx.vb" %>

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

