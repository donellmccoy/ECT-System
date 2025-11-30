<%@ Page Language="C#" MasterPageFile="~/Secure/ReinvestigationRequests/ReinvestigationRequest.master" AutoEventWireup="false" Async="true" Inherits="ALOD.Web.RR.Secure_rr_SMData" Title="Untitled Page" CodeBehind="SMData.aspx.cs" %>

<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/ReinvestigationRequests/ReinvestigationRequest.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="../Shared/UserControls/CaseHistory.ascx" TagName="CaseHistory" TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck"
    TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/SignatureBlock.ascx" TagName="SignatureBlock"
    TagPrefix="uc1" %>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="HeaderNested">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentNested" runat="Server">


    
    <asp:Panel runat="server" ID="MemberDataPanel" CssClass="dataBlock">
        <div class="dataBlock-header">
            Member Information
        </div>
        <div class="dataBlock-body">
            <table class="dataTable">
                <tr>
                    <td class="number">
                        A
                    </td>
                    <td class="label">
                        Name:
                    </td>
                    <td class="value">
                        <asp:Label ID="lblName" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        B
                    </td>
                    <td class="label">
                        Rank:
                    </td>
                    <td class="value">
                        <asp:Label ID="lblRank" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        C
                    </td>
                    <td class="label">
                        DOB:
                    </td>
                    <td class="value">
                        <asp:Label ID="lbldob" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        D
                    </td>
                    <td class="label">
                        Unit:
                    </td>
                    <td class="value">
                        <asp:Label runat="server" ID="lblUnit" />
                        <uc1:SignatureBlock ID="SigBlock" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        E
                    </td>
                    <td class="label">
                        Component:
                    </td>
                    <td class="value">
                        <asp:Label ID="lblCompo" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
       </div>
    </asp:Panel>
    <br />
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:CaseHistory runat="server" ID="CaseHistory" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <uc1:SignatureCheck ID="SigCheck" runat="server" Template="Form348RR" CssClass="sigcheck-form" />
</asp:Content>
