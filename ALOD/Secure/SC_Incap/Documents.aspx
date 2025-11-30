<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SC_Incap/SC_Incap.master" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.IN.Secure_sc_in_Documents" MaintainScrollPositionOnPostback="true" Codebehind="Documents.aspx.vb" %>

<%@ MasterType VirtualPath="~/Secure/SC_Incap/SC_Incap.master" %>
<%@ Import Namespace="ALODWebUtility.Common" %>
<%@ Register Src="../Shared/UserControls/Documents.ascx" TagName="Documents" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:Documents runat="server" ID="Documents" />
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>