<%@ Page Title="" Language="C#" MasterPageFile="~/Secure/SC_BMT/SC_BMT.master" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.BMT.Secure_sc_bmt_HQTech" CodeBehind="HQTech.aspx.cs" %>

<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Register Src="~/secure/Shared/UserControls/FindingsControl.ascx" TagName="Findings"
    TagPrefix="uc" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_BMT/SC_BMT.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck"
    TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    <div class="dataBlock">
        <div class="dataBlock-header">
            HQ 
            AFRC Technician - BMT
        </div>   
        <table>
            <tr>
                <td style="width:20%;">
                    Comments:
                </td>
                <td style="width:80%;">
                    <asp:TextBox runat="server" ID="CommentsTB" Text=""
                        TextMode="MultiLine" Height="50" Width="500px" MaxLength="250" />
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <br />
                </td>
            </tr>
        </table>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">
</asp:Content>
