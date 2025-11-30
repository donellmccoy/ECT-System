<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SC_PH/SC_PH.master" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.PH.Secure_sc_ph_Documents" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" Codebehind="Documents.aspx.vb" %>

<%@ MasterType VirtualPath="~/Secure/SC_PH/SC_PH.master" %>
<%@ Import Namespace="ALODWebUtility.Common" %>
<%@ Register Src="../Shared/UserControls/Documents.ascx" TagName="Documents" TagPrefix="uc1" %>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    
    <asp:Button runat="server" ID="RefreshButton" CssClass="hidden" />
    <br />
    <div>
        <table class="docHeader">
            <tr>
                <td style="width: 950px;">
                    <asp:Label runat="server" ID="PHForm" Text='1 . PH Tracking Forms' />
                </td>
            </tr>
        </table>
        <table class="docDetails">
            <tr class="docRow">
                <td style="text-align: left; width: 950px;">
                    <asp:HyperLink runat="server" ID="ViewDocLink348" NavigateUrl="#">
                        <asp:Image runat="server" ID="Icon" SkinID="iconPdf" CssClass="iconImg" ImageAlign="AbsMiddle" />
                        <asp:Label runat="server" ID="Description" Text='PH Non-Clinical Tracking Form' />
                    </asp:HyperLink>
                </td>
            </tr>
        </table>
    </div>
    <br />
     <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:Documents runat="server" ID="Documents" />
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">

    <script type="text/javascript">

        
        //used to display the memo editor
        function showEditor(memo, template) {

            var refId = getQueryStringValue("refId", 0);

            if (refId === 0) {
                return;
            }

            var url = $_HOSTNAME + "/Secure/Shared/Memos/EditMemo.aspx?id=" + refId + "&template=" + template + "&memo=" + memo;
            showPopup({
                Url: url,
                Width: '666',
                Height: '680',
                Resizable: false,
                ScrollBars: false,
                Center: true,
                Reload: memo === 0, //only reload if this is a new memo
                ReloadButton: element('<%= RefreshButton.ClientId %>')
            });
        }

        function confirmDelete() {
            return confirm("This will permanently delete this document.  Are you sure you want to proceed?");
        }
    
    </script>

</asp:Content>