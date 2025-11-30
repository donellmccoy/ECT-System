<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false"
    Async="true" Inherits="ALOD.Web.Special_Case.Secure_sc_MyCases" Codebehind="MyCases.aspx.vb" %>

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
            <div style="font-weight:bold">My Cases: <%=WorkflowDao.GetCaseType(6) %></div>
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
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("SC_Id").ToString() + ";6" %>'
                                CommandName="view" Text='<%# Eval("Case_Id") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Protected_SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Member_Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Unit_Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="Receive_Date" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                    <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                        <asp:TemplateField HeaderText="Special Case Type" SortExpression="ModuleId">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%#Eval("workflow_title")%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <br />
            <div style="font-weight:bold">My Cases: <%= WorkflowDao.GetCaseType(7)%></div>
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
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("SC_Id").ToString() + ";7"  %>'
                                CommandName="view" Text='<%# Eval("Case_Id") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Protected_SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Member_Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Unit_Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="Receive_Date" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                    <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                        <asp:TemplateField HeaderText="Special Case Type" SortExpression="ModuleId">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%#Eval("workflow_title")%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <br />
            <div style="font-weight:bold">My Cases: <%= WorkflowDao.GetCaseType(8, 1)%> / <%= WorkflowDao.GetCaseType(8, 2)%></div>
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
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("SC_Id").ToString() + ";8"  %>'
                                CommandName="view" Text='<%# Eval("Case_Id") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Protected_SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Member_Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Unit_Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="Receive_Date" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                    <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                        <asp:TemplateField HeaderText="Special Case Type" SortExpression="ModuleId">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%#Eval("workflow_title")%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <br />
            <div style="font-weight:bold">My Cases: <%= IIf(WorkflowDao.GetCaseType(9, 0).Contains("Worldwide Duty"), "Non Duty Disability Evaluation System (NDDES)", WorkflowDao.GetCaseType(9, 0))%></div>
            <asp:GridView ID="gvResults9" runat="server" EmptyDataText="No records found" EmptyDataRowStyle-CssClass="emptyItem"
                AutoGenerateColumns="False" PageSize="20" Width="100%" DataSourceID="SCData_Module9"
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
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("SC_Id").ToString() + ";9"  %>'
                                CommandName="view" Text='<%# Eval("Case_Id") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Protected_SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Member_Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Unit_Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="Receive_Date" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                    <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                        <asp:TemplateField HeaderText="Special Case Type" SortExpression="ModuleId">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%#IIf(Eval("workflow_title") = "Worldwide Duty (WD)", "Non Duty Disability Evaluation System (NDDES)", Eval("workflow_title"))%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <br />
            <div style="font-weight:bold">My Cases: <%= WorkflowDao.GetCaseType(10)%></div>
            <asp:GridView ID="gvResults10" runat="server" EmptyDataText="No records found" EmptyDataRowStyle-CssClass="emptyItem"
                AutoGenerateColumns="False" PageSize="20" Width="100%" DataSourceID="SCData_Module10"
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
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("SC_Id").ToString() + ";10"  %>'
                                CommandName="view" Text='<%# Eval("Case_Id") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Protected_SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Member_Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Unit_Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="Receive_Date" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                    <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                        <asp:TemplateField HeaderText="Special Case Type" SortExpression="ModuleId">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%#Eval("workflow_title")%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <br />
            <div style="font-weight:bold">My Cases: <%= WorkflowDao.GetCaseType(11)%></div>
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
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("SC_Id").ToString() + ";11"  %>'
                                CommandName="view" Text='<%# Eval("Case_Id") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Protected_SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Member_Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Unit_Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="Receive_Date" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                    <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                        <asp:TemplateField HeaderText="Special Case Type" SortExpression="ModuleId">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%#Eval("workflow_title")%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <br />
            <div style="font-weight:bold">My Cases: <%= WorkflowDao.GetCaseType(12)%></div>
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
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("SC_Id").ToString() + ";12"  %>'
                                CommandName="view" Text='<%# Eval("Case_Id") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Protected_SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Member_Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Unit_Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="Receive_Date" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                    <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                        <asp:TemplateField HeaderText="Special Case Type" SortExpression="ModuleId">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%#Eval("workflow_title")%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <br />
            <div style="font-weight:bold">My Cases: <%= WorkflowDao.GetCaseType(13)%></div>
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
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("SC_Id").ToString() + ";13"  %>'
                                CommandName="view" Text='<%# Eval("Case_Id") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Protected_SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Member_Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Unit_Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="Receive_Date" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                    <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                        <asp:TemplateField HeaderText="Special Case Type" SortExpression="ModuleId">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%#Eval("workflow_title")%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <br />
            <div style="font-weight:bold">My Cases: <%= WorkflowDao.GetCaseType(14)%></div>
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
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("SC_Id").ToString() + ";14"  %>'
                                CommandName="view" Text='<%# Eval("Case_Id") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Protected_SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Member_Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Unit_Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="Receive_Date" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                    <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                        <asp:TemplateField HeaderText="Special Case Type" SortExpression="ModuleId">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%#Eval("workflow_title")%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <br />
            <div id="MMSODiv" runat="server">
            <div style="font-weight:bold">My Cases: <%= WorkflowDao.GetCaseType(16)%></div>
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
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("SC_Id").ToString() + ";16"  %>'
                                CommandName="view" Text='<%# Eval("Case_Id") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Protected_SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Member_Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Unit_Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="Receive_Date" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                    <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                        <asp:TemplateField HeaderText="Special Case Type" SortExpression="ModuleId">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%#Eval("workflow_title")%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <br />
            </div>
                    <div style="font-weight:bold">My Cases: <%= WorkflowDao.GetCaseType(17)%></div>
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
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("SC_Id").ToString() + ";17"  %>'
                                CommandName="view" Text='<%# Eval("Case_Id") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Protected_SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Member_Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Unit_Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="Receive_Date" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                    <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                        <asp:TemplateField HeaderText="Special Case Type" SortExpression="ModuleId">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%#Eval("workflow_title")%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <br />
                    <div style="font-weight:bold">My Cases: <%= WorkflowDao.GetCaseType(19)%></div>
            <asp:GridView ID="gvResults19" runat="server" EmptyDataText="No records found" EmptyDataRowStyle-CssClass="emptyItem"
                AutoGenerateColumns="False" PageSize="20" Width="100%" DataSourceID="SCData_Module19"
                AllowPaging="True" AllowSorting="True" EnableModelValidation="True">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Image runat="server" ID="LockImage" ImageAlign="AbsMiddle" ImageUrl="~/images/lock.gif"
                                Visible="false" AlternateText="This case is locked for editing by another user" />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Case Id" SortExpression="Case_Id" ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("SC_Id").ToString() + ";19"  %>'
                                CommandName="view" Text='<%# Eval("Case_Id") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Protected_SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Member_Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Unit_Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="Receive_Date" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                    <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                        <asp:TemplateField HeaderText="Special Case Type" SortExpression="ModuleId">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%#Eval("workflow_title")%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                </Columns>
            </asp:GridView>


            <br />

            <div style="font-weight:bold">My Cases: <%= WorkflowDao.GetCaseType(18)%></div>
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
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("SC_Id").ToString() + ";18"  %>'
                                CommandName="view" Text='<%# Eval("Case_Id") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Protected_SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Member_Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Unit_Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="Receive_Date" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                    <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                        <asp:TemplateField HeaderText="Special Case Type" SortExpression="ModuleId">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%#Eval("workflow_title")%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <br />

            <div style="font-weight:bold">My Cases: <%= WorkflowDao.GetCaseType(20)%></div>
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
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("SC_Id").ToString() + ";20"  %>'
                                CommandName="view" Text='<%# Eval("Case_Id") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Protected_SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Member_Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Unit_Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="Receive_Date" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                    <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                        <asp:TemplateField HeaderText="Special Case Type" SortExpression="ModuleId">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%#Eval("workflow_title")%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <br />

            <div style="font-weight:bold">My Cases: <%= WorkflowDao.GetCaseType(21, 3)%> / <%= WorkflowDao.GetCaseType(21, 4)%></div>
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
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("SC_Id").ToString() + ";21"  %>'
                                CommandName="view" Text='<%# Eval("Case_Id") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Protected_SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Member_Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Unit_Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="Receive_Date" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                    <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                        <asp:TemplateField HeaderText="Special Case Type" SortExpression="ModuleId">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%#Eval("workflow_title")%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <br />

            <div style="font-weight:bold">My Cases: <%= WorkflowDao.GetCaseType(22)%></div>
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
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("SC_Id").ToString() + ";22"%>'
                                CommandName="view" Text='<%# Eval("Case_Id") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Protected_SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Member_Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Unit_Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="Receive_Date" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                    <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                        <asp:TemplateField HeaderText="Special Case Type" SortExpression="ModuleId">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%#Eval("workflow_title")%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <br />

            <div style="font-weight:bold">My Cases: <%= WorkflowDao.GetCaseType(23)%></div>
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
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("SC_Id").ToString() + ";23"%>'
                                CommandName="view" Text='<%# Eval("Case_Id") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Protected_SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Member_Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Unit_Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="Receive_Date" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                    <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                        <asp:TemplateField HeaderText="Special Case Type" SortExpression="ModuleId">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%#Eval("workflow_title")%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                </Columns>
            </asp:GridView>

            <br />

            <div style="font-weight:bold">My Cases: <%= WorkflowDao.GetCaseType(28)%></div>
            <asp:GridView ID="gvResults28" runat="server" EmptyDataText="No records found" EmptyDataRowStyle-CssClass="emptyItem"
                AutoGenerateColumns="False" PageSize="20" Width="100%" DataSourceID="SCData_Module28"
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
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("SC_Id").ToString() + ";28"%>'
                                CommandName="view" Text='<%# Eval("Case_Id") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Protected_SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Member_Name").ToString().ToUpper()%>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Unit_Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="Receive_Date" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                    <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                        <asp:TemplateField HeaderText="Special Case Type" SortExpression="ModuleId">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%#Eval("workflow_title")%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                </Columns>
            </asp:GridView>

        </ContentTemplate>
        <Triggers>
        </Triggers>
    </asp:UpdatePanel>
    <asp:ObjectDataSource ID="SCData_Module6"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialCasesByModule" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="userId" SessionField="UserId" Type="Int32" />
            <asp:Parameter Name="moduleId" DefaultValue="6" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="SCData_Module7"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialCasesByModule" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="userId" SessionField="UserId" Type="Int32" />
            <asp:Parameter Name="moduleId" DefaultValue="7" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="SCData_Module8"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialCasesByModule" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="userId" SessionField="UserId" Type="Int32" />
            <asp:Parameter Name="moduleId" DefaultValue="8" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="SCData_Module9"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialCasesByModule" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="userId" SessionField="UserId" Type="Int32" />
            <asp:Parameter Name="moduleId" DefaultValue="9" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="SCData_Module10"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialCasesByModule" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="userId" SessionField="UserId" Type="Int32" />
            <asp:Parameter Name="moduleId" DefaultValue="10" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="SCData_Module11"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialCasesByModule" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="userId" SessionField="UserId" Type="Int32" />
            <asp:Parameter Name="moduleId" DefaultValue="11" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="SCData_Module12"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialCasesByModule" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="userId" SessionField="UserId" Type="Int32" />
            <asp:Parameter Name="moduleId" DefaultValue="12" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="SCData_Module13"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialCasesByModule" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="userId" SessionField="UserId" Type="Int32" />
            <asp:Parameter Name="moduleId" DefaultValue="13" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="SCData_Module14"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialCasesByModule" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="userId" SessionField="UserId" Type="Int32" />
            <asp:Parameter Name="moduleId" DefaultValue="14" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="SCData_Module16"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialCasesByModule" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="userId" SessionField="UserId" Type="Int32" />
            <asp:Parameter Name="moduleId" DefaultValue="16" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="SCData_Module17"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialCasesByModule" 
        TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="userId" SessionField="UserId" Type="Int32" />
            <asp:Parameter Name="moduleId" DefaultValue="17" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>


        <asp:ObjectDataSource ID="SCData_Module19"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialCasesByModule" 
        TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="userId" SessionField="UserId" Type="Int32" />
            <asp:Parameter Name="moduleId" DefaultValue="19" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>


    <asp:ObjectDataSource ID="SCData_Module18"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialCasesByModule" 
        TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="userId" SessionField="UserId" Type="Int32" />
            <asp:Parameter Name="moduleId" DefaultValue="18" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="SCData_Module20"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialCasesByModule" 
        TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="userId" SessionField="UserId" Type="Int32" />
            <asp:Parameter Name="moduleId" DefaultValue="20" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="SCData_Module21"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialCasesByModule" 
        TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="userId" SessionField="UserId" Type="Int32" />
            <asp:Parameter Name="moduleId" DefaultValue="21" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="SCData_Module22"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialCasesByModule" 
        TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="userId" SessionField="UserId" Type="Int32" />
            <asp:Parameter Name="moduleId" DefaultValue="22" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="SCData_Module23"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialCasesByModule" 
        TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="userId" SessionField="UserId" Type="Int32" />
            <asp:Parameter Name="moduleId" DefaultValue="23" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="SCData_Module28"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialCasesByModule" 
        TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="userId" SessionField="UserId" Type="Int32" />
            <asp:Parameter Name="moduleId" DefaultValue="28" Type="Int32" />
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
