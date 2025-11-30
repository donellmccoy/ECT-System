<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Reports.Secure_Reports_DisapprovedLod" Codebehind="DisapprovedLod.aspx.vb" %>

<%@ Register Src="~/Secure/Shared/UserControls/ReportNav.ascx" TagName="ReportNav"
    TagPrefix="uc1" %>
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
                <div id="spSpacer" class="emptyItem" style="margin-top: 0px; margin-bottom: 4px;
                    height: 22px;">
                    <div id="spWait" class="" style="display: none;">
                        &nbsp;<asp:Image runat="server" ID="imgWait" SkinID="imgBusy" AlternateText="busy"
                            ImageAlign="AbsMiddle" />&nbsp;Loading...
                    </div>
                </div>
                <div id="rpt">
                    <asp:GridView ID="DisapprovedGridView" runat="server" AutoGenerateColumns="False"
                        ShowFooter="True" Width="100%">
                        <Columns>
                            <asp:BoundField DataField="pName" HeaderText="Role" />
                            <asp:TemplateField HeaderText="EPTS - LOD NA">
                                <ItemTemplate>
                                    <asp:LinkButton ID="EPTLink" runat="server" CausesValidation="false" CommandArgument='<%# Eval("pTypeId").ToString() + "," + Eval("eptId").ToString() +","+ Eval("pName") +","+ " EPT "  %>'
                                        CommandName="getDetail" Visible='<%# Eval("eptCount") <>0 %>' Text='<%# Eval("eptCount") %>'></asp:LinkButton>
                                    <asp:Label ID="nEPTLabel" runat="server" Text='<%# Eval("eptCount") %>' Visible='<%# Eval(“eptCount”) =0 %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Not ILOD Own Misconduct">
                                <ItemTemplate>
                                    <asp:LinkButton ID="MiscounductLink" runat="server" CausesValidation="false" CommandName="getDetail"
                                        CommandArgument='<%# Eval("pTypeId").ToString() + "," + Eval("nIlodId").ToString() +","+ Eval("pName") +","+ " Not ILD Own Misconduct " %>'
                                        Text='<%# Eval("nILodCount") %>' Visible='<%# Eval("nILodCount") <>0 %>'></asp:LinkButton>
                                    <asp:Label ID="nILODLabel" runat="server" Text='<%# Eval("nILodCount") %>' Visible='<%# Eval(“nILodCount”) =0 %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Not ILOD Not Own Misconduct">
                                <ItemTemplate>
                                    <asp:LinkButton ID="NotMiscounductLink" runat="server" CausesValidation="false" CommandName="getDetail"
                                        CommandArgument='<%# Eval("pTypeId").ToString() + "," + Eval("nIlodNotId").ToString()+","+ Eval("pName") +","+ " Not ILD Not Own Misconduct "  %>'
                                        Visible='<%# Eval("nIlodNotCount") <>0 %>' Text='<%# Eval("nIlodNotCount") %>'></asp:LinkButton>
                                    <asp:Label ID="nIlodNotCountLabel" runat="server" Text='<%# Eval("nIlodNotCount") %>'
                                        Visible='<%# Eval(“nIlodNotCount”) =0 %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Total">
                                <ItemTemplate>
                                    <asp:Label ID="lblTotal" runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="rptNav" EventName="RptClicked" />
            </Triggers>
        </asp:UpdatePanel>
        <asp:UpdatePanel ID="detailsUpdatePanel" runat="server" ChildrenAsTriggers="True">
            <ContentTemplate>
                <div id="rowDetail" runat="server" visible="false">
                    <br />
                    <div class="dataBlock-header">
                        List Of Cases
                    </div>
                    <table>
                        <tr>
                            <td class="labelNormal">
                                Disapproved by:
                            </td>
                            <td class="value">
                                <asp:Label ID="DetailsGroupLabel" class="labelNormal" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="labelNormal">
                                Reason:
                            </td>
                            <td class="value">
                                <asp:Label ID="DetailsReasonLabel" class="labelNormal" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </table>
                    <br />
                    <asp:GridView ID="DetailGrid" Width="100%" runat="server" AllowSorting="True" AutoGenerateColumns="False"
                        PageSize="15" EmptyDataText="No record found" AllowPaging="True">
                        <Columns>
                            <asp:HyperLinkField DataNavigateUrlFields="lodId" HeaderText="Case Id" DataNavigateUrlFormatString="~/Secure/lod/init.aspx?refId={0}"
                                DataTextField="case_id" SortExpression="case_id" />
                            <asp:BoundField DataField="member_name" HeaderText="Name" SortExpression="member_name" />
                            <asp:BoundField DataField="member_unit" HeaderText="Unit" SortExpression="member_unit" />
                            <asp:BoundField DataField="created_date" HeaderText="Date Created" SortExpression="created_date" />
                            <asp:BoundField DataField="event_nature_type" HeaderText="Injury/Diseaes/Death/Illness"
                                SortExpression="event_nature_type" />
                        </Columns>
                    </asp:GridView>
                    <br />
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="DisapprovedGridView" EventName="RowCommand" />
            </Triggers>
        </asp:UpdatePanel>
        <ajax:UpdatePanelAnimationExtender ID="UpdatePanelAnimationExtender1" runat="server"
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
