<%@ Page Title="PH History" Language="VB" MasterPageFile="~/Secure/SC_PH/SC_PH.master" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.PH.History" Codebehind="History.aspx.vb" %>

<%@ Import Namespace="ALODWebUtility.Common" %>

<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_PH/SC_PH.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" Runat="Server">
    <asp:Panel runat="server" ID="pnlWingPHHistory" CssClass="dataBlock">
        <div class="dataBlock-header">
            <asp:Label runat="server" ID="lblHistoryPanelTitle" />
        </div>

        <div class="dataBlock-body">
            <asp:GridView runat="server" ID="gdvPHHistory" AutoGenerateColumns="false" Width="100%" AllowPaging="true" AllowSorting="false" Visible="false">
                <Columns>
                    <asp:TemplateField HeaderText="Case Id" SortExpression="Case_Id">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkRefSCID" runat="server" CommandArgument='<%# Eval("RefId").ToString() + ";" + Eval("Module_Id").ToString()  %>'
                                CommandName="view" Text='<%# Eval("Case_Id") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Reporting_Period" HeaderText="Reporting Period" SortExpression="Reporting_Period" />
                    <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                    <asp:BoundField DataField="Created_Date" HeaderText="Created Date" SortExpression="Created_Date" />
                    <asp:BoundField DataField="Completed_Date" HeaderText="Completed Date" SortExpression="Completed_Date" />
                </Columns>
                <EmptyDataRowStyle CssClass="emptyItem" />
                <EmptyDataTemplate>
                    No other PH cases exist for this Wing RMU
                </EmptyDataTemplate>
            </asp:GridView>
            <div style="text-align: center;">
                <asp:Label ID="lblNoHistory" Text="No other PH cases found" CssClass="labelNormal" runat="server" Visible="false" />
            </div>
        </div>
    </asp:Panel>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" Runat="Server">
    <script type="text/javascript">

    </script>
</asp:Content>