<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Admin.Secure_Shared_Admin_ChildUnits" Codebehind="ChildUnits.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
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
                .on('mouseenter', function () { // Equivalent to hover's first function
                    $(this).addClass("ui-state-hover");
                })
                .on('mouseleave', function () { // Equivalent to hover's second function
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
            $('#srchBox').dialog('open');

        }
        //Client accepted so Set Corresponding reporting units 
        function SetReportingUnit() {
            var srcherId = '<%=unitSearcher.ClientId %>'
            //Id is stroed in a hidden control.Transfer the value from the control
            element(element('<%=hdnIdClient.ClientId %>').value).value = element(srcherId + '_hdnSelectedUnit').value;
            //Name is stored in a label.Transfer the value from the control
            element(element('<%=hdnNameClient.ClientId %>').value).value = element(srcherId + '_hdnSelectedUnitName').value;
            element('<%=UnitNameTxt.ClientId %>').value = element(srcherId + '_hdnSelectedUnitName').value;



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
            <lod:unitSearcher ID="unitSearcher" ActiveOnly="false" runat="server" />
        </div>
        <!-- end search control -->
        <input type="hidden" id="hdnIdClient" runat="Server" />
        <input type="hidden" id="hdnNameClient" runat="Server" />
        <br />
    </div>
    <asp:Panel runat="server" ID="FeedbackPanel" CssClass="ui-state-highlight info-block msg-panel">
        This view shows the list of all child units along with the selected unit.To view
        the command chain please visit "Manage Units"
    </asp:Panel>
    <br />
    <div id="divSelect">
        &nbsp;Unit:
        <asp:TextBox Width="250px" ReadOnly="True" runat="server" ID="UnitNameTxt"></asp:TextBox>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="UnitNameTxt"
            ErrorMessage="Unit not selected" ValidationGroup="select">*
        </asp:RequiredFieldValidator>
        <input type="hidden" id="SrcNameHdn" runat="Server" />
        <input type="hidden" id="SrcUnitIdHdn" runat="Server" />
        <asp:Button Width="80px" ID="btnFindUnit" CausesValidation="True" ValidationGroup="select"
            Text="Find Unit" runat="server"></asp:Button>
        &nbsp; Report View:
        <asp:DropDownList ID="ChainTypeSelect" runat="server" AutoPostBack="True" DataTextField="description"
            DataValueField="Id">
        </asp:DropDownList>
        &nbsp;
        <asp:RequiredFieldValidator InitialValue="0" ID="RequiredFieldValidator1" runat="server"
            ControlToValidate="ChainTypeSelect" ErrorMessage="Chain type not selected" ValidationGroup="select">*
        </asp:RequiredFieldValidator>
        &nbsp;
        <asp:Button Width="100px" ID="SearchButton" CausesValidation="True" ValidationGroup="select"
            Text="Show Children" runat="server"></asp:Button>
        <asp:Label ForeColor="Red" runat="server" CssClass="hidden" ID="lblUnitMsg" Text="Unit not selected"></asp:Label>
        <br />
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="select"
            Width="207px" />
    </div>
    <br />
    <br />
    <asp:UpdatePanel ID="resultsUpdatePanel" runat="server" ChildrenAsTriggers="True">
        <ContentTemplate>
            <label  for ="parentUnitLabelId" style="font-weight:bold"  >
                Unit:
            </label>
            <asp:Label ID="parentUnitLabelId" runat="server"> </asp:Label>
            <hr />
            <br />
            <div id="spSpacer" class="emptyItem" style="margin-top: 0px; margin-bottom: 4px;
                height: 22px;">
                <div id="spWait" class="" style="display: none;">
                    &nbsp;<asp:Image runat="server" ID="imgWait" SkinID="imgBusy" AlternateText="busy"
                        ImageAlign="AbsMiddle" />&nbsp;Loading...
                </div>
            </div>
            <br />
            <asp:GridView ID="UnitGrid" runat="server" align="center" Width="50%" AutoGenerateColumns="False"
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
                    No unit found
                </EmptyDataTemplate>
                <PagerStyle HorizontalAlign="Center" VerticalAlign="Bottom"></PagerStyle>
            </asp:GridView>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="SearchButton" EventName="Click" />
            <asp:AsyncPostBackTrigger ControlID="ChainTypeSelect" EventName="SelectedIndexChanged" />
            <asp:AsyncPostBackTrigger ControlID="SrcUnitIdHdn" EventName="ServerChange" />
        </Triggers>
    </asp:UpdatePanel>
    <ajax:UpdatePanelAnimationExtender ID="resultsUpdatePanelAnimationExtender" runat="server"
        TargetControlID="resultsUpdatePanel">
        <Animations>
                <OnUpdating>
                    <ScriptAction script="searchStart();" />
                </OnUpdating>  
                <OnUpdated>     
                    <ScriptAction script="searchEnd();" />
                </OnUpdated>           
        </Animations>
    </ajax:UpdatePanelAnimationExtender>
    <br />
</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="head">
</asp:Content>
