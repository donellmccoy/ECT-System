<%@ Page Title="" Language="C#" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Reports.RSDisposition" CodeBehind="RSDisposition.aspx.cs" %>

<%@ Register Src="~/Secure/Shared/UserControls/ReportNav.ascx" TagName="ReportNav" TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/SignatureBlock.ascx" TagName="SignatureBlock" TagPrefix="uc1" %>
<%@ Register Src="~/secure/Shared/UserControls/MemberSearchResultsGrid.ascx" TagName="MemberSearchResultsGrid" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="Server">
    <div class="indent">
            <uc1:ReportNav ID="rptTemplate" runat="server" />
        <div>
            <uc1:SignatureBlock ID="SigBlock" runat="server" />
            <asp:Label runat="server" ID="ErrorLabel" ForeColor="Red" Font-Bold="true" />
        </div>
        <br />
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
                        <asp:Image runat="server" ID="imgWait" SkinID="imgBusy" AlternateText="busy" ImageAlign="AbsMiddle" />
                        &nbsp;
                        Loading...
                    </div>
                </div><br />
                <asp:Panel ID="ResultsPanel" runat="server" Visible="False" CssClass="dataBlock">
                    <div class="dataBlock-header">
                        2 - Report Results
                    </div>
                    <div>
                        <asp:Label runat="server" ID="GridViewErrorLabel" ForeColor="Red" Font-Bold="true" />
                    </div>
                    <div class="dataBlock-body">
                    <asp:GridView ID="ReportGrid" runat="server" Width="100%" AllowSorting="True" AutoGenerateColumns="False" PageSize="10" EmptyDataText="No record found" AllowPaging="True">
                        <Columns>
                            <asp:HyperLinkField DataNavigateUrlFields="refId" ItemStyle-Width="125px" ItemStyle-Wrap="true"
                                HeaderText="Case Id" DataNavigateUrlFormatString="~/Secure/SC_RS/init.aspx?refId={0}"
                                DataTextField="caseId" SortExpression="caseId" />
                            <asp:BoundField DataField="ssn" ItemStyle-Width="80px" ItemStyle-Wrap="true" HeaderText="SSN" HtmlEncode="false" SortExpression="ssn" />
                            <asp:BoundField DataField="birthMonth" ItemStyle-Width="100px" ItemStyle-Wrap="true" HeaderText="Birth Month" SortExpression="birthMonthNum" />
                            <asp:BoundField DataField="lastName" HeaderText="Name" ItemStyle-Width="200px" ItemStyle-Wrap="true" SortExpression="lastName" />
                            <asp:BoundField DataField="unit" HeaderText="Unit" SortExpression="unit" ItemStyle-Width="250px" ItemStyle-Wrap="true" />
                            <asp:TemplateField ItemStyle-Width="85px">
                                <ItemTemplate>
                                    <asp:LinkButton ID="ReassessmentLinkButton" runat="server" CausesValidation="false"
                                        CommandArgument='<%# CType(Eval("refId"), String) + "|" + CType(Eval("caseId"), String)%>' Visible='<%# DetermineReassessmentLinkVisbility(Eval("isFinal"), Eval("status"), Eval("caseId"), Eval("refId"))%>'
                                        CommandName="reassessment" Text='Reassessment'></asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                    </div>
                    <br />
                </asp:Panel>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="rptTemplate" EventName="RptClicked" />
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
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentFooter" runat="Server">
    <script type="text/javascript">

    </script>
</asp:Content>
