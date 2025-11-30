<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false"
    Async="true" Inherits="ALOD.Web.Special_Case.IRILO.Secure_ft_MyCases" Codebehind="MyCases.aspx.vb" %>

<%@ Import Namespace="ALODWebUtility.Common" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
   
    <div class="indent">
        <div class="border-thin">
        </div>
        <br />
        <br />
    </div>
    <asp:UpdatePanel ID="resultsUpdatePanel" runat="server" ChildrenAsTriggers="True">
        <ContentTemplate>
            <div id="spSpacer" class="emptyItem" style="margin-top: 0px; margin-bottom: 4px;
                height: 22px;">
                <div id="spWait" class="" style="display: none;">
                    &nbsp;<asp:Image runat="server" ID="imgWait" SkinID="imgBusy" AlternateText="busy"
                        ImageAlign="AbsMiddle" />&nbsp;Loading...
                </div>
            </div>
            <div style="font-weight:bold">My Cases: <%= WorkflowDao.GetCaseType(13, 0)%></div>
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
                    <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                    <asp:TemplateField HeaderText="Special Case Type" SortExpression="ModuleId">
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%#Eval("workflow_title")%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                    <asp:BoundField DataField="PriorityRank" HeaderText="Priority" SortExpression="PriorityRank" />
                </Columns>
            </asp:GridView>
            <br /><br />
            
              <div style="font-weight: bold">
                My Cases: <%= WorkflowDao.GetCaseType(13, 0) %> - Waiting on Disposition Response
            </div>
               <asp:GridView ID="gvDisposition" runat="server" 
                EmptyDataText="No records found" EmptyDataRowStyle-CssClass="emptyItem"
                AutoGenerateColumns="False" PageSize="20" Width="100%" DataSourceID="SCDisposition"
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
                    <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                    <asp:TemplateField HeaderText="Special Case Type" SortExpression="ModuleId">
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%#Eval("workflow_title")%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                    <asp:BoundField DataField="PriorityRank" HeaderText="Priority" SortExpression="PriorityRank" />
                </Columns>
            </asp:GridView>
            <br /><br />

            <div style="font-weight: bold">
                My Cases: <%= WorkflowDao.GetCaseType(13, 0)%> - Holding
            </div>
               <asp:GridView ID="gvHolding" runat="server" 
                EmptyDataText="No records found" EmptyDataRowStyle-CssClass="emptyItem"
                AutoGenerateColumns="False" PageSize="20" Width="100%" DataSourceID="SCDisposition2"
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
                    <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                    <asp:TemplateField HeaderText="Special Case Type" SortExpression="ModuleId">
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%#Eval("workflow_title")%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                    <asp:BoundField DataField="PriorityRank" HeaderText="Priority" SortExpression="PriorityRank" />
                </Columns>
            </asp:GridView>
        </ContentTemplate>
        <Triggers>
        </Triggers>
    </asp:UpdatePanel>
    <asp:ObjectDataSource ID="SCData_Module13"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSpecialCasesNotHolding" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:SessionParameter Name="userId" SessionField="UserId" Type="Int32" />
            <asp:Parameter Name="moduleId" DefaultValue="13" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="SCDisposition" runat="server" OldValuesParameterFormatString="original_{0}"
                          SelectMethod="GetSpecailCaseDisposition" 
                          TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:Parameter Name="moduleId" DefaultValue="13" Type="Int32" />
            <asp:SessionParameter Name="userId" SessionField="UserId" Type="Int32" />
            <%--<asp:Parameter DefaultValue="92" Name="statusId" Type="Int32" />--%>
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="SCDisposition2" runat="server" OldValuesParameterFormatString="original_{0}"
                          SelectMethod="GetSpecialCasesHolding" 
                          TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:Parameter Name="moduleId" DefaultValue="13" Type="Int32" />
            <asp:SessionParameter Name="userId" SessionField="UserId" Type="Int32" />
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
