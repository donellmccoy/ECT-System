<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SARC_Appeal/SARC_Appeal.master" AutoEventWireup="false" Inherits="ALOD.Web.APSA.SARCDocuments" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" CodeBehind="SARCDocuments.aspx.vb" %>


<%@ MasterType VirtualPath="~/Secure/SARC_Appeal/SARC_Appeal.master" %>
<%@ Import Namespace="ALODWebUtility.Common" %>
<%@ Register Src="../Shared/UserControls/Documents.ascx" TagName="Documents" TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    
    <asp:Button runat="server" ID="RefreshButton" CssClass="hidden" />
    <br />
    <asp:Panel runat="server" ID="pnlForm348R">
        <table class="docHeader">
            <tr>
                <td style="width: 950px;">
                    <asp:Label runat="server" ID="Form348_title" Text='1. AFRC Form 348' />
                </td>
            </tr>
        </table>
        <table class="docDetails">
            <tr class="docRow">
                <td style="text-align: left; width: 950px;">
                    <asp:HyperLink runat="server" ID="ViewDocLink348" NavigateUrl="#">
                        <asp:Image runat="server" ID="Icon" SkinID="iconPdf" CssClass="iconImg" ImageAlign="AbsMiddle" />
                        <asp:Label runat="server" ID="Description" Text='AF Form 348-R' />
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
                <td style="width: 950px;">
                    <asp:Label runat="server" ID="Memorandum_Title" Text='2. Memorandum' />
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
                        <td style="text-align: left; width: 475px;">
                            <%#Server.HtmlEncode(Eval("CreatedDate", "{0:" + DATE_FORMAT + "}"))%>
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
