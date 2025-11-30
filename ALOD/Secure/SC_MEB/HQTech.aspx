<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SC_MEB/SC_MEB.master" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.MEB.Secure_sc_meb_HQTech" EnableEventValidation = "false" Codebehind="HQTech.aspx.vb" %>

<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Register Src="~/secure/Shared/UserControls/FindingsControl.ascx" TagName="Findings"
    TagPrefix="uc" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_MEB/SC_MEB.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck"
    TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/ICDCodeControl.ascx" TagName="ICDCodeControl" TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/ICD7thCharacterControl.ascx" TagName="ICD7thCharacterControl" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    <div class="dataBlock">
        <div class="dataBlock-header">
            HQ Tech - MEB
        </div>
            <br />
        <table style="border-spacing: 5px">
            <tr>
                <td>
                    <asp:Label runat="server" CssClass="label labelRequired" ID="Label3"
                        LabelFor="ddlMedGroups">Med Group Name: </asp:Label>
                </td>
                <td>
                    <asp:DropDownList runat="server" ID="ddlMedGroups"></asp:DropDownList>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" CssClass="label labelRequired" ID="Label2"
                        LabelFor="ddlRMUs">RMU Name: </asp:Label>
                </td>
                <td>
                    <asp:DropDownList runat="server" ID="ddlRMUs"></asp:DropDownList>
             </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" CssClass="label labelRequired" ID="Label4"
                        LabelFor="ddlMemberStatus">Member Status: </asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlMemberStatus" runat="server" Width="500px">
                        <asp:ListItem Value="0" Text="--- Select One ---"></asp:ListItem>
                        <asp:ListItem Value="1" Text="AGR"></asp:ListItem>
                        <asp:ListItem Value="2" Text="IMA"></asp:ListItem>
                        <asp:ListItem Value="3" Text="TR"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label1" CssClass="label labelRequired" runat="server">*Diagnosis:</asp:Label>
                </td>
                <td style="margin-left:-3px;">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <uc1:ICDCodeControl runat="server" ID="ucICDCodeControl" />
                            <br />
                             &nbsp;&nbsp; * - Changing this selection requires moving to a new tab and returning to synchronize data
                        </ContentTemplate>
                   </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label15" CssClass="label" runat="server">7th Character:</asp:Label>
                </td>
                <td style="margin-left:-3px;">
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
                <td>
                    <asp:Label ID="DiagnosisLabel" runat="server" CssClass="lblDisableText"></asp:Label>
                    <asp:TextBox ID="DiagnosisTextBox" Width="500px" TextMode="MultiLine" runat="server"
                        Rows="4" MaxLength="250"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label17" CssClass="label labelRequired" LabelFor="DAWGRecommendation">DAWG Recommendation: </asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="DAWGRecommendation" runat="server">
                        <asp:ListItem Value="0" Text="--- Select One ---" Selected="True"></asp:ListItem>
                        <asp:ListItem Value="1" Text="RTD"></asp:ListItem>
                        <asp:ListItem Value="5" Text="Full MEB"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>

            <tr>
                <td><asp:Label ID="Label20" runat="server" CssClass="label labelRequired">Initial Date of Code 37: </asp:Label></td>
                <td>
                    <asp:TextBox ID="txtCode37Date" MaxLength="10" onchange="DateCheck(this);" runat="server" Width="80"></asp:TextBox>
                </td>
            </tr>

            <tr>
                <td>
                    <asp:Label runat="server" ID="Label5" LabelFor="ddlAssignmentLimitation">Assignment Limitation: </asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlAssignmentLimitation" runat="server">
                        <asp:ListItem Value="0" Text="--- Select One ---" Selected="True"></asp:ListItem>
                        <asp:ListItem Value="1" Text="ALC1"></asp:ListItem>
                        <asp:ListItem Value="2" Text="ALC2"></asp:ListItem>
                        <asp:ListItem Value="3" Text="ALC3"></asp:ListItem>
                        <asp:ListItem Value="4" Text="No ALC"></asp:ListItem>
                        <asp:ListItem Value="5" Text="No ALC (indefinite)"></asp:ListItem>
                        <asp:ListItem Value="6" Text="Discharged"></asp:ListItem>
                        <asp:ListItem Value="7" Text="Retired"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    Memo/Letter:
                </td>
                <td>
                    <asp:DropDownList ID="ddlMemos" runat="server" DataSourceID="ObjectDataSource1" 
                        DataTextField="Title" DataValueField="Id">
                    </asp:DropDownList>
                    <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" 
                        OldValuesParameterFormatString="original_{0}" SelectMethod="GetTemplatesByModule" 
                        TypeName="ALOD.Data.MemoDao2">
                        <SelectParameters>
                            <asp:Parameter DefaultValue="11" Name="module" Type="Byte" />
                        </SelectParameters>
                    </asp:ObjectDataSource>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label6"  LabelFor="ddlApprovingAuthority">Approving Authority: </asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlApprovingAuthority" runat="server">
                        <asp:ListItem Value="0" Text="--- Select One ---" Selected="True"></asp:ListItem>
                        <asp:ListItem Value="1" Text="HQ AFRC/SGP"></asp:ListItem>
                        <asp:ListItem Value="2" Text="SAF"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label7" runat="server">Expiration/Renewal Date:</asp:Label>
                </td>
                <td>               
                            <asp:Label ID="Label8" runat="server" CssClass="lblDisableText"></asp:Label>
                            <asp:TextBox ID="txtExpirationDate" MaxLength="10" onchange="DateCheck(this);" runat="server" Width="80"></asp:TextBox>  
                            <asp:CheckBox ID="chbIndefinite" runat="server" Text="Indefinite" CssClass="boxPosition" />                                                                 
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label9" runat="server">Return To duty Date:</asp:Label>
                </td>
                <td>
                    <asp:Label ID="Label10" runat="server" CssClass="lblDisableText"></asp:Label>
                    <asp:TextBox ID="txtRTD" MaxLength="10" onchange="DateCheck(this);" runat="server" Width="80"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label11" runat="server">Effective Date:</asp:Label>
                </td>
                <td>
                    <asp:Label ID="Label12" runat="server" CssClass="lblDisableText"></asp:Label>
                    <asp:TextBox ID="txtEffectiveDate" MaxLength="10" onchange="DateCheck(this);" runat="server" Width="80"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label13" runat="server" CssClass="label labelRequired" LabelFor="txtForwardDate">Forward Date:</asp:Label>
                </td>
                <td>
                    <asp:Label ID="Label14" runat="server" CssClass="lblDisableText"></asp:Label>
                    <asp:TextBox ID="txtForwardDate" MaxLength="10" onchange="DateCheck(this);" runat="server" Width="80"></asp:TextBox>
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


        // Toggles on/off Expiration Date to Indefinite
           function SetIndefinite() {

            var tx, cb;
            tx = document.getElementById('ctl00_ctl00_ContentMain_ContentNested_txtExpirationDate');
            cb = document.getElementById('ctl00_ctl00_ContentMain_ContentNested_chbIndefinite');

            if (tx.value != "12/31/2100" || tx.value.length == 0) {
                tx.value = "12/31/2100";
 
            }

            else if (tx.value == "12/31/2100") {    
                tx.value = "";
                tx.disabled = false;  
                          
            }
        };


    
    </script>
</asp:Content>
