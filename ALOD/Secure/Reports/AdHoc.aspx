<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Reports.Secure_Reports_AdHoc" MaintainScrollPositionOnPostback="true" Codebehind="AdHoc.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        .fieldButton
        {
            width: 80px;
            margin: 2px 4px;
        }
        .hidden
        {}
        .style1
        {
            width: 698px;
        }
        .gridViewStyle1
        {
            width: 120px;
        }

        .groupLeft-3
        {
            width:32%;
            display:inline-block;
            float:left;
            text-align:center;
        }

        .groupCenter-3
        {
            width:32%;
            display:inline-block;
            float:left;
            text-align:center;
        }

        .groupRight-3
        {
            width:32%;
            display:inline-block;
            float:right;
            text-align:center;
        }

        .groupLeft-2
        {
            width:15%;
            display:inline-block;
            float:left;
        }

        .groupRight-2
        {
            width:85%;
            display:inline-block;
            float:right;
        }

        .phFormOutputButton
        {
            margin: 3px 2px 3px 2px;
            width:175px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="Server">
<!--new added-->
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
                    SetReportingUnit($('#srchBox').data('isEdit'));
                    $(this).dialog('close');
                },
                'Cancel': function () {
                    CancelSelection();
                    $(this).dialog('close');
                }
            }
        });

        $('.open-dialog').on('click', function () {
            $('#srchBox').dialog('open');
        })
            .on('mouseenter', function () {
                $(this).addClass("ui-state-hover");
            })
            .on('mouseleave', function () {
                $(this).removeClass("ui-state-hover");
            })
            .on('mousedown', function () {
                $(this).addClass("ui-state-active");
            })
            .on('mouseup', function () {
                $(this).removeClass("ui-state-active");
            });
    });


    function validateUnit() {
        if (element('<%=SrcUnitIdHdn.ClientId %>').value == "") {
            $(element('<%=lblUnitMsg.ClientId %>')).removeClass("hidden");
        }
        else {
            $(element('<%=lblUnitMsg.ClientId %>')).addClass("hidden");
        }
    }
    //Show Searcher
    function showSearcher(title, targetId, targetlbl) {
        initializeUnitSearcher();
        //element('srhctitle').innerHTML = "Reporting unit for Hierarchy     '" + title + "'";
        //Set Client controls where unit Id and unit names will be transferred
        element('<%=hdnIdClient.ClientId %>').value = targetId;
        element('<%=hdnNameClient.ClientId %>').value = targetlbl;
        $('#srchBox').data('isEdit', false).dialog('open');
    }

    //Show QueryGrid GridView Unit Searcher
    function oldshowGridViewSearcher(title, targetClass, targetlblClass) {
        initializeUnitSearcher();

        //Set Client controls where unit Id and unit names will be transferred
        element('<%=hdnIdClient.ClientId %>').value = $('tr.gridViewEdit > td > ' + targetClass).attr('id')
        element('<%=hdnNameClient.ClientId %>').value = $('tr.gridViewEdit > td > ' + targetlblClass).attr('id')

        $('#srchBox').data('isEdit', true).dialog('open');
    }

    //Show QueryGrid GridView Unit Searcher
    function showGridViewSearcher(title, targetClass, targetlblClass) {
        initializeUnitSearcher();

        //Set Client controls where unit Id and unit names will be transferred
        element('<%=hdnIdClient.ClientId %>').value = $(targetClass).attr('id')
        element('<%=hdnNameClient.ClientId %>').value = $(targetlblClass).attr('id')

        $('#srchBox').data('isEdit', true).dialog('open');
    }

    //Client accepted so Set Corresponding reporting units 
    function OriginalSetReportingUnit() {
        var srcherId = '<%=unitSearcher.ClientId %>'
        //Id is stroed in a hidden control.Transfer the value from the control
        element(element('<%=hdnIdClient.ClientId %>').value).value = element(srcherId + '_hdnSelectedUnit').value;
        //Name is stored in a label.Transfer the value from the control
        element(element('<%=hdnNameClient.ClientId %>').value).value = element(srcherId + '_hdnSelectedUnitName').value;
        element('<%=DataChoiceText.ClientId %>').value = element(srcherId + '_hdnSelectedUnitName').value;
        return false;
    }

    //Client accepted so Set Corresponding reporting units 
    function SetReportingUnit(isEdit) {
        var srcherId = '<%=unitSearcher.ClientId %>'
        //Id is stroed in a hidden control.Transfer the value from the control
        element(element('<%=hdnIdClient.ClientId %>').value).value = element(srcherId + '_hdnSelectedUnit').value;
        //Name is stored in a label.Transfer the value from the control
        element(element('<%=hdnNameClient.ClientId %>').value).value = element(srcherId + '_hdnSelectedUnitName').value;

        if (isEdit == false) {
            element('<%=DataChoiceText.ClientId %>').value = element(srcherId + '_hdnSelectedUnitName').value;
        }
        else {
            $('tr.gridViewEdit > td > .gridEditText').val(element(srcherId + '_hdnSelectedUnitName').value);
        }
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

     <div id="unitseracer">
        <!-- Copy this whole div to copy the seacrher-->
        <div id="srchBox" class="hidden" title="Find Unit">
            <lod:unitSearcher ID="unitSearcher"   ActiveOnly="false" runat="server" />
        </div>
        <!-- end search control -->
        <input type="hidden" id="hdnIdClient" runat="Server" />
        <input type="hidden" id="hdnNameClient" runat="Server" />
        <br />
    </div>
    <div id="divSelect">
        <asp:TextBox Width="250px" ReadOnly="True" runat="server" ID="UnitNameTxtO" Visible="false" ></asp:TextBox>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="DataChoiceText"
            ErrorMessage="Unit not selected" ValidationGroup="create">* </asp:RequiredFieldValidator>
        <input type="hidden" id="SrcNameHdn" class="SrcNameHdn" runat="Server" />
        <input type="hidden" id="SrcUnitIdHdn" class="SrcUnitIdHdn" runat="Server" />
        <asp:Button Width="80px" ID="btnFindUnitO" CausesValidation="True" ValidationGroup="create"
            Text="Find Unit" runat="server" Visible="false" ></asp:Button>
            &nbsp;
        <asp:Label ForeColor="Red" runat="server" CssClass="hidden" ID="lblUnitMsg" Text="Unit not selected"></asp:Label>
        <br />
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="create" Width="207px" />
    </div>

    <div class="indent">
        <asp:Panel runat="server" ID="QueryPanel" CssClass="dataBlock">
            <div class="dataBlock-header">
                Current Query</div>
            <div class="dataBlock-body">
                <table>
                    <tr>
                        <td class="number">
                            A
                        </td>
                        <td class="label-small">
                            Report Title:
                        </td>
                        <td class="value">
                            <asp:Label runat="server" ID="ReportTitleLabel" Text="Unnamed Report" />
                        </td>
                    </tr>
                    <!-- add parameter -->
                </table>
                <hr />
                <table>
                    <tr>
                        <td class="number">
                            B
                        </td>
                        <td class="label-small">
                            Case Type:
                        </td>
                        <td class="value">
                            <asp:DropDownList runat="server" ID="ddlCaseType" AutoPostBack="true" />
                        </td>
                    </tr>
<%--                    <tr runat="server" id="trReportType" visible="false">
                        <td class="number">
                            B-1
                        </td>
                        <td class="label-small">
                            Report Type:
                        </td>
                        <td class="value">
                            <asp:RadioButtonList runat="server" ID="rblReportType" AutoPostBack="true" RepeatDirection="Horizontal">
                                <asp:ListItem Value="1" Selected="True">Totals</asp:ListItem>
                                <asp:ListItem Value="2">Cases</asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                    </tr>--%>
                </table>
                <hr />
                <table>
                    <tr runat="server" id="trCriteriaType" visible="false">
                        <td class="number">
                            C
                        </td>
                        <td class="label-small">
                            Select Criteria Type:
                        </td>
                        <td class="value">
                            <asp:RadioButtonList runat="server" ID="rblCriteriaType" AutoPostBack="true" RepeatDirection="Horizontal">
                                <asp:ListItem Value="1" Selected="True">Workflow</asp:ListItem>
                                <asp:ListItem Value="2">PH Form Field</asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            <asp:Label runat="server" ID="lblCriteriaRow" Text="C" />
                        </td>
                        <td class="label-small">
                            Add Criteria:
                        </td>
                        <td class="value">
                            <asp:DropDownList runat="server" ID="SourceSelect" DataTextField="DisplayName" DataValueField="Id" AutoPostBack="true" />
                            <asp:DropDownList runat="server" ID="ddlPHSection" AutoPostBack="false" Visible="false" />
                            <asp:DropDownList runat="server" ID="ddlPHField" AutoPostBack="false" Visible="false" />
                            <asp:DropDownList runat="server" ID="ddlPHFieldType" AutoPostBack="true" Visible="false" />
                        </td>
                    </tr>
                </table>
                <asp:Panel runat="server" ID="SourcePanel" Visible="false">
                    <table>
                        <tr>
                            <td class="number">
                                C-1
                            </td>
                            <td class="label-small">
                                Value:
                            </td>
                            <td class="value">
                                <asp:Panel runat="server" ID="DataChoice" Width="513px">
                                    <asp:DropDownList runat="server" ID="ChoiceOperator">
                                        <asp:ListItem Value="equal">Is</asp:ListItem>
                                        <asp:ListItem Value="notequal">Is Not</asp:ListItem>
                                    </asp:DropDownList>
                                    &nbsp;
                                    <asp:DropDownList runat="server" ID="DataChoiceSelect">
                                    </asp:DropDownList>
                                </asp:Panel>

                                <asp:Panel runat="server" ID="DataNumber">
                                    <asp:DropDownList runat="server" ID="OperatorNumber">
                                        <asp:ListItem Value="equal">Equals</asp:ListItem>
                                        <asp:ListItem Value="less">Less Than</asp:ListItem>
                                        <asp:ListItem Value="greater">Greater Than</asp:ListItem>
                                        <asp:ListItem Value="between">Between</asp:ListItem>
                                    </asp:DropDownList>
                                    &nbsp;
                                    <asp:TextBox runat="server" ID="NumberStart" MaxLength="10" Width="80px" />
                                    <span id="EndNumber" style="display: none;">and
                                        <asp:TextBox runat="server" ID="NumberStop" MaxLength="10" Width="80px" />
                                    </span>
                                </asp:Panel>

                                <asp:Panel runat="server" ID="DataText">
                                    <asp:TextBox runat="server" ID="DataChoiceText" CssClass="DataChoiceTextbox" MaxLength="100" Width="250px" />
                                    <asp:Button Width="80px" ID="btnFindUnit" CausesValidation="True" ValidationGroup="create"
                                    Text="Find Unit" runat="server" Visible="false" ></asp:Button>&nbsp;
                                    <asp:CheckBox ID="cbSubUnit" Text="Include Subordinate Unit" runat="server" onclick="enableTextBox('.DataChoiceTextbox', this.checked, true)" visible="false" />
                                    <asp:Label runat="server" ID="lblSubUnitInfo" Text="(Must Use Find Unit Dialog)" CssClass="label  labelRequired" Visible="false" />
                                    <asp:Label runat="server" ID="lblSSNMsg" Text="Last Four Digits" Visible="false" />
                                </asp:Panel>

                                <asp:Panel runat="server" ID="DataDates">
                                    <table>
                                        <tr>
                                            <td>
                                                <asp:DropDownList runat="server" ID="OperatorDate">
                                                    <asp:ListItem Value="equal">Equals</asp:ListItem>
                                                    <asp:ListItem Value="less">Less Than</asp:ListItem>
                                                    <asp:ListItem Value="greater">Greater Than</asp:ListItem>
                                                    <asp:ListItem Value="between">Between</asp:ListItem>
                                                </asp:DropDownList>
                                            </td>
                                            <td>
                                                <asp:RadioButton Checked="true" runat="server" ID="DateSpan" GroupName="DateType"
                                                    Text="Time Span" />
                                            </td>
                                            <td>
                                                <span id="RangeInput">&nbsp;<asp:TextBox runat="server" ID="RangeStart" MaxLength="6"
                                                    Width="40px" />
                                                    &nbsp;
                                                    <asp:DropDownList ID="RangeStartSelect" runat="server">
                                                        <asp:ListItem Value="D">Days</asp:ListItem>
                                                        <asp:ListItem Value="W">Weeks</asp:ListItem>
                                                        <asp:ListItem Value="M">Months</asp:ListItem>
                                                        <asp:ListItem Value="Y">Years</asp:ListItem>
                                                    </asp:DropDownList>
                                                    &nbsp; <span id="EndRange" style="display: none;">and
                                                        <asp:TextBox runat="server" ID="RangeEnd" MaxLength="6" Width="40px"></asp:TextBox>
                                                        &nbsp;
                                                        <asp:DropDownList ID="RangeEndSelect" runat="server">
                                                            <asp:ListItem Value="D">Days</asp:ListItem>
                                                            <asp:ListItem Value="W">Weeks</asp:ListItem>
                                                            <asp:ListItem Value="M">Months</asp:ListItem>
                                                            <asp:ListItem Value="Y">Years</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </span></span><span id="ExactInput">&nbsp;
                                                        <asp:TextBox runat="server" ID="DateStart" CssClass="datePicker" MaxLength="10" Width="80px" />
                                                        <span id="EndDate" style="display: none;">and
                                                            <asp:TextBox runat="server" ID="DateStop" CssClass="datePicker" MaxLength="10" Width="80px" />
                                                        </span></span>
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>
                                            </td>
                                            <td>
                                                <asp:RadioButton runat="server" ID="DateExact" GroupName="DateType" Text="Exact Dates" />
                                            </td>
                                            <td>
                                            </td>
                                        </tr>
                                    </table>
                                </asp:Panel>

                                <asp:Panel runat="server" ID="DataBool">
                                    <asp:DropDownList runat="server" ID="DataBoolSelect">
                                        <asp:ListItem Value="True">True</asp:ListItem>
                                        <asp:ListItem Value="False">False</asp:ListItem>
                                    </asp:DropDownList>
                                </asp:Panel>

                                <asp:Panel runat="server" ID="pnlPHFormFieldUnsupported">
                                    <asp:Label runat="server" ID="lblPHFormFieldUnsupported" CssClass="labelRequired">This type of PH Form Field is not supported in Ad Hoc Reports.</asp:Label>
                                </asp:Panel>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                C-2
                            </td>
                            <td class="label-small">
                                Type:
                            </td>
                            <td class="value">
                                <asp:RadioButton runat="server" Checked="true" GroupName="ParamType" Text="AND" ID="ParamTypeAnd" />&nbsp;
                                <asp:RadioButton runat="server" GroupName="ParamType" Text="OR" ID="ParamTypeOr" />
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                C-3
                            </td>
                            <td class="label-small">
                                Execute Order:
                            </td>
                            <td class="value">
                                <asp:TextBox runat="server" ID="txtExecuteOrder" MaxLength="3" Width="25px" />
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                C-4
                            </td>
                            <td class="label-small">
                                Action:
                            </td>
                            <td class="value">
                                <asp:Button runat="server" ID="AddParamButton" Text="Add Criteria" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <!-- current paramaters -->
                <hr />
                <table>
                    <tr>
                        <td class="number">
                            D
                        </td>
                        <td class="label-small">
                            Query Criteria:
                        </td>
                        <td class="value" style="width: 700px;">
                            <asp:Label runat="server" ID="NoParamLabel" Text="No Criteria Added" Visible="false" />
                            <asp:Label runat="server" ID="lblWorkflowParams" Text="Workflow Criteria" Visible="false" Font-Bold="true" />
                            <asp:GridView runat="server" ID="QueryGrid" AutoGenerateColumns="false" Width="100%" DataKeyNames="Id">
                                <Columns>
                                    <asp:TemplateField HeaderText="Order">
                                        <EditItemTemplate>
                                            <asp:TextBox runat="server" ID="txtGridExecuteOrder" MaxLength="3" Width="25px" />
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="lblGridExecuteOrder" Text='<%# Eval("ExecuteOrder") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Type">
                                        <EditItemTemplate>
                                            <asp:DropDownList runat="server" ID="ddlGridType">
                                                <asp:ListItem Value="AND">AND</asp:ListItem>
                                                <asp:ListItem Value="OR">OR</asp:ListItem>
                                            </asp:DropDownList>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="lblGridType" Text='<%# Bind("WhereType") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Field">
<%--                                        <EditItemTemplate>
                                            <asp:DropDownList runat="server" ID="ddlGridField" CssClass="gridViewStyle1" DataTextField="DisplayName" DataValueField="Id"></asp:DropDownList>
                                        </EditItemTemplate>--%>
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="lblGridField" Text='<%# Bind("DisplayName") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Operator">
                                        <EditItemTemplate>
                                            <asp:DropDownList runat="server" ID="ddlGridOperator" CssClass="gridEditOperator"></asp:DropDownList>
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="OperatorLabel" Text='<%# Eval("Operator") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="Value">
                                        <EditItemTemplate>
                                            <asp:DropDownList runat="server" ID="ddlGridChoice" CssClass="gridEditChoice" Visible="false"></asp:DropDownList>
                                            <asp:TextBox runat="server" ID="txtGridText" MaxLength="100" Width="250px" CssClass="gridEditText" Visible="false" />
                                            <input type="hidden" id="hdnGridUnitIdClient" class="GridUnitIdClient" runat="Server" />
                                            <input type="hidden" id="hdnGridUnitNameClient" class="GridUnitNameClient" runat="Server" />
                                            <input runat="server" type="button" id="btnGridFindUnit" causesvalidation="true" validationgroup="create" value="Find Unit" visible="false" onclick="showGridViewSearcher('Find Unit','tr.gridViewEdit > td > .GridUnitIdClient','tr.gridViewEdit > td > .GridUnitNameClient'); return false;" />
                                            <asp:CheckBox runat="server" ID="cbGridSubUnit" Text="Include Sub Unit" onclick="enableTextBox('tr.gridViewEdit > td > .gridEditText', this.checked, true)" Visible="false" />
                                            <asp:Label runat="server" ID="lblGridSubUnitInfo" Text="(Must Use Find Unit Dialog)" CssClass="label  labelRequired" Visible="false" />
                                            <asp:DropDownList runat="server" ID="ddlGridBool" Visible="false">
                                                <asp:ListItem Value="True">True</asp:ListItem>
                                                <asp:ListItem Value="False">False</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:TextBox runat="server" ID="txtGridDateStart" CssClass="gridEditStartDate datePicker" MaxLength="10" Width="80px" />
                                            <asp:Label runat="server" ID="lblGridDateAnd" Text="and" CssClass="gridEditDate" Visible="false" />
                                            <asp:TextBox runat="server" ID="txtGridDateEnd" CssClass="gridEditDate datePicker" MaxLength="10" Width="80px" />
                                            <asp:TextBox runat="server" ID="txtGridRangeStart" MaxLength="6" Width="40px" CssClass="gridEditStartRange" Visible="false" />
                                            <asp:DropDownList ID="ddlGridRangeStartSelect" runat="server" Visible="false">
                                                <asp:ListItem Value="D">Days</asp:ListItem>
                                                <asp:ListItem Value="W">Weeks</asp:ListItem>
                                                <asp:ListItem Value="M">Months</asp:ListItem>
                                                <asp:ListItem Value="Y">Years</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:Label runat="server" ID="lblGridRangeAnd" Text="and" CssClass="gridEditRange" Visible="false" />
                                            <asp:TextBox runat="server" ID="txtGridRangeEnd" MaxLength="6" Width="40px" CssClass="gridEditRange" Visible="false" />
                                            <asp:DropDownList ID="ddlGridRangeEndSelect" runat="server" CssClass="gridEditRange" Visible="false">
                                                <asp:ListItem Value="D">Days</asp:ListItem>
                                                <asp:ListItem Value="W">Weeks</asp:ListItem>
                                                <asp:ListItem Value="M">Months</asp:ListItem>
                                                <asp:ListItem Value="Y">Years</asp:ListItem>
                                            </asp:DropDownList>
                                            <asp:TextBox runat="server" ID="txtGridNumberStart" MaxLength="10" Width="80px" CssClass="gridEditStartNumber" Visible="false" />
                                            <asp:Label runat="server" ID="lblGridNumAnd" Text="and" CssClass="gridEditNumber" Visible="false" />
                                            <asp:TextBox runat="server" ID="txtGridNumberEnd" MaxLength="10" Width="80px" CssClass="gridEditNumber" Visible="false" />
                                        </EditItemTemplate>
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="ValueLabel" Text='<%# Eval("StartDisplay") %>' />
                                            <asp:Label runat="server" ID="AndLabel" Text=" and " />
                                            <asp:Label runat="server" ID="EndValueLabel" Text='<%#Eval("EndDisplay") %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:CommandField CausesValidation="False" InsertVisible="False" ShowEditButton="True" />
                                    <asp:TemplateField>
                                        <ItemStyle HorizontalAlign="Right" />
                                        <ItemTemplate>
                                            <asp:ImageButton SkinID="buttonDelete" AlternateText="Delete Parameter" OnClientClick="return confirm('Are you sure you want to delete this parameter?');"
                                                CommandArgument='<%# Eval("Id") %>' CommandName="DeleteParam" runat="server"
                                                ID="DeleteParamButton" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EmptyDataRowStyle CssClass="emptyItem" />
                                <EmptyDataTemplate>
                                    No Criteria Added
                                </EmptyDataTemplate>
                            </asp:GridView>
                            <asp:Panel runat="server" ID="pnlPHParams" Visible="false">
                                <br />
                                <asp:Label runat="server" ID="lblNoPHParams" Text="No PH Criteria Added" Visible="false" />
                                <asp:Label runat="server" ID="lblPHParams" Text="PH Form Criteria" Visible="false" Font-Bold="true" />
                                <asp:GridView runat="server" ID="gdvPHQuery" AutoGenerateColumns="false" Width="100%" DataKeyNames="Id">
                                    <Columns>
                                        <asp:TemplateField HeaderText="Order">
                                            <EditItemTemplate>
                                                <asp:TextBox runat="server" ID="txtGridExecuteOrder" MaxLength="3" Width="25px" />
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblGridExecuteOrder" Text='<%# Eval("ExecuteOrder") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Type">
                                            <EditItemTemplate>
                                                <asp:DropDownList runat="server" ID="ddlGridType">
                                                    <asp:ListItem Value="AND">AND</asp:ListItem>
                                                    <asp:ListItem Value="OR">OR</asp:ListItem>
                                                </asp:DropDownList>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblGridType" Text='<%# Bind("WhereType") %>'></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Field">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblGridField" Text="Total"></asp:Label>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Operator">
                                            <EditItemTemplate>
                                                <asp:DropDownList runat="server" ID="ddlGridOperator" CssClass="gridEditOperator"></asp:DropDownList>
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="OperatorLabel" Text='<%# Eval("Operator") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Value">
                                            <EditItemTemplate>
                                                <asp:TextBox runat="server" ID="txtGridNumberStart" MaxLength="10" Width="80px" CssClass="gridEditStartNumber" Visible="false" />
                                                <asp:Label runat="server" ID="lblGridNumAnd" Text="and" CssClass="gridEditNumber" Visible="false" />
                                                <asp:TextBox runat="server" ID="txtGridNumberEnd" MaxLength="10" Width="80px" CssClass="gridEditNumber" Visible="false" />
                                            </EditItemTemplate>
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="ValueLabel" Text='<%# Eval("StartDisplay") %>' />
                                                <asp:Label runat="server" ID="AndLabel" Text=" and " />
                                                <asp:Label runat="server" ID="EndValueLabel" Text='<%#Eval("EndDisplay") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:CommandField CausesValidation="False" InsertVisible="False" ShowEditButton="True" />
                                        <asp:TemplateField>
                                            <ItemStyle HorizontalAlign="Right" />
                                            <ItemTemplate>
                                                <asp:ImageButton SkinID="buttonDelete" AlternateText="Delete Parameter" OnClientClick="return confirm('Are you sure you want to delete this parameter?');"
                                                    CommandArgument='<%# Eval("Id") %>' CommandName="DeleteParam" runat="server"
                                                    ID="DeleteParamButton" />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataRowStyle CssClass="emptyItem" />
                                    <EmptyDataTemplate>
                                        No Criteria Added
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </asp:Panel>
                        </td>
                    </tr>
                </table>
                <!-- output options -->
                <table>
                    <tr>
                        <td class="number">
                            E
                        </td>
                        <td class="label-small">
                            Output Fields:
                        </td>
                        <td class="style1">
                            <asp:Panel runat="server" ID="pnlWorkflowOutputUI">
                                <table>
                                    <tr>
                                        <td>
                                            <strong>Available Fields</strong>
                                        </td>
                                        <td style="width: 40px; text-align: center;">
                                            &nbsp;
                                        </td>
                                        <td>
                                            <strong>Selected Fields</strong>
                                        </td>
                                        <td style="width: 40px; text-align: center;">
                                            &nbsp;
                                        </td>
                                        <td>
                                            <strong>Sort Order</strong>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <select id="unusedSelect" multiple="multiple" style="width: 200px; height: 260px;">
                                            </select>
                                            <br />
                                            <asp:TextBox ID="UnselectedFields" runat="server" CssClass="hidden"></asp:TextBox>
                                        </td>
                                        <td style="vertical-align: top; text-align: center;">
                                            <br />
                                            <img runat="server" id="imgAddOutputField" src="../../images/arrow_right.gif" alt="Add Field" onclick="addOutputField();"
                                                class="pointer" />
                                            <br />
                                            <img runat="server" id="imgRemoveOutputField" src="../../images/arrow_left.gif" alt="Remove Field" onclick="removeOutputField();"
                                                class="pointer" />
                                        </td>
                                        <td>
                                            <select id="usedSelect" multiple="multiple" style="width: 200px; height: 260px;">
                                            </select>
                                            <br />
                                            <asp:TextBox ID="SelectedFields" runat="server" CssClass="hidden"></asp:TextBox>
                                        </td>
                                        <td style="vertical-align: top; text-align: center;">
                                            <br />
                                            <img runat="server" id="imgAddToSort" src="../../images/arrow_right.gif" alt="Add To Sort" onclick="addToSort();"
                                                class="pointer" />
                                            <br />
                                            <img runat="server" id="imgRemoveFromSort" src="../../images/arrow_left.gif" alt="Remove From Sort" onclick="removeFromSort();"
                                                class="pointer" />
                                            <br />
                                            <br />
                                            <br />
                                            <img runat="server" id="imgMoveItemUp" src="../../images/arrow_up.gif" alt="Move Up" onclick="moveUpItem();" class="pointer" />
                                            <img runat="server" id="imgMoveItemDown" src="../../images/arrow_down.gif" alt="Move Down" onclick="moveDownItem();"
                                                class="pointer" />
                                            <br />
                                            <br />
                                            <br />
                                            <img runat="server" id="imgFlipSort" src="../../images/arrow_switch.gif" alt="Switch Sort Order" onclick="flipSort();"
                                                class="pointer" />
                                        </td>
                                        <td>
                                            <select id="sortSelect" multiple="multiple" style="width: 200px; height: 260px;">
                                            </select>
                                            <br />
                                            <asp:TextBox ID="SortFields" runat="server" CssClass="hidden"></asp:TextBox>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>
                            <asp:Panel runat="server" ID="pnlPHFormTotalsOutputUI" Visible="false">
                                <table style="width:100%;">
                                    <tr>
                                        <td style="width:100%;">
                                            <asp:DropDownList runat="server" ID="ddlOutputPHSection" Width="222px" AutoPostBack="true" />&nbsp;&nbsp;
                                            <asp:DropDownList runat="server" ID="ddlOutputPHField" Width="222px" AutoPostBack="true" />&nbsp;&nbsp;
                                            <asp:DropDownList runat="server" ID="ddlOutputPHFieldType" Width="222px" AutoPostBack="false" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width:100%;">
                                            <div class="groupLeft-3">
                                                <asp:Button runat="server" ID="btnAddPHFormField" CssClass="phFormOutputButton" Text="Add Field(s)" /><br />
                                                <asp:Button runat="server" ID="btnRemovePHFormField" CssClass="phFormOutputButton" Text="Remove Field(s)" />
                                            </div>

                                            <div class="groupCenter-3">
                                                <asp:Button runat="server" ID="btnAddAllPHFormFields" CssClass="phFormOutputButton" Text="Add All Fields" /><br />
                                                <asp:Button runat="server" ID="btnRemoveAllPHFormFields" CssClass="phFormOutputButton" Text="Remove All Fields" />
                                            </div>

                                            <div class="groupRight-3">
                                            </div>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td style="width:100%;">
                                            <asp:ListBox runat="server" ID="lsbOutputPHFormFields" Width="100%" Height="150px" SelectionMode="Multiple" />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <br />
                                            <div class="groupLeft-2">
                                                <asp:Label runat="server" ID="lblPHFormSortByField" Text="Sort Column" Font-Bold="true" /><br />
                                                <asp:RadioButtonList runat="server" ID="rblPHFormSortByField" >
                                                    <asp:ListItem Value="1" Text="Total" />
                                                    <asp:ListItem Value="2" Text="Form Field" />
                                                </asp:RadioButtonList>
                                            </div>
                                            <div class="groupRight-2">
                                                <asp:Label runat="server" ID="lblPHFormSortByOrder" Text="Sort Order" Font-Bold="true" /><br />
                                                <asp:RadioButtonList runat="server" ID="rblPHFormSortByOrder">
                                                    <asp:ListItem Value="1" Text="ASC" />
                                                    <asp:ListItem Value="2" Text="DESC" />
                                                </asp:RadioButtonList>
                                            </div>
                                        </td>
                                    </tr>
                                </table>
                            </asp:Panel>

                        </td>
                        </tr>
                        <tr>
                            <td class="number">
                                F
                            </td>
                            <td class="label-small">
                                Output Format:
                            </td>
                            <td class="style1">
                                <asp:RadioButton ID="OutputScreen" runat="server" Checked="true" GroupName="Output"
                                    Text="Browser" />
                                <img src="../../images/page_white_world.gif" alt="View in browser" style="vertical-align: middle" />
                                <asp:RadioButton ID="OutputExcel" runat="server" GroupName="Output" Text="Excel" />
                                <img src="../../images/page_white_excel.gif" alt="Export to Excel" style="vertical-align: middle"  />
                                <asp:RadioButton ID="OutputPdf" runat="server" GroupName="Output" Text="PDF" />
                                <img src="../../images/page_white_acrobat.gif" alt="Export to PDF" style="vertical-align: middle"  />
                                <asp:RadioButton ID="OutputCsv" runat="server" GroupName="Output" Text="CSV" />
                                <img src="../../images/page_white_text.gif" alt="Export to CSV" style="vertical-align: middle" " />
                                <asp:RadioButton ID="OutputXml" runat="server" GroupName="Output" Text="XML" />
                                <img src="../../images/page_white_code.gif" alt="Export to XML" style="vertical-align: middle"  />
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                G
                            </td>
                            <td class="label-small">
                                Action:
                            </td>
                            <td class="style1">
                                <asp:Button ID="QueryButton" runat="server" Text="Run Report" />
                                &nbsp;
                                <asp:Button OnClientClick="$('#saveReportDialog').dialog('open'); return false;"
                                    runat="server" ID="ClientSaveButton" Text="Save Report" />
                                <asp:TextBox runat="server" ID="SaveTitlebox" CssClass="hidden" Width="135px" MaxLength="50" />
                                <asp:CheckBox runat="server" ID="chkSaveShared" CssClass="hidden" />
                                <asp:Button runat="server" ID="SaveQueryButton" CssClass="hidden" Width="130px" />
                                <asp:Button runat="server" ID="SaveQueryNewButton" CssClass="hidden" Width="130px" />
                            </td>
                        </tr>
                        <!-- end output -->
                    
                </table>
                <asp:Panel runat="server" ID="ErrorPanel" Visible="false">
                    <table>
                        <tr>
                            <td class="number">
                                &nbsp;
                            </td>
                            <td class="label-small">
                                Error:
                            </td>
                            <td class="value">
                                <asp:BulletedList runat="server" ID="ErrorList" Font-Bold="true" ForeColor="Red" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <hr />
                <table>
                    <!-- Shared Reports -->
                    <tr>
                        <td class="number">
                            H
                        </td>
                        <td class="label-small">
                            Shared Reports:
                        </td>
                        <td class="value">
                            <asp:DropDownList runat="server" ID="ddlSharedReports" DataTextField="Title" DataValueField="Id">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <!-- saved reports -->
                    <tr>
                        <td class="number">
                            I
                        </td>
                        <td class="label-small">
                            Saved Reports:
                        </td>
                        <td class="value">
                            <asp:DropDownList runat="server" ID="MyQuerySelect" DataTextField="Title" DataValueField="Id">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            J
                        </td>
                        <td class="label-small">
                            Action:
                        </td>
                        <td class="value">
                            <span id="EditReportButtonSpan">
                                <asp:Button runat="server" ID="EditReportButton" Text="Load Report" />&nbsp;</span>
                            <span id="DeleteReportButtonSpan"><asp:Button runat="server" OnClientClick="return confirm('Are you sure you want to delete the selected report?');"
                                    ID="DeleteReportButton" Text="Delete Report" />&nbsp; </span>
                            <asp:Button runat="server" ID="NewReportButton" Text="New Report" />
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
        <%--<asp:Label runat="server" ID="lblIsAdmin" CssClass="hidden" />--%>
        <asp:CheckBox runat="server" ID="chkIsAdmin" CssClass="hidden" />
        <asp:TextBox runat="server" ID="ResultsUrl" CssClass="hidden" />
        <div id="saveReportDialog" title="Save Report" class="hidden">
            <table>
                <tr>
                    <td class="number">
                        A
                    </td>
                    <td class="label-small">
                        Title:
                    </td>
                    <td class="value">
                        <input type="text" id="ReportTitle" maxlength="50" onkeypress="return checkFormat(this,event,&#39;AlphaNumeric&#39;,&#39;`,~,!,@,$,%,*,(,),-,_,+,\&#39;,&quot;,.,,,?,/,\\,|,:, &#39;);" style="width: 200px;" />
                    </td>
                </tr>
                <tr>
                    <td class="number">
                        B
                    </td>
                    <td class="label-small">
                        Shared:
                    </td>
                    <td class="value">
                        <input type="checkbox" id="chkReportShared" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFooter" runat="Server">

    <script type="text/javascript">

      $(function () {

            $('.datePicker').datepicker(calendarPick("All", "<%=CalendarImage %>"));

            // Edit Gridview element event handlers
            $(".gridEditOperator").on('change', function (e) {
                var classStr = "";
                var isDate = false;

                // Rehide the elements related to the between operator
                $('tr.gridViewEdit > td > .gridEditNumber').hide();
                $('tr.gridViewEdit > td > .gridEditDate').hide();
                $('tr.gridViewEdit > td > .gridEditRange').hide();

                // Check if a number parameter is being edited...
                if ($('tr.gridViewEdit > td > .gridEditStartNumber').is(':visible')) {
                    classStr = ".gridEditNumber";
                }

                // Check if an exact date parameter is being edited...
                if ($('tr.gridViewEdit > td > .gridEditStartDate').is(':visible')) {
                    classStr = ".gridEditDate";
                    isDate = true;
                }

                // Check if a date range parameter is being edited...
                if ($('tr.gridViewEdit > td > .gridEditStartRange').is(':visible')) {
                    classStr = ".gridEditRange";
                }

                // Show/Hide date picker images
                if (isDate == true) {
                    $('tr.gridViewEdit > td > img.ui-datepicker-trigger').show();
                }
                else {
                    $('tr.gridViewEdit > td > img.ui-datepicker-trigger').hide();
                }

                // Show/Hide controls based on selected value in the ddl
                if (classStr != "") {
                    if ($(this).val() == "between") {
                        classStr = "tr.gridViewEdit > td > " + classStr;
                        $(classStr).show();
                    }
                    else {
                        classStr = "tr.gridViewEdit > td > " + classStr;
                        $(classStr).hide();

                        if (isDate == true) {
                            $('tr.gridViewEdit > td > input.gridEditDate + img.ui-datepicker-trigger').hide();
                        }
                    }
                }
            }).trigger(change);

            // Shared Reports
            $('#<%= ddlSharedReports.ClientId %>').change(function () {

                // Reset the selected value to the 0th index for the Save Reports dropdownlist
                $('#<%= MyQuerySelect.ClientId %>').val("0");

                // Check if the selected report is owned by the logged in user
                if ($('#<%= ddlSharedReports.ClientId %> option:selected').text().indexOf("[Owner]") >= 0) {
                    $('#EditReportButtonSpan').removeAttr("disabled");
                    $('#DeleteReportButtonSpan').removeAttr("disabled");
                }
                // Check if the 0th index was selected
                else if ($(this).val() == "0") {
                    $('#EditReportButtonSpan').attr("disabled", "disabled");
                    $('#DeleteReportButtonSpan').attr("disabled", "disabled");
                }
                // If a non-owned report was selected
                else {
                    $('#EditReportButtonSpan').removeAttr("disabled");

                    // Check if the user is a system admin
                    if ($('#<%= chkIsAdmin.ClientID %>').is(':checked')) {
                        $('#DeleteReportButtonSpan').removeAttr("disabled");
                    }
                    else {
                        $('#DeleteReportButtonSpan').attr("disabled", "disabled");
                    }
                }

            });

            // My Reports
            $('#<%= MyQuerySelect.ClientId %>').change(function () {

                // Reset the selected value to the 0th index for the Shared Reports dropdownlist
                $('#<%= ddlSharedReports.ClientId %>').val("0");

                if ($(this).val() == "0") {
                    $('#ReportOutRow').attr("disabled", "disabled");
                    $('#EditReportButtonSpan').attr("disabled", "disabled");
                    $('#DeleteReportButtonSpan').attr("disabled", "disabled");
                } else {
                    $('#ReportOutRow').removeAttr("disabled");
                    $('#EditReportButtonSpan').removeAttr("disabled");
                    $('#DeleteReportButtonSpan').removeAttr("disabled", "disabled");
                }

            });

            if ($('#<%= MyQuerySelect.ClientId %>').val() == "0" && $('#<%= ddlSharedReports.ClientId %>').val() == "0") {
                $('#ReportOutRow').attr("disabled", "disabled");
                $('#EditReportButtonSpan').attr("disabled", "disabled");
                $('#DeleteReportButtonSpan').attr("disabled", "disabled");
            }

            if ($('#<%= ddlSharedReports.ClientId %>').val() != "0") {
                // Check if a non-owned report was selected
                if ($('#<%= ddlSharedReports.ClientId %> option:selected').text().indexOf("[Owner]") == -1) {
                    // Check if the user is a system admin
                    if ($('#<%= chkIsAdmin.ClientID %>').is(':checked')) {
                        $('#DeleteReportButtonSpan').removeAttr("disabled");
                    }
                    else {
                        $('#DeleteReportButtonSpan').attr("disabled", "disabled");
                    }
                }
            }

            // Create the buttons for the saveReportDialog
            var saveReportButtons = {
                'Save': function () {

                    $('#<%= SaveTitlebox.ClientId %>').val($('#ReportTitle').val());

                    if ($('#chkReportShared').attr('checked')) {
                        $('#<%= chkSaveShared.ClientId %>').attr('checked', 'checked');
                    }
                    else {
                        $('#<%= chkSaveShared.ClientId %>').removeAttr('checked');
                    }

                    $(this).dialog('close');
                    element('<%= SaveQueryButton.ClientId %>').click();
                },

                'Save as New': function () {

                    $('#<%= SaveTitlebox.ClientId %>').val($('#ReportTitle').val());

                    if ($('#chkReportShared').attr('checked')) {
                        $('#<%= chkSaveShared.ClientId %>').attr('checked', 'checked');
                    }
                    else {
                        $('#<%= chkSaveShared.ClientId %>').removeAttr('checked');
                    }

                    $(this).dialog('close');
                    element('<%= SaveQueryNewButton.ClientId %>').click();
                },

                'Cancel': function () {
                    $(this).dialog('close');
                }
            };

            // Check if the 'Save' button for the saveReportDialog needs to be removed because the user does not own the loaded shared report. 
            if ($('#<%= ddlSharedReports.ClientId %>').val() != "0") {
                // Check if a non-owned report was selected
                if ($('#<%= ddlSharedReports.ClientId %> option:selected').text().indexOf("[Owner]") == -1) {
                    delete saveReportButtons['Save'];
                }
            }

            //save report
            $('#saveReportDialog').dialog({
                autoOpen: false,
                modal: true,
                resizable: false,
                width: 380,
                height: 200,
                open: function () {
                    $('#ReportTitle').val($('#<%= SaveTitlebox.ClientId %>').val());

                    if ($('#<%= chkSaveShared.ClientId %>').attr('checked')) {
                        $('#chkReportShared').attr('checked', 'checked');
                    }
                },
                buttons: saveReportButtons

            });

            //input source options
            $('#<%= DateSpan.ClientId %>').click(function () {
                if (!$(this).checked) {
                    $('#RangeInput').css('display', 'inline');
                    $('#ExactInput').css('display', 'none');
                } else {
                    $('#RangeInput').css('display', 'none');
                    $('#ExactInput').css('display', 'inline');
                }
            });

            $('#<%= DateExact.ClientId %>').click(function () {
                if ($(this).checked) {
                    $('#RangeInput').css('display', 'inline');
                    $('#ExactInput').css('display', 'none');
                } else {
                    $('#RangeInput').css('display', 'none');
                    $('#ExactInput').css('display', 'inline');
                }
            });

            var dateOp = $('#<%= OperatorDate.ClientId %>');
            var numOp = $('#<%= OperatorNumber.ClientId %>');

            setChoice(dateOp, 'Date');
            setChoice(dateOp, 'Range');
            setChoice(numOp, 'Number');

          dateOp.on('change', function () {
              setChoice(this, 'Date');
              setChoice(this, 'Range');
          });

          numOp.on('change', function () {
              setChoice(this, 'Number');
          });


            if (!$('#<%= DateSpan.ClientId %>').checked) {
                $('#RangeInput').css('display', 'inline');
                $('#ExactInput').css('display', 'none');
            } else {
                $('#RangeInput').css('display', 'none');
                $('#ExactInput').css('display', 'inline');
            }

            //setup our output fields
            initFieldSelect();
            enableQueryButton();

            //show any results of needed
            showResults();

        });

        function enableQueryButton() {

            if ($('#<%= SelectedFields.ClientId %>').length != 0 && $('#<%= SelectedFields.ClientId %>').val().length == 0) {
                $('#<%= QueryButton.ClientId %>').attr('disabled', 'disabled');
            } else {
                $('#<%= QueryButton.ClientId %>').removeAttr('disabled');
            }
        }


        function setChoice(select, type) {
            $('#End' + type).css('display', $(select).val() == 'between' ? 'inline' : 'none');

            if (type == "Date") {
                //there are two spans to show/hide for dates

            }

        }


        function moveUpItem() {
            moveOptionUp(element('usedSelect'));
            serializeOutputFields();
        }

        function moveDownItem() {
            moveOptionDown(element('usedSelect'));
            serializeOutputFields();
        }

        function initFieldSelect() {

            if (typeof $('#<%= UnselectedFields.ClientId %>').val() == 'undefined') {
                return;
            }

            var fields = $('#<%= UnselectedFields.ClientId %>').val().split("|");

            for (var i = 0; i < fields.length; i++) {
                if (fields[i].length > 0) {
                    $('#unusedSelect').append($("<option></option>").attr("value", i).text(fields[i]));
                }
            }

            fields = $('#<%= SelectedFields.ClientId %>').val().split("|");

            for (var i = 0; i < fields.length; i++) {
                if (fields[i].length > 0) {
                    $('#usedSelect').append($("<option></option>").attr("value", i).text(fields[i]));
                }
            }

            fields = $('#<%= SortFields.ClientId %>').val().split("|");

            for (var i = 0; i < fields.length; i++) {
                if (fields[i].length > 0) {
                    $('#sortSelect').append($("<option></option>").attr("value", i).text(fields[i]));
                }
            }
        }


        function addOutputField() {

            $('#unusedSelect :selected').each(function(i, selected) {
                $('#usedSelect').append($(selected).clone().removeAttr('selected'));
                $(this).remove();
            });

            serializeOutputFields();

        }

        function removeOutputField() {

            $('#usedSelect :selected').each(function(i, selected) {
                var option = $(selected).clone().removeAttr('selected');
                var added = false;

                //we need to put this back in it's place
                if ($('#unusedSelect > option').length == 0) {
                    $('#unusedSelect').append(option);
                } else {
                    $('#unusedSelect > option').each(function() {
                        if (!added && $(this).text() > option.text()) {
                            $(this).before(option);
                            added = true;
                        }
                    });

                    //if it didn't get added, that means it belongs at the end
                    if (!added) {
                        $('#unusedSelect').append(option);
                    }

                }

                $(this).remove();
            });

            serializeOutputFields();
        }

        function addToSort() {
            $('#usedSelect :selected').each(function(i, selected) {
                var optionASC = $(selected).text() + " (ASC)";
                var optionDESC = $(selected).text() + " (DESC)";
                
                var added = false;
                
                $('#sortSelect > option').each(function(){
                   if (! added && ( $(this).text() == optionASC || $(this).text() == optionDESC)) {                    
                        added = true;
                    }
                });
                
                if (!added) {
                    $('#sortSelect').append($(selected).clone().removeAttr('selected').text($(selected).text() + " (ASC)"));
                }
            });

            serializeOutputFields();
        }

        function removeFromSort() {
            $('#sortSelect :selected').each(function(i, selected) {
                $(this).remove();
            });

            serializeOutputFields();
        }

        function flipSort() {
            $('#sortSelect :selected').each(function(i, selected) {
                var text = $(selected).text();
                if (text.match(/ASC/i)) {
                    text = text.replace(/ASC/i, "DESC");
                } else {
                    text = text.replace(/DESC/i, "ASC");
                }

                $(selected).text(text)
            });


            serializeOutputFields();
        }

        function serializeOutputFields() {
            var items = [];

            //unselected fields
            $('#unusedSelect > option').each(function() {
                if ($(this).text().length > 0) {
                    items.push($(this).text());
                }
            });

            $('#<%= UnselectedFields.ClientId %>').val(items.join("|"));

            //selected fields
            items = [];

            $('#usedSelect > option').each(function() {
                if ($(this).text().length > 0) {
                    items.push($(this).text());
                }
            });

            $('#<%= SelectedFields.ClientId %>').val(items.join("|"));

            //sort fields
            items = [];

            $('#sortSelect > option').each(function() {
                if ($(this).text().length > 0) {
                    items.push($(this).text());
                }
            });

            $('#sortSelect').on('focus', function () {
                // Your focus event handler code here
            });


            $('#<%= SortFields.ClientId %>').val(items.join("|"));

            enableQueryButton();
        }

        function showResults() {

            var url = $('#<%= ResultsUrl.ClientId %>').val();

            if (url.length == 0) {
                return;
            }

            $('#<%= ResultsUrl.ClientId %>').val('');

            showPopup({
                Url: url,
                Width: '900',
                Height: '700',
                Resizable: true,
                ScrollBars: true,
                Center: true
            });
        }

        function enableTextBox(cssClass, isChecked, shouldEmpty) {
            if (isChecked) {
                $(cssClass).attr("readonly", true);

                if (shouldEmpty == true) {
                    $(cssClass).val("");
                }
            }
            else {
                $(cssClass).attr("readonly", false);
            }
        }
    
    </script>

</asp:Content>
