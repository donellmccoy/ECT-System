<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ICDCodeModalPopupControl.ascx.vb" Inherits="ALOD.Web.UserControls.ICDCodeModalPopupControl" %>

<%@ Register Src="../UserControls/ICDCodeControl.ascx" TagName="ICDCodeControl" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/ICD7thCharacterControl.ascx" TagName="ICD7thCharacterControl" TagPrefix="uc1" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="asp" %>

<style type="text/css">
    .blockLeft
    {
        display:inline-block;
        width:48%;
        margin-right:3px;
    }
        
    .blockRight
    {
        display:inline-block;
        float:right;
        width:48%;
        text-align:right;
    }
</style>

<asp:UpdatePanel runat="server" ID="upnlICDCodeModalPopup">
    <ContentTemplate>
        <asp:Panel runat="server" ID="pnlICDCodeControl" Style="display:none" CssClass="dataBlock"  >
            <div class="dataBlock-header">
                Select ICD Code
            </div>
            <div class="dataBlock-body">
                <uc1:ICDCodeControl runat="server" ID="ucICDCodeControl" />
                <br />
                <uc1:ICD7thCharacterControl runat="server" ID="ucICD7thCharacterControl" />
                <br />
                <br />
                <div class="blockLeft">
                    <asp:Label runat="server" ID="lblErrorMessages" CssClass="label labelRequired" Visible="false" />
                </div>
                <div class="blockRight">
                    <asp:Button runat="server" ID="btnSubmit" Text="Submit" />
                    <asp:Button runat="server" ID="btnCancel" Text="Cancel" />
                </div>
            </div>
        </asp:Panel>
        <asp:ModalPopupExtender runat="server" ID="mpeICDCodeModalPopup" BackgroundCssClass="modalBackground" Enabled="true" PopupControlID="pnlICDCodeControl" TargetControlID="btnFake" />
        <asp:Button runat="server" ID="btnFake" Style="display:none" />
    </ContentTemplate>
</asp:UpdatePanel>