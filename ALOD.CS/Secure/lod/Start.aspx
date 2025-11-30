<%@ Page Language="C#" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.LOD.Secure_lod_Start" Title="Untitled Page" CodeBehind="Start.aspx.cs" %>

<%@ Register Src="../Shared/UserControls/ValidationResults.ascx" TagName="ValidationResults"
    TagPrefix="uc1" %>
<%@ Register Src="../Shared/UserControls/SignatureBlock.ascx" TagName="SignatureBlock"
    TagPrefix="uc1" %>
<%@ Register Src="~/secure/Shared/UserControls/MemberSearchResultsGrid.ascx" TagName="MemberSearchResultsGrid" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
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
                                    <label for ="SSNTextBox">Member SSN:</label>
                                    <asp:TextBox ID="SSNTextBox" Width="80px" runat="server" MaxLength="9" oncopy="return false" onpaste="return false" oncut="return false" />
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
                                    <label for="SSNTextBox2">Validate SSN:</label>
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
                                <td class="value"><asp:Label runat="server" AssociatedControlID="txtMemberLastName" Text="Last Name:" />
                                    <asp:TextBox runat="server" ID="txtMemberLastName" MaxLength="50" Width="80px" placeholder="Last Name"></asp:TextBox>
                                    <asp:Label runat="server" AssociatedControlID="txtMemberFirstName" Text="First Name:" />
                                    <asp:TextBox runat="server" ID="txtMemberFirstName" MaxLength="50" Width="80px" placeholder="First Name"></asp:TextBox>
                                    <asp:Label runat="server" AssociatedControlID="txtMemberMiddleName" Text="Middle Name:" />
                                    <asp:TextBox runat="server" ID="txtMemberMiddleName" MaxLength="60" Width="80px" placeholder="Middle Name"></asp:TextBox>
                                    &nbsp;
                                    <asp:Label runat="server" ID="lblMemberNotFound" CssClass="labelRequired" Text="Member Not Found" Visible="false"/>
                                    &nbsp;
                                    <asp:Label runat="server" ID="lblInvalidMemberName" CssClass="labelRequired" Text="Invalid Name" Visible="false"/>
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
                    <tr>
                        <td class="number">
                            F
                        </td>
                        <td class="label">
                            Workflow:
                        </td>
                        <td class="value">
                            <asp:Label ID="Label1" runat="server" Visible="false">Line Of Duty</asp:Label>
                            <asp:DropDownList ID="WorkflowSelect" runat="server" Visible="false">
                                <%--<asp:ListItem Text="Original LOD" Value="1"></asp:ListItem>--%>
                                <asp:ListItem Selected="True" Text="LOD" Value="27"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            G
                        </td>
                        <td class="label labelRequired">
                            * Did the member notify the Medical Unit timely IAW AFI 36-2910:
                        </td>
                        <td class="value">
                            <asp:Panel runat="server" ID="TmnPanel">
                                <asp:RadioButton GroupName="tmn" runat="server" ID="TmnYes" Text="Yes" CssClass="tmn" />
                                <asp:RadioButton GroupName="tmn" runat="server" ID="TmnNo" Text="No" CssClass="tmn" />
                            </asp:Panel>
                            <asp:Label ID="lblTMN" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            H
                        </td>
                        <td class="label labelRequired">
                            * Did the member submit medical documents or ROI (Release of Information) timely IAW AFI 36-2910:
                        </td>
                        <td class="value">
                            <asp:Panel runat="server" ID="TmsPanel">
                                <asp:RadioButton GroupName="tms" runat="server" ID="TmsYes" Text="Yes" CssClass="tms" />
                                <asp:RadioButton GroupName="tms" runat="server" ID="TmsNo" Text="No" CssClass="tms" />
                            </asp:Panel>
                            <asp:Label ID="lblTMS" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            I
                        </td>
                        <td class="label labelRequired">
                            * Does this case involve a sexual assault:
                        </td>
                        <td class="value">
                            <asp:Panel runat="server" ID="SarcPanel">
                                <asp:RadioButton GroupName="sarc" runat="server" ID="SarcYes" Text="Yes" CssClass="sarc" AutoPostBack="true"/>
                                <asp:RadioButton GroupName="sarc" runat="server" ID="SarcNo" Text="No" CssClass="sarc" AutoPostBack="true"/>
                            </asp:Panel>
                            <asp:Label ID="lblSARC" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            J
                        </td>
                        <td class="label labelRequired">
                            Is this a restricted report:
                        </td>
                        <td class="value">
                            <asp:Panel runat="server" ID="RestrictedPanel">
                                <asp:RadioButton GroupName="restricted" runat="server" ID="RestrictedYes" Text="Yes"
                                    CssClass="restricted" />
                                <asp:RadioButton GroupName="restricted" runat="server" ID="RestrictedNo" Text="No"
                                    CssClass="restricted" />
                                <asp:RadioButton GroupName="restricted" runat="server" ID="RadioButtonNA" Text="N/A"
                                    CssClass="restricted" Enabled="false"/>
                            </asp:Panel>
                            <asp:Label ID="lblRestricted" runat="server"></asp:Label>
                        </td>
                    </tr>

                    <asp:PlaceHolder runat="server" id="WingRow" Visible="false">
                        <tr>
                        <td class="number">
                            K
                        </td>
                        <td class="label labelRequired" >
                            Does member belong to your wing:
                        </td>
                        <td class="value">
                            <asp:Panel runat="server" ID="WingPanel1" >
                                <asp:RadioButton GroupName="wing" runat="server" ID="WingYes" Text="Yes" CssClass="wing" />
                                <asp:RadioButton GroupName="wing" runat="server" ID="WingNo" Text="No"  CssClass="wing" />
                            </asp:Panel>
                            <asp:Label ID="lblWing2" runat="server"></asp:Label>
                        </td>
                    </tr>
                    </asp:PlaceHolder>

                    <tr>
                        <td class="number">
                            K
                        </td>
                        <td class="label">
                            Action:
                        </td>
                        <td class="value">
                            <asp:Button ID="CreateButton" runat="server" Text="Start LOD" /><br />
                            <uc1:SignatureBlock ID="SigBlock" runat="server" />
                        </td>
                    </tr>
                    <asp:PlaceHolder runat="server" id="ErrorRow1" >
                    <tr>
                        <td class="number">
                            M
                        </td>
                        <td class="label">
                            Error:
                        </td>
                        <td class="value">
                            <asp:Label runat="server" ID="ErrorLabel" ForeColor="Red" Font-Bold="true" />
                        </td>
                    </tr>
                    </asp:PlaceHolder>
                </table>
                <asp:Panel runat="server" ID="ErrorPanel" Visible="false">
                    <table>
                        <tbody>
                            <tr>
                                <td class="number">
                                    &nbsp;
                                </td>                                
                                <td class="label">
                                    Errors:
                                </td>
                                <td class="value">
                                    <asp:BulletedList ID="ValidationList" runat="server" CssClass="labelRequired" Visible="False">
                                    </asp:BulletedList>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </asp:Panel>
            </div>
        </asp:Panel>
        <asp:Panel ID="HistoryPanel" runat="server" Visible="False" CssClass="dataBlock">
            <div class="dataBlock-header">
                2 - Member Case History
            </div>
            <div class="dataBlock-body">
                <asp:GridView runat="server" ID="HistoryGrid" AutoGenerateColumns="false" Width="100%">
                    <Columns>
                        <asp:TemplateField HeaderText="Case Id" SortExpression="CaseId">
                            <ItemTemplate>
                                <asp:HyperLink runat="server" Text='<%# Eval("CaseId") %>' ID="ViewLink" NavigateUrl='<%# Eval("refId", "~/Secure/lod/init.aspx?refId={0}") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="UnitName" HeaderText="Unit" />
                        <asp:BoundField DataField="WorkStatus" HeaderText="Status" />
                        <asp:BoundField DataField="DateCreated" HeaderText="Created" />
                        <asp:TemplateField HeaderText="Print">
                            <ItemTemplate>
                                <asp:HyperLink runat="server" ID="PrintLink" ImageUrl="~/images/pdf.ico" Target="_blank"
                                    NavigateUrl='<%# Eval("refId", "~/Secure/lod/Print.aspx?id={0}") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataRowStyle CssClass="emptyItem" />
                    <EmptyDataTemplate>
                        No results found
                    </EmptyDataTemplate>
                </asp:GridView>
            </div>
        </asp:Panel>
    </div>
    <asp:ObjectDataSource ID="WorkflowData" runat="server" SelectMethod="GetByAllowedCreate"
        TypeName="ALODWebUtility.Worklfow.WorkFlowList">
        <SelectParameters>
            <asp:Parameter Name="compo" Type="String" />
            <asp:Parameter Name="type" Type="Object" />
            <asp:Parameter Name="groupId" Type="Byte" />
        </SelectParameters>
    </asp:ObjectDataSource>
    <asp:Label ID="AttachedUnitNameLabel" runat="server" CssClass="hidden"></asp:Label>
    &nbsp;<asp:Label ID="AttachedUnitCodeLabel" runat="server" CssClass="hidden"></asp:Label>
    <asp:Label ID="AttachedUnitIdLabel" runat="server" CssClass="hidden"></asp:Label>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFooter" runat="Server">

    <script type="text/javascript">


        $(function () {
            // Use the .on() method to attach the click event handler
            $('span.sarc > input').on('click', toggleSarc);

            toggleSarc();
        });


        //$(function() {

        //    $('span.sarc > input').each(function() {
        //        $(this).click(toggleSarc);
        //    });

        //    toggleSarc();

        //});

        function toggleSarc() {

            if (element('<%= SarcYes.ClientId %>') == null) {
                return;
            }

            var sarc = element('<%= SarcYes.ClientId %>').checked;

            $(".restricted").each(function() { this.disabled = !sarc; });

            if (sarc) {
                $('.restricted-label').addClass('labelRequired');
            } else {
                $('.restricted-label').removeClass('labelRequired');
            }

        }

    </script>

</asp:Content>
