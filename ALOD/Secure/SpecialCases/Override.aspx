<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.Secure_SpecialCases_Override" Codebehind="Override.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <div class="indent">
        <div class="border-thin">
            <div class="searchBody">
                      <table>
                        <tr>
                            <td>
                                Case Id:
                            </td>
                            <td class="value">
                                <asp:TextBox ID="CaseIdbox" runat="server" Width="130px"></asp:TextBox>&nbsp;&nbsp;
                                <asp:Button runat="server" ID="SearchButton" Text="Search" />
                                <ajax:FilteredTextBoxExtender ID="FilteredTextBoxExtender1" TargetControlID="CaseIdbox"
                                    runat="server" FilterType="Custom,UppercaseLetters,LowercaseLetters,Numbers"
                                    ValidChars="- " />
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td class="align-left">
                                <asp:Label ID="errlbl" class="label" runat="server" ForeColor="Red"> </asp:Label>
                            </td>
                        </tr>
                    </table>
            </div>
        </div>
        <br />
        <br />
        <div id="memberPanel" visible="false" runat="server">
            <div class="formHeader">
                CASE ID:  <asp:Label ID="caseIdLbl" class="label" runat="server" > </asp:Label>
              
            </div>
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
                        <asp:DropDownList ID="StatusSelect" DataTextField="Description" DataValueField="Id" runat="server" Width="240px"></asp:DropDownList>
                        <asp:Button runat="server" ID="StatusChangeBtn" Text="Change" /><br />
                        <asp:Label ID="StatusNotChangedlbl" runat ="server" ></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
