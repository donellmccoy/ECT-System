<%@ Page Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Admin.Secure_Shared_Admin_EditUser" Title="Untitled Page"
    MaintainScrollPositionOnPostback="true" Codebehind="EditUser.aspx.vb" %>

<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="head">
</asp:Content>
<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="ContentFooter">
    <asp:ScriptManagerProxy runat="server" ID="smp">
        <Scripts>
            <asp:ScriptReference Path="~/Script/ChangeLog.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
    <script type="text/javascript">

        $(function () {
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

            //approve role change dialog
            $('#RoleChangeDialog').dialog({
                autoOpen: false,
                modal: true,
                resizable: false,
                width: 540,
                height: 320,
                buttons: {
                    'Cancel': function () {
                        $(this).dialog('close');
                    },
                    'Update': function () {
                        serializeRoleDialog();
                        $(this).dialog('close');
                        element('<%= RoleDialogButton.ClientId %>').click();
                    }
                }
            });

            //edit role dialog
            $('#RoleEditDialog').dialog({
                autoOpen: false,
                modal: true,
                resizable: false,
                width: 340,
                height: 200,
                buttons: {
                    'Cancel': function () {
                        $(this).dialog('close');
                    },
                    'Update': function () {
                        serializeRoleEditDialog();
                        $(this).dialog('close');
                        element('<%= RoleUpdateButton.ClientId %>').click();
                    }
                }
            });

            $('.open-dialog').click(function () {
                $('#srchBox').dialog('open');
            });

            $('.datePicker').datepicker(calendarPick("TomorrowFuture", "<%=CalendarImage %>"));

        });


        function printHistory(userId) {
            var url = "PrintTracking.aspx?id=" + String(userId);

            showPopup({
                Url: url,
                Width: 600,
                Height: 480,
                Center: true,
                StatusBar: true,
                Resizable: true
            });
        }

        function ClearADUnit(box) {

            element('<%= adUnitTextBox.ClientId %>').value = '';
            element('<%= adUnitIDLabel.ClientId %>').value = '';


        }

        var roleId = 0;
        var groupId = 0;

        function showRoleDialog(id, role, change) {
            roleId = id;

            //clear any old values
            $('#dlComments').val("");
            element('reqApproved').checked = true;

            $('#dlRequestedRole').text(role);
            $('#dlRequestedChange').text(change);
            $('#RoleChangeDialog').dialog('open');
        }

        function showEditRoleDialog(id, group, groupName, active) {

            roleId = id;
            groupId = group;
            
            //populate the roles with those available
            
            //start with the current role
            $('#EditRoleSelect').children().remove();
            $('#EditRoleSelect').append($("<option></option>").attr("value", groupId).text(groupName));

            //then add the other available roles
            $('#<%= ChangeRoleSelect.ClientId %>').children().each(function () {
                $('#EditRoleSelect').append($("<option></option>").attr("value", $(this).attr("value")).text($(this).text()));
            });

            //disable the active box if this role is active
            //active can only be changed by activating a different role, the current active role can not be 'de-activated'            
            element('dlMakeActive').disabled = (active == 1);
            element('dlMakeActive').checked = (active == 1);

            $('#RoleEditDialog').dialog('open');
        }

        function serializeRoleEditDialog() {

            var vals = String(roleId) + "|" + String(groupId) + "|";
            vals += String($('#EditRoleSelect option:selected').val()) + "|";
            vals += (element('dlMakeActive').checked ? "True" : "False");
            element('<%= RoleDialogValues.ClientId %>').value = vals;

        }

        function serializeRoleDialog() {
            var vals = String(roleId) + "|";

            if (element('reqApproved').checked) {
                vals += "True|";
            } else {
                vals += "False|";
            }

            vals += $('#dlComments').val();
            element('<%= RoleDialogValues.ClientId %>').value = vals;
        }

        //Show Searcher
        function showSearcher(title, targetId, targetlbl) {
            initializeUnitSearcher();
            //Set Client controls where unit Id and unit names will be transferred
            element('<%=hdnIdClient.ClientId %>').value = targetId;
            element('<%=hdnNameClient.ClientId %>').value = targetlbl;
            $('#srchBox').dialog('open');
        }
        //Client accepted so Set Corresponding reporting units 
        function SetReportingUnit() {
            var srcherId = '<%=unitSearcher.ClientId %>'
            //Id is stroed in a label control.Transfer the value from the control
            element(element('<%=hdnIdClient.ClientId %>').value).value = element(srcherId + '_hdnSelectedUnit').value;
            //Name is stored in a label.Transfer the value from the control
            element(element('<%=hdnNameClient.ClientId %>').value).value = element(srcherId + '_hdnSelectedUnitName').value;
            return false;

            //Name is stored in a label.Transfer the value from the control
            return false;
        }
        //Client cancelled so ignore the dialog values
        function CancelSelection() {
            var srcherId = '<%=unitSearcher.ClientId %>'
            element(srcherId + '_hdnSelectedUnit').value = "";
            element(srcherId + '_hdnSelectedUnitName').value = "";
            return false;
        }

    </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <!-- Approve role request dialog -->
    <asp:TextBox runat="server" ID="RoleDialogValues" CssClass="hidden" />
    <asp:Button runat="server" ID="RoleDialogButton" CssClass="hidden" />
    <asp:Button runat="server" ID="RoleUpdateButton" CssClass="hidden" />
    <div id="RoleEditDialog" title="Edit Role" style="display: none; margin-bottom: 0px;
        background-color: #EEE;">
        <table>
            <tr>
                <td class="number">
                    A
                </td>
                <td class="label-small">
                    Role:
                </td>
                <td class="value">
                    <select id="EditRoleSelect">
                    </select>
                </td>
            </tr>
            <tr>
                <td class="number">
                    B
                </td>
                <td class="label-small">
                    Active:
                </td>
                <td class="value">
                    <input type="checkbox" id="dlMakeActive" />
                </td>
            </tr>
        </table>
    </div>
    <div id="RoleChangeDialog" title="Complete Role Change Request" style="display: none;
        margin-bottom: 0px; background-color: #EEE;">
        <table>
            <tr>
                <td class="number">
                    A
                </td>
                <td class="label-small">
                    Requested Role:
                </td>
                <td class="value">
                    <span id="dlRequestedRole"></span>
                </td>
            </tr>
            <tr>
                <td class="number">
                    B
                </td>
                <td class="label-small">
                    Requested Change:
                </td>
                <td class="value">
                    <span id="dlRequestedChange"></span>
                </td>
            </tr>
            <tr>
                <td class="number">
                    C
                </td>
                <td class="label-small">
                    Action:
                </td>
                <td class="value">
                    <label for="reqApproved">Approved</label>
                    <input id="reqApproved" type="radio" name="reqAction" value="reqApproved" checked />
                    <label for="reqDenied">Denied</label>
                    <input id="reqDenied" type="radio" name="reqAction" value="reqDenied" />
                </td>
            </tr>
            <tr>
                <td class="number">
                    D
                </td>
                <td class="label-small">
                    Comments:
                </td>
                <td class="value">
                    <textarea id="dlComments" rows="4" cols="50"></textarea>
                </td>
            </tr>
        </table>
    </div>
    <!-- Unit search control -->
    <div id="srchBox" class="hidden" title="Find Unit">
        <lod:unitSearcher ID="unitSearcher" ActiveOnly="true" runat="server" />
    </div>
    <!-- end search control -->
    <input type="hidden" id="hdnIdClient" runat="Server" />
    <input type="hidden" id="hdnNameClient" runat="Server" />
    <div class="indent-small">
        <div style="text-align: right;">
            <asp:LinkButton ID="lnkManageUsers" runat="server" Visible="false">
                <asp:Image runat="server" ID="ReturnImage1" AlternateText="Return to manage users"
                    ImageAlign="AbsMiddle" ImageUrl="~/images/arrow_left.gif" />
                Return to Manage Users
            </asp:LinkButton>
            <asp:LinkButton ID="lnkManageMembers" runat="server" Visible="false">
                <asp:Image runat="server" ID="ReturnImage2" AlternateText="Return to member data"
                    ImageAlign="AbsMiddle" ImageUrl="~/images/arrow_left.gif" />
                Return to Member Data
            </asp:LinkButton>
            <asp:LinkButton ID="lnkManageRoles" runat="server" Visible="false">
                <asp:Image runat="server" ID="ReturnImage3" AlternateText="Return to member data"
                    ImageAlign="AbsMiddle" ImageUrl="~/images/arrow_left.gif" />
                Return to Pending Role Requests
            </asp:LinkButton>
            <asp:LinkButton ID="lnkPermissionReport" runat="server" Visible="false">
                <asp:Image runat="server" ID="ReturnImage4" AlternateText="Return to permissions report"
                    ImageAlign="AbsMiddle" ImageUrl="~/images/arrow_left.gif" />
                Return to Permission Report
            </asp:LinkButton>
        </div>
        <br />
        <asp:Panel ID="PendingPanel" runat="server" CssClass="ui-state-highlight info-block">
            This account is pending approval
        </asp:Panel>
        <asp:Panel ID="DisabledPanel" runat="server" CssClass="ui-state-error info-block">
            This account is currently disabled
        </asp:Panel>
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
                            Username:
                        </td>
                        <td class="value">
                            <asp:Label ID="UsernameLabel" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            B
                        </td>
                        <td class="label">
                            Compo:
                        </td>
                        <td class="value">
                            <asp:DropDownList runat="server" ID="CompoSelect"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            C
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
                            D
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
                            E
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
                            F
                        </td>
                        <td class="label">
                            Access Status:
                        </td>
                        <td style="width: 400px">
                            <asp:Label ID="StatusLabel" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <asp:Label ID="ActionLabel" runat="server">
                    <tr  >
                        <td class="number">
                            G
                        </td>
                        <td class="label" >
                            Action:
                        </td>
                        
                    </asp:Label>
                        <td class="value" >
                            <asp:Button ID="EnableButton" runat="server" Text="Enable" Enabled="False" />&nbsp;
                            <asp:Button ID="DisableButton" runat="server" Text="Disable" Enabled="False" />
                            <asp:Label ID="AdminDisabledLabel" runat="server" Text="Cannot Override System Admins Decision"
                                Visible="false" Style="color: Red;" />
                            <ajax:ConfirmButtonExtender ID="ConfirmButtonExtender1" runat="server" ConfirmText="Are you sure you want to this account?"
                                TargetControlID="EnableButton">
                            </ajax:ConfirmButtonExtender>
                            <ajax:ConfirmButtonExtender ID="ConfirmButtonExtender2" runat="server" ConfirmText="Are you sure you want to disable this account?"
                                TargetControlID="DisableButton">
                            </ajax:ConfirmButtonExtender>
                        </td>
                    </tr>

                </table><asp:Panel runat="server" ID="MultipleAccountsPanel">
                    <table class="dataTable">
                        <tr>
                            <td class="number">
                                H </td><td class="label">
                                Additional Accounts: </td><td style="width: 675px;">
                                <asp:GridView runat="server" ID="AccountsGrid" Width="100%" AutoGenerateColumns="false">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Status" SortExpression="AccessStatusText">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="StatusLabel"><%#Eval("StatusDescription")%></asp:Label></ItemTemplate></asp:TemplateField><asp:TemplateField HeaderText="User Name">
                                            <ItemTemplate>
                                                <asp:LinkButton runat="server" ID="AccountLink" ToolTip="View Account" CommandArgument='<%# Eval("Id") %>'
                                                    Text='<%# Eval("UserName") %>' CommandName="<%# COMMAND_VIEW_ACCOUNT %>" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="CurrentRoleName" HeaderText="Role" />
                                        <asp:BoundField DataField="CurrentUnitName" HeaderText="Unit" />
                                        <asp:BoundField DataField="Component" HeaderText="Compo" />
                                        <asp:BoundField DataField="AccountExpiration" HeaderText="Expires" DataFormatString="{0:MM/dd/yyyy}"
                                            HtmlEncode="false" />
                                        <asp:TemplateField>
                                            <ItemTemplate>
                                                <asp:LinkButton runat="server" ID="DisableLink" Text="Disable" CommandArgument='<%# Eval("Id") %>'
                                                    CommandName="<%# COMMAND_DISABLE_ACCOUNT %>" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataRowStyle CssClass="emptyItem" />
                                    <EmptyDataTemplate>
                                        None Found</EmptyDataTemplate></asp:GridView></td></tr><tr>
                            <td class="number">
                                I </td><td class="label">
                                New Account: </td><td style="width: 400px">
                                <asp:Button runat="server" ID="CreateAccountButton" Text="Create" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </div>
        </div>
        <br />
        <div class="dataBlock">
            <div class="dataBlock-header" id="UserRoles">
                2 - User Roles </div><div class="dataBlock-body">
                <table>
                    <tr>
                        <td class="number">
                            A </td><td class="label">
                            Current Roles: </td><td class="value" style="width: 675px;">
                            
                            <asp:GridView runat="server" ID="RolesGrid" Width="100%" AutoGenerateColumns="False"
                                DataKeyNames="Id" ShowHeader="false">
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:Image runat="server" ID="RoleActive" ImageUrl="~/images/check.gif" ImageAlign="AbsMiddle" />
                                        </ItemTemplate>
                                        <ItemStyle Width="30px"></ItemStyle>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Role">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="RoleTitle" />
                                        </ItemTemplate>
                                        <ItemStyle Width="530px"></ItemStyle>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:LinkButton Visible='<%# DisableLODPm(Eval("Group.Id"))%>' ID="EnableInvestOfficer" OnClick="EnableIOButton_Click" runat="server"  Text="Enable" Enabled="True" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField  ShowHeader="False">
                                        <ItemTemplate>
                                            <a href="#" title="Edit" style="visibility:<%# DisableLODPmIO(Eval("Group.Id"))%>"   onclick='showEditRoleDialog(<%# Eval("Id") %>,<%# Eval("Group.Id") %>, <%#Eval("Group.Description","""{0}""") %>, <%# Iif(Eval("Active") = "True", 1, 0) %>); return false;'>
                                                Edit</a></ItemTemplate>
                                            </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:LinkButton runat="server" ID="DeleteRoleLink" CommandArgument='<%#Eval("Id") %>'
                                                CommandName="DeleteRole" Text="Delete" OnClientClick="return confirm('Are you sure you want to delete this role?');"
                                                Visible='<%# Iif(Eval("Active"),False,True) %>'></asp:LinkButton></ItemTemplate><ItemStyle Width="60px" HorizontalAlign="Center" />
                                    </asp:TemplateField>
                                </Columns>
                                
                            </asp:GridView>
                             <br />
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            B </td><td class="label">
                            Pending Change Requests: </td><td class="value" style="width: 675px;">
                            <asp:GridView runat="server" ID="PendingGrid" Width="100%" AutoGenerateColumns="false">
                                <Columns>
                                    <asp:TemplateField HeaderText="Role">
                                        <ItemTemplate>
                                            <%#Eval("RequestedGroup.Description")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Type">
                                        <ItemTemplate>
                                            <%# Iif(Eval("IsNewRole").ToString = "True", "New Role", "Role Change") %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Date Requested">
                                        <ItemTemplate>
                                            <%# Eval("DateRequested","{0:MM/dd/yyyy}") %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:BoundField HeaderText="Comments" DataField="RequestorComments" HtmlEncode="false" />
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <input type="button" value="Process" onclick='showRoleDialog(<%#Eval("Id")%>,<%#Eval("RequestedGroup.Description","""{0}""") %>,<%# Iif(Eval("IsNewRole").ToString = "True", """New Role""", """Role Change""") %>);' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataTemplate>
                                    No pending requests found</EmptyDataTemplate></asp:GridView><br />
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            C </td><td class="label">
                            Completed Change Requests: </td><td class="value" style="width: 675px;">
                            <asp:GridView runat="server" ID="CompletedGrid" Width="100%" AutoGenerateColumns="false">
                                <Columns>
                                    <asp:TemplateField HeaderText="Role">
                                        <ItemTemplate>
                                            <%#Eval("RequestedGroup.Description")%>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Type">
                                        <ItemTemplate>
                                            <%# Iif(Eval("IsNewRole").ToString = "True", "New Role", "Role Change") %>
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
                                    <asp:BoundField HeaderText="Comments" ItemStyle-Wrap="true" DataField="CompletedComments"
                                        HtmlEncode="false" />
                                </Columns>
                                <EmptyDataTemplate>
                                    No completed requests found</EmptyDataTemplate></asp:GridView><br />
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            &nbsp; </td><td class="label">
                            Add Additional Role </td><td class="value">
                            <hr />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            D </td><td class="label">
                            Role: </td><td class="value">
                            <asp:DropDownList runat="server" ID="ChangeRoleSelect" DataTextField="Description"
                                DataValueField="Id">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                            <td class="number">
                            E </td><td class="label">
                            Action: </td><td class="value">
                            <asp:Button runat="server" ID="RequestRoleButton" Text="Finish" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <asp:Panel ID="ControlsPanel" runat="server">
            <div class="dataBlock">
                <div class="dataBlock-header">
                    3 - Account Settings </div><div style="text-align: left;">
                </div>
                <div class="dataBlock-body">
                    <table class="dataTable">
                        <tr>
                            <td colspan="2" style="width: 100px;">
                            </td>
                            <td>
                                <asp:ValidationSummary ID="vld_Summary" runat="server" ValidationGroup="update" />
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                A </td><td class="label labelRequired">
                                Rank: </td><td class="value">
                                <asp:DropDownList runat="server" ID="RankSelect">
                            </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                B </td><td class="label">
                                Report View: </td>
                            <td class="value" >
                                <%--style="visibility:<%#DisableLODPmReportView(Eval("Group.Id"))%>"--%>
                                <asp:DropDownList ID="ViewSelect" runat="server" >
                                    <asp:ListItem Value="0">Default View</asp:ListItem>
                                    <asp:ListItem Value="1">Total View</asp:ListItem>
                                    <asp:ListItem Value="2">Non Medical Reporting View</asp:ListItem>
                                    <asp:ListItem Value="3">Medical Reporting View</asp:ListItem>
                                    <asp:ListItem Value="4">RMU View (Physical Responsibility)</asp:ListItem>
                                    <asp:ListItem Value="5">JA View</asp:ListItem>
                                    <asp:ListItem Value="6">MPF View</asp:ListItem>
                                    <asp:ListItem Value="7">System Administration View</asp:ListItem>
                                </asp:DropDownList>
                                <asp:DropDownList ID="ViewSelectLODPm" runat="server" >
                                    <asp:ListItem Value="0">Default View</asp:ListItem>
                                </asp:DropDownList>
                               </td>
                            
                        </tr>
                        <tr>
                            <td class="number">
                                C </td><td class="label labelRequired">
                                * EDIPIN: </td><td class="value">
                                <asp:TextBox Enabled="true" runat="server" Width="280px" ID="EDIPINtxt" MaxLength="50" />
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="EDIPINtxt"
                                    ErrorMessage="Invalid EDIPIN." ValidationExpression="^[0-9a-zA-Z]+$" ValidationGroup="update"
                                    SetFocusOnError="True">*</asp:RegularExpressionValidator></td></tr><tr>
                            <td class="number">
                                D </td><td class="label labelRequired">
                                *Current Unit: </td><td class="value">
                                <asp:TextBox Enabled="false" runat="server" Width="280px" ID="UnitTextBox" />
                                <asp:Button runat="server" ID="ChangeUnitButton" Text="Change" />
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="Unit is Required."
                                    ControlToValidate="UnitTextBox" ValidationGroup="update">*</asp:RequiredFieldValidator><input type="hidden" id="newUnitIDLabel" runat="Server" /></td></tr><tr>
                            <td class="number">
                                E </td><td class="label labelRequired">
                                *Attachment Unit: </td><td class="value">
                                <asp:TextBox runat="server" Width="280px" Enabled="false" ID="adUnitTextBoX" />
                                <asp:Button runat="server" ID="ChangeAdUnitBtn" Text="Change" />
                                <input type="button" value="Clear" onclick='ClearADUnit();' />
                                <input type="hidden" id="adUnitIDLabel" runat="Server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                F </td><td class="label labelRequired">
                                * Expiration Date: </td><td class="value">
                                <asp:TextBox ID="ExpirationBox" onchange="DateCheck(this);" CssClass="datePicker"
                                    MaxLength="10" Width="80" runat="server"></asp:TextBox></td></tr><tr>
                            <td class="number">
                                G </td><td class="label">
                                Receive Email: </td><td class="value">
                                <asp:CheckBox ID="ReceiveEmailCheck" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                H </td><td class="label">
                                Receive Reminder Email: </td><td class="value">
                                <asp:CheckBox ID="ReceiveReminderEmailCheck" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                I </td><td class="label">
                                Comment: </td><td class="value">
                                <asp:TextBox ID="CommentsBox" runat="server" Height="60px" MaxLength="200" 
                                    TextMode="MultiLine" Width="360px"></asp:TextBox></td></tr><tr>
                            <td class="number">
                                J </td><td class="label labelRequired">
                                * Phone: </td><td class="value">
                                <asp:TextBox runat="server" ID="PhoneBox" MaxLength="20" />
                                <asp:RequiredFieldValidator ID="rqfd_Comm" runat="server" ControlToValidate="PhoneBox"
                                    ErrorMessage="Commercial Phone is required" ValidationGroup="update" SetFocusOnError="True">*</asp:RequiredFieldValidator><asp:RegularExpressionValidator ID="rgExpCom" runat="server" ControlToValidate="PhoneBox"
                                    ErrorMessage="Invalid Phone." ValidationExpression="\d{3}-\d{3}-\d{4}" ValidationGroup="update"
                                    SetFocusOnError="True">*</asp:RegularExpressionValidator>(XXX-XXX-XXXX) </td></tr><tr>
                            <td class="number">
                                K </td><td class="label">
                                DSN: </td><td class="value">
                                <asp:TextBox runat="server" ID="DsnBox" MaxLength="20" />
                                <asp:RegularExpressionValidator ID="rgExpDSN" runat="server" ControlToValidate="DsnBox"
                                    ErrorMessage="Invalid DSN Phone." ValidationExpression="^\d+$" ValidationGroup="update"
                                    SetFocusOnError="True">*</asp:RegularExpressionValidator><ajax:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" TargetControlID="DsnBox"
                                    runat="server" FilterType="Numbers" />
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                L </td><td class="label labelRequired">
                                *Work Email: </td><td class="value">
                                <asp:TextBox runat="server" ID="EmailBox" MaxLength="100" />
                                <asp:RequiredFieldValidator ID="rqfd_Email" runat="server" ControlToValidate="EmailBox"
                                    ErrorMessage="Work Email is required" ValidationGroup="update" SetFocusOnError="True">*</asp:RequiredFieldValidator><asp:RegularExpressionValidator
                                        ID="rgExp_Email" runat="server" ControlToValidate="EmailBox" ErrorMessage="Invalid Work Email."
                                        ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="update"
                                        SetFocusOnError="True">*</asp:RegularExpressionValidator></td></tr><tr>
                            <td class="number">
                                M </td><td class="label">
                                Personal Email: </td><td class="value">
                                <asp:TextBox runat="server" ID="Email2Box" MaxLength="100" />
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="Email2Box"
                                    ErrorMessage="Invalid Personal Email." ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                                    ValidationGroup="update" SetFocusOnError="True">*</asp:RegularExpressionValidator></td></tr><tr>
                            <td class="number">
                                N </td><td class="label">
                                Unit Email: </td><td class="value">
                                <asp:TextBox runat="server" ID="Email3Box" MaxLength="100" />
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="Email3Box"
                                    ErrorMessage="Invalid Unit Email." ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                                    ValidationGroup="update" SetFocusOnError="True">*</asp:RegularExpressionValidator></td></tr><tr>
                            <td class="number">
                                O </td><td class="label labelRequired">
                                *Address: </td><td class="value">
                                <asp:TextBox runat="server" ID="StreetBox" MaxLength="100" />
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="StreetBox"
                                    ErrorMessage="Address street is required" ValidationGroup="update" SetFocusOnError="True">*</asp:RequiredFieldValidator></td></tr><tr>
                            <td class="number">
                                P </td><td class="label labelRequired">
                                *City: </td><td class="value">
                                <asp:TextBox runat="server" ID="CityBox" MaxLength="50" />
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="CityBox"
                                    ErrorMessage="Address city is required" ValidationGroup="update" SetFocusOnError="True">*</asp:RequiredFieldValidator></td></tr><tr>
                            <td class="number">
                                Q </td><td class="label labelRequired">
                                *State: </td><td class="value">
                                <asp:DropDownList runat="server" ID="StateSelect" DataTextField="state_name" DataValueField="state" />
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="StateSelect"
                                    ErrorMessage="Address state is required" ValidationGroup="update" SetFocusOnError="True">*</asp:RequiredFieldValidator></td></tr><tr>
                            <td class="number">
                                R </td><td class="label labelRequired">
                                *Zip: </td><td class="value">
                                <asp:TextBox runat="server" ID="ZipBox" MaxLength="50" />
                                <asp:RegularExpressionValidator ID="regExp_Zip" runat="server" ControlToValidate="ZipBox"
                                    ErrorMessage="Invalid Zip Code" ValidationExpression="\d{5}(-\d{4})?" ValidationGroup="update"
                                    SetFocusOnError="True">*</asp:RegularExpressionValidator><asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="ZipBox"
                                    ErrorMessage="Address zip code is required" ValidationGroup="update" SetFocusOnError="True">*</asp:RequiredFieldValidator></td></tr><tr>
                            <td class="number">
                                S </td><td class="label">
                                Action: </td><td class="value">
                                <asp:Button ID="updateButton" ValidationGroup="update" runat="server" Text="Update" />&nbsp; </td></tr></table></div></div><input id="hdButtonClicked" runat="Server" type="hidden" /></asp:Panel><div class="dataBlock">
            <div class="dataBlock-header">
                <div style="width: 80%; float: left;">
                    4- Account History </div><div style="width: 19%; float: right; text-align: right;">
                    <asp:ImageButton runat="server" ID="PrintHistory" ImageAlign="AbsMiddle" ImageUrl="~/images/print.gif"
                        AlternateText="Print History" />
                </div>
            </div>
            <div class="dataBlock-body">
                <asp:GridView runat="server" ID="HistoryGrid" AutoGenerateColumns="false" Width="100%">
                    <Columns>
                        <asp:TemplateField HeaderText="Date">
                            <ItemTemplate>
                                <%#Eval("ActionDate", "{0:MM/dd/yyyy hhmm}")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="Action" DataField="ActionName" />
                        <asp:BoundField HeaderText="Setting" DataField="Field" />
                        <asp:BoundField HeaderText="Old Value" DataField="OldVal" HtmlEncode="false" />
                        <asp:BoundField HeaderText="New Value" DataField="NewVal" HtmlEncode="false" />
                        <asp:TemplateField>
                            <ItemTemplate>
                                <a href='#' onclick='getWhois(<%# Eval("UserId") %>); return false;'>
                                    <%#Eval("UserName")%></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
    <div id="hdn">
        <input id="hdnInitialHash" runat="Server" type="hidden" />
        <input id="hdnOldControlList" runat="Server" type="hidden" />
    </div>
</asp:Content>
