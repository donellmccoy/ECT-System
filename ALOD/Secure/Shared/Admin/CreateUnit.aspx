<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Admin.Secure_Shared_Admin_CreateUnit" Codebehind="CreateUnit.aspx.vb" %>

<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="ContentFooter">

    <script type="text/javascript">
        
        $(function () {
            $('#srchBox').dialog({
                autoOpen: false,
                modal: true,
                resizable: true,
                width: 550,
                height: 320,
                buttons: {
                    'Select': function () {
                        SetReportingUnit();
                        $(this).dialog('close');
                    },
                    'Cancel': function () {
                        CancelSelection();
                        $(this).dialog('close');
                    }
                }
            });

            $('.open-dialog')
                .on('click', function () {
                    $('#srchBox').dialog('open');
                })
                .on('mouseenter', function () { // Replaces the first function of .hover()
                    $(this).addClass("ui-state-hover");
                })
                .on('mouseleave', function () { // Replaces the second function of .hover()
                    $(this).removeClass("ui-state-hover");
                })
                .on('mousedown', function () {
                    $(this).addClass("ui-state-active");
                })
                .on('mouseup', function () {
                    $(this).removeClass("ui-state-active");
                });

        });


        //Show Searcher
        function showSearcher(title, targetId, targetlbl) {
            initializeUnitSearcher();
            //Set Client controls where unit Id and unit names will be transferred
            element('<%=hdnIdClient.ClientId %>').value = targetId;
            element('<%=hdnNameClient.ClientId %>').value = targetlbl;

        }
        //Client accepted so Set Corresponding reporting units 
        function SetReportingUnit() {
            var srcherId = '<%=unitSearcher.ClientId %>'
            //Id is stroed in a hidden control.Transfer the value from the control
            setSelectedValue(element('<%=hdnIdClient.ClientId %>').value, element(srcherId + '_hdnSelectedUnit').value);
            //Name is stored in a label.Transfer the value from the control

            return false;
        }
        //Client cancelled so ignore the dialog values
        function CancelSelection() {
            var srcherId = '<%=unitSearcher.ClientId %>'
            element(srcherId + '_hdnSelectedUnit').value = "";
            element(srcherId + '_hdnSelectedUnitName').value = "";
            return false;
        }
    
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="Server">
    <!-- Copy this whole div to copy the seacrher-->
    <div id="srchBox" class="hidden" title="Find Unit">
        <lod:unitSearcher ID="unitSearcher"  ActiveOnly ="true"  runat="server" />
    </div>
    <!-- end search control -->
    <div>
        <input type="hidden" id="hdnIdClient" runat="Server" />
        <input type="hidden" id="hdnNameClient" runat="Server" />
        <asp:Label ID="lblPasCode" Visible="false" runat="server"></asp:Label>
    </div>
    <div class="indent">
        <asp:Panel runat="server" ID="ErrorPanel" Visible="false" CssClass="ui-state-error info-block">
            <asp:Image runat="server" ID="Image1" ImageAlign="AbsMiddle" ImageUrl="~/images/warning.gif" />
            <asp:Label runat="server" ID="ErrorMessageLabel" />
        </asp:Panel>
        <asp:Panel runat="server" ID="SearchPanel" CssClass="dataBlock">
            <div class="dataBlock-header" >
                1 - Unit Search
            </div>
            
            <div class="dataBlock-body">
                <table>
                    <tr>
                        <td class="number">
                            A
                        </td>
                        <td class="label">
                            PasCode:
                        </td>
                        <td class="value">
                            <asp:TextBox ID="PascodeSearchBox" runat="server" MaxLength="4" Width="80px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            B
                        </td>
                        <td class="label">
                            Action:
                        </td>
                        <td class="value">
                            <asp:Button runat="server" ID="SearchButton" Text="Search" />
                        </td>
                    </tr>
                </table>
        <br />
                  <asp:GridView ID="UnitGrid" runat="server"   align="center" width="50%" AutoGenerateColumns="False" 
                DataKeyNames="Id">
                <Columns>
                    <asp:BoundField DataField="PasCode" HeaderText="Unit Pascode" />
                    <asp:BoundField DataField="Name" HeaderText="Unit Name" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:ImageButton ImageAlign="AbsMiddle" SkinID="imgUserEdit" ID="lnk_Edit" CommandArgument='<%# Eval("Id") %>'
                                ToolTip="Edit Unit" runat="server" CommandName="EditUnit" />
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                        <ItemStyle HorizontalAlign="Center"></ItemStyle>
                    </asp:TemplateField>
                </Columns>
                <EmptyDataRowStyle Font-Bold="true" CssClass="emptyItem" />
                <EmptyDataTemplate>
                    No unit found CreateUnit</EmptyDataTemplate>
                <PagerStyle HorizontalAlign="Center" VerticalAlign="Bottom"></PagerStyle>
            </asp:GridView>
                </div>
        </asp:Panel> 
      
        <asp:Panel runat="server" ID="InputPanel" Visible="false" CssClass="dataBlock">
            <div class="dataBlock-header" style="padding:5px 0px 0px 4px;">
                1 - Enter New Unit Information
            </div>
            <div class="dataBlock-body">
                <div>
                    <asp:ValidationSummary ID="ValidationSummary1" Font-Bold="true" runat="server" ValidationGroup="create" />
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
                            <asp:TextBox ID="PasCodeLbl" Enabled="false" runat="server">  
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
                            <asp:TextBox ID="txtUnitDescription" Width="70%" runat="server">  
                            </asp:TextBox>
                            <asp:RequiredFieldValidator ControlToValidate="txtUnitDescription" ID="RequiredFieldValidator14"
                                runat="server" ErrorMessage="Please enter unit description" ValidationGroup="create">*
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
                    <tr style="height: 35px;">
                        <td class="number">
                            D
                        </td>
                        <td class="label">
                            UIC (*):
                        </td>
                        <td style="width:800px;" class="value">
                            <asp:TextBox Width="80px" CssClass="fieldNormal" MaxLength="6" ID="txtUIC" runat="server"> 
                            </asp:TextBox>
                            <asp:RequiredFieldValidator ControlToValidate="txtUIC" ID="RequiredFieldValidator7"
                                runat="server" ErrorMessage="Please enter UIC" ValidationGroup="create">*
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
                        <td class="value" style="width:420px;">
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
                        <td style="width:120px;" class="label">
                            Gaining Command :
                        </td>
                        <td style="width:420px;" class="value">
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
                        <td class="label" style="width:120px;">
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
                        <td style="width:120px;" class="label">
                            Component (*):
                        </td>
                        <td style="width:200px;" class="value">
                            <asp:DropDownList CssClass="fieldNormal" DataSourceID="dsCompos" DataTextField="Name"
                                DataValueField="Value" ID="ddlComponent" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                   <!-- <tr>
                        <td class="number">
                            I
                        </td>
                        <td width="120px" class="label">
                            GeoLoc :
                        </td>
                        <td class="value">
                            <asp:TextBox MaxLength="4" CssClass="fieldNormal" Width="80px" ID="geoLocBox" runat="server"> 
                            </asp:TextBox>
                            <ajax:FilteredTextBoxExtender ID="FilteredTextBoxExtender3" TargetControlID="geoLocBox"
                                runat="server" FilterType="Custom,UppercaseLetters,LowercaseLetters,Numbers"
                                ValidChars=" " />
                        </td>
                    </tr> -->
                    <tr>
                        <td class="number">
                            I
                        </td>
                        <td style="width:120px;" class="label">
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
                                runat="server" ErrorMessage="Please enter Address" ValidationGroup="create">*
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
                            City(*) :
                        </td>
                        <td style="width:200px;" class="value">
                            <asp:TextBox Width="90%" CssClass="fieldNormal" ID="txtCity" runat="server">
                            </asp:TextBox>
                            <asp:RequiredFieldValidator InitialValue="" ControlToValidate="txtCity" ID="RequiredFieldValidator1"
                                runat="server" ErrorMessage="Please enter a city" ValidationGroup="create">*
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
                        <td class="value" style="width:250px;">
                            <asp:DropDownList CssClass="fieldNormal" Width="130px" ID="ddlState" DataSourceID="dsStates"
                                DataTextField="Name" DataValueField="Id" runat="server">
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator InitialValue="" ControlToValidate="ddlState" ID="RequiredFieldValidator17"
                                runat="server" ErrorMessage="Please select a state" ValidationGroup="create">*
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
                        <td class="value" style="width:250px;">
                            <asp:TextBox CssClass="fieldNormal" Width="50%" ID="txtZipCode" runat="server">
                            </asp:TextBox>
                            <ajax:FilteredTextBoxExtender ID="FilteredTextBoxExtender10" TargetControlID="txtZipCode"
                                runat="server" FilterType="Custom,Numbers" ValidChars="- " />
                            <asp:RegularExpressionValidator ID="regExp_Zip" runat="server" ControlToValidate="txtZipCode"
                                ErrorMessage="Invalid Zip Code" ValidationExpression="\d{5}(-\d{4})?" ValidationGroup="create">*</asp:RegularExpressionValidator>
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
                                ValidationGroup="create">*</asp:RegularExpressionValidator>
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
                            Action:
                        </td>
                        <td>
                            <asp:Button runat="server" ID="btnCreate" Text="Create Unit" CssClass="ui-state-default ui-corner-all" />
                        </td>
                    </tr>
                </table>UnitService
            </div>
        </asp:Panel>
    </div>
 
        <asp:ObjectDataSource ID ="dsCommandStruct" runat="server"  SelectMethod="GetUnits"  TypeName="ALOD.Data.Services.UnitService">
        </asp:ObjectDataSource>
     <asp:ObjectDataSource ID ="dsOpType" runat="server"  SelectMethod="GetUnitOperationTypes"  TypeName="ALOD.Data.Services.UnitService">
        </asp:ObjectDataSource>
             <asp:ObjectDataSource ID ="dsTimeZones" runat="server"  SelectMethod="GetTimeZones"  TypeName="ALOD.Data.Services.LookupService">
        </asp:ObjectDataSource>
   
    <asp:ObjectDataSource ID ="dsLevel" runat="server"  SelectMethod="GetUnitLevelTypes"  TypeName="ALOD.Data.Services.UnitService">
        </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="dsCompos" runat="server" SelectMethod="GetCompos" TypeName="ALOD.Data.Services.LookupService">
    </asp:ObjectDataSource>
    <asp:ObjectDataSource ID="dsStates" runat="server" SelectMethod="GetStates" TypeName="ALOD.Data.Services.LookupService">
    </asp:ObjectDataSource>
    

</asp:Content>
