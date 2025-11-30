<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master"  AutoEventWireup="false" Inherits="ALOD.Web.Sys.Secure_Shared_Admin_DBSignTest" Codebehind="TestComponents.aspx.vb" %>

<%@ Register Src="../UserControls/SignatureBlock.ascx" TagName="SignatureBlock"
    TagPrefix="uc1" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_MEB/SC_MEB.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="~/secure/Shared/UserControls/RLB.ascx" TagName="RLB" TagPrefix="uc" %>
<%@ Reference Control="~/Secure/Shared/UserControls/RLB.ascx" %>
<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Register Src="~/secure/Shared/UserControls/FindingsControl.ascx" TagName="Findings"
    TagPrefix="uc" %>


<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="head">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
	<asp:Button ID="SignButton" runat="server" Text="Digitally Sign" /><br />
	<uc1:SignatureBlock ID="SigBlock" runat="server" />
    <asp:Label runat="server" ID="DBSignMessageText" Text=""  Font-Bold="true" Style="color:Green; display:block;" />
        <asp:Label ID="SSNHidden" runat="server"  Text="" style="display:none;" />

    <asp:Button ID="DocumentTestButton" runat="server" Text="SRXLite Test" style="margin-top:50px; display:block;" OnClientClick="uploadDoc()"/>
   
    <asp:Button runat="server" ID="RefreshButton" CssClass="hidden" />


	
	</asp:Content>



    <asp:Content runat="server" ID="Content2" ContentPlaceHolderID="ContentFooter">
        <script type="text/javascript">
    
        //Opens up the  document URL  in a dailog window
        function uploadDoc() {
            showPopup({
                'Url': "../DocumentUpload.aspx?group=999999999&id=999999999&entity=" + document.getElementById('<%=SSNHidden.ClientID%>').innerText,
               // 'Url': "~/default.aspx",
                'Width': 500,
                'Height': 300,
                'Center': true,
                'Reload': true,
                'Resizable': true,
                'ScrollBars': true,
                ReloadButton: element('<%= RefreshButton.ClientId %>')
            });
        }



    </script>

</asp:Content>