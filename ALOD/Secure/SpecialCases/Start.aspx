<%@ Page Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.Secure_sc_ci_Start" Title="Untitled Page" Codebehind="Start.aspx.vb" %>

<%@ Import Namespace="ALODWebUtility.Common" %>

<%@ Register Src="../Shared/UserControls/ValidationResults.ascx" TagName="ValidationResults" TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/SignatureBlock.ascx" TagName="SignatureBlock" TagPrefix="uc1" %>
<%@ Register Src="~/secure/Shared/UserControls/MemberSearchResultsGrid.ascx" TagName="MemberSearchResultsGrid" TagPrefix="uc1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .style1
        {
            height: 26px;
        }
        .style2
        {
            width: 11px;
        }
        .style3
        {
            height: 26px;
            width: 11px;
        }
        .style4
        {
            text-decoration: underline;
        }
        .multipleLODsListDiv
        {
            overflow-y: auto; 
            max-height: 150px
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="Server">
    <div class="indent-small">
        <asp:Panel runat="server" ID="InputPanel" CssClass="dataBlock">
            <div class="dataBlock-header">
                1 - Member Information
            </div>
            <div class="dataBlock-body">
                <asp:UpdatePanel runat="server" ID="upnlMemberLookup" ChildrenAsTriggers="true">
                    <ContentTemplate>
                        <table>
                            <tr runat="server" id="trRadioButtons">
                                <td class="number">
                                    A
                                </td>
                                <td class="label">
                                    Lookup By:
                                </td>
                                <td class="value">
                                    <asp:RadioButton runat="server" ID="rdbSSN" Text="SSN" GroupName="LookupChoice" AutoPostBack="true" Checked="True" />
                                    <asp:RadioButton runat="server" ID="rdbName" Text="Name" GroupName="LookupChoice" AutoPostBack="true" />
                                </td>
                            </tr>
                            <tr runat="server" class="tr-SSN" id="trSSN">
                                <td class="number">
                                    B
                                </td>
                                <td class="label labelRequired">
                                    * Member SSN:
                                </td>
                                <td class="value">
                                    <asp:TextBox ID="SSNTextBox" Width="80px" runat="server" MaxLength="9" oncopy="return false" onpaste="return false" oncut="return false"/>
                                    &nbsp;
                                    <asp:Label ID="NotFoundLabel" runat="server" CssClass="labelRequired" Text="SSN Not Found" Visible="False"/>
                                    &nbsp;
                                    <asp:Label ID="InvalidSSNLabel" runat="server" CssClass="labelRequired" Text="Invalid SSN" Visible="False"/>
                                    &nbsp;
                                    <asp:Label ID="lblInvalidMemberForSSN" runat="server" CssClass="labelRequired" Text="Cannot Select Your Own Service Record" Visible="False"/>
                                </td>
                            </tr>
                            <tr runat="server" class="tr-SSN" id="trSSN2">
                                <td class="number">
                                </td>
                                <td class="label labelRequired">
                                    * Validate SSN:
                                </td>
                                <td class="value">
                                    <asp:TextBox ID="SSNTextBox2" Width="80px" runat="server" MaxLength="9" oncopy="return false" onpaste="return false" oncut="return false"/>
                                    &nbsp;
                                    <asp:Label ID="InvalidLabel" runat="server" CssClass="labelRequired" Text="Invalid SSN" Visible="False"/>
                                    &nbsp;
                                    <asp:Label ID="InvalidSSN" runat="server" CssClass="labelRequired" Text="Different SSN" Visible="False"/>
                                </td>
                            </tr>
                            <tr runat="server" class="tr-Name" id="trName">
                                <td class="number">
                                    B
                                </td>
                                <td class="label labelRequired">
                                    * Member Name (last / first / middle):
                                </td>
                                <td class="value">
                                    <asp:TextBox runat="server" ID="txtMemberLastName" MaxLength="50" Width="80px" placeholder="Last Name"></asp:TextBox>
                                    <asp:TextBox runat="server" ID="txtMemberFirstName" MaxLength="50" Width="80px" placeholder="First Name"></asp:TextBox>
                                    <asp:TextBox runat="server" ID="txtMemberMiddleName" MaxLength="60" Width="80px" placeholder="Middle Name"></asp:TextBox>
                                    &nbsp;
                                    <asp:Label runat="server" ID="lblMemberNotFound" CssClass="labelRequired" Text="Member Not Found" Visible="false"></asp:Label>
                                    &nbsp;
                                    <asp:Label runat="server" ID="lblInvalidMemberName" CssClass="labelRequired" Text="Invalid Name" Visible="false"></asp:Label>
                                    &nbsp;
                                    <asp:Label ID="lblInvalidMemberForName" runat="server" CssClass="labelRequired" Text="Cannot Select Your Own Service Record" Visible="False"/>
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <table>
                    <tr>
                        <td class="number">
                            C
                        </td>
                        <td class="label">
                            Action:
                        </td>
                        <td class="value">
                            <asp:Button ID="LookupButton" runat="server" Text="Lookup" />
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <asp:Panel runat="server" ID="MemberSelectionPanel" Visible="False" CssClass="dataBlock">
            <div class="dataBlock-header">
                2 - Member Selection
            </div>
            <div class="dataBlock-body">
                <uc1:MemberSearchResultsGrid runat="server" ID="ucMemberSelectionGrid"></uc1:MemberSearchResultsGrid>
            </div>
        </asp:Panel>
        <asp:Panel ID="MemberDataPanel" runat="server" Visible="False" CssClass="dataBlock">
            <div class="dataBlock-header">
                1 - Member Information
            </div>
            <div class="dataBlock-body">
                <table>
                    <tr>
                        <td class="number">
                            A
                        </td>
                        <td class="label">
                            Member SSN:
                        </td>
                        <td class="value">
                            <asp:Label ID="SSNLabel" runat="server"></asp:Label>
                            &nbsp;[<asp:LinkButton runat="server" ID="ChangeSsnButton" Text="change" />]
                            <asp:Label runat="server" ID="CompoLabel" Visible="false" />
                        </td>
                    </tr>
                </table>
                <asp:UpdatePanel runat="server" ID="upnlMemberData" ChildrenAsTriggers="true">
                    <ContentTemplate>
                        <table>
                            <tr>
                                <td class="number">
                                    B
                                </td>
                                <td class="label">
                                    Name:
                                </td>
                                <td class="value">
                                    <asp:Label ID="NameLabel" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="number">
                                    C
                                </td>
                                <td class="label">
                                    Date of Birth:
                                </td>
                                <td class="value">
                                    <asp:Label ID="DoBLabel" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="number">
                                    D
                                </td>
                                <td class="label">
                                    Grade:
                                </td>
                                <td class="value">
                                    <asp:Label ID="GradeCodeLabel" runat="server" CssClass="hidden" />
                                    <asp:Label ID="RankLabel" runat="server"></asp:Label>
                                </td>
                            </tr>
                            <tr>
                                <td class="number">
                                    E
                                </td>
                                <td class="label">
                                    Unit:
                                </td>
                                <td class="value">
                                    <asp:Label ID="UnitNameLabel" runat="server"></asp:Label>
                                    &nbsp;(<asp:Label ID="UnitCodeLabel" runat="server"></asp:Label>)
                                    <asp:Label ID="UnitIdLabel" runat="server" CssClass="hidden"></asp:Label>
                                </td>
                            </tr>

                            <tr runat="server" visible="true">
                                <td class="number">
                                         F
                                </td>
                                <td class="label">
                                       Compo:
                                </td>
                                <td class="value">
                                         <asp:Label ID="CompoSelect" runat="server">
                                         </asp:Label>
                                         <asp:DropDownList ID="ddlCompo" runat="server">
                                         </asp:DropDownList>
                                </td>
                           </tr>

                            <tr id="rowModuleType" runat="server" visible="true">
                                <td class="number">
                                    G
                                </td>
                                <td class="label">
                                    Other / Specialty Case Type:
                                </td>
                                <td class="value">
                                    <asp:Label ID="moduleLabel" runat="server"></asp:Label>
                                    <asp:DropDownList runat="server" ID="ddlModule" AutoPostBack="True">
                                    </asp:DropDownList>
                                </td>
                            </tr>

                            <tr id="rowSimDeploy" runat="server" visible="false">
                                <td class="number">
                                    H
                                </td>
                                <td class="label">
                                    Simulated Deployment?
                                </td>
                                <td class="value">
                                    <asp:RadioButton ID="rbYes" runat="server" GroupName="rbSimDeployment" Text="Yes" />
                                    &nbsp;&nbsp;&nbsp;&nbsp;
                                    <asp:RadioButton ID="rbNo" runat="server" GroupName="rbSimDeployment" Text="No" />
                                    <asp:RadioButtonList ID="RadioButtonList1" runat="server">
                                    </asp:RadioButtonList>
                                </td>
                            </tr>
                            <tr id="rowMAJCOM" runat="server" visible="false">
                                <td class="number">
                                    I
                                </td>
                                <td class="label">MAJCOM:</td>
                                <td class="value">
                                    <asp:Label ID="lblMAJCOM" runat="server"></asp:Label>
                                    <asp:DropDownList runat="server" ID="ddlMAJCOM">
                                    </asp:DropDownList>
                                </td>
                            </tr>


                                     <tr id="BMTModuleSubType" runat="server" visible="false">
                                <td class="number">
                                    G
                                </td>
                                <td class="label">
                                    Other / Specialty Case Type:
                                </td>
                                <td class="style1">
                                    <asp:DropDownList runat="server" ID="ddlSubModule" AutoPostBack="True" DataSourceID="Data_BMTSubTypes" DataTextField="Title" DataValueField="Id">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr id="PSCDAssociatedCaseType" runat="server" visible="false">
                                <td class="number">
                                    H
                                </td>
                                <td class="label">
                                    Associated Case Type
                                </td>
                                <td class="style1">
                                    <asp:DropDownList runat="server" ID="AssociatedCaseTypeDropDownList" AutoPostBack="True"  DataTextField="Title" DataValueField="Id">
                                        <asp:ListItem Value="N/A">--- No Associated Case ---</asp:ListItem>
                                        <asp:ListItem Value="IR">IRILO WD (IR)</asp:ListItem>
                                        <asp:ListItem Value="WD">Worldwide Duty (WD)</asp:ListItem>
                                        <asp:ListItem Value="RW">Retention Waiver Renewal (RW)</asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>

                            <tr runat="server" id="AssociatedLODRow" visible="True">
                                <td class="number">
                                <div id="AssociatedLODListHeader" runat="server" visible="false">
                                    <asp:Label runat="server" ID="sectionAssociatedLOD" Text="H"></asp:Label>
                                </div>
                                </td>
                                <td id="lodRequired" runat="server" valign="top">
                                <div id="AssociatedLODListLabel" runat="server" visible="false">
                                    Associated LOD:
                                </div>
                                </td>
                                <td class="value">
                                    <div id="AssociatedLODList" runat="server" visible="false">
                                        <asp:Label runat="server" ID="lblAssociatedLODs" Visible="false" />
                                        <asp:DropDownList runat="server" ID="ddlLODs" AutoPostBack="false">
                                        </asp:DropDownList>
                                    </div>
                                    <div id="AssociatedLODMultiple" runat="server" visible="false" class="multipleLODsListDiv">
                                        <asp:Label runat="server" ID="lblAssociatedLODMultiple" Visible="false" />
                                        <asp:GridView runat="server" ID="MultipleLODs" AutoGenerateColumns="false" Width="98%" AllowSorting="true">
                                            <Columns>
                                                <asp:TemplateField HeaderText="Case Id" SortExpression="CaseId">
                                                    <ItemTemplate>
                                                        <asp:label ID="lblCaseId" runat="server" Text='<%# Eval("item1") %>' />
                                                        <asp:label id="RefId" runat="server" Text='<%# Eval("item2").ToString %>' Visible="false" />
                                                        <asp:label id="workflowId" runat="server" Text='<%# Eval("item3").ToString %>' Visible="false" />
                                                    </Itemtemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Associate Cases">
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="AddLOD" runat="server" />
                                                    </Itemtemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <EmptyDataRowStyle CssClass="emptyItem" />
                                            <EmptyDataTemplate>
                                                No other Other/Special Cases exist for service member
                                            </EmptyDataTemplate>
                                        </asp:GridView>
                                    </div>
                                </td>
                            </tr>
                            <tr runat="server" id="AssociatedWWDRow" visible="False">
                                <td class="number">
                                <div id="AssociatedWWDListHeader" runat="server" visible="false">
                                    <asp:Label runat="server" ID="sectionAssociatedWWD" Text="H"></asp:Label>
                                </div>
                                </td>
                                <td class="label">
                                <div id="AssociatedWWDListLabel" runat="server" visible="false">
                                    Associated WWD:
                                </div>
                                </td>
                                <td class="value">
                                    <div id="AssociatedWWDList" runat="server" visible="false">
                                    <asp:DropDownList runat="server" ID="ddlWWDs" AutoPostBack="false">
                                    </asp:DropDownList>
                                    </div>
                                </td>
                            </tr>
                            <tr runat="server" id="AssociatedIRRow" visible="False">
                                <td class="number">
                                <div id="AssociatedIRListHeader" runat="server" visible="false">
                                    <asp:Label runat="server" ID="sectionAssociatedIR" Text="H"></asp:Label>
                                </div>
                                </td>
                                <td class="label">
                                <div id="AssociatedIRListLabel" runat="server" visible="false">
                                    Associated IRILO:
                                </div>
                                </td>
                                <td class="value">
                                    <div id="AssociatedIRList" runat="server" visible="false">
                                    <asp:DropDownList runat="server" ID="ddlIR" AutoPostBack="false">
                                    </asp:DropDownList>
                                    </div>
                                </td>
                            </tr>
                            <tr runat="server" id="AssociatedSCRow" visible="False">
                                <td class="number">
                                <div id="AssociatedSCListHeader" runat="server" visible="false">
                                    <asp:Label runat="server" ID="sectionAssociatedSC" Text="H"></asp:Label>
                                </div>
                                </td>
                                <td id="scRequired" runat="server" valign="top">
                                <div id="AssociatedSCListLabel" runat="server" visible="false">
                                    <asp:Label runat="server" ID="lblAssociatedSCList">Associated Special Case:</asp:Label>
                                </div>
                                </td>
                                <td class="value">
                                    <div id="AssociatedSCList" runat="server" visible="false">
                                    <asp:DropDownList runat="server" ID="ddlSCs" AutoPostBack="false">
                                    </asp:DropDownList>
                                    </div>
                                </td>
                            </tr>
                            <tr runat="server" id="AssociatedRWRow" visible="False">
                                <td class="number">
                                <div id="AssociatedRWListHeader" runat="server" visible="false">
                                    <asp:Label runat="server" ID="sectionAssociatedRW" Text="H"></asp:Label>
                                </div>
                                </td>
                                   <td class="label">
                                <div id="AssociatedRWListLabel" runat="server" visible="false">
                                    Associated RW Case:
                                </div>
                                </td>
                                <td class="value">
                                    <div id="AssociatedRWList" runat="server" visible="false">
                                    <asp:DropDownList runat="server" ID="ddlRWs" AutoPostBack="false">
                                    </asp:DropDownList>
                                    </div>
                                </td>
                            </tr>
                            <tr id="PEPPSubTypeRow" runat="server" visible="false">
                                <td class="number">
                                    <div id="PEPPSubTypeListHeader" runat="server" visible="false">
                                        <asp:Label runat="server" ID="sectionPEPPSubType" Text="G"></asp:Label>
                                    </div>
                                </td>
                                <td id="peppSubTypeRequired" runat="server" valign="top" class="label">
                                    <div id="PEPPSubTypeListLabel" runat="server" visible="false">
                                        Other / Specialty Case Type:
                                    </div>
                                </td>
                                <td class="value">
                                    <div id="PEPPSubTypeList" runat="server" visible="false">
                                        <asp:DropDownList runat="server" ID="ddlPEPPSubType" DataSourceID="Data_PEPPSubTypes" DataTextField="Title" DataValueField="Id">
                                        </asp:DropDownList>
                                    </div>
                                </td>
                            </tr>


                            <tr id="CaseTypeRow" runat="server" visible="false">
                                <td class="number">
                                    <div id="CaseTypeListHeader" runat="server" visible="false">
                                        <asp:Label runat="server" ID="lblSectionCaseType" Text="G" />
                                    </div>
                                </td>
                                <td id="caseTypeRequired" runat="server" valign="top" class="label">
                                    <div id="caseTypeListLabel" runat="server" visible="false">
                                        Other / Specialty Case Type:
                                    </div>
                                </td>
                                <td class="value">
                                    <div id="caseTypeList" runat="server" visible="false">
                                        <asp:DropDownList runat="server" ID="ddlCaseType" AutoPostBack="true" />
                                    </div>
                                </td>
                            </tr>
                            <tr runat="server" id="OtherCaseTypeRow" visible="false">
                                <td class="number">
                                    G.1
                                </td>
                                <td class="label  labelRequired">
                                    *Enter Specialty Case Type:
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtCaseTypeName" Width="175px" MaxLength="50"></asp:TextBox>
                                </td>
                            </tr>

                            <tr id="SubCaseTypeRow" runat="server" visible="false">
                                <td class="number">
                                    <div id="SubCaseTypeListHeader" runat="server" visible="false">
                                        <asp:Label runat="server" ID="lblSectionSubCaseType" Text="G" />
                                    </div>
                                </td>
                                <td id="subCaseTypeRequired" runat="server" valign="top" class="label">
                                    <div id="subCaseTypeListLabel" runat="server" visible="false">
                                        Case Type:
                                    </div>
                                </td>
                                <td class="value">
                                    <div id="subCaseTypeList" runat="server" visible="false">
                                        <asp:DropDownList runat="server" ID="ddlSubCaseType" AutoPostBack="true" />
                                    </div>
                                </td>
                            </tr>
                            <tr runat="server" id="OtherSubCaseTypeRow" visible="false">
                                <td class="number">
                                    H.1
                                </td>
                                <td class="label  labelRequired">
                                    *Enter Case Type:
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtSubCaseTypeName" Width="175px" MaxLength="50"></asp:TextBox>
                                </td>
                            </tr>


                            <tr runat="server" id="nesRequestEligibleRow" visible="false">
                                <td class="number">
                                    <div id="nesRequestEligibleHeader" runat="server" visible="false">
                                        <asp:Label runat="server" ID="nesRequestEligible" Text="H"></asp:Label>
                                    </div>
                                </td>
                                <td class="label">
                                    <div id="nesRequestEligibleLabel" runat="server" visible="false">
                                        <a class="style4"><asp:Label runat="server" ID="lblNESRequestEligible" Text="Is Member Eligible IAW AFI 41-210?:" ToolTip="IAW AFI 41-210 para.4.75. Is the member within final six months of service?"></asp:Label></a>
                                    </div>
                                </td>
                                <td class="value">
                                    <div id="nesRequestEligibleRadioButtons" runat="server" visible="false">
                                        <asp:RadioButton runat="server" ID="rdbNESYes" GroupName="NESDecision" Text="Yes" AutoPostBack="true" />
                                        <asp:RadioButton runat="server" ID="rdbNESNo" GroupName="NESDecision" Text="No" AutoPostBack="true" />
                                    </div>
                                </td>
                                <td class="value">
                                    <asp:Label runat="server" ID="nesRequestEligibieDecisionLabel" ForeColor="Red" Font-Bold="true" />
                                </td>
                            </tr>
                            <tr>
                                <td class="number">
                                    <asp:Label runat="server" ID="sectionAction" Text="J"></asp:Label>
                                </td>
                                <td class="label">
                                    Action:
                                </td>
                                <td class="value">
                                    <asp:Button ID="CreateButton" runat="server" Text="Start Case" /><br />
                                    <uc1:SignatureBlock ID="SigBlock" runat="server" />
                                </td>
                            </tr>
                            <tr runat="server" id="ErrorRow" visible="false">
                                <td class="number">
                                    <asp:Label runat="server" ID="sectionErrors" Text="J"></asp:Label>
                                </td>
                                <td class="label">
                                    Error:
                                </td>
                                <td class="value">
                                    <asp:Label runat="server" ID="ErrorLabel" ForeColor="Red" Font-Bold="true" />
                                </td>
                            </tr>
                            <tr runat="server" id="WorkflowRow" visible="False">
                                <td class="number">
                                    <asp:Label runat="server" ID="sectionWorkflow" Text="I"></asp:Label>
                                </td>
                                <td class="label">
                                    Workflow:
                                </td>
                                <td class="value">
                                    <asp:DropDownList runat="server" ID="ddlWorkflow" AutoPostBack="False" Visible="False">
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr runat="server" id="ValidationErrorsRow" visible="false">
                                <td class="number">
                                    &nbsp;
                                </td>
                                <td class="label">
                                    Errors:
                                </td>
                                <td class="value">
                                    <asp:BulletedList ID="ValidationList" runat="server" CssClass="labelRequired" Visible="False" />
                                </td>
                            </tr>
                        </table>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </asp:Panel>
    <asp:Panel ID="HistoryPanelLOD" runat="server" Visible="false" CssClass="dataBlock">
        <div class="dataBlock-header">
            Member LOD Case History
        </div>
        <div class="dataBlock-body">
            <asp:GridView runat="server" ID="HistoryGridLOD" AutoGenerateColumns="false" Width="100%" AllowPaging="true" AllowSorting="true">
                <Columns>
                    <asp:TemplateField HeaderText="Case Id" SortExpression="CaseId">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkRefID" runat="server" CommandArgument='<%# Eval("RefId").ToString()  %>'
                                CommandName="view" Text='<%# Eval("CaseId") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="UnitName" HeaderText="Unit" SortExpression="UnitName" />
                    <asp:BoundField DataField="WorkStatus" HeaderText="Status" SortExpression="WorkStatus" />
                    <asp:BoundField DataField="Compo" HeaderText="Component" />
                    <asp:BoundField DataField="DateCreated" HeaderText="Created" SortExpression="DateCreated" />
                    <asp:TemplateField HeaderText="Completed Date">
                        <ItemTemplate>
                            <asp:Label ID="completeDateLbl" runat="server">
                            </asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataRowStyle CssClass="emptyItem" />
                <EmptyDataTemplate>
                    No other active LOD cases exist for service member
                </EmptyDataTemplate>
            </asp:GridView>
            <div style="text-align: center;">
                <asp:Label ID="NoHistoryLODLbl" Text="No other LOD cases found" CssClass="labelNormal" runat="server"> 
                </asp:Label>
            </div>
        </div>
    </asp:Panel>
    <asp:Panel ID="HistoryPanel" runat="server" Visible="false" CssClass="dataBlock">
        <div class="dataBlock-header">
            Member Other/Specialty Case History
        </div>
        <div class="dataBlock-body">
            <asp:GridView runat="server" ID="HistoryGrid" AutoGenerateColumns="false" Width="100%" AllowPaging="true" AllowSorting="true">
                <Columns>
                    <asp:TemplateField HeaderText="Case Id" SortExpression="Case_Id">
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkRefSCID" runat="server" CommandArgument='<%# Eval("RefId").ToString() + ";" + Eval("Module_Id").ToString()  %>'
                                CommandName="view" Text='<%# Eval("Case_Id") %>'>
                            </asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Unit_Name" HeaderText="Unit" SortExpression="Unit_Name" />
                    <asp:TemplateField HeaderText="Case Type" SortExpression="Module">
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%#IIf(Eval("workflow_title").ToString = "Worldwide Duty (WD)", "Non Duty Disability Evaluation System (NDDES)", Eval("workflow_title").ToString)%>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="Status" HeaderText="Status" SortExpression="Status" />
                    <asp:BoundField DataField="ReceiveDate" HeaderText="Received" SortExpression="ReceiveDate" />
                    <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                </Columns>
                <EmptyDataRowStyle CssClass="emptyItem" />
                <EmptyDataTemplate>
                    No other Other/Special Cases exist for service member
                </EmptyDataTemplate>
            </asp:GridView>
            <div style="text-align: center;">
                <asp:Label ID="NoHistoryLbl" Text="No other Other/Special cases found" CssClass="labelNormal" runat="server"> 
                </asp:Label>
            </div>
        </div>
    </asp:Panel>
    </div>
    
    <asp:ObjectDataSource ID="Data_BMTSubTypes"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSCSubWorkflowTypes" 
        TypeName="ALOD.Data.Services.LookupService">
        <SelectParameters>
            <asp:Parameter Name="workflowId" DefaultValue="8" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
    
    <asp:ObjectDataSource ID="Data_PEPPSubTypes"  runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetSCSubWorkflowTypes" 
        TypeName="ALOD.Data.Services.LookupService">
        <SelectParameters>
            <asp:Parameter Name="workflowId" DefaultValue="23" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentFooter" runat="Server">
</asp:Content>
