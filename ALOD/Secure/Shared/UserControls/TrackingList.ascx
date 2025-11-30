<%@ Control Language="VB" AutoEventWireup="false" Inherits="ALOD.Web.UserControls.Secure_Shared_UserControls_TrackingList" Codebehind="TrackingList.ascx.vb" %>

<asp:ScriptManagerProxy runat="server" ID="smp">
    <Scripts>
        <asp:ScriptReference Path="~/Script/changelog.js" />
        <asp:ScriptReference Path="~/Script/tracking.js" />
    </Scripts>
</asp:ScriptManagerProxy>
<table style="width: 100%; margin-bottom: 6px;">
    <tbody>
        <tr>
            <td style="width: 33%; text-align: left;">
                <asp:CheckBox ID="chkShowAll" runat="server" AutoPostBack="True" Text="Show All" /></td>
            <td style="width: 33%; text-align: center;">
                <%--<a href='#' onclick="showWorkflowUsers('<%= TrackingReferenceId.ToString() %>', '<%= StatusCode %>');">
                    <asp:Image runat="server" ID="imgSearch" SkinID="imgUserSmall" ImageAlign="AbsMiddle"
                        AlternateText="Show users who can act on this case" />&nbsp;Who is working this
                    case?</a>--%>
            </td>
            <td style="width: 33%; text-align: right;">
                <a href="#" onclick="printTracking('<%= ModuleType %>','<%= ReferenceId.ToString() %>','<%= SecondaryId.ToString() %>', String(element('<%= chkShowAll.ClientId %>').checked) );">
                    <asp:Image runat="server" ID="imgPrint" SkinID="imgPrintSmall" ImageAlign="AbsMiddle"
                        AlternateText="Print" />&nbsp;Print Tracking </a>
            </td>
        </tr>
    </tbody>
</table>
<asp:GridView ID="gvTracking" runat="server" DataKeyNames="userId, logChanges" AutoGenerateColumns="False"
    Width="900px">
    <Columns>
        <asp:BoundField DataField="actionDate" DataFormatString="{0:MM/dd/yyyy HHmm}" HeaderText="Action Date"
            SortExpression="actiondate">
            <ItemStyle Wrap="False" />
        </asp:BoundField>
        <asp:BoundField DataField="description" HeaderText="Status" SortExpression="description" />
        <asp:TemplateField HeaderText="Action Name" SortExpression="actionName">
            <EditItemTemplate>
                <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("actionName") %>'></asp:TextBox>
            </EditItemTemplate>
            <ItemTemplate>
                <asp:HyperLink ID="lnkAction" runat="server" Text='<%# Eval("actionName") %>'></asp:HyperLink>
            </ItemTemplate>
            <ItemStyle Wrap="False" />
        </asp:TemplateField>
        <asp:BoundField DataField="notes" HeaderText="Comments" SortExpression="notes" />
        <asp:TemplateField HeaderText="User" SortExpression="lastname">
            <ItemTemplate>
                <asp:HyperLink ID="lnkUser" runat="server" Text='<%# Eval("lastname") %>'></asp:HyperLink>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
