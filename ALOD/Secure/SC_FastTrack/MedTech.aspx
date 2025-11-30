<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SC_FastTrack/SC_FastTrack.master"
    MaintainScrollPositionOnPostback="true" AutoEventWireup="false"
    Inherits="ALOD.Web.Special_Case.IRILO.Secure_sc_FT_MedTech" EnableEventValidation="false" Codebehind="MedTech.aspx.vb" %>

<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Register Src="~/secure/Shared/UserControls/FindingsControl.ascx" TagName="Findings"
    TagPrefix="uc" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_FastTrack/SC_FastTrack.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck"
    TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/ICDCodeControl.ascx" TagName="ICDCodeControl" TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/ICD7thCharacterControl.ascx" TagName="ICD7thCharacterControl" TagPrefix="uc1" %>


<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">

    <div class="dataBlock">
        <div class="dataBlock-header">
            Point of Contact Info
        </div>
        <table>
            <tr>
                <td>POC Name:</td>
                <td><asp:TextBox ID="txtPOCNameLabel" MaxLength="180" runat="server" Width="400"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>POC DSN/Phone:</td>
                <td><asp:TextBox ID="txtPOCPhoneLabel" MaxLength="180" runat="server" Width="400"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>POC DSN/Phone:</td>
                <td><asp:TextBox ID="txtPOCEmailLabel" MaxLength="180" runat="server" Width="400"></asp:TextBox>
                </td>
            </tr>
        </table>


    </div>
    <div id="MemberInfo" runat="server" class="dataBlock">
        
        <div class="dataBlock-header">
            IRILO - Patient/Case Info
        </div>
        <br />
        <table style="border-spacing: 5px">
            <tr>
                <td>
                    <asp:Label runat="server" CssClass="label labelRequired" ID="Label1"
                        LabelFor="TMTReceiveDate">IRILO Date: </asp:Label>
                </td>
                <td>
                    <asp:TextBox runat="server" ID="TMTReceiveDate" MaxLength="10" onchange="DateCheck(this);"
                        Text="" />
                </td>
            </tr>

            <tr>
                <td><asp:Label runat="server" CssClass="label labelRequired" ID="Label2"
                        LabelFor="ddlMedGroups">Med Group Name: </asp:Label></td>
                <td><asp:DropDownList runat="server" ID="ddlMedGroups"></asp:DropDownList></td>
            </tr>

            <tr>
                <td><asp:Label runat="server" CssClass="label labelRequired" ID="Label34"
                    LabelFor="ddlRMUs">RMU Name: </asp:Label></td>
                <td><asp:DropDownList runat="server" ID="ddlRMUs"></asp:DropDownList></td>
            </tr>

            <tr>
                <td>
                    <asp:Label runat="server" CssClass="label labelRequired">*Diagnosis:</asp:Label>
                </td>
                <td class="value">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <uc1:ICDCodeControl runat="server" ID="ucICDCodeControl" />                     
                             <br />
                              * - Changing this selection requires moving to a new tab and returning to synchronize data
                        </ContentTemplate>
<%--                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlICDHeading" EventName="SelectedIndexChanged" />
                        </Triggers>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="ddlICDCategory" EventName="SelectedIndexChanged" />
                        </Triggers>--%>

                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server">7th Character:</asp:Label>
                </td>
                <td class="value">
                    <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                        <ContentTemplate>
                            <uc1:ICD7thCharacterControl runat="server" ID="ucICD7thCharacterControl" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label ID="Label55" runat="server">Diagnosis Text:</asp:Label>
                </td>
                <td class="value">
                    <asp:Label ID="DiagnosisLabel" runat="server" CssClass="lblDisableText"></asp:Label>
                    <asp:TextBox ID="DiagnosisTextBox" Width="500px" TextMode="MultiLine" runat="server"
                        Rows="4" MaxLength="250"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label3">Patient Name: </asp:Label>
                </td>
                <td>
                    <asp:TextBox Enabled="false" runat="server" ID="MemberName" Text=""></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label4">Rank: </asp:Label>
                </td>
                <td>
                    <asp:TextBox runat="server" Enabled="false" ID="MemberRank" Text=""></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label5">SSN: </asp:Label>
                </td>
                <td>
                    <asp:TextBox runat="server" Enabled="false" ID="ProtectedSSN" Text=""></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label CssClass="label" runat="server" ID="Label6"
                        LabelFor="DAFSC">DAFSC: </asp:Label>
                </td>
                <td>
                    <asp:TextBox runat="server" Enabled="false" ID="DAFSC" Text=""></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label7" LabelFor="YearsSatisfactoryService">Years Satisfactory Service: </asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="YearsSatisfactoryService" runat="server" AutoPostBack="True">
                        <asp:ListItem Value="0" Text="--- Select One ---" Selected="True"></asp:ListItem>
                        <asp:ListItem Value="1" Text="Less Than 5 Years"></asp:ListItem>
                        <asp:ListItem Value="2" Text="More than 5 years"></asp:ListItem>
                        <asp:ListItem Value="3" Text="Less than/equal 15 years"></asp:ListItem>
                        <asp:ListItem Value="4" Text="More than/equal 15 years"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label33" LabelFor="BodyMassIndex">BMI: </asp:Label>
                </td>
                <td>
                    <asp:TextBox runat="server" Enabled="false" ID="BodyMassIndex" Text=""
                        class="numericOnly"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
    </div>

    <asp:UpdatePanel ID="UpdatePanel2" runat="server" ChildrenAsTriggers="False" UpdateMode="Conditional">
        <ContentTemplate>

    <div id="FastTrackAll" runat="server" class="dataBlock">
        <div class="dataBlock-header">
            <asp:Label ID="lblFastTrackAll" runat="server"></asp:Label>
        </div>
        <br />
        <table style="border-spacing: 5px">
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label8" CssClass="label labelRequired">Missed Work Days (in the last year - due to condition): </asp:Label>
                </td>
                <td style="width:60%;">
                    <asp:DropDownList runat="server" ID="MissedWorkDays">
                        <asp:ListItem Value="0" Text="--- Select One ---" Selected="True"></asp:ListItem>
                        <asp:ListItem Value="1" Text="None"></asp:ListItem>
                        <asp:ListItem Value="2" Text="1 or 2"></asp:ListItem>
                        <asp:ListItem Value="3" Text="3 or 4"></asp:ListItem>
                        <asp:ListItem Value="4" Text="More than 4"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label9" CssClass="label labelRequired">Specialist required for Management: </asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="SpecialistRequired" runat="server">
                        <asp:ListItem Value="0" Text="--- Select One ---" Selected="True"></asp:ListItem>
                        <asp:ListItem Value="1" Text="No"></asp:ListItem>
                        <asp:ListItem Value="2" Text="Once Per Year"></asp:ListItem>
                        <asp:ListItem Value="3" Text="Twice Per Year"></asp:ListItem>
                        <asp:ListItem Value="4" Text="Thrice Per Year"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label10" CssClass="label labelRequired">ER or Urgent Care Visits: </asp:Label>
                </td>
                <td>
                    <asp:RadioButton ID="ERorUrgentCareY" runat="server" AutoPostBack="True" GroupName="ERorUrgentCare"
                        Text="Yes" />&nbsp;&nbsp;&nbsp;<asp:RadioButton ID="ERorUrgentCareN" runat="server" AutoPostBack="True"
                            GroupName="ERorUrgentCare" Text="No" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label11">ER or Urgent Care Visit (details): </asp:Label>
                </td>
                <td>
                    <asp:TextBox runat="server" Enabled="false" ID="ERorUrgentCareDetails"
                        Rows="5" TextMode="MultiLine" Width="80%" MaxLength="250"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label12" CssClass="label labelRequired">Hospitalizations: </asp:Label>
                </td>
                <td>
                    <asp:RadioButton ID="HospitalizationsY" runat="server" AutoPostBack="True" GroupName="Hospitalizations"
                        Text="Yes" />&nbsp;&nbsp;&nbsp;<asp:RadioButton ID="HospitalizationsN" runat="server" AutoPostBack="True"
                            GroupName="Hospitalizations" Text="No" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label13">Hospitalizations (details): </asp:Label>
                </td>
                <td>
                    <asp:TextBox runat="server" Enabled="false" ID="HospitalizationDetails"
                        Text="" Rows="5" TextMode="MultiLine" Width="80%" MaxLength="250"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label14" CssClass="label labelRequired">Risk For Sudden Incapacitation: </asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="IncapacitationRisk" runat="server">
                        <asp:ListItem Value="0" Text="--- Select One ---" Selected="True"></asp:ListItem>
                        <asp:ListItem Value="1" Text="Low"></asp:ListItem>
                        <asp:ListItem Value="2" Text="Moderate"></asp:ListItem>
                        <asp:ListItem Value="3" Text="High"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label15" CssClass="label labelRequired">Recommended Follow Up Interval: </asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="FollowUpInterval" runat="server">
                        <asp:ListItem Value="0" Text="--- Select One ---" Selected="True"></asp:ListItem>
                        <asp:ListItem Value="1" Text="3 Months"></asp:ListItem>
                        <asp:ListItem Value="2" Text="4 Months"></asp:ListItem>
                        <asp:ListItem Value="3" Text="6 Months"></asp:ListItem>
                        <asp:ListItem Value="4" Text="Annual"></asp:ListItem>
                        <asp:ListItem Value="5" Text="Biennial"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label CssClass="label labelRequired" runat="server" ID="Label17">DAWG Recommendation: </asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="DAWGRecommendation" runat="server">
                        <asp:ListItem Value="0" Text="--- Select One ---" Selected="True"></asp:ListItem>
                        <asp:ListItem Value="1" Text="RTD"></asp:ListItem>
                        <asp:ListItem Value="2" Text="C-1"></asp:ListItem>
                        <asp:ListItem Value="3" Text="C-2"></asp:ListItem>
                        <asp:ListItem Value="4" Text="C-3"></asp:ListItem>
                        <asp:ListItem Value="5" Text="Full WWD"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
        </table>
        <br />
    </div>




    <div id="MostCases" runat="server" class="dataBlock">
        <div class="dataBlock-header">
            <asp:Label ID="lblMostCases" runat="server"></asp:Label>
        </div>
        <br />
        <table style="border-spacing: 5px">
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label18">Dx interferes with in-garrison duties: </asp:Label>
                </td>
                <td>
                    <asp:RadioButton ID="DutyInterferenceY" runat="server" GroupName="DutyInterference"
                        Text="Yes" AutoPostBack="False" />&nbsp;&nbsp;&nbsp;<asp:RadioButton ID="DutyInterferenceN"
                            runat="server" GroupName="DutyInterference" Text="No" AutoPostBack="False" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label21">Prognosis: </asp:Label>
                </td>
                <td>
                    <asp:TextBox runat="server" Enabled="false" ID="Prognosis" Text="" Rows="5"
                        TextMode="MultiLine" Width="450" MaxLength="250"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label35">Treatment: </asp:Label>
                </td>
                <td>
                    <asp:TextBox runat="server" Enabled="false" ID="Treatment" Text="" Rows="5"
                        TextMode="MultiLine" Width="450" MaxLength="250"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label19">Medications/Dosage: </asp:Label>
                </td>
                <td>
                    <asp:TextBox runat="server" Enabled="false" ID="MedicationDosages" Text=""
                        Rows="5" TextMode="MultiLine" Width="450" MaxLength="250"></asp:TextBox>
                </td>
            </tr>
        </table>
        <br />
    </div>
    <div id="SleepApnea" runat="server" class="dataBlock">
        <div class="dataBlock-header">
            <asp:Label ID="lblSleepApnea" runat="server"></asp:Label>
        </div>
        <br />
        <table style="border-spacing: 5px">
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label22">Daytime Somnolence: </asp:Label>
                </td>
                <td style="width:60%;">
                    <asp:RadioButton ID="DaytimeSomnolenceY" runat="server" GroupName="DaytimeSomnolence"
                        Text="Yes" />&nbsp;&nbsp;&nbsp;<asp:RadioButton ID="DaytimeSomnolenceN" runat="server"
                            GroupName="DaytimeSomnolence" Text="No" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label23">Daytime Somnolence (details): </asp:Label>
                </td>
                <td>
                    <asp:TextBox runat="server" Enabled="false" ID="DaytimeSomnolenceDetails"
                        Text="" Rows="5" TextMode="MultiLine" Width="80%" MaxLength="250"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label20">Apnea Episodes: </asp:Label>
                </td>
                <td>
                    <asp:RadioButton ID="ApneaEpisodesY" runat="server" GroupName="ApneaEpisodes"
                        Text="Yes" />&nbsp;&nbsp;&nbsp;<asp:RadioButton ID="ApneaEpisodesN" runat="server"
                            GroupName="ApneaEpisodes" Text="No" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label24">Apnea Episodes (details): </asp:Label>
                </td>
                <td>
                    <asp:TextBox runat="server" Enabled="false" ID="ApneaEpisodeDetails"
                        Text="" Rows="5" TextMode="MultiLine" Width="80%" MaxLength="250"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label36">Sleep Study Results (consistent with OSA): </asp:Label>
                </td>
                <td>
                    <asp:RadioButton ID="SleepStudyY" runat="server" GroupName="SleepStudy"
                        Text="Yes" />&nbsp;&nbsp;&nbsp;<asp:RadioButton ID="SleepStudyN" runat="server" GroupName="SleepStudy"
                            Text="No" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label37">Oral Devices Used: </asp:Label>
                </td>
                <td>
                    <asp:RadioButton ID="OralDevicesY" runat="server" GroupName="OralDevices"
                        Text="Yes" />&nbsp;&nbsp;&nbsp;<asp:RadioButton ID="OralDevicesN" runat="server"
                            GroupName="OralDevices" Text="No" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label38">CPAP Required: </asp:Label>
                </td>
                <td>
                    <asp:RadioButton ID="CPAPRequiredY" runat="server" GroupName="CPAPRequired"
                        Text="Yes" />&nbsp;&nbsp;&nbsp;<asp:RadioButton ID="CPAPRequiredN" runat="server"
                            GroupName="CPAPRequired" Text="No" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label39">BIPAP Required: </asp:Label>
                </td>
                <td>
                    <asp:RadioButton ID="BIPAPRequiredY" runat="server" GroupName="BIPAPRequired"
                        Text="Yes" AutoPostBack="True" />&nbsp;&nbsp;&nbsp;
                    <asp:RadioButton ID="BIPAPRequiredN"
                            runat="server" GroupName="BIPAPRequired" Text="No" AutoPostBack="True" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label40">System Response to Oral Devices/CPAP: </asp:Label>
                </td>
                <td>
                    <asp:RadioButton ID="SystemResponseY" runat="server" GroupName="SystemResponse"
                        Text="Controlled" />&nbsp;&nbsp;&nbsp;<asp:RadioButton ID="SystemResponseN" runat="server"
                            GroupName="SystemResponse" Text="Uncontrolled" />
                </td>
            </tr>
        </table>
        <br />
    </div>
    <div id="Diabetes" runat="server" class="dataBlock">
        <div class="dataBlock-header">
            <asp:Label ID="lblDiabetes" runat="server"></asp:Label>
        </div>
        <br />
        <table style="border-spacing: 5px">
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label41">Fasting Blood Sugar (mg/DL): </asp:Label>
                </td>
                <td style="width:50%;">
                    <asp:DropDownList ID="FastingBloodSugar" runat="server">
                        <asp:ListItem Value="0" Text="--- Select One ---" Selected="True"></asp:ListItem>
                        <asp:ListItem Value="1" Text="125-150"></asp:ListItem>
                        <asp:ListItem Value="2" Text="150-200"></asp:ListItem>
                        <asp:ListItem Value="3" Text="> 200"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label42">HgbA1C: </asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="HgbA1C" runat="server">
                        <asp:ListItem Value="0" Text="--- Select One ---" Selected="True"></asp:ListItem>
                        <asp:ListItem Value="1" Text="< 7.1"></asp:ListItem>
                        <asp:ListItem Value="2" Text="7.1 - 7.9"></asp:ListItem>
                        <asp:ListItem Value="3" Text="> 7.9"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label25">Current Optometry Exam: </asp:Label>
                </td>
                <td>
                    <asp:RadioButton ID="OptometryExamY" runat="server" GroupName="OptometryExam"
                        Text="Yes (see attached)" />&nbsp;&nbsp;&nbsp;<asp:RadioButton ID="OptometryExamN"
                            runat="server" GroupName="OptometryExam" Text="No" />
                </td>
            </tr>
            <tr>
                <td>
                    <table>
                        <tr>
                            <td>
                                &nbsp;<asp:Label runat="server" ID="Label26">Other Significant Medical Conditions </asp:Label>
                            </td>
                        </tr>
                    </table>
                    <table>
                        <tr>
                            <td>
                                &nbsp;<asp:Label runat="server" ID="Label54">(including end organ damage/cardiovascular risk factors): </asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
                <td>
                    <table>
                        <tr>
                            <td>
                            </td>
                        </tr>
                    </table>
                    <table>
                        <tr>
                            <td>
                                <asp:RadioButton ID="OtherConditionsY" runat="server" GroupName="OtherConditions"
                                    Text="Yes" />&nbsp;&nbsp;&nbsp;<asp:RadioButton ID="OtherConditionsN" runat="server"
                                        GroupName="OtherConditions" Text="No" />
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label27">Other Significant Medical Conditions (List): </asp:Label>
                </td>
                <td>
                    <asp:TextBox runat="server" Enabled="false" ID="OtherConditionsList"
                        Text="" Rows="5" TextMode="MultiLine" Width="80%" MaxLength="250"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label28">Controlled with Oral Agents: </asp:Label>
                </td>
                <td>
                    <asp:RadioButton ID="OralAgentsY" runat="server" GroupName="OralAgents"
                        Text="Yes" />&nbsp;&nbsp;&nbsp;<asp:RadioButton ID="OralAgentsN" runat="server" GroupName="OralAgents"
                            Text="No" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label43">Oral Agents (List): </asp:Label>
                </td>
                <td>
                    <asp:TextBox runat="server" Enabled="false" ID="OralAgentsList" Text=""
                        Rows="5" TextMode="MultiLine" Width="80%" MaxLength="250"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label44">Requires Insulin: </asp:Label>
                </td>
                <td>
                    <asp:RadioButton ID="RequiresInsulinY" runat="server" GroupName="RequiresInsulin"
                        Text="Yes" />&nbsp;&nbsp;&nbsp;<asp:RadioButton ID="RequiresInsulinN" runat="server"
                            GroupName="RequiresInsulin" Text="No" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label45">Insulin Dosage Regime: </asp:Label>
                </td>
                <td>
                    <asp:TextBox runat="server" Enabled="false" ID="InsulinDosageRegime"
                        Text="" Rows="5" TextMode="MultiLine" Width="80%" MaxLength="250"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label46">Requires Injectible Non-Insulin: </asp:Label>
                </td>
                <td>
                    <asp:RadioButton ID="RequiresNonInsulinY" runat="server" GroupName="RequiresNonInsulin"
                        Text="Yes" />&nbsp;&nbsp;&nbsp;<asp:RadioButton ID="RequiresNonInsulinN" runat="server"
                            GroupName="RequiresNonInsulin" Text="No" />
                </td>
            </tr>
        </table>
        <br />
    </div>
    <div id="Asthma" runat="server" class="dataBlock">
        <div class="dataBlock-header">
            <asp:Label ID="lblAsthma" runat="server"></asp:Label>
        </div>
        <br />
        <table style="border-spacing: 5px">
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label30">Pulmonary Function Test (PFT) consistent with Asthma: </asp:Label>
                </td>
                <td style="width:60%;">
                    <asp:RadioButton ID="PFTY" runat="server" GroupName="PFT" Text="Yes (see attached)" />&nbsp;&nbsp;&nbsp;<asp:RadioButton
                        ID="PFTN" runat="server" GroupName="PFT" Text="No" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label29">Methacholine Challenge consistent with Asthma: </asp:Label>
                </td>
                <td>
                    <asp:RadioButton ID="MethacholineChallengeY" runat="server" GroupName="MethacholineChallenge"
                        Text="Yes, c/w asthma" />&nbsp;&nbsp;&nbsp;<asp:RadioButton ID="MethacholineChallengeN"
                            runat="server" GroupName="MethacholineChallenge" Text="Yes, not c/w asthma" />&nbsp;&nbsp;&nbsp;<asp:RadioButton
                                ID="MethacholineChallengeP" runat="server" GroupName="MethacholineChallenge"
                                Text="No, not performed" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label31">Requires Daily Steroids: </asp:Label>
                </td>
                <td>
                    <asp:RadioButton ID="RequiresSteroidsY" runat="server" GroupName="RequiresSteroids"
                        Text="Yes" />&nbsp;&nbsp;&nbsp;<asp:RadioButton ID="RequiresSteroidsN" runat="server"
                            GroupName="RequiresSteroids" Text="No" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label32">Steroids Dosage: </asp:Label>
                </td>
                <td>
                    <asp:TextBox runat="server" Enabled="false" ID="DailySteroidsDosage"
                        Text="" CssClass="numericOnly"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label47">Frequency of Rescue Inhaler (e.g. Albuterol) use: </asp:Label>
                </td>
                <td>
                    <asp:DropDownList ID="FrequencyOfRescueInhalerUsage" runat="server">
                        <asp:ListItem Value="0" Text="--- Select One ---" Selected="True"></asp:ListItem>
                        <asp:ListItem Value="1" Text="Daily"></asp:ListItem>
                        <asp:ListItem Value="2" Text="Weekly"></asp:ListItem>
                        <asp:ListItem Value="3" Text="Monthly"></asp:ListItem>
                        <asp:ListItem Value="4" Text="Rarely"></asp:ListItem>
                        <asp:ListItem Value="5" Text="Never"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label48">Symptoms Exacerbated by Cold and/or Exercise: </asp:Label>
                </td>
                <td>
                    <asp:RadioButton ID="ColdExerciseExacerbationY" runat="server" GroupName="ColdExerciseExacerbation"
                        Text="Yes" />&nbsp;&nbsp;&nbsp;<asp:RadioButton ID="ColdExerciseExacerbationN" runat="server"
                            GroupName="ColdExerciseExacerbation" Text="No" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label49">Cold and/or Exercise Symptom Exacerbations: </asp:Label>
                </td>
                <td>
                    <asp:TextBox runat="server" Enabled="false" ID="ColdExerciseExacerbationDetails"
                        Text="" Rows="5" TextMode="MultiLine" Width="80%" MaxLength="250"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label51">Exacerbated Symptoms require Oral Steroids: </asp:Label>
                </td>
                <td>
                    <asp:RadioButton ID="ExacerbatedSymptomsOralSteroidsY" runat="server"
                        GroupName="ExacerbatedSymptomsOralSteroids" Text="Yes" />&nbsp;&nbsp;&nbsp;<asp:RadioButton
                            ID="ExacerbatedSymptomsOralSteroidsN" runat="server" GroupName="ExacerbatedSymptomsOralSteroids"
                            Text="No" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label52">Exacerbated Symptoms - Oral Steroids dosage: </asp:Label>
                </td>
                <td>
                    <asp:TextBox runat="server" Enabled="false" ID="ExacerbatedSymptomsOralSteroidsDosage"
                        Text="" Rows="5" TextMode="MultiLine" Width="80%" MaxLength="250"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label53">Normal PFT with Treatment (Oral/Inhaled Steroids): </asp:Label>
                </td>
                <td>
                    <asp:RadioButton ID="NormalPFTWithTreatmentY" runat="server" GroupName="NormalPFTWithTreatment"
                        Text="Yes" />&nbsp;&nbsp;&nbsp;<asp:RadioButton ID="NormalPFTWithTreatmentN" runat="server"
                            GroupName="NormalPFTWithTreatment" Text="No" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:Label runat="server" ID="Label50">H/O Intubation: </asp:Label>
                </td>
                <td>
                    <asp:RadioButton ID="HOIntubationY" runat="server" GroupName="HOIntubation"
                        Text="Yes" />&nbsp;&nbsp;&nbsp;<asp:RadioButton ID="HOIntubationN" runat="server"
                            GroupName="HOIntubation" Text="No" />
                </td>
            </tr>
        </table>
        <br />
    </div>
    </ContentTemplate>
<%--        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ddlICDContent" EventName="Init" />
        </Triggers>--%>
    </asp:UpdatePanel>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">
    <script type="text/javascript">

        $(function () {

            $('.datePicker').datepicker(calendarPick("Past", "<%=CalendarImage %>"));
            $('.datePickerAll').datepicker(calendarPick("All", "<%=CalendarImage %>"));

            $(".numericOnly").on('keydown', function (e) {
                var validKeys = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "Backspace", "Delete", "ArrowRight", "ArrowLeft", ".", "e", "E", "+", "-"];
                if (!validKeys.includes(e.key)) {
                    e.preventDefault();
                }
            });

        });

    </script>
</asp:Content>
