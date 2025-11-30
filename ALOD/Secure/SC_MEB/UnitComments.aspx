<%@ Page Title="Case Comments" Language="VB" MasterPageFile="~/Secure/SC_MEB/SC_MEB.master" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.MEB.UnitComments" Codebehind="UnitComments.aspx.vb" %>

<%@ Import Namespace="ALODWebUtility.Common" %>

<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_MEB/SC_MEB.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck"
    TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/CaseComments.ascx" TagName="CaseComments" TagPrefix="uc1" %>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" Runat="Server">
    
    <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="true">
        <ContentTemplate>
            <uc1:CaseComments runat="server" ID="CaseComment" />
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>

