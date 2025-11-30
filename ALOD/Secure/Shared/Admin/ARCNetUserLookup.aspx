<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" ValidateRequest="false" Inherits="ALOD.Web.Admin.ARCNetUserLookup" Codebehind="ARCNetUserLookup.aspx.vb" %>

<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="head">
    <style type="text/css">
        .inputPanel
        {
            display:inline-block;
            width:60%;
            margin-right:3px;
            padding: 3px 4px 3px 4px;
        }
        
        .infoPanel
        {
            display:inline-block;
            float:right;
            width:38%;
            padding-top:3px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <div class="indent-small">
        <asp:Panel runat="server" ID="pnlLookupInput" CssClass="dataBlock">
            <div class="dataBlock-header">
                Lookup Parameters
            </div>

            <div class="dataBlock-body">
                <div class="inputPanel">
                    <table style="width:100%;">
                        <tr>
                            <td class="number">
                                A
                            </td>
                            <td class="label">
                                EDIPI:
                            </td>
                            <td class="value">
                                <asp:TextBox runat="server" ID="txtEDIPI" MaxLength="100" />
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                B
                            </td>
                            <td class="label">
                                Name:
                            </td>
                            <td class="value">
                                <asp:TextBox runat="server" ID="txtUserLastName" MaxLength="50" Width="80px" placeholder="Last Name" />
                                <asp:TextBox runat="server" ID="txtUserFirstName" MaxLength="50" Width="80px" placeholder="First Name" />
                                <asp:TextBox runat="server" ID="txtUserMiddleName" MaxLength="60" Width="80px" placeholder="Middle Name" />
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
                                <asp:TextBox ID="txtBeginDate" runat="server" CssClass="datePicker" MaxLength="10" onchange="DateCheck(this);" Width="94px" />
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
                                <asp:TextBox ID="txtEndDate" runat="server" CssClass="datePicker" MaxLength="10" onchange="DateCheck(this);" Width="94px" />
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                E
                            </td>
                            <td class="label">
                                Action:
                            </td>
                            <td class="value">
                                <asp:Button runat="server" ID="btnSearch" Text="Search" />
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="infoPanel">
                    <b>Last ARCNet Import:</b>
                    &nbsp;
                    <asp:Label runat="server" ID="lblLastImportExecutionDate" />
                </div>
            </div>
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlLookupOutput" CssClass="dataBlock" Visible="false">
            <div class="dataBlock-header">
                Results
            </div>

            <div class="dataBlock-body">
                <asp:GridView runat="server" SkinID="TestGV" ID="gdvResults" AutoGenerateColumns="False" Width="100%" DataKeyNames="userID" AllowPaging="true" PageSize="20" EmptyDataText="No ARCNet data found.">
                    <Columns>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton runat="server" ID="imgbEditUser" ImageAlign="AbsMiddle" SkinID="imgUserEdit" ToolTip="Edit User Info" CommandName="EditUser" CommandArgument='<%# Eval("userID") %>' />
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        </asp:TemplateField>
                        <asp:BoundField DataField="EDIPIN" HeaderText="EDIPI" ReadOnly="True" ItemStyle-Width="10%" />
                        <asp:BoundField DataField="Username" HeaderText="Username" ReadOnly="True" ItemStyle-Width="15%" />
                        <asp:BoundField DataField="Name" HeaderText="Name" ReadOnly="True" ItemStyle-Width="30%" />
                        <asp:BoundField DataField="ECT_IAAExpirationDate" HeaderText="ECT Expiration Date" ReadOnly="True" ItemStyle-Width="15%" />
                        <asp:BoundField DataField="ARCNET_IAAExpirationDate" HeaderText="ARCNet IAA Expiration Date" ReadOnly="True" ItemStyle-Width="15%" />
                        <asp:BoundField DataField="ARCNET_IAACompletionDate" HeaderText="ARCNet IAA Completion Date" ReadOnly="True" ItemStyle-Width="15%" />
                    </Columns>
                </asp:GridView>
            </div>
        </asp:Panel>
    </div>
</asp:Content>

<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="ContentFooter">
    <script type="text/javascript">
        
        $(function () {
            $('.datePicker').datepicker(calendarPick("All", "<%=CalendarImage %>"));
        });
    </script>
</asp:Content>