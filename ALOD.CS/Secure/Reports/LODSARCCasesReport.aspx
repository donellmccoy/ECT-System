<%@ Page Title="" Language="C#" AutoEventWireup="false" MasterPageFile="~/Secure/Secure.master" CodeBehind="LODSARCCasesReport.aspx.cs" Inherits="ALOD.Web.Reports.LODSARCCasesReport" MaintainScrollPositionOnPostback="true"%>

<%@ Register Src="../Shared/UserControls/SignatureBlock.ascx" TagName="SignatureBlock" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">

    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="server">
    <div>
        <uc1:SignatureBlock ID="SigBlock" runat="server" />
        <asp:Label runat="server" ID="ErrorLabel" ForeColor="Red" Font-Bold="true" />
    </div>
    <asp:Panel runat="server" ID="pnlReportOptions" CssClass="indent-small dataBlock">
        <div class="dataBlock-header">
            1 - Report Options
        </div>
        <div class="dataBlock-body">
            <table>
                <tr>
                    <td class="number">
                        <asp:Label ID="lblBeginDateRow" runat="server" Text="A"></asp:Label>
                    </td>
                    <td class="label">
                        Begin Date:
                    </td>
                    <td class="value">
                        <asp:TextBox ID="txtBeginDate" runat="server" CssClass="datePicker" MaxLength="10" onchange="DateCheck(this);" Width="94px" />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        <asp:Label ID="lblEndDateRow" runat="server" Text="B"></asp:Label>
                    </td>
                    <td class="label">
                        End Date:
                    </td>
                    <td class="value">
                        <asp:TextBox ID="txtEndDate" runat="server" CssClass="datePicker" MaxLength="10" onchange="DateCheck(this);" Width="94px" />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        <asp:Label ID="Label1" runat="server" Text="C"></asp:Label>
                    </td>
                    <td class="label">
                        Restriction Status:
                    </td>
                    <td class="value">
                        <asp:DropDownList runat="server" ID="ddlRestrictionStatus" />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        <asp:Label ID="Label2" runat="server" Text="D"></asp:Label>
                    </td>
                    <td class="label">
                        Completion Status:
                    </td>
                    <td class="value">
                        <asp:RadioButtonList runat="server" ID="rblCompletionStatus" RepeatDirection="Horizontal">
                            <asp:ListItem Value="1" Selected="True">All</asp:ListItem>
                            <asp:ListItem Value="2">Active</asp:ListItem>
                            <asp:ListItem Value="3">Closed</asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        <asp:Label ID="lblOutputFormatRow" runat="server" Text="E"></asp:Label>
                    </td>
                    <td class="label">
                        Output Format:
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
                        <img src="../../images/page_white_text.gif" alt="Export to CSV" style="vertical-align: middle" />
                        &nbsp;
                        <asp:RadioButton ID="rdbOutputXml" runat="server" GroupName="Output" Text="XML" />
                        <img src="../../images/page_white_code.gif" alt="Export to XML" style="vertical-align: middle"  />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        <asp:Label ID="lblActionRow" runat="server" Text="F"></asp:Label>
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
    </asp:Panel>

    <asp:Panel runat="server" ID="pnlResults" Visible="false">
        <asp:Panel runat="server" ID="pnlReportResults" CssClass="indent-small dataBlock" Visible="true">
            <div class="dataBlock-header">
                2 - Report Results
            </div>
            <div class="dataBlock-body">
                <asp:GridView runat="server" SkinID="TestGV" ID="gdvResults" AutoGenerateColumns="False" Width="100%" DataKeyNames="RefId" AllowPaging="true" PageSize="20">
                    <Columns>
                        <asp:TemplateField HeaderText="Case Id">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblCaseId" />
                            </ItemTemplate>
                            <ItemStyle Width="15%" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Is Restricted">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblIsRestricted" />
                            </ItemTemplate>
                            <ItemStyle Width="5%" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Status">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblStatus" />
                            </ItemTemplate>
                            <ItemStyle Width="20%" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Member Name">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblMemberName" />
                            </ItemTemplate>
                            <ItemStyle Width="17%" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Member SSN">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblSSN" CssClass="numericResult" />
                            </ItemTemplate>
                            <ItemStyle Width="10%" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Member Unit">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblMemberUnit" />
                            </ItemTemplate>
                            <ItemStyle Width="20%" />
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-Width="85px" Visible="false">
                                <ItemTemplate>
                                    <asp:linkbutton id="appeallinkbutton" runat="server" causesvalidation="false"
                                        CommandArgument='<%# CType(Eval("RefId"), String) + ";" + CType(Eval("WorkflowId"), String)%>' visible='<%# DetermineAPLinkVisibility(Eval("isComplete"), Eval("CaseId"), Eval("RefId"), Eval("ModuleId"), Eval("WorkflowId"), Eval("isRestricted"))%>'
                                        commandname="appeal" text='Initiate Appeal Request'></asp:linkbutton>
                                        
                                </ItemTemplate>
                            </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <asp:Label runat="server" ID="GridViewErrorLabel" ForeColor="Red" Font-Bold="true" />
                <asp:Panel runat="server" ID="pnlEmptyResults" Visible="false">
                    <asp:Label runat="server" ID="lblEmptyResults" />
                </asp:Panel>
            </div>
        </asp:Panel>
    </asp:Panel>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentFooter" runat="server">
    <script type="text/javascript">
       $(function () {

            $('.datePicker').datepicker(calendarPick("Past", "<%=CalendarImage %>"));
       });
    </script>
</asp:Content>