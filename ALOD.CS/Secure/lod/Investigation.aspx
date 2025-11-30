<%@ Page Title="" Language="C#" MasterPageFile="~/Secure/Lod/LOD.master" AutoEventWireup="false" Inherits="ALOD.Web.LOD.Secure_lod_Investigation" CodeBehind="Investigation.aspx.cs" %>

<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/Lod/LOD.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Reference Control="~/Secure/Shared/UserControls/ModuleHeader.ascx" %>
<%@ Reference Control="~/Secure/Shared/UserControls/ReviewControl.ascx" %>
<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Register Src="~/secure/Shared/UserControls/ReviewControl.ascx" TagName="Review"
    TagPrefix="uc" %>
<%@ Register Src="~/secure/Shared/UserControls/FindingsControl.ascx" TagName="Findings"
    TagPrefix="uc" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck"
    TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentNested" runat="Server">
    <asp:Panel ID="ControlsPanel" runat="server">
        <div class="formHeader">
            1 - Formal Investigation
        </div>
        <table class="dataTable">
            <tr>
                <td class="number">
                    A
                </td>
                <td class="label labelRequired">
                    *Report Date:
                </td>
                <td class="value">
                    <asp:TextBox ID="txtDateReport" onchange="DateCheck(this);" runat="server" Width="80"
                        MaxLength="10"></asp:TextBox>
                    <asp:Label ID="lblDateReport" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="number">
                    B
                </td>
                <td class="label labelRequired">
                    *Investigation Of:
                </td>
                <td class="value">
                    <asp:RadioButtonList ID="rblInvestigationOf" runat="server" RepeatDirection="Horizontal"
                        CellSpacing="0">
                        <asp:ListItem Text="Disease" Value="1" />
                        <asp:ListItem Text="Illness" Value="2" />
                        <asp:ListItem Text="Death" Value="3" />
                        <asp:ListItem Text="Injury (non MVA)" Value="4" />
                        <asp:ListItem Text="Injury (MVA)" Value="5" />
                    </asp:RadioButtonList>
                    <asp:Label ID="lblInvestigationOf" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="3">
                    &nbsp;
                </td>
            </tr>
        </table>
        <div class="formHeader">
            2 - Member Status
        </div>
        <table class="dataTable">
            <tr>
                <td class="number">
                    A
                </td>
                <td class="label">
                    Regular or EAD:
                </td>
                <td class="value">
                    <asp:RadioButton runat="server" ID="rbRegularOrEad" GroupName="MemberStatus" />
                    <asp:Image ID="StatusRegularImage" runat="server" Height="16px" ImageAlign="AbsMiddle"
                        ImageUrl="~/images/check.gif" Visible="False" AlternateText="Is Checked" />
                </td>
            </tr>
            <tr>
                <td class="number">
                    B
                </td>
                <td class="label">
                    Called or Ordered to AD:
                </td>
                <td class="value">
                    <asp:RadioButton runat="server" ID="rbAdMore" GroupName="MemberStatus" Text="More than 30 days" />
                    <asp:RadioButton runat="server" ID="rbAdLess" GroupName="MemberStatus" Text="Less than 30 days" />
                    <asp:Image ID="StatusADImage" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/images/check.gif"
                        Visible="False" AlternateText="Is Checked" />
                    <asp:Label ID="AdDurationLabel" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="number">
                    C
                </td>
                <td class="label">
                    Inactive &nbsp;Duty Training:
                </td>
                <td class="value">
                    <asp:RadioButton runat="server" ID="rbInactive" GroupName="MemberStatus" />
                    <asp:Image ID="StatusInactiveImage" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/images/check.gif"
                        Visible="False" AlternateText="Is Checked" />
                    <asp:TextBox ID="txtInactiveDutyTraining" runat="server" Width="199px" MaxLength="50"></asp:TextBox>
                    <asp:Label ID="lblInactiveDutyTraining" runat="server" CssClass="lblDisableText"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="number">
                    D
                </td>
                <td class="label">
                    Short Tour of AD for Training:
                </td>
                <td class="value">
                    <asp:RadioButton runat="server" ID="rbShortTour" GroupName="MemberStatus" />
                    <asp:Image ID="StatusShortImage" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/images/check.gif"
                        Visible="False" AlternateText="Is Checked" />
                </td>
            </tr>
        </table>
        <div id="dutyDates">
            <table class="dataTable">
                <tr>
                    <td class="number">
                        E
                    </td>
                    <td class="label labelRequired">
                        *Start Date and Hour:
                    </td>
                    <td class="value">
                        <asp:TextBox ID="txtDateStart" onchange="DateCheck(this);" runat="server" Width="80"
                            MaxLength="10"></asp:TextBox>
                        <asp:TextBox ID="txtHrStart" MaxLength="4" onchange="TimeCheck(this);" runat="server"
                            Width="40"></asp:TextBox>
                        <ajax:FilteredTextBoxExtender ID="FilteredTextBoxExtender2" TargetControlID="txtHrStart"
                            runat="server" FilterType="Numbers">
                        </ajax:FilteredTextBoxExtender>
                        <asp:Label ID="lblDateStart" runat="server" class="lblDisableText"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        F
                    </td>
                    <td class="label labelRequired">
                        *Finish Date and Hour:
                    </td>
                    <td class="value">
                        <asp:TextBox ID="txtDateFinish" onchange="DateCheck(this);" runat="server" Width="80"
                            MaxLength="10"></asp:TextBox>
                        <asp:TextBox ID="txtHrFinish" MaxLength="4" onchange="TimeCheck(this);" runat="server"
                            Width="40"></asp:TextBox>
                        <ajax:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" TargetControlID="txtHrFinish"
                            runat="server" FilterType="Numbers">
                        </ajax:FilteredTextBoxExtender>
                        <asp:Label ID="lblDateFinish" runat="server" class="lblDisableText"></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
        <!--Begin Other Personnel-->
        <div class="formHeader">
            3 - Other Personnel Involved With This Incident
        </div>
        <table class="dataTable">
            <tr>
                <td>
                </td>
                <td>
                </td>
                <td style="width: 200px;">
                    Name
                </td>
                <td style="width: 80px;">
                    Grade
                </td>
                <td>
                    Investigation Made
                </td>
            </tr>
            <tr>
                <td class="number">
                    A
                </td>
                <td class="label">
                    Other Personnel #1:
                </td>
                <td>
                    <asp:TextBox ID="txtOtherName1" runat="server" Width="200" MaxLength="50"></asp:TextBox>
                    <asp:Label ID="lblOtherName1" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlGrade1" runat="server" Width="100">
                        <asp:ListItem></asp:ListItem>
                        <asp:ListItem>E1</asp:ListItem>
                        <asp:ListItem>E2</asp:ListItem>
                        <asp:ListItem>E3</asp:ListItem>
                        <asp:ListItem>E4</asp:ListItem>
                        <asp:ListItem>E5</asp:ListItem>
                        <asp:ListItem>E6</asp:ListItem>
                        <asp:ListItem>E7</asp:ListItem>
                        <asp:ListItem>E8</asp:ListItem>
                        <asp:ListItem>E9</asp:ListItem>
                        <asp:ListItem>W1</asp:ListItem>
                        <asp:ListItem>W2</asp:ListItem>
                        <asp:ListItem>W3</asp:ListItem>
                        <asp:ListItem>W4</asp:ListItem>
                        <asp:ListItem>O1</asp:ListItem>
                        <asp:ListItem>O2</asp:ListItem>
                        <asp:ListItem>O3</asp:ListItem>
                        <asp:ListItem>O4</asp:ListItem>
                        <asp:ListItem>O5</asp:ListItem>
                        <asp:ListItem>O6</asp:ListItem>
                        <asp:ListItem>O7</asp:ListItem>
                        <asp:ListItem>O8</asp:ListItem>
                        <asp:ListItem>O9</asp:ListItem>
                        <asp:ListItem>O10</asp:ListItem>
                    </asp:DropDownList>
                    <asp:Label ID="lblOtherGrade1" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:CheckBox ID="chkOtherInvestMade1" Width="20" runat="server"></asp:CheckBox>
                    <asp:Image ID="Invest1Image" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/images/check.gif"
                        Visible="False" AlternateText="Is Checked" />
                </td>
            </tr>
            <tr>
                <td class="number">
                    B
                </td>
                <td class="label">
                    Other Personnel #2:
                </td>
                <td>
                    <asp:TextBox ID="txtOtherName2" runat="server" Width="200" MaxLength="50"></asp:TextBox>
                    <asp:Label ID="lblOtherName2" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlGrade2" runat="server" Width="100">
                        <asp:ListItem></asp:ListItem>
                        <asp:ListItem>E1</asp:ListItem>
                        <asp:ListItem>E2</asp:ListItem>
                        <asp:ListItem>E3</asp:ListItem>
                        <asp:ListItem>E4</asp:ListItem>
                        <asp:ListItem>E5</asp:ListItem>
                        <asp:ListItem>E6</asp:ListItem>
                        <asp:ListItem>E7</asp:ListItem>
                        <asp:ListItem>E8</asp:ListItem>
                        <asp:ListItem>E9</asp:ListItem>
                        <asp:ListItem>W1</asp:ListItem>
                        <asp:ListItem>W2</asp:ListItem>
                        <asp:ListItem>W3</asp:ListItem>
                        <asp:ListItem>W4</asp:ListItem>
                        <asp:ListItem>O1</asp:ListItem>
                        <asp:ListItem>O2</asp:ListItem>
                        <asp:ListItem>O3</asp:ListItem>
                        <asp:ListItem>O4</asp:ListItem>
                        <asp:ListItem>O5</asp:ListItem>
                        <asp:ListItem>O6</asp:ListItem>
                        <asp:ListItem>O7</asp:ListItem>
                        <asp:ListItem>O8</asp:ListItem>
                        <asp:ListItem>O9</asp:ListItem>
                        <asp:ListItem>O10</asp:ListItem>
                    </asp:DropDownList>
                    <asp:Label ID="lblOtherGrade2" runat="server" CssClass="lblDisableText"></asp:Label>
                </td>
                <td>
                    <asp:CheckBox ID="chkOtherInvestMade2" runat="server"></asp:CheckBox>
                    <asp:Image ID="Invest2Image" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/images/check.gif"
                        Visible="False" AlternateText="Is Checked" />
                </td>
            </tr>
            <tr>
                <td class="number">
                    C
                </td>
                <td class="label">
                    Other Personnel #3:
                </td>
                <td>
                    <asp:TextBox ID="txtOtherName3" runat="server" Width="200" MaxLength="50"></asp:TextBox>
                    <asp:Label ID="lblOtherName3" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="ddlGrade3" runat="server" Width="100">
                        <asp:ListItem></asp:ListItem>
                        <asp:ListItem>E1</asp:ListItem>
                        <asp:ListItem>E2</asp:ListItem>
                        <asp:ListItem>E3</asp:ListItem>
                        <asp:ListItem>E4</asp:ListItem>
                        <asp:ListItem>E5</asp:ListItem>
                        <asp:ListItem>E6</asp:ListItem>
                        <asp:ListItem>E7</asp:ListItem>
                        <asp:ListItem>E8</asp:ListItem>
                        <asp:ListItem>E9</asp:ListItem>
                        <asp:ListItem>W1</asp:ListItem>
                        <asp:ListItem>W2</asp:ListItem>
                        <asp:ListItem>W3</asp:ListItem>
                        <asp:ListItem>W4</asp:ListItem>
                        <asp:ListItem>O1</asp:ListItem>
                        <asp:ListItem>O2</asp:ListItem>
                        <asp:ListItem>O3</asp:ListItem>
                        <asp:ListItem>O4</asp:ListItem>
                        <asp:ListItem>O5</asp:ListItem>
                        <asp:ListItem>O6</asp:ListItem>
                        <asp:ListItem>O7</asp:ListItem>
                        <asp:ListItem>O8</asp:ListItem>
                        <asp:ListItem>O9</asp:ListItem>
                        <asp:ListItem>O10</asp:ListItem>
                    </asp:DropDownList>
                    <asp:Label ID="lblOtherGrade3" runat="server"></asp:Label>
                </td>
                <td>
                    <asp:CheckBox ID="chkOtherInvestMade3" runat="server"></asp:CheckBox>
                    <asp:Image ID="Invest3Image" runat="server" ImageAlign="AbsMiddle" ImageUrl="~/images/check.gif"
                        Visible="False" AlternateText="Is Checked" />
                </td>
            </tr>
        </table>
        <!--End Other Personnel-->
        <div class="formHeader">
            4 - Basis for Findings <span class="formSubheader">(As determined by investigation)</span>
        </div>
        <table class="dataTable">
            <tr>
                <td colspan="3">
                    <br />
                </td>
            </tr>
            <tr>
                <td class="number">
                    &nbsp;&nbsp;
                </td>
                <td class="label">
                    Circumstances
                </td>
                <td class="value">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td class="number">
                    A
                </td>
                <td class="label labelRequired">
                    *Date:
                </td>
                <td class="value">
                    <asp:TextBox ID="txtDateCircumstance" onchange="DateCheck(this);" runat="server"
                        Width="80" MaxLength="10"></asp:TextBox>
                    <asp:Label ID="lblDateCircumstance" runat="server" CssClass="lblDisableText"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="number">
                    B
                </td>
                <td class="label labelRequired">
                    *Hour (HHMM):
                </td>
                <td class="value">
                    <asp:TextBox ID="txtHrCircumstance" MaxLength="4" onchange="TimeCheck(this);" runat="server"
                        Width="40"></asp:TextBox>
                    <asp:Label ID="lblHourCircumstance" runat="server" CssClass="lblDisableText"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="number">
                    C
                </td>
                <td class="label labelRequired">
                    *Place:
                </td>
                <td class="value">
                    <asp:TextBox ID="txtCircumstancePlace" runat="server" Width="200" MaxLength="50"></asp:TextBox>
                    <asp:Label ID="lblCircumstancePlace" runat="server" CssClass="lblDisableText"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="number">
                    D
                </td>
                <td class="label labelRequired">
                    *How Sustained:
                </td>
                <td class="value">
                    <asp:TextBox ID="txtCircumstanceSustained" CssClass="fieldNormal" runat="server"
                        Width="500px" MaxLength="250" TextMode="MultiLine" Rows="5"></asp:TextBox>
                    <asp:Label ID="lblCircumstanceSustained" runat="server" CssClass="lblDisableText"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="number">
                    E
                </td>
                <td class="label">
                    Medical Diagnosis:
                </td>
                <td class="value">
                    <asp:TextBox ID="txtDiagnosis" CssClass="fieldNormal" runat="server" Width="500px"
                        MaxLength="250" TextMode="MultiLine" Rows="5"></asp:TextBox>
                    <asp:Label ID="lblDiagnosis" runat="server" CssClass="lblDisableText"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="number">
                    F
                </td>
                <td class="label labelRequired">
                    *Present For Duty?:
                </td>
                <td class="value">
                    <asp:RadioButtonList ID="rblPresentForDuty" CssClass="fieldNormal" 
                        runat="server" RepeatDirection="Horizontal">
                        <asp:ListItem Value="True">Yes</asp:ListItem>
                        <asp:ListItem Value="False">No</asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:Label ID="lblPresentForDuty" runat="server" CssClass="lblDisableText"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="number">
                    G
                </td>
                <td class="label labelRequired">
                    *If Absent:
                </td>
                <td class="value">
                    <span class="normal">
                        <asp:RadioButtonList ID="rblAbsentWithAuthority" CssClass="fieldNormal" 
                        runat="server" RepeatDirection="Horizontal" CellPadding="0">
                            <asp:ListItem Value="True">With Authority</asp:ListItem>
                            <asp:ListItem Value="False">Without Authority</asp:ListItem>
                        </asp:RadioButtonList>
                    </span>
                    <asp:Label ID="lblAbsentWithAuthority" runat="server" CssClass="lblDisableText"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="number">
                    H
                </td>
                <td class="label labelRequired">
                    *Was Intentional Misconduct or Neglect the Proximate Cause?
                </td>
                <td class="value">
                    <span class="normal">
                        <asp:RadioButtonList ID="rblIntentionalMisconduct" CssClass="fieldNormal" 
                        runat="server" RepeatDirection="Horizontal">
                            <asp:ListItem Value="True">Yes</asp:ListItem>
                            <asp:ListItem Value="False">No</asp:ListItem>
                        </asp:RadioButtonList>
                    </span>
                    <asp:Label ID="lblIntentionalMisconduct" runat="server" CssClass="lblDisableText"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="number">
                    I
                </td>
                <td class="label labelRequired">
                    *Was Individual Mentally Sound?
                </td>
                <td class="value">
                    <span class="normal">
                        <asp:RadioButtonList ID="rblMentallySound" CssClass="fieldNormal" 
                        runat="server" RepeatDirection="Horizontal">
                            <asp:ListItem Value="True">Yes</asp:ListItem>
                            <asp:ListItem Value="False">No</asp:ListItem>
                        </asp:RadioButtonList>
                    </span>
                    <asp:Label ID="lblMentallySound" runat="server" CssClass="lblDisableText"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="number">
                    J
                </td>
                <td class="label labelRequired">
                    *Remarks:
                </td>
                <td class="value">
                    <asp:TextBox ID="txtRemarks" runat="server" Width="500px" MaxLength="500" TextMode="MultiLine"
                        Rows="10"></asp:TextBox>
                    <asp:Label ID="lblRemarks" runat="server" CssClass="lblDisableText"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="3">
                </td>
            </tr>
        </table>
        <div class="formHeader">
            5 - Findings
        </div>
        <table class="dataTable">
            <tr>
                <td class="number">
                    A
                </td>
                <td class="label labelRequired" style="margin-top: 4px;">
                    *Findings:
                </td>
                <td class="value">
                    <span class="normal">
                        <asp:RadioButtonList ID="rblFindings" runat="server" RepeatDirection="Vertical" CellPadding="0"
                            CellSpacing="3">
<%--                            <asp:ListItem Value="1">In Line of Duty </asp:ListItem>
                            <asp:ListItem Value="2">EPTS - LOD Not Applicable</asp:ListItem>
                            <asp:ListItem Value="3">EPTS - Service Aggravated</asp:ListItem>
                            <asp:ListItem Value="5">Not in Line of Duty - Not Due to Own Misconduct</asp:ListItem>
                            <asp:ListItem Value="4">Not in Line of Duty - Due to Own Misconduct</asp:ListItem>--%>
                        </asp:RadioButtonList>
                    </span>
                    <asp:Label ID="lblFindings" runat="server" CssClass="lblDisableText"></asp:Label>
                </td>
            </tr>
        </table>
        <br />
        <br />
        <br />
        <!--End of Main Table-->
        <div class="formHeader">
            6 - Investigating Officer
        </div>
        <table class="dataTable">
            <tr>
                <td class="number">
                    A
                </td>
                <td class="label">
                    Name:
                </td>
                <td class="value">
                    <asp:Label ID="lblIOName" runat="server"> </asp:Label>
                </td>
            </tr>
            <tr>
                <td class="number">
                    B
                </td>
                <td class="label">
                    Grade:
                </td>
                <td class="value">
                    <asp:Label ID="lblIOGrade" runat="server"> </asp:Label>
                </td>
            </tr>
<%--            <tr>
                <td class="number">
                    C
                </td>
                <td class="label">
                    Branch of Service:
                </td>
                <td class="value">
                    <asp:Label ID="lblIOBranch" runat="server"> </asp:Label>
                </td>
            </tr>--%>
            <tr>
                <td class="number">
                    C
                </td>
                <td class="label">
                    Organization and Station:
                </td>
                <td class="value">
                    <asp:Label ID="lblIOUnit" runat="server"> </asp:Label>
                </td>
            </tr>
        </table>
        <uc1:SignatureCheck ID="SigCheck" runat="server" Template="Form261" CssClass="sigcheck-form" />
    </asp:Panel>
    <input type="hidden" id="page_readOnly" runat="Server" />
</asp:Content>
<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="FooterNested">

    <script type="text/javascript">

        $(function() {

            if (element('page_readOnly').value == 'Y') {
                return;
            }

            $('.datePicker').datepicker(calendarPick("Past", "<%=CalendarImage %>"));

            $('.datePickerAll').datepicker(calendarPick("All", "<%=CalendarImage %>"));

            CheckAll();
            CheckPresentForDuty();

        });

        function toggleDutyDates(show) {

            if (show) {
                //   $('#dutyDates').slideDown();
            } else {
                //    $('#dutyDates').slideUp();
            }

        }

        function checkSSN(lastId) {

            var ssnText = Trim(element('txtOtherSSN' + lastId).value);
            var ssnTextLength = 0;

            var ssnValue = ssnText.substring(0, 8);
            ssnTextLength = ssnText.length;
            if ((ssnTextLength == 9) && (!isNaN(ssnValue))) {

                return true;
            }
            else {
                alert("Please enter a valid SSN.")
                return false;

            }

        }


        //Checks present for duty.If absent then absence with or without authorty should be checked 

        function CheckPresentForDuty() {
            if (radioElement('rblPresentForDuty')[0].checked) {
                radioElement('rblAbsentWithAuthority')[0].disabled = true;
                radioElement('rblAbsentWithAuthority')[0].checked = false;
                radioElement('rblAbsentWithAuthority')[1].disabled = true;
                radioElement('rblAbsentWithAuthority')[1].checked = false;

            }
            else {
                radioElement('rblAbsentWithAuthority')[0].disabled = false;
                radioElement('rblAbsentWithAuthority')[1].disabled = false;
            }

        }


        //Helper function to check create exclusive list of radio  list buttons for status

        function CheckAll() {

            CheckInactiveDuty();
            CheckDuration();
        }

        //Enable disable Inactive duty text box 
        function CheckInactiveDuty() {
            if (radioElementById('rbInactive').checked) {

                $(element('txtInactiveDutyTraining')).attr("disabled", false);
                $(element('txtInactiveDutyTraining')).attr("class", "");
            }
            else {

                $(element('txtInactiveDutyTraining')).attr("disabled", true);
                $(element('txtInactiveDutyTraining')).attr("class", "ctldisabled");
                $(element('txtInactiveDutyTraining')).val("");
            }
        }



        //Enable disable Start and End Date text boxex.Should be enabled only if person was on a short active duty  or inactive duty
        function CheckDuration() {
            if (radioElementById('rbInactive').checked || radioElementById('rbShortTour').checked) {


                $(element('txtDateStart')).attr("disabled", false);
                $(element('txtHrStart')).attr("disabled", false);
                $(element('txtDateStart')).datepicker('enable');
                $(element('txtHrStart')).datepicker('enable');


                $(element('txtDateFinish')).attr("disabled", false);
                $(element('txtHrFinish')).attr("disabled", false);
                $(element('txtDateFinish')).datepicker('enable');
                $(element('txtHrFinish')).datepicker('enable');

            }
            else {

                $(element('txtDateStart')).attr("disabled", true);
                $(element('txtHrStart')).attr("disabled", true);
                $(element('txtDateStart')).datepicker('disable');
                $(element('txtHrStart')).datepicker('disable');

                $(element('txtDateFinish')).datepicker('disable');
                $(element('txtHrFinish')).datepicker('disable');
                $(element('txtDateFinish')).attr("disabled", true);
                $(element('txtHrFinish')).attr("disabled", true);

                element('txtDateStart').value = '';
                element('txtHrStart').value = '';
                element('txtDateFinish').value = '';
                element('txtHrFinish').value = '';
            }


        }




        function countNum(field) {
            var ssnText = field.value;
            var ssnTextLength = 0;
            var lastID = Right(field.id, 1)
            var val = ssnText.substring(0, 8);
            ssnTextLength = ssnText.length;

            if ((ssnTextLength == 9) && (!isNaN(val))) {

            }

            else {

                $(element('txtOtherName' + lastID)).attr("disabled", true);
                element('txtOtherName' + lastID).value = '';
                element('ddlGrade' + lastID).selectedIndex = 0;
                $(element('ddlGrade' + lastID)).attr("disabled", true);
                $(element('chkOtherInvestMade' + lastID)).attr("disabled", true);
                $(element('chkOtherInvestMade' + lastID)).attr("checked", false);

            }
            return true;
        }


        function Right(str, n) {
            if (n <= 0)
                return "";
            else if (n > String(str).length)
                return str;
            else {
                var iLen = String(str).length;
                return String(str).substring(iLen, iLen - n);
            }
        }

        
     
    </script>

</asp:Content>
