<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" MaintainScrollPositionOnPostback="true" Inherits="ALOD.Web.Sys.SystemEmails" Codebehind="SystemEmails.aspx.vb" %>

<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="head">
</asp:Content>

<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="ContentFooter">
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <div class="indent-small">
        <div class="dataBlock">
            <div class="dataBlock-header">
                1 - Address Selection Criteria
            </div>

            <div class="dataBlock-body">
                <table class="dataTable">
                    <tr>
                        <td class="number">
                            A
                        </td>
                        <td class="label">
                            Address Categories:
                        </td>
                        <td class="value">
                            <asp:CheckBox runat="server" ID="chkWorkEmail" Text="Work" />
                            <asp:CheckBox runat="server" ID="chkPersonalEmail" Text="Personal" />
                            <asp:CheckBox runat="server" ID="chkUnitEmail" Text="Unit" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            B
                        </td>
                        <td class="label">
                            User Groups:
                        </td>
                        <td class="value">
                            <asp:UpdatePanel runat="server" ID="pnlUserGroups" Style="overflow-y:scroll; height:105px; border: 1px solid #707070;">
                                <ContentTemplate>
                                    <asp:CheckBoxList runat="server" ID="chklUserGroups" DataTextField="Description" DataValueField="Id" AutoPostBack="true"></asp:CheckBoxList>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="chklUserGroups" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            C
                        </td>
                        <td class="label">
                            Export Addresses:
                        </td>
                        <td class="value">
                            <asp:Button runat="server" ID="btnExportToExcel" Text="To Excel" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div class="dataBlock">
            <div class="dataBlock-header">
                2 - Compose Email
            </div>

            <div class="dataBlock-body">
                <table class="dataTable">
                    <tr>
                        <td class="number">
                            A
                        </td>
                        <td class="label">
                            Select Template (Optional):
                        </td>
                        <td class="value">
                            <asp:DropDownList runat="server" ID="ddlEmailTemplates" DataTextField="Title" DataValueField="Id" AutoPostBack="true" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            B
                        </td>
                        <td class="label labelRequired">
                            *Subject:
                        </td>
                        <td class="value">
                            <asp:UpdatePanel runat="server" ID="pnlSubject">
                                <ContentTemplate>
                                    <asp:TextBox runat="server" ID="txtSubject" Style="width:250%" />
                                    <asp:RequiredFieldValidator ID="rqfdSubject" runat="server" ControlToValidate="txtSubject" ErrorMessage="Subject Required." ValidationGroup="email" SetFocusOnError="True" Display="Dynamic">*</asp:RequiredFieldValidator>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="ddlEmailTemplates" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            C
                        </td>
                        <td class="label labelRequired">
                            *Body:
                        </td>
                        <td class="value">
                            <asp:UpdatePanel runat="server" ID="pnlBody">
                                <ContentTemplate>
                                    <asp:TextBox runat="server" ID="txtBody" TextMode="MultiLine" Rows="10" Style="width:250%" />
                                    <asp:RequiredFieldValidator ID="rqfdBody" runat="server" ControlToValidate="txtBody" ErrorMessage="Body Required." ValidationGroup="email" SetFocusOnError="True">*</asp:RequiredFieldValidator>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="ddlEmailTemplates" EventName="SelectedIndexChanged" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            D
                        </td>
                        <td class="label">
                            Action:
                        </td>
                        <td class="value">
                            <asp:Button runat="server" ID="btnSend" Text="Send" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            E
                        </td>
                        <td class="label">
                            Results:
                        </td>
                        <td class="value">
                            <asp:Label runat="server" ID="lblResults" ForeColor="Red" />
                        </td>
                    </tr>
                </table>
            </div>
        </div>
    </div>
</asp:Content>
