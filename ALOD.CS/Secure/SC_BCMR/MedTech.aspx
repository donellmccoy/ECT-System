<%@ Page Title="" Language="C#" MasterPageFile="~/Secure/SC_BCMR/SC_BCMR.master" MaintainScrollPositionOnPostback="true"
    AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.BCMR.Secure_sc_bc_MedTech" CodeBehind="MedTech.aspx.cs" %>

<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Register Src="~/secure/Shared/UserControls/FindingsControl.ascx" TagName="Findings"
    TagPrefix="uc" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_BCMR/SC_BCMR.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck"
    TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    <div class="dataBlock">
        <div class="dataBlock-header">
            Wing CC Reinvestigation Request Decision
        </div><br /><br />
        <div class="dataBlock">
            <table>
                <tr>
                    <td>
                        <asp:RadioButton runat="server" ID="DecisionY" GroupName="Decision" 
                            Text="Approve Reinvestigation Request" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:RadioButton runat="server" ID="DecisionN" GroupName="Decision" 
                            Text="Recommend Denial of Reinvestigation Request" />
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td>
                        Decision Explanation
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:TextBox Height="50" Width="500px" runat="server" ID="DecisionComment" 
                            TextMode="MultiLine" MaxLength="250"></asp:TextBox>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">
</asp:Content>
