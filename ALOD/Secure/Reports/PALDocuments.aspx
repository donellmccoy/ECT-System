<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Reports.Secure_Reports_PALDocsReport" Codebehind="PALDocuments.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="Server">
    <script type="text/javascript">

        $(function () {

            $('.datePicker').datepicker(calendarPick("All", "<%=CalendarImage %>"));

            $('#' + '<%=ReportButton.ClientID%>').click(function () {
                $('#' + '<%=errorLbl.ClientID%>').text("");
                var beginTxt = '#' + '<%=txtBeginDate.ClientID%>';
                var endTxt = '#' + '<%=txtEndDate.ClientID%>';


                if ($(beginTxt).val().trim() === ''
                    && $(endTxt).val().trim() === '') {
                    var dtEnd = new Date();
                    var dtBegin = new Date(dtEnd.getFullYear(), dtEnd.getMonth() - 1, dtEnd.getDay());
                    $(beginTxt).val($.datepicker.formatDate("mm/dd/yy", dtBegin));
                    $(endTxt).val($.datepicker.formatDate("mm/dd/yy", dtEnd));
                    return true;

                }

                if ($(beginTxt).val().trim() === ''
                    || $(endTxt).val().trim() === '') {

                    $('#' + '<%=errorLbl.ClientID%>').text("Please enter a valid set of dates or leave date fields empty to default to the last month.").css("color", "red");
                    return false;
                }

                if (Date.parse($(beginTxt).val().trim())
                 > Date.parse($(endTxt).val().trim())) {

                    $('#' + '<%=errorLbl.ClientID%>').text("End date should be  after the Start date.").css("color", "red");
                    return false;
                }
            });
        });
    </script>
    <div class="indent">
        <div class="dataBlock">
            <div class="dataBlock-header">
                1 - Report Options
            </div>
            <div class="dataBlock-body">
                <table runat="server" id="tbl1">
<%--                    <tr runat="server" id="tr3" visible="false">
                        <td class="number">
                            <asp:Label ID="ReportLabel3" runat="server" Text="C"></asp:Label>
                        </td>
                        <td class="label">
                            Start Date:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="txtBeginDate" runat="server" CssClass="datePicker" MaxLength="10" onchange="DateCheck(this);" Width="94px" Visible="false"></asp:TextBox>
                        </td>
                    </tr>
                    <tr runat="server" id="tr4" visible="false">
                        <td class="number">
                            <asp:Label ID="ReportLabel4" runat="server" Text="D"></asp:Label>
                        </td>
                        <td class="label">
                            End Date:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="txtEndDate" runat="server" CssClass="datePicker" MaxLength="10" onchange="DateCheck(this);" Width="94px" Visible="false"></asp:TextBox>
                            &nbsp;
                        </td>
                    </tr>--%>
                    <tr runat="server" id="tr5">
                        <td class="number">
                            <asp:Label ID="ReportLabel5" runat="server" Text="A"></asp:Label>
                        </td>
                        <td class="label">
                            Last 4 SSN:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="txtSSN" runat="server" MaxLength="4"></asp:TextBox>
                            &nbsp;
                        </td>
                    </tr>
                    <tr runat="server" id="tr7">
                        <td class="number">
                            <asp:Label ID="Label1" runat="server" Text="B"></asp:Label>
                        </td>
                        <td class="label">
                            Last Name:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="txtLastName" runat="server" MaxLength="50"></asp:TextBox>
                            &nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label ID="ReportLabel8" runat="server" Text="C"></asp:Label>
                        </td>
                        <td class="label">
                            Action:
                        </td>
                        <td class="value">
                            <asp:Button ID="ReportButton" runat="server" Text="Run Report" />
                        </td>
                    </tr>
                </table>
                <asp:Label ID="errorLbl" runat="server"></asp:Label>

            </div>
        </div>
    </div>
    <div id="spSpacer" class="emptyItem" style="margin-top: 0px; margin-bottom: 4px; height: 22px;">
        <div id="spWait" class="" style="display: none;">
            &nbsp;
            <asp:Image runat="server" ID="imgWait" SkinID="imgBusy" AlternateText="busy" ImageAlign="AbsMiddle" />&nbsp;Loading...
        </div>
    </div>
    <br />
    <div id="rpt">
        <div>
            <asp:Label runat="server" ID="GridViewErrorLabel" ForeColor="Red" Font-Bold="true" /><br />
            NOTE: Document URLs are temporary and may expire
        </div>
        <asp:GridView ID="ReportGrid" runat="server" Width="100%" AllowSorting="True" AutoGenerateColumns="False" PageSize="10" EmptyDataText="No record found" AllowPaging="True">
            <Columns>
                <asp:BoundField DataField="Last4SSN" ItemStyle-Width="80px" ItemStyle-Wrap="true" HeaderText="Last 4 SSN" HtmlEncode="false" SortExpression="SSN" />
                <asp:BoundField DataField="LastName" HeaderText="Last Name" ItemStyle-Width="150px" ItemStyle-Wrap="true" SortExpression="Name" />
                <asp:BoundField DataField="DocYear" HeaderText="Doc Year" ItemStyle-Width="150px" ItemStyle-Wrap="true" SortExpression="DocYear" />
                <asp:BoundField DataField="DocMonth" HeaderText="Doc Month" ItemStyle-Width="150px" ItemStyle-Wrap="true" SortExpression="DocMonth" />
                <asp:TemplateField HeaderText="Document URL" ItemStyle-Width="250px">
                    <ItemTemplate>
                        <asp:HyperLink ID="DocumentURL" runat="server" NavigateUrl='<%# DocumentDao.GetDocumentViewerUrl(Eval("PALDocId")) %>' Text='View PDF Document' Target="_blank" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
        <br />
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFooter" runat="Server">
</asp:Content>
