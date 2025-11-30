<%@ Page Title="" Language="C#" MasterPageFile="~/Secure/SC_Congress/SC_Congress.master"
    AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.CI.Secure_sc_ci_HQTech" CodeBehind="HQTech.aspx.cs" %>

<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Register Src="~/secure/Shared/UserControls/FindingsControl.ascx" TagName="Findings"
    TagPrefix="uc" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_Congress/SC_Congress.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="../Shared/UserControls/SignatureCheck.ascx" TagName="SignatureCheck"
    TagPrefix="uc1" %>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    <div class="dataBlock">
        <div class="dataBlock-header">
            HQ Technician - Congressional Inquiry Info:
        </div>        
            <table style="border-spacing: 5px">
                <tr>
                    <td>
                        &nbsp;&nbsp;<asp:Label runat="server" ID="lblTMTNumber" LabelFor="TMTNumber" Text="TMT Number" />
                    </td>
                    <td>
                        <table>
                            <tr>
                                <td>
                                    <asp:Label runat="server" ID="lblAFRC" Text="AFRC" />
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="TMTNumber" Text="" onkeypress="return checkFormat(this, event, 'else', '');"
                                        MaxLength="15" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;&nbsp;<asp:Label runat="server" ID="lblTMTRecDate" LabelFor="TMTRecDate" Text="TMT Receive Date" />
                    </td>
                    <td>
                        &nbsp;&nbsp;<asp:TextBox runat="server" ID="TMTRecDate" MaxLength="10" onchange="DateCheck(this);"
                            Text="" />
                    </td>
                </tr>
                <tr>
                    <td>
                        &nbsp;&nbsp;<asp:Label runat="server" ID="lblSuspenseDate" LabelFor="SuspenseDate"
                            Text="Suspense Date" />
                    </td>
                    <td>
                        &nbsp;&nbsp;<asp:TextBox runat="server" ID="SuspenseDate" MaxLength="10" onchange="DateCheck(this);"
                            Text="" />
                    </td>
                </tr>
            </table>
            <br />        
    </div>
    <input type="hidden" id="hdnInitialHash" runat="Server" />
    <input type="hidden" id="hdnOldControlList" runat="Server" />
    <input type="hidden" id="page_refid" runat="Server" />
    <input type="hidden" id="page_readOnly" runat="Server" />
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="Server">
    <script type="text/javascript">

        var isReadOnly = false;

        $(function () {

            isReadOnly = element('<%= page_readOnly.ClientId %>').value == "0";

            $('.datePicker').datepicker(calendarPick("Past", "<%=CalendarImage %>"));

            $('.datePickerFuture').datepicker(calendarPick("Future", "<%=CalendarImage %>"));

        });

            
    </script>
</asp:Content>
