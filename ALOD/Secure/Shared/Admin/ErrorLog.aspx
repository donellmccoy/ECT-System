<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Admin.Secure_Shared_Admin_ErrorLog" Codebehind="ErrorLog.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .leftPadding
        {
            padding-left: 7px;
        }
        
        td.log
        {
            padding: 8px 4px 8px 8px;
	        font-weight: normal;
	        background-color: #FFF;
	        border: 1px solid #C0C0C0;
	        vertical-align: top;
        }
        
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="Server">
    <asp:UpdatePanel ID="updMain" runat="server">
        <ContentTemplate>
            <asp:GridView ID="gvAllErrors" runat="server" AutoGenerateColumns="False" DataKeyNames="logId"
                Width="920px" AllowSorting="True" AllowPaging="True" PageSize="30" DataSourceID="dataAllErrors">
                <Columns>
                    <asp:CommandField ShowSelectButton="True" />
                    <asp:BoundField DataField="errorTime" DataFormatString="{0:MM/dd/yyyy HHmm}" HeaderText="Date"
                        SortExpression="errorTime" HtmlEncode="false">
                        <ItemStyle Width="150px" />
                    </asp:BoundField>
                    <asp:TemplateField HeaderText="Page" SortExpression="page">
                        <ItemTemplate>
                            <asp:Label ID="lblPage" runat="server" Text='<%# Eval("page") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Message" SortExpression="message">
                        <ItemTemplate>
                            <asp:Label ID="lblMessage" runat="server" Text='<%# Eval("message") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="User" SortExpression="username">
                        <ItemTemplate>
                            <asp:Label ID="lnkUser" runat="server" Text='<%# Eval("username") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            &nbsp;<br />
            <asp:ObjectDataSource ID="dataAllErrors" runat="server" SelectMethod="GetErrorLog"
                TypeName="ALOD.Logging.LogManager"></asp:ObjectDataSource>
            <br />
            <asp:Panel ID="Panel1" runat="server">
                <div style="width: 665px; height:800px; overflow:auto;">
                    <table class="ui-dialog ui-widget ui-widget-content ui-corner-all jqmDialog" style="width:600px;">
                        <tr>
                            <td colspan="2" class="ui-dialog-titlebar ui-widget-header ui-corner-all">
                                <span style="float: left;">Detail Error Log</span>
                                <span style="float: right;"><img id="imgClose" alt="Close" 
                                    src="../../../images/close.png" class="align-right" style="cursor: pointer;" /></span>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2"></td>
                        </tr>
                        <tr>
                            <td class="log" style="width:100px;">
                                Appversion:
                            </td>
                            <td class="log">
                                <asp:Label ID="lblAppVersion" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="log" style="width:100px;">
                                Browser:
                            </td>
                            <td class="log">
                                <asp:Label ID="lblBrowser" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="log" style="width:100px;">
                                Message:
                            </td>
                            <td class="log">
                                <asp:Label ID="lblMessage" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="log" style="width:100px;">
                                StackTrace:
                            </td>
                            <td class="log">
                                <asp:Label ID="lblStack" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="log" style="width:100px;">
                                Caller:
                            </td>
                            <td class="log">
                                <asp:Label ID="lblCaller" runat="server" />
                            </td>
                        </tr>
                    </table>
                </div>
            </asp:Panel>
            <ajax:ModalPopupExtender ID="mdlPopup" runat="server" BackgroundCssClass="modalBackground"
                CancelControlID="imgClose" DropShadow="False" PopupControlID="Panel1" TargetControlID="btnInvisible">
            </ajax:ModalPopupExtender>
            <asp:Button ID="btnInvisible" runat="server" Style="display: none" Text="Cancel" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFooter" runat="Server">
</asp:Content>
