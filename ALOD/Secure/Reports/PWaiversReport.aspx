<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Reports.Secure_Reports_PWaiversReport" Codebehind="PWaiversReport.aspx.vb" %>

<%@ Register Src="~/secure/Shared/UserControls/MemberSearchResultsGrid.ascx" TagName="MemberSearchResultsGrid" TagPrefix="uc1" %>

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

            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);

            function EndRequestHandler(sender, args) {
                $('.datePicker').datepicker(
                   {
                       showOn: 'both',
                       buttonImage: '<%= CalendarImage %>',
                       buttonImageOnly: true,
                       changeMonth: true,
                       changeYear: true
                   }
                );
            }
        });
    
    </script>
    <div class="indent">
        <asp:Panel runat="server" ID="InputPanel" CssClass="dataBlock">
            <div class="dataBlock-header">
                1 - Report Options
            </div>
            <div class="dataBlock-body">
                <asp:UpdatePanel runat="server" ID="upnlMemberLookup" ChildrenAsTriggers="true">
                    <ContentTemplate>
                <table runat="server" id="tbl1">
                    <tr runat="server" id="tr1">
                        <td class="number">
                            <asp:Label ID="ReportLabel1" runat="server" Text="A"></asp:Label>
                        </td>
                        <td class="label">
                            Select Unit:
                        </td>
                        <td class="value">
                            <asp:DropDownList ID="ddlUnit" DataTextField="Name" DataValueField="Value" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr runat="server" id="tr2">
                        <td class="number">
                            <asp:Label ID="ReportLabel2" runat="server" Text="B"></asp:Label>
                        </td>
                        <td class="label">
                            Include subordinate units:
                        </td>
                        <td class="value">
                            <asp:CheckBox ID="chkSubordinateUnit" runat="server" Checked="True" />
                        </td>
                    </tr>
                    <tr runat="server" id="tr3">
                        <td class="number">
                            <asp:Label ID="ReportLabel3" runat="server" Text="C"></asp:Label>
                        </td>
                        <td class="label">
                            Start Date:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="txtBeginDate" runat="server" CssClass="datePicker" MaxLength="10"
                                onchange="DateCheck(this);" Width="94px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr runat="server" id="tr4">
                        <td class="number">
                            <asp:Label ID="ReportLabel4" runat="server" Text="D"></asp:Label>
                        </td>
                        <td class="label">
                            End Date:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="txtEndDate" runat="server" CssClass="datePicker" MaxLength="10"
                                onchange="DateCheck(this);" Width="94px"></asp:TextBox>
                            &nbsp;
                        </td>
                    </tr>
                            <tr runat="server" id="trRadioButtons">
                                <td class="number">
                                    E
                                </td>
                                <td class="label">
                                    Lookup By:
                                </td>
                                <td class="value">
                                    <asp:RadioButton runat="server" ID="rdbSSN" Text="SSN" GroupName="LookupChoice" AutoPostBack="true" Checked="True" />
                                    <asp:RadioButton runat="server" ID="rdbName" Text="Name" GroupName="LookupChoice" AutoPostBack="true" />
                                </td>
                            </tr>
                            <tr runat="server" class="tr-SSN" id="trSSN">
                        <td class="number">
                                    <asp:Label ID="ReportLabel5" runat="server" Text="F"></asp:Label>
                        </td>
                        <td class="label">
                            SSN:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="txtSSN" runat="server" MaxLength="9"></asp:TextBox>
                            &nbsp;
                        </td>
                    </tr>
                            <tr runat="server" class="tr-Name" id="trName">
                                <td class="number">
                                    <asp:Label runat="server" ID="lblReportMemberName" Text="F"></asp:Label>
                                </td>
                                <td class="label">
                                    Member Name (last / first / middle):
                                </td>
                                <td class="value">
                                    <asp:TextBox runat="server" ID="txtMemberLastName" MaxLength="50" Width="80px" placeholder="Last Name"></asp:TextBox>
                                    <asp:TextBox runat="server" ID="txtMemberFirstName" MaxLength="50" Width="80px" placeholder="First Name"></asp:TextBox>
                                    <asp:TextBox runat="server" ID="txtMemberMiddleName" MaxLength="60" Width="80px" placeholder="Middle Name"></asp:TextBox>
                                    &nbsp;
                                    <asp:Label runat="server" ID="lblMemberNotFound" CssClass="labelRequired" Text="Member Not Found" Visible="false"></asp:Label>
                                    &nbsp;
                                    <asp:Label runat="server" ID="lblInvalidMemberName" CssClass="labelRequired" Text="Invalid Name" Visible="false"></asp:Label>
                                </td>
                            </tr>
        <%--                    <tr runat="server" id="tr6">
                                <td class="number">
                                    <asp:Label ID="ReportLabel6" runat="server" Text="F"></asp:Label>
                                </td>
                                <td class="label">
                                    &nbsp; Interval:
                                </td>
                                <td class="value">
                                    &nbsp;</td>
                            </tr>--%>
                    <tr runat="server" id="tr6">
                        <td class="number">
                                    <asp:Label ID="ReportLabel7" runat="server" Text="G"></asp:Label>
                        </td>
                        <td class="label">
                            SortOrder?
                        </td>
                        <td class="value">
                            <asp:DropDownList ID="SortOrderDDL" runat="server">
                                <asp:ListItem Selected="True" Value="Name">Name</asp:ListItem>
                                <asp:ListItem Value="SSN">SSN</asp:ListItem>
                                <asp:ListItem Value="UnitName">Unit</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <table runat="server" id="tbl2">
                    <tr runat="server" id="tr7">
                        <td class="number">
                            <asp:Label ID="ReportLabel8" runat="server" Text="H"></asp:Label>
                        </td>
                        <td class="label">
                            Action:
                        </td>
                        <td class="value">
                            <asp:Button ID="ReportButton" runat="server" Text="Run Report" />
                        </td>
                    </tr>
                    <tr runat="server" id="tr8">
                        <td colspan="2">
                            <asp:DropDownList ID="IntervalDropDownList" runat="server" Visible="False">
                                <asp:ListItem Value="0">New</asp:ListItem>
                                <asp:ListItem Value="30">30 days</asp:ListItem>
                                <asp:ListItem Value="45">45 days</asp:ListItem>
                                <asp:ListItem Value="90">Expired</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td>
                            <asp:Label ID="errorLbl" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>

        <asp:Panel runat="server" ID="MemberSelectionPanel" Visible="False" CssClass="dataBlock">
            <div class="dataBlock-header">
                2 - Member Selection
            </div>
            <div class="dataBlock-body">
                <uc1:MemberSearchResultsGrid runat="server" ID="ucMemberSelectionGrid"></uc1:MemberSearchResultsGrid>
        </div>
        </asp:Panel>
    </div>
    <div id="spSpacer" class="emptyItem" style="margin-top: 0px; margin-bottom: 4px;
        height: 22px;">
        <div id="spWait" class="" style="display: none;">
            &nbsp;<asp:Image runat="server" ID="imgWait" SkinID="imgBusy" AlternateText="busy"
                ImageAlign="AbsMiddle" />&nbsp;Loading...
        </div>
    </div>
    <br />
    <div class="indent">
        <asp:Panel ID="ResultsPanel" runat="server" Visible="False" CssClass="dataBlock">
            <div class="dataBlock-header">
                2 - Report Results
            </div>
        <div>
            <asp:Label runat="server" ID="GridViewErrorLabel" ForeColor="Red" Font-Bold="true" />
        </div>
            <div class="dataBlock-body">
        <asp:GridView ID="ReportGrid" runat="server" Width="100%" AllowSorting="True" AutoGenerateColumns="False"
            PageSize="10" EmptyDataText="No record found" AllowPaging="True">
            <Columns>
                <asp:TemplateField HeaderText="Case Id" SortExpression="CaseId" ItemStyle-Width="120px">
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("RefId").ToString()  %>'
                            CommandName="view" Text='<%# Eval("CaseId") %>'>
                        </asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="SSN" ItemStyle-Width="80px" ItemStyle-Wrap="true" HeaderText="SSN"
                    HtmlEncode="false" SortExpression="SSN" />
                <asp:BoundField DataField="Name" HeaderText="Name" ItemStyle-Width="150px" ItemStyle-Wrap="true"
                    SortExpression="Name" />
                <asp:BoundField DataField="UnitName" HeaderText="Unit" SortExpression="UnitName"
                    ItemStyle-Width="200px" ItemStyle-Wrap="true" />
                <asp:BoundField DataField="ApprovalDate" HeaderText="Date Of Waiver" SortExpression="ApprovalDate"
                    ItemStyle-Width="60px" DataFormatString="{0:dd-MMMM-yy }" />
                <asp:BoundField DataField="DaysRemaining" HeaderText="# of days left" SortExpression="DaysRemaining"
                    ItemStyle-Width="60px" />
                <asp:BoundField DataField="DaysUsed" HeaderText="# of days used" SortExpression="DaysUsed"
                    ItemStyle-Width="60px" />
                <asp:BoundField DataField="MemberWaivers" HeaderText="total # of waivers" SortExpression="MemberWaivers"
                    ItemStyle-Width="60px" />
            </Columns>
        </asp:GridView>
            </div>
        <br />
        </asp:Panel>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFooter" runat="Server">
</asp:Content>
