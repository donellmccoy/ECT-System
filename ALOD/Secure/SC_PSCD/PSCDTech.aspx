<%@ Page Language="VB" MasterPageFile="~/Secure/SC_PSCD/SC_PSCD.master" AutoEventWireup="false" Async="true" Inherits="ALOD.Web.Special_Case.PSCD.Secure_PSCD_PSCDTech" Title="Untitled Page" CodeBehind="PSCDTech.aspx.vb" %>

<%@ Import Namespace="ALODWebUtility.Common" %>
<%@ Import Namespace="ALOD.Core.Domain.Users" %>

<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_PSCD/SC_PSCD.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck" TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/ICDCodeControl.ascx" TagName="ICDCodeControl" TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/ICD7thCharacterControl.ascx" TagName="ICD7thCharacterControl" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentNested" runat="Server">

        <div class="dataBlock">
        <div class="dataBlock-header">
            0 - Point of Contact Info
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
    <!--Med-->
    <asp:Panel runat="server" ID="MedTech" CssClass="dataBlock">


        <div class="dataBlock-header">
            1 - Medical Section
        </div>
        <table>
            <tr>
                <td class="number">A</td>
                <td class="label ">*Member Status:</td>
                <td class="value">
                    <asp:DropDownList ID="MemberStatusSelect" runat="server" Width="137px" AutoPostBack="true"/>
                    <asp:Label ID="MemberStatusSelectlbl" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="number">B</td>
                <td class="label ">*Member Category:</td>
                <td class="value">
                    <asp:DropDownList ID="MemberCategorySelect" runat="server" Width="137px" />
                    <asp:Label ID="MemberCategorySelectlbl" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="number">C</td>
                <td class="label labelRequired">*RMU:</td>
                <td class="value">
                    <asp:DropDownList ID="RMUSelect" runat="server" Width="137px" />
                    <asp:Label ID="RMULabel" runat="server" />
                </td>
            </tr>

            <tr>
                <td class="number">D</td>
                <td class="label labelRequired">*Diagnosis:</td>
                <td class="value">
                    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                        <ContentTemplate>
                            <uc1:ICDCodeControl runat="server" ID="ucICDCodeControl" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>

            <tr>
                <td class="number">E</td>
                <td class="label">7th Character:</td>
                <td class="value">
                    <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                        <ContentTemplate>
                            <uc1:ICD7thCharacterControl runat="server" ID="ucICD7thCharacterControl" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </td>
            </tr>

            <tr>
                <td class="number">F</td>
                <td class="label labelRequired">
                    <asp:Label runat="server" ID="lblDiagnosisRowText" Text="*Diagnosis Text:" />
                    <br>
                    <asp:Label runat="server" ID="Label1" Text="(Are all conditions related to the above diagnosis must be listed)" />
                </td>
                <td class="value">
                    <asp:Label ID="DiagnosisLabel" runat="server" CssClass="lblDisableText"></asp:Label>
                    <asp:TextBox ID="DiagnosisTextBox" Width="500px" TextMode="MultiLine" runat="server" Rows="4" MaxLength="500"></asp:TextBox>
                </td>
            </tr>
            <tr>
                        <td class="number">G
                        </td>
                        <td class="label ">*Duration of service:
                        </td>
                        <td class="value">From:
                            <asp:TextBox ID="DurationOfServiceFromDate" MaxLength="10" onchange="DateCheck(this);" runat="server" Width="80" />
                            <asp:TextBox ID="DurationOfServiceFromTime" MaxLength="4" onchange="TimeCheck(this);" runat="server" Width="40" />
                            <asp:Label ID="DurationOfServiceFromlbl" runat="server" CssClass="lblDisableText" />

                            &nbsp;&nbsp;

                            To:
                            <asp:TextBox ID="DurationOfServiceToDate" MaxLength="10" onchange="DateCheck(this);" runat="server" Width="80" />
                            <asp:TextBox ID="DurationOfServiceToTime" MaxLength="4" onchange="TimeCheck(this);" runat="server" Width="40" />
                            <asp:Label ID="DurationOfServiceTolbl" runat="server" CssClass="lblDisableText" />
                        </td>
                    </tr>

            <tr runat="server" id="Tr1">
                <td class="number">H
                </td>
                <td class="label">Name and Location of initial treating facility:
                </td>
                <td class="value">
                    <asp:TextBox ID="FacilityLocationTextBox" runat="server" TextMode="MultiLine" Width="500px" Rows="5" />
                    <asp:Label ID="FacilityLocationTextBoxlbl" runat="server" />
                </td>
            </tr>
            <tr>
                        <td class="number">I
                        </td>
                        <td class="label">Date and Time of Initial Treatment:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="InitialTreatmentDate" MaxLength="10" onchange="DateCheck(this);" runat="server" Width="80" />
                            <asp:TextBox ID="InitialTreatmentTime" MaxLength="4" onchange="TimeCheck(this);" runat="server" Width="40" />
                            <asp:Label ID="InitialTreatmentlbl" runat="server" CssClass="lblDisableText" />
            <tr runat="server" id="Tr2">
                <td class="number">J
                </td>
                <td class="label">Details of Accident or History of Disease(how, where, when):
                </td>
                <td class="value">
                    <asp:TextBox ID="AccidentOrHistoryDetailsTextBox" runat="server" TextMode="MultiLine" Width="500px" Rows="5" />
                    <asp:Label ID="AccidentOrHistoryDetailsTextBoxlbl" runat="server" />
                </td>
            </tr>
            <tr runat="server" id="ApprovalCommentsRow">
                <td class="number">K
                </td>
                <td class="label">Local Medical Opinion:
                </td>
                <td class="value">
                    <asp:TextBox ID="medicalOpinionTxtBox" runat="server" TextMode="MultiLine" Width="500px" Rows="5" />
                    <asp:Label ID="medicalOpinionTxtBoxlbl" runat="server" />
                </td>
            </tr>
            <tr>
                <td class="number">L
                </td>
                <td class="label " >*Is condition potentially unfitting IAW AFI 48-123 retention and/or mobility standards:
                </td>
                <td class="value" >
                        <asp:RadioButtonList ID="IAW_AFI_RadioButton" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" >
                        <asp:ListItem Value="0">No</asp:ListItem>
                        <asp:ListItem Value="1">Yes</asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:Label ID="IAW_AFI_RadioButtonLabel" runat="server" />
             </td>
            </tr>

        </table>
    </asp:Panel>
     <input type="hidden" id="page_readOnly" runat="Server" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="FooterNested" runat="Server">
    <script type="text/javascript">
        var isReadOnly = false;

        $(function () {

            isReadOnly = element('<%= page_readOnly.ClientId %>').value == "0";

            $('.datePicker').datepicker(calendarPick("Past", "<%=CalendarImage %>"));

            $('.datePickerFuture').datepicker(calendarPick("All", "<%=CalendarImage %>"));

        });
        
    </script>
    <script type="text/javascript">
        function alertMessage(y) {
            alert(y);
        }
    </script>
</asp:Content>