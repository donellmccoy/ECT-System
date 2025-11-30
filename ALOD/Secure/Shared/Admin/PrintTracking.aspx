<%@ Page Language="VB" AutoEventWireup="false" Inherits="ALOD.Web.Admin.Secure_Shared_Admin_PrintTracking" Codebehind="PrintTracking.aspx.vb" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        An error occurred generating the requested document
        <asp:GridView runat="server" ID="HistoryGrid" AutoGenerateColumns="false" Width="100%">
            <Columns>
                <asp:TemplateField HeaderText="Date">
                    <ItemTemplate>
                        <%#Eval("ActionDate", "{0:MM/dd/yyyy hhmm}")%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField HeaderText="Action" DataField="ActionName" />
                <asp:BoundField HeaderText="Setting" DataField="Field" />
                <asp:BoundField HeaderText="Old Value" DataField="OldVal" />
                <asp:BoundField HeaderText="New Value" DataField="NewVal" />
                <asp:TemplateField>
                    <ItemTemplate>
                        <a href='#' onclick='getWhois(<%# Eval("UserId") %>); return false;'>
                            <%#Eval("UserName")%></a>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
    </form>
</body>
</html>
