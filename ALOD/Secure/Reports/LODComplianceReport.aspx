<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Secure/Secure.master" CodeBehind="LODComplianceReport.aspx.vb" Inherits="ALOD.Web.Reports.LODComplianceReport" MaintainScrollPositionOnPostback="true"%>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .InThresholdTolerance
        {
            color:orange;
        }

        .OverThresholdTolerance
        {
            color:red;
        }

        .optionsPanel
        {
            display:inline-block;
            width:79%;
            margin-right:3px;
        }
        
        .legendPanel
        {
            display:inline-block;
            float:right;
            width:19%;
        }

        .flexPanel
        {
            display:flex;
        }

        .title
        {
            font-weight: bold;
            font-size: large;

        }

        .overallAccuracy
        {
            text-align: right;
            font-weight: bold;
             
        }

        .horizontalScrollPanel-topscroll, .horizontalScrollPanel, .horizontalScrollPanel_Quality, .horizontalScrollPanel-topscroll_Quality
        {
            overflow-x: auto;
            width:100%;
            border-style:solid;
            border-left-width:1px;
            border-right-width:1px;
        }

        .horizontalScrollPanel, .horizontalScrollPanel_Quality
        {
            border-top-width:0px;
            border-bottom-width:1px;
        }

        .horizontalScrollPanel-topscroll, .horizontalScrollPanel-topscroll_Quality
        {

            height: 16px;
            border-top-width:1px;
            border-bottom-width:0px;
        }

        .numericColumn
        {
            text-align:right;
        }

        .numericResult
        {
            font-weight:bold;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="server">
    <asp:Panel runat="server" ID="pnlReportOptions" CssClass="indent-small dataBlock">
        <div class="dataBlock-header">
            1 - Report Options
        </div>
        <div class="dataBlock-body flexPanel">
            <div class="optionsPanel">
                <table>
                     <tr>
                        <td class="number">
                            <asp:Label ID="Label2" runat="server" Text="A.1"></asp:Label>
                        </td>
                        <td class="label">
                            Select Quarter:
                        </td>
                        <td class="value">
                            <asp:DropDownList runat="server" ID="ddlQuarter" Width="175" AutoPostBack="true" />
                            &nbsp;&nbsp;&nbsp;
                            And / Or
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label ID="Label1" runat="server" Text="A.2"></asp:Label>
                        </td>
                        <td class="label">
                            Select Year:
                        </td>
                        <td class="value">
                            <asp:DropDownList runat="server" ID="ddlYear" Width="175" AutoPostBack="true" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label ID="lblOutputFormatRow" runat="server" Text="B"></asp:Label>
                        </td>
                        <td class="label">
                            Output Format
                            <asp:LinkButton runat="server" ID="lbtnHelp" Font-Bold="true" Text="?" OnClientClick="OpenHelpWindow('/Secure/Shared/AFIReportExportInstructions.aspx'); return false;" />
                            :
                        </td>
                        <td class="value">
                            <asp:RadioButton ID="rdbOutputScreen" runat="server" Checked="true" GroupName="Output" Text="Browser" />
                            <img src="../../images/page_white_world.gif" alt="View in browser" style="vertical-align:middle" />
                            &nbsp;
                            <asp:RadioButton ID="rdbOutputExcel" runat="server" GroupName="Output" Text="Excel" />
                            <img src="../../images/page_white_excel.gif" alt="Export to Excel" style="vertical-align:middle" />
                            &nbsp;
                            <asp:RadioButton ID="rdbOutputPdf" runat="server" GroupName="Output" Text="PDF" />
                            <img src="../../images/page_white_acrobat.gif" alt="Export to PDF" style="vertical-align: middle"  />
                            &nbsp;
                            <asp:RadioButton ID="rdbOutputCsv" runat="server" GroupName="Output" Text="CSV" />
                            <img src="../../images/page_white_text.gif" alt="Export to CSV" style="vertical-align: middle" " />
                            &nbsp;
                            <asp:RadioButton ID="rdbOutputXml" runat="server" GroupName="Output" Text="XML" />
                            <img src="../../images/page_white_code.gif" alt="Export to XML" style="vertical-align: middle"  />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label ID="lblActionRow" runat="server" Text="C"></asp:Label>
                        </td>
                        <td class="label">
                            Action:
                        </td>
                        <td class="value">
                            <asp:Button runat="server" ID="btnRunReport" Text="Run Report" />
                        </td>
                    </tr>
                    <tr runat="server" id="trErrors" visible="false">
                        <td class="number">
                            &nbsp;
                        </td>
                        <td class="label">
                            Errors:
                        </td>
                        <td class="value">
                            <asp:BulletedList ID="bllErrors" runat="server" CssClass="labelRequired" />
                        </td> 
                    </tr>
                </table>
            </div>
            <div class="legendPanel">
                <asp:Label runat="server" ID="lblLegend" Text="Legend:" Font-Bold="true" Font-Underline="true" />
                <br /><br />
                <asp:Label runat="server" ID="lblLegendRed" ForeColor="Red" Text="Red" /> = Exceeded AFI Timeline
            </div>

        </div>
    </asp:Panel>
    <asp:Panel runat="server" ID="pnlResults" Visible="false">
        <asp:Panel runat="server" ID="pnlUnitNavigation" CssClass="indent-small dataBlock" Visible="true">
            <div class="dataBlock-header">
                2 - Unit Navigation
            </div>
            <div class="dataBlock-body">
                <asp:Repeater ID="rptUnitNavigation" runat="server">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkUnit" runat="server"></asp:LinkButton>
                    </ItemTemplate>
                    <SeparatorTemplate>
                        <b>></b>
                    </SeparatorTemplate>
                </asp:Repeater>
            </div>
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlQualityResults" CssClass="indent-small dataBlock" Visible="true">
            <div class="dataBlock-header">
                3 - Quality Results
            </div>
            <div class="dataBlock-body">
                <asp:GridView runat="server" SkinID="TestGV" ID="gdvQualityResults" AutoGenerateColumns="False" Width="100%" DataKeyNames="RefId">
                    <Columns>
                        <asp:TemplateField HeaderText="Unit">
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="lkbUnit" CommandName="<%# GRIDCOMMAND_VIEW_UNIT %>" />
                            </ItemTemplate>
                            <ItemStyle Width="10%" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Case Id">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblCaseId" />
                            </ItemTemplate>
                            <ItemStyle Width="15%" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Member Unit">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblMemberUnit" />
                            </ItemTemplate>
                            <ItemStyle Width="15%" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Date Completed">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblDateCompleted" />
                            </ItemTemplate>
                            <ItemStyle Width="15%" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Total RFA">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblTotalRWOA" CssClass="numericResult" />
                            </ItemTemplate>
                            <ItemStyle Width="15%" CssClass="numericColumn" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Reasons for RFA">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblReasons" />
                            </ItemTemplate>
                            <ItemStyle Width="40%" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:Panel runat="server" ID="pnlQualityEmptyResults" Visible="false">
                    <asp:Label runat="server" ID="lblQualityEmptyResults" />
                </asp:Panel>
            </div>
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlTimelinessResults" CssClass="indent-small dataBlock" Visible="true">
            <div class="dataBlock-header">
                4 - Timeliness Results
            </div>
            <div class="dataBlock-body">
                <div runat="server" id="pnlTimelinessTopScrollPanel" visible="true" class="horizontalScrollPanel-topscroll">
                    <div style="width:1483px;">
                        <div style="width:100%;">
                            &nbsp;
                        </div>
                    </div>
                </div>
                <div runat="server" id="pnlTimelinessHorizontalScrollPanel" class="horizontalScrollPanel">
                    <asp:GridView runat="server" SkinID="TestGV" ID="gdvTimelinessResults" AutoGenerateColumns="False" Width="1500px">
                        <Columns>
                            <asp:TemplateField HeaderText="Unit">
                                <ItemTemplate>
                                    <asp:LinkButton runat="server" ID="lkbUnit" CommandName="<%# GRIDCOMMAND_VIEW_UNIT %>" />
                                </ItemTemplate>
                                <ItemStyle Width="8%" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Case Id">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCaseId" />
                                </ItemTemplate>
                                <ItemStyle Width="8%" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Total Days">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDaysOpen" CssClass="numericResult" />
                                </ItemTemplate>
                                <ItemStyle Width="3%" CssClass="numericColumn" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Member Unit">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblMemberUnit" />
                                </ItemTemplate>
                                <ItemStyle Width="15%" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Date Completed">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDateCompleted" />
                                </ItemTemplate>
                                <ItemStyle Width="10%" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Wing SARC Days*">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblWingSARCDays" CssClass="numericResult" />
                                </ItemTemplate>
                                <ItemStyle Width="3%" CssClass="numericColumn" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Med. Tech. Days*">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblMedTechDays" CssClass="numericResult" />
                                </ItemTemplate>
                                <ItemStyle Width="3%" CssClass="numericColumn" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Med. Off. Days*">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblMedOffDays" CssClass="numericResult" />
                                </ItemTemplate>
                                <ItemStyle Width="3%" CssClass="numericColumn" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Unit CC Days">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblUnitCCDays" CssClass="numericResult" />
                                </ItemTemplate>
                                <ItemStyle Width="3%" CssClass="numericColumn" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Wing JA Days">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblWingJADays" CssClass="numericResult" />
                                </ItemTemplate>
                                <ItemStyle Width="3%" CssClass="numericColumn" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Wing CC Days">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblWingCCDays" CssClass="numericResult" />
                                </ItemTemplate>
                                <ItemStyle Width="3%" CssClass="numericColumn" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Informal Board Days **">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblInformalBoardDays" CssClass="numericResult" />
                                </ItemTemplate>
                                <ItemStyle Width="3%" CssClass="numericColumn" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="LOD PM Days*">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblLODPMDays" CssClass="numericResult" />
                                </ItemTemplate>
                                <ItemStyle Width="3%" CssClass="numericColumn" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="IO Days">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblIODays" CssClass="numericResult" />
                                </ItemTemplate>
                                <ItemStyle Width="3%" CssClass="numericColumn" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Formal Wing JA Days">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblFormalWingJADays" CssClass="numericResult" />
                                </ItemTemplate>
                                <ItemStyle Width="3%" CssClass="numericColumn" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Formal Wing CC Days">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblFormalWingCCDays" CssClass="numericResult" />
                                </ItemTemplate>
                                <ItemStyle Width="3%" CssClass="numericColumn" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Formal Board Tech Days **">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblBoardTechDays" CssClass="numericResult" />
                                </ItemTemplate>
                                <ItemStyle Width="3%" CssClass="numericColumn" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Formal Board JA Days **">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblBoardJADays" CssClass="numericResult" />
                                </ItemTemplate>
                                <ItemStyle Width="3%" CssClass="numericColumn" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Formal Board A1 Days **">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblBoardA1Days" CssClass="numericResult" />
                                </ItemTemplate>
                                <ItemStyle Width="3%" CssClass="numericColumn" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Formal Board SG Days **">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblBoardMedDays" CssClass="numericResult" />
                                </ItemTemplate>
                                <ItemStyle Width="3%" CssClass="numericColumn" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Formal Appr. Auth. Days **">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblApprAuthDays" CssClass="numericResult" />
                                </ItemTemplate>
                                <ItemStyle Width="3%" CssClass="numericColumn" />
                            </asp:TemplateField>
                            
                        </Columns>
                    </asp:GridView>
                    <br />
                </div>
                <asp:Panel runat="server" ID="pnlAFINotes" Visible="true">
                    <table>
                        <tr>
                            <td style="vertical-align:top;text-align:right;">
                                *
                            </td>
                            <td>
                                <asp:Label runat="server" ID="lblSingleAsteriskMsg" Text="IAW AFI 36-2910 Table 2.1, Note 3 the MFP (Med Tech), Medical Officer and Notification of IO (Formal) are not measured in the processing timeline.  Total case time tracking begins with submission by the Medical Officer to the Unit/CC and for Formal cases after the IO has been notified." />
                            </td>
                        </tr>
                        <tr>
                            <td style="vertical-align:top;text-align:right;">
                                **
                            </td>
                            <td>
                                <asp:Label runat="server" ID="lblDoubleAsteriskMsg" Text="IAW AFI 36-2910 Table 2.1, Note 2 the ARC LOD Determination Board total processing time is 15 days (Informal) or 20 days (Formal), if required." />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:Panel runat="server" ID="pnlTimelinessEmptyResults" Visible="false">
                    <asp:Label runat="server" ID="lblTimelinessEmptyResults" />
                </asp:Panel>
            </div>
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlAccuracyResults" CssClass="indent-small dataBlock" Visible="true">
            <div class="dataBlock-header">
                5 - Accuracy Results
            </div>
            <div class="dataBlock-body">
                <asp:GridView runat="server" SkinID="TestGV" ID="gdvAccuracyResults" AutoGenerateColumns="False" Width="910px" DataKeyNames="RefId">
                        <Columns>
                            <asp:TemplateField HeaderText="Unit">
                                <ItemTemplate>
                                    <asp:LinkButton runat="server" ID="lkbUnit" CommandName="<%# GRIDCOMMAND_VIEW_UNIT %>" />
                                </ItemTemplate>
                                <ItemStyle Width="10%" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Case Id">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCaseId" />
                                </ItemTemplate>
                                <ItemStyle Width="3%" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Member Unit">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblMemberUnit" />
                                </ItemTemplate>
                                <ItemStyle Width="3%" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Date Completed">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblDateCompleted" />
                                </ItemTemplate>
                                <ItemStyle Width="3%" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Appointing Authority Findings">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblAppointingFindings" />
                                </ItemTemplate>
                                <ItemStyle Width="3%" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Approving Authority Findings">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblApprovingFindings" />
                                </ItemTemplate>
                                <ItemStyle Width="3%" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Formal Appointing Authority Findings">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblFormalAppointingFindings" />
                                </ItemTemplate>
                                <ItemStyle Width="3%" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Formal Approving Authority Findings">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblFormalApprovingFindings" />
                                </ItemTemplate>
                                <ItemStyle Width="3%" />
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="Accuracy">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblAccuracy" CssClass="numericResult" />
                                </ItemTemplate>
                                <ItemStyle Width="3%" CssClass="numericColumn" />
                            </asp:TemplateField>
                        </Columns>
                </asp:GridView>
                <div class="legendPanel">
                    <asp:Label runat="server" ID="lblOverallAccuracy" CssClass="overallAccuracy" />
                </div>
                <asp:Panel runat="server" ID="pnlAccuracyEmptyResults" Visible="false">
                    <asp:Label runat="server" ID="lblAccuracyEmptyResults" />
                </asp:Panel>
            </div>
        </asp:Panel>
        <asp:Label runat="server" ID="lblTripleAsteriskMsg" Text="*** Results are based on a random 10% sampling IAW AFI 36-2910." />
    </asp:Panel>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentFooter" runat="server">
    <script type="text/javascript">
        $(function () {
            $(".horizontalScrollPanel-topscroll").scroll(function () {
                $(".horizontalScrollPanel").scrollLeft($(".horizontalScrollPanel-topscroll").scrollLeft());
            });

            $(".horizontalScrollPanel").scroll(function () {
                $(".horizontalScrollPanel-topscroll").scrollLeft($(".horizontalScrollPanel").scrollLeft());
            });

            $(".horizontalScrollPanel-topscroll_Quality").scroll(function () {
                $(".horizontalScrollPanel_Quality").scrollLeft($(".horizontalScrollPanel-topscroll_Quality").scrollLeft());
            });

            $(".horizontalScrollPanel_Quality").scroll(function () {
                $(".horizontalScrollPanel-topscroll_Quality").scrollLeft($(".horizontalScrollPanel_Quality").scrollLeft());
            });
        });

        function OpenHelpWindow(url) {
            showPopup({
                'Url': $_HOSTNAME + url,
                'Center': true,
                'Width': 900,
                'Height': 800,
                'Resizable': true,
                'ScrollBars': true
            });
        }
    </script>
</asp:Content>