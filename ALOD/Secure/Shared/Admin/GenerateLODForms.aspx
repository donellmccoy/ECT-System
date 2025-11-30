<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Admin.GenerateLODForms" Codebehind="GenerateLODForms.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <style type="text/css">
        .highlightSuccess
        {
            color:green;
        }
        .highlightFailure
        {
            color:red;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" Runat="Server">
    <div class="indent">
        <asp:Panel runat="server" ID="pnlInput" CssClass="dataBlock">
            <div class="dataBlock-header">
                1 - Specify Case
            </div>
            <div class="dataBlock-body">
                <table>
                    <tr>
                        <td class="number">
                            A
                        </td>
                        <td class="label">
                            Case ID:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="txtCaseId" runat="server" MaxLength="14" Width="130px"></asp:TextBox>
                            <asp:RequiredFieldValidator runat="server" ID="rfvCaseId" Display="Dynamic" ControlToValidate="txtCaseId" ValidationGroup="input" ErrorMessage="Case Id Required" Text="Please Enter a Case Id"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="regxCaseId" runat="server" ControlToValidate="txtCaseId"
                                Display="Dynamic" ErrorMessage="Invalid Value." ValidationExpression="\d{8}-\d{3}(-[A-Z]{1})*"
                                ValidationGroup="input" SetFocusOnError="True" Text="Please Enter a Valid Case Id"></asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            B
                        </td>
                        <td class="label">
                            Electronic IO Signature:
                        </td>
                        <td class="value">
                            <asp:CheckBox runat="server" ID="chkIOSig" Text="" />
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
                            <asp:Button runat="server" ID="btnSubmin" Text="Submit" ValidationGroup="input" CausesValidation="true" />
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlResults" CssClass="dataBlock" Visible="false">
            <div class="dataBlock-header">
                2 - Results
            </div>
            <div class="dataBlock-body">
                <asp:Label runat="server" ID="lblResults" Text="Results go here..." /> <br />
                <%--<asp:LinkButton ID="lnkCaseId" runat="server" Text="Case Id Goes Here..." Visible ="false" />--%>
                <asp:HyperLink runat="server" ID="lnkCaseId" Text="Case Id Goes Here..." Visible ="false" />
                <asp:ImageButton ImageAlign="AbsMiddle" runat="server" ID="imgbCaseDocument" ImageUrl="~/images/pdf.ico" Visible="false" />
            </div>
        </asp:Panel>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFooter" Runat="Server">
</asp:Content>
