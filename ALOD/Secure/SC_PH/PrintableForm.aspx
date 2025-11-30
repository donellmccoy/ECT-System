<%@ Language="VB" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.PH.PrintableForm" Codebehind="PrintableForm.aspx.vb" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title></title>
    <meta http-equiv="X-UA-Compatible" content="IE=Edge" /> 
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" EnablePageMethods="true" runat="server" />
        <div>
            <div class="phFormInfoPanel">
                <div class="phFormHeader">
                    <div class="phFormHeader-Left">
                        <span class="phFormHeaderLabel">DPH Name:</span><br />
                        <span class="phFormHeaderLabel">Wing:</span><br />
                        <span class="phFormHeaderLabel">Reporting Period:</span><br />
                        <span class="phFormHeaderLabel">Delinquent:</span><br />
                    </div>
                    <div class="phFormHeader-Right">
                        <asp:Label runat="server" ID="lblDPHName" /><br />
                        <asp:Label runat="server" ID="lblWingName" /><br />
                        <asp:Label runat="server" ID="lblReportingPeriod" /><br />
                        <asp:Label runat="server" ID="lblDelinquent" /><br />
                    </div>
                </div>
                <div class="keyPanel">
                    <span style="font-weight:bold;">KEY:</span><br />
                    <div class="keyPanelTable">
                        <div class="keyPanelTableColumn">
                            <span>Members Seen =</span>&nbsp;<asp:TextBox ID="txtMembersSeenKey" runat="server" Width="22" ReadOnly="true" CssClass="keyPanelTextbox">MS</asp:TextBox><br />
                            <span>Frequency =</span>&nbsp;<asp:TextBox ID="txtFrequencyKey" runat="server" Width="22" ReadOnly="true" CssClass="keyPanelTextbox">FR</asp:TextBox><br />
                            <span>Follow-up =</span>&nbsp;<asp:TextBox ID="txtFollowUpKey" runat="server" Width="22" ReadOnly="true" CssClass="keyPanelTextbox">FO</asp:TextBox>
                        </div>
                        <div class="keyPanelTableColumn">
                            <span>Army =</span>&nbsp;<asp:TextBox ID="txtArmyKey" runat="server" Width="22" ReadOnly="true" CssClass="keyPanelTextbox">Ar</asp:TextBox><br />
                            <span>Navy =</span>&nbsp;<asp:TextBox ID="txtNavyKey" runat="server" Width="22" ReadOnly="true" CssClass="keyPanelTextbox">NA</asp:TextBox><br />
                            <span>Air Force =</span>&nbsp;<asp:TextBox ID="txtAirForceKey" runat="server" Width="22" ReadOnly="true" CssClass="keyPanelTextbox">AF</asp:TextBox>
                        </div>
                        <div class="keyPanelTableColumn">
                            <span>Coast Guard =</span>&nbsp;<asp:TextBox ID="txtCoastGuardKey" runat="server" Width="22" ReadOnly="true" CssClass="keyPanelTextbox">CG</asp:TextBox><br />
                            <span>Marine Corps =</span>&nbsp;<asp:TextBox ID="txtMarineCorpsKey" runat="server" Width="22" ReadOnly="true" CssClass="keyPanelTextbox">MC</asp:TextBox>
                        </div>
                    </div>
                </div>
                <br style="clear:both;" />
            </div>
            
            <asp:Panel runat="server" ID="pnlPHForm" CssClass="acc" />
        </div>
    </form>
</body>
</html>