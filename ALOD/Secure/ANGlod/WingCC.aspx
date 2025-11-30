<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Lod/Lod.master" MaintainScrollPositionOnPostback="true"
    AutoEventWireup="false" Inherits="ALOD.Web.LOD.Secure_lod_WingCC" Codebehind="WingCC.aspx.vb" %>

<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Register Src="~/secure/Shared/UserControls/FindingsControl.ascx" TagName="Findings"
    TagPrefix="uc" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/Lod/LOD.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck"
    TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    <script type="text/javascript">



        $(document).ready(function () {
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

            $('.open-dialog').click(function () {
                $('#srchBox').dialog('open');
            })
            .hover(
            function () {
                $(this).addClass("ui-state-hover");
            },
            function () {
                $(this).removeClass("ui-state-hover");
            }
            )
            .mousedown(function () {
                $(this).addClass("ui-state-active");
            })
            .mouseup(function () {
                $(this).removeClass("ui-state-active");
            });


        })

        //Show Searcher
        function showSearcher(title, targetId, targetlbl) {
            initializeUnitSearcher();
            //Set Client controls where unit Id and unit names will be transferred
            element('<%=hdnIdClient.ClientId %>').value = targetId;
            element('<%=hdnNameClient.ClientId %>').value = targetlbl;
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

    <asp:Panel ID="Original_LOD" Visible="true" runat="server">
        <asp:Panel ID="FormalPanel" Visible="false" runat="server" CssClass="dataBlock">
            <div class="dataBlock-header">
                Formal Appointing Authority Review
            </div>
            <div class="dataBlock-body">
                <uc:Findings ID="FormalFindings" ShowFormText="true" RemarksLableText="Reasons:" FindingsLableText="Substituted Findings:"  ShowPrevFindings="True" ConcurWith="Investigation Officer"
                    SetDecisionToggle="True" ShowRemarks="false" ReasonsLabelText="Reasons:" ShownOnText="(Shown on Form 261)" SetReadOnly="False" runat="Server" UseRowLabels="true">
                </uc:Findings>
                <uc1:SignatureCheck ID="SigCheckFormal" runat="server" Template="Form348Findings" CssClass="sigcheck-form no-border" />
                <br />
            </div>
        </asp:Panel>
        <asp:Panel runat="server" ID="AAEditPanel" CssClass="dataBlock" Visible="false">
            <!-- Edit Panel -->
            <div class="dataBlock-header">
                <asp:Label runat="server" ID="lblAAEditPanelHeader" Text="Appointing Authority Review"></asp:Label>
            </div>
            <div class="dataBlock-body">
                <uc:Findings ID="ucInformalAndReinvestigationFindings" ShowFormText="False" RemarksLableText="Remarks:" FindingsLableText="Findings:"  ShowPrevFindings="False" FindingsOnly="True"
                    ShowRemarks="True" SetReadOnly="False" runat="Server" UseRowLabels="True" FindingsRequired="True" DoDecisionAutoPostBack="True" DoFindingsAutoPostBack="True">
                </uc:Findings>

                <asp:Panel runat="server" ID="pnlIOControls" Visible="false">
                    <table>
                        <tr>
                            <td class="number">
                                <asp:Label runat="server" ID="lblIORow" Text="C" />
                            </td>
                            <td class="label aa-formal">
                                <asp:Label runat="server" ID="lblIOLabel" Text="Investigating Officer:" />
                            </td>
                            <td class="value">
                                <asp:DropDownList runat="server" Width="200" ID="cbAssignIo" DataTextField="name"
                                    DataValueField="userId" />
                                <asp:Label ID="lblIO" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                <asp:Label runat="server" ID="lblIOCompletedByRow" Text="D" />
                            </td>
                            <td class="label aa-formal">
                                <asp:Label runat="server" ID="lblIOCompletionDateLabel" Text="Investigation to be completed by:" />
                            </td>
                            <td class="value">
                                <asp:TextBox ID="txtIOCompletionDate" CssClass="datePicker" MaxLength="10" runat="server"
                                    Width="80"></asp:TextBox>
                                (MM/DD/YYYY)
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                <asp:Label runat="server" ID="lblIOInstructionsRow" Text="E" />
                            </td>
                            <td class="label">
                                Additional Instructions to Investigating Officer:
                            </td>
                            <td class="value">
                                <asp:TextBox ID="txtIOInstruction" runat="server" TextMode="MultiLine" Width="500px"
                                    Rows="5" />
                                <br />
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                <asp:Label runat="server" ID="lblIOPOCRow" Text="F" />
                            </td>
                            <td class="label">
                                POC for Appointing Authority (Name, Phone number, etc):
                            </td>
                            <td class="value">
                                <asp:TextBox ID="txtAAPOC" runat="server" MaxLength="500" TextMode="MultiLine" Width="500px"
                                    Rows="5" />
                                <br />
                                <br />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </div>
        </asp:Panel>
        <asp:Panel runat="server" ID="AAReviewPanel" CssClass="dataBlock" Visible="false">
            <!-- Edit Panel -->
            <div class="dataBlock-header">
                <asp:Label runat="server" ID="lblAAReviewPanelHeader" Text="Appointing Authority Review"></asp:Label>
            </div>
            <div class="dataBlock-body">
                <table>
                    <tr runat="server" id="trIOFindings" visible="false">
                        <td class="number">
                        
                        </td>
                        <td class="label">
                            IO Findings:
                        </td>
                        <td class="value">
                            <asp:Label runat="server" ID="lblAARevIOFindings" />
                        </td>
                    </tr>
                    <tr runat="server" id="trDecision" visible="false">
                        <td class="number">
                            <asp:Label runat="server" ID="lblAARevDecisionRow" Text="A" />
                        </td>
                        <td class="label">
                            Decision:
                        </td>
                        <td class="value">
                            <asp:Label runat="server" ID="lblAARevDecision" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label runat="server" ID="lblAARevFindingsRow" Text="A" />
                        </td>
                        <td class="label">
                            <asp:Label runat="server" ID="lblAARevFindings" Text="Findings:" />
                        </td>
                        <td class="value">
                            <asp:Label runat="server" ID="FindingsLabel" />
                            <br />
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label runat="server" ID="lblAARevRemarksRow" Text="B" />
                        </td>
                        <td class="label">
                            <asp:Label runat="server" ID="lblAARevRemarks" Text="Remarks:" />
                        </td>
                        <td class="value">
                            <asp:Label runat="server" ID="RemarksLabel" />
                            <br />
                            <br />
                        </td>
                    </tr>
                </table>
                <asp:Panel runat="server" ID="IoPanel" Visible="false">
                    <table>
                        <tr>
                            <td class="number">
                                <asp:Label runat="server" ID="lblAARevIORow" Text="C" />
                            </td>
                            <td class="label">
                                Investigating Officer:
                            </td>
                            <td class="value">
                                <asp:Label ID="CurrentIoLabel" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                <asp:Label runat="server" ID="lblAARevIOCompletedByRow" Text="D" />
                            </td>
                            <td class="label">
                                Investigation to be completed by:
                            </td>
                            <td class="value">
                                <asp:Label runat="server" ID="CompleteByLabel"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                <asp:Label runat="server" ID="lblAARevIOInstructionsRow" Text="E" />
                            </td>
                            <td class="label">
                                Additional Instructions to Investigating Officer:
                            </td>
                            <td class="value">
                                <asp:Label runat="server" ID="InstructionsLabel" />
                                <br />
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                <asp:Label runat="server" ID="lblAARevIOPOCRow" Text="F" />
                            </td>
                            <td class="label">
                                POC for Appointing Authority (Name, Phone number, etc):
                            </td>
                            <td class="value">
                                <asp:Label runat="server" ID="PocLabel" />
                                <br />
                                <br />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </div>
            <uc1:SignatureCheck ID="SigCheckInformal" runat="server" Template="WingCC" CssClass="sigcheck-form no-border" />
            <br />
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlAppointedIOReview" CssClass="dataBlock" Visible="false">
            <div class="dataBlock-header">
                <asp:Label runat="server" ID="lblAppointedIOHeader" Text="Appointed Investigating Officer"></asp:Label>
            </div>
            <div class="dataBlock-body">
                <table>
                    <tr>
                        <td class="number">
                            <asp:Label runat="server" ID="lblAppointedIONameRow" Text="A" />
                        </td>
                        <td class="label">
                            Name:
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblAppointedIOName" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label runat="server" ID="lblAppointedIORankRow" Text="B" />
                        </td>
                        <td class="label">
                            Rank:
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblAppointedIORank" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label runat="server" ID="lblAppointedIOEDIPIRow" Text="C" />
                        </td>
                        <td class="label labelRequired">
                            EDIPI:
                        </td>
                        <td>
                            <asp:TextBox Enabled="true" runat="server" Width="280px" ID="txtAppointedIOEDIPI" />
                            <asp:Label runat="server" ID="lblAppointedIOEDIPI" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label runat="server" ID="lblAppointedIOUnitRow" Text="D" />
                        </td>
                        <td class="label labelRequired">
                            Unit:
                        </td>
                        <td>
                            <asp:TextBox Enabled="false" runat="server" Width="280px" ID="txtAppointedIOUnit" />
                            <asp:Button runat="server" ID="btnChangeUnit" Text="Change" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" SetFocusOnError="True" runat="server"
                                    ErrorMessage="Unit Required." ControlToValidate="lblNewUnitID" ValidationGroup="input" Display="Dynamic">*</asp:RequiredFieldValidator>
                            <asp:TextBox ID="lblNewUnitID" CssClass="hidden" runat="server"></asp:TextBox>

                            <asp:Label runat="server" ID="lblAppointedIOUnit" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label runat="server" ID="lblAppointedIOComponentRow" Text="E" />
                        </td>
                        <td class="label">
                            Component:
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblAppointedIOComponent" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label runat="server" ID="lblAppointedIOIATrainingRow" Text="F" />
                        </td>
                        <td class="label labelRequired">
                            <asp:Label runat="server" ID="lblAppointedIOIATrainingRowText" Text="IA Training Expiration Date:" ToolTip="The date one year from the IA Training date of completion." />
                        </td>
                        <td>
                            <asp:TextBox ID="txtAppointedIOIATraining" onchange="DateCheck(this);" runat="server" Width="80" MaxLength="10" CssClass="datePicker" />
                            <asp:Label runat="server" ID="lblAppointedIOIATraining" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label runat="server" ID="lblAppointedIOEmailRow" Text="G" />
                        </td>
                        <td class="label labelRequired">
                            Work Email Address:
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtAppointedIOEmail" Width="280px" MaxLength="150" Visible="False" />
                            <asp:Label runat="server" ID="lblAppointedIOEmail" Visible="False" />
                        </td>
                        <td>
                            <asp:RequiredFieldValidator runat="server" ID="rqfdAppointedIOEmail" ControlToValidate="txtAppointedIOEmail" SetFocusOnError="True" ErrorMessage="Email required" ValidationGroup="SetAppointedIO" Display="Dynamic">
                                Please Enter Work Email
                            </asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator runat="server" ID="rgExpAppointedIOEmail" ControlToValidate="txtAppointedIOEmail" ErrorMessage="Invalid Work Email." ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="SetAppointedIO">
                                Invalid Work Email
                            </asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr runat="server" id="trAppointedIOError" visible="false">
                        <td class="number">
                            <asp:Label runat="server" ID="lblAppointedIOErrors" Text="H" />
                        </td>
                        <td class="label">
                            ERRORS:
                        </td>
                        <td>
                            <asp:BulletedList runat="server" ID="bllAppointedIOErrors" CssClass="labelRequired" />
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
    </asp:Panel>

    <asp:Panel runat="server" ID="LOD_v2_Panel" Visible="false">
        <asp:Panel ID="FormalPanel_v2" Visible="false" runat="server" CssClass="dataBlock">
            <div class="dataBlock-header">
                Formal Appointing Authority Review
            </div>
            <div class="dataBlock-body">
                <uc:Findings ID="FormalFindings_v2" ShowFormText="true" RemarksLableText="Reasons:" FindingsLableText="Substituted Findings:"  ShowPrevFindings="True" ConcurWith="Investigation Officer"
                    SetDecisionToggle="True" ShowRemarks="false" ReasonsLabelText="Reasons:" ShownOnText="(Shown on Form 261)" SetReadOnly="False" LoadAll="true" runat="Server" UseRowLabels="true">
                </uc:Findings>
                <uc1:SignatureCheck ID="SigCheckFormal_v2" runat="server" Template="Form348Findings" CssClass="sigcheck-form no-border" />
                <br />
            </div>
        </asp:Panel>

        <asp:Panel runat="server" ID="AAEditPanel_v2" CssClass="dataBlock" Visible="false">
            <!-- Edit Panel -->
            <div class="dataBlock-header">
                <asp:Label runat="server" ID="lblAAEditPanelHeader_v2" Text="Appointing Authority Review"></asp:Label>
            </div>
            <div class="dataBlock-body">
                <uc:Findings ID="ucInformalAndReinvestigationFindings_v2" ShowFormText="False" RemarksLableText="Remarks:" FindingsLableText="Findings:"  ShowPrevFindings="False" FindingsOnly="True"
                    ShowRemarks="True" SetReadOnly="False" runat="Server" UseRowLabels="True" FindingsRequired="True" DoDecisionAutoPostBack="True" DoFindingsAutoPostBack="True">
                </uc:Findings>

                <asp:Panel runat="server" ID="NILOD_subFindings" Visible="false">
                    <table class="dataTable">
                        <tr>
                            <td class="number">
                                C
                            </td>
                            <td class="label">
                                Not ILOD reason:
                            </td>
                            <td class="value">
                                <asp:RadioButtonList ID="rblSubFindings" runat="server" RepeatDirection="Vertical" RepeatLayout="Flow" autopostback="true">
                                </asp:RadioButtonList>
                                <asp:Label ID="lblSubFindings" runat="server"></asp:Label>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>

                <asp:Panel runat="server" ID="pnlIOControls_v2" Visible="false">
                    <table>
                        <tr>
                            <td class="number">
                                <asp:Label runat="server" ID="lblIORow_v2" Text="C" />
                            </td>
                            <td class="label aa-formal">
                                <asp:Label runat="server" ID="lblIOLabel_v2" Text="Investigating Officer:" />
                            </td>
                            <td class="value">
                                <asp:DropDownList runat="server" Width="200" ID="cbAssignIo_v2" DataTextField="name"
                                    DataValueField="userId" />
                                <asp:Label ID="lblIO_v2" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                <asp:Label runat="server" ID="lblIOCompletedByRow_v2" Text="D" />
                            </td>
                            <td class="label aa-formal">
                                <asp:Label runat="server" ID="lblIOCompletionDateLabel_v2" Text="Investigation to be completed by:" />
                            </td>
                            <td class="value">
                                <asp:TextBox ID="txtIOCompletionDate_v2" CssClass="datePicker" MaxLength="10" runat="server"
                                    Width="80"></asp:TextBox>
                                (MM/DD/YYYY)
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                <asp:Label runat="server" ID="lblIOInstructionsRow_v2" Text="E" />
                            </td>
                            <td class="label">
                                Additional Instructions to Investigating Officer:
                            </td>
                            <td class="value">
                                <asp:TextBox ID="txtIOInstruction_v2" runat="server" TextMode="MultiLine" Width="500px"
                                    Rows="5" />
                                <br />
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                <asp:Label runat="server" ID="lblIOPOCRow_v2" Text="F" />
                            </td>
                            <td class="label">
                                POC for Appointing Authority (Name, Phone number, etc):
                            </td>
                            <td class="value">
                                <asp:TextBox ID="txtAAPOC_v2" runat="server" MaxLength="500" TextMode="MultiLine" Width="500px"
                                    Rows="5" />
                                <br />
                                <br />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </div>
        </asp:Panel>

        <asp:Panel runat="server" ID="AAReviewPanel_v2" CssClass="dataBlock" Visible="false">
            <!-- Edit Panel -->
            <div class="dataBlock-header">
                <asp:Label runat="server" ID="lblAAReviewPanelHeader_v2" Text="Appointing Authority Review"></asp:Label>
            </div>
            <div class="dataBlock-body">
                <table>
                    <tr runat="server" id="trIOFindings_v2" visible="false">
                        <td class="number">
                        
                        </td>
                        <td class="label">
                            IO Findings:
                        </td>
                        <td class="value">
                            <asp:Label runat="server" ID="lblAARevIOFindings_v2" />
                        </td>
                    </tr>
                    <tr runat="server" id="trDecision_v2" visible="false">
                        <td class="number">
                            <asp:Label runat="server" ID="lblAARevDecisionRow_v2" Text="A" />
                        </td>
                        <td class="label">
                            Decision:
                        </td>
                        <td class="value">
                            <asp:Label runat="server" ID="lblAARevDecision_v2" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label runat="server" ID="lblAARevFindingsRow_v2" Text="A" />
                        </td>
                        <td class="label">
                            <asp:Label runat="server" ID="lblAARevFindings_v2" Text="Findings:" />
                        </td>
                        <td class="value">
                            <asp:Label runat="server" ID="FindingsLabel_v2" />
                            <br />
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label runat="server" ID="lblAARevRemarksRow_v2" Text="B" />
                        </td>
                        <td class="label">
                            <asp:Label runat="server" ID="lblAARevRemarks_v2" Text="Remarks:" />
                        </td>
                        <td class="value">
                            <asp:Label runat="server" ID="RemarksLabel_v2" />
                            <br />
                            <br />
                        </td>
                    </tr>
                </table>
                <asp:Panel runat="server" ID="IoPanel_v2" Visible="false">
                    <table>
                        <tr>
                            <td class="number">
                                <asp:Label runat="server" ID="lblAARevIORow_v2" Text="C" />
                            </td>
                            <td class="label">
                                Investigating Officer:
                            </td>
                            <td class="value">
                                <asp:Label ID="CurrentIoLabel_v2" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                <asp:Label runat="server" ID="lblAARevIOCompletedByRow_v2" Text="D" />
                            </td>
                            <td class="label">
                                Investigation to be completed by:
                            </td>
                            <td class="value">
                                <asp:Label runat="server" ID="CompleteByLabel_v2"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                <asp:Label runat="server" ID="lblAARevIOInstructionsRow_v2" Text="E" />
                            </td>
                            <td class="label">
                                Additional Instructions to Investigating Officer:
                            </td>
                            <td class="value">
                                <asp:Label runat="server" ID="InstructionsLabel_v2" />
                                <br />
                                <br />
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                <asp:Label runat="server" ID="lblAARevIOPOCRow_v2" Text="F" />
                            </td>
                            <td class="label">
                                POC for Appointing Authority (Name, Phone number, etc):
                            </td>
                            <td class="value">
                                <asp:Label runat="server" ID="PocLabel_v2" />
                                <br />
                                <br />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </div>
            <uc1:SignatureCheck ID="SigCheckInformal_v2" runat="server" Template="WingCC" CssClass="sigcheck-form no-border" />
            <br />
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlAppointedIOReview_v2" CssClass="dataBlock" Visible="false">
            <div class="dataBlock-header">
                <asp:Label runat="server" ID="lblAppointedIOHeader_v2" Text="Appointed Investigating Officer"></asp:Label>
            </div>
            <div class="dataBlock-body">
                <table>
                    <tr>
                        <td class="number">
                            <asp:Label runat="server" ID="lblAppointedIONameRow_v2" Text="A" />
                        </td>
                        <td class="label">
                            Name:
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblAppointedIOName_v2" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label runat="server" ID="lblAppointedIORankRow_v2" Text="B" />
                        </td>
                        <td class="label">
                            Rank:
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblAppointedIORank_v2" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label runat="server" ID="lblAppointedIOEDIPIRow_v2" Text="C" />
                        </td>
                        <td class="label labelRequired">
                            EDIPI:
                        </td>
                        <td>
                            <asp:TextBox Enabled="true" runat="server" Width="280px" ID="txtAppointedIOEDIPI_v2" />
                            <asp:Label runat="server" ID="lblAppointedIOEDIPI_v2" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label runat="server" ID="lblAppointedIOUnitRow_v2" Text="D" />
                        </td>
                        <td class="label labelRequired">
                            Unit:
                        </td>
                        <td>
                            <asp:TextBox Enabled="false" runat="server" Width="280px" ID="txtAppointedIOUnit_v2" />
                            <asp:Button runat="server" ID="btnChangeUnit_v2" Text="Change" />
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1_v2" SetFocusOnError="True" runat="server"
                                    ErrorMessage="Unit Required." ControlToValidate="lblNewUnitID" ValidationGroup="input" Display="Dynamic">*</asp:RequiredFieldValidator>
                            <asp:TextBox ID="lblNewUnitID_v2" CssClass="hidden" runat="server"></asp:TextBox>

                            <asp:Label runat="server" ID="lblAppointedIOUnit_v2" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label runat="server" ID="lblAppointedIOComponentRow_v2" Text="E" />
                        </td>
                        <td class="label">
                            Component:
                        </td>
                        <td>
                            <asp:Label runat="server" ID="lblAppointedIOComponent_v2" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label runat="server" ID="lblAppointedIOIATrainingRow_v2" Text="F" />
                        </td>
                        <td class="label labelRequired">
                            <asp:Label runat="server" ID="lblAppointedIOIATrainingRowText_v2" Text="IA Training Expiration Date:" ToolTip="The date one year from the IA Training date of completion." />
                        </td>
                        <td>
                            <asp:TextBox ID="txtAppointedIOIATraining_v2" onchange="DateCheck(this);" runat="server" Width="80" MaxLength="10" CssClass="datePicker" />
                            <asp:Label runat="server" ID="lblAppointedIOIATraining_v2" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label runat="server" ID="lblAppointedIOEmailRow_v2" Text="G" />
                        </td>
                        <td class="label labelRequired">
                            Work Email Address:
                        </td>
                        <td>
                            <asp:TextBox runat="server" ID="txtAppointedIOEmail_v2" Width="280px" MaxLength="150" Visible="False" />
                            <asp:Label runat="server" ID="lblAppointedIOEmail_v2" Visible="False" />
                        </td>
                        <td>
                            <asp:RequiredFieldValidator runat="server" ID="rqfdAppointedIOEmail_v2" ControlToValidate="txtAppointedIOEmail_v2" SetFocusOnError="True" ErrorMessage="Email required" ValidationGroup="SetAppointedIO" Display="Dynamic">
                                Please Enter Work Email
                            </asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator runat="server" ID="rgExpAppointedIOEmail_v2" ControlToValidate="txtAppointedIOEmail_v2" ErrorMessage="Invalid Work Email." ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ValidationGroup="SetAppointedIO">
                                Invalid Work Email
                            </asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr runat="server" id="trAppointedIOError_v2" visible="false">
                        <td class="number">
                            <asp:Label runat="server" ID="lblAppointedIOErrors_v2" Text="H" />
                        </td>
                        <td class="label">
                            ERRORS:
                        </td>
                        <td>
                            <asp:BulletedList runat="server" ID="bllAppointedIOErrors_v2" CssClass="labelRequired" />
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>

    </asp:Panel>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">

    <script type="text/javascript">

        $(document).ready(function() {

            $('.datePicker').datepicker(calendarPick("TomorrowFuture", "<%=CalendarImage %>"));
        });
    </script>

</asp:Content>
