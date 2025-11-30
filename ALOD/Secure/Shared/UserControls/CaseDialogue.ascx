<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="CaseDialogue.ascx.vb" Inherits="ALOD.Secure.Shared.UserControls.CaseDialogue" %>


<asp:Panel ID="Comments" runat="server">
    <style type="text/css">
        .CreatedByLabel {
            margin-right: 5px;
        }
        a.btn {
              -webkit-appearance: button;
              -moz-appearance: button;
              appearance: button;
        }
        .PersonIcon{
            padding-bottom: 3px;
            padding-top: 0px;
        }
        .RoleLabel{
            color:blue;
        }
        .RoleLabel2{
            color:tomato;
        }
        .cssbutton {
          -webkit-transition-duration: 0.4s; /* Safari */
          transition-duration: 0.4s;
          background-color:white;
        }
        .cssbutton:hover {
          background-color:cornflowerblue; /* Blue */
          color: white;
        }
        .cssbuttonDelete {
          -webkit-transition-duration: 0.4s; /* Safari */
          transition-duration: 0.4s;
          background-color:white;
        }
        .cssbuttonDelete:hover {
          background-color: #f44336; /* Red */
          color: white;
        }
        
        .modal {
    position: fixed;
    top: 0;
    left: 0;
    /* background-color: black; */
    z-index: 99;
    opacity: 0.8;
    min-height: 100%;
    width: 100%;
}



        .loading
        {
            font-family: Arial;
            font-size: 10pt;
            border: 5px solid #67CFF5;
            height: 30px;
            width: 200px;
            display: none;
            position: fixed;
            background-color: White;
            z-index: 999;
        }
    </style>

    <script type="text/javascript">

        /*
         * Find the Length of a Paragraph or String
         *  CheckStringLength
         */
        function GetStringLength(words) {
            return words.replace(/ /g, '').length;
        }

        //GridView Comment
        function showReply(n) {
            $("#divReply" + n).show();
            return false;
        }
        function closeReply(n) {
            $("#divReply" + n).hide();
            return false;
        }

        function ShowProgress() {
                    setTimeout(function () {
                    var modal = $('<div />');
                    modal.addClass("modal");
                    $('body').append(modal);
                    var loading = $(".loading");
                    loading.show();
                    var top = Math.max($(window).height() / 2 - loading[0].offsetHeight / 2, 0);
                    var left = Math.max($(window).width() / 2 - loading[0].offsetWidth / 2, 0);
                    loading.css({ top: top, left: left });
                    }, 100);
            return;
        }

        //When Add Unit Comment
        /// <summary>
        /// Summary for toAddUnitComment Function:
        /// Initialize the Modal Dialog (options), then open the Modal Dialog (when the board
        /// member click the checkbox).
        /// </summary>
        function toAddUnitComment() {
            $('.loading').hide();
            $('.modal').hide();

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
            $('#myDialogAdd').dialog("moveToTop");
        }

        /// <summary>
        /// Summary for okAddUnitComment Function:
        /// Validate the user add some text on the comment box.
        /// If the user add comments, it will call the server side script to insert the comment,
        /// otherwise, will alert the user to add some text(comments).
        /// </summary>
        function okAddUnitComment() {
	        //Passing value to Original Comments Box : ModalCommentsBox : CommentsBox

	        //var valueToValidate = $.trim($('#myDialogAdd #ModalCommentsBoxAdd').text());
            var valueToValidate = $.trim(document.getElementById('<%=ModalCommentsBoxAdd.ClientID %>').value);
	        if (valueToValidate === '') {
		        alert('Unit Comment cannot be empty!');
		        return false;
	        }
	        else {
                $('#<%= CommentsBox.ClientId %>').val(valueToValidate);
                //ShowProgress();
		        //Now the call to the ASPServer Button
		        $('#<%= SaveButton.ClientID %>').click();
                
            }
            
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
            $('.loading').hide();

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
            $('#myDialogEdit').dialog("moveToTop");
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
	        //var valueToValidate = $.trim($('#myDialogEdit #ModalCommentsBoxEdit').text());
            var valueToValidate = $.trim(document.getElementById('<%=ModalCommentsBoxEdit.ClientID %>').value);
	        if (valueToValidate == '') {
		        alert('Unit Comment cannot be empty!');
		        return false;
	        }
	        else {
                $('#<%= CommentsBox.ClientId %>').val(valueToValidate);
                //ShowProgress();
		        //Now the call to the ASPServer Button
                $('#' + editASPButton.id).on('click', function () {
                    // Your click event handler code here
                });

                
            }   
        }
        
        $(function () {
            $("#<%= CommentsBox.ClientId %>").val("");
            $('#myDialogAdd').dialog({ autoOpen: false });
            $('#myDialogEdit').dialog({ autoOpen: false });

        });

        
    </script>
    
    <asp:Image runat="server" ImageAlign="Middle" ImageUrl="~/images/fountain-pulse-30.gif" CssClass="loading" />

    <div class="formHeader">
        <asp:Label ID="lblCommentType" runat="server" />
    </div>
    <table style="width: 100%;">
        <tr>
            <td class="align-left">
                <asp:Button ID="AddUnitCommentButton" runat="server" Text="Add New Comment" OnClientClick="toAddUnitComment();return false;" CssClass="cssbutton" autopostback="false" />
            </td>
            <td class="align-right">
                <asp:CheckBox ID="SortCheckBox" runat="server" Text="Newest On Top" AutoPostBack="true"></asp:CheckBox>
            </td>
        </tr>
    </table>
    <br />
    <div runat="server" id="divCaseDialogueRepeater" visible="false" class="dataBlock">
        <div class="dataBlock-center">
            <asp:Label ID="CaseDialogue" runat="server" Text="Case Dialogue"></asp:Label>
        </div>
        <div class="dataBlock-body" runat="server">
            <asp:Repeater ID="CaseDialogueRepeater" runat="server">
                <HeaderTemplate>
                </HeaderTemplate>
                <ItemTemplate>
                    <div id="mainDiv" runat="server" class="dataBlock">
                        <div class="dataBlock-header">
                            <table style="width: 885px;">
                                <tr>
                                    <td class="align-left">
                                        <asp:Label ID="dateLabel" runat="server" />
                                    </td>
                                    <td class="align-right">
                                        <asp:Image runat="server" ID="LockImage" ImageAlign="AbsMiddle" ImageUrl="~/images/icons8-user-16.png" CssClass="PersonIcon"/>
                                        <asp:Label ID="RoleLabel" runat="server" CssClass="RoleLabel" Text='<%#Eval("role") %>' />
                                        <asp:Label ID="createdByLabel" runat="server" CssClass="CreatedByLabel" />
                                        <asp:TextBox ID="lb1COmmenId" runat="server" Visible="false" Text='<%#Eval("Id") %>'></asp:TextBox>
                                    </td>
                                </tr>
                            </table>
                        </div>
                        <div class="dataBlock-body">
                            <asp:Label ID="commentLabel" runat="server" CssClass="labelToSearch" />
                            <br />
                            <asp:Panel ID="pnlDelete" runat="server" Visible="true">
                                <br />
                                <%--<asp:Button ID="jQueryEditButton" runat="server" AutoPostBack="False" OnClientClick="toEditUnitComment(this);return false;" Text="Edit" CssClass="cssbutton" Visible="True" />
                                <asp:Button ID="EditButton" runat="server" AutoPostBack="True" CommandArgument='<%# Eval("Id") %>' CommandName="Edit" CssClass="hidden" Visible="True" />
                                <asp:Button ID="DeleteButton" runat="server" AutoPostBack="True" CommandArgument='<%# Eval("Id") %>' CommandName="Delete" OnClientClick="javascript:if(!confirm('Delete this Comment?'))return false;" Text="Delete" CssClass="cssbuttonDelete" Visible="True" />--%>

                                <a class="" id="lnkReplyParent" href="javascript:void(0)" onclick="showReply(<%#Eval("Id") %>);return false;"><button type="button" class="cssbutton">Reply</button></a>
                                <%--<a class="" id="lnkCancel" href="javascript:void(0)" onclick="closeReply(<%#Eval("Id") %>);return false;"><button type="button" class="cssbutton">Cancel</button></a>--%>

                                 <!-- divReply -->
                                <div id='divReply<%#Eval("Id") %>' style="display: none; margin-top: 5px;">
                                    <asp:TextBox ID="textCommentReplyParent" CssClass="input-group" runat="server" Width="700px" Height="80px" TextMode="MultiLine" onkeypress="return checkFormat(this,event,&#39;AlphaNumeric&#39;,&#39;`,~,!,@,$,%,*,(,),-,_,+,\&#39;,&quot;,.,,,?,/,\\,|,:, &#39;);" ></asp:TextBox>
                                    <br />
                                    <asp:Button ID="btnReplyParent" runat="server" Text="Send" CssClass="input-group btn cssbutton" OnClick="btnReplyParent_Click"  AutoPostBack="false" /> <%--OnClientClick="ShowProgress();"--%>
                                </div>
                                <br />
                                <%--<div id="spinner" class="loader" style="margin-top: 15px;" runat="server"></div>--%>
                            </asp:Panel>
                        </div>
                        <div style="margin: auto;padding: 5px 5px 5px 60px;">
                            <asp:Repeater ID="CaseDialogueRepeater2" runat="server">
                                <HeaderTemplate>
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <div id="mainDiv2" runat="server" class="dataBlock"  >
                                        <div class="dataBlock-header" >
                                            <table style="width: 100%;">
                                                <tr>
                                                    <td class="align-left">
                                                        <asp:Label ID="dateLabel2" runat="server" Text='<%#Eval("createdDate") %>'/>
                                                    </td>
                                                    <td class="align-right" style="padding-right: 15px;">
                                                        <asp:Image runat="server" ID="LockImage" ImageAlign="AbsMiddle" ImageUrl="~/images/icons8-user-16.png" CssClass="PersonIcon" />
                                                        <asp:Label ID="RoleLabel" runat="server" CssClass="RoleLabel2" Text='<%#Eval("role") %>' />
                                                        <asp:Label ID="createdByLabel2" runat="server" CssClass="CreatedByLabel" Text='<%#Eval("createdBy") %>' />
                                                        <asp:TextBox ID="lb1ChildCOmmenId" runat="server" Visible="false" Text='<%#Eval("Id") %>'></asp:TextBox>
                                                    </td>
                                                </tr>
                                            </table>
                                        </div>
                                        <div class="dataBlock-body" style="background-color:lightsteelblue">
                                            <asp:Label ID="commentLabel2" runat="server" CssClass="labelToSearch" Text='<%#Eval("comment") %>' />
                                            <br />
                                            <asp:Panel ID="pnlDelete2" runat="server" Visible="false">
                                                <br />
                                                <%--<a class="" id='lnkReplyParent<%#Eval("Id") %>' href="javascript:void(0)" onclick="showReply(<%#Eval("Id") %>);return false;" ><button type="button" class="cssbutton" >Edit</button></a>--%>
                                                <%--<a class="" id="lnkReplyParent2" href="javascript:void(0)" onclick="showReply(<%#Eval("Id") %>);return false;" ><button type="button" class="cssbutton" >Edit</button></a>
                                                <a class="" id="lnkCancel2" href="javascript:void(0)" onclick="closeReply(<%#Eval("Id") %>);return false;"><button type="button" class="cssbutton">Cancel</button></a>--%>
                                                <!-- divReply -->
                                                <div id='divReply<%#Eval("Id") %>' style="display: none; margin-top: 5px;">
                                                    <asp:TextBox ID="textUpdateChildComment" CssClass="input-group" runat="server" Text='<%#Eval("comment") %>' Width="700px" Height="80px" TextMode="MultiLine" onkeypress="return checkFormat(this,event,&#39;AlphaNumeric&#39;,&#39;`,~,!,@,$,%,*,(,),-,_,+,\&#39;,&quot;,.,,,?,/,\\,|,:, &#39;);" ></asp:TextBox>
                                                    <br />
                                                    <asp:Button ID="btnUpdateChildComment" runat="server" Text="Save" CssClass="input-group btn cssbutton" OnClick="btnUpdateChildComment_Click" AutoPostBack="false"/>
                                                </div>
                                                <br />
                                                <%--<div id="spinner" class="loader" style="margin-top: 5px;" runat="server"></div>--%>
                                            </asp:Panel>
                                        </div>
                                    </div>
                                </ItemTemplate>
                                <FooterTemplate>
                                </FooterTemplate>
                            </asp:Repeater>
                      </div>
                    </div>  
                </ItemTemplate>
                <FooterTemplate>
                </FooterTemplate>
            </asp:Repeater>
        </div>
    </div>
    <div id="myDialogs">
        <div id="myDialogEdit" class="hidden">
            <%--<textarea id="ModalCommentsBoxEdit" style="width: 100%; height: 100%;" cols="20" name="ModalCommentsBoxEdit" rows="2" onkeypress="return checkFormat(this,event,&#39;AlphaNumeric&#39;,&#39;`,~,!,@,$,%,*,(,),-,_,+,\,&quot;,.,,,?,/,\\,|,: &#39;);" style="height: 130px; width: 540px;" ></textarea>--%>
            <asp:TextBox ID="ModalCommentsBoxEdit" Columns="20" Rows="2" ClientIDMode="Static" style="height: 130px; width: 700px;" name="ModalCommentsBoxEdit" TextMode="MultiLine" runat="server" onkeypress="return checkFormat(this,event,&#39;AlphaNumeric&#39;,&#39;`,~,!,@,$,%,*,(,),-,_,+,\&#39;,&quot;,.,,,?,/,\\,|,:, &#39;);" ></asp:TextBox>
        </div>

        <div id="myDialogAdd" class="hidden">
            <%--<textarea id="ModalCommentsBoxAdd" style="width: 100%; height: 100%;" cols="20" name="ModalCommentsBoxAdd" rows="2" onkeypress="return checkFormat(this,event,&#39;AlphaNumeric&#39;,&#39;`,~,!,@,$,%,*,(,),-,_,+,\,&quot;,.,,,?,/,\\,|,: &#39;);" style="height: 130px; width: 540px;" ></textarea>--%>
            <asp:TextBox ID="ModalCommentsBoxAdd" Columns="20" Rows="2" ClientIDMode="Static" style="height: 130px; width: 700px;" name="ModalCommentsBoxAdd" TextMode="MultiLine" runat="server" onkeypress="return checkFormat(this,event,&#39;AlphaNumeric&#39;,&#39;`,~,!,@,$,%,*,(,),-,_,+,\&#39;,&quot;,.,,,?,/,\\,|,:, &#39;);" ></asp:TextBox>
        </div>
    </div>


    <div id="divUnitComment" runat="server" class="hidden">
        <table>
            <tr>
                <td class="number">A
                </td>
                <td class="label ">Add Comment:
                </td>
                <td class="value">
                    <asp:TextBox ID="CommentsBox" runat="server" TextMode="MultiLine" Width="600px" Height="130px" onkeypress="return checkFormat(this,event,&#39;AlphaNumeric&#39;,&#39;`,~,!,@,$,%,*,(,),-,_,+,\&#39;,&quot;,.,,,?,/,\\,|,:, &#39;);"> </asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="number">B
                </td>
                <td class="label ">Action:
                </td>
                <td class="value">
                    <asp:Button ID="SaveButton" runat="server" Text="Save" Width="100px"></asp:Button><br />
                    <asp:Button ID="CancelButton" runat="server" Text="Cancel" Width="100px"></asp:Button>
                </td>
            </tr>
        </table>
    </div>

    
</asp:Panel>
