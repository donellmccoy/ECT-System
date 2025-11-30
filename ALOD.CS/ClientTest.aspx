<%@ Page Title="" Language="C#" MasterPageFile="~/Public/Public.master" AutoEventWireup="false" Inherits="ALOD.Web.ClientTest" CodeBehind="ClientTest.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentHeader" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="Server">
    <div>
        <div id="front-actions">
            <div style="float: left; width: 600px; height: 275px; float: left; background-repeat: no-repeat;
                background-color: transparent;">
                <img style="width: 600px; height: 275px" src="App_Themes/DefaultBlue/images/ALOD1.jpg"
                    id="banner" alt="ALOD Banner"/>
            </div>
            <div id="front-login">
                <div style="padding: 40px 30px 20px 0px;">
                    <strong>Welcome to the ALOD Client Test page.</strong>
                    <br /><br />
                    If you can see this text you're computer is
                    properly configured to access the ALOD application
                </div>
            </div>
        </div>
        <div id="frontpage-content">
            <div id="TimeoutMessage">
                <asp:Label runat="server" ID="ErrorLabel" Visible="false"></asp:Label></div>
            <p class="disclaimer">
                <asp:Label runat="server" ID="SiteDisclaimer" Text="<%$ Resources:Global, SiteDisclaimer %>"></asp:Label></p>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFooter" runat="Server">

<script type="text/javascript">
    /* $(document).ready(function () {Deprecated ready Syntax: -- Diamante Lawson 10/12/2023 */

  (function() {
        element('ctl00_lnkHome').href = '#';
    });
</script>
</asp:Content>
