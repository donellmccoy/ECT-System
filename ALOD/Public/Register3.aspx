<%@ Page Title="" Language="VB" MasterPageFile="~/Public/Public.master" AutoEventWireup="false" Inherits="ALOD.Web.Public_Register3" Codebehind="Register3.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentHeader" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="Server">
    <div id="content">
        <h1>
            ECT Registration > Review Information</h1>
        <div>
            <br />
            <asp:Label runat="server" ID="lblInfo" Text="Please review the following information. To make any corrections click the 'Previous' button below"
                CssClass="labelNormal" />
            <br />
            <br />
            <asp:Panel ID="Displaypanel" runat="server" CssClass="dataBlock">
                <div class="dataBlock-header">
                    1 - User Information</div>
                <div class="dataBlock-body">
                    <table cellpadding="2" cellspacing="2" style="text-align: left;">
                        <tr>
                            <td class="number">
                                B
                            </td>
                            <td class="label">
                                Name:
                            </td>
                            <td class="value">
                                <asp:Label runat="server" ID="FullName" />&nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                C
                            </td>
                            <td class="label">
                                Rank:
                            </td>
                            <td class="value">
                                <asp:Label runat="server" ID="RankDisplay" />&nbsp;
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                D
                            </td>
                            <td class="label">
                                Unit:
                            </td>
                            <td class="value">
                                <asp:Label ID="UnitLabel" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                E
                            </td>
                            <td class="label">
                                User Role:
                            </td>
                            <td class="value">
                                <asp:Label ID="UserRoleLabel" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                F
                            </td>
                            <td class="label">
                                Work Email:
                            </td>
                            <td class="value">
                                <asp:Label ID="EmailLabel" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                G
                            </td>
                            <td class="label">
                                Personal Email:
                            </td>
                            <td class="value">
                                <asp:Label ID="Email2Label" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                H
                            </td>
                            <td class="label">
                                Unit Email:
                            </td>
                            <td class="value">
                                <asp:Label ID="Email3Label" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                I
                            </td>
                            <td class="label">
                                Phone:
                            </td>
                            <td class="value">
                                <asp:Label ID="PhoneLabel" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                J
                            </td>
                            <td class="label">
                                DSN:
                            </td>
                            <td class="value">
                                <asp:Label ID="DsnLabel" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                K
                            </td>
                            <td class="label">
                                Address:
                            </td>
                            <td class="value">
                                <asp:Label ID="AddressLabel" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                L
                            </td>
                            <td class="label">
                                City:
                            </td>
                            <td class="value">
                                <asp:Label ID="CityLabel" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                M
                            </td>
                            <td class="label">
                                State:
                            </td>
                            <td class="value">
                                <asp:Label ID="StateLabel" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                N
                            </td>
                            <td class="label">
                                Zip:
                            </td>
                            <td class="value">
                                <asp:Label ID="ZipLabel" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                O
                            </td>
                            <td class="label">
                                Receive Email:
                            </td>
                            <td class="value">
                                <asp:Label ID="RecieveEmailLabel" runat="server"></asp:Label>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                            </td>
                            <td class="label">
                                Action:
                            </td>
                            <td>
                                <asp:Button ID="PrevButton" runat="server" Text="Previous" ValidationGroup="register" />
                                &nbsp;
                                <asp:Button ID="NextButton" runat="server" Text="Next" ValidationGroup="register" />
                            </td>
                            
                        </tr>
                    </table>
                </div>
            </asp:Panel>
        </div>
    </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFooter" runat="Server">
</asp:Content>
