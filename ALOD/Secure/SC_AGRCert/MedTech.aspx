<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SC_AGRCert/SC_AGRCert.master"
    MaintainScrollPositionOnPostback="true" AutoEventWireup="false"
    EnableEventValidation="false" Inherits="ALOD.Web.Special_Case.AGR.Secure_sc_agr_MedTech" CodeBehind="MedTech.aspx.vb" %>

<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Register Src="~/secure/Shared/UserControls/FindingsControl.ascx" TagName="Findings"
    TagPrefix="uc" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_AGRCert/SC_AGRCert.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck"
    TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/ICDCodeControl.ascx" TagName="ICDCodeControl" TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/ICD7thCharacterControl.ascx" TagName="ICD7thCharacterControl" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
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
            Data Entry
        </div>
        <table>
            <tr>
                <td class="auto-style1">A
                </td>
                <td class="label  labelRequired">*AFSC :
                </td>
                <td class="value">
                    <asp:Label ID="AFSCLabel" runat="server" CssClass="lblDisableText"></asp:Label>
                    <asp:TextBox ID="AFSCTextBox" Width="100px" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="auto-style1">B
                </td>
                <td class="label labelRequired">*Base:
                </td>
                <td>
                    <asp:DropDownList runat="server" ID="ddlRMUs"></asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="auto-style1">C
                </td>
                <td class="label labelRequired">*PHA Date:
                </td>
                <td class="auto-style1">
                    <asp:TextBox runat="server" ID="PHADate" MaxLength="10" onchange="DateCheck(this);"
                        Text="" Width="90px" />
                </td>
            </tr>
            <tr>
                <td class="auto-style1">D
                </td>
                <td class="label  labelRequired">*POC Unit:
                </td>
                <td>
                    <asp:TextBox runat="server" ID="POCUnitLabel"  Text="" Width="350px" />
                </td>
            </tr>
            <tr>
                <td class="auto-style1">E
                </td>
                <td class="label  labelRequired">*POC Address 1:
                </td>
                <td>
                    <asp:TextBox runat="server" ID="POCAddressLabel" placeholder="123 Anywhere Street, STE 1" Text="" Width="350px" />
                </td>
            </tr>
            <tr>
                <td class="auto-style1">E
                </td>
                <td class="label  labelRequired">*POC Address 2:
                </td>
                <td>
                    <asp:TextBox runat="server" ID="POCAddressLabelCityStateZip"  placeholder="City, State, Zipcode" Text="" Width="350px" />
                </td>
            </tr>
            <tr>
                <td class="auto-style1">F
                </td>
                <td class="label  labelRequired">*POC Name:
                </td>
                <td>
                    <asp:TextBox runat="server" ID="POCNameLabel"  Text="" Width="350px" />
                </td>
            </tr>
            <tr>
                <td class="auto-style1">G
                </td>
                <td class="label  labelRequired">*POC DSN/Phone:
                </td>
                <td>
                    <asp:TextBox runat="server" ID="POCPhoneLabel"  Text="" Width="90px" />
                </td>
            </tr>
            <tr>
                <td class="auto-style1">H
                </td>
                <td class="label  labelRequired">*POC Email:
                </td>
                <td>
                    <asp:TextBox runat="server" ID="POCEmailLabel"  Text="" Width="350px" />
                </td>
            </tr>
            <tr><td class="auto-style1">&nbsp;</td></tr>
            <tr>
                <td class="auto-style1">
                    <asp:Label runat="server" ID="ALCLabel" CssClass="labelRequired">ALC (if applicable): </asp:Label>
                </td>
                <td>&nbsp;</td>
                <td>
                    <asp:RadioButton ID="ALCY" runat="server" GroupName="ALC"
                        Text="Yes" />&nbsp;&nbsp;&nbsp;<asp:RadioButton ID="ALCN" runat="server"
                            GroupName="ALC" Text="No" />
                </td>
            </tr>
            <tr><td class="auto-style1">&nbsp;</td></tr>
            <tr>
                <td class="auto-style1">
                    <asp:Label runat="server" ID="MAJCOMLabel" CssClass="labelRequired">MAJCOM or higher: </asp:Label>
                </td>
                <td>&nbsp;</td>
                <td>
                    <asp:RadioButton ID="MAJCOMY" runat="server" GroupName="MAJCOM"
                        Text="Yes" />&nbsp;&nbsp;&nbsp;<asp:RadioButton ID="MAJCOMN" runat="server"
                            GroupName="MAJCOM" Text="No" />
                </td>
            </tr>
            <tr><td class="auto-style1">&nbsp;</td></tr>
            <tr>
                <td class="auto-style1">
                    <asp:Label runat="server" ID="InitialTourLabel" CssClass="labelRequired">Initial Tour?: </asp:Label>
                </td>
                <td>&nbsp;</td>
                <td>
                    <asp:RadioButton ID="InitialTourY" runat="server" AutoPostBack="true" GroupName="InitialTour"
                        Text="Yes" />&nbsp;&nbsp;&nbsp;<asp:RadioButton ID="InitialTourN" runat="server" AutoPostBack="true"
                            GroupName="InitialTour" Text="No" />
                </td>
            </tr>
            <tr><td class="auto-style1">&nbsp;</td></tr>
            <tr>
                <td class="auto-style1">
                    <asp:Label runat="server" ID="FollowOnLabel" CssClass="labelRequired">Follow On Tour?: </asp:Label>
                </td>
                <td>&nbsp;</td>
                <td>
                    <asp:RadioButton ID="FollowOnY" runat="server" AutoPostBack="true"  GroupName="FollowOnTour"
                        Text="Yes" />&nbsp;&nbsp;&nbsp;<asp:RadioButton ID="FollowOnN" runat="server" AutoPostBack="true"
                            GroupName="FollowOnTour" Text="No" />
                </td>
            </tr>
            <tr><td class="auto-style1">&nbsp;</td></tr>
        </table>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">
    <script type="text/javascript">

        $(function () {

            $('.datePicker').datepicker(calendarPick("Past", "<%=CalendarImage %>"));
            $('.datePickerAll').datepicker(calendarPick("All", "<%=CalendarImage %>"));

            $(".numericOnly").on('keydown', function (e) {
                var validKeys = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "Backspace", "Delete", "ArrowRight", "ArrowLeft", ".", "e", "E", "+", "-"];
                if (!validKeys.includes(e.key)) {
                    e.preventDefault();
                }
            });

        });

    </script>
</asp:Content>
<asp:Content ID="Content4" runat="server" contentplaceholderid="HeaderNested">
    <style type="text/css">
        .auto-style1 {
            width: 136px;
        }
        </style>
</asp:Content>

