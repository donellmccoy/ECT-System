<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Reports.Secure_Reports_Rowa" CodeBehind="Rowa.aspx.vb" %>

<%@ Register Src="~/Secure/Shared/UserControls/ReportNav.ascx" TagName="ReportNav" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="Server">
    <div class="indent-small">
        <div>
            <uc1:ReportNav ID="rptNav" runat="server" />
        </div>
        <asp:UpdatePanel ID="resultsUpdatePanel" runat="server" ChildrenAsTriggers="True">
            <ContentTemplate>
                <div id="spSpacer" class="emptyItem" style="margin-top: 0px; margin-bottom: 4px; height: 22px;">
                    <div id="spWait" class="" style="display: none;">
                        &nbsp;
                        <asp:Image runat="server" ID="imgWait" SkinID="imgBusy" AlternateText="busy" ImageAlign="AbsMiddle" />&nbsp;Loading...
                    </div>
                </div>
                <div id="rowaCount" runat="server" class="dataBlock" visible="false">
                    <div class="dataBlock-header">
                        Reason Counts
                    </div>
                    <div class="dataBlock-body">
                        <asp:GridView ID="GridView1" runat="server" Width="100%" AutoGenerateColumns="False" DataKeyNames="id" ShowFooter="True">
                            <Columns>
                                <asp:BoundField DataField="Description" HeaderText="Reason Returned" />
                                <asp:TemplateField HeaderText="Total Count" ShowHeader="False">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="false" CommandName="getDetail"
                                            CommandArgument='<%#  CType(Eval("id"),String) + ";" + Eval("Description")%>'
                                            Visible='<%# Eval(“TotalCount”) <> 0 %>' Text='<%# Eval("TotalCount") %>'></asp:LinkButton>
                                        <asp:Label ID="countLabel" runat="server" Visible='<%# Eval(“TotalCount”)=0 %>' Text='<%# Eval("TotalCount") %>'></asp:Label>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="rptNav" EventName="RptClicked" />
            </Triggers>
        </asp:UpdatePanel>
        <asp:UpdatePanel ID="detailsUpdatePanel" runat="server" ChildrenAsTriggers="True">
            <ContentTemplate>
                <div id="rowaDetail" runat="server" class="dataBlock" visible="false">
                    <div class="dataBlock-header">
                        List Of Cases
                    </div>
                    <div class="dataBlock-body">
                        <table>
                            <tr>
                                <td class="labelNormal">RWOA Reason:
                                </td>
                                <td class="value">
                                    <asp:Label ID="DetailsReasonLabel" class="labelNormal" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                        <br />
                        <asp:GridView ID="GridView2" runat="server" Width="100%" AutoGenerateColumns="False" AllowPaging="True" AllowSorting="True" PageSize="15">
                            <Columns>
                                <asp:HyperLinkField DataNavigateUrlFields="lodId" ItemStyle-Width="13%" HeaderText="Case Id" DataNavigateUrlFormatString="~/Secure/lod/init.aspx?refId={0}" DataTextField="case_id" SortExpression="case_id" />
                                <asp:BoundField DataField="member_name" HeaderText="Member Name" ItemStyle-Width="15%" ItemStyle-Wrap="true" SortExpression="member_name" />
                                <asp:BoundField DataField="member_unit" HeaderText="Unit" ItemStyle-Width="20%" ItemStyle-Wrap="true" SortExpression="member_unit" />
                                <asp:BoundField DataField="rwoa_date" HeaderText="RFA Date" ItemStyle-Width="15%" ItemStyle-Wrap="true" SortExpression="rwoa_date" />
                                <asp:BoundField DataField="rwoa_reason" HeaderText="RFA Reason" ItemStyle-Width="15%" ItemStyle-Wrap="true" SortExpression="rwoa_reason" />
                                <asp:BoundField DataField="rwoa_explanation" HeaderText="Sender Comments" ItemStyle-Width="22%" ItemStyle-Wrap="true" SortExpression="rwoa_explanation" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="GridView1" EventName="RowCommand" />
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
