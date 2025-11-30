<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/Secure/Secure.master" CodeBehind="MyLodConsult.aspx.vb" Inherits="ALOD.Web.LOD.Secure_lod_MyLodConsult" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
   
    <div class="indent">
        <div class="border-thin">
            <div class="searchTitle">
                <asp:Label ID="txtTitle" runat="server" Text="Filter Results" />
            </div>
            <div class="searchBody">
                <p style="text-align: center;">
                    
                    <asp:Label runat="server" AssociatedControlID="SsnBox" Text="SSN:" />
                    <asp:TextBox ID="SsnBox" runat="server" AutoPostBack="True" MaxLength="9" Width="100px"></asp:TextBox>
                    <asp:Label runat="server" AssociatedControlID="NameBox" Text="Name:" />
                    <asp:TextBox ID="NameBox" runat="server" AutoPostBack="True" Width="100px"></asp:TextBox>
                    <asp:Label runat="server" AssociatedControlID="CaseIdbox" Text="Case Id:" />
                    <asp:TextBox ID="CaseIdbox" runat="server" AutoPostBack="True" Width="100px"></asp:TextBox>
                    <asp:Label runat="server" AssociatedControlID="FormalSelect" Text="Formal:" />
                    <asp:DropDownList ID="FormalSelect" runat="server" AutoPostBack="True">
                        <asp:ListItem Value="">-All--</asp:ListItem>
                        <asp:ListItem Value="False">No</asp:ListItem>
                        <asp:ListItem Value="True">Yes</asp:ListItem>
                    </asp:DropDownList>
                    <asp:Label runat="server" AssociatedControlID="UnitSelect" Text="Unit:" />
                    <asp:DropDownList ID="UnitSelect" AutoPostBack="True" runat="server" Width="230px"
                        DataTextField="Name" DataValueField="Value">
                    </asp:DropDownList>
                </p>
                <p style="text-align: right;">
                    <asp:Button runat="server" ID="SearchButton" Text="Search" />&nbsp;&nbsp;
                </p>
            </div>
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
            <asp:GridView ID="gvResults" runat="server" EmptyDataText="No records found" EmptyDataRowStyle-CssClass="emptyItem"
                AutoGenerateColumns="False" PageSize="20" Width="100%" DataSourceID="LodData"
                AllowPaging="True" AllowSorting="True">
                <Columns>
                    
                    <asp:TemplateField HeaderText="Case Id" SortExpression="CaseId" ItemStyle-Width="150px">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("RefId").ToString()  %>'
                                CommandName="view" Text='<%# Eval("CaseId") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                        <ItemStyle Width="150px" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="SSN" HeaderText="SSN" SortExpression="SSN" />
                    <asp:TemplateField HeaderText="Name" ItemStyle-Width="200px" SortExpression="name">
                        <ItemTemplate>
                            <%#Eval("name").ToString().ToUpper()%>
                        </ItemTemplate>
                        <ItemStyle Width="200px" />
                    </asp:TemplateField>
                    <asp:BoundField DataField="UnitName" HeaderText="Unit" SortExpression="UnitName" />
                   
                    <asp:BoundField DataField="State" HeaderText="State" SortExpression="State" />
                    <asp:BoundField DataField="ReceiveDate" HeaderText="Date Received" SortExpression="ReceiveDate" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                    <asp:BoundField DataField="Description" HeaderText="Description" SortExpression="Description" />
                    <asp:BoundField DataField="PriorityRank" HeaderText="Priority" SortExpression="PriorityRank" />
                </Columns>
                <EmptyDataRowStyle CssClass="emptyItem" />
            </asp:GridView>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="SearchButton" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="UnitSelect" EventName="SelectedIndexChanged" />
            <asp:AsyncPostBackTrigger ControlID="FormalSelect" EventName="SelectedIndexChanged" />
            <asp:AsyncPostBackTrigger ControlID="CaseIdBox" EventName="TextChanged" />
            <asp:AsyncPostBackTrigger ControlID="NameBox" EventName="TextChanged" />
            <%--<asp:AsyncPostBackTrigger ControlID="txtLastName" EventName="TextChanged" />
            <asp:AsyncPostBackTrigger ControlID="txtFirstName" EventName="TextChanged" />
            <asp:AsyncPostBackTrigger ControlID="txtMiddleName" EventName="TextChanged" />--%>
            <asp:AsyncPostBackTrigger ControlID="SsnBox" EventName="TextChanged" />
        </Triggers>
    </asp:UpdatePanel>
    <asp:ObjectDataSource ID="LodData"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetByPilotUser" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:Parameter DefaultValue="0" Name="wsId" Type="Int32" />
            <asp:Parameter DefaultValue="0" Name="compo" Type="Int16" />
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
