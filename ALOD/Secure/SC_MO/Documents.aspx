<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SC_MO/SC_MO.master" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.MO.Secure_sc_mo_Documents" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" Codebehind="Documents.aspx.vb" %>

<%@ MasterType VirtualPath="~/Secure/SC_MO/SC_MO.master" %>
<%@ Import Namespace="ALODWebUtility.Common" %>
<%@ Register Src="../Shared/UserControls/Documents.ascx" TagName="Documents" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:Documents runat="server" ID="Documents" />
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
