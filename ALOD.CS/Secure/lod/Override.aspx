<%@ Page Title="" Language="C#" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.LOD.Secure_lod_Override" CodeBehind="Override.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <asp:Panel runat="server" ID="pnlCaseSearch" CssClass="indent-small dataBlock">
        <div class="dataBlock-header">
            1 - Case Search
        </div>
        <div class="dataBlock-body">
            <table class="dataTable">
                <tr>
                    <td class="number">
                        A
                    </td>
                    <td class="label">
                        Workflow:
                    </td>
                    <td class="value">
                        <asp:DropDownList runat="server" ID="ddlModules" />
                    </td>
                    <td>
                        NOTE: Select Line of Duty for Unrestricted SARC and Legacy Restricted SARC cases.
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        B
                    </td>
                    <td class="label">
                        Case Id:
                    </td>
                    <td class="value">
                        <asp:TextBox ID="CaseIdbox" runat="server" Width="130px"></asp:TextBox>&nbsp;&nbsp;
                        <ajax:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" TargetControlID="CaseIdbox"
                            runat="server" FilterType="Custom,UppercaseLetters,LowercaseLetters,Numbers"
                            ValidChars="- " />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        C
                    </td>
                    <td class="label">
                        Action:
                    </td>
                    <td class="value">
                        <asp:Button runat="server" ID="SearchButton" Text="Search" />
                    </td>
                </tr>
                <tr runat="server" id="trErrors" visible="false">
                    <td class="number">
                    </td>
                    <td class="label">
                        Errors:
                    </td>
                    <td class="value">
                        <asp:Label ID="errlbl" class="label" runat="server" ForeColor="Red" />
                    </td>
                </tr>
            </table>

        </div>
    </asp:Panel>

    <asp:Panel runat="server" ID="pnlCaseInfo" CssClass="indent-small dataBlock" Visible="false">
        <div class="dataBlock-header">
            2 - Case Information <asp:Label ID="lblCaseId" class="label" runat="server" />
        </div>
        <div class="dataBlock-body">
            <table class="dataTable">
                <tr>
                    <td class="number">
                        A
                    </td>
                    <td class="label">
                        Name:
                    </td>
                    <td class="value">
                        <asp:Label ID="Namelbl" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        B
                    </td>
                    <td class="label">
                        Rank:
                    </td>
                    <td class="value">
                        <asp:Label ID="Ranklbl" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        C
                    </td>
                    <td class="label">
                        Unit:
                    </td>
                    <td class="value">
                        <asp:Label runat="server" ID="Unitlbl" />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        D
                    </td>
                    <td class="label">
                        Current Status:
                    </td>
                    <td class="value">
                        <asp:Label ID="Statuslbl" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        E
                    </td>
                    <td class="label">
                        Change Status:
                    </td>
                    <td class="value">
                        <asp:DropDownList runat="server" ID="ddlWorkStatus" Width="240px" />
                        <br />
                        <asp:Label ID="StatusNotChangedlbl" runat ="server" ></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        F
                    </td>
                    <td class="label">
                        Action:
                    </td>
                    <td class="value">
                        <asp:Button runat="server" ID="StatusChangeBtn" Text="Change" />
                    </td>
                </tr>
            </table>
        </div>
    </asp:Panel>
</asp:Content>
