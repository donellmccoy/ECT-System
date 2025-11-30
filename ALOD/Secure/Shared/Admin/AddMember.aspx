<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Admin.Secure_Shared_Admin_AddMember" Codebehind="AddMember.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="Server">
    <div class="indent">
        <asp:Panel runat="server" ID="ErrorPanel" Visible="false" CssClass="ui-state-error info-block">
            <asp:Image runat="server" ID="Image1" ImageAlign="AbsMiddle" ImageUrl="~/images/warning.gif" />
            <asp:Label runat="server" ID="ErrorMessageLabel" />
        </asp:Panel>
        <asp:Panel runat="server" ID="FeedbackPanel" Visible="false" CssClass="ui-state-highlight info-block msg-panel">
            <asp:Label runat="server" ID="FeedbackMessageLabel" />
        </asp:Panel>
        <asp:Panel runat="server" ID="SearchPanel" CssClass="dataBlock">
            <div class="dataBlock-header">
                1 - Member Information
            </div>
            <div class="dataBlock-body">
                <table>
                    <tr>
                        <td class="number">
                            A
                        </td>
                        <td class="label">
                            SSN:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="SsnSearchBox" runat="server" MaxLength="9" Width="80px" oncopy="return false" onpaste="return false" oncut="return false"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            B
                        </td>
                        <td class="label">
                            Validate SSN:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="SsnSearchBox2" runat="server" MaxLength="9" Width="80px" oncopy="return false" onpaste="return false" oncut="return false"></asp:TextBox>
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
                            <asp:Button runat="server" ID="SearchButton" Text="Search" />
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <asp:Panel runat="server" ID="InputPanel" CssClass="dataBlock" Visible="false">
            <div class="dataBlock-header">
                1 - Member Information
            </div>
            <div class="dataBlock-body">
                <table>
                    <tr>
                        <td class="number">
                            A
                        </td>
                        <td class="label">
                            SSN:
                        </td>
                        <td class="value">
                            <asp:Label ID="SsnLabel" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            B
                        </td>
                        <td class="label labelRequired">
                            * Last Name:
                        </td>
                        <td class="value">
                            <asp:TextBox runat="server" ID="LastNameTextBox" Width="200px" />
                            <asp:RequiredFieldValidator runat="server" ID="LastNameValidator" ControlToValidate="LastNameTextBox"
                                Text="*" ValidationGroup="Input" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            C
                        </td>
                        <td class="label labelRequired">
                            * First Name:
                        </td>
                        <td class="value">
                            <asp:TextBox runat="server" ID="FirstNameTextBox" Width="200px" />
                            <asp:RequiredFieldValidator runat="server" ID="FirstNameValidator" ControlToValidate="FirstNameTextBox"
                                Text="*" ValidationGroup="Input" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            D
                        </td>
                        <td class="label">
                            Middle Name:
                        </td>
                        <td class="value">
                            <asp:TextBox runat="server" ID="MiddleNameTextBox" Width="200px" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            E
                        </td>
                        <td class="label labelRequired">
                            * Rank:
                        </td>
                        <td class="value">
                            <asp:DropDownList runat="server" ID="RankSelect">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            F
                        </td>
                        <td class="label labelRequired">
                            * Unit:
                        </td>
                        <td class="value">
                            <asp:DropDownList ID="UnitSelect" runat="server" DataTextField="Name" DataValueField="Value">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            G
                        </td>
                        <td class="label labelRequired">
                            * DoB:
                        </td>
                        <td class="value">
                            <asp:TextBox runat="server" ID="DobTextBox" CssClass="datePicker" Width="80px" />
                            <asp:RangeValidator runat="server" ID="DobValidator" ControlToValidate="DobTextBox" ErrorMessage="Invalid Date"
                                Type="Date" MinimumValue="01/01/1000" ValidationGroup="Input" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            H
                        </td>
                        <td class="label">
                            Component:
                        </td>
                        <td class="value">
                            <asp:DropDownList runat="server" ID="CompoSelect">
                                <asp:ListItem Value="6">Air Force Reserve</asp:ListItem>
                                <asp:ListItem Value="5">Air National Guard</asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            I
                        </td>
                        <td class="label">
                            Action:
                        </td>
                        <td class="value">
                            <asp:Button runat="server" ID="SaveMemberButton" Text="Add Member" ValidationGroup="Input" />&nbsp;
                            <asp:Button runat="server" ID="ResetButton" Text="Cancel" CausesValidation="false" />
                        </td>
                    </tr>
                </table>
                <asp:Panel runat="server" ID="ErrorListPanel" Visible="false">
                    <table>
                        <tbody>
                            <tr>
                                <td class="number">
                                    &nbsp;
                                </td>
                                <td class="label">
                                    Errors:
                                </td>
                                <td class="value">
                                    <asp:BulletedList ID="ValidationList" runat="server" CssClass="labelRequired" Visible="False">
                                    </asp:BulletedList>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </asp:Panel>
            </div>
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
