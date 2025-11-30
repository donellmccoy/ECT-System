<%@ Page Language="VB" MasterPageFile="~/Secure/Lod/LOD.master" AutoEventWireup="false" EnableViewState="True" EnableEventValidation="false" Inherits="ALOD.Web.LOD.Secure_lod_c2_Audit" Title="Untitled Page" CodeBehind="Audit.aspx.vb" MaintainScrollPositionOnPostback="true" %>
<%@ Import Namespace="ALODWebUtility.Common" %>
<%@ Import Namespace="ALOD.Core.Domain.Users" %>

<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/Lod/LOD.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck" TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/ICDCodeControl.ascx" TagName="ICDCodeControl" TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/ICD7thCharacterControl.ascx" TagName="ICD7thCharacterControl" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentNested" runat="Server">
    <asp:Panel runat="server" ID="SG_Section" CssClass="dataBlock" >
        <div class="formHeader">
            Board Medical
        </div>
        <table>
            <tr>
                <td class="number">A
                </td>
                <td class="label labelRequired" >*Was the case medically appropriate:
                </td>
                <td class="value" >
                        <asp:RadioButtonList ID="SG_AppropriateStatusSelect" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true">
                        <asp:ListItem Value="0">No</asp:ListItem>
                        <asp:ListItem Value="1">Yes</asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:Label ID="MemberStatusLabel" runat="server" />
             </td>
            </tr>
            
            <tr id="SG_Reason" >
                <td class="number">B
                </td>
                        <td class="label labelRequired" >*Reason for No:</td>
                        <td class="value">
                            <asp:Panel ID="SG_DX" runat="server">
                                <asp:CheckBox ID="SG_DXCheckBox" runat="server" RepeatLayout="Flow" />
                                Incorrect DX on 348.
                                <asp:Label ID="SignOrderLabel" runat="server">

                            </asp:Label>
                            </asp:Panel>
                            <asp:Panel ID="SG_ISupport" runat="server">
                                <asp:CheckBox ID="SG_ISupportCheckBox" runat="server" RepeatLayout="Flow" />
                                Insufficient supporting clinical records.
                                <asp:Label ID="Label27" runat="server">

                            </asp:Label>
                            </asp:Panel>
                            <asp:Panel ID="SG_EPTS" runat="server">
                                <asp:CheckBox ID="SG_EPTSCheckBox" runat="server" RepeatLayout="Flow" />
                                EPTS recommendation not accurate or IAW with policy or accepted medical principles.
                                <asp:Label ID="Label28" runat="server">

                            </asp:Label>
                            </asp:Panel>
                            <asp:Panel ID="SG_Aggravation" runat="server">
                                <asp:CheckBox ID="SG_AggravationCheckBox" runat="server" RepeatLayout="Flow" />
                                Service Aggravation recommendation not accurate IAW policy or accepted medical principles.
                                <asp:Label ID="Label29" runat="server">

                            </asp:Label>
                            </asp:Panel>
                            <asp:Panel ID="SG_MedicalPrinciple" runat="server">
                                <asp:CheckBox ID="SG_MedicalPrincipleCheckBox" runat="server" RepeatLayout="Flow" />
                                Medical Opinion not IAW with accepted medical principles.
                                <asp:Label ID="Label30" runat="server">

                            </asp:Label>
                            </asp:Panel>
                            <asp:Panel ID="SG_OtherTitle" runat="server">
                                Other:
                            </asp:Panel>
                            <asp:Label ID="SG_OtherLabel" runat="server" CssClass="SG_OtherText" />
                            <asp:TextBox ID="SG_OtherTextBox" runat="server" MaxLength="1000" Multiline="True" Rows="4" TextMode="MultiLine" Width="500px" />
                        </td>
                 
                    </tr>
            <tr>
                <td class="number">C
                </td>
                <td class="label labelRequired">*Was correct standard of proof applied:
                </td>
                <td class="value">
                        <asp:RadioButtonList ID="SG_ProofRadioList" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                        <asp:ListItem Value="0">No</asp:ListItem>
                        <asp:ListItem Value="1">Yes</asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:Label ID="Label16" runat="server" />
                </td>
            </tr>

            <tr>
                <td class="number" >D
                    </td>
             <td class="label labelRequired">*What was correct standard of proof:
                </td>
                <td class="value">
                        <asp:RadioButtonList ID="SG_StandardOfProofRadioList" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                        <asp:ListItem Value="0">Clear and Unmistakable</asp:ListItem>
                        <asp:ListItem Value="1">Preponderance of Evidence</asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:Label ID="Label17" runat="server" />
                </td>
            </tr>
             
            <tr>
                <td class="number" >E
                    </td>
             <td class="label labelRequired">*Was the standard for proof met:
                </td>
                <td class="value">
                        <asp:RadioButtonList ID="SG_ProofMetRadioList" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                        <asp:ListItem Value="0">No</asp:ListItem>
                        <asp:ListItem Value="1">Yes</asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:Label ID="Label18" runat="server" />
                </td>
            </tr>

            <tr>
                <td class="number">F
                    </td>
             <td class="label labelRequired">*Was there evidence the member had the condition that was subject of the LOD:
                </td>
                <td class="value">
                        <asp:RadioButtonList ID="SG_EvidenceConditionRadioList" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                        <asp:ListItem Value="0">No</asp:ListItem>
                        <asp:ListItem Value="1">Yes</asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:Label ID="Label19" runat="server" />
                </td>
            </tr>
            
            <tr>
                <td class="number" >G
                    </td>
             <td class="label labelRequired">*Was there any evidence of misconduct:
                </td>
                <td class="value">
                        <asp:RadioButtonList ID="SG_MisconductRadioList" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                        <asp:ListItem Value="0">No</asp:ListItem>
                        <asp:ListItem Value="1">Yes</asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:Label ID="Label26" runat="server" />
                </td>
            </tr>

            <tr>
                <td class="number">H
                    </td>
             <td class="label labelRequired">*Should a formal investigation have been conducted:
                </td>
                <td class="value">
                        <asp:RadioButtonList ID="SG_FormalInvestigationRadioList" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                        <asp:ListItem Value="0">No</asp:ListItem>
                        <asp:ListItem Value="1">Yes</asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:Label ID="Label31" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="number">I
                    </td>
             <td class="label labelRequired" style="color:black">Comment:
                </td>
                <td class="value">
                        <asp:Label ID="SG_Comment"  runat="server"  Visible="false" />
                    <asp:TextBox ID="SG_CommentTextBox"  Width="500px" runat="server" Multiline="True" Rows="4" TextMode="MultiLine" MaxLength="10000000"/>
                    <asp:Label ID="Label23" runat="server" />
                </td>
            </tr>
            
    </table>

        </asp:Panel>
    <asp:Panel runat="server" ID="JA_Section" CssClass="dataBlock" >
                <div class="formHeader">
            Board Legal
        </div>
        <table>
            <tr>
                <td class="number">A
                </td>
                <td class="label labelRequired">*Was case legally sufficient:
                </td>
                <td class="value">
                        <asp:RadioButtonList ID="JA_LegallySelect" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true">
                        <asp:ListItem Value="0">No</asp:ListItem>
                        <asp:ListItem Value="1">Yes</asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:Label ID="Label6" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="number">B
                </td>
                    <td class="label labelRequired">*Reason for No:
                </td>
                <td class="value">

                    <asp:Panel runat="server" ID="JA_StandardOfProof">
                        <asp:CheckBox ID="JA_StandardOfProofCheckBox" runat="server" RepeatLayout="Flow"></asp:CheckBox>
                           Correct standard of proof not applied. 
                            <asp:Label ID="Label1" runat="server">

                            </asp:Label>
                        </asp:Panel>

                    <asp:Panel runat="server" ID="JA_DeathAndMVA">
                        <asp:CheckBox ID="JA_DeathAndMVACheckBox" runat="server" RepeatLayout="Flow"></asp:CheckBox>
                           Other source records not included or verified, Death and MVA specific requirements.
                            <asp:Label ID="Label2" runat="server">

                            </asp:Label>
                        </asp:Panel>

                    <asp:Panel runat="server" ID="JA_FormalPolicy">
                        <asp:CheckBox ID="JA_FormalPolicyCheckBox" runat="server" RepeatLayout="Flow"></asp:CheckBox>
                          Required Formal not directed IAW Policy.
                            <asp:Label ID="Label3" runat="server">

                            </asp:Label>
                        </asp:Panel>

                    <asp:Panel runat="server" ID="JA_AFI">
                        <asp:CheckBox ID="JA_AFICheckBox" runat="server" RepeatLayout="Flow"></asp:CheckBox>
                           Other requirements of AFI 36-2910 Att 2 not adhered to or applied; pregnancy, suicide, alcohol use, etc.
                            <asp:Label ID="Label4" runat="server">

                            </asp:Label>
                        </asp:Panel>

                    <%--<asp:Panel runat="server" ID="JA_Delete">
                        <asp:CheckBox ID="JA_DeleteCheckBox" runat="server" RepeatLayout="Flow"></asp:CheckBox>
                          N/A not needed!!!!!!
                            <asp:Label ID="Label5" runat="server">

                            </asp:Label>
                        </asp:Panel>--%>
                    <asp:Panel runat="server" ID="JA_Other">
                        Other:
                    </asp:Panel>
                    <asp:Label ID="Label7"  runat="server" CssClass="JA_OtherText" />
                    <asp:TextBox ID="JA_OtherTextBox"  Width="500px" TextMode="MultiLine" runat="server" Multiline="True" Rows="4" MaxLength="1000" />
                </td>
            </tr>
            <tr>
                <td class="number">C
                </td>
                <td class="label labelRequired">*Was correct standard of proof applied:
                </td>
                <td class="value">
                        <asp:RadioButtonList ID="JA_ProofRadioList" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                        <asp:ListItem Value="0">No</asp:ListItem>
                        <asp:ListItem Value="1">Yes</asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:Label ID="Label32" runat="server" />
                </td>
            </tr>

            <tr>
                <td class="number" >D
                    </td>
             <td class="label labelRequired">*What was correct standard of proof:
                </td>
                <td class="value">
                        <asp:RadioButtonList ID="JA_StandardOfProofRadioList" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                        <asp:ListItem Value="0">Clear and Unmistakable</asp:ListItem>
                        <asp:ListItem Value="1">Preponderance of Evidence</asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:Label ID="Label33" runat="server" />
                </td>
            </tr>
             
            <tr>
                <td class="number" >E
                    </td>
             <td class="label labelRequired">*Was the standard for proof met:
                </td>
                <td class="value">
                        <asp:RadioButtonList ID="JA_ProofMetRadioList" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true">
                        <asp:ListItem Value="0">No</asp:ListItem>
                        <asp:ListItem Value="1">Yes</asp:ListItem>
                        <asp:ListItem Value="2">Unable to determine</asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:Panel runat="server" ID="JA_UnableToDetermineText" Visible="false">
                        Reason:
                    </asp:Panel>
                    <asp:Label ID="JA_DetermineText"  runat="server" CssClass="JA_OtherText" />
                    <asp:TextBox ID="JA_DetermineTextBox" Visible="false"  Width="500px" TextMode="MultiLine" runat="server" Multiline="True" Rows="4" MaxLength="1000" />
                    <asp:Label ID="lblJA_DetermineText" runat="server" />
                </td>
            </tr>

            <tr>
                <td class="number">F
                    </td>
             <td class="label labelRequired">*Was there evidence the member had the condition that was subject of the LOD:
                </td>
                <td class="value">
                        <asp:RadioButtonList ID="JA_EvidenceConditionRadioList" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                        <asp:ListItem Value="0">No</asp:ListItem>
                        <asp:ListItem Value="1">Yes</asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:Label ID="Label35" runat="server" />
                </td>
            </tr>
            
            <tr>
                <td class="number" >G
                    </td>
             <td class="label labelRequired">*Was there any evidence of misconduct:
                </td>
                <td class="value">
                        <asp:RadioButtonList ID="JA_MisconductRadioList" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                        <asp:ListItem Value="0">No</asp:ListItem>
                        <asp:ListItem Value="1">Yes</asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:Label ID="Label36" runat="server" />
                </td>
            </tr>

            <tr>
                <td class="number">H
                    </td>
             <td class="label labelRequired">*Should a formal investigation have been conducted:
                </td>
                <td class="value">
                        <asp:RadioButtonList ID="JA_FormalInvestigationRadioList" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                        <asp:ListItem Value="0">No</asp:ListItem>
                        <asp:ListItem Value="1">Yes</asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:Label ID="Label37" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="number">I
                    </td>
             <td class="label labelRequired" style="color:black">Comment:
                </td>
                <td class="value">
                        <asp:Label ID="JA_Comment"  runat="server"  Visible="false" />
                    <asp:TextBox ID="JA_CommentTextBox"  Width="500px" TextMode="MultiLine" runat="server" Multiline="True" Rows="4" MaxLength="10000000"/>
                    <asp:Label ID="Label22" runat="server" />
                </td>
            </tr>
    </table>

         </asp:Panel>
    <asp:Panel runat="server" ID="A1_Section" CssClass="dataBlock" >

        <div class="formHeader">
            Approving Authority/SAF MRR Questions  
        </div>
        <table>
            <tr>
                <td class="number">A
                </td>
                <td class="label labelRequired">*Was status validated and accurate:
                </td>
                <td class="value">
                        <asp:RadioButtonList ID="A1_ValidatedSelect" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true">
                        <asp:ListItem Value="0">No</asp:ListItem>
                        <asp:ListItem Value="1">Yes</asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:Label ID="Label13" runat="server" />
                </td>
            </tr>

            <tr>
                <td class="number">B
                </td>
                    <td class="label labelRequired">*Reason for No:
                </td>
                <td class="value" >

                    <asp:Panel runat="server" ID="A1_Status">
                        <asp:CheckBox ID="A1_StatusCheckBox" runat="server" RepeatLayout="Flow" ></asp:CheckBox>
                           Status of member was not correctly identified when injury or illness occurred.
                            <asp:Label ID="Label8" runat="server">

                            </asp:Label>
                        </asp:Panel>

                    <asp:Panel runat="server" ID="A1_Orders" >
                        <asp:CheckBox ID="A1_OrdersCheckBox" runat="server" RepeatLayout="Flow"></asp:CheckBox>
                           Orders (pre-cert, mods and close out if available) were not verified and attached.
                            <asp:Label ID="Label9" runat="server">

                            </asp:Label>
                        </asp:Panel>

                    <asp:Panel runat="server" ID="A1_EPTS" >
                        <asp:CheckBox ID="A1_EPTSCheckBox" runat="server" RepeatLayout="Flow"></asp:CheckBox>
                          EPTS recommendation not accurate or IAW with policy or accepted medical principles.
                            <asp:Label ID="Label10" runat="server">

                            </asp:Label>
                        </asp:Panel>

                    <asp:Panel runat="server" ID="A1_IDT" >
                        <asp:CheckBox ID="A1_IDTCheckBox" runat="server" RepeatLayout="Flow"></asp:CheckBox>
                           IDT status was not certified/approved via a 40A or UTAPS report.
                            <asp:Label ID="Label11" runat="server">

                            </asp:Label>
                        </asp:Panel>

                    <asp:Panel runat="server" ID="A1_PCARS" >
                        <asp:CheckBox ID="A1_PCARSCheckBox" runat="server" RepeatLayout="Flow"></asp:CheckBox>
                          Point Credit Accounting and Reporting System (PCARS) was not attached.
                            <asp:Label ID="Label12" runat="server">

                            </asp:Label>
                        </asp:Panel>
                    <asp:Panel runat="server" ID="A1_8YearRule" >
                        <asp:CheckBox ID="A1_8YearRuleCheckBox" runat="server" RepeatLayout="Flow"></asp:CheckBox>
                          Criteria for potential application of 8 year rule not identified.
                            <asp:Label ID="Label15" runat="server">

                            </asp:Label>
                        </asp:Panel>
                    <asp:Panel runat="server" ID="A1_Other" >
                        Other:
                    </asp:Panel>
                    <asp:Label ID="A1_OtherText"  runat="server" CssClass="A1_OtherText" Visible="false" />
                    <asp:TextBox ID="A1_OtherTextBox"  Width="500px" TextMode="MultiLine" runat="server" Multiline="True" Rows="4" MaxLength="1000" />
                </td>
            </tr>
            <tr>
                <td class="number">C
                </td>
                <td class="label labelRequired">*Was Informal LOD determination correct:
                </td>
                <td class="value">
                        <asp:RadioButtonList ID="A1_DeterminationRadioList" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true">
                        <asp:ListItem Value="0">No</asp:ListItem>
                        <asp:ListItem Value="1">Yes</asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:Label ID="Label14" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="number">D
                </td>
                    <td class="label labelRequired">*If No, what should finding be:
                </td>
                <td class="value">
                        <asp:RadioButtonList ID="A1_DeterminationNotCorrectRadioList" runat="server" RepeatLayout="Flow">
                        <asp:ListItem Value="0">ILOD</asp:ListItem>
                        <asp:ListItem Value="1">NILOD Not Due to Misconduct</asp:ListItem>
                        <asp:ListItem Value="2">NILOD Not Due to Misconduct by reason of EPTS-NSA</asp:ListItem>
                        <asp:ListItem Value="4">NILOD-Due to Misconduct</asp:ListItem>
                        <asp:ListItem Value="3">Not medically and/or legally appropriate - RFA (Return for Action)</asp:ListItem>
                        <asp:ListItem Value="5">Direct Formal</asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:Label ID="Label21" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="number">E
                </td>
                <td class="label labelRequired">*Did member sign the Member LOD Initiation Form certifying the information to be true:
                </td>
                <td class="value">
                        <asp:RadioButtonList ID="A1_LODInitRadioList" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                        <asp:ListItem Value="0">No</asp:ListItem>
                        <asp:ListItem Value="1">Yes</asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:Label ID="Label38" runat="server" />
                </td>
            </tr>

            <tr>
                <td class="number" >F
                    </td>
             <td class="label labelRequired">*Is there a written diagnosis from a medical provider that supports the claimed condition:
                </td>
                <td class="value">
                        <asp:RadioButtonList ID="A1_WrittenDiagnosisRadioList" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                        <asp:ListItem Value="0">No</asp:ListItem>
                        <asp:ListItem Value="1">Yes</asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:Label ID="Label39" runat="server" />
                </td>
            </tr>
             
            <tr>
                <td class="number" >G
                    </td>
             <td class="label labelRequired">*Did the member request the LOD within 180 days of completing the qualified duty status, or is the diagnosis a laten onset condition, e.g., post-traumatic stress disorder and other mental, behavioral, neurodevelopmental conditions:
                </td>
                <td class="value">
                        <asp:RadioButtonList ID="A1_MemberRequestRadioList" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                        <asp:ListItem Value="0">No</asp:ListItem>
                        <asp:ListItem Value="1">Yes</asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:Label ID="Label40" runat="server" />
                </td>
            </tr>

            <tr>
                <td class="number">H
                    </td>
             <td class="label labelRequired">*Was the injury incurred or aggravated during a time period covered by the qualified duty status:
                </td>
                <td class="value">
                        <asp:RadioButtonList ID="A1_IncurredOrAggravatedRadioList" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                        <asp:ListItem Value="0">No</asp:ListItem>
                        <asp:ListItem Value="1">Yes</asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:Label ID="Label41" runat="server" />
                </td>
            </tr>
            
            <tr>
                <td class="number" >I
                    </td>
             <td class="label labelRequired">*Is there medical evidence the illness or disease existed prior to the qualified duty status:
                </td>
                <td class="value">
                        <asp:RadioButtonList ID="A1_IllnessOrDiseaseRadioList" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true">
                        <asp:ListItem Value="0">No</asp:ListItem>
                        <asp:ListItem Value="1">Yes</asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:Label ID="Label42" runat="server" />
                </td>
            </tr>

            <tr>
                <td class="number">J
                    </td>
             <td class="label labelRequired">*Is there medical evidence that activities during the qualified duty status worsened the pre-service condition beyond its natural progression:
                </td>
                <td class="value">
                        <asp:RadioButtonList ID="A1_ActivitesRadioList" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                        <asp:ListItem Value="0">No</asp:ListItem>
                        <asp:ListItem Value="1">Yes</asp:ListItem>
                        <asp:ListItem Value="2">N/A</asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:Label ID="Label43" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="number">K
                    </td>
             <td class="label labelRequired" style="color:black">Comment:
                </td>
                <td class="value">
                        <asp:Label ID="A1_Comment"  runat="server"  Visible="false" />
                    <asp:TextBox ID="A1_CommentTextBox"  Width="500px" TextMode="MultiLine" runat="server" Multiline="True" Rows="4" MaxLength="1000" />
                    <asp:Label ID="Label20" runat="server" />
                </td>
            </tr>
            
    </table>

        </asp:Panel>
    
           
        <input type="hidden" id="hdnInitialHash" runat="Server" />
    <input type="hidden" id="hdnOldControlList" runat="Server" />
    <input type="hidden" id="page_refid" runat="Server" />
    <input type="hidden" id="page_readOnly" runat="Server" />
</asp:Content>

<asp:Content ID="Content2" runat="server" contentplaceholderid="HeaderNested">
    <style type="text/css">color
        .auto-style1 {
            width: 830px;
        }
    </style>
</asp:Content>

