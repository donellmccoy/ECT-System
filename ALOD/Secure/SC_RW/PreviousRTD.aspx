<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SC_RW/SC_RW.master" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.RW.Secure_sc_rw_PreviousRTD" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" Codebehind="PreviousRTD.aspx.vb" %>

<%@ Import Namespace="ALODWebUtility.Common" %>

<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    <asp:Button runat="server" ID="RefreshButton" CssClass="hidden" />
    <asp:Panel runat="server" ID="pnlAssociatedDocumentation">
        <asp:Panel runat="server" ID="pnlMemos" Visible="false" >
            <table class="docHeader">
                <tr>
                    <td style="width: 596px;">
                        1 . Determination Memorandum
                    </td>
                    <td style="width: 196px;">
                        &nbsp;
                    </td>
                    <td colspan="3" style="text-align: right; width: 248px;">
                        &nbsp;
                    </td>
                </tr>
            </table>
            <asp:Repeater runat="server" ID="MemoRepeater">
                <ItemTemplate>
                    <table class="docDetails">
                        <tr class="docRow">
                            <td style="text-align: left; width: 440px;">
                                <a href="#" onclick='displayMemo(<%#Eval("Id") %>);'>
                                    <asp:Image runat="server" ID="Icon" SkinID="iconPdf" CssClass="iconImg" ImageAlign="AbsMiddle" />
                                    <%#HTMLEncodeNulls(Eval("Template.Title"))%>
                                </a>
                            </td>
                            <td style="text-align: center; width: 100px;">
                                <%#HTMLEncodeNulls(Eval("CreatedDate", "{0:" + DATE_FORMAT + "}"))%>
                            </td>
                            <td style="text-align: left; width: 320px;">
                                &nbsp;
                            </td>
                            <td style="text-align: right; width: 20px;">
                                &nbsp;
                            </td>
                            <td style="text-align: right; width: 20px;">
                                &nbsp;
                            </td>
                        </tr>
                    </table>
                </ItemTemplate>
            </asp:Repeater>
            <p>
                &nbsp;
            </p>
        </asp:Panel>
        <asp:Repeater runat="server" ID="CategoryRepeater">
            <SeparatorTemplate>
                <p>
                    &nbsp;
                </p>
            </SeparatorTemplate>
             <ItemTemplate>
                <div id='parentRow' runat="Server">
                    <table class="docHeader">
                        <tr>
                            <td style="width: 596px;">
                                <%#Container.ItemIndex + IndexIncrement%> .&nbsp;<%#Server.HtmlEncode(Eval("CategoryDescription"))%>
                            </td>
                            <td style="width: 196px;">
                                <asp:Label runat="server" ID="lblStatus" />
                            </td>
                            <td style="width: 196px;">
                                <asp:Label runat="server" ID="lblReqd" CssClass="labelRequired" Font-Bold="true" />
                            </td>
                            <td style="width: 52px; text-align: right;">
                                &nbsp;
                            </td>
                        </tr>
                    </table>
                    <!--Start of Parent rows Table -->
                
                        <!--Start of ChildRows Table -->
                        <asp:Repeater ID="ChildDocRepeater" runat="server" OnItemDataBound="ChildDataBound">
                            <ItemTemplate>
                                <table class="docDetails">
                                    <tr class="docRow">
                                        <td style="text-align: left; width: 440px;">
                                            <asp:HyperLink runat="server" ID="ViewDocLink" NavigateUrl="#">
                                                <asp:Image runat="server" ID="Icon" ImageUrl='<%# Eval("IconUrl") %>' CssClass="iconImg"
                                                    ImageAlign="AbsMiddle" />
                                        
                                                <%# IIf(String.IsNullOrEmpty(DataBinder.Eval(Container.DataItem, "Description")), Server.HtmlEncode(Eval("DocTypeName")) + "_" + Server.HtmlEncode(Eval("UploadedBy")) + "_" + Server.HtmlEncode(Eval("DocDate", "{0:" + DATE_FORMAT + "}")), Server.HtmlDecode(Eval("Description")))%>
                                            </asp:HyperLink></td>
                                         <td style="text-align: center; width: 100px;">
                                            <%#Server.HtmlEncode(Eval("DocDate", "{0:" + DATE_FORMAT + "}"))%> </td>
                                         <td style="text-align: left; width: 160px;">
                                            <%#Server.HtmlEncode(Eval("UploadedBy"))%> </td>
                                         <td style="text-align: right; width: 200px;">
                                             &nbsp;
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                        </asp:Repeater>
               
                    <!--End of ChildRows Table -->
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </asp:Panel>
    <asp:Label runat="server" ID="lblAdminSpecialCase" Text="Admin Special Case" Visible="false" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">
    <script type="text/javascript">
        $(document).ready(function() {

        });

        function displayMemo(memo) {
            var refId = '<%= AssociatedSpecCaseId %>';

            if (refId === 0) {
                return;
            }

            var url = $_HOSTNAME + "/Secure/Shared/Memos/ViewPdf.aspx?id=" + refId + "&memo=" + memo + "&mod=" + <%=ModuleType %>;

            showPopup({
                'Url': url,
                'Width': '690',
                'Height': '700',
                'Resizable': true,
                'Center': true,
                'Reload': false
            });
        }

        function viewDoc(url) {
            showPopup({
                'Url': url,
                'Width': 642,
                'Height': 668,
                'Center': true,
                'Resizable': true,
                'ScrollBars': true
            });
        }
    </script>
</asp:Content>
