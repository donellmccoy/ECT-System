<%@ Page Title="Case Comments" Language="C#" MasterPageFile="~/Secure/SC_AGRCert/SC_AGRCert.master" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.AGR.UnitComments" CodeBehind="UnitComments.aspx.cs" %>

<%@ Import Namespace="ALODWebUtility.Common" %>

<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_AGRCert/SC_AGRCert.master" %>
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

