<%@ Page Title="" Language="C#" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Reports.Secure_Reports_PhsicianCanceledLod" CodeBehind="PhsicianCanceledLod.aspx.cs" %>

<%@ Register Src="~/Secure/Shared/UserControls/ReportNav.ascx" TagName="ReportNav" TagPrefix="uc1" %>
<%@ Register Src="~/secure/Shared/UserControls/MemberSearchResultsGrid.ascx" TagName="MemberSearchResultsGrid" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="Server">
    <div class="indent">
        <uc1:ReportNav ID="rptNav" runat="server" />
        <asp:UpdatePanel runat="server" ID="memberSelectionUpdatePanel" ChildrenAsTriggers="true">
            <ContentTemplate>
                <asp:Panel runat="server" ID="MemberSelectionPanel" Visible="False" CssClass="dataBlock">
                    <div class="dataBlock-header">
                        2 - Member Selection
                    </div>
                    <div class="dataBlock-body">
                        <uc1:MemberSearchResultsGrid runat="server" ID="ucMemberSelectionGrid"></uc1:MemberSearchResultsGrid>
                    </div>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>

        <br />

        <asp:UpdatePanel ID="resultsUpdatePanel" runat="server" ChildrenAsTriggers="True">
            <ContentTemplate>
                <div id="spSpacer" class="emptyItem" style="margin-top: 0px; margin-bottom: 4px; height: 22px;">
                    <div id="spWait" class="" style="display: none;">
                        &nbsp;
                        <asp:Image runat="server" ID="imgWait" SkinID="imgBusy" AlternateText="busy" ImageAlign="AbsMiddle" />&nbsp;Loading...
                    </div>
                </div>
                <br />
                <asp:Panel ID="ResultsPanel" runat="server" Visible="False" CssClass="dataBlock">
                    <div class="dataBlock-header">
                        2 - Report Results
                    </div>
                    <div class="dataBlock-body">
                        <asp:GridView ID="grvCanceledLOD" Width="100%" runat="server" AutoGenerateColumns="False" EmptyDataText="No Record Found" AllowPaging="True" AllowSorting="True" DataKeyNames="lodid" PageSize="20">
                            <Columns>
                                <asp:TemplateField SortExpression="case_id" ItemStyle-Width="100px" ItemStyle-Wrap="true" HeaderText="Case Id">
                                    <ItemTemplate>
                                        <asp:HyperLink ID="HyperLink1" Width="86px" runat="server" NavigateUrl='<%# Eval("lodid", "~/Secure/lod/init.aspx?refId={0}") %>' Text='<%# Eval("case_id") %>'></asp:HyperLink>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:BoundField DataField="ssn" HeaderText="SSN" HtmlEncode="false" ItemStyle-Width="80px" ItemStyle-Wrap="true" SortExpression="ssn" />
                                <asp:BoundField DataField="member_name" HeaderText="Name" ItemStyle-Width="160px" ItemStyle-Wrap="true" SortExpression="member_name" />
                                <asp:BoundField DataField="birthmonth" HeaderText="Birth Month" ItemStyle-Width="100px" ItemStyle-Wrap="true" SortExpression="birthmonth" />
                                <asp:BoundField DataField="member_unit" HeaderText="Unit" ItemStyle-Width="250px" ItemStyle-Wrap="true" SortExpression="member_unit" />
                                <asp:BoundField DataField="reason" HeaderText="Cancel Reason" ItemStyle-Width="250px" ItemStyle-Wrap="true" SortExpression="reason" HtmlEncode="false" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </asp:Panel>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="rptNav" EventName="RptClicked" />
                <asp:AsyncPostBackTrigger ControlID="ucMemberSelectionGrid" EventName="MemberSelected" />
            </Triggers>
        </asp:UpdatePanel>
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
        <br />
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentFooter" runat="Server">
</asp:Content>
