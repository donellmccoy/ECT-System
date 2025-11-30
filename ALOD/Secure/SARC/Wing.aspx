<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SARC/SARCMaster.master" AutoEventWireup="false" Inherits="ALOD.Web.SARC.Wing" CodeBehind="Wing.aspx.vb" %>

<%@ Import Namespace="ALODWebUtility.Common" %>

<%@ MasterType VirtualPath="~/Secure/SARC/SARCMaster.master" %>

<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>

<%@ Register Src="~/Secure/Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck" TagPrefix="uc1" %>
<%@ Register Src="~/Secure/Shared/UserControls/ICDCodeModalPopupControl.ascx" TagName="ICDCodeModalPopupControl" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderNested" runat="Server">
    <style type="text/css">
        .otherICDFullNameBlock
        {
            display:inline-block;
            float:right;
            width:89%;
        }
        
        .otherICDRemoveBlock
        {
            display:inline-block;
            margin-right:3px;
            width:10%;
        }
        .tableStyle
        {
            width:100%;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    <asp:Panel runat="server" ID="pnlWingSARCControls" CssClass="dataBlock">
        <div class="dataBlock-header">
            Wing SARC/RSL Section:
        </div>
        <div class="dataBlock-body">
            <table class="tableStyle">
                <tr>
                    <td class="number">
                        A
                    </td>
                    <td class="label labelRequired">
                        *Date of Incident:
                    </td>
                    <td class="value">
                        <asp:TextBox runat="server" ID="txtDateOfIncident" MaxLength="10" onchange="DateCheck(this);" Width="80" CssClass="datePickerPast" Visible="false" />
                        <asp:Label runat="server" ID="lblDateOfIncident" Visible="false" />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        B
                    </td>
                    <td class="label labelRequired">
                        *Defense Sexual Assualt Incident Database Case Number:
                    </td>
                    <td class="value">
                        <asp:TextBox runat="server" ID="txtSexualAssaultDatabaseCaseNumber" MaxLength="50" Width="200px" Visible="false" />
                        <asp:Label runat="server" ID="lblSexualAssaultDatabaseCaseNumber" Visible="false" />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        C.1
                    </td>
                    <td class="label labelRequired">
                        *Membership and Duty Status:
                    </td>
                    <td class="value">
                        <asp:RadioButtonList runat="server" ID="rdblstMembershipStatus" RepeatDirection="Horizontal" Visible="false" />
                        <asp:Label runat="server" ID="lblMembershipStatus" Visible="false" />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        C.2
                    </td>
                    <td class="label labelRequired">
                        *Duration of Orders or IDT Date and Time:
                    </td>
                    <td class="value">
                        <table>
                            <tr>
                                <td class="align-right">
                                    START -
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtDurationStartDate" MaxLength="10" onchange="DateCheck(this);" Width="80" CssClass="datePickerPast" Visible="false" />
                                    <asp:TextBox runat="server" ID="txtDurationStartTime" MaxLength="4" onchange="TimeCheck(this);" Width="40" Visible="false" />
                                    <asp:Label runat="server" ID="lblDurationStart" Visible="false" />
                                </td>
                            </tr>
                            <tr>
                                <td class="align-right">
                                    END -
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtDurationEndDate" MaxLength="10" onchange="DateCheck(this);" Width="80" CssClass="datePickerPast" Visible="false" />
                                    <asp:TextBox runat="server" ID="txtDurationEndTime" MaxLength="4" onchange="TimeCheck(this);" Width="40" Visible="false" />
                                    <asp:Label runat="server" ID="lblDurationEnd" Visible="false" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
            <asp:UpdatePanel runat="server" ID="upnlAddICDCode">
                <ContentTemplate>
                    <table runat="server" id="tblICDCodes" class="tableStyle">
                        <tr>
                            <td class="number">
                                <asp:Label runat="server" ID="lblICDCodesRowId" />
                            </td>
                            <td class="label labelRequired">
                                *ICD Codes:
                            </td>
                            <td class="value">
                                <asp:CheckBoxList runat="server" ID="chklstICDCodes" RepeatDirection="Vertical" AutoPostBack="true" Visible="false">
                                    <asp:ListItem Text="E968.8 (Assault by Other Specified Means)" />
                                    <asp:ListItem Text="E969.9 (Poisoning by Unspecified Psychotropic Agent)" />
                                    <asp:ListItem Text="Other, Enter ICD Code(s)" />
                                </asp:CheckBoxList>
                                <asp:Label runat="server" ID="lblICDCodes" Visible="false" />
                            </td>
                        </tr>
                        <tr runat="server" id="trOtherICDCodes" visible="false">
                            <td class="number">
                                D.2
                            </td>
                            <td class="label">
                                Other ICD Codes:
                            </td>
                            <td class="value">
                                <asp:Label runat="server" ID="lblTemp" />
                                <asp:Repeater runat="server" ID="rptrICDOthers">
                                    <SeparatorTemplate>
                                        <br />
                                    </SeparatorTemplate>
                                    <ItemTemplate>
                                        <div class="otherICDRemoveBlock">
                                            <asp:LinkButton runat="server" ID="lkbtnRemove" Text="Remove" />
                                        </div>
                                        <div class="otherICDFullNameBlock">
                                            <asp:Label runat="server" ID="lblOtherICDFullName" />
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <uc1:ICDCodeModalPopupControl runat="server" ID="ucICDCodeModulePopup" />
                                <br />
                                <asp:Button runat="server" ID="btnAddICDCode" Text="Add ICD Code" />
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
            <table class="tableStyle">
                <tr>
                    <td class="number">
                        E
                    </td>
                    <td class="label labelRequired">
                        *Reported incident occurred while member was in a Duty Status:
                    </td>
                    <td class="value">
                        <asp:RadioButtonList runat="server" ID="rdblDutyStatusDetermination" RepeatDirection="Horizontal" Visible="false">
                            <asp:ListItem Value="true" Text="Yes" />
                            <asp:ListItem Value="false" Text="No" />
                        </asp:RadioButtonList>
                        <asp:Label runat="server" ID="lblDutyStatusDetermination" Visible="false" />
                    </td>
                </tr>
            </table>
            <uc1:SignatureCheck runat="server" ID="ucSigCheckWingSARCRSL" Template="Form348SARCWing" CssClass="sigcheck-form no-border" Visible="false" />
        </div>
    </asp:Panel>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">
    <script type="text/javascript">
        $(function () {
            $('.datePickerPast').datepicker(calendarPick("Past", "<%=CalendarImage %>"));
            $('.datePickerFuture').datepicker(calendarPick("All", "<%=CalendarImage %>"));
        });



    </script>
</asp:Content>
