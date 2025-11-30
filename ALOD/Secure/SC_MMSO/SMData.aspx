<%@ Page Language="VB" MasterPageFile="~/Secure/SC_MMSO/SC_MMSO.master" AutoEventWireup="false" Async="true" Inherits="ALOD.Web.Special_Case.MMSO.Secure_sc_mmso_SMData" Title="Untitled Page" Codebehind="SMData.aspx.vb" %>

<%@ Import Namespace="ALODWebUtility.Common" %>

<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_MMSO/SC_MMSO.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="../Shared/UserControls/CaseHistory.ascx" TagName="CaseHistory" TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck"
    TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/SignatureBlock.ascx" TagName="SignatureBlock"
    TagPrefix="uc1" %>
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="HeaderNested">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentNested" runat="Server">

    <script type="text/javascript">


        $(document).ready(function() {
            $('#srchBox').dialog({
                autoOpen: false,
                modal: true,
                resizable: true,
                width: 530,
                height: 320,
                buttons: {
                    'Select': function() {
                        SetReportingUnit();
                        $(this).dialog('close');
                        element('<%= SaveUnitButton.ClientId %>').click();
                    },
                    'Cancel': function() {
                        CancelSelection();
                        $(this).dialog('close');
                    }
                }

            });

            $('.open-dialog').click(function() {
                $('#srchBox').dialog('open');
            });
        })

        //Show Searcher
        function showSearcher(title) {
            initializeUnitSearcher();
            //Set Client controls where unit Id and unit names will be transferred
            $('#srchBox').dialog('open');
        }

        //Client accepted so Set Corresponding reporting units 
        function SetReportingUnit() {
            var srcherId = '<%=unitSearcher.ClientId %>'
            //Id is stroed in a label control.Transfer the value from the control
            element('<%=newUnitIDLabel.ClientId %>').value = element(srcherId + '_hdnSelectedUnit').value;
            //Name is stored in a label.Transfer the value from the control
            $('#<%=UnitTextBox.ClientId %>').text(element(srcherId + '_hdnSelectedUnitName').value);


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
    <div id="postCompletionDiv" runat="server" visible="false">
        <div class="formHeader">
            Post Completion
        </div>
        <div>
            <table class="dataTable">
                <tr>
                    <td class="number">
                        A
                    </td>
                    <td class="label">
                        Member Informed :
                    </td>
                    <td class="value">
                        <asp:CheckBox ID="MemberInformedCheckBox" runat="server"></asp:CheckBox>
                        <asp:Label ID="MemberNotifiedLabel" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        B
                    </td>
                    <td class="label">
                        Member Memo Created:
                    </td>
                    <td class="value">
                        <asp:Label ID="NotificationMemoCreatedLabel" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div class="formHeader">
        Member Information
    </div>
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
                <asp:Label runat="server" ID="UnitTextBox" />
                <asp:LinkButton runat="server" ID="ChangeUnitButton" Text="[change]" />
                <asp:TextBox CssClass="hidden" ID="newUnitIDLabel" runat="Server" />
                <asp:Button runat="server" ID="SaveUnitButton" CssClass="hidden" />
                <uc1:SignatureBlock ID="SigBlock" runat="server" />
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
    <br />
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:CaseHistory runat="server" ID="CaseHistory" />
        </ContentTemplate>
    </asp:UpdatePanel>
    <uc1:SignatureCheck ID="SigCheck" runat="server" Template="MMSO_Unit" CssClass="sigcheck-form" />
</asp:Content>
