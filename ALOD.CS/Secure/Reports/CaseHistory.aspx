<%@ Page Language="C#" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Reports.Secure_Reports_CaseHistory" Title="" CodeBehind="CaseHistory.aspx.cs" %>

<%@ Import Namespace="ALODWebUtility.Common" %>

<%@ Register Src="~/secure/Shared/UserControls/MemberSearchResultsGrid.ascx" TagName="MemberSearchResultsGrid" TagPrefix="uc1" %>

<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="head">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <script type="text/javascript">
    
    </script>

    <div id="srchBox" class="indent-small dataBlock">
        <div class="dataBlock-header">
            Find Service Member
        </div>
        <div class="dataBlock-body">
            <asp:UpdatePanel runat="server" ID="upnlMemberLookup" ChildrenAsTriggers="true">
                <ContentTemplate>
                    <table class="dataTable">
                        <tr runat="server" id="trRadioButtons">
                            <td class="number">
                                A
                            </td>
                            <td class="label">
                                Lookup By:
                            </td>
                            <td class="value">
                                <asp:RadioButton runat="server" ID="rdbSSN" Text="SSN" GroupName="LookupChoice" AutoPostBack="true" Checked="True" />
                                <asp:RadioButton runat="server" ID="rdbName" Text="Name" GroupName="LookupChoice" AutoPostBack="true" />
                            </td>
                        </tr>
                        <tr runat="server" class="tr-SSN" id="trSSN">
                            <td class="number">
                                B
                            </td>
                            <td class="label">
                                Member SSN :
                            </td>
                            <td class="value">
                                <asp:TextBox ID="SSNText" MaxLength="9" runat="server" />
                                <asp:RegularExpressionValidator ID="SSNValidator" runat="server" ControlToValidate="SSNText" ErrorMessage="Invalid SSN" ValidationExpression="\d{9}?" ValidationGroup="Search">
                                    Invalid SSN
                                </asp:RegularExpressionValidator>
                            </td>
                        </tr>
                        <tr runat="server" class="tr-Name" id="trName">
                            <td class="number">
                                B
                            </td>
                            <td class="label labelRequired">
                                * Member Name (last / first / middle):
                            </td>
                            <td class="value">
                                <asp:TextBox runat="server" ID="txtMemberLastName" MaxLength="50" Width="80px" placeholder="Last Name"></asp:TextBox>
                                <asp:TextBox runat="server" ID="txtMemberFirstName" MaxLength="50" Width="80px" placeholder="First Name"></asp:TextBox>
                                <asp:TextBox runat="server" ID="txtMemberMiddleName" MaxLength="60" Width="80px" placeholder="Middle Name"></asp:TextBox>
                                &nbsp;
                                <asp:Label runat="server" ID="lblMemberNotFound" CssClass="labelRequired" Text="Member Not Found" Visible="false"></asp:Label>
                                &nbsp;
                                <asp:Label runat="server" ID="lblInvalidMemberName" CssClass="labelRequired" Text="Invalid Name" Visible="false"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </ContentTemplate>
            </asp:UpdatePanel>
            <table class="dataTable">
                <tr>
                    <td class="number">
                        C
                    </td>
                    <td class="label">
                        &nbsp;
                    </td>
                    <td class="value">
                        <asp:Button ID="SearchButton" ValidationGroup="Search" runat="server" Text="Search" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <asp:Label runat="server" ID="ErrorLabel" ForeColor="Red" ViewState="false" Font-Bold="true" />
    <!-- end search control -->

    <asp:UpdatePanel ID="resultsUpdatePanel" runat="server" ChildrenAsTriggers="True">
        <ContentTemplate>
            <asp:Panel runat="server" ID="MemberSelectionPanel" Visible="False" CssClass="indent-small dataBlock">
                <div class="dataBlock-header">
                    2 - Member Selection
                </div>
                <div class="dataBlock-body">
                    <uc1:MemberSearchResultsGrid runat="server" ID="ucMemberSelectionGrid"></uc1:MemberSearchResultsGrid>
                </div>
            </asp:Panel>
            
            <div id="spSpacer" class="emptyItem" style="margin-top: 0px; margin-bottom: 4px;height: 22px;">
                <div id="spWait" class="" style="display: none;">
                    &nbsp;
                    <asp:Image runat="server" ID="imgWait" SkinID="imgBusy" AlternateText="busy" ImageAlign="AbsMiddle" />&nbsp;Loading...
                </div>
            </div>

            <asp:Panel ID="ResultsPanel" runat="server" Visible="false" CssClass="indent-small">
                <asp:Panel runat="server" ID="pnlMemberInfo" CssClass="dataBlock">
                    <div class="dataBlock-header">
                        Member Information
                    </div>
                    <div class="dataBlock-body">
                        <table class="dataTable">
                            <tr>
                                <td class="number">
                                    A
                                </td>
                                <td class="label">
                                    Name:
                                </td>
                                <td class="value">
                                    <asp:Label ID="lblName" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="number">
                                    B
                                </td>
                                <td class="label">
                                    Rank:
                                </td>
                                <td class="value">
                                    <asp:Label ID="lblRank" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="number">
                                    C
                                </td>
                                <td class="label">
                                    DOB:
                                </td>
                                <td class="value">
                                    <asp:Label ID="lbldob" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="number">
                                    D
                                </td>
                                <td class="label">
                                    Unit:
                                </td>
                                <td class="value">
                                    <asp:Label runat="server" ID="UnitTextBox" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </asp:Panel>

                <asp:Panel ID="HistoryPanelPAL" runat="server" CssClass="dataBlock">
                    <div class="dataBlock-header">
                        Member PAL Data
                    </div>
                    <div class="dataBlock-body">
                        <asp:GridView ID="HistoryGridPAL" runat="server" AutoGenerateColumns="false" Width="100%">
                            <Columns>
                                <asp:BoundField DataField="Field" HeaderText="Field" />
                                <asp:BoundField DataField="Value" HeaderText="Value" />
                            </Columns>
                        </asp:GridView>
                        <div style="text-align: center;">
                            <asp:Label ID="NoHistoryPALLbl" Text="No PAL Data found" CssClass="labelNormal" runat="server" /> 
                        </div>
                    </div>
                </asp:Panel>

                <asp:Panel ID="HistoryPanel" runat="server" CssClass="dataBlock">
                    <div class="dataBlock-header">
                        Member Case History
                    </div>
                    <div class="dataBlock-body">
                        <asp:GridView runat="server" ID="HistoryGrid" AutoGenerateColumns="false" Width="100%" AllowPaging="true" AllowSorting="true" PageSize="20">
                            <Columns>
                                <asp:TemplateField HeaderText="Case Id" SortExpression="CaseId">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("RefId").ToString()  %>' CommandName="view" Text='<%# Eval("CaseId") %>' />
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
                                <asp:TemplateField HeaderText="Print">
                                    <ItemTemplate>
                                        <asp:ImageButton ImageAlign="AbsMiddle" runat="server" ID="PrintImage" ImageUrl="~/images/pdf.ico" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataRowStyle CssClass="emptyItem" />
                            <EmptyDataTemplate>
                                No other active cases exist for service member
                            </EmptyDataTemplate>
                        </asp:GridView>
                        <div style="text-align: center;">
                            <asp:Label ID="NoHistoryLbl" Text="No cases found" CssClass="labelNormal" runat="server" /> 
                        </div>
                    </div>
                </asp:Panel>

                <asp:Panel ID="SCHistoryPanel" runat="server" CssClass="dataBlock">
                    <div class="dataBlock-header">
                        Member Other/Specialty Case History
                    </div>
                    <div class="dataBlock-body">
                        <asp:GridView runat="server" ID="SCHistoryGrid" AutoGenerateColumns="false" Width="100%" AllowPaging="true" AllowSorting="true" PageSize="20">
                            <Columns>
                                <asp:TemplateField HeaderText="Case Id" SortExpression="Case_Id">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="lnkRefSCID" runat="server" CommandArgument='<%# Eval("RefId").ToString() + ";" + Eval("Module_Id").ToString()  %>' CommandName="view" Text='<%# Eval("Case_Id") %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Unit_Name" HeaderText="Unit" SortExpression="Unit_Name" />
                                <asp:TemplateField HeaderText="Case Type" SortExpression="Module">
                                    <ItemTemplate>
                                        <asp:Label ID="Label1" runat="server" Text='<%#IIf(Eval("workflow_title").ToString = "Worldwide Duty (WD)", "Non Duty Disability Evaluation System (NDDES)", Eval("workflow_title").ToString) %>' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                                <asp:BoundField DataField="ReceiveDate" HeaderText="Received" SortExpression="ReceiveDate" />
                                <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                            </Columns>
                            <EmptyDataRowStyle CssClass="emptyItem" />
                            <EmptyDataTemplate>
                                No other Other/Specialty Cases exist for service member
                            </EmptyDataTemplate>
                        </asp:GridView>
                        <div style="text-align: center;">
                            <asp:Label ID="NoSCHistoryLbl" Text="No other Other/Special cases found" CssClass="labelNormal" runat="server" /> 
                        </div>
                    </div>
                </asp:Panel>
            </asp:Panel>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="SearchButton" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ucMemberSelectionGrid" EventName="MemberSelected" />
        </Triggers>
    </asp:UpdatePanel>
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
<asp:Content ID="Content2" ContentPlaceHolderID="ContentFooter" runat="Server">
    <script type="text/javascript">

    </script>
</asp:Content>
