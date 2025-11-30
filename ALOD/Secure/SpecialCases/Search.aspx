<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.Secure_sc_Search"  Async="true" Codebehind="Search.aspx.vb" %>

<%@ Import Namespace="ALODWebUtility.Common" %>

<asp:Content runat="Server" ID="Content2" ContentPlaceHolderID="ContentFooter">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <div class="indent">
        <div class="border-thin">
            <div class="searchTitle">
                Search Criteria
            </div>

            <div class="searchBody">
                <p style="text-align: center;">
                    <asp:Label runat="server" AssociatedControlID="SsnBox" Text="SSN:" />
                    <asp:TextBox ID="SsnBox" runat="server" AutoPostBack="True" MaxLength="9" Width="130px"/>
                    <asp:Label runat="server" AssociatedControlID="NameBox" Text="Name:" />
                    <asp:TextBox ID="NameBox" runat="server" AutoPostBack="True" Width="130px"/>
                    <asp:Label runat="server" AssociatedControlID="CaseIdBox" Text="Case Id:" />
                    <asp:TextBox ID="CaseIdBox" runat="server" AutoPostBack="True" Width="130px"></asp:TextBox>
                    <br />
                    <br />
                    <asp:Label runat="server" AssociatedControlID="StatusSelect" Text="Status:" />
                    <asp:DropDownList ID="StatusSelect" AutoPostBack="True" DataSourceID="StatusData" DataTextField="Description" DataValueField="Id" runat="server" Width="240px"/>
                    <asp:Label runat="server" AssociatedControlID="UnitSelect" Text="Unit:" />
                    <asp:DropDownList ID="UnitSelect" AutoPostBack="True" runat="server" DataTextField="Name" DataValueField="Value"/>
                    <br />
                    <br />
                    <asp:Label runat="server" AssociatedControlID="ddlModule" Text="Other/Specialty Cases Type:" />
                    <asp:DropDownList ID="ddlModule" runat="server" AutoPostBack="True"/>
                </p>
                <p style="text-align: right;">
                    <asp:Button ID="SearchButton" runat="server" CausesValidation="False" Text="Search" />
                    &nbsp;
                </p>
            </div>
        </div>

        <asp:UpdatePanel ID="resultsUpdatePanel" runat="server" ChildrenAsTriggers="True">
            <ContentTemplate>
                 <asp:Panel runat="server" ID="NoFiltersPanel" Visible="false" CssClass="emptyItem">
                    Please enter at least one criteria to search on
                  </asp:Panel><br />

                <div id="spSpacer" class="emptyItem" style="margin-top: 0px; margin-bottom: 4px;">
                    <div id="spWait" class="" style="display: none;">
                        &nbsp;<asp:Image runat="server" ID="imgWait" SkinID="imgBusy" AlternateText="busy"
                            ImageAlign="AbsMiddle" />&nbsp;Loading...
                    </div>
                </div>

                <asp:GridView ID="ResultGrid" runat="server" AutoGenerateColumns="False" PageSize="20"
                    Width="100%" DataSourceID="SearchData" AllowPaging="True" 
                    AllowSorting="True" EnableModelValidation="True">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:Image runat="server" ID="LockImage" ImageAlign="AbsMiddle" ImageUrl="~/images/lock.gif"
                                    Visible="false" AlternateText="This case is locked for editing by another user" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Case Id" SortExpression="CaseId">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("RefId").ToString() + ";" + Eval("ModuleId").ToString()  %>'
                                    CommandName="view" Text='<%# Eval("CaseId") %>' >
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="SSN" HeaderText="SSN" SortExpression="SSN" />
                        <asp:TemplateField HeaderText="Name" SortExpression="name">
                            <ItemTemplate>
                                <%#Eval("name").ToString().ToUpper()%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="UnitName" HeaderText="Unit" SortExpression="UnitName" />
                        <asp:TemplateField HeaderText="Other/Specialty Case Type" SortExpression="ModuleId">
                            <ItemTemplate>
                                <asp:Label ID="Label1" runat="server" Text='<%#IIf(Eval("workflow_title") = "Worldwide Duty (WD)", "Non Duty Disability Evaluation System (NDDES)", Eval("workflow_title"))%>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="WorkStatus" HeaderText="Status" SortExpression="WorkStatus" />
                        <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                        <asp:TemplateField HeaderText="Print">
                            <ItemTemplate>
                                <asp:ImageButton ImageAlign="AbsMiddle" runat="server" ID="PrintImage" AlternateText="Print Forms"
                                    ImageUrl="~/images/pdf.ico" OnClientClick='<%# Eval("PrintId", "printForms({0}); return false;") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataRowStyle CssClass="emptyItem" />
                </asp:GridView>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="SearchButton" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="UnitSelect" EventName="SelectedIndexChanged" />
                <asp:AsyncPostBackTrigger ControlID="StatusSelect" EventName="SelectedIndexChanged" />
                <asp:AsyncPostBackTrigger ControlID="CaseIdBox" EventName="TextChanged" />
                <asp:AsyncPostBackTrigger ControlID="NameBox" EventName="TextChanged" />
                <asp:AsyncPostBackTrigger ControlID="SsnBox" EventName="TextChanged" />
            </Triggers>
        </asp:UpdatePanel>
    </div>

    <asp:ObjectDataSource ID="StatusData" runat="server" SelectMethod="GetWorkstatusByWorkflow" TypeName="ALOD.Data.Services.LookupService">
        <SelectParameters>
            <asp:Parameter DefaultValue="1" Name="workflow" Type="Byte" />
        </SelectParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="SearchData" runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="SpecialCaseSearch" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:ControlParameter ControlID="CaseIdBox" Name="caseID" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="SsnBox" Name="ssn" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="NameBox" Name="name" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="StatusSelect" DefaultValue="0" Name="status" PropertyName="SelectedValue" Type="Int32" />
            <asp:SessionParameter Name="userId" SessionField="UserId" Type="Int32" />
            <asp:SessionParameter Name="rptView" SessionField="ReportView" Type="Byte" />
            <asp:SessionParameter Name="compo" SessionField="Compo" Type="String" />
            <asp:Parameter DefaultValue="0" Name="maxCount" Type="Int32" />
            <asp:ControlParameter ControlID="ddlModule" DefaultValue="" Name="Module" PropertyName="SelectedValue" Type="Byte" />
            <asp:ControlParameter ControlID="UnitSelect" DefaultValue="0" Name="unitId" PropertyName="SelectedValue" Type="Int32" />
            <asp:Parameter Name="sarcpermission" Type="Boolean" />
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
