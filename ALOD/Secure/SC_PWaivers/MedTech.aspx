<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SC_PWaivers/SC_Pwaiver.master"
    MaintainScrollPositionOnPostback="true" AutoEventWireup="false"
     EnableEventValidation="false" Inherits="ALOD.Web.Special_Case.PW.Secure_sc_pw_MedTech" Codebehind="MedTech.aspx.vb" %>

<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Register Src="~/secure/Shared/UserControls/FindingsControl.ascx" TagName="Findings"
    TagPrefix="uc" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_PWaivers/SC_Pwaiver.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck"
    TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/ICDCodeControl.ascx" TagName="ICDCodeControl" TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/ICD7thCharacterControl.ascx" TagName="ICD7thCharacterControl" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    <div class="dataBlock">
        <div class="dataBlock-header">
            Select a Category
        </div>
        <br />
            &nbsp;&nbsp;&nbsp;
            <asp:DropDownList ID="PWCategoryDropdownListSelect" DataSourceID="PWCategoriesObjectDataSource"
                DataTextField="Name" DataValueField="Id" runat="server" Width="240px" AppendDataBoundItems="True">
                <asp:ListItem Value="0">--Select a Category--</asp:ListItem>
            </asp:DropDownList>
            <br />
            <br />
    </div> 
    <div class="dataBlock">
        <div class="dataBlock-header">
            0 - Point of Contact Info
        </div>
        <table>
            <tr>
                <td>POC Name:</td>
                <td><asp:TextBox ID="txtPOCNameLabel" MaxLength="180" runat="server" Width="400"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>POC DSN/Phone:</td>
                <td><asp:TextBox ID="txtPOCPhoneLabel" MaxLength="180" runat="server" Width="400"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>POC DSN/Phone:</td>
                <td><asp:TextBox ID="txtPOCEmailLabel" MaxLength="180" runat="server" Width="400"></asp:TextBox>
                </td>
            </tr>
        </table>
    </div>
    <div class="dataBlock">
        <div class="dataBlock-header">
            Select an ICD code
        </div>
        <table>
            <tr>
                <td class="number">
                    A
                </td>
                <td class="label  labelRequired" >
                    *Diagnosis:
                </td>
                <td class="value">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <uc1:ICDCodeControl runat="server" ID="ucICDCodeControl" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td class="number">
                    B
                </td>
                <td class="label">
                    7th Character:
                </td>
                <td class="value">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <uc1:ICD7thCharacterControl runat="server" ID="ucICD7thCharacterControl" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td class="number">
                    C
                </td>
                <td class="label labelRequired">
                    *Diagnosis Text:
                </td>
                <td class="value">
                    <asp:Label ID="DiagnosisLabel" runat="server" CssClass="lblDisableText"></asp:Label>
                    <asp:TextBox ID="DiagnosisTextBox" Width="500px" TextMode="MultiLine" runat="server"
                        Rows="4" MaxLength="250"></asp:TextBox>
                </td>
            </tr>
        </table>
    </div>
    <asp:ObjectDataSource ID="PWCategoriesObjectDataSource" runat="server" SelectMethod="GetPWCategories"
        TypeName="ALOD.Data.Services.LookupService"></asp:ObjectDataSource>
</asp:Content>