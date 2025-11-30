<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Admin.Secure_Shared_Admin_EditPasCode" Codebehind="EditPasCode.aspx.vb" %>

<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="ContentFooter">

    <script type="text/javascript">

        $(document).ready(function() {
            // Initialize dialog only if the element exists
            if ($('#srchBox').length > 0) {
                $('#srchBox').dialog({
                    autoOpen: false,
                    modal: true,
                    resizable: true,
                    width: 550,
                    height: 320,
                    buttons: {
                        'Select': function() {
                            SetReportingUnit();
                            $(this).dialog('close');
                        },
                        'Cancel': function() {
                            CancelSelection();
                            $(this).dialog('close');
                        }
                    }
                });
            }

            $('.open-dialog').click(function() {
                if ($('#srchBox').length > 0) {
                    $('#srchBox').dialog('open');
                }
            })
            .hover(
            function() {
                $(this).addClass("ui-state-hover");
            },
            function() {
                $(this).removeClass("ui-state-hover");
            }
            )
            .mousedown(function() {
                $(this).addClass("ui-state-active");
            })
            .mouseup(function() {
                $(this).removeClass("ui-state-active");
            });
        });

        //Show Searcher
        function showSearcher(title, targetId, targetlbl) {
            if (typeof initializeUnitSearcher === 'function') {
                initializeUnitSearcher();
            }
            //Set Client controls where unit Id and unit names will be transferred
            var hiddenIdClient = element('<%=hdnIdClient.ClientId %>');
            var hiddenNameClient = element('<%=hdnNameClient.ClientId %>');
            if (hiddenIdClient) hiddenIdClient.value = targetId;
            if (hiddenNameClient) hiddenNameClient.value = targetlbl;
        }
        
        //Client accepted so Set Corresponding reporting units 
        function SetReportingUnit() {
            var srcherId = '<%=unitSearcher.ClientId %>';
            var hiddenIdClient = element('<%=hdnIdClient.ClientId %>');
            if (hiddenIdClient && hiddenIdClient.value) {
                var selectedUnitElement = element(srcherId + '_hdnSelectedUnit');
                if (selectedUnitElement && typeof setSelectedValue === 'function') {
                    setSelectedValue(hiddenIdClient.value, selectedUnitElement.value);
                }
            }
            return false;
        }
        
        //Client cancelled so ignore the dialog values
        function CancelSelection() {
            var srcherId = '<%=unitSearcher.ClientId %>';
            var selectedUnit = element(srcherId + '_hdnSelectedUnit');
            var selectedUnitName = element(srcherId + '_hdnSelectedUnitName');
            if (selectedUnit) selectedUnit.value = "";
            if (selectedUnitName) selectedUnitName.value = "";
            return false;
        }
    
    </script>

</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <!-- Copy this whole div to copy the seacrher-->

    <div id="srchBox" class="hidden" title="Find Unit">
        <lod:unitSearcher ID="unitSearcher"  ActiveOnly ="true" runat="server" />
    </div>
    <!-- end search control -->
    <input type="hidden" id="hdnIdClient" runat="Server" />
    <input type="hidden" id="hdnNameClient" runat="Server" />
    <div style="text-align: right;">
        <asp:LinkButton ID="lnkManageUnits" runat="server" Visible="true" >
        <asp:Image runat="server" ID="ReturnImage1" AlternateText="Return to manage units"
            ImageAlign="AbsMiddle" ImageUrl="~/images/arrow_left.gif" />
        Return to Manage Units
    </asp:LinkButton>
    </div>

    <asp:Label ID="lblUnitId" Visible="false" runat="server"></asp:Label>
    <div>
        <input type="hidden" id="Hidden1" runat="Server" />
        <input type="hidden" id="Hidden2" runat="Server" />
        <asp:Label ID="Label1" Visible="false" runat="server"></asp:Label>
    </div>
    <div class="indent">
        <asp:Panel runat="server" ID="ErrorPanel" Visible="false" CssClass="ui-state-error info-block">
            <asp:Image runat="server" ID="Image1" ImageAlign="AbsMiddle" ImageUrl="~/images/warning.gif" />
            <asp:Label runat="server" ID="ErrorMessageLabel" />
        </asp:Panel>
        <asp:Panel runat="server" ID="FeedbackPanel" Visible="false" CssClass="ui-state-highlight info-block msg-panel">
            <asp:Label runat="server" ID="FeedbackMessageLabel" />
        </asp:Panel>
        <asp:Panel runat="server" ID="InputPanel" CssClass="dataBlock">
            <div class="dataBlock-header">
                1 - Edit Unit Information
            </div>
            <div class="dataBlock-body">
                <div>
                    <asp:ValidationSummary ID="ValidationSummary1" Font-Bold="true" runat="server" ValidationGroup="edit" />
                </div>
                <table>
                    <tr>
                        <td class="number">
                            A
                        </td>
                        <td class="label">
                            PasCode:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="txtPasCode" Enabled="false" runat="server">  
                            </asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            B
                        </td>
                        <td class="label">
                            Unit Description:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="txtUnitDescription" Width="450px" runat="server">  
                            </asp:TextBox>
                            <asp:RequiredFieldValidator ControlToValidate="txtUnitDescription" ID="RequiredFieldValidator14"
                                runat="server" ErrorMessage="Please enter unit description " ValidationGroup="edit">*
                            </asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            C
                        </td>
                        <td class="label">
                            Number/Kind/Type/Det:
                        </td>
                        <td style="width:300px;" class="value">
                            <asp:TextBox ID="txtUnitNbr" MaxLength="4" CssClass="fieldNormal" Width="50px" runat="server"></asp:TextBox>
                            <ajax:FilteredTextBoxExtender ID="FilteredTextBoxExtender7" TargetControlID="txtUnitNbr"
                                runat="server" FilterType="Custom,UppercaseLetters,LowercaseLetters,Numbers"
                                ValidChars=" " />
                            <asp:TextBox ID="txtUnitKnd" MaxLength="5" CssClass="fieldNormal" Width="50px" runat="server"></asp:TextBox>
                            <ajax:FilteredTextBoxExtender ID="FilteredTextBoxExtender6" TargetControlID="txtUnitKnd"
                                runat="server" FilterType="Custom,UppercaseLetters,LowercaseLetters,Numbers"
                                ValidChars=" " />
                            <asp:TextBox ID="txtUnitType" CssClass="fieldNormal" MaxLength="2" Width="50px" runat="server"></asp:TextBox>
                            <ajax:FilteredTextBoxExtender ID="FilteredTextBoxExtender5" TargetControlID="txtUnitType"
                                runat="server" FilterType="Custom,UppercaseLetters,LowercaseLetters,Numbers"
                                ValidChars=" " />
                            <asp:TextBox ID="txtUnitDet" CssClass="fieldNormal" MaxLength="4" Width="50px" runat="server"></asp:TextBox>
                            <ajax:FilteredTextBoxExtender ID="FilteredTextBoxExtender4" TargetControlID="txtUnitDet"
                                runat="server" FilterType="Custom,UppercaseLetters,LowercaseLetters,Numbers"
                                ValidChars=" " />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            D
                        </td>
                        <td class="label">
                            UIC (*):
                        </td>
                        <td style="width:300px;" class="value">
                            <asp:TextBox Width="80px" CssClass="fieldNormal" MaxLength="6" ID="txtUIC" runat="server"> 
                            </asp:TextBox>
                            <asp:RequiredFieldValidator ControlToValidate="txtUIC" ID="RequiredFieldValidator7"
                                runat="server" ErrorMessage="Please enter UIC" ValidationGroup="edit">*
                            </asp:RequiredFieldValidator>
                            <ajax:FilteredTextBoxExtender ID="FilteredTextBoxExtender2" TargetControlID="txtUIC"
                                runat="server" FilterType="Custom,UppercaseLetters,LowercaseLetters,Numbers"
                                ValidChars=" " />
                            Base Code :
                            <asp:TextBox Width="80px" MaxLength="2" CssClass="fieldNormal" ID="txtBaseCode" runat="server"> 
                            </asp:TextBox>
                            <ajax:FilteredTextBoxExtender ID="FilteredTextBoxExtender8" TargetControlID="txtBaseCode"
                                runat="server" FilterType="Custom,UppercaseLetters,LowercaseLetters,Numbers"
                                ValidChars=" " />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            E
                        </td>
                        <td class="label">
                            Unit Reports to :
                        </td>
                        <td class="value" style="width:500px;">
                            <asp:DropDownList CssClass="fieldNormal" Width="80%" ID="ddlReportToUnits" DataSourceID="dsCommandStruct"
                                DataTextField="LONG_NAME" DataValueField="CS_ID" runat="server">
                            </asp:DropDownList>
                            <asp:Button ID="btnFindReporting" Text="Find" runat="server" CssClass="open-dialog ui-state-default ui-corner-all" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            F
                        </td>
                        <td class="label">
                            Gaining Command :
                        </td>
                        <td style="width:500px;" class="value">
                            <asp:DropDownList CssClass="fieldNormal" Width="80%" ID="ddlGainingCommand" DataSourceID="dsCommandStruct"
                                DataTextField="LONG_NAME" DataValueField="CS_ID" runat="server">
                            </asp:DropDownList>
                            <asp:Button ID="btnFindGaining" Text="Find" runat="server" CssClass="open-dialog ui-state-default ui-corner-all" />
                        </td>
                    </tr>
                    <tr>
                      <td class="number">
                            G
                        </td>
                        <td class="label">
                            Unit Level :
                        </td>
                        <td class="value">
                            <asp:DropDownList CssClass="fieldNormal" ID="ddlUnitLevel" runat="server" DataSourceID="dsLevel"
                                DataValueField="LevelID" DataTextField="Description">
                            </asp:DropDownList>
                            Operation Type :
                            <asp:DropDownList CssClass="fieldNormal" ID="ddlOperationType" DataSourceID="dsOpType"
                                DataValueField="OperationTypeId" DataTextField="Description" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            H
                        </td>
                        <td class="label">
                            Component (*):
                        </td>
                        <td style="width:200px;" class="value">
                            <asp:DropDownList CssClass="fieldNormal" DataSourceID="dsCompos" DataTextField="Name"
                                DataValueField="Value" ID="ddlComponent" Width="80%" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                   <!-- <tr>
                        <td class="number">
                            I
                        </td>
                        <td class="label">
                            GeoLoc :
                        </td>
                        <td class="value">
                            <asp:TextBox MaxLength="4" CssClass="fieldNormal" Width="80px" ID="geoLocBox" runat="server"> 
                            </asp:TextBox>
                        </td>
                    </tr> -->
                    <tr>
                        <td class="number">
                            I
                        </td>
                        <td class="label">
                            Time Zone :
                        </td>
                        <td class="value">
                            <asp:DropDownList CssClass="fieldNormal" ID="ddlTimeZone" DataSourceID="dsTimeZones"
                                DataTextField="Description" DataValueField="ZoneId" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                   <!-- <tr>
                        <td class="number">
                            K
                        </td>
                        <td class="label">
                            Physical Exam Unit :
                        </td>
                        <td class="value">
                            <asp:RadioButtonList CssClass="fieldNormal" ID="rblPhysicaUnit" RepeatDirection="Horizontal"
                                runat="server">
                                <asp:ListItem Text="Yes" Value="Y"> </asp:ListItem>
                                <asp:ListItem Text="No" Value="N"> </asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                    </tr> 
                    <tr> 
                        <td class="number">
                            L
                        </td>
                        <td class="label">
                            Use RCPHRA Scheduler :
                        </td>
                        <td class="value">
                            <asp:RadioButtonList CssClass="fieldNormal" ID="rblRCPHRAScheduler" RepeatDirection="Horizontal"
                                runat="server">
                                <asp:ListItem Text="Yes" Value="Yes"> </asp:ListItem>
                                <asp:ListItem Text="No" Value="No"> </asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                    </tr> -->
                    <tr> 
                        <td class="number">
                            J
                        </td>
                        <td class="label">
                            Address 1 (*):
                        </td>
                        <td class="value" style="width:250px;">
                            <asp:TextBox ID="txtAddr1" CssClass="fieldNormal" Width="90%" runat="server">
                            </asp:TextBox>
                            <asp:RequiredFieldValidator ControlToValidate="txtAddr1" ID="RequiredFieldValidator15"
                                runat="server" ErrorMessage="Please enter Address" ValidationGroup="edit">*
                            </asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            K
                        </td>
                        <td class="label">
                            Address 2 :
                        </td>
                        <td class="value" style="width:250px;">
                            <asp:TextBox ID="txtAddr2" Width="90%" CssClass="fieldNormal" runat="server">
                            </asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            L
                        </td>
                        <td class="label">
                            City (*):
                        </td>
                        <td style="width:200px;" class="value">
                            <asp:TextBox Width="90%" CssClass="fieldNormal" ID="txtCity" runat="server">
                            </asp:TextBox>
                            <asp:RequiredFieldValidator InitialValue="" ControlToValidate="txtCity" ID="RequiredFieldValidator1"
                                runat="server" ErrorMessage="Please enter a city" ValidationGroup="edit">*
                            </asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            M
                        </td>
                        <td class="label">
                            State (*):
                        </td>
                        <td class="value" style="width:200px;">
                            <asp:DropDownList CssClass="fieldNormal" Width="130px" ID="ddlState" DataSourceID="dsStates"
                                DataTextField="Name" DataValueField="Id" runat="server">
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator InitialValue="" ControlToValidate="ddlState" ID="RequiredFieldValidator17"
                                runat="server" ErrorMessage="Please select a state" ValidationGroup="edit">*
                            </asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            N
                        </td>
                        <td class="label">
                            Country :
                        </td>
                        <td style="width:120px;" class="value">
                            <asp:TextBox CssClass="fieldNormal" Width="80%" ID="txtCountry" runat="server">
                            </asp:TextBox>
                            <ajax:FilteredTextBoxExtender ID="FilteredTextBoxExtender9" TargetControlID="txtCountry"
                                runat="server" FilterType="Custom,UppercaseLetters,LowercaseLetters" ValidChars=" " />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            O
                        </td>
                        <td class="label">
                            Zipcode :
                        </td>
                        <td class="value">
                            <asp:TextBox CssClass="fieldNormal" Width="130px" ID="txtZipCode" runat="server">
                            </asp:TextBox>
                            <ajax:FilteredTextBoxExtender ID="FilteredTextBoxExtender10" TargetControlID="txtZipCode"
                                runat="server" FilterType="Custom,Numbers" ValidChars="- " />
                            <asp:RegularExpressionValidator ID="regExp_Zip" runat="server" ControlToValidate="txtZipCode"
                                ErrorMessage="Invalid Zip Code" ValidationExpression="\d{5}(-\d{4})?" ValidationGroup="edit">*</asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            P
                        </td>
                        <td class="label">
                            Email Address:
                        </td>
                        <td style="width:250px;" class="value">
                            <asp:TextBox CssClass="fieldNormal" ID="txtEmail" Width="90%" runat="server">
                            </asp:TextBox>
                            <asp:RegularExpressionValidator ID="rgExp_Email" runat="server" ControlToValidate="txtEmail"
                                ErrorMessage="Please enter a valid email address" ValidationExpression="([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})"
                                ValidationGroup="edit">*</asp:RegularExpressionValidator>
                        </td>
                    </tr>
                    
                      <tr>
                        <td class="number">
                            Q
                        </td>
                        <td class="label">
                           InActive:
                        </td>
                        <td>
                            <asp:CheckBox runat="server" ID="InActiveCheckBox"  />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            R
                        </td>
                        <td class="label">
                            Is Collocated:
                        </td>
                        <td>
                            <asp:CheckBox runat="server" ID="chkIsCollocated"/>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            S
                        </td>
                        <td class="label">
                            Action:
                        </td>
                        <td>
                            <asp:Button runat="server" ID="btnUpdate" Text="Update" CssClass="ui-state-default ui-corner-all" />
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <br />
        <asp:Panel runat="server" ID="reportingPanel" CssClass="dataBlock">
            <div class="dataBlock-header">
                2 - Edit Unit Reporting
            </div>
            <br />
            <div class="dataBlock-body">
                <asp:GridView ID="gvReporting" Width="500px" runat="server" AutoGenerateColumns="False"
                    EmptyDataText="No Record Found" DataKeyNames="cs_id">
                    <Columns>
                        <asp:TemplateField Visible="false" HeaderText="Hierarchy Type">
                            <ItemTemplate>
                                <asp:Label ID="lblChainType" runat="server" Text='<%# Bind("Chain_Type") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="200px" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Hierarchy Type">
                            <ItemTemplate>
                                <asp:Label ID="lblChainDescription" runat="server" Text='<%# Bind("Chain_Description") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="200px" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Unit to Report To">
                            <ItemTemplate>
                                <asp:Label ID="lblUnitName" runat="server" Text='<%# Eval("parent_long_name") + " (" + Eval("parent_pas") + ")" %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="300px" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <br />
                <table style="width:500px;">
                    <tr>
                        <td class="align-right" style="width:500px;">
                            <asp:Button runat="server" Width="140px" ID="btnEdit" CssClass="ui-state-default ui-corner-all"
                                Text="Manage Reporting" />
                        </td>
                </table>
            </div>
        </asp:Panel>
        <br />
    </div>
    <br />
    <asp:ObjectDataSource ID="dsCommandStruct" runat="server" SelectMethod="GetUnits"
        TypeName="ALOD.Data.Services.UnitService"></asp:ObjectDataSource>
    <asp:ObjectDataSource ID="dsOpType" runat="server" SelectMethod="GetUnitOperationTypes"
        TypeName="ALOD.Data.Services.UnitService"></asp:ObjectDataSource>
    <asp:ObjectDataSource ID="dsTimeZones" runat="server" SelectMethod="GetTimeZones"
        TypeName="ALOD.Data.Services.LookupService"></asp:ObjectDataSource>
    <asp:ObjectDataSource ID="dsLevel" runat="server" SelectMethod="GetUnitLevelTypes"
        TypeName="ALOD.Data.Services.UnitService"></asp:ObjectDataSource>
    <asp:ObjectDataSource ID="dsCompos" runat="server" SelectMethod="GetCompos" TypeName="ALOD.Data.Services.LookupService">
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="dsStates" runat="server" SelectMethod="GetStates" TypeName="ALOD.Data.Services.LookupService">
    </asp:ObjectDataSource>
</asp:Content>
