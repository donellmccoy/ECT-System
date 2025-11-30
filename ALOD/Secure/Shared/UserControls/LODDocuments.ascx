<%@ Control Language="VB" AutoEventWireup="false" Inherits="ALOD.Web.UserControls.LODDocuments" Codebehind="LODDocuments.ascx.vb" %>


<%@ Import Namespace="ALODWebUtility.Common" %>
<%@ Register Src="Documents.ascx" TagName="Documents" TagPrefix="uc1" %>

<asp:Panel ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    <div id="AdminDocs" runat="server" visible="false">
    &nbsp;&nbsp;Admin LODs have no LOD Documentation.
    </div>
    <div id="NonAdminDocs" runat="server">

        <table class="docHeader">
            <tr>
                <td style="width: 950px;">
                    <asp:Label runat="server" ID="Form348_title" Text='1 - AFRC Form 348 / DD Form 261' />
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
            <tr id="RowAppeal" class="docRow" visible="false" runat="server">
                <td style="text-align: left; width: 950px;">
                    <asp:HyperLink runat="server" ID="ViewAppealLink" NavigateUrl="#">
                        <asp:Image runat="server" ID="IconA" SkinID="iconPdf" CssClass="iconImg" ImageAlign="AbsMiddle" />
                        <asp:Label runat="server" ID="AppealDescription" Text='' />
                    </asp:HyperLink>
                    <asp:Label runat="server" ID="AppealMemoError" Visible="false" CssClass="labelRequired" Text="No Appeal Memo found please contact help desk" />
                </td>
            </tr>
            <tr id="RowSARC" class="docRow" visible="false" runat="server">
                <td style="text-align: left; width: 950px;">
                    <asp:HyperLink runat="server" ID="ViewSARCAppealLink" NavigateUrl="#">
                        <asp:Image runat="server" ID="IconB" SkinID="iconPdf" CssClass="iconImg" ImageAlign="AbsMiddle" />
                        <asp:Label runat="server" ID="SARCAppealDescription" Text='' />
                    </asp:HyperLink>
                    <asp:Label runat="server" ID="SARCAppealMemoError" Visible="false" CssClass="labelRequired" Text="No Appeal Memo found please contact help desk" />
                </td>
            </tr>
        </table>
        <br />
        <table class="docHeader">
            <tr>
                <td style="width: 950px;">
                    <asp:Label runat="server" ID="Memorandum_Title" Text='2. Memorandum' />
                </td>
            </tr>
        </table>
    </div>
    <table class="docDetails">
        <asp:Repeater runat="server" ID="MemoRepeater">
            <ItemTemplate>
                <tr class="docRow">
                    <td style="text-align: left; width: 480px;">

                        <asp:HyperLink runat="server" ID="ViewMemoLink" NavigateUrl="#">
                            <asp:Image runat="server" ID="Icon" SkinID="iconPdf" CssClass="iconImg" ImageAlign="AbsMiddle" />
                            <asp:Label runat="server" ID="memoTitle" Text='<%#Server.HtmlEncode(Eval("Template.Title"))%>' />
                        </asp:HyperLink>

                    </td>
                    <td style="text-align: left; width: 470px;">
                        <%#Server.HtmlEncode(Eval("CreatedDate", "{0:" + DATE_FORMAT + "}"))%> 
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>
    

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" ChildrenAsTriggers="true">
        <ContentTemplate>
            <uc1:Documents  runat="server" ID="Documents" />
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Panel>

    <script type="text/javascript">

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
