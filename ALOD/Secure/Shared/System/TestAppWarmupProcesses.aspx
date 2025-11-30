<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Sys.TestAppWarmupProcesses" Codebehind="TestAppWarmupProcesses.aspx.vb" %>

<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="head">
    <style type="text/css">
        .inputPanel
        {
            display:inline-block;
            width:49%;
            margin-right:3px;
            padding: 3px 4px 3px 4px;
        }
        
        .infoPanel
        {
            display:inline-block;
            float:right;
            width:49%;
        }
    </style>
</asp:Content>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <div class="indent-small">
        <asp:Panel runat="server" ID="pnlInput" CssClass="dataBlock">
            <div class="dataBlock-header">
                1 - Test Automatic Processes
            </div>

            <div class="dataBlock-body" style="display:flex;">
                <div class="inputPanel">
                    
                    <asp:UpdatePanel runat="server" ID="upnlInput">
                        <ContentTemplate>
                            <table runat="server" id="tblInput">
                                <tr>
                                    <td class="number">
                                        A
                                    </td>
                                    <td class="label">
                                        Select Type of Process to Test:
                                    </td>
                                    <td class="value">
                                        <asp:DropDownList runat="server" ID="ddlTypeOfProcess" AutoPostBack="true" />
                                    </td>
                                </tr>
                                
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                    <table runat="server" id="tblInput2">
                        <tr>
                            <td class="number">
                                B
                            </td>
                            <td class="label">
                                Test Execution Date:
                            </td>
                            <td class="value">
                                <asp:TextBox ID="txtTestExecutionDate" MaxLength="10" onchange="DateCheck(this);" runat="server" Width="80"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                C
                            </td>
                            <td class="label">
                                Action:
                            </td>
                            <td class="value">
                                <asp:Button runat="server" ID="btnExecuteTest" Text="Execute Test" />
                            </td>
                        </tr>
                    </table>
                    <asp:Label runat="server" ID="lblDisabled" ForeColor="Red" Visible="false">TESTING DISABLED IN PRODUCTION ENVIRONMENT</asp:Label>
                </div>
                <div class="infoPanel">
                    <span style="font-weight:bold;">PROCESS INFO: </span>
                    <asp:UpdatePanel runat="server" ID="upnlProcessInfo">
                        <ContentTemplate>
                            <table runat="server" id="tblPHProcessInfo" visible="false">
                                <tr>
                                    <td style="text-align:right;">
                                        Last day of the Month
                                    </td>
                                    <td>
                                        - Push Report
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align:right;">
                                        Fourth day of the month
                                    </td>
                                    <td>
                                        - Delinquent Collection Period
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align:right;">
                                        Seventh to last day of the Month
                                    </td>
                                    <td>
                                        - Seven Day Warning
                                    </td>
                                </tr>
                            </table>
                            <table runat="server" id="tblReportProcessInfo" visible="false">
                                <tr>
                                    <td style="text-align:right;">
                                        Jan 1st
                                    </td>
                                    <td>
                                        - Annual Program Status
                                    </td>
                                </tr>
                                <tr>
                                    <td style="text-align:right;">
                                        Jan 1st, Apr 1st, Jul 1st, Oct 1st
                                    </td>
                                    <td>
                                        - Quarterly Program Status
                                    </td>
                                </tr>
                            </table>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlAppWarmupProcessLog" CssClass="dataBlock">
            <div class="dataBlock-header">
                2 - Application Warmup Automatic Process Log
            </div>

            <div class="dataBlock-body">
                <asp:GridView runat="server" ID="gdvProcessLogs" AutoGenerateColumns="False" Width="100%" DataKeyNames="Id" AllowPaging="true" PageSize="20">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" ItemStyle-Width="5%" />
                        <asp:TemplateField HeaderText="Process">
                            <ItemTemplate>
                                <asp:Label ID="lblName" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="25%" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Execution Date">
                            <ItemTemplate>
                                <asp:Label ID="lblExecutionDate" runat="server" Text='<%# Bind("ExecutionDate") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="20%" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Message">
                            <ItemTemplate>
                                <asp:Label ID="lblMessage" runat="server" Text='<%# Bind("Message") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="45%" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="btnDeleteLog" CommandName="DeleteLog" Text="Delete" CommandArgument='<%# Eval("Id").ToString()%>' />
                            </ItemTemplate>
                            <ItemStyle Width="5%" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </asp:Panel>
    </div>
</asp:Content>

<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="ContentFooter">
    <script type="text/javascript">

        $(function () {
            $('.datePickerAll').datepicker(calendarPick("All", "<%=CalendarImage %>"));
        });

    </script>
</asp:Content>