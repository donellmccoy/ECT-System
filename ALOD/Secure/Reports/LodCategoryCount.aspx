<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Reports.Secure_Reports_LodCategoryCount" Codebehind="LodCategoryCount.aspx.vb" %>

<%@ Register Src="~/Secure/Shared/UserControls/ReportNav.ascx" TagName="ReportNav"
    TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="Server">
    <div class="indent">
        <asp:UpdatePanel ID="resultsUpdatePanel" runat="server" ChildrenAsTriggers="True">
            <ContentTemplate>
                <div>
                    <asp:Repeater ID="rptNav" runat="server">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkButton" runat="server"></asp:LinkButton>
                        </ItemTemplate>
                        <SeparatorTemplate>
                            <b>></b>
                        </SeparatorTemplate>
                    </asp:Repeater>
                </div>
                <br />
                <div>
                    <uc1:ReportNav ID="rptTemplate" runat="server" />
                </div>
                <div id="spSpacer" class="emptyItem" style="margin-top: 0px; margin-bottom: 4px;
                    height: 22px;">
                    <div id="spWait" class="" style="display: none;">
                        &nbsp;<asp:Image runat="server" ID="imgWait" SkinID="imgBusy" AlternateText="busy"
                            ImageAlign="AbsMiddle" />&nbsp;Loading...
                    </div>
                </div>
                <div id="rpt">
                    <br />
                    <asp:Panel runat="server" ID="FeedbackPanel" Visible="false" CssClass="ui-state-highlight info-block msg-panel">
                        <asp:Label runat="server" ID="FeedbackMessageLabel" />
                    </asp:Panel>
                    <asp:GridView ID="ReportGridView" runat="server" Width="100%" DataKeyNames="cs_id,viewType"
                        AutoGenerateColumns="False" AllowPaging="True" AllowSorting="True" >
                        <Columns>
                            <asp:TemplateField ShowHeader="true" HeaderText="Unit" SortExpression="name">
                                <FooterTemplate>
                                </FooterTemplate>
                                <ItemTemplate>
                                    <asp:LinkButton ID="NameLinkButton" runat="server" CausesValidation="false" CommandArgument='<%# CType(Eval("cs_id"),String)   +";"+ Eval("name") %>'
                                        Visible='<%# Eval(“Total”) <>0 %>' CommandName="drill" Text='<%# Eval("name") %>'></asp:LinkButton>
                                    <asp:Label ID="NameLinkLabel" runat="server" Text='<%# Eval("name") %>' Visible='<%# Eval(“Total”) =0 %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="Disease" HeaderText="Disease" SortExpression="Disease" />
                            <asp:BoundField DataField="Injury" HeaderText="Injury" SortExpression="Injury" />
                            <asp:BoundField DataField="Illness" HeaderText="Illness" SortExpression="Illness" />
                            <asp:BoundField DataField="Death" HeaderText="Death" SortExpression="Death" />
                            <asp:BoundField DataField="Total" HeaderText="Total" SortExpression="Total" />
                        </Columns>
                    </asp:GridView>
                </div>
                <br />
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:UpdatePanel ID="detailsUpdatePanel" runat="server" ChildrenAsTriggers="True">
            <ContentTemplate>
                <div id="rowDetail" runat="server" visible="false">
                    <div class="dataBlock-header">
                        List Of Cases
                    </div>
                    <table>
                        <tr>
                            <td class="labelNormal">
                                Unit Name:
                            </td>
                            <td class="value">
                                <asp:Label ID="UnitNameLbl" class="labelNormal" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <asp:GridView ID="DetailGridView" runat="server" Width="100%" AutoGenerateColumns="False"
                        EmptyDataText="No Record Found" AllowPaging="True" AllowSorting="True" PageSize="15">
                        <Columns>
                            <asp:HyperLinkField DataNavigateUrlFields="lodid" ControlStyle-Width="100px" DataTextField="case_Id"
                                HeaderText="CaseId" SortExpression="case_Id" Text="caseId" DataNavigateUrlFormatString="~/Secure/lod/init.aspx?refId={0}" />
                            <asp:BoundField DataField="member_unit" HeaderText="Unit" ControlStyle-Width="350px"
                                SortExpression="member_unit">
                                <ControlStyle Width="500px"></ControlStyle>
                            </asp:BoundField>
                            <asp:BoundField DataField="description" HeaderText="Status" SortExpression="description" />
                            <asp:BoundField DataField="icd9Id" HeaderText="ICD9 Code" SortExpression="icd9Id" />
                            <asp:BoundField DataField="icdName" HeaderText="ICD9 Name" SortExpression="icdName" />
                            <asp:BoundField DataField="event_nature_type" HeaderText="EventType" ControlStyle-Width="100px"
                                SortExpression="event_nature_type">
                                <ControlStyle Width="100px"></ControlStyle>
                            </asp:BoundField>
                        </Columns>
                    </asp:GridView>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="ReportGridView" EventName="RowCommand" />
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
        <ajax:UpdatePanelAnimationExtender ID="detailsUpdatePanelAnimationExtender1" runat="server"
            TargetControlID="detailsUpdatePanel">
            <Animations>
                <OnUpdating>
                    <ScriptAction script="searchStart();" />
                </OnUpdating>  
                <OnUpdated>     
                    <ScriptAction script="searchEnd();" />
                </OnUpdated>           
            </Animations>
        </ajax:UpdatePanelAnimationExtender>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFooter" runat="Server">
</asp:Content>
