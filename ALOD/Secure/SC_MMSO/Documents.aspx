<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SC_MMSO/SC_MMSO.master" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.MMSO.Secure_sc_mmso_Documents" MaintainScrollPositionOnPostback="true" Codebehind="Documents.aspx.vb" %>

<%@ Import Namespace="ALODWebUtility.Common" %>

<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    <div id="EditDocument" class="dataBlock" title="Edit Document Details" style="display: none;
        margin-bottom: 0px; background-color: #EEE;">
        <table>
            <tr>
                <td class="number">
                    1
                </td>
                <td class="label-small">
                    Category:
                </td>
                <td class="value">
                    <asp:DropDownList runat="server" ID="DocumentTypeSelect" DataTextField="CategoryDescription"
                        DataValueField="DocCatId">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td class="number">
                    2
                </td>
                <td class="label-small">
                    Date:
                </td>
                <td class="value">
                    <asp:TextBox runat="server" ID="DocumentDate" MaxLength="12" Width="100px" CssClass="datePickerToToday" />
                </td>
            </tr>
            <tr>
                <td class="number">
                    3
                </td>
                <td class="label-small">
                    Description:
                </td>
                <td class="value">
                    <asp:TextBox runat="server" ID="DocumentDescription" Width="200px" MaxLength="50" onkeypress="return checkFormat(this,event,&#39;AlphaNumeric&#39;,&#39;`,~,!,@,$,%,*,(,),-,_,+,\&#39;,&quot;,.,,,?,/,\\,|,:, &#39;);" />
                </td>
            </tr>
        </table>
    </div>
    <asp:Button runat="server" ID="UpdateDocumentButton" CssClass="hidden" />
    <asp:TextBox runat="server" ID="DocumentEditValues" CssClass="hidden" />
    <div id="CreateMemo" title="Create Memorandum" style="display: none;">
        Select Memo Template:<br />
        <asp:DropDownList runat="server" ID="TemplateSelect" DataTextField="Title" DataValueField="Id">
        </asp:DropDownList>
    </div>
    <asp:Button runat="server" ID="RefreshButton" CssClass="hidden" />
    <br />
 <%--   <table class="docHeader">
        <tr>
            <td style="width: 596px;">
                1 - AFRC Form 348 / DD Form 261
            </td>
            <td style="width: 196px;">
                &nbsp;
            </td>
            <td style="width: 196px;">
                &nbsp;
            </td>
            <td style="width: 52px; text-align: right;">
                &nbsp;
            </td>
        </tr>
    </table>--%>
<%--    <table class="docDetails" cellpadding="4" cellspacing="0">
        <tr class="docRow">
            <td style="text-align: left; width: 440px;">
                <asp:HyperLink runat="server" ID="ViewDocLink348" NavigateUrl="#">
                    <asp:Image runat="server" ID="Icon" SkinID="iconPdf" CssClass="iconImg" ImageAlign="AbsMiddle" />
                    <asp:Label runat="server" ID="Description" Text='AFRC Form 348' />
                </asp:HyperLink></td><td style="text-align: center; width: 100px;">
                &nbsp; </td><td style="text-align: left; width: 160px;">
                &nbsp; </td><td style="text-align: right; width: 200px;">
                &nbsp; </td></tr><tr class="docRow" runat="server" id="Form261Row">
            <td style="text-align: left; width: 440px;">
                <asp:HyperLink runat="server" ID="ViewDocLink261" NavigateUrl="#">
                    <asp:Image runat="server" ID="Image1" SkinID="iconPdf" CssClass="iconImg" ImageAlign="AbsMiddle" />
                    <asp:Label runat="server" ID="Label1" Text='DD Form 261' />
                </asp:HyperLink></td><td style="text-align: center; width: 100px;">
                &nbsp; </td><td style="text-align: left; width: 160px;">
                &nbsp; </td><td style="text-align: right; width: 200px;">
                &nbsp; </td></tr>
        </table>--%>
<%--        <p>&nbsp;</p>
    <table class="docHeader">
        <tr>
            <td style="width: 596px;">
                1 . Memorandum </td><td style="width: 196px;">
                &nbsp; </td><td style="width: 248px;" colspan="3" style="text-align: right;">
                <asp:Image runat="server" ID="CreateMemo" ImageAlign="AbsMiddle" ImageUrl="~/images/create_document.gif"
                    CssClass="iconUpload" AlternateText="Create New Memo" />
            </td>
        </tr>
    </table>
    <table class="docDetails" cellpadding="4" cellspacing="0">
        <asp:Repeater runat="server" ID="MemoRepeater">
            <ItemTemplate>
                <tr class="docRow">
                    <td style="text-align: left; width: 440px;">
                        <a href="#" onclick='displayMemo(<%#Eval("Id") %>);'>
                            <asp:Image runat="server" ID="Icon" SkinID="iconPdf" CssClass="iconImg" ImageAlign="AbsMiddle" />
                            <%#Server.HtmlEncode(Eval("Template.Title"))%> </a></td><td style="text-align: center; width: 100px;">
                        <%#Server.HtmlEncode(Eval("CreatedDate", "{0:" + DATE_FORMAT + "}"))%> </td><td style="text-align: left; width: 320px;">
                        &nbsp; </td><td style="text-align: right; width: 20px;">
                        &nbsp; <asp:Image runat="server" ID="EditMemo" ImageAlign="AbsMiddle" AlternateText="Edit Document"
                            SkinID="imgEditSmall" CssClass="pointer" />
                    </td>
                    <td style="text-align: right; width: 20px;">
                        &nbsp; <asp:ImageButton runat="server" SkinID="buttonDelete" ID="DeleteMemo" CommandArgument='<%# Eval("Id") %>'
                            CommandName="DeleteMemo" ImageAlign="AbsMiddle" AlternateText="Delete Memo" />
                        <ajax:ConfirmButtonExtender runat="server" ID="confirmDelete" ConfirmText="Are you sure you want to delete this memo?"
                            TargetControlID="DeleteMemo" />
                        <asp:Image runat="server" ID="LockedDocument" SkinID="imgLocked" AlternateText="Document is Read-Only"
                            Visible="false" />
                    </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>
--%>    
<p> &nbsp;</p>
    <asp:Repeater runat="server" ID="CategoryRepeater">
        <SeparatorTemplate>
            <p>
                &nbsp;</p></SeparatorTemplate><ItemTemplate>
            <div id='parentRow' runat="Server">
                <table class="docHeader">
                    <tr>
                        <td style="width: 596px;">
                            <%#Container.ItemIndex + 1%> .&nbsp;<%#Server.HtmlEncode(Eval("CategoryDescription"))%></td><td style="width: 196px;">
                            <asp:Label runat="server" ID="lblStatus" />
                        </td>
                        <td style="width: 196px;">
                            <asp:Label runat="server" ID="lblReqd" CssClass="labelRequired" Font-Bold="true" />
                        </td>
                        <td style="width: 52px; text-align: right;">
                            <asp:Image runat="server" ID="UploadImage" ImageAlign="AbsMiddle" ImageUrl="~/images/upload.gif"
                                CssClass="iconUpload" />
                        </td>
                    </tr>
                </table>
                <!--Start of Parent rows Table -->
                
                    <!--Start of ChildRows Table -->
                    <asp:Repeater ID="ChildDocRepeater" runat="server" OnItemDataBound="ChildDataBound"
                        OnItemCommand="Document_ItemCommand">
                        <ItemTemplate>
                            <table class="docDetails">
                            <tr class="docRow">
                                <td style="text-align: left; width: 440px;">
                                    <asp:HyperLink runat="server" ID="ViewDocLink" NavigateUrl="#">
                                        <asp:Image runat="server" ID="Icon" ImageUrl='<%# Eval("IconUrl") %>' CssClass="iconImg"
                                            ImageAlign="AbsMiddle" />
                                        
                                        <%# IIf(String.IsNullOrEmpty(DataBinder.Eval(Container.DataItem, "Description")), Server.HtmlEncode(Eval("DocTypeName")) + "_" + Server.HtmlEncode(Eval("UploadedBy")) + "_" + Server.HtmlEncode(Eval("DocDate", "{0:" + DATE_FORMAT + "}")), Server.HtmlDecode(Eval("Description")))%>
                                    </asp:HyperLink></td><td style="text-align: center; width: 100px;">
                                    <%#Server.HtmlEncode(Eval("DocDate", "{0:" + DATE_FORMAT + "}"))%> </td><td style="text-align: left; width: 160px;">
                                    <%#Server.HtmlEncode(Eval("UploadedBy"))%> </td><td style="text-align: right; width: 200px;">
                                    <asp:Image runat="server" ID="EditDocument" ImageAlign="AbsMiddle" AlternateText="Edit Document"
                                        SkinID="imgEditSmall" CssClass="pointer" />
                                    &nbsp; <asp:ImageButton runat="server" ID="DeleteDocument" ImageAlign="AbsMiddle" AlternateText="Delete Document"
                                        SkinID="buttonDelete" CssClass="pointer" CommandName="DeleteDocument" CommandArgument='<%# Eval("Id").ToString() + "|" + Eval("Description") %>' />
                                    <ajax:ConfirmButtonExtender runat="server" ID="ConfirmDelete" TargetControlID="DeleteDocument"
                                        ConfirmText="Are you sure you want to delete this document?" />
                                    <asp:Image runat="server" ID="LockedDocument" SkinID="imgLocked" AlternateText="Document is Read-Only"
                                        Visible="false" />
                                </td>
                            </tr></table>
                        </ItemTemplate>
                    </asp:Repeater>
                
                <!--End of ChildRows Table -->
            </div>
        </ItemTemplate>
    </asp:Repeater>
    <p>
        &nbsp;</p></asp:Content><asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">

    <script type="text/javascript">

        var activeDocId = 0;

        function editDocument(docId, category, docDate, description) {

            activeDocId = docId;
       
            setSelectedValue('<%= DocumentTypeSelect.ClientId %>', category);
            element('<%= DocumentDate.ClientId %>').value = docDate;
            element('<%= DocumentDescription.ClientId %>').value = description;

            $('#EditDocument').dialog('open');
        }

        function serializeEditor() {

            var values = String(activeDocId) + "|"
                + getSelectedValue('<%= DocumentTypeSelect.ClientId %>') + "|"
                + element('<%= DocumentDate.ClientId %>').value + "|"
                + element('<%= DocumentDescription.ClientId %>').value;

            element('<%= DocumentEditValues.ClientId %>').value = values;
        }
        
        $(function () {

            //prepare the edit document dialog
            $('#EditDocument').dialog({
                autoOpen: false,
                modal: false,
                resizable: false,
                width: 400,
                height: 230,
                position: 'center',
                buttons: {
                    'Cancel': function() {
                        $(this).dialog('close');

                    },
                    'Update': function() {
                        serializeEditor();
                        $(this).dialog('close');
                        element('<%= UpdateDocumentButton.ClientId %>').click();
                    }
                }

            });

            //date pickers
            $('.datePickerToToday').datepicker(calendarPick("DocPast", "<%=CalendarImage %>"));
            $('.datePicker').datepicker(calendarPick("DocAll", "<%=CalendarImage %>"));

        });



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

        //used to display a memo as a pdf
       <%-- function displayMemo(memo) {

            var refId = '<%= SpecCase.id %>';

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

        }--%>

        function displayMemo(memo) {
            var refId = '<%= SpecCase.id %>';
    if (refId === 0) {
        return;
    }
    
            var url = $_HOSTNAME + "/Secure/Shared/Memos/ViewPdf.aspx?id=" + refId + "&memo=" + memo + "&mod=" + ModuleType; // Remove the 

            showPopup({
                'Url': url,
                'Width': '690',
                'Height': '700',
                'Resizable': true,
                'Center': true,
                'Reload': false
            });
        }


        //Opens up the  document URL  in a dailog window
        function uploadDoc(url) {
            showPopup({
                'Url': url,
                'Width': 500,
                'Height': 300,
                'Center': true,
                'Reload': true,
                'Resizable': true,
                'ScrollBars': true,
                ReloadButton: element('<%= RefreshButton.ClientId %>')
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

        function confirmDelete() {
            return confirm("This will permanently delete this document.  Are you sure you want to proceed?");
        }
    
    </script>

</asp:Content>
