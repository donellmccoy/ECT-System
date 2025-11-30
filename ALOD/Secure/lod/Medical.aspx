<%@ Page Language="VB" MasterPageFile="~/Secure/Lod/LOD.master" AutoEventWireup="false" EnableViewState="True" EnableEventValidation="false" Inherits="ALOD.Web.LOD.Secure_lod_c2_Medical" Title="Untitled Page" CodeBehind="Medical.aspx.vb" MaintainScrollPositionOnPostback="true" %>
<%@ Import Namespace="ALODWebUtility.Common" %>
<%@ Import Namespace="ALOD.Core.Domain.Users" %>

<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/Lod/LOD.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck" TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/ICDCodeControl.ascx" TagName="ICDCodeControl" TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/ICD7thCharacterControl.ascx" TagName="ICD7thCharacterControl" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentNested" runat="Server">
    <asp:Panel runat="server" ID="OriginalLOD" CssClass="dataBlock" Visible="false">
        <div class="formHeader">
            1 - Medical Section
        </div>
        <table>
            <tr>
                <td class="number">A
                </td>
                <td class="label labelRequired">*Member status:
                </td>
                <td class="value">
                    <asp:DropDownList ID="MemberStatusSelect" runat="server" Width="137px">
                        <asp:ListItem Value="" Text="" />
                        <asp:ListItem Value="Active" Text="Active Duty" />
                        <asp:ListItem Value="Inactive" Text="Inactive duty" />
                        <asp:ListItem Value="AGR" Text="AGR" />
                        <asp:ListItem Value="ART" Text="ART" />
                        <asp:ListItem Value="TR" Text="TR" />
                        <asp:ListItem Value="IMA" Text="IMA" />
                    </asp:DropDownList>
                    <asp:Label ID="MemberStatusLabel" runat="server" />
                </td>
            </tr>

            <tr>
                <td class="number">B
                </td>
                <td class="label  labelRequired">*Diagnosis:
                </td>
                <td class="value">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <uc1:ICDCodeControl runat="server" ID="ucICDCodeControl" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>

            <tr>
                <td class="number">C
                </td>
                <td class="label">7th Character:
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
                <td class="number">D
                </td>
                <td class="label labelRequired">*Nature of Incident:
                </td>
                <td class="value">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <asp:DropDownList ID="IncidentTypeSelect" runat="server" Width="250px" />
                            <asp:Label ID="IncidentTypeLabel" runat="server" CssClass="lblDisableText" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>

            <tr>
                <td class="number">E
                </td>
                <td class="label labelRequired">*Diagnosis Text:
                </td>
                <td class="value">
                    <asp:Label ID="DiagnosisLabel" runat="server" CssClass="lblDisableText" />
                    <asp:TextBox ID="DiagnosisTextBox" Width="500px" TextMode="MultiLine" runat="server" Multiline="True" Rows="4" MaxLength="500" />
                </td>
            </tr>
            <tr>
                <td class="number">F
                </td>
                <td class="label labelRequired">*Type of Medical Facility:
                </td>
                <td class="value">
                    <asp:DropDownList ID="HospitalTypeSelect" runat="server" Width="250">
                        <asp:ListItem Text="" Value="" />
                        <asp:ListItem Text="Military" Value="Military" />
                        <asp:ListItem Text="Civilian" Value="Civilian" />
                    </asp:DropDownList>
                    <asp:Label ID="HospitalTypeLabel" runat="server" CssClass="lblDisableText" />
                </td>
            </tr>
            <tr>
                <td class="number">G
                </td>
                <td class="label labelRequired">*Name and Location of initial treating facility:
                </td>
                <td class="value">
                    <asp:TextBox ID="HospitalNameBox" runat="server" Width="244px" TextMode="MultiLine" Rows="3" MaxLength="200" />
                    <asp:Label ID="HospitalNameLabel" runat="server" CssClass="lblDisableText" />
                </td>
            </tr>
            <tr>
                <td class="number">H
                </td>
                <td class="label labelRequired">*Date and Time of Initial Treatment:
                </td>
                <td class="value">
                    <asp:TextBox ID="TreatmentDateBox" MaxLength="10" onchange="DateCheck(this);" runat="server" Width="80" />
                    <asp:TextBox ID="TreatmentHourBox" MaxLength="4" onchange="TimeCheck(this);" runat="server" Width="40" />
                    <asp:Label ID="TreatmentDateLabel" runat="server" CssClass="lblDisableText" />
                </td>
            </tr>
            <tr>
                <td class="number">I
                </td>
                <td class="label">
                    <span class="labelRequired">*Details of Accident or History of Disease:</span>
                    <br />
                    (how, where, when)
                </td>
                <td class="value">
                    <asp:TextBox ID="EventDetailsBox" runat="server" MaxLength="650" TextMode="MultiLine" Width="500px" Rows="5" />
                    <asp:Label ID="EventDetailsLabel" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="number">J
                </td>
                <td class="label">Does this case involve death:
                </td>
                <td class="value">
                    <asp:RadioButtonList ID="DeathCaseChoice" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                        <asp:ListItem>Yes</asp:ListItem>
                        <asp:ListItem>No</asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:Label ID="DeathCaseLabel" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="number">K
                </td>
                <td class="label">Does this case involve a Motor Vehicle Accident (MVA):
                </td>
                <td class="value">
                    <asp:RadioButtonList ID="MvaInvolvedChoice" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                        <asp:ListItem>Yes</asp:ListItem>
                        <asp:ListItem>No</asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:Label ID="MvaLabel" runat="server" />
                </td>
            </tr>
            <tr runat="server" id="EptsRow">
                <td class="number">L
                </td>
                <td class="label">EPTS:
                </td>
                <td class="value">
                    <asp:RadioButtonList ID="rblEPTS" runat="server" RepeatLayout="Flow">
                        <asp:ListItem Value="0">EPTS No</asp:ListItem>
                        <asp:ListItem Value="1">EPTS Yes: Service Aggravated</asp:ListItem>
                        <asp:ListItem Value="2">EPTS Yes: Not Service Aggravated</asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:Label ID="lblEPTS" runat="server" />
                </td>
            </tr>
            <tr runat="server" id="ApprovalCommentsRow">
                <td class="number">M
                </td>
                <td class="label">Local Medical Opinion:
                </td>
                <td class="value">
                    <asp:TextBox ID="txtApprovalComments" runat="server" TextMode="MultiLine" Width="500px" Rows="5" />
                    <asp:Label ID="lblApprovalComments" runat="server" />
                </td>
            </tr>
        </table>
    </asp:Panel>

    <asp:Panel runat="server" ID="LOD_v2" Visible="false">
        <asp:Panel runat="server" ID="MedTech_v2" CssClass="dataBlock">
            <div class="dataBlock-header">
                1 - Medical Technician
            </div>
            <div class="dataBlock-body">
                <table>
                    <tr>
                        <td class="number">A
                        </td>
                        <td class="label labelRequired">*Component:
                        </td>
                        <td class="value">
                            <asp:DropDownList ID="ComponentSelect_v2" runat="server" Width="137px" />
                            <asp:Label ID="ComponentSelectlbl_v2" runat="server" />
                        </td>
                    </tr>

                    <tr>
                        <td class="number">B
                        </td>
                        <td class="label labelRequired">*Duration of orders or IDT date and time:
                        </td>
                        <td class="value">From:
                            <asp:TextBox ID="ComponentFromDate_v2" MaxLength="10" onchange="DateCheck(this);" runat="server" Width="80" />
                            <asp:TextBox ID="ComponentFromTime_v2" MaxLength="4" onchange="TimeCheck(this);" runat="server" Width="40" />
                            <asp:Label ID="ComponentFromlbl_v2" runat="server" CssClass="lblDisableText" />

                            &nbsp;&nbsp;

                            To:
                            <asp:TextBox ID="ComponentToDate_v2" MaxLength="10" onchange="DateCheck(this);" runat="server" Width="80" />
                            <asp:TextBox ID="ComponentToTime_v2" MaxLength="4" onchange="TimeCheck(this);" runat="server" Width="40" />
                            <asp:Label ID="ComponentTolbl_v2" runat="server" CssClass="lblDisableText" />
                        </td>
                    </tr>

                    <tr>
                        <td class="number">C
                        </td>
                        <% 
				If Session("Compo") = "5" And (SessionInfo.SESSION_GROUP_ID = UserGroups.ANGMedicalTechnician Or SessionInfo.SESSION_GROUP_ID = UserGroups.WingSarc) Then
                        %>
                        <td class="label">Category:
                        </td>
                        <%
				Else
                        %>
                        <td class="label labelRequired">*Category:
                        </td>
                        <%
				End If
                        %>
                        <td class="value">
                            <asp:DropDownList ID="CategorySelect_v2" runat="server" Width="137px" />
                            <asp:Label ID="CategorySelectlbl_v2" runat="server" />
                        </td>
                    </tr>

                    <tr>
                        <td class="number">D
                        </td>
                        <td class="label labelRequired">*Member Status:
                        </td>
                        <td class="value">
                            <asp:DropDownList ID="MemberStatusSelect_v2" runat="server" Width="137px" />
                            <asp:Label ID="MemberStatusSelectlbl_v2" runat="server" />
                        </td>
                    </tr>

                    <tr>
                        <td class="number">E
                        </td>
                        <td class="label labelRequired">*From:
                        </td>
                        <td class="value">
                            <asp:DropDownList ID="FromSelect_v2" runat="server" Width="137px" />
                            <asp:Label ID="FromSelectlbl_v2" runat="server" />
                        </td>
                    </tr>

                    <tr>
                        <td class="number">F
                        </td>
                        <% 
                If Session("Compo") = "5" And (SessionInfo.SESSION_GROUP_ID = UserGroups.ANGMedicalTechnician Or SessionInfo.SESSION_GROUP_ID = UserGroups.WingSarc) Then
                        %>
                        <td class="label">Diagnosis:
                        </td>
                        <%
                Else
                        %>
                        <td class="label labelRequired">*Diagnosis:
                        </td>
                        <%
                End If
                        %>
                        <td class="value">
                            <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                                <ContentTemplate>
                                    <uc1:ICDCodeControl runat="server" ID="ucICDCodeControl_v2" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>

                    <tr>
                        <td class="number">G
                        </td>
                        <% 
                If Session("Compo") = "5" And (SessionInfo.SESSION_GROUP_ID = UserGroups.ANGMedicalTechnician Or SessionInfo.SESSION_GROUP_ID = UserGroups.WingSarc) Then
                        %>
                        <td class="label">7th Character:
                        </td>
                        <%
                Else
                        %>
                        <td class="label labelRequired">*7th Character:
                        </td>
                        <%
                End If
                        %>
                        <td class="value">
                            <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                                <ContentTemplate>
                                    <uc1:ICD7thCharacterControl runat="server" ID="ucICD7thCharacterControl_v2" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>

                    <tr>
                        <td class="number">H
                        </td>
                        <% 
                If Session("Compo") = "5" And (SessionInfo.SESSION_GROUP_ID = UserGroups.ANGMedicalTechnician Or SessionInfo.SESSION_GROUP_ID = UserGroups.WingSarc) Then
                        %>
                        <td class="label">Nature of Incident:
                        </td>
                        <%
                Else
                        %>
                        <td class="label labelRequired">*Nature of Incident:
                        </td>
                        <%
                End If
                        %>
                        <td class="value">
                            <asp:UpdatePanel ID="Incident_v2" runat="server" ChildrenAsTriggers="true">
                                <ContentTemplate>
                                    <asp:DropDownList ID="IncidentTypeSelect_v2" runat="server" Width="250px" AutoPostBack="true" />
                                    <asp:Label ID="IncidentTypelbl_v2" runat="server" CssClass="lblDisableText" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>

                    <tr>
                        <td class="number">I
                        </td>
                        <% 
                If Session("Compo") = "5" And (SessionInfo.SESSION_GROUP_ID = UserGroups.ANGMedicalTechnician Or SessionInfo.SESSION_GROUP_ID = UserGroups.WingSarc) Then
                        %>
                        <td class="label">Diagnosis Text:
                        </td>
                        <%
                Else
                        %>
                        <td class="label labelRequired">*Diagnosis Text:
                        </td>
                        <%
                End If
                        %>
                        <td class="value">
                            <asp:Label ID="Diagnosislbl_v2" runat="server" CssClass="lblDisableText" />
                            <asp:TextBox ID="DiagnosisTextBox_v2" Width="500px" TextMode="MultiLine" runat="server" Multiline="True" Rows="4" MaxLength="500" />
                        </td>
                    </tr>

                    <tr>
                        <td class="number">J
                        </td>
                        <td class="label labelRequired">*Type of Medical Facility:
                        </td>
                        <td class="value">
                            <asp:DropDownList ID="HospitalTypeSelect_v2" runat="server" Width="250" />
                            <asp:Label ID="HospitalTypeSelectlbl_v2" runat="server" CssClass="lblDisableText" />
                        </td>
                    </tr>

                    <tr>
                        <td class="number">K
                        </td>
                        <td class="label labelRequired">*Name and Location of initial treating facility:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="HospitalNameTextBox_v2" runat="server" Width="244px" TextMode="MultiLine" Rows="3" MaxLength="200" />
                            <asp:Label ID="HospitalNamelbl_v2" runat="server" CssClass="lblDisableText" />
                        </td>
                    </tr>

                    <tr>
                        <td class="number">L
                        </td>
                        <td class="label labelRequired">*Date and Time of Initial Treatment:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="TreatmentDateBox_v2" MaxLength="10" onchange="DateCheck(this);" runat="server" Width="80" />
                            <asp:TextBox ID="TreatmentHourBox_v2" MaxLength="4" onchange="TimeCheck(this);" runat="server" Width="40" />
                            <asp:Label ID="TreatmentDateLabel_v2" runat="server" CssClass="lblDisableText" />
                        </td>
                    </tr>

                    <tr>
                        <td class="number">M
                        </td>
                        <td class="label">
                            <span class="labelRequired">*Details of Accident or History of Disease:</span>
                            <br />
                            (how, where, when)
                        </td>
                        <td class="value">
                            <asp:TextBox ID="EventDetailsBox_v2" runat="server" MaxLength="650" TextMode="MultiLine" Width="500px" Rows="5" />
                            <asp:Label ID="EventDetailsLabel_v2" runat="server" />
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <asp:Panel runat="server" ID="MedOfficer_v2" Visible="false" CssClass="dataBlock">
            <div class="dataBlock-header">
                2 - Medical Officer
            </div>
            <div class="dataBlock-body">
                <table>
                    <tr>
                        <td class="number">A
                        </td>
                        <td class="label">
                            <span class="labelRequired">*Does this case involve death:</span>
                        </td>
                        <td class="value">
                            <asp:UpdatePanel ID="DeathCasePanel" runat="server">
                                <ContentTemplate>
                                    <asp:RadioButtonList ID="DeathCaseChoice_v2" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                        <asp:ListItem Value="No">No</asp:ListItem>
                                        <asp:ListItem Value="Yes">Yes</asp:ListItem>
                                    </asp:RadioButtonList>
                                    <asp:Label ID="DeathCaseLabel_v2" runat="server" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>

                    <tr>
                        <td class="number">B
                        </td>
                        <td class="label">
                            <span class="labelRequired">*Does this case involve a Motor Vehicle Accident (MVA):</span>
                        </td>
                        <td class="value">
                            <asp:UpdatePanel ID="MvaInvoledPanel" runat="server">
                                <ContentTemplate>
                                    <asp:RadioButtonList ID="MvaInvolvedChoice_v2" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                        <asp:ListItem Value="No">No</asp:ListItem>
                                        <asp:ListItem Value="Yes">Yes</asp:ListItem>
                                    </asp:RadioButtonList>
                                    <asp:Label ID="MvaLabel_v2" runat="server" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>

                    <tr>
                        <td class="number">C
                        </td>
                        <td class="label">
                            <span class="labelRequired">*Medical Opinion of Member's Condition when first treated:</span>
                        </td>
                        <td class="value">
                            <asp:RadioButtonList ID="MemberConditionSelect_v2" runat="server" RepeatLayout="Flow" AutoPostBack="true">
                                <asp:ListItem Value="was">was under the influence of alcohol or drugs</asp:ListItem>
                                <asp:ListItem Value="was not">was not under the influence of alcohol or drugs</asp:ListItem>
                            </asp:RadioButtonList>
                            <asp:Label ID="MemberConditionlbl_v2" runat="server" CssClass="lblDisableText" />
                        </td>
                    </tr>

                    <tr>
                        <td class="number">D
                        </td>
                        <td class="label">
                            <span class="labelRequired">*If member was under the influence, please specify:</span>
                        </td>
                        <td class="value">
                            <asp:DropDownList ID="InfluenceSelect_v2" runat="server" Width="250" AutoPostBack="true" />
                            <asp:Label ID="Influencelbl_v2" runat="server" CssClass="lblDisableText" />
                        </td>
                    </tr>

                    <tr>
                        <td class="number">E
                        </td>
                        <td class="label">
                            <span class="labelRequired">*Test Done:</span>
                        </td>
                        <td class="value">Alcohol:&nbsp;
                        <asp:RadioButtonList ID="AlcoholTest_v2" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                            <asp:ListItem Value="No">No</asp:ListItem>
                            <asp:ListItem Value="Yes">Yes</asp:ListItem>
                        </asp:RadioButtonList>
                            <asp:Label ID="AlcoholTestlbl_v2" runat="server" />

                            &nbsp;&nbsp;&nbsp;

                        Drug:&nbsp;
                        <asp:RadioButtonList ID="DrugTest_v2" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                            <asp:ListItem Value="No">No</asp:ListItem>
                            <asp:ListItem Value="Yes">Yes</asp:ListItem>
                        </asp:RadioButtonList>
                            <asp:Label ID="DrugTestlbl_v2" runat="server" />
                        </td>
                    </tr>

                    <tr>
                        <td class="number">F
                        </td>
                        <td class="label">
                            <span class="labelRequired">*Member was mentally responsible:</span>
                        </td>
                        <td class="value">
                            <asp:RadioButtonList ID="MentallyResponsiblerbl_v2" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                <asp:ListItem Value="No">No</asp:ListItem>
                                <asp:ListItem Value="Yes">Yes</asp:ListItem>
                            </asp:RadioButtonList>
                            <asp:Label ID="MentallyResponsiblelbl_v2" runat="server" CssClass="lblDisableText" />
                        </td>
                    </tr>

                    <tr>
                        <td class="number">G
                        </td>
                        <td class="label">
                            <span class="labelRequired">*Psychiatric Evaluation Completed:</span>
                        </td>
                        <td class="value">
                            <asp:RadioButtonList ID="PsychiatricEval_v2" runat="server" RepeatDirection="Horizontal" AutoPostBack="true" RepeatLayout="Flow">
                                <asp:ListItem Value="No">No</asp:ListItem>
                                <asp:ListItem Value="Yes">Yes</asp:ListItem>
                            </asp:RadioButtonList>
                            <asp:Label ID="PsychiatricEvallbl_v2" runat="server" />

                            <div id="PsychiatricDate_v2" runat="server">
                                <br />
                                Date:&nbsp;&nbsp;
                            <asp:TextBox ID="PsychiatricDatePicker_v2" MaxLength="10" onchange="DateCheck(this);" runat="server" Width="80" />
                                <asp:Label ID="PsychiatricDatelbl_v2" runat="server" CssClass="lblDisableText" />
                            </div>
                        </td>
                    </tr>

                    <tr>
                        <td class="number">H
                        </td>
                        <td class="label">
                            <span>Other relevant condition(s):</span>
                        </td>
                        <td class="value">
                            <asp:TextBox ID="RelevantConditionTextBox_v2" runat="server" MaxLength="250" TextMode="MultiLine" Width="500px" Rows="5" />
                            <asp:Label ID="RelevantConditionlbl_v2" runat="server" />
                        </td>
                    </tr>

                    <tr>
                        <td class="number">I
                        </td>
                        <td class="label">
                            <span class="labelRequired">*Other Test(s):</span>
                        </td>
                        <td class="value">
                            <asp:RadioButtonList ID="OtherTest_v2" runat="server" RepeatDirection="Horizontal" AutoPostBack="true" RepeatLayout="Flow">
                                <asp:ListItem Value="No">No</asp:ListItem>
                                <asp:ListItem Value="Yes">Yes</asp:ListItem>
                            </asp:RadioButtonList>
                            <asp:Label ID="OtherTestlbl_v2" runat="server" />

                            <div id="OtherTestDate_v2" runat="server">
                                <br />
                                Date:&nbsp;&nbsp;
                            <asp:TextBox ID="OtherTestDatePicker_v2" MaxLength="10" onchange="DateCheck(this);" runat="server" Width="80" />
                                <asp:Label ID="OtherTestDatelbl_v2" runat="server" CssClass="lblDisableText" />
                            </div>
                        </td>
                    </tr>

                    <tr>
                        <td class="number">J
                        </td>
                        <td class="label">
                            <span class="labelRequired">*Member at Deployed Location:</span>
                        </td>
                        <td class="value">
                            <asp:RadioButtonList ID="DeployedLocation_v2" runat="server" RepeatDirection="Horizontal" AutoPostBack="true" RepeatLayout="Flow">
                                <asp:ListItem Value="No">No</asp:ListItem>
                                <asp:ListItem Value="Yes">Yes</asp:ListItem>
                            </asp:RadioButtonList>
                            <asp:Label ID="DeployedLocationlbl_v2" runat="server" />
                            <asp:Label ID="LocationWarning_v2" runat="server" class="labelRequired" Visible="false" Width="550">A Yes answer should only be entered if this LOD is being initiated by deployed medical clinic 
                                        personnel in the deployed environment. LODs initiated by Reserve Medical Units for injuries, illness, disease or death that occurred secondary to member that
                                        was deployed should remain the default No</asp:Label>
                        </td>
                    </tr>

                    <tr>
                        <td class="number">K
                        </td>
                        <td class="label">
                            <span class="labelRequired">*EPTS:</span>
                        </td>
                        <td class="value">
                            <asp:RadioButtonList ID="rblEPTS_v2" runat="server" RepeatLayout="Flow">
                                <asp:ListItem Value="0">EPTS No</asp:ListItem>
                                <asp:ListItem Value="1">EPTS Yes: Service Aggravated</asp:ListItem>
                                <asp:ListItem Value="2">EPTS Yes: Not Service Aggravated</asp:ListItem>
                            </asp:RadioButtonList>
                            <asp:Label ID="lblEPTS_v2" runat="server" />
                        </td>
                    </tr>

                    <tr>
                        <td class="number">L
                        </td>
                        <td class="label">
                            <span class="labelRequired">*Condition Potentially Unfitting IAW AFI 48-123 Retention and/or Mobility Standards:</span>
                        </td>
                        <td class="value">
                            <asp:RadioButtonList ID="MobilityStandards_v2" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                <asp:ListItem Value="No">No</asp:ListItem>
                                <asp:ListItem Value="Yes">Yes</asp:ListItem>
                            </asp:RadioButtonList>
                            <asp:Label ID="MobilityStandardslbl_v2" runat="server" />
                        </td>
                    </tr>

                    <tr>
                        <td class="number">M
                        </td>
                        <td class="label">
                            <span class="labelRequired">*Requires ARC LOD Determination Board finalization:</span>
                        </td>
                        <td class="value">
                            <asp:UpdatePanel runat="server" ChildrenAsTriggers="true">
                                <ContentTemplate>
                                    <asp:RadioButtonList ID="BoardFinalizationrbl_v2" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                                        <asp:ListItem Value="No">No</asp:ListItem>
                                        <asp:ListItem Value="Yes">Yes</asp:ListItem>
                                    </asp:RadioButtonList>
                                    <asp:Label ID="BoardFinalizationlbl_v2" runat="server" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </td>
                    </tr>

                    <tr runat="server" id="ApprovalCommentsRow_v2" visible="false">
                        <td class="number">
                            <asp:Label ID="ApprovalCommentsRow_row_v2" runat="server">N</asp:Label>
                        </td>
                        <td class="label">
                            <asp:Label runat="server" ID="lblApprovalCommentsRow_v2">Local Medical Opinion:</asp:Label>
                        </td>
                        <td class="value">
                            <asp:TextBox ID="txtApprovalComments_v2" runat="server" MaxLength="1500" TextMode="MultiLine" Width="500px" Rows="5" />
                            <asp:Label ID="lblApprovalComments_v2" runat="server" />
                        </td>
                    </tr>
                    
                    <tbody runat="server" id="DocumentationQuestions" visible="false">
                    <%--<tr>
                        <td class="number">N
                        </td>
                        <td class="label">       ---Task 1 Changes Diamante Lawson ---
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
                   <%-- <tr>
                        <td class="number">O
                        </td>
                        <td class="label">          ---Task 1 Changes Diamante Lawson ---
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
                        <td class="value">          ---Task 1 Changes Diamante Lawson ---
                                    <asp:RadioButtonList ID="rblMemberRequest" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true" Enabled="false">
                                        <asp:ListItem Value="False">No</asp:ListItem>
                                        <asp:ListItem Value="True">Yes</asp:ListItem>
                                    </asp:RadioButtonList>
                                    <asp:Label ID="lblMemberRequest" runat="server" />
                        </td>
                    </tr>--%>
                    <%--<tr>
                        <td class="number">R
                        </td>
                        <td class="label">
                            <span class="labelRequired">*Was the injury incurred or aggravated during a time period covered by the qualified duty status:</span>
                        </td>
                        <td class="value">                  ---Task 1 Changes Diamante Lawson ---
                                    <asp:RadioButtonList ID="rblIncurredOrAggravated" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true" Enabled="false">
                                        <asp:ListItem Value="No">No</asp:ListItem>
                                        <asp:ListItem Value="Yes">Yes</asp:ListItem>
                                    </asp:RadioButtonList>
                                    <asp:Label ID="lblIncurredOrAggravated" runat="server" />
                        </td>
                    </tr>--%>
                    <tr>
                        <td class="number">Q
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
                    </tr>
                   <%-- <tr>
                        <td class="number">R
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
    </asp:Panel>
    <uc1:SignatureCheck ID="SigCheck" runat="server" Template="Form348Medical" CssClass="sigcheck-form" />
    <input type="hidden" id="hdnInitialHash" runat="Server" />
    <input type="hidden" id="hdnOldControlList" runat="Server" />
    <input type="hidden" id="page_refid" runat="Server" />
    <input type="hidden" id="page_readOnly" runat="Server" />
</asp:Content>

<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="FooterNested">
    <script type="text/javascript">
        var isReadOnly = false;

     
        $(function () {

            isReadOnly = element('<%= page_readOnly.ClientId %>').value == "0";

           $('.datePicker').datepicker(calendarPick("Past", "<%= CalendarImage %>"));
           $('.datePickerFuture').datepicker(calendarPick("All", "<%= CalendarImage %>"));
       });

    </script>
    <script type="text/javascript">
        function alertMessage(y) {
            alert(y);
        }
    </script>

</asp:Content>
