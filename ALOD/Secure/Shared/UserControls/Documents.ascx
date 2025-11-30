<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="Documents.ascx.vb" Inherits="ALOD.Web.UserControls.Secure_Shared_UserControls_Documents" %>

<%@ Import Namespace="ALODWebUtility.Common" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>

<asp:Panel ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
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
                    <asp:DropDownList runat="server" ID="DocumentTypeSelect" DataTextField="Description" DataValueField="Id" style="max-width:300px;">
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
    
    <br />

    <asp:Repeater runat="server" ID="CategoryRepeater">
        <SeparatorTemplate>
            <br />
        </SeparatorTemplate>
        <ItemTemplate>
            <div id='parentRow' runat="Server">
                <table class="docHeader">
                    <tr>
                        <td style="width: 600px;">
                            <%#Container.ItemIndex + DocStart %>  .&nbsp;<%#Server.HtmlEncode(Eval("CategoryDescription"))%> </td><td style="width: 196px;">
                            <asp:Label runat="server" ID="lblStatus" />
                        </td>
                        <td style="width: 300px;">
                            <asp:Label runat="server" ID="lblReqd" CssClass="labelRequired" Font-Bold="true" />
                        </td>
                        <td style="width: 50px; text-align: right;">
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
                                <td style="text-align: left; width: 480px;" title="Document title">
                                    <asp:HyperLink runat="server" ID="ViewDocLink" NavigateUrl="#">
                                        <asp:Image runat="server" ID="Icon" ImageUrl='<%# Eval("IconUrl") %>' CssClass="iconImg"
                                            ImageAlign="AbsMiddle" />
                                        
                                        <%#IIf(String.IsNullOrEmpty(DataBinder.Eval(Container.DataItem, "Description")), Server.HtmlEncode(Eval("DocTypeName")) + "_" + Server.HtmlEncode(Eval("UploadedBy")) + "_" + Server.HtmlEncode(Eval("DocDate", "{0:" + DATE_FORMAT + "}")), Server.HtmlDecode(Eval("Description")))%>
                                    </asp:HyperLink>

                                </td>
                                <td style="text-align: Left; width: 100px;" title="Document date">
                                    <%#Server.HtmlEncode(Eval("DocDate", "{0:" + DATE_FORMAT + "}"))%> 

                                </td>
                                <td style="text-align: Left; width: 100px;" title="Upload date">
                                    <%#Server.HtmlEncode(Eval("DateAdded", "{0:" + DATE_FORMAT + "}"))%> 

                                </td>
                                <td style="text-align: left; width: 170px;" title="Uploaded by">
                                    <%#Server.HtmlEncode(Eval("UploadedBy"))%> 

                                </td>
                                <td style="text-align: right; width: 200px;">
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
    
    <br />
</asp:Panel>

<asp:Panel ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">

    <script type="text/javascript">

        var activeDocId = 0;

        function editDocument(docId, category, docDate, description) {

            activeDocId = docId;
       
            setSelectedValue('<%= DocumentTypeSelect.ClientId %>', category);
            element('<%= DocumentDate.ClientId %>').value = docDate;
            element('<%= DocumentDescription.ClientId %>').value = description;

            
            $('#EditDocument').dialog({
                autoOpen: false,
                modal: false,
                resizable: false,
                width: 500,
                height: 250,
                position: 'center',
                buttons: {
                    'Cancel': function() {
                        $(this).dialog('destroy');

                    },
                    'Update': function() {
                        serializeEditor();
                        $(this).dialog('close');
                        element('<%= UpdateDocumentButton.ClientId %>').click();
                    }
                }

            });

            $('#EditDocument').dialog('open');
        }

        function serializeEditor() {

            var values = String(activeDocId) + "|"
                + getSelectedValue('<%= DocumentTypeSelect.ClientId %>') + "|"
                + element('<%= DocumentDate.ClientId %>').value + "|"
                + element('<%= DocumentDescription.ClientId %>').value;

            element('<%= DocumentEditValues.ClientId %>').value = values;
        }
        /* $(document).ready(function () {Deprecated ready Syntax: -- Diamante Lawson 10/12/2023 */

       (function() {


            //prepare the edit document dialog
            $('#EditDocument').dialog({ autoOpen: false });

            //date pickers
            $('.datePickerToToday').datepicker(calendarPick("DocPast", "<%=CalendarImage %>"));
            $('.datePicker').datepicker(calendarPick("DocAll", "<%=CalendarImage %>"));

        });


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
            });
        }

        function ViewDoc(docId, description) {

            var url = $_HOSTNAME + "/Secure/Shared/DocumentViewer.aspx?docId=" + docId + "&modId=" + <% moduleId.ToString() %> + "&refId=" + <% refID.ToString() %> + "&doc=" + description;
            showPopup({
                'Url': url,
                'Width': '690',
                'Height': '700',
                'Resizable': true,
                'Center': true,
                'Reload': false
            });

        }

    </script>

</asp:Panel>