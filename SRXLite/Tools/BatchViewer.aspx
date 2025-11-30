<%@ Page Language="VB" AutoEventWireup="false" Inherits="SRXLite.Web.Tools.Tools_BatchViewer" Codebehind="BatchViewer.aspx.vb" %>
<%@ Register TagPrefix="uc" TagName="Document" Src="~/Controls/Document/Document.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Batch Viewer</title>
    <link href="../styles.css" type="text/css" rel="stylesheet" />
    <script src="../Scripts/jquery-3.6.0.min.js" type="text/javascript"></script>
    <script src="../Includes/jquery-migrate-1.2.1.min.js" type="text/javascript"></script>
    <script src="../includes/BatchViewer.js" type="text/javascript"></script>
    <script src="../includes/Util.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
		<asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
		<div style="height:100%; width:100%">
			<div style="float:left; height:100%; width:165px; overflow:scroll; overflow-x:hidden">
				<uc:Document id="Document1" runat="server" Mode="Thumbnail" Width="150px" BackgroundColor="whitesmoke"></uc:Document>
			</div>
			<div style="height:100%; overflow:auto; margin-left: 150px">
				<div style="height:50px">
					test<br /><br />
				</div>
				<div style="clear:left; overflow:auto">
					<uc:Document id="Document2" runat="server"></uc:Document>
				</div>
			</div>
		</div>
    </form>
</body>
</html>
