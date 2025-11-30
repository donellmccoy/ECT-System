<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SC_FastTrack/SC_FastTrack.master" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.IRILO.Secure_sc__ft_lod_Documents" MaintainScrollPositionOnPostback="true" Codebehind="LODDocuments.aspx.vb" %>

<%@ Import Namespace="ALODWebUtility.Common" %>

<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ Register Src="../Shared/UserControls/LODDocuments.ascx" TagName="LODDocuments" TagPrefix="uc1" %>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    <ajax:Accordion ID="accLODDocuments" runat="server" HeaderCssClass="LODHeader" ContentCssClass="LODContent" CssClass="LODDocuments" HeaderSelectedCssClass="LODHeaderSelected">
        <Panes>
        </Panes>
    </ajax:Accordion>
    <asp:Panel runat="server" ID="pnlNoLODs" Visible="false">
        <asp:Label runat="server" ID="lblNoLODsMessage" />
    </asp:Panel>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" Runat="Server">
    <script type="text/javascript">
        function pageLoad() {
            $find('<%= accLODDocuments.ClientID %>' + '_AccordionExtender').add_selectedIndexChanged(accordion_selectedIndexChanged);

            $find('<%= accLODDocuments.ClientID %>' + '_AccordionExtender').get_Pane(0).header.title = "Click another header to collapse this one...";
        }

        function accordion_selectedIndexChanged(sender, args) {
            var oldIndex = args.get_oldIndex();
            var newIndex = args.get_selectedIndex();

            var oldPane = sender.get_Pane(oldIndex);
            oldPane.header.title = "Click to expand...";

            var newPane = sender.get_Pane(newIndex);
            newPane.header.title = "Click another header to collapse this one...";
        }
        
        function redirect(id) {
            var url = $_HOSTNAME + '/Secure/lod/init.aspx?refId='
            window.location = url + id;
            return false;
        }
    </script>
</asp:Content>

