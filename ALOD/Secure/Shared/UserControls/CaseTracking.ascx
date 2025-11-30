<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="CaseTracking.ascx.vb" Inherits="ALOD.Web.UserControls.Secure_Shared_UserControls_CaseTracking" %>


<%@ Import Namespace="ALODWebUtility.Common" %>
<%@ Import Namespace="ALOD.styles" %>

<asp:Panel ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    <table style="width: 100%; margin-bottom: 6px;">
        <tbody>
            <tr>
                <td style="width: 33%; text-align: left;">
                    <asp:CheckBox ID="chkShowAll" runat="server" AutoPostBack="True" Text="Show All" />
                </td>
                <td style="width: 33%; text-align: center;">
                    <%--<a href='#' onclick="showWorkflowUsers('<%= RequestId.ToString() %>', '<%= ModuleType %>', '<%= wsStatus %>', '<%= MemberUnitId %>');">
                        <asp:Image runat="server" ID="imgSearch" SkinID="imgUserSmall" ImageAlign="AbsMiddle"
                            AlternateText="Show users who can act on this case" />&nbsp;Who is working this
                        case?</a>--%>
                </td>
                <td style="width: 33%; text-align: right;">
                    <a href="#" onclick="printTracking();">
                        <asp:Image runat="server" ID="imgPrint" SkinID="imgPrintSmall" ImageAlign="AbsMiddle"
                            AlternateText="Print" />&nbsp;Print Tracking </a>
                </td>
            </tr>
        </tbody>
    </table>
    <div class="gridViewMain">
        <table style="width: 912px;">
            <thead>
                <tr class="gridViewHeader">
                    <th class="expand">
                    </th>
                    <th class="procName">
                        Process Name
                    </th>
                    <th class="startDate">
                        Start Date
                    </th>
                    <th class="endDate">
                        End Date
                    </th>
                    <th class="dayCount">
                        Days in process
                    </th>
                    <th class="completedBy">
                        Completed By
                    </th>
                </tr>
            </thead>
        </table>
        <asp:Repeater runat="server" ID="TrackingList">
            <ItemTemplate>
                <asp:Panel runat="server" ID="TitlePanel">
                    <table style="width: 912px;">
                        <tbody>
                            <tr class="gridViewRow">
                                <td class="expand">
                                    <asp:Image ID="ExpandImage" runat="server" ImageUrl="~/images/arrow_down.gif" CssClass="pointer"
                                        AlternateText="Toggle Details" />
                                </td>
                                <td class="procName">
                                    <%#Eval("WorkflowStatus.Description")%>
                                </td>
                                <td class="startDate">
                                    <%#Eval("StartDate", "{0:MM/dd/yyyy}")%>
                                </td>
                                <td class="endDate">
                                    <%#Eval("EndDate", "{0:MM/dd/yyyy}")%>&nbsp;
                                </td>
                                <td class="dayCount">
                                    <%#Eval("DaysInStep.TotalDays", "{0:N2}")%>
                                </td>
                                <td class="completedBy">
                                    <asp:HyperLink ID="lnkUser" runat="server" Text=""></asp:HyperLink>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </asp:Panel>
                <asp:Panel runat="server" ID="DetailsPanel" CssClass="collapsePanel hidden">
                    <asp:Repeater runat="server" ID="DetailsRepeater" OnItemDataBound="DetailsList_ItemDataBound">
                        <HeaderTemplate>
                            <table style="width: 912px; border-top: 1px solid black; border-bottom: 1px solid black;">
                                <tbody>
                        </HeaderTemplate>
                        <FooterTemplate>
                            </tbody> </table>
                        </FooterTemplate>
                        <ItemTemplate>
                            <tr class="gridViewAlternateRow">
                                <td style="width: 130px;">
                                    <%#Eval("ActionDate", "{0:MM/dd/yyyy HHmm}")%>
                                </td>
                                <td style="width: 180px;">
                                    <asp:HyperLink ID="lnkAction" runat="server" Text='<%# Eval("actionName") %>'></asp:HyperLink>
                                </td>
                                <td style="width: 472px;">
                                    <%#NullStringToEmptyString(Eval("Notes")).Replace(Environment.NewLine, "<br />")%>&nbsp;
                                </td>
                                <td style="width: 130px;">
                                    <asp:HyperLink ID="lnkUser" runat="server" Text='<%# Eval("lastname") %>'></asp:HyperLink>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <AlternatingItemTemplate>
                            <tr class="gridViewRow" style="background-color: #EEE;">
                                <td style="width: 130px;">
                                    <%#Eval("ActionDate", "{0:MM/dd/yyyy HHmm}")%>
                                </td>
                                <td style="width: 180px;">
                                    <asp:HyperLink ID="lnkAction" runat="server" Text='<%# Eval("actionName") %>'></asp:HyperLink>
                                </td>
                                <td style="width: 472px;">
                                    <%#NullStringToEmptyString(Eval("Notes")).Replace(Environment.NewLine, "<br />")%>&nbsp;
                                </td>
                                <td style="width: 130px;">
                                    <asp:HyperLink ID="lnkUser" runat="server" Text='<%# Eval("lastname") %>'></asp:HyperLink>
                                </td>
                            </tr>
                        </AlternatingItemTemplate>
                    </asp:Repeater>
                </asp:Panel>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</asp:Panel>
<asp:Panel ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">
    <asp:ScriptManagerProxy runat="server" ID="smp">
        <Scripts>
            <asp:ScriptReference Path="~/Script/changelog.js" />
            <asp:ScriptReference Path="~/Script/tracking.js" />
        </Scripts>
    </asp:ScriptManagerProxy>

    <script type="text/javascript">

        function toggleDetails(id, img) {
            $('#' + id).toggle('blind', {}, 500);

            var image = $('#' + img);
            var src = image.attr('src');

            if (src.match(/up/)) {
                src = src.replace('up', 'down');
            }
            else {
                src = src.replace('down', 'up');
            }

            image.attr('src', src);
        }

        function printTracking() {
            var pathname = '/secure/Shared/PrintCaseTracking.aspx'
            var url = $_HOSTNAME + pathname + '?refId=<%=RequestId.ToString() %>&module=<%=ModuleType.ToString() %>&CaseId=<%=CaseId %>&showAll=' + String(element('<%= chkShowAll.ClientId %>').checked);

            showPopup({
                'Url': url,
                'Width': 800,
                'Height': 600,
                'Center': true,
                'Reload': false,
                Resizable: true
            });
        }
    </script>

</asp:Panel>
