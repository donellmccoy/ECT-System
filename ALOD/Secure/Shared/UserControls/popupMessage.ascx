<%@ Control AutoEventWireup="false" Inherits="ALOD.Web.UserControls.Secure_Shared_UserControls_popupMessage"
	Language="VB" Codebehind="popupMessage.ascx.vb" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
 
<script type="text/javascript">
    var msg;
    var gTimer;
    
    function Messages() {
        this._curPos = 0;
        this._maxPos = 0;
        this.getMessages();
        this.startTimer();
    }
    Messages.prototype._curPos;
    Messages.prototype._maxPos;
    Messages.prototype._Messages;
    Messages.prototype._From;
    Messages.prototype._When;
    
    
    Messages.prototype.getMessages = function() {

    
          $.ajax ({
                type: 'POST',
                url: $_HOSTNAME + '/Secure/utility/dataservice.asmx/GetMessages',
                dataType: 'xml',
                data: '',
                
                error: function(obj, errorText, exc) {

	            },
        		
                success: function(xml) {
                 
                    msg.readMessages(xml);
                } //end success call
            })
            this.startTimer();
    }
    
    

    
    Messages.prototype.readMessages = function(xml)
		{
            var nodes = xml.documentElement.childNodes;
            var count = 0;
            for (var x = 0; x < nodes.length; x++)
            {
                if (nodes[x].attributes != null) {
                    count++;
                }
            }
            this._Messages = new Array(count);
            this._From = new Array(count);
            this._When = new Array(count);
            count = 0;
            
            for (var x = 0; x < nodes.length; x++)
            {
                if (nodes[x].attributes != null) {
                    this._Messages[count] = nodes[x].attributes[0].nodeValue;
                    this._From[count] = nodes[x].attributes[1].nodeValue;
                    this._When[count] = nodes[x].attributes[2].nodeValue;
                    count++;
                }
            }
            //Pull our list of messages here!
            this._maxPos = this._Messages.length - 1;
            if (this._maxPos >= 0) {
                if (this._curPos > this._maxPos) {
                    this._curPos = 0;
                }
                if (this._curPos == 0) {
                document.getElementById("btnMessageClose").disabled = "disabled"; 
                this.loadMessage();
                }
                this.open();
            } 
            else {
                $find('mdlMessageBehavior').hide();
            }
        }
    
    Messages.prototype.loadMessage = function() {
        var cur = this._curPos;
        var max = this._maxPos;
        cur += 1;
        max += 1;
        document.getElementById("divMessage").innerHTML = this._Messages[this._curPos];
        document.getElementById("spnFrom").innerHTML = this._From[this._curPos];
        document.getElementById("spnWhen").innerHTML = this._When[this._curPos];
        document.getElementById("spnCount").innerHTML = "Message " + cur + " of " + max;
        this.setVisible();
    }
    
    Messages.prototype.setVisible = function() {
        if (this._curPos == 0) {
            document.getElementById("btnMessagePrev").disabled = "disabled";
        }
        else {
            document.getElementById("btnMessagePrev").disabled = "";
        }
        if (this._curPos == this._maxPos) {
            document.getElementById("btnMessageNext").disabled = "disabled";
            document.getElementById("btnMessageClose").disabled = "";
        }
        else {
            document.getElementById("btnMessageNext").disabled = "";           
        }
    }
    
    Messages.prototype.startTimer = function() {
        if (gTimer > 0) {
            window.clearInterval(gTimer);
        }
        gTimer = window.setTimeout('msg.getMessages();', 60000);   
    }
    
    Messages.prototype.nextMessage = function() {
        if (this._curPos < this._maxPos) {
            this._curPos += 1;
            this.loadMessage();
        }
    }
    
    Messages.prototype.prevMessage = function() {
        if (this._curPos > 0) {
            this._curPos -= 1;
            this.loadMessage();
        }
    }
    
    Messages.prototype.close = function() {
        //Mark all messages as read.
        this.readAll();
        this._curPos = 0;
        this._maxPos = 0;
        $find('mdlMessageBehavior').hide();
    } 
    
    Messages.prototype.readAll = function() {
              $.ajax ({
                type: 'POST',
                url: $_HOSTNAME + '/Secure/utility/dataservice.asmx/SetMessagesRead',
                dataType: 'xml',
                
                error: function(obj, errorText, exc) {
                    alert(obj);
		            alert(errorText);
		            if (exc != null) {
			            alert ('Exception: ' + exc.message);
		            }
	            },
        		
                success: function(xml) {
                } //end success call
            })
    }
    Messages.prototype.open = function() {
        $find('mdlMessageBehavior').show();
    }
</script>
<asp:Panel ID="pnlPopup" runat="server" style="height:150px;width:400px;display:none">
				<div class="modalPanel" style="position: relative; padding-bottom: 40px; min-width: 300px;
					min-height: 150px;">
					<h4>New Message(s)</h4>
					<div class="modalBody" style="position: relative">
					    <b>From:</b><span id="spnFrom"></span><br />
					    <b>Date:</b><span id="spnWhen"></span><br /><br />
					    <fieldset style="padding: 5px 5px 5px 5px;">
					    <span id="divMessage">
					    </span>
					    </fieldset>
					    <div style="position:absolute;top:5px;right:5px;">
				            <span id="spnCount"></span> <br />
				        </div>
				    </div>

				    <div style="position:absolute;left:5px;bottom:5px">
					    <span>
					    <input id="btnMessagePrev" type="button" onclick="msg.prevMessage();"  value="Prev"/>
					    <input id="btnMessageNext" type="button" onclick="msg.nextMessage();" value="Next"/>
					    </span>
					</div>
					<div style="position:absolute;bottom:5px;right:5px"> 
					    <input id="btnMessageClose" type="button" onclick="msg.close();"  value="Close"/>
					</div>
				</div>
			    </asp:Panel>		
<asp:Button ID="btnHidden" runat="server" style="display:none;" />	
<cc1:ModalPopupExtender ID="mdlPopupMessage" runat="server" 
        PopupControlID="pnlPopup" 
        TargetControlID="btnHidden" BehaviorID="mdlMessageBehavior"
        BackgroundCssClass="modalBackground">
</cc1:ModalPopupExtender>

<script type="text/javascript">
function pageLoad()
    {
    msg = new Messages();
    }
</script>


