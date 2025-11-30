<%@ Page Title="" Language="vb" MasterPageFile="~/Secure/SC_PSCD/SC_PSCD.Master" AutoEventWireup="false" CodeBehind="Documents.aspx.vb" Inherits="ALOD.Web.Special_Case.PSCD.Secure_PSCD_Documents" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" %>

<%@ MasterType VirtualPath="~/Secure/SC_PSCD/SC_PSCD.Master" %>
<%@ Import Namespace="ALODWebUtility.Common" %>
<%@ Register Src="../Shared/UserControls/Documents.ascx" TagName="Documents" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="server">
    <asp:Button runat="server" ID="RefreshButton" CssClass="hidden" />
    <br />
    <asp:Panel runat="server" ID="pnlMemos" >
       <table class="docHeader">
        <tr>
            <td style="width: 950px;">
                <asp:Label runat="server" ID="Memorandum_Title" Text='1. PSC-D Findings Memo' />
                <td style="text-align: right; width: 950px;">
                <%--<asp:ImageButton runat="server" ID="MemorandumLink" ImageUrl="~/images/upload.gif">
                </asp:ImageButton>--%>
            </td>
            </td>
            <td style="width: 300px; text-align: right;" colspan="3">
                <asp:Image runat="server" ID="ViewPSCDMemo" ImageAlign="AbsMiddle" ImageUrl="~/images/create_document.gif"
                    CssClass="iconUpload" AlternateText="View PSC-D Memo" />
            </td>
        </tr>
    </table>

         <asp:Repeater runat="server" ID="MemoRepeater">
            <ItemTemplate>
                <table class="docDetails">
                    <tr class="docRow">
                        <td style="text-align:left; width:475px;">
                            <asp:HyperLink runat="server" ID="ViewMemoLink" NavigateUrl="#">
                                <asp:Image runat="server" ID="Icon" SkinID="iconPdf" CssClass="iconImg" ImageAlign="AbsMiddle" />
                                <asp:Label runat="server" ID="memoTitle" Text='<%#Server.HtmlEncode(Eval("Template.Title"))%>' />
                            </asp:HyperLink></td><td style="text-align:left; width:275px;">
                            <%#HTMLEncodeNulls(Eval("CreatedDate", "{0:" + DATE_FORMAT + "}"))%>
                        </td>
                        <td style="text-align:right; width:200px;">
                            <asp:Image runat="server" ID="EditMemo" ImageAlign="AbsMiddle" AlternateText="Edit Document" SkinID="imgEditSmall" CssClass="pointer" />
                            &nbsp; <asp:ImageButton runat="server" SkinID="buttonDelete" ID="DeleteMemo" CommandArgument='<%# Eval("Id") %>'  CommandName="DeleteMemo" ImageAlign="AbsMiddle" AlternateText="DeleteMemo" />
                            <ajax:ConfirmButtonExtender runat="server" ID="confirmDelete" ConfirmText="Are you sure you want to delete this memo?" TargetControlID="DeleteMemo" />
                            <asp:Image runat="server" ID="LockedDocument" SkinID="imgLocked" AlternateText="Document is Read-Only" Visible="false" />
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
        </asp:Repeater>
    </asp:Panel>

        <br />
   

<asp:Panel runat="server" ID="pnlWorksheets" >
       <table class="docHeader">
        <tr>
            <td style="width: 950px;">
                <asp:Label runat="server" ID="Worksheet_Title" Text='2. PSC-D Case Commentary Worksheet' />
                <td style="text-align: right; width: 950px;">
            </td>
            </td>
            <td style="width: 300px; text-align: right;" colspan="3">
                <asp:Image runat="server" ID="ViewPSCDWorksheet" ImageAlign="AbsMiddle" ImageUrl="~/images/create_document.gif"
                    CssClass="iconUpload" AlternateText="View PSC-D Commentary Worksheet" />
            </td>
        </tr>
    </table>

         <asp:Repeater runat="server" ID="CommentaryRepeater">
            <ItemTemplate>
                <table class="docDetails">
                    <tr class="docRow">
                        <td style="text-align:left; width:475px;">
                            <asp:HyperLink runat="server" ID="CommentaryLink" NavigateUrl="#">
                                <asp:Image runat="server" ID="Icon" SkinID="iconPdf" CssClass="iconImg" ImageAlign="AbsMiddle" />
                                <asp:Label runat="server" ID="worksheetTitle" Text='<%#Server.HtmlEncode(Eval("Template.Title"))%>' />
                            </asp:HyperLink></td><td style="text-align:left; width:275px;">
                            <%#HTMLEncodeNulls(Eval("CreatedDate", "{0:" + DATE_FORMAT + "}"))%>
                        </td>
                        <td style="text-align:right; width:200px;">
                            <asp:Image runat="server" ID="EditWorksheet" ImageAlign="AbsMiddle" AlternateText="Edit Document" SkinID="imgEditSmall" CssClass="pointer" />
                            &nbsp; <asp:ImageButton runat="server" SkinID="buttonDelete" ID="DeleteWorksheet" CommandArgument='<%# Eval("Id") %>'  CommandName="DeleteWorksheet" ImageAlign="AbsMiddle" AlternateText="DeleteWorksheet" />
                            <ajax:ConfirmButtonExtender runat="server" ID="confirmDelete" ConfirmText="Are you sure you want to delete this worksheet?" TargetControlID="DeleteWorksheet" />
                            <asp:Image runat="server" ID="LockedDocument" SkinID="imgLocked" AlternateText="Document is Read-Only" Visible="false" />
                        </td>
                    </tr>
                </table>
            </ItemTemplate>
        </asp:Repeater>
    </asp:Panel>
      <%-- <table class="docHeader">
        <tr>
            <td style="width: 950px;">
                <asp:Label runat="server" ID="Pertinent_Title" Text='3. Pertinent Medical Records' />
                <td style="text-align: right; width: 950px;">
                <asp:ImageButton runat="server" ID="PertinentLink" ImageUrl="~/images/upload.gif" >
                </asp:ImageButton>
            </td>
            </td>
        </tr>
   </table>
        <br />
    <table class="docHeader">
        <tr>
            <td style="width: 950px;">
                <asp:Label runat="server" ID="Status_Title" Text='4. Status Verification Documents' />
                <td style="text-align: right; width: 950px;">
                <asp:ImageButton runat="server" ID="StatusLink" ImageUrl="~/images/upload.gif">
                 </asp:ImageButton>
            </td>
            </td>
        </tr>
    </table>--%>
   

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <uc1:Documents runat="server" ID="Documents" />
        </ContentTemplate>
    </asp:UpdatePanel>


    

<style>
    div.b {
  position: absolute;
  right: 0;
  width: 100px;
  height: 120px;
  border: 3px solid blue;
} 
</style>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="server">
    <script type="text/javascript">

        // Used to display the memo editor
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
            return confirm("This will permanently delete this document. Are you sure you want to proceed?");
        }
    </script>

    
</asp:Content>
