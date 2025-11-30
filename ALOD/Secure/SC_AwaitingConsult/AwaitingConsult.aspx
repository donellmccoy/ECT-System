<%@ Page Language="vb" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Async="true" CodeBehind="AwaitingConsult.aspx.vb" Inherits="ALOD.AwaitingConsult" %>

<%@ Import Namespace="ALODWebUtility.Common" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
   
    <div class="indent">
       <%-- <div class="border-thin"></div>--%>

    </div>
    <asp:UpdatePanel ID="resultsUpdatePanel" runat="server" ChildrenAsTriggers="True">
        <ContentTemplate>
<%--            <div id="spSpacer" class="emptyItem" style="margin-top: 0px; margin-bottom: 4px;
                height: 22px;">--%>
                <div id="spWait" class="" style="display: none;">
                    &nbsp;<asp:Image runat="server" ID="imgWait" SkinID="imgBusy" AlternateText="busy"
                        ImageAlign="AbsMiddle" />&nbsp;Loading...
                </div>
          <%--  </div>--%>
            <asp:Panel runat="server" ID="Panel6">
            <div style="font-weight:bold">  <%=WorkflowDao.GetStatusDescription(6) %></div>
            <asp:GridView ID="gvResults6" runat="server" EmptyDataText="No records found" EmptyDataRowStyle-CssClass="emptyItem"
                AutoGenerateColumns="False" PageSize="20" Width="100%" DataSourceID="SCData_Module6"
                AllowPaging="True" AllowSorting="True">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Image runat="server" ID="LockImage" ImageAlign="AbsMiddle" ImageUrl="~/images/lock.gif"
                                Visible="false" AlternateText="This case is locked for editing by another user" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Case Id" SortExpression="Case_Id" ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("CaseId").ToString() + ";6" %>'
                                CommandName="view" Text='<%# Eval("CaseId") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="ReceiveDate" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                </Columns>
            </asp:GridView>
            <br />
            </asp:Panel>
            <asp:Panel runat="server" ID="Panel7">
            <div style="font-weight:bold">  <%=WorkflowDao.GetStatusDescription(7) %></div>
            <asp:GridView ID="gvResults7" runat="server" EmptyDataText="No records found" EmptyDataRowStyle-CssClass="emptyItem"
                AutoGenerateColumns="False" PageSize="20" Width="100%" DataSourceID="SCData_Module7"
                AllowPaging="True" AllowSorting="True">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Image runat="server" ID="LockImage" ImageAlign="AbsMiddle" ImageUrl="~/images/lock.gif"
                                Visible="false" AlternateText="This case is locked for editing by another user" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Case Id" SortExpression="Case_Id" ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("CaseId").ToString() + ";6" %>'
                                CommandName="view" Text='<%# Eval("CaseId") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="ReceiveDate" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                </Columns>
            </asp:GridView>
            <br />
            </asp:Panel>
            <asp:Panel runat="server" ID="Panel8">
            <div style="font-weight:bold">  <%=WorkflowDao.GetStatusDescription(8) %></div>
            <asp:GridView ID="gvResults8" runat="server" EmptyDataText="No records found" EmptyDataRowStyle-CssClass="emptyItem"
                AutoGenerateColumns="False" PageSize="20" Width="100%" DataSourceID="SCData_Module8"
                AllowPaging="True" AllowSorting="True">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Image runat="server" ID="LockImage" ImageAlign="AbsMiddle" ImageUrl="~/images/lock.gif"
                                Visible="false" AlternateText="This case is locked for editing by another user" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Case Id" SortExpression="Case_Id" ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("CaseId").ToString() + ";6" %>'
                                CommandName="view" Text='<%# Eval("CaseId") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="ReceiveDate" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                </Columns>
            </asp:GridView>
            <br />           
            </asp:Panel>
            <asp:Panel runat="server" ID="Panel11">
            <div style="font-weight:bold">  <%=WorkflowDao.GetStatusDescription(11) %></div>
            <asp:GridView ID="gvResults11" runat="server" EmptyDataText="No records found" EmptyDataRowStyle-CssClass="emptyItem"
                AutoGenerateColumns="False" PageSize="20" Width="100%" DataSourceID="SCData_Module11"
                AllowPaging="True" AllowSorting="True">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Image runat="server" ID="LockImage" ImageAlign="AbsMiddle" ImageUrl="~/images/lock.gif"
                                Visible="false" AlternateText="This case is locked for editing by another user" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Case Id" SortExpression="Case_Id" ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("CaseId").ToString() + ";6" %>'
                                CommandName="view" Text='<%# Eval("CaseId") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="ReceiveDate" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                </Columns>
            </asp:GridView>
            <br />
            </asp:Panel>
            <asp:Panel runat="server" ID="Panel12">
            <div style="font-weight:bold">  <%=WorkflowDao.GetStatusDescription(12) %></div>
            <asp:GridView ID="gvResults12" runat="server" EmptyDataText="No records found" EmptyDataRowStyle-CssClass="emptyItem"
                AutoGenerateColumns="False" PageSize="20" Width="100%" DataSourceID="SCData_Module12"
                AllowPaging="True" AllowSorting="True">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Image runat="server" ID="LockImage" ImageAlign="AbsMiddle" ImageUrl="~/images/lock.gif"
                                Visible="false" AlternateText="This case is locked for editing by another user" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Case Id" SortExpression="Case_Id" ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("CaseId").ToString() + ";6" %>'
                                CommandName="view" Text='<%# Eval("CaseId") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="ReceiveDate" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                </Columns>
            </asp:GridView>
            <br />
            </asp:Panel>
            <asp:Panel runat="server" ID="Panel13">
            <div style="font-weight:bold">  <%=WorkflowDao.GetStatusDescription(13) %></div>
            <asp:GridView ID="gvResults13" runat="server" EmptyDataText="No records found" EmptyDataRowStyle-CssClass="emptyItem"
                AutoGenerateColumns="False" PageSize="20" Width="100%" DataSourceID="SCData_Module13"
                AllowPaging="True" AllowSorting="True">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Image runat="server" ID="LockImage" ImageAlign="AbsMiddle" ImageUrl="~/images/lock.gif"
                                Visible="false" AlternateText="This case is locked for editing by another user" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Case Id" SortExpression="Case_Id" ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("CaseId").ToString() + ";6" %>'
                                CommandName="view" Text='<%# Eval("CaseId") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="ReceiveDate" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                </Columns>
            </asp:GridView>
            <br />
            </asp:Panel>
            <asp:Panel runat="server" ID="Panel14">
            <div style="font-weight:bold">  <%=WorkflowDao.GetStatusDescription(14) %></div>
            <asp:GridView ID="gvResults14" runat="server" EmptyDataText="No records found" EmptyDataRowStyle-CssClass="emptyItem"
                AutoGenerateColumns="False" PageSize="20" Width="100%" DataSourceID="SCData_Module14"
                AllowPaging="True" AllowSorting="True">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Image runat="server" ID="LockImage" ImageAlign="AbsMiddle" ImageUrl="~/images/lock.gif"
                                Visible="false" AlternateText="This case is locked for editing by another user" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Case Id" SortExpression="Case_Id" ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("CaseId").ToString() + ";6" %>'
                                CommandName="view" Text='<%# Eval("CaseId") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="ReceiveDate" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                </Columns>
            </asp:GridView>
            <br />
            </asp:Panel>
            <asp:Panel runat="server" ID="Panel15">
            <div style="font-weight:bold">  <%=WorkflowDao.GetStatusDescription(15) %></div>
            <asp:GridView ID="gvResults15" runat="server" EmptyDataText="No records found" EmptyDataRowStyle-CssClass="emptyItem"
                AutoGenerateColumns="False" PageSize="20" Width="100%" DataSourceID="SCData_Module15"
                AllowPaging="True" AllowSorting="True">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Image runat="server" ID="LockImage" ImageAlign="AbsMiddle" ImageUrl="~/images/lock.gif"
                                Visible="false" AlternateText="This case is locked for editing by another user" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Case Id" SortExpression="Case_Id" ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("CaseId").ToString() + ";6" %>'
                                CommandName="view" Text='<%# Eval("CaseId") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="ReceiveDate" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                </Columns>
            </asp:GridView>
            <br />
            </asp:Panel>
            <asp:Panel runat="server" ID="Panel16">
            <div style="font-weight:bold">  <%=WorkflowDao.GetStatusDescription(16) %></div>
            <asp:GridView ID="gvResults16" runat="server" EmptyDataText="No records found" EmptyDataRowStyle-CssClass="emptyItem"
                AutoGenerateColumns="False" PageSize="20" Width="100%" DataSourceID="SCData_Module16"
                AllowPaging="True" AllowSorting="True">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Image runat="server" ID="LockImage" ImageAlign="AbsMiddle" ImageUrl="~/images/lock.gif"
                                Visible="false" AlternateText="This case is locked for editing by another user" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Case Id" SortExpression="Case_Id" ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("CaseId").ToString() + ";6" %>'
                                CommandName="view" Text='<%# Eval("CaseId") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="ReceiveDate" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                </Columns>
            </asp:GridView>
            <br />
            </asp:Panel>
            <asp:Panel runat="server" ID="Panel17">
            <div style="font-weight:bold">  <%=WorkflowDao.GetStatusDescription(17) %></div>
            <asp:GridView ID="gvResults17" runat="server" EmptyDataText="No records found" EmptyDataRowStyle-CssClass="emptyItem"
                AutoGenerateColumns="False" PageSize="20" Width="100%" DataSourceID="SCData_Module17"
                AllowPaging="True" AllowSorting="True">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Image runat="server" ID="LockImage" ImageAlign="AbsMiddle" ImageUrl="~/images/lock.gif"
                                Visible="false" AlternateText="This case is locked for editing by another user" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Case Id" SortExpression="Case_Id" ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("CaseId").ToString() + ";6" %>'
                                CommandName="view" Text='<%# Eval("CaseId") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="ReceiveDate" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                </Columns>
            </asp:GridView>
            <br />
            </asp:Panel>
            <asp:Panel runat="server" ID="Panel18">
            <div style="font-weight:bold">  <%=WorkflowDao.GetStatusDescription(18) %></div>
            <asp:GridView ID="gvResults18" runat="server" EmptyDataText="No records found" EmptyDataRowStyle-CssClass="emptyItem"
                AutoGenerateColumns="False" PageSize="20" Width="100%" DataSourceID="SCData_Module18"
                AllowPaging="True" AllowSorting="True">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Image runat="server" ID="LockImage" ImageAlign="AbsMiddle" ImageUrl="~/images/lock.gif"
                                Visible="false" AlternateText="This case is locked for editing by another user" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Case Id" SortExpression="Case_Id" ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("CaseId").ToString() + ";6" %>'
                                CommandName="view" Text='<%# Eval("CaseId") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="ReceiveDate" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                </Columns>
            </asp:GridView>
            <br />
            </asp:Panel>
            <asp:Panel runat="server" ID="Panel19">
            <div style="font-weight:bold">  <%=WorkflowDao.GetStatusDescription(19) %></div>
            <asp:GridView ID="gvResults19" runat="server" EmptyDataText="No records found" EmptyDataRowStyle-CssClass="emptyItem"
                AutoGenerateColumns="False" PageSize="20" Width="100%" DataSourceID="SCData_Module19"
                AllowPaging="True" AllowSorting="True">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Image runat="server" ID="LockImage" ImageAlign="AbsMiddle" ImageUrl="~/images/lock.gif"
                                Visible="false" AlternateText="This case is locked for editing by another user" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Case Id" SortExpression="Case_Id" ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("CaseId").ToString() + ";6" %>'
                                CommandName="view" Text='<%# Eval("CaseId") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="ReceiveDate" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                </Columns>
            </asp:GridView>
            <br />
            </asp:Panel>
            <asp:Panel runat="server" ID="Panel20">
            <div style="font-weight:bold">  <%=WorkflowDao.GetStatusDescription(20) %></div>
            <asp:GridView ID="gvResults20" runat="server" EmptyDataText="No records found" EmptyDataRowStyle-CssClass="emptyItem"
                AutoGenerateColumns="False" PageSize="20" Width="100%" DataSourceID="SCData_Module20"
                AllowPaging="True" AllowSorting="True">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Image runat="server" ID="LockImage" ImageAlign="AbsMiddle" ImageUrl="~/images/lock.gif"
                                Visible="false" AlternateText="This case is locked for editing by another user" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Case Id" SortExpression="Case_Id" ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("CaseId").ToString() + ";6" %>'
                                CommandName="view" Text='<%# Eval("CaseId") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="ReceiveDate" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                </Columns>
            </asp:GridView>
            <br />
            </asp:Panel>
            <asp:Panel runat="server" ID="Panel21">
            <div style="font-weight:bold">  <%=WorkflowDao.GetStatusDescription(21) %></div>
            <asp:GridView ID="gvResults21" runat="server" EmptyDataText="No records found" EmptyDataRowStyle-CssClass="emptyItem"
                AutoGenerateColumns="False" PageSize="20" Width="100%" DataSourceID="SCData_Module21"
                AllowPaging="True" AllowSorting="True">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Image runat="server" ID="LockImage" ImageAlign="AbsMiddle" ImageUrl="~/images/lock.gif"
                                Visible="false" AlternateText="This case is locked for editing by another user" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Case Id" SortExpression="Case_Id" ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("CaseId").ToString() + ";6" %>'
                                CommandName="view" Text='<%# Eval("CaseId") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="ReceiveDate" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                </Columns>
            </asp:GridView>
            <br />
            </asp:Panel>
            <asp:Panel runat="server" ID="Panel22">
            <div style="font-weight:bold">  <%=WorkflowDao.GetStatusDescription(22) %></div>
            <asp:GridView ID="gvResults22" runat="server" EmptyDataText="No records found" EmptyDataRowStyle-CssClass="emptyItem"
                AutoGenerateColumns="False" PageSize="20" Width="100%" DataSourceID="SCData_Module22"
                AllowPaging="True" AllowSorting="True">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Image runat="server" ID="LockImage" ImageAlign="AbsMiddle" ImageUrl="~/images/lock.gif"
                                Visible="false" AlternateText="This case is locked for editing by another user" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Case Id" SortExpression="Case_Id" ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("CaseId").ToString() + ";6" %>'
                                CommandName="view" Text='<%# Eval("CaseId") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="ReceiveDate" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                </Columns>
            </asp:GridView>
            <br />
            </asp:Panel>
            <asp:Panel runat="server" ID="Panel23">
            <div style="font-weight:bold">  <%=WorkflowDao.GetStatusDescription(23) %></div>
            <asp:GridView ID="gvResults23" runat="server" EmptyDataText="No records found" EmptyDataRowStyle-CssClass="emptyItem"
                AutoGenerateColumns="False" PageSize="20" Width="100%" DataSourceID="SCData_Module23"
                AllowPaging="True" AllowSorting="True">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Image runat="server" ID="LockImage" ImageAlign="AbsMiddle" ImageUrl="~/images/lock.gif"
                                Visible="false" AlternateText="This case is locked for editing by another user" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Case Id" SortExpression="Case_Id" ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("CaseId").ToString() + ";6" %>'
                                CommandName="view" Text='<%# Eval("CaseId") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="ReceiveDate" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                </Columns>
            </asp:GridView>
            <br />
            </asp:Panel>
            <asp:Panel runat="server" ID="Panel24">
            <div style="font-weight:bold">  <%=WorkflowDao.GetStatusDescription(24) %></div>
            <asp:GridView ID="gvResults24" runat="server" EmptyDataText="No records found" EmptyDataRowStyle-CssClass="emptyItem"
                AutoGenerateColumns="False" PageSize="20" Width="100%" DataSourceID="SCData_Module24"
                AllowPaging="True" AllowSorting="True">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Image runat="server" ID="LockImage" ImageAlign="AbsMiddle" ImageUrl="~/images/lock.gif"
                                Visible="false" AlternateText="This case is locked for editing by another user" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Case Id" SortExpression="Case_Id" ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("CaseId").ToString() + ";6" %>'
                                CommandName="view" Text='<%# Eval("CaseId") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="ReceiveDate" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                </Columns>
            </asp:GridView>
            <br />
            </asp:Panel>
            <asp:Panel runat="server" ID="Panel25">
            <div style="font-weight:bold">  <%=WorkflowDao.GetStatusDescription(25) %></div>
            <asp:GridView ID="gvResults25" runat="server" EmptyDataText="No records found" EmptyDataRowStyle-CssClass="emptyItem"
                AutoGenerateColumns="False" PageSize="20" Width="100%" DataSourceID="SCData_Module25"
                AllowPaging="True" AllowSorting="True">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Image runat="server" ID="LockImage" ImageAlign="AbsMiddle" ImageUrl="~/images/lock.gif"
                                Visible="false" AlternateText="This case is locked for editing by another user" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Case Id" SortExpression="Case_Id" ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("CaseId").ToString() + ";6" %>'
                                CommandName="view" Text='<%# Eval("CaseId") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="ReceiveDate" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                </Columns>
            </asp:GridView>
            <br />
            </asp:Panel>
            <asp:Panel runat="server" ID="Panel30">
            <div style="font-weight:bold">  <%=WorkflowDao.GetStatusDescription(30) %></div>
            <asp:GridView ID="gvResults30" runat="server" EmptyDataText="No records found" EmptyDataRowStyle-CssClass="emptyItem"
                AutoGenerateColumns="False" PageSize="20" Width="100%" DataSourceID="SCData_Module30"
                AllowPaging="True" AllowSorting="True">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Image runat="server" ID="LockImage" ImageAlign="AbsMiddle" ImageUrl="~/images/lock.gif"
                                Visible="false" AlternateText="This case is locked for editing by another user" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Case Id" SortExpression="Case_Id" ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("CaseId").ToString() + ";6" %>'
                                CommandName="view" Text='<%# Eval("CaseId") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="ReceiveDate" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                </Columns>
            </asp:GridView>
            <br />
            </asp:Panel>
            <asp:Panel runat="server" ID="Panel31">
            <div style="font-weight:bold">  <%=WorkflowDao.GetStatusDescription(31) %></div>
            <asp:GridView ID="gvResults31" runat="server" EmptyDataText="No records found" EmptyDataRowStyle-CssClass="emptyItem"
                AutoGenerateColumns="False" PageSize="20" Width="100%" DataSourceID="SCData_Module31"
                AllowPaging="True" AllowSorting="True">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Image runat="server" ID="LockImage" ImageAlign="AbsMiddle" ImageUrl="~/images/lock.gif"
                                Visible="false" AlternateText="This case is locked for editing by another user" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Case Id" SortExpression="Case_Id" ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("CaseId").ToString() + ";6" %>'
                                CommandName="view" Text='<%# Eval("CaseId") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="ReceiveDate" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                </Columns>
            </asp:GridView>
            <br />
            </asp:Panel>
            <asp:Panel runat="server" ID="Panel32">
            <div style="font-weight:bold">  <%=WorkflowDao.GetStatusDescription(32) %></div>
            <asp:GridView ID="gvResults32" runat="server" EmptyDataText="No records found" EmptyDataRowStyle-CssClass="emptyItem"
                AutoGenerateColumns="False" PageSize="20" Width="100%" DataSourceID="SCData_Module32"
                AllowPaging="True" AllowSorting="True">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Image runat="server" ID="LockImage" ImageAlign="AbsMiddle" ImageUrl="~/images/lock.gif"
                                Visible="false" AlternateText="This case is locked for editing by another user" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Case Id" SortExpression="Case_Id" ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("CaseId").ToString() + ";6" %>'
                                CommandName="view" Text='<%# Eval("CaseId") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="ReceiveDate" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                </Columns>
            </asp:GridView>
            <br />
            </asp:Panel>
        </ContentTemplate>
        <Triggers>
        </Triggers>
    </asp:UpdatePanel>
    <asp:ObjectDataSource ID="SCData_Module6"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialAwaitingConsult" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="wsId" DefaultValue="15" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="SCData_Module7"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialAwaitingConsult" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="wsId" DefaultValue="30" Type="Int32" />
            
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="SCData_Module8"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialAwaitingConsult" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="wsId" DefaultValue="11" Type="Int32" />
            
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="SCData_Module9"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialAwaitingConsult" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="wsId" DefaultValue="31" Type="Int32" />
            
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="SCData_Module10"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialAwaitingConsult" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="wsId" DefaultValue="13" Type="Int32" />
            
        </SelectParameters>
    </asp:ObjectDataSource>


    <asp:ObjectDataSource ID="SCData_Module11"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialAwaitingConsult" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="wsId" DefaultValue="6" Type="Int32" />
            
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="SCData_Module12"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialAwaitingConsult" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="wsId" DefaultValue="7" Type="Int32" />
            
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="SCData_Module13"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialAwaitingConsult" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="wsId" DefaultValue="8" Type="Int32" />
            
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="SCData_Module14"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialAwaitingConsult" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="wsId" DefaultValue="12" Type="Int32" />
            
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="SCData_Module15"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialAwaitingConsult" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="wsId" DefaultValue="14" Type="Int32" />
            
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="SCData_Module16"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialAwaitingConsult" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="wsId" DefaultValue="16" Type="Int32" />
            
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="SCData_Module17"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialAwaitingConsult" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="wsId" DefaultValue="17" Type="Int32" />
            
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="SCData_Module18"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialAwaitingConsult" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="wsId" DefaultValue="19" Type="Int32" />
            
        </SelectParameters>
    </asp:ObjectDataSource>

        <asp:ObjectDataSource ID="SCData_Module19"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialAwaitingConsult" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="wsId" DefaultValue="19" Type="Int32" />
            
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="SCData_Module20"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialAwaitingConsult" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="wsId" DefaultValue="20" Type="Int32" />
            
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="SCData_Module21"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialAwaitingConsult" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="wsId" DefaultValue="21" Type="Int32" />
            
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="SCData_Module22"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialAwaitingConsult" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="wsId" DefaultValue="22" Type="Int32" />
            
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="SCData_Module23"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialAwaitingConsult" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="wsId" DefaultValue="23" Type="Int32" />
            
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="SCData_Module24"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialAwaitingConsult" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="wsId" DefaultValue="24" Type="Int32" />
            
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="SCData_Module25"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialAwaitingConsult" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="wsId" DefaultValue="25" Type="Int32" />
            
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="SCData_Module30"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialAwaitingConsult" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="wsId" DefaultValue="32" Type="Int32" />
            
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="SCData_Module31"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialAwaitingConsult" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="wsId" DefaultValue="352" Type="Int32" />
            
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="SCData_Module32"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialAwaitingConsult" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="wsId" DefaultValue="352" Type="Int32" />
            
        </SelectParameters>
    </asp:ObjectDataSource>

    <ajax:UpdatePanelAnimationExtender ID="resultsUpdatePanelAnimationExtender" runat="server"
        TargetControlID="resultsUpdatePanel">
        <Animations>
                <OnUpdating>
                    <ScriptAction script="searchStart();" />
                </OnUpdating>  
                <OnUpdated>     
                    <ScriptAction script="searchEnd();" />
                </OnUpdated>           
        </Animations>
    </ajax:UpdatePanelAnimationExtender>
</asp:Content>
