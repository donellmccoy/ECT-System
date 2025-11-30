<%@ Page Title="" Language="VB" MasterPageFile="~/Public/Public.master" AutoEventWireup="false" Inherits="ALOD.Web.Public_RegisterLink" Codebehind="RegisterLink.aspx.vb" %>

<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="ContentFooter">

    <script type="text/javascript">

        $(document).ready(function() {

            $('.openrcpha').click(function() {
                CheckRadio();
                Redirect();
            });

            CheckRadio();
        });

        function CheckRadio() {

            if ($("input[@id='" + '<%=rcphaRadio.ClientID%>' + '_0' + "']:checked").val() == 'Y') {
                $('#rcphadiv').removeClass("hidden");
            }
            else {
                $('#rcphadiv').addClass("hidden");
            }
        }

        function Redirect() {
            if ($("input[@id='" + '<%=rcphaRadio.ClientID%>' + '_1' + "']:checked").val() == 'N') {
                $('#rcphadiv').addClass("hidden");
                var url = $_HOSTNAME + "/Public/Register1.aspx";
                window.location.href = url;
            }


        }
    </script>

</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <br />
    <br />
    <div style="padding: 10px 30px;">
        <div class="dataBlock">
            <div class="dataBlock-header">
                Account not found.Are you a returning RCPHA user ?
            </div>
            <asp:RadioButtonList ID="rcphaRadio" class="openrcpha" runat="server" RepeatDirection="Vertical">
                <asp:ListItem Value="Y">Yes</asp:ListItem>
                <asp:ListItem Value="N">No</asp:ListItem>
            </asp:RadioButtonList>
        </div>
    </div>
    <div id="rcphadiv" style="padding: 10px 30px;" class="hidden">
        <div class="dataBlock">
            <div class="dataBlock-header">
                <asp:Label ID="rcphaLbl" runat="server" Text="Please enter your credentials to link your existing account with our new system.Please note that this 
            is a one time requirement.">
                </asp:Label>
            </div>
            <br />
            <div style="text-align: center;">
              <asp:Label ID="IncorrectSSNLabel"  ForeColor ="Red" runat="server" Text="SSN does not match with electronic ID" Visible="False"></asp:Label>
             </div>
            <div style="text-align: left;">
              <asp:Label ID="InvalidSSNLabel"  ForeColor ="Red" runat="server" Text="  Invalid SSN" Visible="False"></asp:Label>
                <asp:ValidationSummary ID="vld_Summary" runat="server" ValidationGroup="link" />
            </div>
            <div class="dataBlock-body">
                <table>
                    <tr>
                        <td class="number">
                            <asp:Label ID="SSNNum" runat="server" Text="A" />
                        </td>
                        <td class="label labelRequired">
                            SSN:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="SSNTextBox"  MaxLength="9" runat="server"></asp:TextBox>
                               <asp:RequiredFieldValidator ID="rqfd_ssn" runat="server" ControlToValidate="SSNTextBox"
                                ErrorMessage="SSN is required" ValidationGroup="link" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="regExpssn" runat="server" ControlToValidate="SSNTextBox" 
                                ErrorMessage="SSN Must be at least 9 digits" ValidationGroup="link"  ValidationExpression="\d{9}">*</asp:RegularExpressionValidator>

                           
                            
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label ID="UserNameNum" runat="server" Text="B" />
                        </td>
                        <td class="label labelRequired">
                            User Name:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="UserNameTextBox" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rqfd_un" runat="server" ControlToValidate="UserNameTextBox"
                                ErrorMessage="User Name is required" ValidationGroup="link" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label ID="FirstNameNum" runat="server" Text="C" />
                        </td>
                        <td class="label labelRequired">
                            First Name:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="FirstNameTextBox" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rqfd_fn" runat="server" ControlToValidate="FirstNameTextBox"
                                ErrorMessage="First Name is required" ValidationGroup="link" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label ID="LastNameNum" runat="server" Text="D" />
                        </td>
                        <td class="label labelRequired">
                            Last Name:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="LastNameTextBox" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="rqfd_ln" runat="server" ControlToValidate="LastNameTextBox"
                                ErrorMessage="Last Name is required" ValidationGroup="link" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label ID="Actionnum" runat="server" Text="E" />
                        </td>
                        <td class="label">
                            Action:
                        </td>
                        <td class="value">
                            <asp:Button ID="linkBtn" ValidationGroup="link" Text="Link Account" runat="server">
                            </asp:Button>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
         <br />
 
        <div id="accountLinkedDiv" style="text-align: center; color: green;" runat="server"  visible="false">
             Your Account was successfully linked.<asp:LinkButton runat="server" ID="accountLinkButton"
                Text="Please click the here to login."></asp:LinkButton>
        </div>
          <div id="registerLinkedDiv" style="text-align: center;" runat="server"  visible="false">
           
            Your Account could not be linked.<asp:LinkButton runat="server" ID="registerLinkButton"
                Text="Please click  here to be redirected to registration"></asp:LinkButton>or  try again.
        </div>
        <br />
    </div>
</asp:Content>
