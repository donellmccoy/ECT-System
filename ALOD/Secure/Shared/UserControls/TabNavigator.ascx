<%@ Control Language="VB" AutoEventWireup="false" Inherits="ALOD.Web.UserControls.TabNavigator" Codebehind="TabNavigator.ascx.vb" %>

<script type="text/javascript">

    var pageLoaded = false;

    $(document).ready(function() {
        pageLoaded = true;

        $('li.normal').hover(
        function() {
            $(this).addClass("hover");
        },
        function() {
            $(this).removeClass("hover");
        });
    });
    
    function navigateTab(cmd) 
    {
        if (!pageLoaded)
        {
            return;
        }
           
        element('<%= Me.ClientId %>_txtTabName').value = cmd;
        element('<%= Me.ClientId %>_btnTabClick').click();
        $.blockUI();
    }
    
    

</script>
<div class="hidden">
       <asp:TextBox runat="server" ID="txtTabName" />
       <asp:Button runat="server" ID="btnTabClick" />
</div>
<asp:Repeater id="rptLinks" runat="server">
	<HeaderTemplate>
		<ul id="wizardbar">
	</HeaderTemplate>
	<ItemTemplate>
		<li runat="server" id="Tab">
			<asp:LinkButton CausesValidation="False" OnClick="NavHeaderClicked" Runat="server" ID="hlink"></asp:LinkButton>
		</li>
	</ItemTemplate>
	<SeparatorTemplate>
	</SeparatorTemplate>
	<FooterTemplate>
		</ul>
	</FooterTemplate>
</asp:Repeater>
