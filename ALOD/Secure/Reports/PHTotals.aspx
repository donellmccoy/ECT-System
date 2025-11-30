<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Secure/Secure.master" CodeBehind="PHTotals.aspx.vb" Inherits="ALOD.Web.Reports.PHTotals"%>



<%@ Register src="../Shared/UserControls/ReportNavPH.ascx" tagname="ReportNavPH" tagprefix="uc1" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .groupLeft-3
        {
            width:32%;
            display:inline-block;
            float:left;
            margin:5px 5px 5px 5px;
        }

        .groupCenter-3
        {
            width:32%;
            display:inline-block;
            float:left;
            margin:5px 5px 5px 5px;
        }

        .groupRight-3
        {
            width:32%;
            display:inline-block;
            float:right;
            margin:5px 5px 5px 5px;
        }

        .groupLeft-2
        {
            width:15%;
            display:inline-block;
            float:left;
        }

        .groupRight-2
        {
            width:85%;
            display:inline-block;
            float:right;
        }

        .group-1
        {
            width:100%;
            margin:5px 10px 5px 5px;
        }

        .flexDiv
        {
            display:flex;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="server">
    <div>
        <div class="indent">
            <asp:UpdatePanel ID="resultsUpdatePanel" runat="server">
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
                    <uc1:ReportNavPH ID="ReportNavPH" runat="server" />
                    <asp:Panel ID="ResultsPanel" runat="server" Visible="False" CssClass="dataBlock">
                        <div class="dataBlock-header">
                            2 - Report Results
                        </div>
                        <div>
                            <asp:Label runat="server" ID="GridViewErrorLabel" ForeColor="Red" Font-Bold="true" />
                        </div>
                        <div class="dataBlock-body flexDiv">
                            <asp:Panel runat="server" ID="pnlBrowserResults" Visible="false">

                                <asp:Panel runat="server" ID="pnlHumanPerformanceImprovementOutreachResults">
                                    <h1>Human Performance Improvement/Outreach</h1>
                                    <asp:Panel runat="server" ID="pnlWalkaboutUnitVisitPairs" CssClass="groupLeft-3">
                                        <h2>Walkabout/Unit Visits:</h2>
                                        <asp:GridView runat="server" ID="gdvWalkaboutUnitVisits" AutoGenerateColumns="false" Width="100%">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Field">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblFieldName" Text='<%# DataBinder.Eval(Container.DataItem, "Field1.Field.Name")%>'/>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="60%" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Frequency / Members Seen">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblTotal" Text='<%# DataBinder.Eval(Container.DataItem, "CombinedValue")%>' />
                                                    </ItemTemplate>
                                                    <ItemStyle Width="40%" />
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>

                                    <asp:Panel runat="server" ID="pnlWalkaboutUnitVisitHours" CssClass="groupCenter-3">
                                        <h2>&nbsp;</h2>
                                        <asp:GridView runat="server" ID="gdvWalkaboutUnitVisitsHours" AutoGenerateColumns="false" Width="100%">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:HiddenField runat="server" ID="hdfFieldId" Value='<%# DataBinder.Eval(Container.DataItem, "Key.Item2")%>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Field">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblFieldName"/>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="60%" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Total" HeaderText="Hours" ItemStyle-Width="40%" />
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>

                                    <asp:Panel runat="server" ID="pnlHumanPerformanceImprovement" CssClass="groupCenter-3">
                                        <h2>&nbsp;</h2>
                                        <asp:GridView runat="server" ID="gdvHumanPerformanceImprovement" AutoGenerateColumns="false" Width="100%">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:HiddenField runat="server" ID="hdfFieldId" Value='<%# DataBinder.Eval(Container.DataItem, "Key.Item2")%>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Field">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblFieldName"/>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="60%" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Total" HeaderText="Total" ItemStyle-Width="40%" />
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>

                                    <br style="clear:both;" />

                                    <asp:Panel runat="server" ID="pnlHumanPerformanceImprovementStringValues" CssClass="group-1" Visible="false">
                                        <asp:GridView runat="server" ID="gdvHumanPerformanceImprovementStringValues" AutoGenerateColumns="false" Width="98.5%">
                                            <Columns>
                                                <asp:BoundField DataField="WingRMU" HeaderText="Wing RMU" ItemStyle-Width="20%" />
                                                <asp:BoundField DataField="ReportingPeriod" HeaderText="Reporting Period" ItemStyle-Width="15%" />
                                                <asp:BoundField DataField="SectionName" HeaderText="Section" ItemStyle-Width="15%" />
                                                <asp:BoundField DataField="FieldName" HeaderText="Field Name" ItemStyle-Width="15%" />
                                                <asp:BoundField DataField="Value" HeaderText="Text" ItemStyle-Width="35%" />
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>

                                </asp:Panel>

                                <br style="clear:both;" /><br />

                                <asp:Panel runat="server" ID="pnlHumanPerformanceSustainmentResults_TrendingActivity">
                                    <h1>Human Performance Sustainment - Trending Activity</h1>

                                    <asp:Panel runat="server" ID="pnlTrendingActivity1" CssClass="groupLeft-3">
                                        <h2>Abuse:</h2>
                                        <asp:GridView runat="server" ID="gdvTrendingActivity_Abuse" AutoGenerateColumns="false" Width="100%">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Field">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblFieldName" Text='<%# DataBinder.Eval(Container.DataItem, "Field1.Field.Name")%>'/>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="60%" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Frequency / Members Seen">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblTotal" Text='<%# DataBinder.Eval(Container.DataItem, "CombinedValue")%>' />
                                                    </ItemTemplate>
                                                    <ItemStyle Width="40%" />
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>

                                    <asp:Panel runat="server" ID="pnlTrendingActivity2" CssClass="groupCenter-3">
                                        <h2>&nbsp;</h2>
                                        <asp:GridView runat="server" ID="gdvTrendingActivity_FrMs" AutoGenerateColumns="false" Width="100%">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Field">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblFieldName" Text='<%# DataBinder.Eval(Container.DataItem, "Field1.Field.Name")%>'/>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="60%" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Frequency / Members Seen">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblTotal" Text='<%# DataBinder.Eval(Container.DataItem, "CombinedValue")%>' />
                                                    </ItemTemplate>
                                                    <ItemStyle Width="40%" />
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>

                                    <asp:Panel runat="server" ID="pnlTrendingActivity3" CssClass="groupRight-3">
                                        <h2>&nbsp;</h2>
                                        <asp:GridView runat="server" ID="gdvTrendingActivity_FrFu" AutoGenerateColumns="false" Width="100%">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Field">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblFieldName" Text='<%# DataBinder.Eval(Container.DataItem, "Field1.Field.Name")%>'/>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="60%" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Frequency / Follow-Ups">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblTotal" Text='<%# DataBinder.Eval(Container.DataItem, "CombinedValue")%>' />
                                                    </ItemTemplate>
                                                    <ItemStyle Width="40%" />
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                        <br />
                                        <asp:GridView runat="server" ID="gdvTrendingActivity_Misc" AutoGenerateColumns="false" Width="100%">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:HiddenField runat="server" ID="hdfFieldId" Value='<%# DataBinder.Eval(Container.DataItem, "Key.Item2")%>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Field">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblFieldName"/>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="60%" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Total" HeaderText="Total" ItemStyle-Width="40%" />
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>

                                    <br style="clear:both;" />

                                    <asp:Panel runat="server" ID="pnlTrendingActivityStringValues" CssClass="group-1" Visible="false">
                                        <asp:GridView runat="server" ID="gdvTrendingActivityStringValues" AutoGenerateColumns="false" Width="98.5%">
                                            <Columns>
                                                <asp:BoundField DataField="WingRMU" HeaderText="Wing RMU" ItemStyle-Width="20%" />
                                                <asp:BoundField DataField="ReportingPeriod" HeaderText="Reporting Period" ItemStyle-Width="15%" />
                                                <asp:BoundField DataField="SectionName" HeaderText="Section" ItemStyle-Width="15%" />
                                                <asp:BoundField DataField="FieldName" HeaderText="Field Name" ItemStyle-Width="15%" />
                                                <asp:BoundField DataField="Value" HeaderText="Text" ItemStyle-Width="35%" />
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>
                                </asp:Panel>

                                <br style="clear:both;" /><br />

                                <asp:Panel runat="server" ID="pnlHumanPerformanceSustainmentResults_Deaths">
                                    <h1>Human Performance Sustainment - Deaths/Near Deaths</h1>

                                    <asp:Panel runat="server" ID="pnlDeaths1" CssClass="groupCenter-3">
                                        <asp:GridView runat="server" ID="gdvDeaths1" AutoGenerateColumns="false" Width="100%">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:HiddenField runat="server" ID="hdfFieldId" Value='<%# DataBinder.Eval(Container.DataItem, "Key.Item2")%>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Field">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblFieldName"/>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="60%" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Total" HeaderText="Total" ItemStyle-Width="40%" />
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>

                                    <asp:Panel runat="server" ID="pnlDeaths2" CssClass="groupCenter-3">
                                        <asp:GridView runat="server" ID="gdvDeaths2" AutoGenerateColumns="false" Width="100%">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:HiddenField runat="server" ID="hdfFieldId" Value='<%# DataBinder.Eval(Container.DataItem, "Key.Item2")%>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Field">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblFieldName"/>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="60%" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Total" HeaderText="Total" ItemStyle-Width="40%" />
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>

                                    <asp:Panel runat="server" ID="pnlDeaths3" CssClass="groupCenter-3">
                                        <asp:GridView runat="server" ID="gdvSuicideMethodTotals" AutoGenerateColumns="false" Width="98.5%">
                                            <Columns>
                                                <asp:BoundField DataField="Key" HeaderText="Suicide Method" ItemStyle-Width="75%" />
                                                <asp:BoundField DataField="Value" HeaderText="Total" ItemStyle-Width="25%" />
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>

                                    <br style="clear:both;" />

                                    <asp:Panel runat="server" ID="pnlDeathsStringValues" CssClass="group-1" Visible="false">
                                        <asp:GridView runat="server" ID="gdvDeathsStringValues" AutoGenerateColumns="false" Width="98.5%">
                                            <Columns>
                                                <asp:BoundField DataField="WingRMU" HeaderText="Wing RMU" ItemStyle-Width="20%" />
                                                <asp:BoundField DataField="ReportingPeriod" HeaderText="Reporting Period" ItemStyle-Width="15%" />
                                                <asp:BoundField DataField="SectionName" HeaderText="Section" ItemStyle-Width="15%" />
                                                <asp:BoundField DataField="FieldName" HeaderText="Field Name" ItemStyle-Width="15%" />
                                                <asp:BoundField DataField="Value" HeaderText="Text" ItemStyle-Width="35%" />
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>
                                </asp:Panel>

                                <br style="clear:both;" /><br />

                                <asp:Panel runat="server" ID="pnlHumanPerformanceSustainmentResults_PresentingProblems">
                                    <h1>Human Performance Sustainment - Presenting Problems</h1>

                                    <asp:Panel runat="server" ID="pnlPresentingProblems1" CssClass="groupLeft-3">
                                        <asp:GridView runat="server" ID="gdvPresentingProblems1" AutoGenerateColumns="false" Width="100%">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Field">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblFieldName" Text='<%# DataBinder.Eval(Container.DataItem, "Field1.Field.Name")%>'/>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="60%" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Members Seen / Follow-Ups">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblTotal" Text='<%# DataBinder.Eval(Container.DataItem, "CombinedValue")%>' />
                                                    </ItemTemplate>
                                                    <ItemStyle Width="40%" />
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>

                                    <asp:Panel runat="server" ID="pnlPresentingProblems2" CssClass="groupCenter-3">
                                        <asp:GridView runat="server" ID="gdvPresentingProblems2" AutoGenerateColumns="false" Width="100%">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Field">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblFieldName" Text='<%# DataBinder.Eval(Container.DataItem, "Field1.Field.Name")%>'/>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="60%" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Members Seen / Follow-Ups">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblTotal" Text='<%# DataBinder.Eval(Container.DataItem, "CombinedValue")%>' />
                                                    </ItemTemplate>
                                                    <ItemStyle Width="40%" />
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>

                                    <asp:Panel runat="server" ID="pnlPresentingProblems3" CssClass="groupRight-3">
                                        <asp:GridView runat="server" ID="gdvPresentingProblems3" AutoGenerateColumns="false" Width="100%">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Field">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblFieldName" Text='<%# DataBinder.Eval(Container.DataItem, "Field1.Field.Name")%>'/>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="60%" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Members Seen / Follow-Ups">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblTotal" Text='<%# DataBinder.Eval(Container.DataItem, "CombinedValue")%>' />
                                                    </ItemTemplate>
                                                    <ItemStyle Width="40%" />
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>

                                    <br style="clear:both;" />

                                    <asp:Panel runat="server" ID="pnlPresentingProblemsStringValues" CssClass="group-1" Visible="false">
                                        <asp:GridView runat="server" ID="gdvPresentingProblemsStringValues" AutoGenerateColumns="false" Width="98.5%">
                                            <Columns>
                                                <asp:BoundField DataField="WingRMU" HeaderText="Wing RMU" ItemStyle-Width="20%" />
                                                <asp:BoundField DataField="ReportingPeriod" HeaderText="Reporting Period" ItemStyle-Width="15%" />
                                                <asp:BoundField DataField="SectionName" HeaderText="Section" ItemStyle-Width="15%" />
                                                <asp:BoundField DataField="FieldName" HeaderText="Field Name" ItemStyle-Width="15%" />
                                                <asp:BoundField DataField="Value" HeaderText="Text" ItemStyle-Width="35%" />
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>
                                </asp:Panel>

                                <br style="clear:both;" /><br />

                                <asp:Panel runat="server" ID="pnlReferralsResults">
                                    <h1>Referrals</h1>

                                    <asp:Panel runat="server" ID="pnlReferrals1" CssClass="groupCenter-3">
                                        <asp:GridView runat="server" ID="gdvReferrals1" AutoGenerateColumns="false" Width="100%">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:HiddenField runat="server" ID="hdfFieldId" Value='<%# DataBinder.Eval(Container.DataItem, "Key.Item2")%>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Field">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblFieldName"/>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="60%" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Total" HeaderText="Total" ItemStyle-Width="40%" />
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>

                                    <asp:Panel runat="server" ID="pnlReferrals2" CssClass="groupCenter-3">
                                        <asp:GridView runat="server" ID="gdvReferrals2" AutoGenerateColumns="false" Width="100%">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:HiddenField runat="server" ID="hdfFieldId" Value='<%# DataBinder.Eval(Container.DataItem, "Key.Item2")%>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Field">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblFieldName"/>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="60%" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Total" HeaderText="Total" ItemStyle-Width="40%" />
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>

                                    <asp:Panel runat="server" ID="pnlReferrals3" CssClass="groupCenter-3">
                                        <asp:GridView runat="server" ID="gdvReferrals3" AutoGenerateColumns="false" Width="100%">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:HiddenField runat="server" ID="hdfFieldId" Value='<%# DataBinder.Eval(Container.DataItem, "Key.Item2")%>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Field">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblFieldName"/>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="60%" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Total" HeaderText="Total" ItemStyle-Width="40%" />
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>

                                    <br style="clear:both;" />

                                    <asp:Panel runat="server" ID="pnlReferralsStringValues" CssClass="group-1" Visible="false">
                                        <asp:GridView runat="server" ID="gdvReferralsStringValues" AutoGenerateColumns="false" Width="98.5%">
                                            <Columns>
                                                <asp:BoundField DataField="WingRMU" HeaderText="Wing RMU" ItemStyle-Width="20%" />
                                                <asp:BoundField DataField="ReportingPeriod" HeaderText="Reporting Period" ItemStyle-Width="15%" />
                                                <asp:BoundField DataField="SectionName" HeaderText="Section" ItemStyle-Width="15%" />
                                                <asp:BoundField DataField="FieldName" HeaderText="Field Name" ItemStyle-Width="15%" />
                                                <asp:BoundField DataField="Value" HeaderText="Text" ItemStyle-Width="35%" />
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>
                                </asp:Panel>


                                <br style="clear:both;" /><br />

                                <asp:Panel runat="server" ID="pnlRMUInteractionResults">
                                    <h1>RMU Interaction</h1>

                                    <asp:Panel runat="server" ID="pnlRMUInteraction1" CssClass="groupCenter-3">
                                        <asp:GridView runat="server" ID="gdvRMUInteraction1" AutoGenerateColumns="false" Width="100%">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:HiddenField runat="server" ID="hdfFieldId" Value='<%# DataBinder.Eval(Container.DataItem, "Key.Item2")%>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Field">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblFieldName"/>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="60%" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Total" HeaderText="Total" ItemStyle-Width="40%" />
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>

                                    <asp:Panel runat="server" ID="pnlRMUInteraction2" CssClass="groupCenter-3">
                                        <asp:GridView runat="server" ID="gdvRMUInteraction2" AutoGenerateColumns="false" Width="100%">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:HiddenField runat="server" ID="hdfFieldId" Value='<%# DataBinder.Eval(Container.DataItem, "Key.Item2")%>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Field">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblFieldName"/>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="60%" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Total" HeaderText="Total" ItemStyle-Width="40%" />
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>

                                    <asp:Panel runat="server" ID="pnlRMUInteraction3" CssClass="groupCenter-3">
                                        <asp:GridView runat="server" ID="gdvRMUInteraction3" AutoGenerateColumns="false" Width="100%">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:HiddenField runat="server" ID="hdfFieldId" Value='<%# DataBinder.Eval(Container.DataItem, "Key.Item2")%>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Field">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblFieldName"/>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="60%" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Total" HeaderText="Total" ItemStyle-Width="40%" />
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>

                                    <br style="clear:both;" />

                                    <asp:Panel runat="server" ID="pnlRMUInteractionStringValues" CssClass="group-1" Visible="false">
                                        <asp:GridView runat="server" ID="gdvRMUInteractionStringValues" AutoGenerateColumns="false" Width="98.5%">
                                            <Columns>
                                                <asp:BoundField DataField="WingRMU" HeaderText="Wing RMU" ItemStyle-Width="20%" />
                                                <asp:BoundField DataField="ReportingPeriod" HeaderText="Reporting Period" ItemStyle-Width="15%" />
                                                <asp:BoundField DataField="SectionName" HeaderText="Section" ItemStyle-Width="15%" />
                                                <asp:BoundField DataField="FieldName" HeaderText="Field Name" ItemStyle-Width="15%" />
                                                <asp:BoundField DataField="Value" HeaderText="Text" ItemStyle-Width="35%" />
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>
                                </asp:Panel>


                                <br style="clear:both;" /><br />

                                <asp:Panel runat="server" ID="pnlDemographicsResults">
                                    <h1>Only New Non-Medical//Medical Client Demographics</h1>

                                    <asp:Panel runat="server" ID="pnlDemographics1" CssClass="groupLeft-3">
                                        <h2>Status:</h2>
                                        <asp:GridView runat="server" ID="gdvDemographics_Status" AutoGenerateColumns="false" Width="100%">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:HiddenField runat="server" ID="hdfFieldId" Value='<%# DataBinder.Eval(Container.DataItem, "Key.Item2")%>' />
                                                        <asp:HiddenField runat="server" ID="hdfFieldTypeId" Value='<%# DataBinder.Eval(Container.DataItem, "Key.Item3")%>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Field">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblFieldName"/>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="60%" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Total" HeaderText="Total" ItemStyle-Width="40%" />
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>

                                    <asp:Panel runat="server" ID="pnlDemographics2" CssClass="groupCenter-3">
                                        <h2>Age:</h2>
                                        <asp:GridView runat="server" ID="gdvDemographics_Age" AutoGenerateColumns="false" Width="100%">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:HiddenField runat="server" ID="hdfFieldId" Value='<%# DataBinder.Eval(Container.DataItem, "Key.Item2")%>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Field">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblFieldName"/>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="60%" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Total" HeaderText="Total" ItemStyle-Width="40%" />
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>

                                    <asp:Panel runat="server" ID="pnlDemographics3" CssClass="groupRight-3">
                                        <h2>Gender:</h2>
                                        <asp:GridView runat="server" ID="gdvDemographics_Gender" AutoGenerateColumns="false" Width="100%">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:HiddenField runat="server" ID="hdfFieldId" Value='<%# DataBinder.Eval(Container.DataItem, "Key.Item2")%>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Field">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblFieldName"/>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="60%" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Total" HeaderText="Total" ItemStyle-Width="40%" />
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>

                                    <br style="clear:both;" /><br />

                                    <asp:Panel runat="server" ID="pnlDemographics4" CssClass="groupLeft-3">
                                        <h2>Rank:</h2>
                                        <asp:GridView runat="server" ID="gdvDemographics_Rank" AutoGenerateColumns="false" Width="100%">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:HiddenField runat="server" ID="hdfFieldId" Value='<%# DataBinder.Eval(Container.DataItem, "Key.Item2")%>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Field">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblFieldName"/>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="60%" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Total" HeaderText="Total" ItemStyle-Width="40%" />
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>

                                    <asp:Panel runat="server" ID="pnlDemographics5" CssClass="groupCenter-3">
                                        <h2>Marital Status:</h2>
                                        <asp:GridView runat="server" ID="gdvDemographics_MaritalStatus" AutoGenerateColumns="false" Width="100%">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:HiddenField runat="server" ID="hdfFieldId" Value='<%# DataBinder.Eval(Container.DataItem, "Key.Item2")%>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Field">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblFieldName"/>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="60%" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Total" HeaderText="Total" ItemStyle-Width="40%" />
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>

                                    <asp:Panel runat="server" ID="pnlDemographics6" CssClass="groupRight-3">
                                        <h2>Ethnicity:</h2>
                                        <asp:GridView runat="server" ID="gdvDemographics_Ethnicity" AutoGenerateColumns="false" Width="100%">
                                            <Columns>
                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:HiddenField runat="server" ID="hdfFieldId" Value='<%# DataBinder.Eval(Container.DataItem, "Key.Item2")%>' />
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Field">
                                                    <ItemTemplate>
                                                        <asp:Label runat="server" ID="lblFieldName"/>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="60%" />
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="Total" HeaderText="Total" ItemStyle-Width="40%" />
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>

                                    <br style="clear:both;" />

                                    <asp:Panel runat="server" ID="pnlDemographicsStringValues" CssClass="group-1" Visible="false">
                                        <asp:GridView runat="server" ID="gdvDemographicsStringValues" AutoGenerateColumns="false" Width="98.5%">
                                            <Columns>
                                                <asp:BoundField DataField="WingRMU" HeaderText="Wing RMU" ItemStyle-Width="20%" />
                                                <asp:BoundField DataField="ReportingPeriod" HeaderText="Reporting Period" ItemStyle-Width="15%" />
                                                <asp:BoundField DataField="SectionName" HeaderText="Section" ItemStyle-Width="15%" />
                                                <asp:BoundField DataField="FieldName" HeaderText="Field Name" ItemStyle-Width="15%" />
                                                <asp:BoundField DataField="Value" HeaderText="Text" ItemStyle-Width="35%" />
                                            </Columns>
                                        </asp:GridView>
                                    </asp:Panel>
                                </asp:Panel>


                                <br style="clear:both;" /><br />

                                <asp:Panel runat="server" ID="pnlCommentsResults">
                                    <h1>Comments</h1>
                                    <asp:GridView runat="server" ID="gdvComments" AutoGenerateColumns="false" Width="100%">
                                        <Columns>
                                            <asp:BoundField DataField="PH Wing RMU" HeaderText="Wing RMU" ItemStyle-Width="10%" />
                                            <asp:BoundField DataField="Reporting Period" HeaderText="Reporting Period" ItemStyle-Width="10%" />
                                            <asp:BoundField DataField="Value" HeaderText="Comment" ItemStyle-Width="80%" />
                                        </Columns>
                                    </asp:GridView>
                                </asp:Panel>

                            </asp:Panel>
                            
                            <asp:Panel runat="server" ID="pnlExcelResults" Visible="false">
                                <asp:GridView runat="server" ID="gdvExcelResults" AutoGenerateColumns="false">
                                    <Columns>
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:HiddenField runat="server" ID="hdfSectionId" Value='<%# DataBinder.Eval(Container.DataItem, "Key.Item1")%>' />
                                                <asp:HiddenField runat="server" ID="hdfFieldId" Value='<%# DataBinder.Eval(Container.DataItem, "Key.Item2")%>' />
                                                <asp:HiddenField runat="server" ID="hdfFieldTypeId" Value='<%# DataBinder.Eval(Container.DataItem, "Key.Item3")%>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Section">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblSectionName"></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Width="30%" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Field Name">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblFieldName"></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Width="30%" />
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Field Type">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblFieldTypeName"></asp:Label>
                                            </ItemTemplate>
                                            <ItemStyle Width="30%" />
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Total" HeaderText="Total" />
                                    </Columns>
                                </asp:GridView>
                                <asp:GridView runat="server" ID="gdvExcelSuicideMethodTotals" AutoGenerateColumns="false" Width="100%">
                                    <Columns>
                                        <asp:BoundField DataField="Key" HeaderText="Suicide Method" ItemStyle-Width="75%" />
                                        <asp:BoundField DataField="Value" HeaderText="Total" ItemStyle-Width="25%" />
                                    </Columns>
                                </asp:GridView>
                                <asp:GridView runat="server" ID="gdvExcelStringValueResults" AutoGenerateColumns="false" Width="100%">
                                    <Columns>
                                        <asp:BoundField DataField="WingRMU" HeaderText="Wing RMU" ItemStyle-Width="20%" />
                                        <asp:BoundField DataField="ReportingPeriod" HeaderText="Reporting Period" ItemStyle-Width="15%" />
                                        <asp:BoundField DataField="SectionName" HeaderText="Section" ItemStyle-Width="15%" />
                                        <asp:BoundField DataField="FieldName" HeaderText="Field Name" ItemStyle-Width="15%" />
                                        <asp:BoundField DataField="Value" HeaderText="Text" ItemStyle-Width="35%" />
                                    </Columns>
                                </asp:GridView>
                                <asp:GridView runat="server" ID="gdvExcelCommentResults" AutoGenerateColumns="false" Width="100%">
                                    <Columns>
                                        <asp:BoundField DataField="PH Wing RMU" HeaderText="Wing RMU" ItemStyle-Width="10%" />
                                        <asp:BoundField DataField="Reporting Period" HeaderText="Reporting Period" ItemStyle-Width="10%" />
                                        <asp:BoundField DataField="Value" HeaderText="Comment" ItemStyle-Width="80%" />
                                    </Columns>
                                </asp:GridView>
                            </asp:Panel>
                        </div>
                    </asp:Panel>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentFooter" runat="server">
</asp:Content>
