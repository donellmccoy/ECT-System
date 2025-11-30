<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Reports.Secure_Reports_MetricsReport" Codebehind="MetricsReport.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .total-row
        {
            border-left: 1px solid #C0C0C0;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="Server">
    <div class="indent">
                <div class="dataBlock">
                    <div class="dataBlock-header">
                        1 - Report Options
                    </div>
                    <div class="dataBlock-body">
                        <table>
                            <tr>
                                <td class="number">
                                    A
                                </td>
                                <td class="label">
                                    Select Unit:
                                </td>
                                <td class="value">
                                    <asp:DropDownList ID="UnitSelect" runat="server" DataTextField="Name" DataValueField="Value">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td class="number">
                                    B
                                </td>
                                <td class="label">
                                    Include subordinate units:
                                </td>
                                <td class="value">
                                    <asp:CheckBox ID="IncludeSubordinatesCheck" runat="server" Checked="True" />
                                </td>
                            </tr>
                            <tr>
                                <td class="number">
                                    C
                                </td>
                                <td class="label">
                                    Begin Date:
                                </td>
                                <td class="value">
                                    <asp:TextBox ID="BeginDateBox" runat="server" CssClass="datePicker" MaxLength="10"
                                        onchange="DateCheck(this);" Width="94px"></asp:TextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="number">
                                    D
                                </td>
                                <td class="label">
                                    End Date:
                                </td>
                                <td class="value">
                                    <asp:TextBox ID="EndDateBox" runat="server" CssClass="datePicker" MaxLength="10"
                                        onchange="DateCheck(this);" Width="94px"></asp:TextBox>
                                    &nbsp;
                                </td>
                            </tr>
                            <tr>
                                <td class="number">
                                    E
                                </td>
                                <td class="label">
                                    Include Completed:
                                </td>
                                <td class="value">
                                    <asp:RadioButtonList ID="StatusSelect" runat="server" RepeatDirection="Horizontal">
                                        <asp:ListItem Selected="True" Value="2">All</asp:ListItem>
                                        <asp:ListItem Value="0">Active</asp:ListItem>
                                        <asp:ListItem Value="1">Closed</asp:ListItem>
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr>
                                <td class="number">
                                    F
                                </td>
                                <td class="label">
                                    Action:
                                </td>
                                <td class="value">
                                    <asp:Button ID="ReportButton" runat="server" Text="Run Report" />
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                </td>
                                <td>
                                    <asp:Label ID="ErrorMessageLabel" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
                <br />
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
                <asp:Panel runat="server" ID="ResultsPanel" Visible="false">
                    <asp:GridView runat="server" ID="ResultsGrid" AutoGenerateColumns="False" Width="100%">
                        <Columns>
                            <asp:TemplateField HeaderText="Unit">
                                <ItemTemplate>
                                    <asp:LinkButton runat="server" ID="UnitLink" Text='<%# Eval("long_name") %>' CommandArgument='<%# Eval("cs_id") %>'
                                        CommandName="<%# COMMAND_VIEW_UNIT %>" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="med_tech" HeaderText="Medical Unit" />
                            <asp:BoundField DataField="med_off" HeaderText="Medical Officer" />
                            <asp:BoundField DataField="unit_cmdr" HeaderText="Unit CC" />
                            <asp:BoundField DataField="wing_ja" HeaderText="Wing JA" />
                            <asp:BoundField DataField="wing_cc" HeaderText="Wing CC" />
                            <asp:BoundField DataField="invest" HeaderText="Formal Inv" />
                            <asp:BoundField DataField="board" HeaderText="HQ Board" />
                            <asp:TemplateField HeaderText="Total">
                                <ItemStyle CssClass="total-row" />
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="TotalLabel" Text="0.0" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            No results found
                        </EmptyDataTemplate>
                        <EmptyDataRowStyle CssClass="emptyItem" />
                    </asp:GridView>
                </asp:Panel>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFooter" runat="Server">
    <script type="text/javascript">

        $(function () {
            $('.datePicker').datepicker(calendarPick("Past", "<%=CalendarImage %>"));
        });
    </script>
</asp:Content>
