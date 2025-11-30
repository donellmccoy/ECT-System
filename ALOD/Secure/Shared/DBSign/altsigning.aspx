<%@ Page Language="VB" AutoEventWireup="false" Inherits="ALOD.Web.DBSign.Secure_Shared_DBSign_altsigning" Codebehind="altsigning.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head runat="server">
    <title>Untitled Page</title>

    

    <script type="text/javascript" src="../../../Script/jquery-3.6.0.min.js"></script>
    <script type="text/javascript" src="../../../Script/jquery-migrate-3.4.1.min.js"></script>

    <script type="text/javascript" src="../../../Script/jquery.blockUI.min.js"></script>
    <script type="text/javascript" src="../../../Script/common.js"></script>

    <script type="text/javascript">
    
        function next()
        {   
            window.parent.endSign();
        }
               
		function rbCACJS() {
			element('rbSSN').checked=false;
			element('txtSSN').disabled=true;
			element('btnVerifySSN').disabled=true;
			element('btnGo').disabled=false;
			element('lblMsg').style.display="none";
		}
		
		function rbSSNJS() {
			element('rbCAC').checked=false;
			element('txtSSN').disabled=false;
			element('txtSSN').value="";
			element('txtSSN').focus();
			element('btnVerifySSN').disabled=false;
			onChangeSSN();
		}
		
		function onChangeSSN() {
			element('hValid').value="N";
			element('lblMsg').innerHTML="";
			element('btnGo').disabled=true;
		}
		
		function enableGo() {
			element('btnGo').disabled=false;
			element('btnGo').focus();
		}
		
		function disableGo() {
			element('txtSSN').focus();
			element('btnGo').disabled=true;
		}
		
		function processGo() {
			if (element('rbCAC').checked) {
				url = document.URL.replace('altsigning.aspx', 'signing.aspx');
				document.location.replace(url);
				return false;
			} else {
				//element('Form1').submit();
			    return true;
			}
		}
        function watchEnterKey(event, buttonId) {
            if (event.key === "Enter") {
                var button = document.getElementById(buttonId);
                if (button) {
                    button.focus();
                    button.click();
                }
            }
        }

// Usage example:
// Call the watchEnterKey function from your HTML input element with the event object and the button's ID.
// For example: <input type="text" onkeydown="watchEnterKey(event, 'myButton')" />

		//function Watch(oTextbox) {
		//	var oBtn = document.all[oTextbox.EnterKey];
		//	if (window.event.keyCode == 13) {
		//		oBtn.focus();
		//		oBtn.click();
		//	}
		//}
    </script>

</head>
<body runat="server" id="BodyTag" style="background-color: #c4caea;">
    <form id="form1" runat="server">
        <div>
            <input id="hValid" runat="server" type="hidden" value="N" />
            <input type="hidden" id="txtSender" runat="Server" />
            <table id="tblInput" align="center" runat="server" border="0">
                <tr>
                    <td align="center" colspan="2">
                        <span style="font-size: x-large; color: Red;">Authentication Required:</span></td>
                </tr>
                <tr>
                    <td class="label" colspan="2">
                        <asp:RadioButton ID="rbCAC" runat="server"></asp:RadioButton>Use my CAC for identification.</td>
                </tr>
                <tr>
                    <td class="label">
                        <asp:RadioButton ID="rbSSN" runat="server"></asp:RadioButton>Use my SSN:
                        <asp:TextBox ID="txtSSN" runat="server" MaxLength="9" Enabled="false"></asp:TextBox></td>
                    <td>
                        <asp:Button ID="btnVerifySSN" Text="Verify" Enabled="False" runat="server"></asp:Button></td>
                </tr>
                <tr>
                    <td class="label" colspan="2" align="center" height="10">
                        <asp:Label ID="lblMsg" runat="server" ForeColor="red"></asp:Label></td>
                </tr>
                <tr align="center">
                    <td colspan="2">
                        <asp:Button runat="server" ID="btnGo" Text="Proceed" Enabled="false" />
                    </td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
