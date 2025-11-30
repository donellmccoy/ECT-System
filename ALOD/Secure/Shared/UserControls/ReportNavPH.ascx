<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="ReportNavPH.ascx.vb" Inherits="ALOD.Web.UserControls.Secure_Shared_UserControls_ReportNavPH" %>

<asp:Panel runat="server" ID="InputPanel" CssClass="dataBlock">
    <div class="dataBlock-header">
        1 - Report Options
    </div>
    <div class="dataBlock-body">
        <asp:UpdatePanel runat="server" ID="upnlReportParameters" ChildrenAsTriggers="true">
            <ContentTemplate>
                <table runat="server" id="tbl1">
                    <tr runat="server" id="tr1">
                        <td class="number">
                            <asp:Label ID="ReportLabel1" runat="server" Text="A"></asp:Label>
                        </td>
                
                        <td class="label">
                            Select: 
                            <asp:RadioButtonList ID="rblSelect" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="true">
                                <asp:ListItem Selected="True">Unit</asp:ListItem>
                                <asp:ListItem>NAF</asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                         <td class="value">
                             <asp:DropDownList ID="ddlUnit" runat="server" DataTextField="Name" DataValueField="Value" />
                        </td>
                    </tr>

                    <tr runat="server" id="tr2">
                         <td class="number">
                           <asp:Label ID="ReportLabel2" runat="server" Text="B"></asp:Label>
                        </td>
                        <td class="label">
                            Include subordinate units:
                        </td>
                         <td class="value">
                            <asp:CheckBox ID="chkSubordinateUnit" runat="server" Checked="True" />
                        </td>
                    </tr>

                    <tr runat="server" id="trCollocated">
                         <td class="number">
                           <asp:Label ID="lblCollocatedRow" runat="server" Text="C"></asp:Label>
                        </td>
                        <td class="label">
                            Collocated Units:
                        </td>
                         <td class="value">
                            <asp:RadioButtonList ID="rblCollocated" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="false">
                                <asp:ListItem Value="1" Selected="True">Both&nbsp;&nbsp;</asp:ListItem>
                                <asp:ListItem Value="2">Collocated&nbsp;&nbsp;</asp:ListItem>
                                <asp:ListItem Value="3">Non-Collocated</asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                    </tr>

                    <tr runat="server" id="tr3">
                         <td class="number">
                            <asp:Label ID="ReportLabel4" runat="server" Text="D"></asp:Label>
                        </td>
                        <td class="label">
                           Begin Reporting Period:
                        </td>
                         <td class="value">
                            <asp:DropDownList runat="server" ID="ddlBeginYear" AutoPostBack="false" />
                            <asp:DropDownList runat="server" ID="ddlBeginMonth" AutoPostBack="false" />
                        </td>
                    </tr>
                    <tr runat="server" id="tr4">
                         <td class="number">
                       <asp:Label ID="ReportLabel3" runat="server" Text="E"></asp:Label>
                        </td>
                        <td class="label">
                            End Reporting Period:
                        </td>
                        <td class="value">
                            <asp:DropDownList runat="server" ID="ddlEndYear" AutoPostBack="false" />
                            <asp:DropDownList runat="server" ID="ddlEndMonth" AutoPostBack="false" />
                        </td>
                    </tr>

                    <tr runat="server" id="tr7">
                        <td class="number">
                            <asp:Label ID="Label3" runat="server" Text="F"></asp:Label>
                        </td>
                        <td class="label">
                            Include Text Fields:
                        </td>
                        <td class="value">
                            <asp:CheckBox runat="server" ID="chkIncludeTextFields" Checked="true" />
                        </td>
                    </tr>

                    <tr runat="server" id="tr6">
                        <td class="number">
                            <asp:Label ID="Label2" runat="server" Text="G"></asp:Label>
                        </td>
                        <td class="label">
                            Include Comments:
                        </td>
                        <td class="value">
                            <asp:CheckBox runat="server" ID="chkIncludeComments" Checked="true" />
                        </td>
                    </tr>

                    <tr runat="server" id="tr5">
                        <td class="number">
                            <asp:Label ID="Label1" runat="server" Text="H"></asp:Label>
                        </td>
                        <td class="label">
                            Output Format:
                        </td>
                        <td class="value">
                            <asp:RadioButton ID="OutputScreen" runat="server" Checked="true" GroupName="Output" Text="Browser" />
                            <img src="../../images/page_white_world.gif" alt="View in browser" style="vertical-align:middle" />
                            &nbsp;
                            <asp:RadioButton ID="OutputExcel" runat="server" GroupName="Output" Text="Excel" />
                            <img src="../../images/page_white_excel.gif" alt="Export to Excel" style="vertical-align:middle" />
                        </td>
                    </tr>
                </table>

                <table runat="server" id="tbl2">
                    <tr>
                        <td class="number">
                            <asp:Label ID="ReportLabel8" runat="server" Text="I"></asp:Label>
                        </td>
                        <td class="label">
                            Action:
                        </td>
                        <td class="value">
                            <asp:Button ID="ReportButton" runat="server" Text="Run Report" />
                        </td>
                    </tr>
                    <tr runat="server" id="trErrors" visible="false">
                        <td class="number">
                            &nbsp;
                        </td>
                        <td class="label">
                            Errors:
                        </td>
                        <td class="value">
                            <asp:BulletedList ID="bllErrors" runat="server" CssClass="labelRequired" />
                        </td> 
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Panel>

