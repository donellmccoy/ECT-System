<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.PH.Start" Codebehind="Start.aspx.vb" %>

<%@ Import Namespace="ALODWebUtility.Common" %>


<%@ Register Src="../Shared/UserControls/SignatureBlock.ascx" TagName="SignatureBlock" TagPrefix="uc1" %>

<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="head">
</asp:Content>

<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="ContentFooter">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <div class="indent-small">
        <asp:Panel runat="server" ID="pnlPHRMUs" CssClass="dataBlock">
            <div class="dataBlock-header">
                DPH Information
            </div>

            <div class="dataBlock-body">
                <asp:UpdatePanel runat="server">
                    <ContentTemplate>
                        <table>
                            <tr>
                                <td class="number">
                                    A
                                </td>
                                <td class="label">
                                    DPH Name:
                                </td>
                                <td>
                                    <asp:Label runat="server" ID="lblDPHName" />
                                </td>
                            </tr>
                            <tr>
                                <td class="number">
                                    B
                                </td>
                                <td class="label">
                                    DPH Unit:
                                </td>
                                <td>
                                    <asp:Label runat="server" ID="lblDPHUnit" />
                                </td>
                            </tr>
                            <tr runat="server" id="trReportingPeriod">
                                <td class="number">
                                    C
                                </td>
                                <td class="label">
                                    Select Reporting Period:
                                </td>
                                <td>
                                    <asp:DropDownList runat="server" ID="ddlYear" AutoPostBack="false" />
                                    <asp:DropDownList runat="server" ID="ddlMonth" AutoPostBack="false" />
                                </td>
                            </tr>
                            <tr>
                                <td class="number">
                                    <asp:Label runat="server" ID="sectionAction" Text="D"></asp:Label>
                                </td>
                                <td class="label">
                                    Action:
                                </td>
                                <td class="value">
                                    <asp:Button ID="CreateButton" runat="server" Text="Start Case" />
                                </td>
                            </tr>
                            <tr runat="server" id="ValidationErrorsRow" visible="false">
                                <td class="number">
                                    &nbsp;
                                </td>
                                <td class="label">
                                    Errors:
                                </td>
                                <td class="value">
                                    <asp:BulletedList ID="ValidationList" runat="server" CssClass="labelRequired" Visible="False" />
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlWingPHHistory" CssClass="dataBlock">
            <div class="dataBlock-header">
                <asp:Label runat="server" ID="lblHistoryPanelTitle" />
            </div>

            <div class="dataBlock-body">
                <asp:GridView runat="server" ID="gdvPHHistory" AutoGenerateColumns="false" Width="100%" AllowPaging="true" AllowSorting="false" Visible="false">
                    <Columns>
                        <asp:TemplateField HeaderText="Case Id" SortExpression="Case_Id">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkRefSCID" runat="server" CommandArgument='<%# Eval("RefId").ToString() + ";" + Eval("Module_Id").ToString()  %>'
                                    CommandName="view" Text='<%# Eval("Case_Id") %>'>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="Reporting_Period" HeaderText="Reporting Period" SortExpression="Reporting_Period" />
                        <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                        <asp:BoundField DataField="Created_Date" HeaderText="Created Date" SortExpression="Created_Date" />
                        <asp:BoundField DataField="Completed_Date" HeaderText="Completed Date" SortExpression="Completed_Date" />
                    </Columns>
                    <EmptyDataRowStyle CssClass="emptyItem" />
                    <EmptyDataTemplate>
                        No other PH cases exist for this Wing RMU
                    </EmptyDataTemplate>
                </asp:GridView>
                <div style="text-align: center;">
                    <asp:Label ID="lblNoHistory" Text="No other PH cases found" CssClass="labelNormal" runat="server" Visible="false" />
                </div>
            </div>
        </asp:Panel>
    </div>
</asp:Content>