<%@ Page Title="" Language="C#" MasterPageFile="~/Secure/SC_FastTrack/SC_FastTrack.master" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.IRILO.Secure_sc_FastTrack_HQTech" CodeBehind="HQTech.aspx.cs" %>

<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Register Src="~/secure/Shared/UserControls/FindingsControl.ascx" TagName="Findings"
    TagPrefix="uc" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_FastTrack/SC_FastTrack.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck"
    TagPrefix="uc1" %>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    <div class="dataBlock">
        <div class="dataBlock-header">
            <asp:Label runat="server" ID="lblHeader" Text="HQ AFRC Technician Decision" />
        </div>
        <div class="dataBlock"><br /><br />
        <table>
            <tr>
                <td>
                    <br />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:RadioButton runat="server" ID="DecisionY" GroupName="Decision" 
                        Text="Recommend Approval of WWD Fast Track" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:RadioButton runat="server" ID="DecisionN" GroupName="Decision" 
                        Text="Recommend Denial of WWD Fast Track" />
                </td>
            </tr>
            <tr>
                <td>
                    <br />
                </td>
            </tr>
            <tr>
                <td>
                    Recommendation Explanation
                </td>
            </tr>
            <tr>
                <td>
                    <asp:TextBox Height="50" Width="500px" runat="server" ID="DecisionComment" 
                        TextMode="MultiLine" MaxLength="250"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <br />
                </td>
            </tr>
        </table>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">
</asp:Content>
