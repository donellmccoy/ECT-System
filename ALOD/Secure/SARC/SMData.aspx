<%@ Page Language="VB" MasterPageFile="~/Secure/SARC/SARCMaster.master" AutoEventWireup="false" Async="true" Inherits="ALOD.Web.SARC.SMData" Title="Untitled Page" CodeBehind="SMData.aspx.vb" %>

<%@ MasterType VirtualPath="~/Secure/SARC/SARCMaster.master" %>

<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>

<%@ Register Src="../Shared/UserControls/CaseHistory.ascx" TagName="CaseHistory" TagPrefix="uc1" %>
<%@ Register Src="~/Secure/Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck" TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/SignatureBlock.ascx" TagName="SignatureBlock" TagPrefix="uc1" %>

<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="HeaderNested">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentNested" runat="Server">
    <script type="text/javascript">
        $(function () {
            $('#srchBox').dialog({
                autoOpen: false,
                modal: true,
                resizable: true,
                width: 530,
                height: 320,
                buttons: {
                    'Select': function () {
                        SetReportingUnit();
                        $(this).dialog('close');
                        element('<%= SaveUnitButton.ClientId %>').click();
                    },
                    'Cancel': function () {
                        CancelSelection();
                        $(this).dialog('close');
                    }
                }

            });

            $('.open-dialog').click(function () {
                $('#srchBox').dialog('open');
            });
        })

        function pageLoad() {
            $('.datePickerPast').datepicker(calendarPick("Past", "<%=CalendarImage %>"));
        }

        //Show Searcher
        function showSearcher(title) {
            $('#srchBox').dialog('open');
        }

        //Client accepted so Set Corresponding reporting units 
        function SetReportingUnit() {
            var srcherId = '<%=unitSearcher.ClientId %>'
            //Id is stroed in a label control.Transfer the value from the control
            element('<%=newUnitIDLabel.ClientId %>').value = element(srcherId + '_hdnSelectedUnit').value;
            //Name is stored in a label.Transfer the value from the control
            $('#<%=lblUnit.ClientId %>').text(element(srcherId + '_hdnSelectedUnitName').value);


            // return false;

        }

        //Client cancelled so ignore the dialog values
        function CancelSelection() {
            var srcherId = '<%=unitSearcher.ClientId %>'
            element(srcherId + '_hdnSelectedUnit').value = "";
            element(srcherId + '_hdnSelectedUnitName').value = "";
            return false;
        }
    </script>

    <div id="srchBox" class="hidden" title="Find Unit">
        <lod:unitSearcher ID="unitSearcher" ActiveOnly="true" runat="server" />
    </div>
    <!-- end search control -->
    <asp:Panel runat="server" ID="pnlPostCompletion" CssClass="dataBlock" Visible="False">
        <div class="dataBlock-header">
            Post Completion <i>(Digitally signing will complete case processing and make all changes final)</i>
        </div>
        <div class="dataBlock-body">
            <table class="dataTable">
                <tr runat="server" id="trWingSARC" Visible="false">
                    <td class="number">
                        <asp:Label runat="server" ID="lblWingSARCRow" Text="A" />
                    </td>
                    <td class="label labelRequired">
                        Select Wing SARC:
                    </td>
                    <td class="value">
                        <asp:dropdownlist runat="server" ID="ddlWingSARC" AutoPostBack="true" />
                    </td>
                </tr>
                <tr runat="server" id="trAppealAddress">
                    <td class="number">
                        <asp:Label runat="server" ID="lblAppealAddressRow" Text="A-1" />
                    </td>
                    <td class="label labelRequired">
                        *Wing SARC/RSL Address:
                    </td>
                    <td class="value">
                        <asp:Label ID="lblAppealAddress" runat="server" Visible="false" />
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
                <tr runat="server" id="trExtensionNumber">
                    <td class="number">
                        <asp:Label runat="server" ID="lblExtensionNumberRow" Text="A-3" />
                    </td>
                    <td class="label labelRequired">
                        *Wing SARC Contact Info:
                    </td>
                    <td class="value">
                        <asp:Label ID="lblHelpExtensionNumber" runat="server" Visible="false" />
                        <asp:TextBox runat="server" ID="txtHelpExtensionNumber" MaxLength="50" Visible="false" />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        <asp:Label runat="server" ID="lblMemberNotifiedRow" Text="B" />
                    </td>
                    <td class="label labelRequired">
                        *Member Notified:
                    </td>
                    <td class="value">
                        <asp:UpdatePanel ID="memberInformedUpdatePanel" runat="server" ChildrenAsTriggers="True">
                            <ContentTemplate>
                                <asp:CheckBox ID="MemberInformedCheckBox" runat="server" AutoPostBack="true" Visible="false" />
                                <asp:Label ID="MemberNotifiedLabel" runat="server" Visible="false" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        <asp:Label runat="server" ID="lblMemberMemoCreatedRow" Text="C" />
                    </td>
                    <td class="label">Member Memo Uploaded:
                    </td>
                    <td class="value">
                        <asp:Label ID="NotificationMemoCreatedLabel" runat="server" Text="No" />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        <asp:Label runat="server" ID="lblMemberNotificationDateRow" Text="D" />
                    </td>
                    <td class="label labelRequired">
                        *Member Notification Date:
                    </td>
                    <td class="value">
                        <asp:TextBox ID="NotificationDate" MaxLength="10" onchange="DateCheck(this);" CssClass="datePickerPast" runat="server" Width="80" Visible="False" />
                        <asp:Label ID="NotificationDatelbl" runat="server" Visible="false" />
                    </td>
                </tr>
                <tr runat="server" id="trSave" visible="false">
                    <td class="number">
                        <asp:Label runat="server" ID="lblSaveRow" Text="E" />
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
                        <asp:Label runat="server" ID="lblDigitallySignRow" Text="F" />
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
        <uc1:SignatureCheck runat="server" ID="ucSigCheckWingSARCPostCompletion" Template="Form348SARCPostProcessing" CssClass="sigcheck-form no-border" />
    </asp:Panel>

    <asp:Panel runat="server" ID="Panel1" CssClass="dataBlock">
        <div class="dataBlock-header">
            Member Information
        </div>
        <div class="dataBlock-body">
            <table class="dataTable">
                <tr>
                    <td class="number">A
                    </td>
                    <td class="label">Name:
                    </td>
                    <td class="value">
                        <asp:Label ID="lblName" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="number">B
                    </td>
                    <td class="label">Rank:
                    </td>
                    <td class="value">
                        <asp:Label ID="lblRank" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="number">C
                    </td>
                    <td class="label">DOB:
                    </td>
                    <td class="value">
                        <asp:Label ID="lbldob" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="number">D
                    </td>
                    <td class="label">Unit:
                    </td>
                    <td class="value">
                        <asp:Label runat="server" ID="lblUnit" />
                        <asp:LinkButton runat="server" ID="ChangeUnitButton" Text="[change]" />
                        <asp:TextBox CssClass="hidden" ID="newUnitIDLabel" runat="Server" />
                        <asp:Button runat="server" ID="SaveUnitButton" CssClass="hidden" />
                        <uc1:SignatureBlock runat="server" ID="ucUnitChangeSigBlock" Visible="False" />
                    </td>
                </tr>
                <tr>
                    <td class="number">E
                    </td>
                    <td class="label">Component:
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
    <uc1:SignatureCheck ID="SigCheck" runat="server" Template="Form348SARC" CssClass="sigcheck-form" />
</asp:Content>
