<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SC_MH/SC_MH.master" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.MH.Secure_sc_mh_HQTech" EnableEventValidation = "false" Codebehind="HQTech.aspx.vb" %>

<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Register Src="~/secure/Shared/UserControls/FindingsControl.ascx" TagName="Findings"
    TagPrefix="uc" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_MH/SC_MH.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck"
    TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/ICDCodeControl.ascx" TagName="ICDCodeControl" TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/ICD7thCharacterControl.ascx" TagName="ICD7thCharacterControl" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    <div class="dataBlock">
        <div class="dataBlock-header"><asp:Label runat="server" ID="lblHeader" Text="HQ AFRC Technician - MH" /></div>
            <br />
              
        <table style="border-spacing: 5px">
            <tr>
                <td>
                    <asp:Label ID="Label3" runat="server" CssClass="label labelRequired" Text ="*HYDT/MSD:"></asp:Label>
                    </td>
                <td><asp:Label ID="Label10" runat="server" CssClass="lblDisableText"></asp:Label>
                    <asp:TextBox ID="txtHTD" MaxLength="10" onchange="DateCheck(this);" 
                        runat="server" Width="80"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label1" CssClass="label labelRequired" runat="server">*Diagnosis:</asp:Label>
                </td>
                <td class="value">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <uc1:ICDCodeControl runat="server" ID="ucICDCodeControl" />
                            <asp:Label runat="server" ID="footnote" Text="<br/>* - Changing this selection requires moving to a new tab and returning to synchronize data" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label2" CssClass="label" runat="server">7th Character:</asp:Label>
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
                <td>
                    <asp:Label ID="Label55" CssClass="label labelRequired" runat="server">Diagnosis Text:</asp:Label>
                </td>
                <td class="value">
                    <asp:Label ID="DiagnosisLabel" runat="server" CssClass="lblDisableText"></asp:Label>
                    <asp:TextBox ID="DiagnosisTextBox" Width="500px" TextMode="MultiLine" runat="server"
                        Rows="4" MaxLength="250"></asp:TextBox>
                </td>
            </tr>
        </table>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">
    <script type="text/javascript">

        $(function () {

            $('.datePicker').datepicker(calendarPick("Past", "<%=CalendarImage %>"));

            $('.datePickerAll').datepicker(calendarPick("All", "<%=CalendarImage %>"));

        });        
    
    </script>
</asp:Content>
