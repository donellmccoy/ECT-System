<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.InactiveSettings" CodeBehind="InactiveSettings.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="Server">
    <asp:Panel runat="server" ID="FeedbackPanel" Visible="false" CssClass="ui-state-highlight info-block msg-panel">
        <asp:Label runat="server" ID="FeedbackMessageLabel" />
    </asp:Panel>
    <asp:Panel runat="server" ID="SearchPanel" CssClass="dataBlock">

        <div class="dataBlock-header">
            Inactive Notification Settings
        </div>
        <div class="dataBlock-body">
            <table>
                <tr>
                    <td></td>

                    <td>Disable Accounts After
                    </td>
                    <td style="padding-left: 20px;"">
                        <asp:TextBox ID="inactive" runat="server" MaxLength="3" Width="28px"></asp:TextBox>
                        Day(s) of Inactivity
                        </td>
                    <td>
                        <asp:RegularExpressionValidator ID="rgExpCom" runat="server" ControlToValidate="inactive"
                                Display="Dynamic" ErrorMessage="Invalid Value." ValidationExpression="[1-9]\d{0,2}"
                                ValidationGroup="settings" SetFocusOnError="True" Text="Enter a Value between 1 and 999"></asp:RegularExpressionValidator>

                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>Send Notification
                    </td>
                    <td style="padding-left: 20px;">
                        <asp:TextBox ID="notif" runat="server" MaxLength="3" Width="28px"></asp:TextBox>
                        Day(s) Before Disabling
                    </td>
                    <td>

                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="notif"
                            Display="Dynamic" ErrorMessage="Invalid Value." ValidationExpression="[1-9]\d{0,2}"
                            ValidationGroup="settings" SetFocusOnError="True" Text="Enter a Value between 1 and 999"></asp:RegularExpressionValidator>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>Template:
                    </td>

                    <td class="align-left" style="width: 220px; padding-left: 20px;">
                        <asp:DropDownList ID="cbData" Width="100%" runat="server" DataTextField="Text" DataValueField="Value">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        Active:
                    </td>
                    <td style="padding-left:20px;">
                        <asp:CheckBox id="active" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        
                    </td>
                    <td style="padding-left:20px;">
                        <asp:Button  runat="server" ID="updateButton" text="Save" OnClick="updateButton_Click" ValidationGroup="settings"/>
                    </td>

                </tr>
            </table>
        </div>

    </asp:Panel>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentFooter" runat="Server">
</asp:Content>
