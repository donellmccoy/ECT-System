<%@ Control Language="VB" AutoEventWireup="false" Inherits="ALOD.Web.UserControls.Secure_Shared_UserControls_CaseComments" Codebehind="CaseComments.ascx.vb"  %>

<%@ Import Namespace="ALODWebUtility.Common" %>

<asp:Panel ID="Comments" runat="server">
    <style type="text/css">
        .CreatedByLabel 
        {
            margin-right: 5px;
        }
    </style>

    <script type="text/javascript">


        //When Add Unit Comment
        /// <summary>
        /// Summary for toAddUnitComment Function:
        /// Initialize the Modal Dialog (options), then open the Modal Dialog (when the board
        /// member click the checkbox).
        /// </summary>
        function toAddUnitComment() {

            $('#myDialogAdd').dialog({
                autoOpen: false,
                modal: true
            });

            $('#myDialogAdd').dialog('option', 'width', 740);
            $('#myDialogAdd').dialog('option', 'height', 280);
            $('#myDialogAdd').dialog('option', 'title', "Add Comment");
            $('#myDialogAdd').dialog('option', 'resizeable', false);

            $('#myDialogAdd').dialog('option', 'buttons', {
                'Close': function () { $(this).dialog('destroy'); },
                'Ok': function () { okAddUnitComment(); $(this).dialog('destroy'); }
            });

            $('#myDialogAdd').dialog('open');
        }

        /// <summary>
        /// Summary for okAddUnitComment Function:
        /// Validate the user add some text on the comment box.
        /// If the user add comments, it will call the server side script to insert the comment,
        /// otherwise, will alert the user to add some text(comments).
        /// </summary>
        function okAddUnitComment() {
            //Passing value to Original Comments Box : ModalCommentsBox : CommentsBox

            //var valueToValidate = $.trim($('#myDialogAdd ModalCommentsBoxAdd').text());
            var valueToValidate = $.trim(document.getElementById('<%=ModalCommentsBoxAdd.ClientID %>').value);
            //alert("value: " + valueToValidate)
	        if (valueToValidate === '') {
		        alert('Unit Comment cannot be empty!');
		        return false;
	        }
	        else {
		        $('#<%= CommentsBox.ClientId %>').val(valueToValidate);
		        //Now the call to the ASPServer Button
		        $('#<%= SaveButton.ClientID %>').click();
		
            }
            return null;
        }

        //When Edit Unit Comment
        /// <summary>
        /// Summary for toEditUnitComment Function:
        /// It will look for specific elements on the row (gridview), and will take the Comments (Label)
        /// and write the Comments Box on the Modal Dialog.
        /// Initialize the Modal Dialog (options), then open the Modal Dialog (when the board
        /// member click the edit button).
        /// </summary>
        /// <param name="editButtonObj">Is the Edit button that user clicked.</param>
        function toEditUnitComment(editButtonObj) {
            //ASP Edit Button
            var editASPButton = $('#' + editButtonObj.id).next('.hidden')[0];
            var mainDiv = $('#' + editButtonObj.id).closest('.dataBlock')[0];
            var commentsLabel = $('#' + mainDiv.id).find('.labelToSearch')[0];

            $("#myDialogEdit").dialog({
                autoOpen: false,
                modal: true
            });

            $('#myDialogEdit').dialog('option', 'width', 740);
            $('#myDialogEdit').dialog('option', 'height', 280);
            $('#myDialogEdit').dialog('option', 'title', "Edit Comment");
            $('#myDialogEdit').dialog('option', 'resizeable', false);

            $('#myDialogEdit').dialog('option', 'buttons', {
                'Close': function () { $(this).dialog('destroy'); },
                'Ok': function () { okEditUnitComment(editASPButton); $(this).dialog('destroy'); }
            });

            $('#myDialogEdit #ModalCommentsBoxEdit').val(commentsLabel.innerHTML.replace(/<br>/g, '\n'));

            $('#myDialogEdit').dialog('open');
        }

        /// <summary>
        /// Summary for okEditUnitComment Function:
        /// Validate the comment box has some text.
        /// If the comments box has some text, it will call the server side script to edit the comment,
        /// otherwise, will alert the user to add some text(comments).
        /// </summary>
        /// <param name="editASPButton">Is the ASPServerControl Edit button.</param>		
        function okEditUnitComment(editASPButton) {
	        //Passing value to Original Comments Box : ModalCommentsBox : CommentsBox
            var valueToValidate = $.trim(document.getElementById('<%=ModalCommentsBoxEdit.ClientID %>').value);
            //alert("value: " + valueToValidate)
	        if (valueToValidate == '') {
		        alert('Unit Comment cannot be empty!');
		        return false;
	        }
	        else {
		        $('#<%= CommentsBox.ClientId %>').val(valueToValidate);
		        //Now the call to the ASPServer Button
                $('#' + editASPButton.id).trigger(click);
	        }
        }
        
        $(function () {
            $("#<%= CommentsBox.ClientId %>").val("");
            $('#myDialogAdd').dialog({ autoOpen: false });
            $('#myDialogEdit').dialog({ autoOpen: false });
        });


    </script>
    <div class="formHeader">
        <asp:Label ID="lblCommentType" runat="server" />
    </div>
    <table style="width:100%;">
        <tr>
            <td class="align-left">
                <asp:Button ID="AddUnitCommentButton" runat="server" Text="Add Comment" OnClientClick="toAddUnitComment();;return false;" autopostback="false"/>
            </td>
            <td class="align-right">
                <asp:CheckBox ID="SortCheckBox" runat="server" Text="Newest On Top" AutoPostBack="true">
                </asp:CheckBox>
            </td>
        </tr>
    </table>
    <br />
    <div runat="server" id="divLessonsRepeater" visible="false" class="dataBlock">
        <div class="dataBlock-center">
            <asp:Label ID="LeasonsLearnedComment" runat="server" Text="Lessons Learned Comments"></asp:Label>
        </div>
        <div class="dataBlock-body" runat="server">
            <asp:Repeater runat="server" ID="LessonsLearnedRepeater">
                <HeaderTemplate>
                </HeaderTemplate>
                <ItemTemplate>
                    <div runat="server" class="dataBlock" id="mainDiv">
                        <div class="dataBlock-header">
                            <table style="width:885px;">
                                <tr>
                                    <td class="align-left">
                                        <asp:Label ID="dateLabel" runat="server" />
                                    </td>
                                    <td class="align-right">
                                        <asp:Label ID="createdByLabel" runat="server" CssClass="CreatedByLabel" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div class="dataBlock-body">
                            <asp:Label ID="commentLabel" runat="server" CssClass="labelToSearch" />
                            <br />
                        
                            <asp:Panel ID="pnlDelete" runat="server" Visible="false">
                                <br />
                                <asp:Button ID="jQueryEditButton" runat="server" Text="Edit" Visible="True" AutoPostBack="False" OnClientClick='toEditUnitComment(this);return false;' />
                                <asp:Button ID="EditButton" runat="server" CssClass="hidden" Visible="True" AutoPostBack="True" CommandName="Edit" CommandArgument='<%# Eval("Id") %>' />
                                <asp:Button ID="DeleteButton" Text="Delete" runat="server" Visible="True" AutoPostBack="True" CommandName="Delete" CommandArgument='<%# Eval("Id") %>' OnClientClick="javascript:if(!confirm('Delete this Comment?'))return false;" />
                                <br />
                            </asp:Panel>

                        </div>
                    
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                </FooterTemplate>
            </asp:Repeater>
        </div>
    </div>
    <div runat="server" id="divsaRepeater" visible="false" class="dataBlock">
        <div class="dataBlock-center">
            <asp:Label ID="SysAdminComment" runat="server" Text="System Admin Comments"></asp:Label>
        </div>
        <div class="dataBlock-body" runat="server">
            <asp:Repeater runat="server" ID="SAcommentRepeater">
                <HeaderTemplate>
                </HeaderTemplate>
                <ItemTemplate>
                    <div runat="server" class="dataBlock" id="mainDiv">
                        <div class="dataBlock-header">
                            <table style="width:885px;">
                                <tr>
                                    <td class="align-left">
                                        <asp:Label ID="dateLabel" runat="server" />
                                    </td>
                                    <td class="align-right">
                                        <asp:Label ID="createdByLabel" runat="server" CssClass="CreatedByLabel" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div class="dataBlock-body">
                            <asp:Label ID="commentLabel" runat="server" CssClass="labelToSearch" />
                            <br />
                        
                            <asp:Panel ID="pnlDelete" runat="server" Visible="false">
                                <br />
                                <asp:Button ID="jQueryEditButton" runat="server" Text="Edit" Visible="True" AutoPostBack="False" OnClientClick='toEditUnitComment(this);return false;' />
                                <asp:Button ID="EditButton" runat="server" CssClass="hidden" Visible="True" AutoPostBack="True" CommandName="Edit" CommandArgument='<%# Eval("Id") %>' />
                                <asp:Button ID="DeleteButton" Text="Delete" runat="server" Visible="True" AutoPostBack="True" CommandName="Delete" CommandArgument='<%# Eval("Id") %>' OnClientClick="javascript:if(!confirm('Delete this Comment?'))return false;" />
                                <br />
                            </asp:Panel>

                        </div>
                    
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                </FooterTemplate>
            </asp:Repeater>
        </div>
    </div>
    <div runat="server" id="divBoardRepeater" visible="false" class="dataBlock">
        <div class="dataBlock-center">
            <asp:Label ID="BoardComment" runat="server" Text="Board Comments"></asp:Label>
        </div>
        <div class="dataBlock-body" runat="server">
            <asp:Repeater runat="server" ID="BoardcommentRepeater" >
                <HeaderTemplate>
                </HeaderTemplate>
                <ItemTemplate>
                    <div runat="server" class="dataBlock" id="mainDiv">
                        <div class="dataBlock-header">
                            <table style="width:885px;">
                                <tr>
                                    <td class="align-left">
                                        <asp:Label ID="dateLabel" runat="server" />
                                    </td>
                                    <td class="align-right">
                                        <asp:Label ID="createdByLabel" runat="server" CssClass="CreatedByLabel" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <br />
                        <div class="dataBlock-body">
                            <asp:Label ID="commentLabel" runat="server" CssClass="labelToSearch" />
                            <br />
                        
                            <asp:Panel ID="pnlDelete" runat="server" Visible="false">
                                <br />
                                <asp:Button ID="jQueryEditButton" runat="server" Text="Edit" Visible="True" AutoPostBack="False" OnClientClick='toEditUnitComment(this);return false;' />
                                <asp:Button ID="EditButton" runat="server" CssClass="hidden" Visible="True" AutoPostBack="True" CommandName="Edit" CommandArgument='<%# Eval("Id") %>' />
                                <asp:Button ID="DeleteButton" Text="Delete" runat="server" Visible="True" AutoPostBack="True" CommandName="Delete" CommandArgument='<%# Eval("Id") %>' OnClientClick="javascript:if(!confirm('Delete this Comment?'))return false;" />
                                <br />
                            </asp:Panel>

                        </div>
                    
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                </FooterTemplate>
            </asp:Repeater>
           </div>
    </div>
    <div runat="server" id="divUnitRepeater" visible="false" class="dataBlock">
        <div class="dataBlock-center">
            <asp:Label ID="UnitComment" runat="server" Text="Unit Comments"></asp:Label>
        </div>
        <div class="dataBlock-body" runat="server">
            <asp:Repeater runat="server" ID="UnitcommentRepeater">
                <HeaderTemplate>
                </HeaderTemplate>
                <ItemTemplate>
                    <div runat="server" class="dataBlock" id="mainDiv">
                        <div class="dataBlock-header">
                            <table style="width:885px;">
                                <tr>
                                    <td class="align-left">
                                        <asp:Label ID="dateLabel" runat="server" />
                                    </td>
                                    <td class="align-right" >
                                        <asp:Label ID="createdByLabel" runat="server" CssClass="CreatedByLabel" />
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <br />
                        <div class="dataBlock-body" runat="server">
                            <asp:Label ID="commentLabel" runat="server" CssClass="labelToSearch" />
                            <br />

                            <asp:Panel ID="pnlDelete" runat="server" Visible="false">
                                <br />
                                <asp:Button ID="jQueryEditButton" runat="server" Text="Edit" Visible="True" AutoPostBack="False" OnClientClick='toEditUnitComment(this);return false;' />
                                <asp:Button ID="EditButton" runat="server" CssClass="hidden" Visible="True" AutoPostBack="True" CommandName="Edit" CommandArgument='<%# Eval("Id") %>' ></asp:Button>
                                <asp:Button ID="DeleteButton" Text="Delete" runat="server" Visible="True" AutoPostBack="True" CommandName="Delete" CommandArgument='<%# Eval("Id") %>' OnClientClick="javascript:if(!confirm('Delete this Comment?'))return false;" />
                                 <br />
                            </asp:Panel>
                        </div>

                    </div>
                </ItemTemplate>
                <FooterTemplate>
                </FooterTemplate>
            </asp:Repeater>
        </div>
    </div>
    <div id="myDialogs" >
        <div id="myDialogEdit" class="hidden">
            <%--<textarea id="ModalCommentsBoxEdit" style="width: 100%; height: 100%;" cols="20" name="ModalCommentsBoxEdit" rows="2" onkeypress="return checkFormat(this,event,&#39;AlphaNumeric&#39;,&#39;`,~,!,@,$,%,*,(,),-,_,+,\&#39;,&quot;,.,,,?,/,\\,|,:, &#39;);" style="height: 130px; width: 540px;" ></textarea>--%>
            <asp:TextBox ID="ModalCommentsBoxEdit" Columns="20" Rows="2" ClientIDMode="Static" style="height: 130px; width: 700px;" name="ModalCommentsBoxEdit" TextMode="MultiLine" runat="server" onkeypress="return checkFormat(this,event,&#39;AlphaNumeric&#39;,&#39;`,~,!,@,$,%,*,(,),-,_,+,\&#39;,&quot;,.,,,?,/,\\,|,:, &#39;);" ></asp:TextBox>
        </div>

        <div id="myDialogAdd" class="hidden">
            <%--<textarea id="ModalCommentsBoxAdd" style="width: 100%; height: 100%;" cols="20" name="ModalCommentsBoxAdd" rows="2" onkeypress="return checkFormat(this,event,&#39;AlphaNumeric&#39;,&#39;`,~,!,@,$,%,*,(,),-,_,+,\&#39;,&quot;,.,,,?,/,\\,|,:, &#39;);" style="height: 130px; width: 540px;" ></textarea>--%>
            <asp:TextBox ID="ModalCommentsBoxAdd" Columns="20" Rows="2" ClientIDMode="Static" style="height: 130px; width: 700px;" name="ModalCommentsBoxAdd" TextMode="MultiLine" runat="server" onkeypress="return checkFormat(this,event,&#39;AlphaNumeric&#39;,&#39;`,~,!,@,$,%,*,(,),-,_,+,\&#39;,&quot;,.,,,?,/,\\,|,:, &#39;);" ></asp:TextBox>
        </div>
    </div>
    

    <div id="divUnitComment" runat="server" class="hidden">
        <table>
            <tr>
                <td class="number">
                    A
                </td>
                <td class="label ">
                    Add Comment:
                </td>
                <td class="value">
                    <asp:TextBox ID="CommentsBox" runat="server" TextMode="MultiLine" Width="600px" Height="130px" onkeypress="return checkFormat(this,event,&#39;AlphaNumeric&#39;,&#39;`,~,!,@,$,%,*,(,),-,_,+,\&#39;,&quot;,.,,,?,/,\\,|,:, &#39;);"> </asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="number">
                    B
                </td>
                <td class="label ">
                    Action:
                </td>
                <td class="value">
                    <asp:Button ID="SaveButton" runat="server" Text="Save" Width="100px"></asp:Button><br />
                    <asp:Button ID="CancelButton" runat="server" Text="Cancel" Width="100px"></asp:Button>
                </td>
            </tr>
        </table>
    </div>
</asp:panel>
