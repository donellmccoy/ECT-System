<%@ Page Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Secure_Welcome" Title="Untitled Page" Codebehind="Welcome.aspx.vb" %>

<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="head">
    <style type="text/css">
        .msg-title
        {
            font-weight: bold;
        }
        .msg-byline
        {
            font-style: italic;
        }
        .error-panel
        {
            border: 1px solid red;
            text-align: center;
            width: 600px;
            margin-left: auto;
            margin-right: auto;
            margin-bottom: 20px;
            padding: 6px;
            color: #000;
            background-color: #FFB9AD;
            font-weight: bold;
            font-size: 14px;
        }
        .msg-panel
        {
            background-color: #FFED9E;
            font-weight: normal;
            font-size: 14px;
            background-image: none;
        }
        .survey-panel
        {
            background-color: #FFED9E;
            font-weight: normal;
            font-size: 14px;
            font-style:italic;
            background-image: none;
            text-align:center;
        }		
        .minHeight
        {
            min-height:90px;
        }
        .auto-style1 {
            height: 23px;
        }
    </style>
    <script type="text/javascript">
        function viewDoc(url) {
            showPopup({
                'Url': url,
                'Width': 642,
                'Height': 668,
                'Center': true,
                'Resizable': true,
                'ScrollBars': true
            });
        }
    </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <div class="indent">
        <asp:Panel runat="server" ID="pnlWelcomeBanner" Visible="false" CssClass="survey-panel">
            <asp:Label runat="server" ID="lblWelcomeBannerMessage" />                
        </asp:Panel>
        <asp:Panel runat="server" ID="ErrorPanel" Visible="false" CssClass="ui-state-error info-block">
            <asp:Image runat="server" ID="Image1" ImageAlign="AbsMiddle" ImageUrl="~/images/warning.gif" />
            <asp:Label runat="server" ID="ErrorMessageLabel" />
        </asp:Panel>
        <asp:Panel runat="server" ID="FeedbackPanel" Visible="false" CssClass="ui-state-highlight info-block msg-panel">
            <asp:Label runat="server" ID="FeedbackMessageLabel" />
        </asp:Panel>
    </div>
    <br/>
    <!-- Left panel -->
    <div style="display:flex">
    <div style="width: 49%; float: left;">
        <!-- Message board -->
            <%--<div class="section minHeight">
            <div class="sectionHeader">
                System Messages</div>
            <div class="sectionBody">
                <asp:Panel runat="server" ID="NoMessagesPanel" CssClass="emptyItem" Visible="false">
                        No Messages
                    </asp:Panel>
                <asp:Repeater ID="rptMessages" runat="server">
                    <HeaderTemplate>
                        <table>
                            <tbody>
                    </HeaderTemplate>
                    <FooterTemplate>
                        </tbody> </table>
                    </FooterTemplate>
                    <SeparatorTemplate>
                        <tr>
                            <td colspan="2">
                                &nbsp;
                            </td>
                        </tr>
                    </SeparatorTemplate>
                    <ItemTemplate>
                        <tr>
                            <td>
                                <span class="msg-title">&bull;
                                    <%# DataBinder.Eval(Container.DataItem, "Title") %></span>
                            </td>
                            <td style="text-align: right;">
                                <%#DataBinder.Eval(Container.DataItem, "StartTime", "{0:MM/dd/yyyy}")%>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <span class="msg-byline">&nbsp;&nbsp; - posted by
                                    <%# DataBinder.Eval(Container.DataItem, "Name") %></span>
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2" style="border-top: 1px solid #C0C0C0; padding-top: 4px;">
                                <%# DataBinder.Eval(Container.DataItem, "Message") %>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
        </div>--%>

        <div class="section minHeight" ID="NoMessagesPanel" runat="server">
            <div class="sectionHeader">
                System Messages

            </div>
            <div class="sectionBody">
                <asp:Panel runat="server" ID="Panel1" CssClass="emptyItem">
                        No Messages
                </asp:Panel>
            </div>
       </div>

        <asp:Repeater ID="rptMessages" runat="server">
            <ItemTemplate>
                <div class="section minHeight" ID="NoMessagesPanel" runat="server">
                    <div class="sectionHeader msg-title"> System Message: 
                        <%# DataBinder.Eval(Container.DataItem, "Title") %>
                    </div>
                    <div class="sectionBody">
                        <table style="width: 100%; border-collapse:collapse;">
                            <tr>
                                <td >
                                    <span class="msg-byline">&nbsp;&nbsp; posted by
                                        <%# DataBinder.Eval(Container.DataItem, "Name") %></span>
                                </td>

                                <td style="text-align: right;">
                                    <%#DataBinder.Eval(Container.DataItem, "StartTime", "{0:MM/dd/yyyy}")%>
                                </td>
                            </tr>
                            <tr style="border-top: 1px solid #C0C0C0;">
                                <td>

                                    <%# DataBinder.Eval(Container.DataItem, "Message") %>
                                </td>
                            </tr>
                        </table>
                    </div>
                </div>
                        
            </ItemTemplate>
       </asp:Repeater>

    </div>
    <!-- Right panel -->
    <div style="width: 49%; float: left; position: relative;">
        <!-- Line of Duty -->
            <asp:Panel runat="server" CssClass="section minHeight" ID="LodPanel" visible="false">
            <div class="sectionHeader">
                Line of Duty</div>
            <div class="sectionBody">
                <table>
                    <tr runat="server" id="MyLodLink" visible="false">
                        <td>
                            &bull;&nbsp;<a href="lod/MyLods.aspx">Review and Act on My LODs</a>&nbsp;[<asp:Label
                                runat="server" ID="PendingLodCount">0</asp:Label>]
                        </td>
                    </tr>
                    <tr runat="server" id="CompletedLodLink" visible="false">
                        <td>
                            &bull;&nbsp;<a href="lod/PostCompletionLOD.aspx">Process Completed Cases</a>&nbsp;[<asp:Label
                                runat="server" ID="CompletedLodCount">0</asp:Label>]
                        </td>
                    </tr>
                    <tr runat="server" id="AppointingAuthority_P_Link" visible="false">
                        <td>
                            &bull;&nbsp;<a href="lod/MyLods.aspx">LODs awaiting ARC SME Consult</a>&nbsp;[<asp:Label
                                runat="server" ID="AppointingAuthority_P_Count">0</asp:Label>]
                        </td>
                    </tr>
                    <tr runat="server" id="Consult_Link" visible="false">
                        <td class="auto-style1">
                            &bull;&nbsp;
                            <asp:LinkButton ID="Consult_LinkButton" runat="server">LODs awaiting Consult </asp:LinkButton>[<asp:Label
                                runat="server" ID="Consult_Count">0</asp:Label>]
                            
                        </td>
                    </tr>

                    <tr runat="server" id="Audit_Link" visible="false">
                        <td>
                            &bull;&nbsp;
                            <asp:LinkButton ID="Audit_LinkButton" runat="server">LODs awaiting Audit </asp:LinkButton>[<asp:Label
                                runat="server" ID="Audit_Count">0</asp:Label>]
                        </td>
                    </tr>
                  <tr runat="server" id="UnitCC_Link" visible="false">
                        <td>
                            &bull;&nbsp;<a href="lod/MyLods.aspx">Process Completed LODs Audit</a>&nbsp;[<asp:Label
                                runat="server" ID="UnitCC_Count">0</asp:Label>]
                        </td>
                    </tr>
                    
                    
                    <tr runat="server" id="AppealLink" visible="false">
                        <td>
                            &bull;&nbsp;<a href="AppealRequests/search.aspx">Appeal Request (AP)</a>&nbsp;
                        </td>
                    </tr>
                    <tr runat="server" id="NewLodLink" visible="false">
                        <td>
                            &bull;&nbsp;<a href="lod/start.aspx">Start New LOD</a>
                        </td>
                    </tr>
                    <tr runat="server" id="ReinvestigateLink" visible="false">
                        <td>
                            &bull;&nbsp;<a href="ReinvestigationRequests/MyRequests.aspx">LOD Reinvestigation Requests</a>&nbsp;[<asp:Label
                                runat="server" ID="ReinvestigatedLodCount">0</asp:Label>]
                        </td>
                    </tr>
                    <tr runat="server" id="SpecialCaseLink_MMSO" visible="false">
                        <td>
                            &bull;&nbsp;<a href="SpecialCases/MyCases.aspx"><asp:Literal ID="SpecCaseTitle" runat="server" /></a>&nbsp;[<asp:Label
                                runat="server" ID="SpecialCaseCount">0</asp:Label>]
                        </td>
                    </tr>
                    <tr runat="server" id="MyAppealLink" visible="false">
                        <td>
                            &bull;&nbsp;<a href="AppealRequests/MyRequests.aspx">Review and Act on My Appeals</a>&nbsp;[<asp:Label
                                runat="server" ID="PendingAppealCount">0</asp:Label>]
                        </td>
                    </tr>
                    <tr runat="server" id="CompletedAppealLink" visible="false">
                        <td>
                            &bull;&nbsp;<a href="AppealRequests/PostCompletionAppeal.aspx">Process Completed Appeals</a>&nbsp;[<asp:Label
                                runat="server" ID="CompletedAppealCount">0</asp:Label>]
                        </td>
                    </tr>
                    <tr runat="server" id="ReinvestigationLink" visible="false">
                        <td>
                            &bull;&nbsp;<a href="ReinvestigationRequests/search.aspx">Reinvestigation Request (RR)</a>&nbsp;
                        </td>
                    </tr>
                    <tr runat="server" id="trRestrictedSARCsLink" visible="false">
                        <td>
                            &bull;
                            <a href="SARC/MyCases.aspx">Review and Act on My Restricted SARCs</a>
                            [<asp:Label runat="server" ID="lblRestrictedSARCsCount">0</asp:Label>]
                        </td>
                    </tr>
                    <tr runat="server" id="trComletedRestrictedSARCsLink" visible="false">
                        <td>
                            &bull;
                            <a href="SARC/PostCompletion.aspx">Process Completed Restricted SARCs</a>
                            [<asp:Label runat="server" ID="lblCompletedRestrictedSARCsCount">0</asp:Label>]
                        </td>
                    </tr>
                    <tr runat="server" id="trSARCAppealsLink" visible="false">
                        <td>
                            &bull;
                            <a href="SARC_Appeal/MyRequests.aspx">Review and Act on My SARC Appeals</a>
                            [<asp:Label runat="server" ID="lblSARCAppealsCount">0</asp:Label>]
                        </td>
                    </tr>
                    <tr runat="server" id="trComletedSARCAppealsLink" visible="false">
                        <td>
                            &bull;
                            <a href="SARC_Appeal/PostCompletionSARCAppeal.aspx">Process Completed SARC Appeals</a>
                            [<asp:Label runat="server" ID="lblCompletedSARCAppealssCount">0</asp:Label>]
                        </td>
                    </tr>
                    <tr runat="server" id="SearchLodLink" visible="false">
                        <td>
                                LOD Search:&nbsp;
                            <asp:TextBox runat="server" ID="txtLodSearch" Width="150" />&nbsp;
                            <asp:Button runat="server" ID="btnLodSearch" Text="Go" />
                        </td>
                    </tr>
                    
                </table>
            </div>
        </asp:Panel>
        <!-- Special Cases -->
        <asp:Panel runat="server" CssClass="section" ID="SpecCasePanel" visible="false">
            <div class="sectionHeader">
                Other/Special Cases</div>
            <div class="sectionBody">
                <table>
                    <tr runat="server" id="SpecialCaseLink_BCMR" visible="false">
                        <td>
                            &bull;&nbsp;<a href="SC_BCMR/MyCases.aspx">Board for Correction on Military Records (BCMR)</a>&nbsp;[<asp:Label
                                runat="server" ID="SpecialCaseCount_BCMR">0</asp:Label>]
                        </td>
                    </tr>
                    <tr runat="server" id="BCMR_search" visible="false">
                        <td>
                            &bull;&nbsp;<a href="SpecialCases/search.aspx?mid=12">Board for Correction on Military Records (BCMR)</a>&nbsp;
                        </td>
                    </tr>


                    <tr runat="server" id="SpecialCaseLink_BMT" visible="false">
                        <td>
                            &bull;&nbsp;<a href="SC_BMT/MyCases.aspx">Basic Military Training Waivers (BMT)/Military Entrance Processing Station (MEPS)</a>&nbsp;[<asp:Label
                                runat="server" ID="SpecialCaseCount_BMT">0</asp:Label>]
                        </td>
                    </tr>
                    <tr runat="server" id="BMT_search" visible="false">
                        <td>
                            &bull;&nbsp;<a href="SpecialCases/search.aspx?mid=8">Basic Military Training Waivers (BMT)/Military Entrance Processing Station (MEPS)</a>&nbsp;
                        </td>
                    </tr>


                    <tr runat="server" id="SpecialCaseLink_CMAS" visible="false">
                        <td>
                            &bull;&nbsp;<a href="SC_CMAS/MyCases.aspx">Command Man-day Allocation System (CMAS)</a>&nbsp;[<asp:Label
                                runat="server" ID="SpecialCaseCount_CMAS">0</asp:Label>]
                        </td>
                    </tr>
                    <tr runat="server" id="CMAS_search" visible="false">
                        <td>
                            &bull;&nbsp;<a href="SpecialCases/search.aspx?mid=14">Command Man-day Allocation System (CMAS)</a>&nbsp;
                        </td>
                    </tr>


                    <tr runat="server" id="SpecialCaseLink_Congress" visible="false">
                        <td>
                            &bull;&nbsp;<a href="SC_Congress/MyCases.aspx">Congressionals (CI)</a>&nbsp;[<asp:Label
                                runat="server" ID="SpecialCaseCount_Congress">0</asp:Label>]
                        </td>
                    </tr>
                    <tr runat="server" id="CI_search" visible="false">
                        <td>
                            &bull;&nbsp;<a href="SpecialCases/search.aspx?mid=7">Congressionals (CI)</a>&nbsp;
                        </td>
                    </tr>


                    <tr runat="server" id="SpecialCaseLink_DW" visible="false">
                        <td>
                            &bull;&nbsp;<a href="SC_DW/MyCases.aspx">Deployment Waivers (DW)</a>&nbsp;[<asp:Label
                                runat="server" ID="SpecialCaseCount_DW">0</asp:Label>]
                        </td>
                    </tr>
                    <tr runat="server" id="DW_search" visible="false">
                        <td>
                            &bull;&nbsp;<a href="SpecialCases/search.aspx?mid=19">Deployment Waivers (DW)</a>&nbsp;
                        </td>
                    </tr>


                    <tr runat="server" id="SpecialCaseLink_FastTrack" visible="false">
                        <td>
                            &bull;&nbsp;<a href="SC_FastTrack/MyCases.aspx">Initial Review In Lieu Of (IRILO)</a>&nbsp;[<asp:Label
                                runat="server" ID="SpecialCaseCount_FastTrack">0</asp:Label>]
                        </td>
                    </tr>
                    <tr runat="server" id="IRILO_search" visible="false">
                        <td>
                            &bull;&nbsp;<a href="SpecialCases/search.aspx?mid=13">Initial Review In Lieu Of (IRILO)</a>&nbsp;
                        </td>
                    </tr>


                    <tr runat="server" id="SpecialCaseLink_Incap" visible="false">
                        <td>
                            &bull;&nbsp;<a href="SC_Incap/MyCases.aspx">Incapacitation (INCAP)</a>&nbsp;[<asp:Label
                                runat="server" ID="SpecialCaseCount_Incap">0</asp:Label>]
                        </td>
                    </tr>
                    <tr runat="server" id="Incap_search" visible="false">
                        <td>
                            &bull;&nbsp;<a href="SpecialCases/search.aspx?mid=6">Incapacitation (INCAP)</a>&nbsp;
                        </td>
                    </tr>


                    <tr runat="server" id="SpecialCaseLink_MEB" visible="false">
                        <td>
                            &bull;&nbsp;<a href="SC_MEB/MyCases.aspx">Medical Evaluation Board (MEB)</a>&nbsp;
                            [<asp:Label runat="server" ID="SpecialCaseCount_MEB">0</asp:Label>]
                        </td>
                    </tr>
                    <tr runat="server" id="MB_search" visible="false">
                        <td>
                            &bull;&nbsp;<a href="SpecialCases/search.aspx?mid=11">Medical Evaluation Board (MEB)</a>&nbsp;
                        </td>
                    </tr>


                    <tr runat="server" id="SpecialCaseLink_MH" visible="false">
                        <td>
                            &bull;&nbsp;<a href="SC_MH/MyCases.aspx">Medical Holds (MH)</a>&nbsp;
                            [<asp:Label runat="server" ID="SpecialCaseCount_MH">0</asp:Label>]
                        </td>
                    </tr>
                    <tr runat="server" id="MH_search" visible="false">
                        <td>
                            &bull;&nbsp;<a href="SpecialCases/search.aspx?mid=17">Medical Holds (MH)</a>&nbsp;
                        </td>
                    </tr>


                    <tr runat="server" id="SpecialCaseLink_MO" visible="false">
                        <td>
                            &bull;&nbsp;<a href="SC_MO/MyCases.aspx">Modifications (MO)</a>&nbsp;[<asp:Label
                                runat="server" ID="SpecialCaseCount_MO">0</asp:Label>]
                        </td>
                    </tr>
                    <tr runat="server" id="MO_search" visible="false">
                        <td>
                            &bull;&nbsp;<a href="SpecialCases/search.aspx?mid=20">Modifications (MO)</a>&nbsp;
                        </td>
                    </tr>


                    <tr runat="server" id="SpecialCaseLink_NE" visible="false">
                        <td>
                            &bull;&nbsp;<a href="SC_NE/MyCases.aspx">Non Emergent Surgery Requests (NE)</a>&nbsp;[<asp:Label
                                runat="server" ID="SpecialCaseCount_NE">0</asp:Label>]
                        </td>
                    </tr>
                    <tr runat="server" id="NE_search" visible="false">
                        <td>
                            &bull;&nbsp;<a href="SpecialCases/search.aspx?mid=18">Non Emergent Surgery Requests (NE)</a>&nbsp;
                        </td>
                    </tr>


                    <tr runat="server" id="SpecialCaseLink_PWaivers" visible="false">
                        <td>
                            &bull;&nbsp;<a href="SC_PWaivers/MyCases.aspx">Participation Waiver (PWaiver)</a>&nbsp;[<asp:Label
                                runat="server" ID="SpecialCaseCount_PWaivers">0</asp:Label>]
                        </td>
                    </tr>
                    <tr runat="server" id="PW_search" visible="false">
                        <td>
                            &bull;&nbsp;<a href="SpecialCases/search.aspx?mid=10">Participation Waiver (PWaiver)</a>&nbsp;
                        </td>
                    </tr>

                    <tr runat="server" id="SpecialCaseLink_AGRCert" visible="false">
                        <td>
                            &bull;&nbsp;<a href="SC_AGRCert/MyCases.aspx">AGR Medical Certification (AGRCert)</a>&nbsp;[<asp:Label
                                runat="server" ID="SpecialCaseCount_AGRCert">0</asp:Label>]
                        </td>
                    </tr>
                    <tr runat="server" id="AGRCertSearch" visible="false">
                        <td>
                            &bull;&nbsp;<a href="SpecialCases/search.aspx?mid=28">AGR Medical Certification (AGRCert)</a>&nbsp;
                        </td>
                    </tr>

                    <tr runat="server" id="SpecialCaseLink_PEPP" visible="false">
                        <td>
                            &bull;&nbsp;<a href="SC_PEPP/MyCases.aspx">Physical Examination Processing Program (PEPP)/AIMWITS (AW)</a>&nbsp;[<asp:Label
                                runat="server" ID="SpecialCaseCount_PEPP">0</asp:Label>]
                        </td>
                    </tr>
                    <tr runat="server" id="PE_search" visible="false">
                        <td>
                            &bull;&nbsp;<a href="SpecialCases/search.aspx?mid=21">Physical Examination Processing Program (PEPP)/AIMWITS (AW)</a>&nbsp;
                        </td>
                    </tr>


                    <tr runat="server" id="SpecialCaseLink_PH" visible="false">
                        <td>
                            &bull;&nbsp;<a href="SC_PH/MyCases.aspx">PH Non-Clinical Tracking (PH)</a>&nbsp;[<asp:Label
                                runat="server" ID="SpecialCaseCount_PH">0</asp:Label>]
                        </td>
                    </tr>
                    <tr runat="server" id="SpecialCasLink_StartNewPH" visible="false">
                        <td>
                            &bull;&nbsp;<a href="SC_PH/Start.aspx?mid=23">Start New PH Case</a>
                        </td>
                    </tr>


                    <tr runat="server" id="SpecialCaseLink_RS" visible="false">
                        <td>
                            &bull;&nbsp;<a href="SC_RS/MyCases.aspx">Recruiting Services (RS)</a>&nbsp;[<asp:Label
                                runat="server" ID="SpecialCaseCount_RS">0</asp:Label>]
                        </td>
                    </tr>
                    <tr runat="server" id="RS_search" visible="false">
                        <td>
                            &bull;&nbsp;<a href="SpecialCases/search.aspx?mid=22">Recruiting Services (RS)</a>&nbsp;
                        </td>
                    </tr>


                    <tr runat="server" id="SpecialCaseLink_RW" visible="false">
                        <td>
                            &bull;&nbsp;<a href="SC_RW/MyCases.aspx">Retention Waiver Renewal (RW)</a>&nbsp;[<asp:Label
                                runat="server" ID="SpecialCaseCount_RW">0</asp:Label>]
                        </td>
                    </tr>
                    <tr runat="server" id="RW_search" visible="false">
                        <td>
                            &bull;&nbsp;<a href="SpecialCases/search.aspx?mid=27">Retention Waiver Renewal (RW)</a>&nbsp;
                        </td>
                    </tr>


                    <tr runat="server" id="SpecialCaseLink_WWD" visible="false">
                        <td>
                            &bull;&nbsp;<a runat="server" id="SpecialCaseLink_WWD_hl" href="SC_WWD/MyCases.aspx">Worldwide Duty (WWD)</a>&nbsp;[<asp:Label
                                runat="server" ID="SpecialCaseCount_WWD">0</asp:Label>]
                        </td>
                    </tr>
                    <tr runat="server" id="WD_search" visible="false">
                        <td>
                            &bull;&nbsp;<a runat="server" id="WD_search_hl" href="SpecialCases/search.aspx?mid=9">Worldwide Duty (WWD)</a>&nbsp;
                        </td>
                    </tr>


                    <tr runat="server" id="SpecialCaseLink_PSCD" visible="false">
                        <td>
                            &bull;&nbsp;<a runat="server" id="SpecialCaseLink_PSCD_hl" href="SC_PSCD/MyCases.aspx">Prior Service Condition Determination (PSC-D)</a>&nbsp;[<asp:Label runat="server" ID="SpecialCaseCount_PSCD">0</asp:Label>]
                        </td>
                    </tr>
                    <tr runat="server" id="PSCD_search" visible="false">
                        <td>
                            &bull;&nbsp;<a runat="server" id="PSCD_search_hl" href="SpecialCases/search.aspx?mid=30">Prior Service Condition Determination (PSC-D)</a>&nbsp;
                        </td>
                    </tr>
                    <asp:Panel runat="server" ID="SpecialCaseLink_SCAC_Panel" Visible ="false">
                    <tr runat="server" id="SpecialCaseLink_SCAC" visible="true">
                        <td class="auto-style1">
                            &bull;&nbsp;
                            <asp:LinkButton ID="SpecialCaseLink_SCAC_hl" runat="server" href="SC_AwaitingConsult/AwaitingConsult.aspx">Other/Special Cases awaiting Consult </asp:LinkButton>[<asp:Label
                                runat="server" ID="SpecialCaseLink_SCAC_Count">0</asp:Label>]
                        </td>
                    </tr>
                    </asp:Panel>


                    <tr runat="server" id="SearchSCLink" visible="false">
                        <td>
                            Search:&nbsp;
                            <asp:TextBox runat="server" ID="txtSCSearch" Width="150" />&nbsp;
                            <asp:Button runat="server" ID="btnSCSearch" Text="Go" />
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <!-- Admin -->
        <asp:Panel runat="server" CssClass="section" ID="AdminPanel" Visible ="false">
            <div class="sectionHeader">
                Administration</div>
            <div class="sectionBody">
                <table>
                    <tr runat="server" id="UserManageLink">
                        <td>
                            &bull;&nbsp;<a href="Shared/Admin/ManageUsers.aspx?status=2">Pending Users</a>&nbsp;[<asp:Label
                                runat="server" ID="PendingUserCount">0</asp:Label>]<br />
                            &bull;&nbsp;<a href="Shared/Admin/RoleRequests.aspx">Pending Role Requests</a>&nbsp;[<asp:Label
                                runat="server" ID="PendingRolesLabel">0</asp:Label>]
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
    </div>
    </div>
    <asp:ObjectDataSource ID="dataMessages" runat="server" SelectMethod="RetrieveMessages"
        TypeName="ALODWebUtility.Common.MessageList">
        <SelectParameters>
            <asp:Parameter Name="userId" Type="Int32" />
            <asp:Parameter Name="groupId" Type="Int32" />
            <asp:Parameter Name="popup" Type="Boolean" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>
