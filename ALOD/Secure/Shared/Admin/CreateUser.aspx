<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Admin.Secure_Shared_Admin_CreateUser" Codebehind="CreateUser.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">

    <script type="text/javascript">

        function initializeDialog() {
            if (!$('#srchBox').data('ui-dialog') && !$('#srchBox').data('dialog')) {
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
            }
        }

        $(function () {
            initializeDialog();

            $('.open-dialog')
                .on('click', function () {
                    initializeDialog();
                    $('#srchBox').dialog('open');
                })
                .on('mouseenter', function () { // Replaces the first function of .hover()
                    $(this).addClass("ui-state-hover");
                })
                .on('mouseleave', function () { // Replaces the second function of .hover()
                    $(this).removeClass("ui-state-hover");
                })
                .on('mousedown', function () {
                    $(this).addClass("ui-state-active");
                })
                .on('mouseup', function () {
                    $(this).removeClass("ui-state-active");
                });
        });

        //Show Searcher
        function showSearcher(title, targetId, targetlbl) {
            initializeUnitSearcher();
            //Set Client controls where unit Id and unit names will be transferred
            element('<%=hdnIdClient.ClientId %>').value = targetId;
            element('<%=hdnNameClient.ClientId %>').value = targetlbl;
            
            // Ensure dialog is initialized before opening
            initializeDialog();
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

        }
        //Client cancelled so ignore the dialog values
        function CancelSelection() {
            var srcherId = '<%=unitSearcher.ClientId %>'
            element(srcherId + '_hdnSelectedUnit').value = "";
            element(srcherId + '_hdnSelectedUnitName').value = "";
            return false;
        }
    
    </script>

    <div id="unitseracer">
        <!-- Copy this whole div to copy the seacrher-->
        <div id="srchBox" class="hidden" title="Find Unit">
            <lod:unitsearcher id="unitSearcher"  ActiveOnly ="true"  runat="server" />
        </div>
        <!-- end search control -->
        <input type="hidden" id="hdnIdClient" runat="Server" />
        <input type="hidden" id="hdnNameClient" runat="Server" />
        <br />
    </div>
    <div>
    </div>
    <div id="content" class="indent-small">
        <div style="text-align: right;">
            <asp:LinkButton ID="lnkManageMembers" runat="server">
                <asp:Image runat="server" ID="ReturnImage2" AlternateText="Return to member data"
                    ImageAlign="AbsMiddle" ImageUrl="~/images/arrow_left.gif" />
                Return to Member Data
            </asp:LinkButton>
        </div>
        <asp:Panel runat="server" ID="srchSSN" CssClass="dataBlock">
            <div class="dataBlock-header">
                1 - Member Search
            </div>
            <div class="dataBlock-body">
                <table>
                    <tr>
                        <td class="number">
                            A
                        </td>
                        <td class="label">
                            Member SSN:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="SSNTextBox" Width="80px" runat="server" MaxLength="9"></asp:TextBox>
                            &nbsp;
                            <asp:Label ID="InvalidSSNLabel" runat="server" CssClass="labelRequired" Text="Invalid SSN"
                                Visible="False"></asp:Label>
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
                            B
                        </td>
                        <td class="label">
                            Action:
                        </td>
                        <td class="value">
                            <asp:Button ID="LookupButton" runat="server" ValidationGroup="lookUp" Text="Lookup" />
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <div style="text-align:center;">
            <asp:Label ID="NotFoundLabel" runat="server" CssClass="labelRequired" Text="Member not found"
                Visible="False"></asp:Label>
            &nbsp;
            <asp:Label ID="userFoundLabel" runat="server" CssClass="labelRequired" Text="A User with this SSN exists"
                Visible="False"></asp:Label>
        </div>
        <br>
        <asp:Panel ID="MemberDataPanel" runat="server" Visible="False" CssClass="dataBlock">
            <div class="dataBlock-header">
                1 - User Information
            </div>
            <div class="dataBlock-body">
                <div style="text-align: left;">
                    <asp:ValidationSummary ID="vld_Summary" runat="server" ValidationGroup="register" />
                </div>
                <!-- Name display -->
                <table style="text-align: left;">
                    <tr>
                        <td class="number">
                            <asp:Label ID="SSNNum" runat="server" Text="A" />
                        </td>
                        <td class="label">
                            SSN:
                        </td>
                        <td class="value">
                            <asp:Label ID="SSNLabel" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>
                <asp:Panel runat="server" Visible="false" ID="NameDisplayPanel">
                    <table style="text-align: left;">
                        <tr>
                            <td class="number">
                                <asp:Label ID="NameNum" runat="server" Text="B" />
                            </td>
                            <td class="label">
                                Name:
                            </td>
                            <td class="value">
                                <asp:Label runat="server" ID="FullName" />
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                <asp:Label ID="RankNumDisplay" runat="server" Text="C" />
                            </td>
                            <td class="label">
                                Rank:
                            </td>
                            <td class="value">
                                <asp:Label runat="server" ID="RankDisplay" />
                            </td>
                            <td>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <!-- Name entry -->
                <asp:Panel runat="server" Visible="false" ID="NameEntryPanel">
                    <table style="text-align: left;">
                        <tr>
                            <td class="number">
                                <asp:Label ID="TitleNum" Text="B" runat="server" />
                            </td>
                            <td class="label">
                                <asp:Label ID="TitleLabel" runat="server" Text="Title:"></asp:Label>
                                &nbsp;
                            </td>
                            <td class="value">
                                <asp:DropDownList ID="TitleSelect" runat="server" Width="196px">
                                    <asp:ListItem Value="">---Select---</asp:ListItem>
                                    <asp:ListItem Value="Mr.">Mr.</asp:ListItem>
                                    <asp:ListItem Value="Ms.">Ms.</asp:ListItem>
                                    <asp:ListItem Value="Mrs.">Mrs.</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                <asp:Label ID="FirstNameNum" Text="C" runat="server" />
                            </td>
                            <td class="label labelRequired">
                                *First Name:
                            </td>
                            <td class="value">
                                <asp:Label ID="FirstNameLabel" runat="server"></asp:Label>
                                <asp:TextBox ID="FirstNameBox" runat="server" Width="190px" MaxLength="50"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rqfd_FName" SetFocusOnError="True" runat="server"
                                    ControlToValidate="FirstNameBox" ErrorMessage="First Name required" ValidationGroup="register">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                <asp:Label ID="MiddleNameNum" Text="D" runat="server" />
                            </td>
                            <td class="label">
                                Middle Name:
                            </td>
                            <td class="value">
                                <asp:Label ID="MiddleNameLabel" runat="server"></asp:Label>
                                <asp:TextBox ID="MIddleNameBox" runat="server" Width="190px" MaxLength="50"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                <asp:Label ID="LastNameNum" Text="E" runat="server" />
                            </td>
                            <td class="labelRequired label">
                                *Last Name:
                            </td>
                            <td class="value">
                                <asp:Label ID="LastNameLabel" runat="server"></asp:Label>
                                <asp:TextBox ID="LastNameBox" runat="server" Width="190px" MaxLength="50"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rqfd_LName" SetFocusOnError="True" runat="server"
                                    ControlToValidate="LastNameBox" ErrorMessage="Last Name required" ValidationGroup="register">*</asp:RequiredFieldValidator>
                        </tr>
                        <tr>
                            <td class="number">
                                <asp:Label ID="RankNumEntry" Text="F" runat="server" />
                            </td>
                            <td class="labelRequired label">
                                *Rank:
                            </td>
                            <td class="value">
                                <asp:Label ID="RankLabel" runat="server"></asp:Label>
                                <asp:DropDownList ID="RankSelect" runat="server" DataTextField="text" DataValueField="value"
                                    Width="196px">
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="rqfld_rank" SetFocusOnError="True" runat="server"
                                    InitialValue="-1" ErrorMessage="Rank rquired." ControlToValidate="RankSelect"
                                    ValidationGroup="register">*</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <table style="text-align: left;">
                <tr>
                    <td class="number">
                            *
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
                </table>
                <!-- end Name entry -->
                <table style="text-align: left;">
                    
                     <tr>
                            <td class="number">
                                <asp:Label runat="server" ID="Lbl1" Text="D" />
                            </td>
                            <td class="label labelRequired">
                                <asp:Label runat="server" ID="EDIPINLbl" Text="EDIPIN" />
                            </td>
                            <td class="value">
                                <asp:TextBox Enabled="true" runat="server" Width="280px" ID="EDIPINtxt" />
                                <asp:RegularExpressionValidator ID="RegularExpressionValidator3" runat="server" ControlToValidate="EDIPINtxt"
                                    ErrorMessage="Invalid EDIPIN." ValidationExpression="^[0-9a-zA-Z]+$" ValidationGroup="update"
                                    SetFocusOnError="True">*</asp:RegularExpressionValidator>
                            </td>
                        </tr>
                    
                    <tr>
                        <td class="number">
                            <asp:Label ID="UnitNum" runat="server" Text="E" />
                        </td>
                        <td class="label labelRequired">
                            *Unit:
                        </td>
                        <td class="value">
                            <asp:TextBox Enabled="false" runat="server" Width="280px" ID="UnitTextBox" />
                            <asp:Button runat="server" ID="ChangeUnitButton" Text="Change" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" SetFocusOnError="True" runat="server"
                                ErrorMessage="Unit Required." ControlToValidate="newUnitIDLabel" ValidationGroup="register">*</asp:RequiredFieldValidator>
                    
                            <asp:TextBox ID="newUnitIDLabel" CssClass="hidden" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label ID="UserRoleNum" runat="server" Text="F" />
                        </td>
                        <td class="label labelRequired">
                            *User Role:
                        </td>
                        <td class="value">
                            <asp:DropDownList ID="RoleSelect" runat="server" Width="196px">
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="rqfld_role" SetFocusOnError="True" runat="server"
                                InitialValue="" ErrorMessage="Role is required." ControlToValidate="RoleSelect"
                                ValidationGroup="register">*</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label ID="EmailNum" runat="server" Text="G" />
                        </td>
                        <td class="label labelRequired">
                            *Work Email:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="EmailBox" runat="server" Width="190px" MaxLength="100"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rqfd_Email" SetFocusOnError="True" runat="server"
                                ControlToValidate="EmailBox" ErrorMessage="Work Email is required" ValidationGroup="register"
                                Display="Dynamic">*</asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="rgExp_Email" SetFocusOnError="True" runat="server"
                                ControlToValidate="EmailBox" ErrorMessage="Invalid Work Email." ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                                ValidationGroup="register">*</asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label ID="EmailNum2" runat="server" Text="H" />
                        </td>
                        <td class="label">
                            Personal Email:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="Email2Box" runat="server" Width="190px" MaxLength="100"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="Email2Box"
                                ErrorMessage="Invalid Personal Email." ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                                ValidationGroup="register">*</asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label ID="EmailNum3" runat="server" Text="I" />
                        </td>
                        <td class="label">
                            Unit Email:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="Email3Box" runat="server" Width="190px" MaxLength="100"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="Email3Box"
                                ErrorMessage="Invalid Unit Email." ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                                ValidationGroup="register">*</asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label ID="PhoneNum" runat="server" Text="J" />
                        </td>
                        <td class="label labelRequired">
                            *Phone:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="PhoneBox" runat="server" Width="190px" MaxLength="15"></asp:TextBox>
                            <asp:RequiredFieldValidator SetFocusOnError="True" ID="rqfd_Comm" runat="server"
                                ControlToValidate="PhoneBox" ErrorMessage="Phone is required" ValidationGroup="register"
                                Display="Dynamic">*</asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="rgExpCom" runat="server" ControlToValidate="PhoneBox"
                                ErrorMessage="Invalid Commercial Phone." ValidationExpression="\d{3}-\d{3}-\d{4}"
                                ValidationGroup="register" SetFocusOnError="True">*</asp:RegularExpressionValidator>
                            (XXX-XXX-XXXX)
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label ID="DSNNum" runat="server" Text="K" />
                        </td>
                        <td class="label">
                            DSN:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="DsnBox" runat="server" Width="190px" MaxLength="15"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="rgExDSN" runat="server" ControlToValidate="DsnBox"
                                ErrorMessage="DSN Phone is invalid" ValidationExpression="^\d+$" ValidationGroup="register">*</asp:RegularExpressionValidator>
                            <ajax:filteredtextboxextender id="FilteredTextBoxExtender1" targetcontrolid="DsnBox"
                                runat="server" filtertype="Numbers" />
                        </td>
                        <td>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label ID="AddressNum" runat="server" Text="L" />
                        </td>
                        <td class="label labelRequired">
                            *Address:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="txtAddress" runat="server" Width="190px" MaxLength="50"></asp:TextBox>
                            <asp:RequiredFieldValidator SetFocusOnError="True" ID="rqfd_Address" runat="server"
                                ControlToValidate="txtAddress" ErrorMessage="Address is required" ValidationGroup="register">*</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label ID="CityNum" runat="server" Text="M" />
                        </td>
                        <td class="label labelRequired">
                            *City:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="txtCity" runat="server" Width="190px" MaxLength="25"></asp:TextBox>
                            <asp:RequiredFieldValidator SetFocusOnError="True" ID="rqfd_City" ValidationGroup="register"
                                runat="server" ControlToValidate="txtCity" ErrorMessage="City is required">*</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label ID="StateNum" runat="server" Text="N" />
                        </td>
                        <td class="label labelRequired">
                            *State:
                        </td>
                        <td class="value">
                            <asp:DropDownList ID="StateSelect" runat="server" DataTextField="Name" DataValueField="Id"
                                Width="196px">
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator SetFocusOnError="True" ID="rqfld_state" runat="server"
                                InitialValue="" ErrorMessage="State is required." ControlToValidate="StateSelect"
                                ValidationGroup="register">*</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label ID="ZipNum" runat="server" Text="O" />
                        </td>
                        <td class="label labelRequired">
                            *Zip:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="txtZip" runat="server" MaxLength="10" Width="190px"></asp:TextBox>
                            <asp:RequiredFieldValidator SetFocusOnError="True" ID="rqfd_Zip" runat="server" ControlToValidate="txtZip"
                                ErrorMessage="Zip is required" ValidationGroup="register" Display="Dynamic">*</asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="regExp_Zip" runat="server" ControlToValidate="txtZip"
                                ErrorMessage="Invalid Zip Code" ValidationExpression="\d{5}(-\d{4})?" ValidationGroup="register">*</asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label ID="RecieveEmailNum" runat="server" Text="P" />
                        </td>
                        <td class="label">
                            Recieve Email:
                        </td>
                        <td class="value">
                            <asp:CheckBox ID="chkReceiveEmail" runat="server" />
                        </td>
                        <td>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label ID="ActnNum" runat="server" Text="Q" />
                        </td>
                        <td class="label">
                            Action:
                        </td>
                        <td class="value">
                            <asp:Button ID="Create" ValidationGroup="register" runat="server" Text="Create" />&nbsp;
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <br />
    </div>
</asp:Content>
