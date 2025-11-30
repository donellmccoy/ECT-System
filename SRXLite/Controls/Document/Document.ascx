<%@ Control Language="VB" AutoEventWireup="false" Inherits="SRXLite.Web.Controls.Document.Controls_Document" Codebehind="Document.ascx.vb" %>

<asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
	<Scripts>
		<asp:ScriptReference Path="~/Includes/Util.js" />
		<asp:ScriptReference Path="~/Controls/Document/Document.js" />
	</Scripts>
	<Services>
		<asp:ServiceReference Path="~/Services/DocumentScriptService.svc" />
	</Services>
</asp:ScriptManagerProxy>
<ul id="menu" runat="server" class="contextMenu" oncontextmenu="return HideMenu();">
	<li onclick="Zoom(1);"><div class="contextMenu_ZoomIn">Zoom In (+)</div></li>
	<li onclick="Zoom(-1);"><div class="contextMenu_ZoomOut">Zoom Out (-)</div></li>
	<li onclick="Rotate(-1);"><div class="contextMenu_RotateCCW">Rotate Counterclockwise (Ctrl+K)</div></li>
	<li onclick="Rotate(1);"><div class="contextMenu_RotateCW">Rotate Clockwise (Ctrl+L)</div></li>
	<li onclick="Resize(0);"><div class="contextMenu_FitWidth">Fit Width (Ctrl+W)</div></li>
	<li onclick="Resize(1);"><div class="contextMenu_BestFit">Best Fit (Ctrl+B)</div></li>
	<li onclick="Resize(2);"><div class="contextMenu_ActualSize">Actual Size (Ctrl+A)</div></li>
	<li id="menuDelete" runat="server" onclick="Delete();"><div class="contextMenu_Delete">Delete (Ctrl+D)</div></li>
</ul>
<div style="height:100%; width:100%">
	<div style="float:left; height:100%; width:165px; overflow:scroll; overflow-x:hidden">
		<div id="divThumbnails" runat="server" style="text-align:center; background-color:#d3d3d3">test</div>
	</div>
	<div style="height:100%; overflow:auto; margin-left: 150px">
		<div style="height:50px">
			test<br /><br />
		</div>
		<div style="clear:left; overflow:auto">
			<div id="divContent" runat="server" style="text-align:center; background-color:#d3d3d3"></div>
		</div>
	</div>
</div>