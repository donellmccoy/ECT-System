<%@ Page Title="" Language="C#" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Reports.Secure_Reports_LodDisposition" CodeBehind="LodDisposition.aspx.cs" %>

<%@ Register Src="~/Secure/Shared/UserControls/ReportNav.ascx" TagName="ReportNav" TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/SignatureBlock.ascx" TagName="SignatureBlock" TagPrefix="uc1" %>
<%@ Register Src="~/secure/Shared/UserControls/MemberSearchResultsGrid.ascx" TagName="MemberSearchResultsGrid" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="Server">
    <div class="indent">
            <uc1:ReportNav ID="rptTemplate" runat="server" />
        <div>
            <asp:UpdatePanel runat="server" ID="upnlSigBlock">
                <ContentTemplate>
                    <uc1:SignatureBlock ID="SigBlock" runat="server" />
                    <asp:Label runat="server" ID="ErrorLabel" ForeColor="Red" Font-Bold="true" />
                </ContentTemplate>
            </asp:UpdatePanel>
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
                <div id="spSpacer" class="emptyItem" style="margin-top: 0px; margin-bottom: 4px;
                    height: 22px;">
                    <div id="spWait" class="" style="display: none;">
                        &nbsp;<asp:Image runat="server" ID="imgWait" SkinID="imgBusy" AlternateText="busy"
                            ImageAlign="AbsMiddle" />&nbsp;Loading...
                    </div>
                </div><br />
                <asp:Panel ID="ResultsPanel" runat="server" Visible="False" CssClass="dataBlock">
                    <div class="dataBlock-header">
                        2 - Report Results
                    </div>
                    <div style="margin-left: auto; margin-right: auto; text-align: center; padding-top: 10px;">
                        <asp:Label runat="server" ID="GridViewErrorLabel" ForeColor="Red" Font-Bold="true" allign="center"/>
                    </div>
                    <div class="dataBlock-body">
                    <asp:GridView ID="ReportGrid" runat="server" Width="100%" AllowSorting="True" AutoGenerateColumns="False"
                        PageSize="10" EmptyDataText="No record found" AllowPaging="True">
                        <Columns>
                            <asp:HyperLinkField DataNavigateUrlFields="lodId" ItemStyle-Width="100px" ItemStyle-Wrap="true"
                                HeaderText="Case Id" DataNavigateUrlFormatString="~/Secure/lod/init.aspx?refId={0}"
                                DataTextField="case_id" SortExpression="case_id" />
                            <asp:BoundField DataField="SSN" ItemStyle-Width="80px" ItemStyle-Wrap="true" HeaderText="SSN"
                                HtmlEncode="false" SortExpression="SSN" />
                            <asp:BoundField DataField="birthmonth" ItemStyle-Width="100px" ItemStyle-Wrap="true"
                                HeaderText="Birth Month" SortExpression="birthmonthNum" />
                            <asp:BoundField DataField="LASTNAME" HeaderText="Name" ItemStyle-Width="200px" ItemStyle-Wrap="true"
                                SortExpression="LASTNAME" />
                            <asp:BoundField DataField="UNIT" HeaderText="Unit" SortExpression="UNIT" ItemStyle-Width="250px"
                                ItemStyle-Wrap="true" />
                            <asp:TemplateField ItemStyle-Width="85px">
                                <ItemTemplate>
                                    <asp:LinkButton ID="ReinvestigationLinkButton" runat="server" CausesValidation="false"
                                        CommandArgument='<%# CType(Eval("lodId"),String)  %>' Visible='<%# DetermineRRLinkVisibility(Eval("isFinal"), Eval("FinalFindings"), Eval("case_id"), Eval("lodId"), Eval("isFormal"))%>'
                                        CommandName="reinvestigate" Text='Reinvestigation Request'></asp:LinkButton>
                                    <asp:linkbutton id="appeallinkbutton" runat="server" causesvalidation="false"
                                        CommandArgument='<%# CType(Eval("lodId"),String) %>' visible='<%# DetermineAPLinkVisibility(Eval("isFinal"), Eval("FinalFindings"), Eval("case_id"), Eval("isFormal"), Eval("lodId"))%>'
                                        commandname="appeal" text='Initiate Appeal Request'></asp:linkbutton>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Print">
                                <ItemTemplate>

                                   <asp:ImageButton ImageAlign="AbsMiddle" runat="server" ID="PrintImage" ImageUrl="~/images/pdf.ico" />


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
