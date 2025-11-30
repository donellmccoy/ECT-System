<%@ Page Language="VB" AutoEventWireup="false" Inherits="SRXLite.Web.Tools.Tools_DocumentViewer" Codebehind="DocumentViewer.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Document Viewer</title>
	<link href="../styles.css" type="text/css" rel="stylesheet" />
    <script src="../Scripts/jquery-3.6.0.min.js" type="text/javascript"></script>
    <script src="../Includes/jquery-migrate-1.2.1.min.js" type="text/javascript"></script>
    <script src="../Includes/jquery-ui-1.11.3.min.js" type="text/javascript"></script>
    <script src="../includes/DocumentViewer.js" type="text/javascript"></script>
    <script src="../includes/Util.js" type="text/javascript"></script>
     <link type="text/css" href="../css/smoothness/jquery-ui-1.11.3.custom.css" rel="stylesheet" />
    <style type="text/css">
    	@media Print {
			.mainContainer {
				height: auto;
				width: auto;
			}
			
			.leftColumn { margin-left:0px; }
			.leftColumnInner { margin-left:0px; }
			.rightColumn { display: none; }
			.topPanel { display: none; }
			img.page, img.pageActive { border-width: 0px; }
		}
		
		@media Screen {
			html, body, form {height: 100%;}
			html {overflow: auto;} /* Hide disabled vertical scrollbar */

			.mainContainer {
				height:100%;
				width:100%;
			}

			.leftColumn {
				float:left;
				width:100%; 
				margin-left:-180px; 
				height:100%; 
				overflow:hidden;
			}
			
			.leftColumnInner {
				height:100%; 
				margin-left:180px; 
				overflow-y:scroll; 
				overflow-x:auto;
			}
			
			.rightColumn {
				float:left; 
				height:100%; 
				width:180px; 
				overflow-y:scroll; 
				overflow-x:hidden;
			}
			
			.topPanel {
				float:left;
				padding:10px;
				background-color:white; 
				border-bottom:solid 1px gray;
			}
			
						#dialog_link {padding: .4em 1em .4em 20px;text-decoration: none;position: relative;}
			#dialog_link span.ui-icon {margin: 0 5px 0 0;position: absolute;left: .2em;top: 50%;margin-top: -8px;}
		}
    </style>
    <script type="text/javascript">

    </script>
</head>
<body id="HtmlBody" runat="server" class="viewerBody" style="overflow:hidden" onclick="HideMenu();" onkeydown="return CheckHotKeys(event);">
    <form id="form1" runat="server">
		<asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>
		<div id="divContent" runat="server" class="mainContainer">
			<div class="leftColumn">
				<div class="leftColumnInner">
					<div class="topPanel">
						<asp:Label ID="lblDocType" runat="server" Font-Bold="true" CssClass="title" style="float:left"></asp:Label>
						<div id="divPrint" runat="server" style="float:right; cursor:pointer; color:gray" onclick="Print();"><img src="../images/printer.png" alt="" style="margin-right:5px" />Print</div><div id="divPDF" runat="server" style="float:right; cursor:pointer; color:gray; margin-right:20px"><img src="../images/pdf.png" alt="" style="margin-right:5px" />Download</div>
					</div>
					<div id="divImages" runat="server" style="clear:left; text-align:center; background-color:#d3d3d3"></div>
				</div>
			</div>
			<div class="rightColumn"><br />
				<p style="display:none"><a href="#" id="dialog_link" class="ui-state-default ui-corner-all"><span class="ui-icon ui-icon-newwin"></span>Edit Keys</a></p>
				<div id="divThumbnails" runat="server" style="text-align:center; background-color:#d3d3d3; padding:5px"></div>
			</div>
			
			<ul id="menu" runat="server" class="contextMenu printHide" oncontextmenu="return HideMenu();">
				<li onclick="Zoom(1);"><div class="contextMenu_ZoomIn">Zoom In (+)</div></li>
				<li onclick="Zoom(-1);"><div class="contextMenu_ZoomOut">Zoom Out (-)</div></li>
				<li onclick="Rotate(-1);"><div class="contextMenu_RotateCCW">Rotate Counterclockwise (Ctrl+K)</div></li>
				<li onclick="Rotate(1);"><div class="contextMenu_RotateCW">Rotate Clockwise (Ctrl+L)</div></li>
				<li onclick="Resize(0);"><div class="contextMenu_FitWidth">Fit Width (Ctrl+W)</div></li>
				<li onclick="Resize(1);"><div class="contextMenu_BestFit">Best Fit (Ctrl+B)</div></li>
				<li onclick="Resize(2);"><div class="contextMenu_ActualSize">Actual Size (Ctrl+A)</div></li>
				<li id="menuDelete" runat="server" onclick="Delete();"><div class="contextMenu_Delete">Delete (Ctrl+D)</div></li>
			</ul>

			<div id="editDialog" title="Edit Document Info" style="clear:both;">
				<table>
					<tr>
						<td>For:</td>
						<td><asp:TextBox ID="txtFor" runat="server"></asp:TextBox></td>
					</tr>
					<tr>
						<td>Status:</td>
						<td><asp:DropDownList ID="ddlDocStatus" runat="server"></asp:DropDownList></td>
					</tr>
					<tr>
						<td>Document Date:</td>
						<td><uc:TextBoxCalendar ID="tcDocDate" runat="server" /></td>
					</tr>
					<tr>
						<td>Document Type:</td>
						<td><asp:DropDownList ID="ddlDocType" runat="server"></asp:DropDownList></td>
					</tr>
					<tr>
						<td>Description:</td>
						<td><asp:TextBox ID="txtDocDescr" runat="server"></asp:TextBox></td>
					</tr>
				</table>
				<hr />
				<div><asp:Button ID="btnSave" runat="server" Text="Save" /></div>
			</div>
		</div>

    </form>
</body>
</html>