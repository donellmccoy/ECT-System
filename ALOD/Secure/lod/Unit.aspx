<%@ Page Language="VB" MasterPageFile="~/Secure/Lod/LOD.master" AutoEventWireup="false" Inherits="ALOD.Web.LOD.Secure_lod_c2_Unit" Title="Untitled Page" Codebehind="Unit.aspx.vb" MaintainScrollPositionOnPostback="true"%>

<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/Lod/LOD.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck"
    TagPrefix="uc1" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentNested" runat="Server">
    <asp:Panel ID="OriginalLOD" Visible="false" runat="server">
        <div class="formHeader">
            1 - Unit Section
        </div>
        <table class="dataTable">
            <tr>
                <td class="number">
                    A
                </td>
                <td class="label">
                    Was member activated:
                </td>
                <td class="value">
                    <asp:RadioButtonList ID="rblcmdractivated" runat="server" RepeatDirection="Horizontal"
                        RepeatLayout="Flow">
                        <asp:ListItem Value="Y">Yes</asp:ListItem>
                        <asp:ListItem Value="N">No</asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:Label ID="lblcmdractivated" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="number">
                    B
                </td>
                <td class="label  labelRequired">
                    *Duty Status:
                </td>
                <td class="value">
                    <asp:DropDownList ID="DutyStatusSelect" runat="Server" DataTextField="text" DataValueField="value">
                        <asp:ListItem Value="active">Active Duty Status</asp:ListItem>
                        <asp:ListItem Value="uta">UTA</asp:ListItem>
                        <asp:ListItem Value="aftp">AFTP</asp:ListItem>
                        <asp:ListItem Value="snr">Saturday Night Rule</asp:ListItem>
                        <asp:ListItem Value="travel">Travel to/from Duty</asp:ListItem>
                        <asp:ListItem Value="use">Unit Sponsored Event</asp:ListItem>
                        <asp:ListItem Value="other">Other (provide details below)</asp:ListItem>
                    </asp:DropDownList>
                    <asp:Label ID="DutyStatusLabel" runat="server" CssClass="lblDisableText"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="number">
                    C
                </td>
                <td class="label">
                    Start Date:
                </td>
                <td class="value">
                    <asp:TextBox ID="AbsenceFromBox" MaxLength="10" onchange="DateCheck(this);" runat="server"
                        Width="80"></asp:TextBox>
                    <asp:TextBox ID="AbsenceHourFromBox" MaxLength="4" onchange="TimeCheck(this);" runat="server"
                        Width="40"></asp:TextBox>
                    <asp:Label ID="AbsenceFromLabel" runat="server" CssClass="lblDisableText"></asp:Label>
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
                    <asp:TextBox ID="AbsenceToBox" MaxLength="10" onchange="DateCheck(this);" runat="server"
                        Width="80"></asp:TextBox>
                    <asp:TextBox ID="AbsenceHourToBox" MaxLength="4" onchange="TimeCheck(this);" runat="server"
                        Width="40"></asp:TextBox>
                    <asp:Label ID="AbsenceToLabel" runat="server" CssClass="lblDisableText"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="number">
                    E
                </td>
                <td class="label">
                    Other:
                </td>
                <td class="value">
                    <asp:TextBox ID="OtherDutyBox" runat="server" Width="359px" MaxLength="50"></asp:TextBox>
                    <asp:Label ID="OtherDutyLabel" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="number">
                    F
                </td>
                <td class="label" style="height: 100px;">
                    <span class="labelRequired">*Details Of Accident:</span><br />
                    (Who, What, Where, When)
                </td>
                <td class="value">
                    <asp:TextBox ID="DetailsBox" runat="Server" MaxLength="730" Rows="10" TextMode="MultiLine"
                        Width="500px"></asp:TextBox>
                    <asp:Label ID="DetailsLabel" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="number">
                    G
                </td>
                <td class="label">
                    <span class="labelRequired">*Recommended Finding:</span>
                </td>
                <td class="value">
                    <asp:Panel runat="server" ID="unitFindings">
                        <asp:RadioButtonList ID="rblInLOD" runat="server" RepeatLayout="Flow">
<%--                            <asp:ListItem Value="1">In Line Of Duty (ILOD)</asp:ListItem>
                            <asp:ListItem Value="2">EPTS  - LOD Not Applicable</asp:ListItem>
                            <asp:ListItem Value="3">EPTS - Service aggravated</asp:ListItem>
                            <asp:ListItem Value="4">Not ILOD - Due To Own Misconduct</asp:ListItem>
                            <asp:ListItem Value="5">Not ILOD - Not Due To Own Misconduct</asp:ListItem>
                            <asp:ListItem Value="6">Recommend Formal LOD Investigation</asp:ListItem>--%>
                        </asp:RadioButtonList>
                    </asp:Panel>
                    <asp:Label ID="InLodLabel" runat="server"></asp:Label>
                </td>
            </tr>
        </table>
    </asp:Panel>

    <asp:Panel ID="LOD_v2" Visible="false" runat="server" CssClass="dataBlock">
        <div class="dataBlock-header">
            1 - Unit Section
        </div>
        <div class="dataBlock-body">
            <table class="dataTable">
                <tr>
                    <td class="number">
                        A
                    </td>
                    <td class="label">
                        Was member activated:
                    </td>
                    <td class="value">
                        <asp:RadioButtonList ID="rblcmdractivated_v2" runat="server" RepeatDirection="Horizontal"
                            RepeatLayout="Flow">
                            <asp:ListItem Value="N">No</asp:ListItem>
                            <asp:ListItem Value="Y">Yes</asp:ListItem>
                        </asp:RadioButtonList>
                        <asp:Label ID="lblcmdractivated_v2" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        B
                    </td>
                    <td class="label  labelRequired">
                        *Duty Status:
                    </td>
                    <td class="value">
                        <asp:DropDownList ID="DutyStatusSelect_v2" runat="Server" DataTextField="text" DataValueField="value" AutoPostBack="true"></asp:DropDownList>
                        <asp:Label ID="DutyStatusLabel_v2" runat="server" CssClass="lblDisableText"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        C
                    </td>
                    <td class="label">
                        Other:
                    </td>
                    <td class="value">
                        <asp:TextBox ID="OtherDutyBox_v2" MaxLength="50" runat="server"
                            Width="359px"></asp:TextBox>
                        <asp:Label ID="OtherDutyLabel_v2" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="number">D </td>
                    <td class="label">Start Date: </td>
                    <td class="value">
                        <asp:TextBox ID="DutyStatusFromDate_v2" runat="server" MaxLength="10" onchange="DateCheck(this);" Width="80"></asp:TextBox>
                        <asp:TextBox ID="DutyStatusFromTime_v2" runat="server" MaxLength="4" onchange="TimeCheck(this);" Width="40"></asp:TextBox>
                        <asp:Label ID="DutyStatusFromLabel_v2" runat="server" CssClass="lblDisableText"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        E
                    </td>
                    <td class="label">
                        End Date:
                    </td>
                    <td class="value">
                        <asp:TextBox ID="DutyStatusToDate_v2" MaxLength="10" onchange="DateCheck(this);" runat="server"
                            Width="80"></asp:TextBox>
                        <asp:TextBox ID="DutyStatusToTime_v2" MaxLength="4" onchange="TimeCheck(this);" runat="server"
                            Width="40"></asp:TextBox>
                        <asp:Label ID="DutyStatusToLabel_v2" runat="server" CssClass="lblDisableText"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        F
                    </td>
                    <td class="label">
                        Add Sources of Information:
                    </td>
                    <td class="value">
                        <asp:DropDownList ID="InfoSourceSelect_v2" runat="Server" DataTextField="text" DataValueField="value" AutoPostBack="true"></asp:DropDownList>
                        <asp:Label ID="InfoSourcelbl_v2" runat="server" CssClass="lblDisableText"></asp:Label>

                        <div id="InfoSourceOther_v2">

                            <br />

                            <asp:TextBox ID="InfoSourceOtherTextBox_v2" runat="Server" MaxLength="50" Width="250px"></asp:TextBox>
                            <asp:Label ID="InfoSourceOtherlbl_v2" runat="server"></asp:Label>
                     
                        </div>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        G
                    </td>
                    <td class="label">
                        name(s), address(es), and phone number(s) of witness(es):
                    </td>
                    <td class="value">
                        <%--<asp:TextBox ID="WitnessInfoTextBox_v2" runat="Server" MaxLength="730" Rows="10" TextMode="MultiLine"
                            Width="500px"></asp:TextBox>
                        <asp:Label ID="WitnessInfolbl_v2" runat="server"></asp:Label>--%>
                        <asp:Label runat="server" ID="lblNoWitnesses" Text="No witnesses presented" Visible="false" />
                        <table>
                            <tr runat="server" id="trWitnessesHeader">
                                <td align="center">
                                    Name
                                </td>
                                <td align="center">
                                    Address
                                </td>
                                <td align="center">
                                    Phone Number
                                </td>
                            </tr>
                            <tr id="witness1" runat="server">
                                <td>
                                    1.
                                    <asp:TextBox ID="WitnessName1" runat="server" Width="200" MaxLength="50"></asp:TextBox>
                                    <asp:Label ID="WitnessName1lbl" runat="server"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="WitnessAddress1" runat="server" Width="200" MaxLength="50"></asp:TextBox>
                                    <asp:Label ID="WitnessAddress1lbl" runat="server"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="WitnessPhoneNumber1" runat="server" Width="200" MaxLength="50"></asp:TextBox>
                                    <asp:Label ID="WitnessPhoneNumber1lbl" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr id="witness2" runat="server">
                                <td>
                                    2.
                                    <asp:TextBox ID="WitnessName2" runat="server" Width="200" MaxLength="50"></asp:TextBox>
                                    <asp:Label ID="WitnessName2lbl" runat="server"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="WitnessAddress2" runat="server" Width="200" MaxLength="50"></asp:TextBox>
                                    <asp:Label ID="WitnessAddress2lbl" runat="server"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="WitnessPhoneNumber2" runat="server" Width="200" MaxLength="50"></asp:TextBox>
                                    <asp:Label ID="WitnessPhoneNumber2lbl" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr id="witness3" runat="server">
                                <td>
                                    3.
                                    <asp:TextBox ID="WitnessName3" runat="server" Width="200" MaxLength="50"></asp:TextBox>
                                    <asp:Label ID="WitnessName3lbl" runat="server"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="WitnessAddress3" runat="server" Width="200" MaxLength="50"></asp:TextBox>
                                    <asp:Label ID="WitnessAddress3lbl" runat="server"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="WitnessPhoneNumber3" runat="server" Width="200" MaxLength="50"></asp:TextBox>
                                    <asp:Label ID="WitnessPhoneNumber3lbl" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr id="witness4" runat="server">
                                <td>
                                    4.
                                    <asp:TextBox ID="WitnessName4" runat="server" Width="200" MaxLength="50"></asp:TextBox>
                                    <asp:Label ID="WitnessName4lbl" runat="server"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="WitnessAddress4" runat="server" Width="200" MaxLength="50"></asp:TextBox>
                                    <asp:Label ID="WitnessAddress4lbl" runat="server"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="WitnessPhoneNumber4" runat="server" Width="200" MaxLength="50"></asp:TextBox>
                                    <asp:Label ID="WitnessPhoneNumber4lbl" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr id="witness5" runat="server">
                                <td>
                                    5.
                                    <asp:TextBox ID="WitnessName5" runat="server" Width="200" MaxLength="50"></asp:TextBox>
                                    <asp:Label ID="WitnessName5lbl" runat="server"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="WitnessAddress5" runat="server" Width="200" MaxLength="50"></asp:TextBox>
                                    <asp:Label ID="WitnessAddress5lbl" runat="server"></asp:Label>
                                </td>
                                <td>
                                    <asp:TextBox ID="WitnessPhoneNumber5" runat="server" Width="200" MaxLength="50"></asp:TextBox>
                                    <asp:Label ID="WitnessPhoneNumber5lbl" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        H
                    </td>
                    <td class="label  labelRequired">
                        *At the time of this occurrence, the member was:
                    </td>
                    <td class="value">
                        <asp:DropDownList ID="MemberOccurrenceSelect_v2" runat="Server" DataTextField="text" DataValueField="value" AutoPostBack="true"></asp:DropDownList>
                        <asp:Label ID="MemberOccurrencelbl_v2" runat="server" CssClass="lblDisableText"></asp:Label>

                        <div id="MemberOccurrence_v2" runat="server">
                            <br />

                            From:&nbsp; 
                            <asp:TextBox ID="AbsenceFromDate_v2" MaxLength="10" onchange="DateCheck(this);" runat="server"
                            Width="80"></asp:TextBox>
                            <asp:TextBox ID="AbsenceFromTime_v2" MaxLength="4" onchange="TimeCheck(this);" runat="server"
                            Width="40"></asp:TextBox>
                            <asp:Label ID="AbsenceFromlbl_v2" runat="server" CssClass="lblDisableText"></asp:Label>
                            <br />
                            To:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <asp:TextBox ID="AbsenceToDate_v2" MaxLength="10" onchange="DateCheck(this);" runat="server"
                            Width="80"></asp:TextBox>
                            <asp:TextBox ID="AbsenceToTime_v2" MaxLength="4" onchange="TimeCheck(this);" runat="server"
                            Width="40"></asp:TextBox>
                            <asp:Label ID="AbsenceTolbl_v2" runat="server" CssClass="lblDisableText"></asp:Label>
                        </div>

                        <div id="TravelOccurrence_v2" runat="server">
                            <br />

                            <asp:DropDownList ID="TravelOccurrenceSelect_v2" runat="Server" DataTextField="text" DataValueField="value" AutoPostBack="true"></asp:DropDownList>
                            <asp:Label ID="TravelOccurrencelbl_v2" runat="server" CssClass="lblDisableText"></asp:Label>
                        </div>

                    </td>
                </tr>
                <tr>
                    <td class="number">
                        I
                    </td>
                    <td class="label" style="height: 100px;">
                        <span class="labelRequired">*Details Of Accident:</span><br />
                        (Who, What, Where, When)
                    </td>
                    <td class="value">
                        <asp:TextBox ID="DetailsBox_v2" runat="Server" MaxLength="730" Rows="10" TextMode="MultiLine"
                            Width="500px"></asp:TextBox>
                        <asp:Label ID="DetailsLabel_v2" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        J
                    </td>
                    <td class="label">
                        <span class="labelRequired">*Does the member have 8 years of TAFMS service:</span>
                        <br /><br />
                        <span class="labelRequired">*Was the member on orders of 30 days or more:</span>
                    </td>
                    <td>
                    
                        <span class="normal">
                            <asp:Label runat="server" ID="lblCredibleService_v2" />
                            <asp:RadioButtonList ID="rblCredibleService_v2" CssClass="fieldNormal" runat="server" RepeatDirection="Horizontal">
                                <asp:ListItem Value="No">No</asp:ListItem>
                                <asp:ListItem Value="Yes">Yes</asp:ListItem>
                            </asp:RadioButtonList>

                            <br />
                            <br />

                            <asp:Label runat="server" ID="lblMemberOnOrders_v2" />
                            <asp:RadioButtonList ID="rblMemberOnOrders_v2" CssClass="fieldNormal" runat="server" RepeatDirection="Horizontal">
                                <asp:ListItem Value="No">No</asp:ListItem>
                                <asp:ListItem Value="Yes">Yes</asp:ListItem>
                            </asp:RadioButtonList>
                        </span>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        K
                    </td>
                    <td class="label labelRequired">
                        *Proximate cause of the member's death, injury, illness, or disease was:
                    </td>
                    <td class="value">
                    
                        <asp:DropDownList ID="proximateCause_v2" runat="Server" DataTextField="text" DataValueField="value" AutoPostBack="true"></asp:DropDownList>
                        <asp:Label ID="proximateCauselbl_v2" runat="server" CssClass="lblDisableText"></asp:Label>

                        <div id="freeTextCause_v2">

                            <br />

                            <asp:TextBox ID="otherCauseTextBox_v2" runat="Server" MaxLength="50" Rows="2" TextMode="MultiLine"
                                Width="250px"></asp:TextBox>
                            <asp:Label ID="otherCauseTextlbl_v2" runat="server"></asp:Label>
                     
                        </div>

                    </td>
                </tr>
                <tr>
                    <td class="number">
                        L
                    </td>
                    <td class="label">
                        <span class="labelRequired">*Recommended Finding:</span>
                    </td>
                    <td class="value">
                        <asp:Panel runat="server" ID="unitFindings_v2">
                            <asp:RadioButtonList ID="rblInLOD_v2" runat="server"
                                RepeatLayout="Flow">
                            </asp:RadioButtonList>
                        </asp:Panel>
                        <asp:Label ID="InLodLabel_v2" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        M
                    </td>
                    <td class="label">
                        <span class="labelRequired">*Member�s Signed Orders :</span>
                    </td>
                    <td class="value">
                    <asp:Panel runat="server" ID="MStatus">
                        <asp:CheckBox ID="MStatusCheckBox" runat="server" RepeatLayout="Flow"></asp:CheckBox>
                           I have provided substantiating documentation to verify the member�s status
                    </asp:Panel>
                    <asp:Panel runat="server" ID="MVStatusText" Visible="false">
                           Member�s status verified
                    </asp:Panel>
                        <asp:Panel runat="server" ID="MProof">
                        <asp:CheckBox ID="MProofCheckBox" runat="server" RepeatLayout="Flow"></asp:CheckBox>
                           I have verified the member�s proof of status
                            <asp:Label ID="SignOrderLabel" runat="server">

                            </asp:Label>
                        </asp:Panel>
                        <asp:Panel runat="server" ID="MVProofText" Visible="false">
                           Member�s proof of status verified
                    </asp:Panel>
                        
                </td>
                </tr>
                
                <tbody runat="server" id="DocumentationQuestions" visible="false">
                    <%--<tr>
                        <td class="number">N
                        </td>
                        <td class="label">
                            <span class="labelRequired">*Did member sign the Member LOD Initiation form certifying the information to be true:</span>
                        </td>
                        <td class="value">
                                    <asp:RadioButtonList ID="rblLODInitiation" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true" Enabled="false">
                                        <asp:ListItem Value="False">No</asp:ListItem>
                                        <asp:ListItem Value="True">Yes</asp:ListItem>
                                    </asp:RadioButtonList>
                                    <asp:Label ID="lblLODInitiation" runat="server" />
                        </td>
                    </tr>--%>
                    <%--<tr>
                        <td class="number">O
                        </td>
                        <td class="label">
                            <span class="labelRequired">*Is there a written diagnosis from a medical provider that supports the claimed condition:</span>
                        </td>
                        <td class="value">
                                    <asp:RadioButtonList ID="rblWrittenDiagnosis" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true" Enabled="false">
                                        <asp:ListItem Value="False">No</asp:ListItem>
                                        <asp:ListItem Value="True">Yes</asp:ListItem>
                                    </asp:RadioButtonList>
                                    <asp:Label ID="lblWrittenDiagnosis" runat="server" />
                        </td>
                    </tr>--%>
                   <%-- <tr>
                        <td class="number">P
                        </td>
                        <td class="label">
                            <span class="labelRequired">*Did the member request the LOD within 180 days of completing the qualified duty status:</span>
                        </td>
                        <td class="value">
                                    <asp:RadioButtonList ID="rblMemberRequest" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true" Enabled="false">
                                        <asp:ListItem Value="False">No</asp:ListItem>
                                        <asp:ListItem Value="True">Yes</asp:ListItem>
                                    </asp:RadioButtonList>
                                    <asp:Label ID="lblMemberRequest" runat="server" />
                        </td>
                    </tr>--%>
                    <tr>
    <td class="number">
        <!-- Q1.
        <br/>
        <br/>
        <br/>
        <br/>
        <br/>-->
        <!-- Q2.
        <br/>
        <br/>
        <br/>
        <br/>
        <br/>
        Q3.
        <br/>
        <br/>
        <br/>
        <br/>
        <br/>
        <br/>
        <br/>-->
        <!-- Q4a.
        <br/>
        <br/>
        <br/>
        <br/>
        <br/>
        Q4b.
        <br/>
        <br/>
        <br/>
        <br/>
        <br/>
        <br/>
        <br/>-->
        <!-- Q5.
        <br/>
        <br/>
        <br/>
        <br/>
        <br/>
        <br/>-->
        <!-- Q6.-->
        <!-- <br/>
        <br/>
        <br/>
        <br/>
        <br/>
        <br/>
        <br/>
        <br/>
        <br/>-->
        <!-- Q7.-->
        <br/>
        <br/>
        <br/>
        <br/>
        <!-- Q9.-->
    </td>
</tr>
         
                    <tr>


                        <td class="label">
                            <%--<span class="labelRequired">*Was the injury incurred or aggravated during a time period covered by the qualified duty status:</span>--%>
                            <%--<br/>
                            <br/>--%>
                            <%--<span class="labelRequired">*What status was this member in when injury or illness occurred (orders or UTA status):</span>--%>
                            <%--<br/>
                            <br/>
                            <span class="labelRequired">*If on orders - Are the orders (to include pre-certification, all modifications, and close-out if available) verified and attached to the Line of Duty for the member:</span>
                            <br/>
                            <br/>--%>
                            <%-- <span class="labelRequired">*For IDT status, is a certified or approved AF 40A or ANG equivalent attached to this Line of Duty:</span>
                            <br/>
                            <br/>
                            <span class="labelRequired">*If a certified 40A or ANG equivalent is not available, is a participation report, UTAPS export or participation history from UTAPS attached to this LOD:</span>
                            <br/>
                            <br/>
                            <span class="labelRequired">*Is the Point Credit Accounting and Reporting System (PCARS) Report attached for the member from the Military Personnel Data System (MilPDS):</span>
                            <br/>
                            <br/>--%>
                            <%--<span class="labelRequired">*Does the PCARS show/reflect the date of injury or illness in the history:</span>
                            <br />
                            <span class="labelRequired">Note: PCARS must reflect the date of injury or illness to accurately calculate Total Active Federal Military Service (TAFMS).</span>
                            <br />
                            <br />
                            <span class="labelRequired">*Does the member�s Total Active Federal Military Service (TAFMS) meet the Eight Year Rule (AFI 36-2910, para 1.10.2.2.2):</span>--%>

                        </td>
                    </tr>
                        <%--Q1--%>
                        <%--<td class="value">--%>
                                    <%--<asp:RadioButtonList ID="rblIncurredOrAggravated" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true" Enabled="false">
                                        <asp:ListItem Value="False">No</asp:ListItem>
                                        <asp:ListItem Value="True">Yes</asp:ListItem>
                                    </asp:RadioButtonList>
                                    <asp:Label ID="lblIncurredOrAggravated" runat="server" />
                            
                            <br/>
                            <br/>
                            <br/>
                            <br/>--%>
                        <%--Q2--%>
                            <%--<asp:RadioButtonList ID="rblStatusWhenOccured" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true" Enabled="false">
                                        <asp:ListItem Value="IDT">IDT</asp:ListItem>
                                        <asp:ListItem Value="MPA">MPA</asp:ListItem>
                                        <asp:ListItem Value="AT">AT</asp:ListItem>
                                        <asp:ListItem Value="RPA">RPA</asp:ListItem>
                                        <asp:ListItem Value="1610">1610</asp:ListItem>
                                        <asp:ListItem Value="Travel">Travel</asp:ListItem>
                                        <asp:ListItem Value="Other">Other:</asp:ListItem>
                                    </asp:RadioButtonList>
                            <asp:Label ID="lblStatusWhenOccured" runat="server" />
                            <br/>
                                    <asp:Label ID="txtStatusWhenOccured" runat="server" />
                                        <asp:TextBox ID="txtStatusWhenOccuredTextBox" runat="Server" Enabled="false" AutoPostBack="true" MaxLength="730" Rows="1"
                                        Width="500px"></asp:TextBox>
                                        <asp:Label ID="lblStatusWhenOccuredText" runat="server"></asp:Label>
                            <br/>
                            <br/>
                            <br/>
                            <br/>
                            <br/>--%>
                        <%--Q3--%>
                            <%--<asp:RadioButtonList ID="rblIfNoOrders" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true" Enabled="false">
                                        <asp:ListItem Value="No">No</asp:ListItem>
                                        <asp:ListItem Value="Yes">Yes</asp:ListItem>
                                        <asp:ListItem Value="N/A">N/A</asp:ListItem>
                                    </asp:RadioButtonList>
                                    <asp:Label ID="lblIfNoOrders" runat="server" />
                            <br/>
                            <br/>
                            <br/>
                            <br/>
                            <br/>
                            <br/>
                            <br/>--%>
                        <%--Q4a--%>
                            <%--<asp:RadioButtonList ID="rblIDTDocumentAttached" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true" Enabled="false">
                                        <asp:ListItem Value="No">No</asp:ListItem>
                                        <asp:ListItem Value="Yes">Yes</asp:ListItem>
                                        <asp:ListItem Value="N/A">N/A</asp:ListItem>
                                    </asp:RadioButtonList>
                                    <asp:Label ID="lblIDTDocumentAttached" runat="server" />
                            <br/>
                            <br/>
                            <br/>
                            <br/>
                            <br/>
                            <br/>--%>
                        <%--<%--Q4b--%>
                            <%--<asp:RadioButtonList ID="rblUTAPSAttached" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true" Enabled="false">
                                        <asp:ListItem Value="No">No</asp:ListItem>
                                        <asp:ListItem Value="Yes">Yes</asp:ListItem>
                                        <asp:ListItem Value="N/A">N/A</asp:ListItem>
                                    </asp:RadioButtonList>
                                    <asp:Label ID="lblUTAPSAttached" runat="server" />
                            <br/>
                            <br/>
                            <br/>
                            <br/>
                            <br/>
                            <br/>
                            --%>
                        <%--Q5--%>
                            <%--<asp:RadioButtonList ID="rblPCARSAttached" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true" Enabled="false">
                                        <asp:ListItem Value="False">No</asp:ListItem>
                                        <asp:ListItem Value="True">Yes</asp:ListItem>
                                    </asp:RadioButtonList>
                                    <asp:Label ID="lblPCARSAttached" runat="server" />
                           <%-- <br/>
                            <br/>
                            <br/>
                            <br/>--%>
                            <%--<br/>
                            <br/>--%>
                        <%--Q6--%>
                            <%--<asp:RadioButtonList ID="rblPCARSHistory" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true" Enabled="false">
                                        <asp:ListItem Value="False">No</asp:ListItem>
                                        <asp:ListItem Value="True">Yes</asp:ListItem>
                                    </asp:RadioButtonList>
                                    <asp:Label ID="lblPCARSHistory" runat="server" />
                            <br/>
                            <br/>
                            <br/>
                            <br/>
                            <br/>
                            <br/>
                            <br/>--%>
                        <%--Q7--%>
                            <%--<asp:RadioButtonList ID="rblEightYearRule" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true" Enabled="false">
                                        <asp:ListItem Value="False">No</asp:ListItem>
                                        <asp:ListItem Value="True">Yes</asp:ListItem>
                                    </asp:RadioButtonList>
                                    <asp:Label ID="lblEightYearRule" runat="server" />
                            
                        </td>
                    </tr>--%>
                   <%-- <tr>
                        <td class="number">R
                        </td>
                        <td class="label">
                            <span class="labelRequired">*Is there medical evidence the illness or disease existed prior to the qualified duty status time period:</span>
                        </td>
                        <td class="value">
                                    <asp:RadioButtonList ID="rblPriorToDutytatus" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true" Enabled="false">
                                        <asp:ListItem Value="False">No</asp:ListItem>
                                        <asp:ListItem Value="True">Yes</asp:ListItem>
                                    </asp:RadioButtonList>
                                    <asp:Label ID="lblPriorToDutytatus" runat="server" />
                        </td>
                    </tr>--%>
                    <%--<tr>
                        <td class="number">S
                        </td>
                        <td class="label">
                            <span class="labelRequired">*Is there medical evidence that activities during the qualified duty status worsened the pre-service condition beyond its natural progression:</span>
                        </td>
                        <td class="value">
                                    <asp:RadioButtonList ID="rblStatusWorsened" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" Enabled="false">
                                        <asp:ListItem Value="No">No</asp:ListItem>
                                        <asp:ListItem Value="Yes">Yes</asp:ListItem>
                                        <asp:ListItem Value="N/A" Enabled="false">N/A</asp:ListItem>
                                    </asp:RadioButtonList>
                                    <asp:Label ID="lblStatusWorsened" runat="server" />
                        </td>
                    </tr>--%>
                        </tbody>
            </table>
        </div>
    </asp:Panel>
        <br />
    <uc1:SignatureCheck ID="SigCheck" runat="server" Template="Form348Unit" CssClass="sigcheck-form" />
</asp:Content>
<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="FooterNested">

    <script type="text/javascript">

        $(function () {

            $('.datePicker').datepicker(calendarPick("Past", "<%=CalendarImage %>"));

            $('.datePickerFuture').datepicker(calendarPick("All", "<%=CalendarImage %>"));

            CheckDutyStatus();
        });

        //Checks if the set of fields which are required have data.  
        //function validateData() {
        //    return true;
        //}

        function ClientValidateData() {

            var isValid = true;
            $('input[type=text].fieldRequired').each(function(n, element) {

                var val = $(element).val().replace(/^\s+|\s+$/, '');
                if (val.length == 0) {

                    $(element).addClass("fieldInvalid");
                    isValid = false;
                }
            });

            $('select.fieldRequired').each(function(n, element) {

                if ($(element).val() === "0") {
                    $(element).addClass("fieldInvalid");
                    isValid = false;
                }
            });

            if (!IsChecked(radioElement('rblInLOD'))) {
                $(element('rblInLOD')).addClass("fieldInvalid");
                isValid = false;
            }

            return isValid;
        }

        //Checks If the duty status is  Present on Duty--1.If Yes then the Absence dates are disabled
        function CheckDutyStatus() {
            var condition;
            var classDisable;

            if ($(element('DutyStatusSelect')).attr("class") == "fieldDisabled") {
                return;
            }

            if ($(element('DutyStatusSelect')).val() != "active") {
                condition = true;
                classDisable = 'disable';
                $(element('AbsenceFromBox')).val("");
                $(element('AbsenceHourFromBox')).val("");
                $(element('AbsenceToBox')).val("");
                $(element('AbsenceHourToBox')).val("");

            }
            else {
                condition = false;
                classDisable = 'enable';

            }

            $(element('AbsenceFromBox')).datepicker(classDisable)
            $(element('AbsenceHourFromBox')).datepicker(classDisable)
            $(element('AbsenceToBox')).datepicker(classDisable)
            $(element('AbsenceHourToBox')).datepicker(classDisable)

            $(element('AbsenceFromBox')).attr("disabled", condition);
            $(element('AbsenceHourFromBox')).attr("disabled", condition);

            $(element('AbsenceToBox')).attr("disabled", condition);
            $(element('AbsenceHourToBox')).attr("disabled", condition);

            if ($(element('DutyStatusSelect')).val() != "other") {

                condition = true;
                classDisable = "fieldDisabled";
                $(element('OtherDutyBox')).val("");
            }
            else {

                condition = false;
                classDisable = "";
            }


            $(element('OtherDutyBox')).attr("disabled", condition);
            $(element('OtherDutyBox')).attr("class", classDisable);

            if (($(element('DutyStatusSelect')).val() == "other") && ($(element('OtherDutyBox')).val().length == 0)) {

                $(element('OtherDutyBox')).attr("class", "fieldRequired");

            }

        }	
        
    </script>

</asp:Content>
