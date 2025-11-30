<%@ Control Language="VB" AutoEventWireup="false" Inherits="ALOD.Web.UserControls.Secure_Shared_UserControls_SignatureBlock" Codebehind="SignatureBlock.ascx.vb" %>

<asp:Button runat="server" ID="ProceedButton" Text="Proceed" CssClass="hidden" />

<iframe id="DBSignConversation" src="/Secure/Shared/DBSign/clear.htm#" frameborder="0" style="width:100%;"
    scrolling="no" height="0" runat="server"></iframe>
        
    <script type="text/javascript">

    function initSign()
    {
        return confirm('Are you sure you want to Digitally Sign?');
    }
    
    function endSign()
    {       
        element('<%= ProceedButton.ClientId %>').disabled = false;
        element('<%= ProceedButton.ClientId %>').click();
        $.blockUI();
    }
            
</script>