<%@ Page Language="C#" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Help.Secure_Shared_Admin_MyAccount" Title="Untitled Page"
    MaintainScrollPositionOnPostback="true" CodeBehind="MyAccount.aspx.cs" %>

<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="head">
</asp:Content>
<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="ContentFooter">
    <script type="text/javascript">

        $(document).ready(function () {
            $('#srchBox').dialog({
                autoOpen: false,
                modal: true,
                resizable: true,
                width: 550,
                height: 320,
                buttons: {
                    'Select': function () {
                        SetReportingUnit();
                        $(this).dialog('close');
                    },
                    'Cancel': function () {
                        CancelSelection();
                        $(this).dialog('close');
                    }
                }
            });
        });

        // Show Searcher
        function showSearcher(title, targetId, targetlbl) {
            initializeUnitSearcher();
            // Set Client controls where unit Id and unit names will be transferred
            element('<%=hdnIdClient.ClientId %>').value = targetId;
            element('<%=hdnNameClient.ClientId %>').value = targetlbl;
            $('#srchBox').dialog('open');
        }

        // Client accepted so Set Corresponding reporting units 
        function SetReportingUnit() {
            var srcherId = '<%=unitSearcher.ClientId %>'

            if (element(srcherId + '_hdnSelectedUnit').value === "") {
                return false;
            }

            // Id is stroed in a label control.Transfer the value from the control
            element(element('<%=hdnIdClient.ClientId %>').value).value = element(srcherId + '_hdnSelectedUnit').value;
            // Name is stored in a label.Transfer the value from the control
            element(element('<%=hdnNameClient.ClientId %>').value).value = element(srcherId + '_hdnSelectedUnitName').value;
            return false;

        }

        // Client cancelled so ignore the dialog values
        function CancelSelection() {
            var srcherId = '<%=unitSearcher.ClientId %>'
            element(srcherId + '_hdnSelectedUnit').value = "";
            element(srcherId + '_hdnSelectedUnitName').value = "";
            return false;
        }

    </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <!-- Unit search control -->
    <div id="srchBox" class="hidden" title="Find Unit">
        <lod:unitSearcher ID="unitSearcher" ActiveOnly="true" runat="server" />
    </div>
    <input type="hidden" id="hdnIdClient" runat="Server" />
    <input type="hidden" id="hdnNameClient" runat="Server" />
    <!-- end search control -->

    <div class="indent-small">
        <div class="dataBlock">
            <div class="dataBlock-header">
                1 - Account Status
            </div>
            <div class="dataBlock-body">
                <table class="dataTable">
                    <tr>
                        <td class="number">
                            A
                        </td>
                        <td class="label">
                            User Name:
                        </td>
                        <td class="value">
                            <asp:Label ID="UserNameLabel" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            B
                        </td>
                        <td class="label">
                            User Role:
                        </td>
                        <td class="value">
                            <asp:Label runat="server" ID="RoleLabel" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            C
                        </td>
                        <td class="label">
                            Access Status:
                        </td>
                        <td class="value">
                            <asp:Label ID="StatusLabel" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            D
                        </td>
                        <td class="label">
                            Expiration Date:
                        </td>
                        <td class="value">
                            <asp:Label runat="server" ID="ExpirationDateLabel" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            E
                        </td>
                        <td class="label">
                            Current Role:
                        </td>
                        <td class="value">
                            <asp:Label runat="server" ID="CurrentRoleLabel"></asp:Label>
                            <asp:DropDownList runat="server" ID="CurrentRoleSelect" DataTextField="Description"
                                DataValueField="Id">
                            </asp:DropDownList>
                            <asp:Button runat="server" ID="ChangeRoleButton" Text="Change" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            F
                        </td>
                        <td class="label">
                            View Subordinate Units:
                        </td>
                        <td class="value">
                            <asp:CheckBox ID="chkSubUnitView" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            G
                        </td>
                        <td class="label">
                            Action:
                        </td>
                        <td class="value">
                            <asp:Button ID="btnAccountStatusUpdate" runat="server" Text="Update" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <br />
        <div class="dataBlock">
            <div class="dataBlock-header">
                2 - User Roles
            </div>
            <div class="dataBlock-body">
                <table style="height: 415px">
                    <tr>
                        <td class="number">
                            A
                        </td>
                        <td class="label">
                            Current Roles:
                        </td>
                        <td class="value" style="width: 675px;">
                            <asp:GridView runat="server" ID="RolesGrid" Width="100%" AutoGenerateColumns="false"
                                ShowHeader="false">
                                <Columns>
                                    <asp:TemplateField ItemStyle-Width="30px">
                                        <ItemTemplate>
                                            <asp:Image runat="server" ID="RoleActive" ImageUrl="~/images/check.gif" ImageAlign="AbsMiddle"
                                                Visible='<%# Iif(Eval("Active"),True,False) %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Role" ItemStyle-Width="585">
                                        <ItemTemplate>
                                            <%#Eval("Group.Description")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField ItemStyle-Width="60">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="RoleStatus" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            <br />
                            <br />
                        </td>
                    </tr>
   
                    <tr>
                        <td class="number">
                            B
                        </td>
                        <td class="label">
                            Completed Change Requests:
                        </td>
                        <td class="value" style="width: 675px;">
                            <asp:GridView runat="server" ID="CompletedGrid" Width="100%" AutoGenerateColumns="false">
                                <Columns>
                                    <asp:TemplateField HeaderText="Role">
                                        <ItemTemplate>
                                            <%#Eval("RequestedGroup.Description")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Type">
                                        <ItemTemplate>
                                            <%# IIf(Eval("IsNewRole").ToString = "True", "New Role", "Role Change") %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Status">
                                        <ItemTemplate>
                                            <%#IIf(Eval("Status").ToString = "Approved", "Approved", "Denied")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Completed">
                                        <ItemTemplate>
                                            <%#Eval("DateCompleted", "{0:MM/dd/yyyy}")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Completed By">
                                        <ItemTemplate>
                                            <a href='#' onclick='getWhois(<%# Eval("CompletedBy.Id") %>); return false;'>
                                                <%#Eval("CompletedBy.UserName")%></a>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="Comments" DataField="CompletedComments" HtmlEncode="false" />
                                </Columns>
                                <EmptyDataTemplate>
                                    No completed requests found
                                </EmptyDataTemplate>
                            </asp:GridView>
                            <br />
                            <br />
                        </td>
                    </tr>
 <%-- Hidden for task 214--%>
                    <tr style="visibility:hidden;">
                        <td class="number">
                            B
                        </td>
                        <td class="label">
                            Pending Change Requests:
                        </td>
                        <td class="value" style="width: 675px;">
                            <asp:GridView runat="server" ID="PendingGrid" Width="100%" AutoGenerateColumns="false">
                                <Columns>
                                    <asp:TemplateField HeaderText="Role">
                                        <ItemTemplate>
                                            <%#Eval("RequestedGroup.Description")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Type">
                                        <ItemTemplate>
                                            <%# IIf(Eval("IsNewRole").ToString = "True", "New Role", "Role Change") %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Date Requested">
                                        <ItemTemplate>
                                            <%# Eval("DateRequested", "{0:MM/dd/yyyy}") %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="Comments" DataField="RequestorComments" HtmlEncode="false" />
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:ImageButton runat="server" ID="DeleteRequestLink" SkinID="buttonDelete" AlternateText="Delete Request"
                                                OnClientClick="return confirm('Are you sure you want to delete this request?');"
                                                CommandName="DeleteRequest" CommandArgument='<%# Eval("Id") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    No pending requests found
                                </EmptyDataTemplate>
                            </asp:GridView>
                        </td>
                    </tr>
</table>
                    <table style="visibility:hidden; height: 52px; font-size: xx-small; overflow: hidden; empty-cells: hide;">
                    <tr>
                        <td class="number">
                            &nbsp;
                        </td>
                        <td class="label">
                            Request Role Change test
                        </td>
                        <td class="value">
                            <hr />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            D
                        </td>
                        <td class="label">
                            Role:
                        </td>
                        <td class="value">
                            <asp:DropDownList runat="server" ID="ChangeRoleSelect" DataTextField="Description"
                                DataValueField="Id">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            E
                        </td>
                        <td class="label">
                            Request Type:
                        </td>
                        <td class="value">
                            <asp:RadioButton runat="server" ID="ChangeRole" GroupName="changeType" Text="Change Role"
                                Checked="true" />
                            <asp:RadioButton runat="server" ID="NewRole" GroupName="changeType" Text="New Role" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            F
                        </td>
                        <td class="label">
                            Comments:
                        </td>
                        <td class="value">
                            <asp:TextBox runat="server" ID="RoleChangeComments" TextMode="MultiLine" Width="400px"
                                Rows="4" MaxLength="500"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            G
                        </td>
                        <td class="label">
                            Action:
                        </td>
                        <td class="value">
                            <asp:Button runat="server" ID="RequestRoleButton" Text="Request Change" />
                        </td>
                    </tr>
            </table>
            </div>

   

        </div>
        <asp:Panel ID="ControlsPanel" runat="server" Height="655px">
            <div class="dataBlock">
                <div class="dataBlock-header">
                    3 - Account Settings
                </div>
                <div class="dataBlock-body">
                    <table class="dataTable">
                        <tr>
                            <td class="number">
                                A
                            </td>
                            <td class="label">
                                Name:
                            </td>
                            <td class="value">
                                <asp:Label ID="NameLabel" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                B
                            </td>
                            <td class="label">
                                Last Four:
                            </td>
                            <td class="value">
                                <asp:Label ID="SsnLabel" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                C
                            </td>
                            <td class="label">
                                Rank:
                            </td>
                            <td class="value">
                                <asp:Label ID="RankLabel" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                D
                            </td>
                            <td class="label">
                                Unit:
                            </td>
                            <td class="value">
                                <asp:Label runat="server" ID="UnitLabel" />
                                <asp:TextBox Enabled="false" runat="server" Width="280px" ID="UnitTextBox" />
                                <asp:Button runat="server" ID="ChangeUnitButton" Text="Change" />
                                <input type="hidden" id="newUnitIDLabel" runat="Server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                E
                            </td>
                            <td class="label">
                                Receive Email:
                            </td>
                            <td class="value">
                                <asp:CheckBox ID="ReceiveEmailCheck" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                F
                            </td>
                            <td class="label">
                                Receive Reminder Email:
                            </td>
                            <td>
                                <asp:CheckBox ID="ReceiveReminderEmailCheck" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                G
                            </td>
                            <td class="label labelRequired">
                                * Phone:
                            </td>
                            <td class="value">
                                <asp:TextBox runat="server" ID="PhoneBox" MaxLength="20" />
                                <asp:RequiredFieldValidator ID="rqfd_Comm" runat="server" ControlToValidate="PhoneBox"
                                    ErrorMessage="Commercial Phone required" ValidationGroup="update" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="rgExpCom" runat="server" ControlToValidate="PhoneBox"
                                    ErrorMessage="Invalid Commercial Phone." ValidationExpression="\d{3}-\d{3}-\d{4}"
                                    ValidationGroup="update" SetFocusOnError="True">*</asp:RegularExpressionValidator>
                                (XXX-XX-XXXX)
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                H
                            </td>
                            <td class="label">
                                DSN:
                            </td>
                            <td class="value">
                                <asp:TextBox runat="server" ID="DsnBox" MaxLength="20" />
                                <asp:RegularExpressionValidator ID="rgExpDSN" runat="server" ControlToValidate="DsnBox"
                                    ErrorMessage="Invalid DSN Phone." ValidationExpression="^\d+$" ValidationGroup="update"
                                    SetFocusOnError="True">*</asp:RegularExpressionValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                I
                            </td>
                            <td class="label labelRequired">
                                * Work Email:
                            </td>
                            <td class="value">
                                <asp:TextBox runat="server" ID="EmailBox" MaxLength="100" />
                                <asp:RequiredFieldValidator ID="rqfd_Email" runat="server" ControlToValidate="EmailBox"
                                    ErrorMessage="Work Email Required." ValidationGroup="update" SetFocusOnError="True">*</asp:RequiredFieldValidator><asp:RegularExpressionValidator
                                        ID="rgExp_Email" runat="server" ControlToValidate="EmailBox" ErrorMessage="Invalid Work Email."
                                        ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="update"
                                        SetFocusOnError="True">*</asp:RegularExpressionValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                J
                            </td>
                            <td class="label">
                                Personal Email:
                            </td>
                            <td class="value">
                                <asp:TextBox runat="server" ID="Email2Box" MaxLength="100" />
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="Email2Box"
                                    ErrorMessage="Invalid Personal Email." ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                                    ValidationGroup="update" SetFocusOnError="True">*</asp:RegularExpressionValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                K
                            </td>
                            <td class="label">
                                Unit Email:
                            </td>
                            <td class="value">
                                <asp:TextBox runat="server" ID="Email3Box" MaxLength="100" />
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="Email3Box"
                                    ErrorMessage="Invalid Unit Email." ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                                    ValidationGroup="update" SetFocusOnError="True">*</asp:RegularExpressionValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                L
                            </td>
                            <td class="label">
                                Address:
                            </td>
                            <td class="value">
                                <asp:TextBox runat="server" ID="StreetBox" MaxLength="100" />
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                M
                            </td>
                            <td class="label">
                                City
                            </td>
                            <td class="value">
                                <asp:TextBox runat="server" ID="CityBox" MaxLength="50" />
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                N
                            </td>
                            <td class="label">
                                State:
                            </td>
                            <td class="value">
                                <asp:DropDownList runat="server" ID="StateSelect" DataTextField="state_name" DataValueField="state" />
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                O
                            </td>
                            <td class="label">
                                Zip:
                            </td>
                            <td class="value">
                                <asp:TextBox runat="server" ID="ZipBox" MaxLength="50" />
                            </td>
                        </tr>
                        <tr id="pAction">
                            <td class="number">
                                P
                            </td>
                            <td class="label">
                                Action:
                            </td>
                            <td class="value">
                                <asp:Button ID="updateButton" ValidationGroup="update" runat="server" Text="Update" />&nbsp;
                                <asp:ValidationSummary ID="vld_Summary" runat="server" ValidationGroup="update" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
        </asp:Panel>
    </div>
    <div id="hdn">
        <input id="hdnInitialHash" runat="Server" type="hidden" />
        <input id="hdnOldControlList" runat="Server" type="hidden" />
    </div>
</asp:Content>
