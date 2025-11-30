<%@ Page Title="" Language="C#" MasterPageFile="~/Secure/SC_Incap/SC_Incap.master" MaintainScrollPositionOnPostback="true" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.IN.Secure_sc_in_MedOff" CodeBehind="MedOff.aspx.cs" %>

<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_Incap/SC_Incap.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>

<asp:Content runat="Server" ID="Content1" ContentPlaceHolderID="HeaderNested">
    <style type="text/css">
        .FindingsIndent 
        {
            margin-left: 15px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    <asp:Panel runat="server" ID="pnlIncapPMInformation" CssClass="dataBlock">
        <div class="dataBlock-header">
            INCAP PM
        </div>
        <div class="dataBlock-body">
            <table>
                <tr>
                        <td class="number">A
                        </td>
                        <td class="label labelRequired">Initial:
                        </td>
                        <td class="value">Start Date:
                            <asp:TextBox ID="InitialStartDate_Date" MaxLength="10" onchange="DateCheck(this);" runat="server" Width="80" />
                            <asp:TextBox ID="InitialStartDate_Time" MaxLength="4" onchange="TimeCheck(this);" runat="server" Width="40" />
                            <asp:Label ID="InitialStartDatelbl" runat="server" CssClass="lblDisableText" />

                            &nbsp;&nbsp;

                            End Date:
                            <asp:TextBox ID="InitialEndDate_Date" MaxLength="10" onchange="DateCheck(this);" runat="server" Width="80" />
                            <asp:TextBox ID="InitialEndDate_Time" MaxLength="4" onchange="TimeCheck(this);" runat="server" Width="40" />
                            <asp:Label ID="InitialEndDatelbl" runat="server" CssClass="lblDisableText" />
                        </td>
                </tr>
                <tr runat="server" id="PMLateSubmissionSection">
                        <td class="number">B
                        </td>
                        <td class="label labelRequired">Is this a late submission:
                        </td>
                        <td class="value">
                            <asp:RadioButtonList runat="server" ID="rblLateSubmission" RepeatDirection="Horizontal" Enabled="false">
                                        <asp:ListItem Value="1">Yes</asp:ListItem>
                                        <asp:ListItem Value="0">No</asp:ListItem>
                                    </asp:RadioButtonList>
                        </td>
                </tr>
                
                <tr runat="server" id="PMAppealOrCompleteSection" visible="false">
                        <td class="number">C
                        </td>
                        <td class="label labelRequired">Does member want to appeal or complete this case:
                        </td>
                        <td class="value">
                            <asp:RadioButtonList runat="server" ID="rblAppealorComplete" RepeatDirection="Horizontal" >
                                        <asp:ListItem Value="1">Appeal</asp:ListItem>
                                        <asp:ListItem Value="0">Complete</asp:ListItem>
                                    </asp:RadioButtonList>
                        </td>
                </tr>
                <tr runat="server" id="PMExtNumSection" visible="false">
                    <td class="number">C
                        </td>
                        <td class="label labelRequired">Extension Number:
                        </td>
                    <td>
                    <asp:Label runat="server" ID="txtExtCount" Enabled="false">
                                    </asp:Label>
                    </td>
                    </tr>
                <tr runat="server" id="PMExtDateSection" visible="false">
                        <td class="number">D
                        </td>
                        <td class="label labelRequired">Extension:
                        </td>
                        <td class="value">Start Date:
                            <asp:TextBox ID="ExtensionStartDate_Date" MaxLength="10" onchange="DateCheck(this);" runat="server" Width="80" />
                            <asp:TextBox ID="ExtensionStartDate_Time" MaxLength="4" onchange="TimeCheck(this);" runat="server" Width="40" />
                            <asp:Label ID="ExtensionStartDatelbl" runat="server" CssClass="lblDisableText" />

                            &nbsp;&nbsp;

                            End Date:
                            <asp:TextBox ID="ExtensionEndDate_Date" MaxLength="10" onchange="DateCheck(this);" runat="server" Width="80" />
                            <asp:TextBox ID="ExtensionEndDate_Time" MaxLength="4" onchange="TimeCheck(this);" runat="server" Width="40" />
                            <asp:Label ID="ExtensionEndDatelbl" runat="server" CssClass="lblDisableText" />
                        </td>
                   
                </tr>
                <tr runat="server" id="PMExtOrCompleteSection" visible="false">
                        <td class="number"  runat="server" id="PMExtOrCompleteTitle">C
                        </td>
                        <td class="label labelRequired">Does member want to extend or complete this case:
                        </td>
                        <td class="value">
                            <asp:RadioButtonList runat="server" ID="rblExtorComplete" RepeatDirection="Horizontal" >
                                        <asp:ListItem Value="1">Extend</asp:ListItem>
                                        <asp:ListItem Value="0">Complete</asp:ListItem>
                                    </asp:RadioButtonList>
                        </td>
                </tr>
                <tr>
                    <td>
                        <br />
                    </td>
                </tr>
            </table>  
        </div>      
    </asp:Panel>
    
    <asp:Panel runat="server" Id="pnlMedTechInformation" CssClass="dataBlock">
        <div class="dataBlock-header">
            Medical Input
        </div>
        <div class="dataBlock-body">
            <table>
                <tr>
                    <%--<td class="number">Initial
                        </td>
                    <td class="label labelRequired">
                        </td>--%>
                    
                </tr>
                <%--<tr>
                    <td class="number">A
                        </td>
                        <td class="label labelRequired">AF Form 469, Duty Limiting Condition Report:
                        </td>
                    <td>
                    <asp:RadioButtonList runat="server" ID="rblMedRevreport" RepeatDirection="Horizontal" Enabled="false" >
                                        <asp:ListItem Value="31">AAC 31</asp:ListItem>
                                        <asp:ListItem Value="37">AAC 37</asp:ListItem>
                                    </asp:RadioButtonList>
                    </td>
                    </tr>--%>
                 <tr runat="server" id="MedAvailabilityCodeSection" visible="false">
                        <td class="number">A
                        </td>
                        <td class="label labelRequired">What is the current assignment availability code:
                        </td>
                    <td>
                    <asp:DropDownList runat="server" ID="ddlMedAvailabilityCode" RepeatDirection="Horizontal"  Enabled="false">
                                    </asp:DropDownList>
                        </td>
                </tr>
                    <tr>
                        <td class="number">B
                        </td>
                        <td class="label labelRequired">Is member able to perform military duties:
                        </td>
                    <td>
                        <asp:UpdatePanel runat="server" ID="UpdatePanel1">
                            <ContentTemplate>
                    <asp:RadioButtonList runat="server" ID="rblMedRevDecision" RepeatDirection="Horizontal" AutoPostBack="True" Enabled="false">
                                        <asp:ListItem Value="1">Yes</asp:ListItem>
                                        <asp:ListItem Value="0">No</asp:ListItem>
                                    </asp:RadioButtonList>
                                 </ContentTemplate>
                        </asp:UpdatePanel>
                    </td>
                </tr>
               
                <tr runat="server" id="MedAMROSection" visible="false">
                        <td class="number">D
                        </td>
                        <td class="label labelRequired">When was the last AMRO review:
                        </td>
                        <td class="value">Start Date:
                            <asp:TextBox ID="AMROStartDate_Date" MaxLength="10" onchange="DateCheck(this);" runat="server" Width="80" Enabled="false"/>
                            <asp:TextBox ID="AMROStartDate_Time" MaxLength="4" onchange="TimeCheck(this);" runat="server" Width="40" Enabled="false"/>
                            <asp:Label ID="Label1" runat="server" CssClass="lblDisableText" />

                            &nbsp;&nbsp;

                            End Date:
                            <asp:TextBox ID="AMROEndDate_Date" MaxLength="10" onchange="DateCheck(this);" runat="server" Width="80" Enabled="false"/>
                            <asp:TextBox ID="AMROEndDate_Time" MaxLength="4" onchange="TimeCheck(this);" runat="server" Width="40" Enabled="false"/>
                            <asp:Label ID="Label2" runat="server" CssClass="lblDisableText" />
                        </td>
                </tr>
                <tr runat="server" id="MedDispositionSection" visible="false">
                        <td class="number">E
                        </td>
                        <td class="label labelRequired">What was the disposition at that review:
                        </td>
                    <td>
                    <asp:DropDownList runat="server" ID="ddlMedDisposition" RepeatDirection="Horizontal" Enabled="false" >
                                    </asp:DropDownList>
                        </td>
                </tr>
                <tr runat="server" id="MedNextAMROSection" visible="false">
                        <td class="number">F
                        </td>
                        <td class="label labelRequired">When is the next ARMO review scheduled:
                        </td>
                        <td class="value">Start Date:
                            <asp:TextBox ID="NextAMROStartDate_Date" MaxLength="10" onchange="DateCheck(this);" runat="server" Width="80" Enabled="false"/>
                            <asp:TextBox ID="NextAMROStartDate_Time" MaxLength="4" onchange="TimeCheck(this);" runat="server" Width="40" Enabled="false"/>
                            <asp:Label ID="Label3" runat="server" CssClass="lblDisableText" />

                            &nbsp;&nbsp;

                            End Date:
                            <asp:TextBox ID="NextAMROEndDate_Date" MaxLength="10" onchange="DateCheck(this);" runat="server" Width="80" Enabled="false"/>
                            <asp:TextBox ID="NextAMROEndDate_Time" MaxLength="4" onchange="TimeCheck(this);" runat="server" Width="40" Enabled="false"/>
                            <asp:Label ID="Label4" runat="server" CssClass="lblDisableText" />
                        </td>
                </tr>
                <tr runat="server" id="MedIRILOSection" visible="false">
                        <td class="number">G
                        </td>
                        <td class="label labelRequired">If I-RILO (Pre-IDES) has been identified as required what is the current status:
                        </td>
                    <td>
                    <asp:DropDownList runat="server" ID="ddlMedIRILO" RepeatDirection="Horizontal"  Enabled="false">
                                    </asp:DropDownList>
                    </td>
                </tr>
                <tr runat="server" id="MedExtRecommendationSection" visible="false">
                        <td class="number">H
                        </td>
                        <td class="label labelRequired">Extention Recommendation:
                        </td>
                    <td>
                    <asp:RadioButtonList runat="server" ID="rblMedExtRecommendation" RepeatDirection="Horizontal" Enabled="false" >
                                        <asp:ListItem Value="1">Approval</asp:ListItem>
                                        <asp:ListItem Value="0">Disapproval</asp:ListItem>
                                    </asp:RadioButtonList>
                        </td>
                </tr>
                <tr>
                    <td>
                        <br/>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <asp:Panel runat="server" Id="pnlICInformation" CssClass="dataBlock">
        <div class="dataBlock-header">
            Immediate Commander
        </div>
        <div class="dataBlock-body">
            <table>
                <tr runat="server" id="ICINSection" visible="false">
                    <td class="number">A
                        </td>
                        <td class="label labelRequired">Recommendation:
                        </td>
                    <td>
                    <asp:RadioButtonList runat="server" ID="rblICINRecommendation" RepeatDirection="Horizontal" Enabled="false" >
                                        <asp:ListItem Value="1">Approval</asp:ListItem>
                                        <asp:ListItem Value="0">Disapproval</asp:ListItem>
                                    </asp:RadioButtonList>
                    </td>
                    </tr>
                <tr runat="server" id="ICExtSection" visible="false">
                    <td class="number">A
                        </td>
                        <td class="label labelRequired">Extention Recommendation:
                        </td>
                    <td>
                    <asp:RadioButtonList runat="server" ID="rblICExtRecommendation" RepeatDirection="Horizontal" Enabled="false" >
                                        <asp:ListItem Value="1">Approval</asp:ListItem>
                                        <asp:ListItem Value="0">Disapproval</asp:ListItem>
                                    </asp:RadioButtonList>
                    </td>
                    </tr>
                    <%--<tr>
                        <td class="number">B
                        </td>
                        <td class="label labelRequired">Type:
                        </td>
                    <td>
                    <asp:Label id="ICTypeText" runat="server"/>
                </tr>--%>
                <tr>
                    <td>
                        <br/>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
    <asp:Panel runat="server" Id="pnlWJAInformation" CssClass="dataBlock">
        <div class="dataBlock-header">
            Wing JA
        </div>
        <div class="dataBlock-body">
            <table>
                <tr>
                    <td class="number">A
                        </td>
                        <td class="label labelRequired" runat="server">Concur or Nonconcur with Immediate Commander:
                        </td>
                    <td>
                    <asp:RadioButtonList runat="server" ID="rblWJAConcur" RepeatDirection="Horizontal" Enabled="false" >
                                        <asp:ListItem Value="1">Concur</asp:ListItem>
                                        <asp:ListItem Value="0">Nonconcur</asp:ListItem>
                                    </asp:RadioButtonList>
                    </td>
                    </tr>
                <tr>
                    <td>
                        <br/>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
        <asp:Panel runat="server" Id="pnlFinanceInformation" CssClass="dataBlock">
        <div class="dataBlock-header">
            Finance
        </div>
        <div class="dataBlock-body">
            <table>
                <tr runat="server" id="FinIntSection" visible="false">
                    <td class="number">A
                        </td>
                        <td class="label labelRequired">Member has loss of earned income:
                        </td>
                    <td>
                    <asp:RadioButtonList runat="server" ID="rblFinLossEarning" RepeatDirection="Horizontal" Enabled="false" >
                                        <asp:ListItem Value="1">Yes</asp:ListItem>
                                        <asp:ListItem Value="0">No</asp:ListItem>
                                    </asp:RadioButtonList>
                    </td>
                    </tr>
                <tr runat="server" id="FinSelfEmployedSection" visible="false">
                    <td class="number" id="FinSelfEmployedSection_text" runat="server">B
                        </td>
                        <td class="label labelRequired" runat="server">Is the member self employed:
                        </td>
                    <td>
                    <asp:RadioButtonList runat="server" ID="rblFinSelfEmployed" RepeatDirection="Horizontal" Enabled="false" >
                                       <asp:ListItem Value="1">Yes</asp:ListItem>
                                        <asp:ListItem Value="0">No</asp:ListItem>
                                    </asp:RadioButtonList>
                    </td>
                    </tr>
                <tr runat="server" id="FinExtSection" visible="false">
                    <td class="number">B
                        </td>
                        <td class="label labelRequired" runat="server">Is the member still experiencing loss of earned income:
                        </td>
                    <td>
                    <asp:RadioButtonList runat="server" ID="rblFinExtLossEarning" RepeatDirection="Horizontal" Enabled="false" >
                                       <asp:ListItem Value="1">Yes</asp:ListItem>
                                        <asp:ListItem Value="0">No</asp:ListItem>
                                    </asp:RadioButtonList>
                    </td>
                    </tr>
                <tr>
                    <td>
                        <br/>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
        
        <asp:Panel runat="server" Id="pnlWCCInformation" CssClass="dataBlock">
        <div class="dataBlock-header">
            Wing Commander
        </div>
        <div class="dataBlock-body">
            <table>
                <tr runat="server" id="WingCC_INTSection" visible="false">
                    <td class="number">A
                        </td>
                        <td class="label labelRequired">Initial INCAP Pay:
                        </td>
                    <td>
                    <asp:RadioButtonList runat="server" ID="rblWCC_INPAY" RepeatDirection="Horizontal" Enabled="false" >
                                        <asp:ListItem Value="1">Approve</asp:ListItem>
                                        <asp:ListItem Value="0">Disapprove</asp:ListItem>
                                    </asp:RadioButtonList>
                    </td>
                    </tr>
                <tr runat="server" id="WingCC_EXTSection" visible="false">
                    <td class="number">A
                        </td>
                        <td class="label labelRequired">INCAP Pay Extension:
                        </td>
                    <td>
                    <asp:RadioButtonList runat="server" ID="rblWCC_EXTPAY" RepeatDirection="Horizontal" Enabled="false" >
                                        <asp:ListItem Value="1">Approve</asp:ListItem>
                                        <asp:ListItem Value="0">Disapprove</asp:ListItem>
                                    </asp:RadioButtonList>
                    </td>
                    </tr>
                 <tr runat="server" id="WingCC_AppealSection" visible="false">
                    <td class="number">B
                        </td>
                        <td class="label labelRequired">If Wing CC's Initial INCAP PAY Dissapproval is Appealed:
                        </td>
                    <td>
                    <asp:RadioButtonList runat="server" ID="rblWCC_AppealPAY" RepeatDirection="Horizontal" Enabled="false" >
                                        <asp:ListItem Value="1">Approve</asp:ListItem>
                                        <asp:ListItem Value="0">Disapprove</asp:ListItem>
                                    </asp:RadioButtonList>
                    </td>
                    </tr>
                <tr>
                    <td>
                        <br/>
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
     <input type="hidden" id="page_readOnly" runat="Server" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">
   <script type="text/javascript">
        var isReadOnly = false;

        $(function () {

           isReadOnly = element('<%= page_readOnly.ClientId %>').value == "0";

           $('.datePicker').datepicker(calendarPick("Past", "<%=CalendarImage %>"));

           $('.datePickerFuture').datepicker(calendarPick("All", "<%=CalendarImage%>"));

           $('.datePickerPlusFuture').datepicker(calendarPick("TomorrowFuture", "<%=CalendarImage %>"));

        });

   </script>
    <script type="text/javascript">
        function alertMessage(y) {
            alert(y);
        }
    </script>
</asp:Content>
