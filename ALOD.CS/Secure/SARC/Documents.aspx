<%@ Page Title="" Language="C#" MasterPageFile="~/Secure/SARC/SARCMaster.master" AutoEventWireup="false" Inherits="ALOD.Web.SARC.Documents" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" CodeBehind="Documents.aspx.cs" %>

<%@ MasterType VirtualPath="~/Secure/SARC/SARCMaster.master" %>
<%@ Import Namespace="ALODWebUtility.Common" %>
<%@ Register Src="../Shared/UserControls/Documents.ascx" TagName="Documents" TagPrefix="uc1" %>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    
    <asp:Button runat="server" ID="RefreshButton" CssClass="hidden" />
    <asp:Button runat="server" ID="NewMemoButton" CssClass="hidden" />
    <br />
    <asp:Panel runat="server" ID="pnlForm348R">
        <table class="docHeader">
            <tr>
                <td style="width: 950px;">
                    <asp:Label runat="server" ID="Form348_title" Text='1. AF Form 348-R' />
                </td>
            </tr>
        </table>
        <table class="docDetails">
            <tr class="docRow">
                <td style="text-align: left; width: 950px;">
                    <asp:HyperLink runat="server" ID="ViewDocLink348" NavigateUrl="#">
                        <asp:Image runat="server" ID="Icon" SkinID="iconPdf" CssClass="iconImg" ImageAlign="AbsMiddle" />
                        <asp:Label runat="server" ID="Description" Text='AFRC Form 348' />
                    </asp:HyperLink>
                </td>
            </tr>

            <tr id="RowSARC" class="docRow" visible="false" runat="server">
                <td style="text-align: left; width: 440px;">
                    <asp:HyperLink runat="server" ID="ViewSARCAppealLink" NavigateUrl="#">
                        <asp:Image runat="server" ID="IconB" SkinID="iconPdf" CssClass="iconImg" ImageAlign="AbsMiddle" />
                        <asp:Label runat="server" ID="SARCAppealDescription" Text='' />
                    </asp:HyperLink>
                    <asp:Label runat="server" ID="SARCAppealMemoError" Visible="false" CssClass="labelRequired" Text="No Appeal Memo found please contact help desk" />
                </td>
            </tr>
        </table>
        <br />
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlMemo">
        <table class="docHeader">
            <tr>
                <td style="width: 650px;">
                    <asp:Label runat="server" ID="Memorandum_Title" Text='2. Memorandum' />
                </td>
                <td style="width: 300px; text-align: right;" colspan="3">
                    <asp:Image runat="server" ID="CreateMemo" ImageAlign="AbsMiddle" ImageUrl="~/images/create_document.gif"
                        CssClass="iconUpload" AlternateText="Create New Memo" />
                </td>
            </tr>
        </table>
        <asp:Repeater runat="server" ID="MemoRepeater">
            <ItemTemplate>
                <table class="docDetails">
                    <tr class="docRow">
                        <td style="text-align: left; width: 475px;">

                            <asp:HyperLink runat="server" ID="ViewMemoLink" NavigateUrl="#">
                                <asp:Image runat="server" ID="Icon" SkinID="iconPdf" CssClass="iconImg" ImageAlign="AbsMiddle" />
                                <asp:Label runat="server" ID="memoTitle" Text='<%#Server.HtmlEncode(Eval("Template.Title"))%>' />
                            </asp:HyperLink>

                        </td>
                        <td style="text-align: left; width: 275px;">
                            <%#Server.HtmlEncode(Eval("CreatedDate", "{0:" + DATE_FORMAT + "}"))%> 
                        </td>

                        <td style="text-align: right; width: 200px;">
                            <asp:Image runat="server" ID="EditMemo" ImageAlign="AbsMiddle" AlternateText="Edit Document"
                                SkinID="imgEditSmall" CssClass="pointer" />
                            &nbsp;
                            <asp:ImageButton runat="server" SkinID="buttonDelete" ID="DeleteMemo" CommandArgument='<%# Eval("Id") %>'
                                CommandName="DeleteMemo" ImageAlign="AbsMiddle" AlternateText="Delete Memo" />
                            <ajax:ConfirmButtonExtender runat="server" ID="confirmDelete" ConfirmText="Are you sure you want to delete this memo?"
                                TargetControlID="DeleteMemo" />
                            <asp:Image runat="server" ID="LockedDocument" SkinID="imgLocked" AlternateText="Document is Read-Only"
                                Visible="false" />
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
        </asp:Repeater>
    </asp:Panel>
    
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:Documents runat="server" ID="Documents" />
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">

    <script type="text/javascript">

        
        //used to display the memo editor
        function showEditor(memo, template, refId) {

            /*var refId = getQueryStringValue("refId", 0);*/

            //if (refId === 0) {
            //    return;
            //}

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

        <%--function showEditor(memo, template, isDeterminationTemplate) {

            var refId = getQueryStringValue("refId", 0);
            var reloadButtonName = "<%= RefreshButton.ClientId %>";

            //if (refId === 0) {
            //    return;
            //}
            if (memo === 0 && isDeterminationTemplate == true) {
                reloadButtonName = "<%= NewMemoButton.ClientId %>";
            }

            var url = $_HOSTNAME + "/Secure/Shared/Memos/EditMemo.aspx?id=" + refId + "&template=" + template + "&memo=" + memo + "&mod=" + <%= ModuleType%>;
            showPopup({
                Url: url,
                Width: '666',
                Height: '680',
                Resizable: false,
                ScrollBars: false,
                Center: true,
                Reload: memo === 0, //only reload if this is a new memo
                ReloadButton: element(reloadButtonName)
            });
        }--%>

        function showEditor(memo, template, isDeterminationTemplate, refId) {

            var refId = refId;
            var reloadButtonName = "<%= RefreshButton.ClientId %>";

            //if (refId === 0) {
            //    return;
            //}
            if (memo === 0 && isDeterminationTemplate == true) {
                reloadButtonName = "<%= NewMemoButton.ClientId %>";
            }

            var url = $_HOSTNAME + "/Secure/Shared/Memos/EditMemo.aspx?id=" + refId + "&template=" + template + "&memo=" + memo + "&mod=" + <%= ModuleType%>;
            showPopup({
                Url: url,
                Width: '666',
                Height: '680',
                Resizable: false,
                ScrollBars: false,
                Center: true,
                Reload: memo === 0, //only reload if this is a new memo
                ReloadButton: element(reloadButtonName)
            });
        }

        //used to display a memo as a pdf
        function displayMemo(url) {

            showPopup({
                'Url': url,
                'Width': '690',
                'Height': '700',
                'Resizable': true,
                'Center': true,
                'Reload': false
            });

        }

        function confirmDelete() {
            return confirm("This will permanently delete this document.  Are you sure you want to proceed?");
        }
    
    </script>

</asp:Content>
