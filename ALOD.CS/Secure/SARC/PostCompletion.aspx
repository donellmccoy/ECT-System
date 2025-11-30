<%@ Page Title="" Language="C#" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.SARC.PostCompletion" CodeBehind="PostCompletion.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <div class="indent">
        <div class="border-thin">
            <div class="searchTitle">
                <asp:Label ID="txtTitle" runat="server" Text="Filter Results" />
            </div>
            <div class="searchBody">
                <table style="width:100%;">
                    <tr>
                        <td style="text-align:center;">
                            <div style="margin-bottom:10px;">
                                <asp:Label runat="server" AssociatedControlID="SsnBox" Text="SSN:" />
                                <asp:TextBox ID="SsnBox" runat="server" AutoPostBack="true" MaxLength="9" Width="100px"></asp:TextBox>
                                <asp:Label runat="server" AssociatedControlID="NameBox" Text="Name:" />
                                <asp:TextBox ID="NameBox" runat="server" AutoPostBack="true" Width="100px"></asp:TextBox>
                                <asp:Label runat="server" AssociatedControlID="CaseIdbox" Text="Case Id:" />
                                <asp:TextBox ID="CaseIdbox" runat="server" AutoPostBack="true" Width="100px"></asp:TextBox>
                            </div>
                            <div>
                                <asp:Label runat="server" AssociatedControlID="UnitSelect" Text="Unit:" />
                                <asp:DropDownList ID="UnitSelect" AutoPostBack="True" runat="server" Width="230px" DataTextField="Name" DataValueField="Value" />
                            </div>
                        </td>
                    </tr>
                </table>
                <p style="text-align: right;">
                    <asp:Button runat="server" ID="SearchButton" Text="Search" />&nbsp;&nbsp;
                </p>
            </div>
        </div>
        <br />
        <br />
        <asp:UpdatePanel ID="resultsUpdatePanel" runat="server" ChildrenAsTriggers="True">
            <ContentTemplate>
                <asp:Panel runat="server" ID="NoFiltersPanel" Visible="false" CssClass="emptyItem">
                    Please enter at least one criteria (SSN or Name or CaseId).
                </asp:Panel>
                <asp:GridView ID="gvResults" runat="server" AutoGenerateColumns="False" PageSize="10" Width="100%" DataSourceID="SARCData" AllowPaging="True" AllowSorting="True">
                    <Columns>
                        <asp:TemplateField HeaderText="Case Id" SortExpression="CaseId">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("RefId").ToString() + "|" + Eval("ModuleId").ToString() %>' CommandName="view" Text='<%# Eval("CaseId") %>'>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="MemberSSN" HeaderText="SSN" SortExpression="MemberSSN" />
                        <asp:BoundField DataField="MemberName" HeaderText="Name" SortExpression="MemberName" />
                        <asp:BoundField DataField="MemberUnit" HeaderText="Unit" SortExpression="MemberUnit" />
                        <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                        <asp:BoundField DataField="DaysCompleted" HeaderText="Days Completed" SortExpression="DaysCompleted" />
                    </Columns>
                </asp:GridView>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="SearchButton" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="UnitSelect" EventName="SelectedIndexChanged" />
                <asp:AsyncPostBackTrigger ControlID="CaseIdBox" EventName="TextChanged" />
                <asp:AsyncPostBackTrigger ControlID="NameBox" EventName="TextChanged" />
                <asp:AsyncPostBackTrigger ControlID="SsnBox" EventName="TextChanged" />
            </Triggers>
        </asp:UpdatePanel>
    </div>

    <asp:ObjectDataSource ID="SARCData" runat="server" OldValuesParameterFormatString="original_{0}" SelectMethod="GetPostCompletionSearchResults" TypeName="ALOD.Data.Services.SARCService">
        <SelectParameters>
            <asp:SessionParameter Name="userId" SessionField="UserId" Type="Int32" />
            <asp:ControlParameter ControlID="CaseIdbox" Name="caseID" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="SsnBox" Name="memberSSN" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="NameBox" Name="memberName" PropertyName="Text" Type="String" />
            <asp:SessionParameter Name="reportView" SessionField="ReportView" Type="Byte" />
            <asp:SessionParameter Name="compo" SessionField="Compo" Type="String" />
            <asp:ControlParameter ControlID="UnitSelect" Name="unitId" PropertyName="SelectedValue" Type="Int32" DefaultValue="0" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <ajax:UpdatePanelAnimationExtender ID="resultsUpdatePanelAnimationExtender" runat="server" TargetControlID="resultsUpdatePanel">
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
