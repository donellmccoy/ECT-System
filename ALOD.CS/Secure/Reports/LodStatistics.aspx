<%@ Page Title="" Language="C#" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Reports.Secure_Reports_LodStatistics" CodeBehind="LodStatistics.aspx.cs" %>

<%@ Register Src="~/Secure/Shared/UserControls/ReportNav.ascx" TagName="ReportNav" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="Server">
    <div class="indent">
        <div>
            <uc1:ReportNav ID="rptNav" runat="server" />
        </div>
        <br />
        <asp:UpdatePanel ID="resultsUpdatePanel" runat="server" ChildrenAsTriggers="True">
            <ContentTemplate>
                <div id="spSpacer" class="emptyItem" style="margin-top: 0px; margin-bottom: 4px; height: 22px;">
                    <div id="spWait" class="" style="display: none;">
                        &nbsp;
                        <asp:Image runat="server" ID="imgWait" SkinID="imgBusy" AlternateText="busy" ImageAlign="AbsMiddle" />&nbsp;Loading...
                    </div>
                </div>
                <asp:GridView ID="StatGrid" runat="server" Width="100%" AutoGenerateColumns="False"
                    ShowFooter="True">
                    <Columns>
                        <asp:BoundField DataField="start" HeaderText="Month" DataFormatString="{0:MMMM-yyyy}"
                            HtmlEncode="false" ItemStyle-Width="60px">
                            <ItemStyle Width="120px"></ItemStyle>
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="LOD's <br/> Initiated" ShowHeader="False">
                            <ItemTemplate>
                                <asp:LinkButton ID="LodCountLink" runat="server" CausesValidation="false" CommandName="getDetail"
                                    CommandArgument='<%# Eval("start") + ";" + Eval("last") + ";lodCount;LODs Initiated" %>'
                                    Text='<%# Eval("lodInitiated") %>' Visible='<%# Eval(�lodInitiated�) <>0 %>'></asp:LinkButton>
                                <asp:Label ID="lodiLabel" runat="server" Text='<%# Eval("lodInitiated") %>' Visible='<%# Eval(�lodInitiated�) =0 %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="80px" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Closed By <br/> Wing CC" ShowHeader="False">
                            <ItemTemplate>
                                <asp:LinkButton ID="ClosedWingLink" runat="server" CausesValidation="false" Visible='<%# Eval(�wingClosed�) <>0 %>'
                                    CommandArgument='<%# Eval("start") + ";" + Eval("last") + ";closedWing;Closed By Wing CC" %>'
                                    CommandName="getDetail" Text='<%# Eval("wingClosed") %>'></asp:LinkButton>
                                <asp:Label ID="lodWingClosedLabel" runat="server" Text='<%# Eval("wingClosed") %>'
                                    Visible='<%# Eval(�wingClosed�) =0 %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="80px" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Closed by <br/>RLB" ShowHeader="False">
                            <ItemTemplate>
                                <asp:LinkButton ID="ClosedRLBLink" runat="server" CausesValidation="false" Visible='<%# Eval(�rlpClosed�) <>0 %>'
                                    CommandArgument='<%# Eval("start") + ";" + Eval("last") + ";closedRLB;Closed By RLB" %>'
                                    CommandName="getDetail" Text='<%# Eval("rlpClosed") %>'></asp:LinkButton>
                                <asp:Label ID="lodCloseRLBClosedLabel" runat="server" Text='<%# Eval("rlpClosed") %>'
                                    Visible='<%# Eval(�rlpClosed�) =0 %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="80px" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Conflict <br/> WC/RLB" ShowHeader="False">
                            <ItemTemplate>
                                <asp:LinkButton ID="ConflictWCRLBLink" runat="server" CausesValidation="false" Visible='<%# Eval(�wcrlb�) <>0 %>'
                                    CommandArgument='<%# Eval("start") + ";" + Eval("last") + ";conflictWCRLB;Conflict WC/RLB" %>'
                                    CommandName="getDetail" Text='<%# Eval("wcrlb") %>'></asp:LinkButton>
                                <asp:Label ID="lodCCWCRLBLabel" runat="server" Text='<%# Eval("wcrlb") %>' Visible='<%# Eval(�wcrlb�) =0 %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="80px" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Conflict <br/> WC/AA" ShowHeader="False">
                            <ItemTemplate>
                                <asp:LinkButton ID="ConflictWCAALink" runat="server" CausesValidation="false" Visible='<%# Eval(�wcaa�) <>0 %>'
                                    CommandArgument='<%# Eval("start") + ";" + Eval("last") + ";conflictWCAA;Conflict WC/AA" %>'
                                    CommandName="getDetail" Text='<%# Eval("wcaa") %>'></asp:LinkButton>
                                <asp:Label ID="lodCCWCAALabel" runat="server" Text='<%# Eval("wcaa") %>' Visible='<%# Eval(�wcaa�) =0 %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="80px" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Conflict <br/> RLB/AA" ShowHeader="False">
                            <ItemTemplate>
                                <asp:LinkButton ID="ConflictRLBAALink" runat="server" CausesValidation="false" Visible='<%# Eval(�rlbaa�) <>0 %>'
                                    CommandArgument='<%# Eval("start") + ";" + Eval("last") + ";conflictRLBAA;Conflict RLB/AA" %>'
                                    CommandName="getDetail" Text='<%# Eval("rlbaa") %>'></asp:LinkButton>
                                <asp:Label ID="lodCCrlbAALabel" runat="server" Text='<%# Eval("rlbaa") %>' Visible='<%# Eval(�rlbaa�) =0 %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="80px" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="rptNav" EventName="RptClicked" />
            </Triggers>
        </asp:UpdatePanel>
        <br />
        <asp:UpdatePanel ID="detailsUpdatePanel" runat="server" ChildrenAsTriggers="True">
            <ContentTemplate>
                <div id="DetailDiv" visible="false" runat="server">
                    <div class="dataBlock-header">
                        List Of Cases
                    </div>
                    <table>
                        <tr>
                            <td class="labelNormal">
                                Category:
                            </td>
                            <td class="value">
                                <asp:Label ID="DetailsCatLabel" class="labelNormal" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="labelNormal">
                                Month:
                            </td>
                            <td class="value">
                                <asp:Label ID="DetailsMonthLabel" class="labelNormal" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <asp:GridView ID="DetailGrid" runat="server" Width="100%" AllowSorting="True" AutoGenerateColumns="False"
                        PageSize="15" EmptyDataText="No record found" AllowPaging="True">
                        <Columns>
                            <asp:HyperLinkField DataNavigateUrlFields="lodId" HeaderText="Case Id" ItemStyle-Width="120px"
                                DataNavigateUrlFormatString="~/Secure/lod/init.aspx?refId={0}" DataTextField="case_id"
                                SortExpression="case_id" />
                            <asp:BoundField DataField="member_name" HeaderText="Name" ItemStyle-Width="200px" ItemStyle-Wrap="true" SortExpression="member_name" />
                            <asp:BoundField DataField="member_unit" HeaderText="Unit" ItemStyle-Width="200px" ItemStyle-Wrap="true" SortExpression="member_unit" />
                            <asp:BoundField DataField="ReceiveDate" ItemStyle-Width="150px" ItemStyle-Wrap="true" HeaderText="Date Closed" SortExpression="ReceiveDate" />
                            <asp:TemplateField HeaderText="DecisionA" SortExpression="">
                                <ItemTemplate>
                                    <asp:Label ID="DecisionaALabel" runat="server"></asp:Label>
                                </ItemTemplate>
                                <ItemStyle Width="200px" Wrap="true" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="DecisionB">
                                <ItemTemplate>
                                    <asp:Label ID="DecisionaBLabel" runat="server"></asp:Label>
                                </ItemTemplate>
                                <ItemStyle Width="200px" Wrap="true" />
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="StatGrid" EventName="RowCommand" />
            </Triggers>
        </asp:UpdatePanel>
    </div>
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
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFooter" runat="Server">
</asp:Content>
