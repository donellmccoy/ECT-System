<%@ Control Language="VB" AutoEventWireup="false"
    Inherits="ALOD.Web.UserControls.ModuleHeader" Codebehind="ModuleHeader.ascx.vb" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>

<script type="text/javascript">
   

    function OpenDialog() {
        
        $('#myDialog').dialog('open');
    }

    function CloseDialog() {

        $('#myDialog').dialog('close');
        
    }

    function OpenDialog2() {

        $('#myDialog').dialog('open');
    }

    function CloseDialog2() {

        $('#myDialog').dialog('close');

    }

    $(function () {
        $('#myDialog').dialog({
            autoOpen: false,
            height: 300,
            width: 300,
            modal: true,
            title: "Associated LOD Cases",
            buttons: { 'Close': function () { $(this).dialog('close'); } }
        });

        $('#myDialog2').dialog({
            autoOpen: false,
            height: 300,
            width: 300,
            modal: true,
            title: "Associated Special Cases",
            buttons: { 'Close': function () { $(this).dialog('close'); } }
        });
    });

</script>

<table class="moduleHeader" style="width: 940px; height: 18px;">
    <tr>
        <td style="text-align: left; width: 33%;">
            <asp:Image runat="server" ID="LockImage" ImageAlign="AbsMiddle" ImageUrl="~/images/lock.gif"
                AlternateText="This case is read-only" Visible="true" />
            <asp:Label ID="lblCaseId" runat="server" Text="YYYYMMDD-XXX  Workflow"></asp:Label>
            
            <div id="divAssociatedCase" style="visibility:hidden" runat="server">
                <asp:Label ID="lblAssociatedCase" runat="server" Text="" />
                <asp:LinkButton ID="lnkAssociatedCase" runat="server" CssClass="AssociatedLink" visible="false"/>
                <asp:LinkButton ID="MultipleCases" visible="false" runat="server" OnClientClick="OpenDialog(); return false;" Text="Show All Associated LODs" CssClass="AssociatedLink"/>
            </div>
            <div id="divAssociatedCase2" style="visibility:hidden" runat="server">
                <asp:Label ID="lblAssociatedCase2" runat="server" Text="" />
                <asp:LinkButton ID="lnkAssociatedCase2" runat="server" CssClass="AssociatedLink" visible="false"/>
                <asp:LinkButton ID="MultipleCases2" visible="false" runat="server" OnClientClick="OpenDialog(); return false;" Text="Show All Associated SCs" CssClass="AssociatedLink"/>
            </div>
        </td>
        <td style="text-align: center; width: 34%;">
            <asp:Label ID="lblSmName" runat="server" Text="DOE JOHN,  XXX-XX-1234"></asp:Label>
        </td>
        <td style="text-align: right; width: 33%;">
            <asp:Label ID="lblStatus" runat="server" Text="Status"></asp:Label>
        </td>
    </tr>
</table>
<div style="width: 940px; margin-left: 11px;">
    <asp:Panel runat="server" ID="FeedbackPanel" Visible="false" CssClass="ui-state-highlight info-block msg-panel no-margin">
        <asp:Label runat="server" ID="FeedbackMessageLabel" />
        <asp:HyperLink runat="server" ID="FeedbackLink" />
    </asp:Panel>
</div>
<asp:Panel runat="server" ID="pnlDeleted" Visible="false" CssClass="deleted">
    This case has been deleted.
    <asp:Label runat="server" ID="lblRestore">Click
        <asp:LinkButton runat="server" ID="btnRestore" OnClientClick="return confirmAction('restore');">here</asp:LinkButton>
        to restore this case.</asp:Label>
</asp:Panel>

<div id="myDialog" class="hidden" style="overflow-y: scroll;" >
    <asp:GridView runat="server" ID="GridView1" AutoGenerateColumns="false" Width="98%" AllowSorting="true">
        <Columns>
            <asp:TemplateField HeaderText="Case Id" SortExpression="CaseId">
                <ItemTemplate>
                    <asp:LinkButton ID="lnkRefID" runat="server" Text='<%# Eval("associated_caseId") %>' CommandArgument='<%# Eval("associated_RefId").ToString() + "," + Eval("associated_workflowId").ToString() %>' CommandName="view" Visible="false"/>
                    <asp:Label ID="RefIdlbl" runat="server" Text='<%# Eval("associated_caseId") %>' visible="false"/>
                </Itemtemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</div>

<div id="myDialog2" class="hidden" style="overflow-y: scroll;" >
    <asp:GridView runat="server" ID="GridView2" AutoGenerateColumns="false" Width="98%" AllowSorting="true">
        <Columns>
            <asp:TemplateField HeaderText="Case Id" SortExpression="CaseId">
                <ItemTemplate>
                    <asp:LinkButton ID="lnkRefID" runat="server" Text='<%# Eval("associated_caseId") %>' CommandArgument='<%# Eval("associated_RefId").ToString() + "," + Eval("associated_workflowId").ToString() %>' CommandName="view" Visible="false"/>
                    <asp:Label ID="RefIdlbl" runat="server" Text='<%# Eval("associated_caseId") %>' visible="false"/>
                </Itemtemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</div>


&nbsp; 