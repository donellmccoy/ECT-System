<%@ Control AutoEventWireup="false" Inherits="ALOD.Web.UserControls.Secure_Shared_UserControls_popupUpdateMessage" Language="VB" Codebehind="popupUpdateMessage.ascx.vb" %>
<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>

    <asp:Panel runat="server" ID="pnlBannerConfiguration" CssClass="dataBlock">
            <div class="dataBlock-header">
                <asp:Label ID="title" runat="server" Text="Create New Message"></asp:Label>
            </div>

            <div class="dataBlock-body">
                <table style="width:100%;">
                    <tr>
                        <td class="number">
                            A
                        </td>
                        <td class="label">
                            <asp:Label id="lblTitle" runat="server" Text="Title:"></asp:Label>
                        </td>
                        <td class="value">
                            <asp:TextBox ID="txtTitle" MaxLength="50" runat="server" Width="300px"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="valTitle" runat="server" ValidationGroup="Message" ControlToValidate="txtTitle" ErrorMessage="Title is a required field."></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            B
                        </td>
                        <td class="label">
                            <asp:Label id="lblFrom" runat="server" Text="From:"></asp:Label>
                        </td>
                        <td class="value">
                            <asp:TextBox ID="txtFrom" MaxLength="50" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="valFrom" runat="server" ValidationGroup="Message" ControlToValidate="txtFrom" ErrorMessage="Name is a required field."></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            C
                        </td>
                        <td class="label">
                            <asp:Label id="lblStart" runat="server" Text="Start Date:"/>
                        </td>
                        <td class="value">
                            <asp:TextBox ID="txtStart" runat="server" Width="100" MaxLength="10"></asp:TextBox>
                            <asp:Image ID="imgCalendar1" runat="server" SkinID="imgCalendar" />
                            <ajax:CalendarExtender ID="ceProbDate" runat="server" PopupButtonID="imgCalendar1" SkinID="imgCalendar" TargetControlID="txtStart"> </ajax:CalendarExtender>
                            <asp:RequiredFieldValidator ID="valStart" ValidationGroup="Message" runat="server" ControlToValidate="txtStart" ErrorMessage="Start Date is a required field."></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            D
                        </td>
                        <td class="label">
                            <asp:Label id="lblEnd" runat="server" Text="End Date:"/>
                        </td>
                        <td class="value">
                            <asp:TextBox ID="txtEnd" runat="server" Width="100" MaxLength="10"></asp:TextBox>
                            <asp:Image ID="ImageButton1" runat="server" SkinID="imgCalendar" />
                            <ajax:CalendarExtender ID="CalendarExtender1" runat="server" PopupButtonID="ImageButton1" SkinID="imgCalendar" TargetControlID="txtEnd"></ajax:CalendarExtender>
                            <asp:RequiredFieldValidator id="val2" runat="server" ValidationGroup="Message" ControlToValidate="txtEnd" ErrorMessage="End Date is a required field."></asp:RequiredFieldValidator>

                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            E
                        </td>
                        <td class="label">
                            <asp:Label id="lblMessage" runat="server" Text="Message:"/>
                        </td>
                        <td class="value">
                            <asp:TextBox ID="txtMessage" TextMode="MultiLine" width="450px" height="100px" runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator id="valMessage" runat="server" ValidationGroup="Message" ControlToValidate="txtMessage" ErrorMessage="Message is a required field."></asp:RequiredFieldValidator>
                        </td>
                        
                    </tr>
                </table>
                <asp:CheckBox ID="chkPopup" runat="server" Text="Popup" />

                <br />
                <br />

                <fieldset>
                    <legend style="color:Maroon">
                    User Groups
                    </legend>
                    <asp:CheckBox ID="chkAll" runat="server" AutoPostBack="true" Text="All Groups" />
                    <br />
                    <table style="text-align:center">
                        <tr>
                            <td>
                                <asp:ListBox runat="server" ID="lstAvailGroups" Width="250px" SelectionMode="Multiple" Height="100px" />
                            </td>
                            <td style="text-align:center;">
                                <asp:Button ID="btnUnAssign" runat="server" Text="<-" />
                                <asp:Button ID="btnAssign" runat="server" Text="->" />
                            </td>
                            <td>
                                <asp:ListBox runat="server" SelectionMode="Multiple" Width="250px" ID="lstAssignGroups" Height="100px" />
                            </td>
                        </tr>
                    </table>
                </fieldset>

                <div style="text-align:right;width:100%">
                    <asp:Button ID="btnCancel" CausesValidation="false" runat="server" Text="Cancel" />
                    <asp:Button ID="btnSave" ValidationGroup="Message" runat="server" Text="Save" />
                </div>
            </div>
        </asp:Panel>


