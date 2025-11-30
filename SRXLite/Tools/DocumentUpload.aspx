<%@ Page Language="VB" AutoEventWireup="false" Inherits="SRXLite.Web.Tools.Tools_DocumentUpload" Codebehind="DocumentUpload.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Document Upload</title>
   	<link id="lnkStyleSheet" runat="server" href="../styles.css" type="text/css" rel="stylesheet" />
    <script src="../Scripts/jquery-3.6.0.min.js" type="text/javascript"></script>
    <script src="../Includes/jquery-migrate-1.2.1.min.js" type="text/javascript"></script>
    <script src="../includes/DocumentUpload.js" type="text/javascript"></script>
    <script src="../includes/Util.js" type="text/javascript"></script>
</head>
<body style="margin:10px">
    <form id="form1" runat="server">
		<asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <asp:Label ID="TestLabel" runat="server" Text="Testing was Successful! Connection to SRXLite was Made!" style="color:Green; font-size:larger;" Visible="false"></asp:Label> 
		<div id="divDocumentUpload" runat="server">
			<h1 id="DocTypeName" runat="server" class="title"></h1>
			<div id="divSubtitle" runat="server" class="subtitle"></div>
			<div id="divValidation" runat="server" style="color:red;"></div>
			<table cellpadding="2" cellspacing="0">
				<tr>
					<td class="label">Document&nbsp;Date:</td>
					<td><uc:TextBoxCalendar ID="tcDocDate" runat="server" TextBoxOnChange="CheckDate(this);" />
					</td>
				</tr>
				<tr>
					<td class="label">Description:</td>
					<td><asp:TextBox ID="txtDocDescr" runat="Server" MaxLength="50" Width="210"></asp:TextBox><asp:RegularExpressionValidator ID="regex_txtDocDescr" runat="server" ValidationExpression="^[\w\.\-\(\) ]*$" Display="Dynamic" ControlToValidate="txtDocDescr" ErrorMessage="<br />* Alphanumeric characters only"></asp:RegularExpressionValidator></td>
				</tr>
				<tr style="display:none">
					<td class="label">Keywords:</td>
					<td><asp:TextBox ID="txtKeywords" runat="Server" MaxLength="50" Width="210"></asp:TextBox></td>
				</tr>	
				<tr>
					<td class="label" valign="top"><div style="margin-top:4px">Source:</div></td>
					<td>
						<span><input id="rbUpload" runat="server" name="InputSource" type="radio" value="Upload" onclick="SwitchInputMethod(this.value);" checked="true" /><asp:Label runat="server" AssociatedControlID="rbUpload" Text="File" /></span>
						<span><input id="rbScan" runat="server" name="InputSource" type="radio" value="Scan" onclick="SwitchInputMethod(this.value);" /><asp:Label runat="server" AssociatedControlID="rbScan" Text="Scanner" /></span>
					</td>
				</tr>
				<tr id="trUpload" runat="server" style="display:none">
					<td class="label">File Name:</td>
					<td><asp:FileUpload ID="FileUpload1" runat="server" /></td>
				</tr>
				<tr id="trScan" runat="server" style="display:none">
					<td class="label" valign="top"><div style="margin-top:4px">Scan Settings:</div></td>
					<td>
						<div id="scanSettings" runat="server">
							<div class="floatLeft">
								<div>Mode:</div>
								<select id="ddlMode" runat="server">
									<option value="0">Black & White</option>
									<option value="1">Grayscale</option>
									<option value="2">Color</option>
								</select>
							</div>
							<div class="floatLeft">
								<div>Resolution:</div>
								<select id="ddlResolution" runat="server">
									<option value="high">High</option>
									<option value="normal">Normal</option>
									<option value="low">Low</option>
								</select>
							</div>
							<div class="floatLeft">
								<div>Page Side:</div>
								<select id="ddlPageSide" runat="server">
									<option value="single">Single Side</option>
									<option value="both">Both Sides</option>
								</select>
							</div>
						</div>
						<div class="clearLeft">
							<input id="chkScanCustom" runat="server" type="checkbox" onclick="ScanCustomToggle();" /><asp:Label runat="server" AssociatedControlID="chkScanCustom" Text="Custom Settings" />
						</div>
					</td>
				</tr>
				<tr>
					<td></td>
					<td style="padding-top:20px"><input id="btnUploadScan" runat="server" type="button" value="Upload" onclick="SubmitForm();" />
						<input id="btnSubmit" runat="server" type="submit" style="display:none" />
						&nbsp;<input id="btnCancel" type="button" value="Cancel" onclick="Cancel();" />
						<div id="divLoading" class="loading"><span id="lblLoadingMsg"></span></div>
						<div id="divStatusMsg" class="status"></div>
					</td>
				</tr>
			</table>
		</div>
    </form>
</body>
</html>
