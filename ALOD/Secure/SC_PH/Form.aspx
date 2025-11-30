<%@ Page Title="PH Form" Language="VB" MasterPageFile="~/Secure/SC_PH/SC_PH.master" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.PH.Form" Codebehind="Form.aspx.vb" %>

<%@ Import Namespace="ALODWebUtility.Common" %>

<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_PH/SC_PH.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck" TagPrefix="uc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="HeaderNested" Runat="Server">


    <style type="text/css">
        .keyPanel
        {
            display:inline-block;
            width:49%;
            margin-right:3px;
            background-color:#DDDDDD;
            padding: 3px 4px 3px 4px;
        }

        .keyPanelTable 
        {
            display:table;
            width:100%;
        }

        .keyPanelTableColumn
        {
            display:table-cell;
            text-align:right;
        }

        .keyPanelTextbox
        {
            border:1px solid black;
            margin: 1px 1px 1px 1px;
        }
        
        .errorValidationPanel
        {
            display:inline-block;
            float:right;
            width:49%;
            background-color:#DDDDDD;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" Runat="Server">
    <div style="display:flex">
        <div class="keyPanel">
            <span style="font-weight:bold;">Reporting Period: </span>
            <asp:Label runat="server" ID="lblReportingPeriod" Text="Month & Year" />
            <br />
            <br />
            <span style="font-weight:bold;">Form Last Modified: </span>
            <asp:Label runat="server" ID="lblLastModified" Text="Month & Year" />
            <br />
            <br />
            <span style="font-weight:bold;" aria-hidden="true">KEY: </span>
            <br />
            <div class="keyPanelTable" aria-hidden="true">
                <div class="keyPanelTableColumn">
                    <span>Members Seen =</span>&nbsp;<asp:TextBox ID="txtMembersSeenKey" runat="server" Width="22" Enabled="false" ReadOnly="true" CssClass="keyPanelTextbox">MS</asp:TextBox><br />
                    <span>Frequency =</span>&nbsp;<asp:TextBox ID="txtFrequencyKey" runat="server" Width="22" Enabled="false" ReadOnly="true" CssClass="keyPanelTextbox">FR</asp:TextBox><br />
                    <span>Follow-up =</span>&nbsp;<asp:TextBox ID="txtFollowUpKey" runat="server" Width="22" Enabled="false" ReadOnly="true" CssClass="keyPanelTextbox">FO</asp:TextBox>
                </div>
                <div class="keyPanelTableColumn">
                    <span>Army =</span>&nbsp;<asp:TextBox ID="txtArmyKey" runat="server" Width="22" Enabled="false" ReadOnly="true" CssClass="keyPanelTextbox">Ar</asp:TextBox><br />
                    <span>Navy =</span>&nbsp;<asp:TextBox ID="txtNavyKey" runat="server" Width="22" Enabled="false" ReadOnly="true" CssClass="keyPanelTextbox">NA</asp:TextBox><br />
                    <span>Air Force =</span>&nbsp;<asp:TextBox ID="txtAirForceKey" runat="server" Width="22" Enabled="false" ReadOnly="true" CssClass="keyPanelTextbox">AF</asp:TextBox>
                </div>
                <div class="keyPanelTableColumn">
                    <span>Coast Guard =</span>&nbsp;<asp:TextBox ID="txtCoastGuardKey" runat="server" Width="22" Enabled="false" ReadOnly="true" CssClass="keyPanelTextbox">CG</asp:TextBox><br />
                    <span>Marine Corps =</span>&nbsp;<asp:TextBox ID="txtMarineCorpsKey" runat="server" Width="22" Enabled="false" ReadOnly="true" CssClass="keyPanelTextbox">MC</asp:TextBox>
                </div>
            </div>
        </div>
        <div class="errorValidationPanel">
            <p style="font-weight:bold;">
                * Remember to save often using the SAVE button at the bottom of the page.
            </p>
            <asp:Panel runat="server" ID="pnlValidationErrors" Visible="false">
                <p style="font-weight:bold;">Input Errors were found in the following sections:</p>
                <asp:BulletedList ID="bllValidationList" runat="server" CssClass="labelRequired" />
            </asp:Panel>
        </div>
        
    </div>
    <br style="clear:both;" />

    <ajax:Accordion ID="accPHForm" runat="server" HeaderCssClass="accHeader" ContentCssClass="accContent" CssClass="acc" HeaderSelectedCssClass="accHeaderSelected">
        <Panes>
        </Panes>
    </ajax:Accordion>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" Runat="Server">
    <script type="text/javascript">
        function pageLoad() {
            $find('<%= accPHForm.ClientID %>' + '_AccordionExtender').add_selectedIndexChanged(accordion_selectedIndexChanged);

            $find('<%= accPHForm.ClientID %>' + '_AccordionExtender').get_Pane(0).header.title = "Click another header to collapse this one...";
        }

        function accordion_selectedIndexChanged(sender, args) {
            var oldIndex = args.get_oldIndex();
            var newIndex = args.get_selectedIndex();

            var oldPane = sender.get_Pane(oldIndex);
            oldPane.header.title = "Click to expand...";

            var newPane = sender.get_Pane(newIndex);
            newPane.header.title = "Click another header to collapse this one...";
        }
    </script>
</asp:Content>

