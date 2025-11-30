<%@ Page Language="VB" MasterPageFile="~/Public/Public.master" AutoEventWireup="false" Inherits="ALOD.Web.Public_Register2" Title="ECT Registration" Codebehind="Register2.aspx.vb" %>

<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="ContentHeader">
</asp:Content>
<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="ContentFooter">
    <asp:ScriptManagerProxy runat="server" ID="scripts">
        <Scripts>

           <%--<script type="text/javascript" src="https://code.jquery.com/jquery-3.6.0.js"></script>--%>
           <%--<script type="text/javascript" src="https://code.jquery.com/jquery-migrate-3.4.1.js"></script>--%>
           <%--<script type="text/javascript" src="../../../Script/jquery-block.js"></script>--%>
           <%--<script type="text/javascript" src="../../../Script/common.js"></script>--%>

            <%--<asp:ScriptReference Path="~/Script/jquery-migrate-3.3.2.min.js" />
            <asp:ScriptReference Path="../../../Script/jquery-ui-1.13.0.min.js" />
            <asp:ScriptReference Path="~/Script/jquery-block.js" />--%>

            <asp:ScriptReference Path="~/Script/jquery-3.6.0.min.js" />
            <asp:ScriptReference Path="~/Script/jquery-migrate-3.4.1.min.js" />
            <asp:ScriptReference Path="../../../Script/jquery-ui-1.13.0.min.js" />
            <asp:ScriptReference Path="~/Script/jquery.blockUI.min.js" />

            <asp:ScriptReference Path="~/Script/common.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">

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

            $('.open-dialog').on({
                mouseenter: function () {
                    $(this).addClass("ui-state-hover");
                },
                mouseleave: function () {
                    $(this).removeClass("ui-state-hover");
                },
                mousedown: function () {
                    $(this).addClass("ui-state-active");
                },
                mouseup: function () {
                    $(this).removeClass("ui-state-active");
                }
            });
        });


        function validateUnit() {
            if (element('<%=SrcUnitIdHdn.ClientId %>').value == "") {
                $(element('<%=lblUnitMsg.ClientId %>')).removeClass("hidden");

            }
            else {
                $(element('<%=lblUnitMsg.ClientId %>')).addClass("hidden");
            }


        }

        //Show Searcher
        function showSearcher(title, targetId, targetlbl) {
            initializeUnitSearcher();
            //element('srhctitle').innerHTML = "Reporting unit for Hierarchy     '" + title + "'";
            //Set Client controls where unit Id and unit names will be transferred
            element('<%=hdnIdClient.ClientId %>').value = targetId;
            element('<%=hdnNameClient.ClientId %>').value = targetlbl;
            $('#srchBox').dialog('open');
        }
        //Client accepted so Set Corresponding reporting units 
        function SetReportingUnit() {
            var srcherId = '<%=unitSearcher.ClientId %>'
            //Id is stroed in a hidden control.Transfer the value from the control
            element(element('<%=hdnIdClient.ClientId %>').value).value = element(srcherId + '_hdnSelectedUnit').value;
            //Name is stored in a label.Transfer the value from the control
            element(element('<%=hdnNameClient.ClientId %>').value).value = element(srcherId + '_hdnSelectedUnitName').value;
            element('<%=PasCodeBox.ClientId %>').value = element(srcherId + '_hdnSelectedUnitName').value;



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
            <lod:unitSearcher ID="unitSearcher" ActiveOnly="true" runat="server" />
        </div>
        <!-- end search control -->
        <input type="hidden" id="hdnIdClient" runat="Server" />
        <input type="hidden" id="hdnNameClient" runat="Server" />
        <br />
    </div>
    <div id="content" class="indent-small">
        <h1>
            ECT Registration > User Information</h1>
        <br />
        <asp:Label runat="server" ID="lblInfo" Text="Please complete the following form to provide or update your user information."
            CssClass="labelNormal" />
        <br />
        <br />
        <asp:Panel ID="inputpanel" runat="server" CssClass="dataBlock">
            <div class="dataBlock-header">
                1 - User Information</div>
            <!-- Name display -->
            <div class="dataBlock-body">
                <asp:Panel runat="server" ID="NameDisplayPanel">
                    <table cellpadding="2" cellspacing="2" style="text-align: left;">
                        <tr>
                            <td class="number">
                                <asp:Label ID="NameNum" runat="server" Text="C" />
                            </td>
                            <td class="label">
                                Name:
                            </td>
                            <td class="value">
                                <asp:Label runat="server" ID="FullName" />
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                <asp:Label ID="RankNumDisplay" runat="server" Text="D" />
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
                <asp:Panel runat="server" ID="NameEntryPanel">
                    <table cellpadding="2" cellspacing="2" style="text-align: left;">
                        <tr>
                            <td class="number">
                                <asp:Label ID="TitleNum" runat="server" />
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
                                <asp:Label ID="FirstNameNum" runat="server" />
                            </td>
                            <td class="label labelRequired">
                                *First Name:
                            </td>
                            <td class="value">
                                <asp:Label ID="FirstNameLabel" runat="server"></asp:Label>
                                <asp:TextBox ID="FirstNameBox" runat="server" Width="190px" MaxLength="50"></asp:TextBox>
                            </td>
                            <td class="error">
                                <asp:RequiredFieldValidator SetFocusOnError="True" ID="rqfd_FName" runat="server"
                                    ControlToValidate="FirstNameBox" ErrorMessage="First Name required" ValidationGroup="register">Please enter your First Name</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                <asp:Label ID="MiddleNameNum" runat="server" />
                            </td>
                            <td class="label">
                                Middle Name:
                            </td>
                            <td class="value">
                                <asp:Label ID="MiddleNameLabel" runat="server"></asp:Label>
                                <asp:TextBox ID="MIddleNameBox" runat="server" Width="190px" MaxLength="50"></asp:TextBox>
                            </td>
                            <td>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                <asp:Label ID="LastNameNum" runat="server" />
                            </td>
                            <td class="labelRequired label">
                                *Last Name:
                            </td>
                            <td class="value">
                                <asp:Label ID="LastNameLabel" runat="server"></asp:Label>
                                <asp:TextBox ID="LastNameBox" runat="server" Width="190px" MaxLength="50"></asp:TextBox>
                            </td>
                            <td class="error">
                                <asp:RequiredFieldValidator SetFocusOnError="True" ID="rqfd_LName" runat="server"
                                    ControlToValidate="LastNameBox" ErrorMessage="Last Name required" ValidationGroup="register">Please enter your Last Name</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                <asp:Label ID="RankNumEntry" runat="server" />
                            </td>
                            <td class="labelRequired label">
                                *Rank:
                            </td>
                            <td class="value">
                                <asp:Label ID="RankLabel" runat="server"></asp:Label>
                                <asp:DropDownList ID="RankSelect" runat="server" DataTextField="text" DataValueField="value"
                                    Width="196px">
                                </asp:DropDownList>
                            </td>
                            <td class="error">
                                <asp:RequiredFieldValidator SetFocusOnError="True" ID="rqfld_rank" runat="server"
                                    InitialValue="-1" ErrorMessage="Rank rquired." ControlToValidate="RankSelect"
                                    ValidationGroup="register">Please select your Rank</asp:RequiredFieldValidator>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <!-- end Name entry -->
                <table cellpadding="2" cellspacing="2" style="text-align: left;">
                    <tr>
                        <td class="number">
                            <asp:Label ID="UnitNum" runat="server" Text="D" />
                        </td>
                        <td class="label labelRequired">
                            *Unit:
                            <input type="hidden" id="SrcNameHdn" runat="Server" />
                            <asp:TextBox ID="SrcUnitIdHdn" runat="server" CssClass="hidden"></asp:TextBox>
                        </td>
                        <td class="value">
                            <asp:Label ID="PasCodeHidden" runat="server" Visible="false"></asp:Label>
                            <asp:Label ID="PasCodeLabel" runat="server"></asp:Label>
                            <asp:TextBox ID="PasCodeBox" runat="server" Width="280px" Enabled="False"></asp:TextBox>
                            <asp:Button Width="80px" ID="btnFindUnit" CausesValidation="True" ValidationGroup="create"
                                Text="Find Unit" runat="server"></asp:Button>
                            <asp:Label ForeColor="Red" runat="server" CssClass="hidden" ID="lblUnitMsg" Text="Unit not selected"></asp:Label>
                        </td>
                        <td class="error">
                            <asp:RequiredFieldValidator SetFocusOnError="True" ID="rqfd_pascode" runat="server"
                                ControlToValidate="SrcUnitIdHdn" ErrorMessage="pascode is required" ValidationGroup="register"
                                Display="Dynamic">Please select your unit</asp:RequiredFieldValidator>
                            <asp:CustomValidator ID="CustomValidator1" runat="server" ErrorMessage="Invalid pascode"
                                ValidationGroup="register">Invalid Unit</asp:CustomValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label ID="UserRoleNum" runat="server" Text="E" />
                        </td>
                        <td class="label labelRequired">
                            *User Role:
                        </td>
                        <td class="value">
                            <asp:DropDownList ID="RoleSelect" runat="server" Width="196px">
                            </asp:DropDownList>
                        </td>
                        <td class="error">
                            <asp:RequiredFieldValidator SetFocusOnError="True" ID="rqfld_role" runat="server"
                                InitialValue="-1" ErrorMessage="Role is required." ControlToValidate="RoleSelect"
                                ValidationGroup="register">Please select a User Role</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label ID="EmailNum" runat="server" Text="F" />
                        </td>
                        <td class="label labelRequired">
                            *Work Email:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="EmailBox" runat="server" Width="190px" MaxLength="100"></asp:TextBox>
                        </td>
                        <td class="error">
                            <asp:RequiredFieldValidator SetFocusOnError="True" ID="rqfd_Email" runat="server"
                                ControlToValidate="EmailBox" ErrorMessage="Email required" ValidationGroup="register"
                                Display="Dynamic">Please Enter Work Email</asp:RequiredFieldValidator><asp:RegularExpressionValidator
                                    ID="rgExp_Email" runat="server" ControlToValidate="EmailBox" ErrorMessage="Invalid Work Email."
                                    ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="register">Invalid Work Email</asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label ID="EmailNum2" runat="server" Text="G" />
                        </td>
                        <td class="label">
                            Personal Email:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="Email2Box" runat="server" Width="190px" MaxLength="100"></asp:TextBox>
                        </td>
                        <td class="error">
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="Email2Box"
                                ErrorMessage="Invalid Personal Email." ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                                ValidationGroup="register">Invalid Personal Email</asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label ID="Component" runat="server" Text="H" />
                        </td>
                        <td class="label">
                            Service:
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
                            <asp:Label ID="EmailNum3" runat="server" Text="H" />
                        </td>
                        <td class="label">
                            Unit Email:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="Email3Box" runat="server" Width="190px" MaxLength="100"></asp:TextBox>
                        </td>
                        <td class="error">
                            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="Email3Box"
                                ErrorMessage="Invalid Unit Email." ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"
                                ValidationGroup="register">Invalid Unit Email</asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label ID="PhoneNum" runat="server" Text="I" />
                        </td>
                        <td class="label labelRequired">
                            *Phone:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="PhoneBox" runat="server" Width="190px" MaxLength="15"></asp:TextBox>
                        </td>
                        <td class="error">
                            <asp:RegularExpressionValidator ID="rgExpCom" runat="server" ControlToValidate="PhoneBox"
                                Display="Dynamic" ErrorMessage="Invalid Phone." ValidationExpression="\d{3}-\d{3}-\d{4}"
                                ValidationGroup="register" SetFocusOnError="True">Invalid Commercial Phone</asp:RegularExpressionValidator>
                            (XXX-XXX-XXXX)
                            <asp:RequiredFieldValidator SetFocusOnError="True" ID="rqfd_Comm" runat="server"
                                ControlToValidate="PhoneBox" ErrorMessage="Phone required" ValidationGroup="register"
                                Display="Dynamic">Please enter your phone number</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label ID="DSNNum" runat="server" Text="J" />
                        </td>
                        <td class="label">
                            DSN:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="DsnBox" runat="server" Width="190px" MaxLength="15"></asp:TextBox>
                        </td>
                        <td>
                            <asp:RegularExpressionValidator ID="rgExDSN" runat="server" ControlToValidate="DsnBox"
                                ErrorMessage="DSN Phone is invalid" ValidationExpression="^\d+$" ValidationGroup="register">*</asp:RegularExpressionValidator>
                            <ajax:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" TargetControlID="DsnBox"
                                runat="server" FilterType="Numbers" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label ID="AddressNum" runat="server" Text="K" />
                        </td>
                        <td class="label">
                            Address:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="txtAddress" runat="server" Width="190px" MaxLength="50"></asp:TextBox>
                        </td>
                        <td class="error">
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label ID="CityNum" runat="server" Text="L" />
                        </td>
                        <td class="label">
                            City:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="txtCity" runat="server" Width="190px" MaxLength="25"></asp:TextBox>
                        </td>
                        <td class="error">
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label ID="StateNum" runat="server" Text="M" />
                        </td>
                        <td class="label">
                            State:
                        </td>
                        <td class="value">
                            <asp:DropDownList ID="cbState" runat="server" DataTextField="text" DataValueField="value"
                                Width="196px">
                            </asp:DropDownList>
                        </td>
                        <td class="error">
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label ID="ZipNum" runat="server" Text="N" />
                        </td>
                        <td class="label">
                            Zip:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="txtZip" runat="server" MaxLength="10" Width="190px"></asp:TextBox>
                        </td>
                        <td class="error">
                            <asp:RegularExpressionValidator ID="regExp_Zip" runat="server" ControlToValidate="txtZip"
                                ErrorMessage="Invalid Zip Code" ValidationExpression="\d{5}(-\d{4})?" ValidationGroup="register">Invalid Zipcode</asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label ID="RecieveEmailNum" runat="server" Text="O" />
                        </td>
                        <td class="label">
                            Receive Email:
                        </td>
                        <td class="value">
                            <asp:CheckBox ID="chkReceiveEmail" runat="server" />
                        </td>
                        <td>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                        </td>
                        <td class="label">
                            Action:
                        </td>
                        <td class="value">
                            <asp:Button ID="NextButton" runat="server" Text="Next" ValidationGroup="register" />
                        </td>
                        <td>
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <br />
    </div>
</asp:Content>
