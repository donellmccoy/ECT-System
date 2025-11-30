<%@ Page Language="vb" AutoEventWireup="false" Inherits="ALOD.Web.SessionViewer" Codebehind="SessionViewer.aspx.vb" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN">
<html lang="en">
<head runat="server">
    <title>SessionViewer</title>
    <meta name="GENERATOR" content="Microsoft Visual Studio .NET 7.1">
    <meta name="CODE_LANGUAGE" content="Visual Basic .NET 7.1">
    <meta name="vs_defaultClientScript" content="JavaScript">
    <meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">

    <script type="text/javascript">
			function DoRefresh() 
			{
				window.location.reload(true);
			}
    </script>

</head>
<body>
    <form id="Form1" method="post" runat="server">
        <asp:Panel ID="Panel1" runat="server">
            <asp:LinkButton ID="lnkRefresh" runat="server">Refresh</asp:LinkButton>&nbsp;&nbsp;
            <br>
            Session Mode:&nbsp;
            <asp:Label ID="lblMode" Font-Bold="True" runat="server"></asp:Label>&nbsp;&nbsp;
            Current Objects:&nbsp;
            <asp:Label ID="lblCount" runat="server" Font-Bold="True">0</asp:Label>&nbsp;&nbsp;&nbsp;Null
            Objects:&nbsp;
            <asp:Label ID="lblNulls" runat="server" Font-Bold="True">0</asp:Label>
            <br />
            Permissions:&nbsp;
            <asp:Label id="lblPerms" runat="server" Font-Bold="True"></asp:Label>
            <br />
            <br />
            <table style="font-size: 10pt; color: black; font-family: courier new" cellspacing="0"
                cellpadding="0" border="0">
                <tr>
                    <td bgcolor="#484848">
                        <span style="color: white">&nbsp;Key</span></td>
                    <td bgcolor="#484848">
                        &nbsp;&nbsp;&nbsp;</td>
                    <td bgcolor="#484848">
                        <span style="color: white">Type</span></td>
                    <td bgcolor="#484848">
                        &nbsp;&nbsp;&nbsp;</td>
                    <td bgcolor="#484848">
                        <span style="color: white">Value</span></td>
                </tr>
                <asp:Literal ID="ltSummary" runat="server"></asp:Literal></table>
        </asp:Panel>
    </form>
</body>
</html>
