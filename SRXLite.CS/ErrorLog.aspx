<%@ Page Language="C#" AutoEventWireup="true" Inherits="SRXLite.Web.ErrorLog" CodeBehind="ErrorLog.aspx.cs" %>

    <!DOCTYPE html
        PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

    <html xmlns="http://www.w3.org/1999/xhtml">

    <head runat="server">
        <title>Error Log</title>
        <link href="styles.css" type="text/css" rel="stylesheet" />
        <script type="text/javascript">
            function expand(spanID) {
                var span = document.getElementById(spanID);
                span.style.display = "inline";

                var cell = span.parentNode;
                cell.innerHTML = cell.innerHTML.replace("...", "");
                cell.style.cursor = "auto";
                cell.onclick = null;
            }
        </script>
    </head>

    <body style="margin:10px">
        <form id="form1" runat="server">
            <div>
                <h1>Error Log</h1>
                <div style="text-align:right;padding-bottom:2px">Last
                    <asp:DropDownList ID="ddlTop" runat="server" AutoPostBack="true">
                        <asp:ListItem>25</asp:ListItem>
                        <asp:ListItem>50</asp:ListItem>
                        <asp:ListItem>100</asp:ListItem>
                        <asp:ListItem>300</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <asp:GridView ID="gvErrorLog" runat="server" EnableViewState="false" AutoGenerateColumns="false"
                    AllowSorting="false" CellPadding="4" Width="100%" OnRowDataBound="gvErrorLog_RowDataBound">
                    <Columns>
                        <asp:BoundField DataField="ErrorDate" HeaderText="Date" ItemStyle-Wrap="false" />
                        <asp:BoundField DataField="ErrorMsg" HeaderText="Error" />
                        <asp:BoundField DataField="ID" HeaderText="ID" />
                        <asp:BoundField DataField="UserName" HeaderText="User" />
                        <asp:BoundField DataField="SubuserName" HeaderText="Subuser" />
                    </Columns>
                    <HeaderStyle BackColor="black" ForeColor="white" HorizontalAlign="Left" Font-Bold="true" />
                    <RowStyle BackColor="#f5f5f5" />
                    <AlternatingRowStyle BackColor="#DCDCDC" />
                </asp:GridView>
            </div>
        </form>
    </body>

    </html>