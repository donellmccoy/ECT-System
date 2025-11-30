<%@ Page Title="" Language="C#" MasterPageFile="~/Secure/SC_Congress/SC_Congress.master" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.CI.Secure_sc_ci_Documents" MaintainScrollPositionOnPostback="true" CodeBehind="Documents.aspx.cs" %>

<%@ MasterType VirtualPath="~/Secure/SC_Congress/SC_Congress.master" %>
<%@ Import Namespace="ALODWebUtility.Common" %>
<%@ Register Src="../Shared/UserControls/Documents.ascx" TagName="Documents" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:Documents runat="server" ID="Documents" />
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
