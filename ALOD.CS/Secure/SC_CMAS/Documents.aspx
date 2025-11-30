<%@ Page Title="" Language="C#" MasterPageFile="~/Secure/SC_CMAS/SC_CMAS.master" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.CMAS.Secure_sc_cm_Documents" MaintainScrollPositionOnPostback="true" CodeBehind="Documents.aspx.cs" %>

<%@ MasterType VirtualPath="~/Secure/SC_CMAS/SC_CMAS.master" %>
<%@ Import Namespace="ALODWebUtility.Common" %>
<%@ Register Src="../Shared/UserControls/Documents.ascx" TagName="Documents" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:Documents runat="server" ID="Documents" />
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>