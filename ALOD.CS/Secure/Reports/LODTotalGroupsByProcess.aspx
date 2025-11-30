<%@ Page Title="" Language="C#" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Reports.Secure_Reports_LODTotalGroupsByProcess" CodeBehind="LODTotalGroupsByProcess.aspx.cs" %>

<%--<%@ Register Src="~/Secure/Shared/UserControls/ReportNav.ascx" TagName="ReportNav"
    TagPrefix="uc1" %>--%>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="Server">
    <div>
        <div class="indent">
            <p>THE PAGE HAS LOADED!!!</p>
            <%--<asp:UpdatePanel ID="resultsUpdatePanel" runat="server" ChildrenAsTriggers="True">
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
                    <div>
                        <div id="rpt">
                            <div id="spSpacer" class="emptyItem" style="margin-top: 0px; margin-bottom: 4px;
                                height: 22px;">
                                <div id="spWait" class="" style="display: none;">
                                    &nbsp;<asp:Image runat="server" ID="imgWait" SkinID="imgBusy" AlternateText="busy"
                                        ImageAlign="AbsMiddle" />&nbsp;Loading...
                                </div>
                            </div>
                            <asp:Panel runat="server" ID="FeedbackPanel" Visible="false" CssClass="ui-state-highlight info-block msg-panel">
                                <asp:Label runat="server" ID="FeedbackMessageLabel" />
                            </asp:Panel>
                            <br />
                            <asp:GridView ID="ReportGridView" runat="server" Width="100%" DataKeyNames="cs_id,viewType"
                                AutoGenerateColumns="False" AllowPaging="false" AllowSorting="True">
                                <Columns>
                                    <asp:TemplateField ShowHeader="true" HeaderText="Unit" SortExpression="name" ItemStyle-Width="240px">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="NameLinkButton" runat="server" CausesValidation="false" CommandArgument='<%# CType(Eval("cs_id"),String)   +";"+ Eval("name") %>'
                                                Visible='<%# Eval(�Total�) <>0 %>' CommandName="drill" Text='<%# Eval("name") %>'></asp:LinkButton>
                                            <asp:Label ID="NameLinkLabel" runat="server" Text='<%# Eval("name") %>' Visible='<%# Eval(�Total�) =0 %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ShowHeader="true" HeaderText="Medical" SortExpression="medical"
                                        ItemStyle-Width="5px">
                                        <ItemTemplate>
                                            <%#Eval("medical")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ShowHeader="true" HeaderText="Unit CC" SortExpression="unit">
                                        <ItemTemplate>
                                            <%#Eval("unit")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ShowHeader="true" HeaderText="Wing JA" SortExpression="wingja">
                                        <ItemTemplate>
                                            <%#Eval("wingja")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ShowHeader="true" HeaderText="Wing CC" SortExpression="wingcc">
                                        <ItemTemplate>
                                            <%#Eval("wingcc")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ShowHeader="true" HeaderText="MPF" SortExpression="mpf">
                                        <ItemTemplate>
                                            <%#Eval("mpf")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ShowHeader="true" HeaderText="Formal IO" SortExpression="formalInvestigation">
                                        <ItemTemplate>
                                            <%#Eval("formalInvestigation")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ShowHeader="true" HeaderText="Formal Wing JA" SortExpression="formalActionWingJA">
                                        <ItemTemplate>
                                            <%#Eval("formalActionWingJA")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ShowHeader="true" HeaderText="Formal Wing CC" SortExpression="formalActionWingCC">
                                        <ItemTemplate>
                                            <%#Eval("formalActionWingCC")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ShowHeader="true" HeaderText="Lod Board" SortExpression="LodBoard">
                                        <ItemTemplate>
                                            <%#Eval("LodBoard")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ShowHeader="true" HeaderText="Formal Investigation HQ" SortExpression="formalInvestigationDirected">
                                        <ItemTemplate>
                                            <%#Eval("formalInvestigationDirected")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ShowHeader="true" HeaderText="Approving Authority" SortExpression="ApprovingAuthority">
                                        <ItemTemplate>
                                            <%#Eval("ApprovingAuthority")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ShowHeader="true" HeaderText="Total" SortExpression="Total">
                                        <ItemTemplate>
                                            <%#Eval("Total")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
            <br />
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
                        <br />
                        <asp:GridView ID="DetailGridView" runat="server" Width="100%" AutoGenerateColumns="False"
                            AllowPaging="True" AllowSorting="True" PageSize="15">
                            <Columns>
                                <asp:HyperLinkField DataNavigateUrlFields="lodId" HeaderText="Case Id" DataNavigateUrlFormatString="~/Secure/lod/init.aspx?refId={0}"
                                    DataTextField="case_id" SortExpression="case_id" />
                                <asp:BoundField DataField="SSN" HeaderText="SSN" SortExpression="SSN" />
                                <asp:BoundField DataField="member_name" HeaderText="Member Name" SortExpression="member_name" />
                                <asp:BoundField DataField="description" HeaderText="Status" SortExpression="description" />
                                <asp:BoundField DataField="created_date" HeaderText="Start Date" SortExpression="created_date"
                                    DataFormatString="{0:d}" />
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
            </ajax:UpdatePanelAnimationExtender>--%>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFooter" runat="Server">
</asp:Content>
