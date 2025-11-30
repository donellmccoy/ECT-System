<%@ Page Language="VB" MasterPageFile="~/Secure/AppealRequests/AppealRequest.master" AutoEventWireup="false" Async="true" Inherits="ALOD.Web.AP.Secure_ap_SMData" Title="Untitled Page" Codebehind="SMData.aspx.vb" %>

<%@ MasterType VirtualPath="~/Secure/AppealRequests/AppealRequest.master" %>

<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>

<%@ Register Src="../Shared/UserControls/CaseHistory.ascx" TagName="CaseHistory" TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck" TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/SignatureBlock.ascx" TagName="SignatureBlock" TagPrefix="uc1" %>

<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="HeaderNested">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentNested" runat="Server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('.datePicker').datepicker(calendarPick("DocPast", "<%=CalendarImage %>"));
        })
    </script>

    <!-- end search control -->
    <asp:Panel runat="server" ID="pnlPostCompletion" CssClass="dataBlock" Visible="False">
        <div class="dataBlock-header">
            Post Completion <i>(Digitally signing will complete case processing and make all changes final)</i>
        </div>
        <div class="dataBlock-body">
            <table class="dataTable">
                <tr runat="server" id="trLODPM" Visible="false">
                    <td class="number">
                        <asp:Label runat="server" ID="lblLODPMRow" Text="A" />
                    </td>
                    <td class="label labelRequired">
                        Select LOD PM:
                    </td>
                    <td class="value">
                        <asp:dropdownlist runat="server" ID="ddlLODPM" AutoPostBack="true" />
                    </td>
                </tr>
                <tr runat="server" id="trAppealAddress">
                    <td class="number">
                        <asp:Label runat="server" ID="lblAppealAddressRow" Text="A-1" />
                    </td>
                    <td class="label labelRequired">
                        LOD PM Address:
                    </td>
                    <td class="value">
                        <asp:Label ID="lblAppealAddress" runat="server" Visible="false"/>
                        <asp:TextBox runat="server" ID="txtAppealStreet" MaxLength="200" Width="180px" placeholder="Street" Visible="false"/>
                        <asp:TextBox runat="server" ID="txtAppealCity" MaxLength="100" Width="180px" placeholder="City" Visible="false"/>
                        <asp:TextBox runat="server" ID="txtAppealState" MaxLength="50" Width="80px" placeholder="State" Visible="false"/>
                        <asp:TextBox runat="server" ID="txtAppealZip" MaxLength="100" Width="80px" placeholder="ZIP" Visible="false"/>
                        <asp:TextBox runat="server" ID="txtAppealCountry" MaxLength="50" Width="80px" placeholder="Country" Visible="false"/>
                    </td>
                </tr>
                <tr runat="server" id="trEmail">
                    <td class="number">
                        <asp:Label runat="server" ID="lblEmailRow" Text="A-2" />
                    </td>
                    <td class="label labelRequired">
                        LOD PM Email:
                    </td>
                    <td class="value">
                        <asp:Label ID="lblEmail" runat="server" Visible="false"/>
                        <asp:TextBox runat="server" ID="txtEmail" Visible="false"/>
                    </td>
                </tr>
                <tr runat="server" id="trExtensionNumber" >
                    <td class="number">
                        <asp:Label runat="server" ID="lblExtensionNumberRow" Text="A-3"/>
                    </td>
                    <td class="label labelRequired">
                        LOD PM Phone:
                    </td>
                    <td class="value">
                        <asp:Label ID="lblHelpExtensionNumber" runat="server" Visible="false"/>
                        <asp:TextBox runat="server" ID="txtHelpExtensionNumber" Visible="false"/>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        <asp:Label runat="server" ID="lblMemberInformedRow" Text="B" />
                    </td>
                    <td class="label">
                        Member Informed:
                    </td>
                    <td class="value">
                        <asp:UpdatePanel ID="memberInformedUpdatePanel" runat="server" ChildrenAsTriggers="True">
                            <ContentTemplate>
                                <asp:CheckBox ID="MemberInformedCheckBox" runat="server" AutoPostBack="true" Visible="false"></asp:CheckBox>
                                <asp:Label ID="MemberNotifiedLabel" runat="server" Visible="false"></asp:Label>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        <asp:Label runat="server" ID="lblMemberDateRow" Text="C" />
                    </td>
                    <td class="label">
                        Member Date of Notification:
                    </td>
                    <td class="value">
                        <asp:TextBox ID="NotificationDate" runat="server" MaxLength="10" onchange="DateCheck(this);" Width="80" CssClass="datePicker" Visible="false"></asp:TextBox>
                        <asp:Label ID="NotificationDatelbl" runat="server" Visible="false"></asp:Label>
                    </td>
                </tr>
                <tr runat="server" id="trSave" visible="false">
                    <td class="number">
                        <asp:Label runat="server" ID="lblSaveRow" Text="D" />
                    </td>
                    <td class="label">
                        Save:
                    </td>
                    <td class="value">
                        <asp:Button runat="server" ID="btnSavePostProcessingData" Text="Save" />
                    </td>
                </tr>
                <tr runat="server" id="trDigitallySign" visible="false">
                    <td class="number">
                        <asp:Label runat="server" ID="lblDigitallySignRow" Text="E" />
                    </td>
                    <td class="label">
                        <asp:Label runat="server" ID="lblDigitallySign" Text="Sign:" />
                    </td>
                    <td class="value">
                        <asp:UpdatePanel ID="upnlDigitallySign" runat="server" ChildrenAsTriggers="True">
                            <ContentTemplate>
                                <asp:Button runat="server" ID="btnDigitallySign" Text="Digitally Sign" />
                                <uc1:SignatureBlock runat="server" ID="ucPostProcessingSigBlock" Visible="False" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr runat="server" id="trError" visible="false">
                    <td class="number">
                        <asp:Label runat="server" ID="lblErrorRow" />
                    </td>
                    <td class="label">
                        Error:
                    </td>
                    <td class="value">
                        <asp:Label runat="server" ID="lblError" cssclass="labelRequired"/>
                    </td>
                </tr>
            </table>
        </div>
        <uc1:SignatureCheck runat="server" ID="ucPostCompletionSigCheck" Template="Form348AppealPostProcessing" CssClass="sigcheck-form no-border" />
    </asp:Panel>
    
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
                        <uc1:SignatureBlock runat="server" ID="SigBlock" Visible="False" />
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
    <uc1:SignatureCheck ID="SigCheck" runat="server" Template="Form348AP" CssClass="sigcheck-form" />
</asp:Content>