<%@ Page Title="" Language="C#" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Async="true" Inherits="ALOD.Web.SARC.Secure_sarc_MyCases" CodeBehind="MyCases.aspx.cs" %>

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
            <div style="font-weight:bold">My SARCs:</div>
            <asp:GridView ID="gvResults" runat="server" EmptyDataText="No records found" EmptyDataRowStyle-CssClass="emptyItem"
                AutoGenerateColumns="False" PageSize="20" Width="100%" DataSourceID="SARCData"
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
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("sarc_id").ToString()%>'
                                CommandName="view" Text='<%# Eval("Case_Id") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                        <ItemStyle Width="150px" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="Protected_SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="Member_Name">
                        <ItemTemplate>
                            <%# Eval("Member_Name").ToString().ToUpper()%>
                        </ItemTemplate>
                        <ItemStyle Width="200px" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="Unit_Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:BoundField DataField="Receive_Date" HeaderText="Date Received" SortExpression="Receive_Date" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                    <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                </Columns>
                <EmptyDataRowStyle CssClass="emptyItem" />
            </asp:GridView>
            <asp:SqlDataSource ID="sarc" runat="server"></asp:SqlDataSource>
            <br />
        </ContentTemplate>
        <Triggers>
        </Triggers>
    </asp:UpdatePanel>
    <asp:ObjectDataSource ID="SARCData"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetOpenRestrictedSARCCasesForUser" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
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