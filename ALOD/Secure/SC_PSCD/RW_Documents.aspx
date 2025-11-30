<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SC_PSCD/SC_PSCD.Master" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.PSCD.Secure_PSCD_RW_Documents" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" Codebehind="RW_Documents.aspx.vb" %>

<%@ MasterType VirtualPath="~/Secure/SC_PSCD/SC_PSCD.Master" %>
<%@ Import Namespace="ALODWebUtility.Common" %>
<%@ Register Src="../Shared/UserControls/Documents.ascx" TagName="Documents" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    <asp:Button runat="server" ID="RefreshButton" CssClass="hidden" />
    <br />
    <table class="docHeader">
        <tr>
            <td style="width: 650px;">
                <asp:Label runat="server" ID="Memorandum_Title" Text='1 . Determination Memorandum' />
            </td>
            <td colspan="3" style="text-align: right; width: 300px;">
                <asp:Image runat="server" ID="CreateMemo" ImageAlign="AbsMiddle" ImageUrl="~/images/create_document.gif" CssClass="iconUpload" AlternateText="Create New RW Memo" />
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
                        <%#HTMLEncodeNulls(Eval("CreatedDate", "{0:" + DATE_FORMAT + "}"))%>
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

     <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:Documents runat="server" ID="Documents" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">
    <script type="text/javascript">
        function showEditor(memo, template, mod) {
            var refId = getQueryStringValue("refId", 0);

            if (refId === 0) {
                return;
            }

            var url = $_HOSTNAME + "/Secure/Shared/Memos/EditMemo.aspx?id=" + refId + "&template=" + template + "&memo=" + memo + "&mod=" + mod;
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
