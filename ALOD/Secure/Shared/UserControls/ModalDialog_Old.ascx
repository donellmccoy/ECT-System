<%@ Control Language="C#" AutoEventWireup="true" Inherits="ALOD.Web.UserControls.UserControl_ModalDialog" Codebehind="ModalDialog_Old.ascx.cs" %>
<div id="timeoutDialog">
    <asp:Image runat="server" ID="WarningIcon" AlternateText="Warning" ImageUrl="~/images/warning.gif" ImageAlign="AbsMiddle" />
    <span style="color: red; font-weight: bold; font-size: 14px;">Your session is about
        to expire</span>
    <br />
    <br />
    <span style="color: black; font-weight: normal; font-size: 12px;">To continue using
        the website, press the 'Continue' button below.</span>
    <br />
    <br />
    <span id="TimeLeft"></span>
</div>
<asp:Button runat="server" ID="ButtonContinue" CssClass="hidden" />
