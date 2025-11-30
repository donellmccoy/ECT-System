<%@ Control Language="VB" AutoEventWireup="false"
    Inherits="ALOD.Web.UserControls.Secure_Shared_UserControls_CaseHistory" Codebehind="CaseHistory.ascx.vb" %>
<%@ Import Namespace="ALODWebUtility.Common" %>

<asp:Panel ID="HistoryPanelLOD" runat="server" CssClass="dataBlock">
        <div class="dataBlock-header">
            Member LOD Case History
        </div>
        <div class="dataBlock-body">
            <asp:GridView runat="server" ID="HistoryGridLOD" AutoGenerateColumns="false" Width="100%" AllowPaging="true" AllowSorting="true" PageSize="20">
                <Columns>
                    <asp:TemplateField HeaderText="Case Id" SortExpression="CaseId">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("RefId").ToString() + ";" + Eval("ModuleId").ToString() %>'
                                CommandName="view" Text='<%# Eval("CaseId") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="UnitName" HeaderText="Unit" SortExpression="UnitName" />
                    <asp:BoundField DataField="WorkStatus" HeaderText="Status" SortExpression="WorkStatus" />
                    <asp:BoundField DataField="DateCreated" HeaderText="Created" SortExpression="DateCreated" />
                    <asp:TemplateField HeaderText="Completed Date">
                        <ItemTemplate>
                            <asp:Label ID="completeDateLbl" runat="server">
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataRowStyle CssClass="emptyItem" />
                <EmptyDataTemplate>
                    No other active LOD cases exist for service member
                </EmptyDataTemplate>
            </asp:GridView>
            <div style="text-align: center;">
                <asp:Label ID="NoHistoryLODLbl" Text="No other LOD cases found" CssClass="labelNormal" runat="server"> 
                </asp:Label>
            </div>
        </div>
    </asp:Panel>
    <asp:Panel ID="HistoryPanel" runat="server" CssClass="dataBlock">
        <div class="dataBlock-header">
            Member Other/Specialty Case History
        </div>
        <div class="dataBlock-body">
            <asp:GridView runat="server" ID="HistoryGrid" AutoGenerateColumns="false" Width="98%" AllowPaging="true" AllowSorting="true" PageSize="20">
                <Columns>
                    <asp:TemplateField HeaderText="Case Id" SortExpression="Case_Id">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkRefSCID" runat="server" CommandArgument='<%# Eval("RefId").ToString() + ";" + Eval("Module_Id").ToString()  %>'
                                CommandName="view" Text='<%# Eval("Case_Id") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Unit_Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:TemplateField HeaderText="Case Type" SortExpression="Module">
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%# IIf(Eval("workflow_title").ToString = "Worldwide Duty (WD)", "Non Duty Disability Evaluation System (NDDES)", Eval("workflow_title").ToString)%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                    <asp:BoundField DataField="ReceiveDate" HeaderText="Received" SortExpression="ReceiveDate" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                </Columns>
                <EmptyDataRowStyle CssClass="emptyItem" />
                <EmptyDataTemplate>
                    No other Other/Special Cases exist for service member
                </EmptyDataTemplate>
            </asp:GridView>
            <div style="text-align: center;">
                <asp:Label ID="NoHistoryLbl" Text="No other Other/Specialty cases found" CssClass="labelNormal" runat="server"> 
                </asp:Label>
            </div>
        </div>
    </asp:Panel>
    <asp:Panel ID="HistoryPanelPAL" runat="server">
        <div class="formHeader">
            Member PAL Data
        </div>
        <div class="dataTable">
            <asp:GridView ID="HistoryGridPAL" runat="server" AutoGenerateColumns="false" Width="98%">
                <Columns>
                    <asp:BoundField DataField="Field" HeaderText="Field" />
                    <asp:BoundField DataField="Value" HeaderText="Value" />
                </Columns>
            </asp:GridView>
            <div style="text-align: center;">
                <asp:Label ID="NoHistoryPALLbl" Text="No PAL Data found" CssClass="labelNormal" runat="server"> 
                </asp:Label>
            </div>
        </div>
    </asp:Panel>
