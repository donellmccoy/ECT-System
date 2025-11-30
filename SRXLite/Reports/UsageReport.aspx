<%@ Page Language="VB" AutoEventWireup="false" Inherits="SRXLite.Web.Reports.Reports_UsageReport" Codebehind="UsageReport.aspx.vb" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"
	Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Usage Report</title>
   	<link href="../styles.css" type="text/css" rel="stylesheet" />
</head>
<body id="HtmlBody" runat="server" style="margin:10px">
    <form id="form1" runat="server">
	<asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <div>
		<h1 class="title">Usage Report</h1>
		<asp:UpdatePanel ID="UpdatePanel1" runat="server">
			<ContentTemplate>
				<div class="graydiv">
					<div style="font-size:1.2em; font-style:italic">Usage Summary</div><br />
					<div class="floatLeft">Start Date:<br /><uc:TextBoxCalendar ID="UsageSummary_DateStart" runat="server" /></div>
					<div class="floatLeft">End Date:<br /><uc:TextBoxCalendar ID="UsageSummary_DateEnd" runat="server" /></div>
					<div class="floatLeft">Category:<br /><asp:DropDownList ID="UsageSummary_ddlCategory" runat="server"></asp:DropDownList></div>
					<div class="floatLeft">User:<br /><asp:DropDownList ID="UsageSummary_ddlUser" runat="server"></asp:DropDownList></div>
					<div class="floatLeft"><br /><asp:Button ID="btnViewUsageSummary" runat="server" Text="View" /></div>
				</div>
				<asp:GridView ID="gvUsageSummary" runat="server" style="clear:both; float:left; width:50%; margin-right:100px" AutoGenerateColumns="false" CellPadding="5">
					<Columns>
						<asp:BoundField DataField="UserName" HeaderText="User" />
						<asp:BoundField DataField="CategoryName" HeaderText="Category" />
						<asp:BoundField DataField="ActionTypeName" HeaderText="Action" />
						<asp:BoundField DataField="Count" HeaderText="Count" />
					</Columns>
					<HeaderStyle BackColor="LightSteelBlue" HorizontalAlign="left" />
				</asp:GridView>
				<asp:Chart ID="chartUsageSummary" runat="server">
					<Series>
						<asp:Series Name="Series1">
						</asp:Series>
					</Series>
					<ChartAreas>
						<asp:ChartArea Name="ChartArea1">
						</asp:ChartArea>
					</ChartAreas>
				</asp:Chart>
			</ContentTemplate>
		</asp:UpdatePanel>
		<asp:UpdatePanel ID="UpdatePanel2" runat="server">
			<ContentTemplate>
				<div class="graydiv">
					<div style="font-size:1.2em; font-style:italic">Request Statistics</div><br />
					<div class="floatLeft">Start Date:<br /><uc:TextBoxCalendar ID="RequestStats_DateStart" runat="server" /></div>
					<div class="floatLeft">End Date:<br /><uc:TextBoxCalendar ID="RequestStats_DateEnd" runat="server" /></div>
					<div class="floatLeft"><br /><asp:Button ID="btnViewRequestStats" runat="server" Text="View" /></div>
				</div>
				<asp:GridView ID="gvRequestStats" runat="server" AutoGenerateColumns="false" CellPadding="2" Width="100%">
					<Columns>
						<asp:BoundField DataField="ReportDate" HeaderText="Date" DataFormatString="{0:f}" />
						<asp:BoundField DataField="DocGuidCount" HeaderText="Documents" />
						<asp:BoundField DataField="DocPageGuidCount" HeaderText="Pages" />
						<asp:TemplateField ItemStyle-Width="25%">
							<ItemTemplate>
								<div id="divDocs" runat="server" style="height:5px; background-color:lightgrey">&nbsp;</div>
								<div id="divPages" runat="server" style="height:5px; background-color:gray">&nbsp;</div>
							</ItemTemplate>
						</asp:TemplateField>
					</Columns>
					<HeaderStyle BackColor="LightSteelBlue" HorizontalAlign="left" />
				</asp:GridView>
			</ContentTemplate>
		</asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
