<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SC_WWD/SC_WWD.master" MaintainScrollPositionOnPostback="true"
    EnableEventValidation="false" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.WWD.Secure_sc_WD_MedTech" Codebehind="MedTech.aspx.vb" %>

<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Register Src="~/secure/Shared/UserControls/FindingsControl.ascx" TagName="Findings"
    TagPrefix="uc" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_WWD/SC_WWD.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck"
    TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/ICDCodeControl.ascx" TagName="ICDCodeControl" TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/ICD7thCharacterControl.ascx" TagName="ICD7thCharacterControl" TagPrefix="uc1" %>



<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    <div class="dataBlock">
        <div class="dataBlock-header">
            WWD Case Info - RMU
        </div>
        <br />
        <asp:Label runat="server" CssClass="label labelRequired" ID="Label6"
                        LabelFor="ddlMedGroups">Med Group Name: </asp:Label>
        <asp:DropDownList runat="server" ID="ddlMedGroups"></asp:DropDownList>
        <br />
        <br />
        <asp:Label runat="server" CssClass="label labelRequired" ID="Label5"
            LabelFor="ddlRMUs">RMU Name: </asp:Label>
        <asp:DropDownList runat="server" ID="ddlRMUs"></asp:DropDownList>
        <br />
        <br />
        <asp:Label runat="server">POC Unit: </asp:Label>
        <asp:Label ID="POCUnitLabel" runat="server"></asp:Label>
        <br />
        <br />
        <asp:Label ID="Label1" runat="server">POC Name: </asp:Label>
        <asp:Label ID="POCNameLabel" runat="server"></asp:Label>
        <br />
        <br />
        <asp:Label ID="Label3" runat="server">POC DSN/Phone: </asp:Label>
        <asp:Label ID="POCPhoneLabel" runat="server"></asp:Label>
        <br />
        <br />
        <asp:Label ID="Label2" runat="server">POC Email: </asp:Label>
        <asp:Label ID="POCEmailLabel" runat="server"></asp:Label>
        <br />
        <br />
        <asp:Label ID="Label17" runat="server">Documents Included with WWD case: </asp:Label>
            <asp:RadioButton ID="DocumentsAttachedY" runat="server" GroupName="DocumentsAttached" Text="Yes" AutoPostBack="True" />
            &nbsp;&nbsp;&nbsp;
            <asp:RadioButton ID="DocumentsAttachedN" runat="server" GroupName="DocumentsAttached" Text="No" AutoPostBack="True" />
        <br />
        <br />
        <asp:Label ID="Label7" runat="server">Cover Letter (uploaded): </asp:Label><asp:RadioButton
            ID="CoverLetterY" runat="server" GroupName="CoverLetter" Text="Yes" Enabled="False" />&nbsp;&nbsp;&nbsp;<asp:RadioButton
                ID="CoverLetterN" runat="server" GroupName="CoverLetter" Text="No" Enabled="False" />
        <br />
        <br />
    </div>
    <div class="dataBlock">
        <div class="dataBlock-header">
            Select an ICD code
             </div>
        <table>
            <tr>
                <td class="number">
                    A
                </td>
                <td class="label  labelRequired">
                    *Diagnosis:
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
                <td class="number">
                    B
                </td>
                <td class="label">
                    7th Character:
                </td>
                <td class="value">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <uc1:ICD7thCharacterControl runat="server" ID="ucICD7thCharacterControl" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>
            <tr>
                <td class="number">
                    C
                </td>
                <td class="label labelRequired">
                    *Diagnosis Text:
                </td>
                <td class="value">
                    <asp:Label ID="DiagnosisLabel" runat="server" CssClass="lblDisableText"></asp:Label>
                    <asp:TextBox ID="DiagnosisTextBox" Width="500px" TextMode="MultiLine" runat="server"
                        Rows="4" MaxLength="250"></asp:TextBox>
                </td>
            </tr>
        </table>
    </div>
    <div id="Documentation" class="dataBlock" runat="server">
        <div class="dataBlock-header">
            WWD Case Info - Documents Included
        </div>
        <br />
        <asp:Label ID="Label8" runat="server">AF Form 469 (uploaded): </asp:Label><asp:RadioButton
            ID="AFForm469Y" runat="server" GroupName="AFForm469" Text="Yes" />&nbsp;&nbsp;&nbsp;<asp:RadioButton
                ID="AFForm469N" runat="server" GroupName="AFForm469" Text="No" />
        <br />
        <br />

        <asp:Label ID="Label20" runat="server">Initial Date of Code 37: </asp:Label>
        <asp:TextBox runat="server" ID="txtCode37Date" MaxLength="10" onchange="DateCheck(this);" Text="" />
        <br />
        <br />

        <asp:Label ID="Label14" runat="server">Narrative Summary (uploaded): </asp:Label><asp:RadioButton
            ID="NarrativeSummaryY" runat="server" GroupName="NarrativeSummary" Text="Yes" />&nbsp;&nbsp;&nbsp;<asp:RadioButton
                ID="NarrativeSummaryN" runat="server" GroupName="NarrativeSummary" Text="No" />
        <br />
        <br />
        <asp:Label runat="server" ID="Label10">IPEB Election: </asp:Label><asp:RadioButton
            ID="IPEBElectionY" runat="server" GroupName="IPEBElection" Text="Yes" />&nbsp;&nbsp;&nbsp;<asp:RadioButton
                ID="IPEBElectionN" runat="server" GroupName="IPEBElection" Text="No" />
        <br />
        <br />

            <asp:Label runat="server" ID="Label4">IPEB Refusal: </asp:Label><asp:RadioButton
                ID="IPEBRefusalY" runat="server" GroupName="IPEBRefusal" Text="Yes" />&nbsp;&nbsp;&nbsp;<asp:RadioButton
                    ID="IPEBRefusalN" runat="server" GroupName="IPEBRefusal" Text="No" />
            <br />
            <br />
            <asp:Label ID="Label15" runat="server">IPEB Signature Date (Election or Refusal): </asp:Label><asp:TextBox
                runat="server" ID="IPEBSignatureDate" MaxLength="10" onchange="DateCheck(this);"
                Text="" />
            <br />
            <br />

        <asp:Label ID="Label16" runat="server">Member Utilization Questionnaire (MUQ) Request Date: </asp:Label><asp:TextBox
            runat="server" ID="MUQRequestDate" MaxLength="10" onchange="DateCheck(this);"
            Text="" />
        <br />
        <br />
        <asp:Label ID="Label9" runat="server">MUQ Upload Date: </asp:Label><asp:TextBox runat="server"
            ID="MUQUploadDate" MaxLength="10" onchange="DateCheck(this);" Text="" AutoPostBack="true" />
        <br />
        <br />

        <asp:Panel ID="pnlAFRC" runat="server">
            <asp:Label ID="Label11" runat="server">Is Statement included in Cover Letter (if MUQ not valid/received): </asp:Label><asp:RadioButton
                ID="MemberStatementY" runat="server" GroupName="MemberStatement" Text="Yes" />&nbsp;&nbsp;&nbsp;<asp:RadioButton
                    ID="MemberStatementN" runat="server" GroupName="MemberStatement" Text="No" />
            <br />
            <br />
            <asp:Label ID="Label12" runat="server">Unit Commander Memorandum (attached): </asp:Label><asp:RadioButton
                ID="UnitCommanderMemoY" runat="server" GroupName="UnitCommanderMemo" Text="Yes" />&nbsp;&nbsp;&nbsp;<asp:RadioButton
                    ID="UnitCommanderMemoN" runat="server" GroupName="UnitCommanderMemo" Text="No" />
            <br />
            <br />
            <asp:Label ID="Label13" runat="server">Medical Evaluation Fact Sheet Signature Date: </asp:Label><asp:TextBox
                runat="server" ID="MEFSSignatureDate" MaxLength="10" onchange="DateCheck(this);"
                Text="" />
            <br />
            <br />
            <asp:Label ID="Label18" runat="server">ME Fact Sheet Waiver - Signature Date: </asp:Label><asp:TextBox
                runat="server" ID="MEFSWaiverDate" MaxLength="10" onchange="DateCheck(this);"
                Text="" />
        </asp:Panel>


        <br />
        <br />
        <asp:Label ID="Label19" runat="server">Private Physician Documentation (attached): </asp:Label><asp:RadioButton
            ID="PrivatePhysicianY" runat="server" GroupName="PrivatePhysician" Text="Yes" />&nbsp;&nbsp;&nbsp;<asp:RadioButton
                ID="PrivatePhysicianN" runat="server" GroupName="PrivatePhysician" Text="No" />
        <br />
        <br />
    </div>
    <div id="MailingRequests" class="dataBlock" runat="server">
        <div class="dataBlock-header">
            WWD Case Info - Mailing Requests
        </div>
        <br />
        <asp:Label ID="Label25" runat="server">Certified Mail (PS 3811) Request Date: </asp:Label><asp:TextBox
            runat="server" ID="PS3811RequestDate" MaxLength="10" onchange="DateCheck(this);"
            Text="" />
        <br />
        <br />
        <asp:Label ID="Label26" runat="server">PS 3811 Signature Date: </asp:Label><asp:TextBox
            runat="server" ID="PS3811SignatureDate" MaxLength="10" onchange="DateCheck(this);"
            Text="" />
        <br />
        <br />
        <asp:Label ID="Label30" runat="server">PS 3811 (uploaded): </asp:Label><asp:RadioButton
            ID="PS3811UploadY" runat="server" GroupName="PS3811Upload" Text="Yes" />&nbsp;&nbsp;&nbsp;<asp:RadioButton
                ID="PS3811UploadN" runat="server" GroupName="PS3811Upload" Text="No" />
        <br />
        <br />
        <asp:Label ID="Label27" runat="server">First Class Mailing Date: </asp:Label><asp:TextBox
            runat="server" ID="FirstClassMailDate" MaxLength="10" onchange="DateCheck(this);"
            Text="" />
        <br />
        <br />
        <asp:Label ID="Label28" runat="server">Attempts to Contact Member included in Cover Letter: </asp:Label><asp:RadioButton
            ID="MemberContactY" runat="server" GroupName="MemberContact" Text="Yes" />&nbsp;&nbsp;&nbsp;<asp:RadioButton
                ID="MemberContactN" runat="server" GroupName="MemberContact" Text="No" />
        <br />
        <br />
        <asp:Label ID="Label29" runat="server">Copy of Letter to Member & Attachment List (uploaded): </asp:Label><asp:RadioButton
            ID="MemberLetterY" runat="server" GroupName="MemberLetter" Text="Yes" />&nbsp;&nbsp;&nbsp;<asp:RadioButton
                ID="MemberLetterN" runat="server" GroupName="MemberLetter" Text="No" />
        <br />
        <br />
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">
    <script type="text/javascript">

        $(function () {

            $('.datePicker').datepicker(calendarPick("Past", "<%=CalendarImage %>"));

            $('.datePickerAll').datepicker(calendarPick("All", "<%=CalendarImage %>"));

        });

    </script>
</asp:Content>
