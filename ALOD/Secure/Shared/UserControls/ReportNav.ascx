<%@ Control Language="VB" AutoEventWireup="false" Inherits="ALOD.Web.UserControls.Secure_Shared_UserControls_ReportNav" Codebehind="ReportNav.ascx.vb" %>

<script type="text/javascript">

    $(document).ready(function() {

        $('.datePicker').datepicker(calendarPick("Past", "<%=CalendarImage %>"));


               $('#' + '<%=Me.ClientID%>' + '_ReportButton').click(function() {
               //Clear the error
               $('#' + '<%=Me.ClientID%>' + '_errorLbl').text("");
               var beginTxt = '#' + '<%=Me.ClientID%>' + '_txtBeginDate';
               var endTxt = '#' + '<%=Me.ClientID%>' + '_txtEndDate';
           
            
            if ($(beginTxt).val().trim() === ''
                    && $(endTxt).val().trim() === '') {
                var dtEnd = new Date();
                var dtBegin = new Date(dtEnd.getFullYear(), dtEnd.getMonth() - 1, dtEnd.getDay());
                $(beginTxt).val($.datepicker.formatDate("mm/dd/yy", dtBegin));
                $(endTxt).val($.datepicker.formatDate("mm/dd/yy", dtEnd));
                return true;

            }

            if ($(beginTxt).val().trim() === ''
                    || $(endTxt).val().trim() === '') {
           
                $('#' + '<%=Me.ClientID%>' + '_errorLbl').text("Please enter a valid set of dates or leave date fields empty to default to the last month.").css("color","red");
                return false;
            }

            if (Date.parse ($(beginTxt).val().trim()) 
                 > Date.parse($(endTxt).val().trim()) ) {

                $('#' + '<%=Me.ClientID%>' + '_errorLbl').text("End date should be  after the Begin date.").css("color", "red");
                return false;
            }




   

        }

            );

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(EndRequestHandler);

        function EndRequestHandler(sender, args) {
            $('.datePicker').datepicker(calendarPick("Past", "<%=CalendarImage %>"));
        }

    }
    );
    
</script>

<asp:Panel runat="server" ID="InputPanel" CssClass="dataBlock">
    <div class="dataBlock-header">
        1 - Report Options
    </div>
    <div class="dataBlock-body">
        <asp:UpdatePanel runat="server" ID="upnlMemberLookup" ChildrenAsTriggers="true">
            <ContentTemplate>
                <table runat="server" id="tbl1">
                    <tr runat="server" id="tr1">
                        <td class="number">
                            <asp:Label ID="ReportLabel1" runat="server" Text="A"></asp:Label>
                        </td>
                
                        <td class="label">
                            Select Unit:
                 
                        </td>
                         <td class="value">
                            <asp:DropDownList ID="ddlUnit" DataTextField="Name" DataValueField="Value" runat="server">
                            </asp:DropDownList>
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
                    <tr runat="server" id="tr3">
                         <td class="number">
                            <asp:Label ID="ReportLabel3" runat="server" Text="C"></asp:Label>
                        </td>
                        <td class="label">
                           Begin Date:
                        </td>
                         <td class="value">
                            <asp:TextBox ID="txtBeginDate" runat="server" CssClass="datePicker" MaxLength="10"
                                onchange="DateCheck(this);" Width="94px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr runat="server" id="tr4">
                         <td class="number">
                       <asp:Label ID="ReportLabel4" runat="server" Text="D"></asp:Label>
                        </td>
                        <td class="label">
                            End Date:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="txtEndDate" runat="server" CssClass="datePicker" MaxLength="10"
                                onchange="DateCheck(this);" Width="94px"></asp:TextBox>
                            &nbsp;
                        </td>
                    </tr>

                    <tr runat="server" id="trRadioButtons">
                        <td class="number">
                            <asp:Label runat="server" ID="lblMemberNameRadioButtons" Text ="E" ></asp:Label>
                        </td>
                        <td class="label">
                            Lookup By:
                        </td>
                        <td class="value">
                            <asp:RadioButton runat="server" ID="rdbSSN" Text="SSN" GroupName="LookupChoice" AutoPostBack="true" Checked="True" />
                            <asp:RadioButton runat="server" ID="rdbName" Text="Name" GroupName="LookupChoice" AutoPostBack="true" />
                        </td>
                    </tr>

                    <tr runat="server" id="tr5">
                         <td class="number">
                            <asp:Label ID="ReportLabel5" runat="server" Text="F"></asp:Label>
                        </td>
                        <td class="label">
                           SSN:
                        </td>
                         <td class="value">
                            <asp:TextBox ID="txtSSN" runat="server" MaxLength="9"></asp:TextBox>
                           SSN search ignores <%--&quot;Select Unit&quot; and --%>dates
                        </td>
                    </tr>

                    <tr runat="server" class="tr-Name" id="trName">
                        <td class="number">
                            <asp:Label runat="server" ID="lblReportMemberName" Text="F"></asp:Label>
                        </td>
                        <td class="label">
                            Member Name (last / first / middle):
                        </td>
                        <td class="value">
                            <asp:TextBox runat="server" ID="txtMemberLastName" MaxLength="50" Width="80px" placeholder="Last Name"></asp:TextBox>
                            <asp:TextBox runat="server" ID="txtMemberFirstName" MaxLength="50" Width="80px" placeholder="First Name"></asp:TextBox>
                            <asp:TextBox runat="server" ID="txtMemberMiddleName" MaxLength="60" Width="80px" placeholder="Middle Name"></asp:TextBox>
                            &nbsp;
                            <asp:Label runat="server" ID="lblMemberNotFound" CssClass="labelRequired" Text="Member Not Found" Visible="false"></asp:Label>
                            &nbsp;
                            <asp:Label runat="server" ID="lblInvalidMemberName" CssClass="labelRequired" Text="Invalid Name" Visible="false"></asp:Label>
                            Member Name search ignores dates
                        </td>
                    </tr>

                    <tr runat="server" id="tr6">
                         <td class="number">
                        <asp:Label ID="ReportLabel6" runat="server" Text="G"></asp:Label>
                        </td>
                        <td class="label">
                           Completion Status:
                        </td>
                         <td class="value">
                            <asp:RadioButtonList ID="rblLodStatus" runat="server" RepeatDirection="Horizontal">
                                <asp:ListItem Selected="True" Value="2">All</asp:ListItem>
                                <asp:ListItem Value="0">Active</asp:ListItem>
                                <asp:ListItem Value="1">Closed</asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
            
                     <tr runat="server" id="tr7">
                         <td class="number">
                        <asp:Label ID="ReportLabel7" runat="server" Text="H"></asp:Label>
                        </td>
                        <td class="label">
                           SortOrder?
                        </td>
                         <td class="value">
                            <asp:DropDownList ID="SortOrdeDDL" runat="server"  >

                                <asp:ListItem Selected="True" Value="LASTNAME">Last Name</asp:ListItem>
                                <asp:ListItem Value="SSN">Last Four of SSN</asp:ListItem>  
                                <asp:ListItem  Value="UNIT">Unit</asp:ListItem>                                   
                            </asp:DropDownList>
                        </td>
                    </tr>

                    <tr runat="server" id="tr8">
                        <td class="number">
                            <asp:Label runat="server" ID="CompoLabel" Text="I"></asp:Label>
                        </td>
                        <td class="label">
                            Compo:
                        </td>
                        <td class="value">
                            <asp:DropDownList ID="CompoSelect" runat="server"></asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </ContentTemplate>
        </asp:UpdatePanel>
        <table runat="server" id="tbl2">
            <tr>
                 <td class="number">
                   <asp:Label ID="ReportLabel8" runat="server" Text="J"></asp:Label>
                </td>
                <td class="label">
                   Action:
                </td>
                 <td class="value">
                    <asp:Button ID="ReportButton" runat="server" Text="Run Report" />
                </td>
            </tr>
             <tr>
                 <td colspan="2"></td><td><asp:Label id="errorLbl" runat="server"  ></asp:Label></td> 
             </tr>
             
        </table>
    </div>
</asp:Panel>
