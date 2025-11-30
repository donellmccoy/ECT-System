<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SC_BCMR/SC_BCMR.master" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.BCMR.Secure_sc_bc_Documents" MaintainScrollPositionOnPostback="true" Codebehind="Documents.aspx.vb" %>
    
<%@ MasterType VirtualPath="~/Secure/SC_BCMR/SC_BCMR.master" %>
<%@ Import Namespace="ALODWebUtility.Common" %>
<%@ Register Src="../Shared/UserControls/Documents.ascx" TagName="Documents" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:Documents runat="server" ID="Documents" />
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
