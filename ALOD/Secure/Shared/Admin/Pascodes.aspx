<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Admin.Secure_Shared_Admin_Pascodes" Codebehind="Pascodes.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">

    <script type="text/javascript">
        $(document).ready(function() {
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
            $('.open-dialog').click(function() {

                $('#srchBox').dialog('open');
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
            <lod:unitSearcher ID="unitSearcher"   ActiveOnly="false" runat="server" />
        </div>
        <!-- end search control -->
        <input type="hidden" id="hdnIdClient" runat="Server" />
        <input type="hidden" id="hdnNameClient" runat="Server" />
        <br />
    </div>
    <div id="divSelect">
        &nbsp;Unit: 
        <asp:TextBox Width="250px" ReadOnly="True" runat="server" ID="UnitNameTxt"></asp:TextBox>
        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="UnitNameTxt"
            ErrorMessage="Unit not selected" ValidationGroup="create">*
        </asp:RequiredFieldValidator>
        <input type="hidden" id="SrcNameHdn" runat="Server" />
        <input type="hidden" id="SrcUnitIdHdn" runat="Server" />
        <asp:Button Width="80px" ID="btnFindUnit" CausesValidation="True" ValidationGroup="create"
            Text="Find Unit" runat="server"></asp:Button>
            &nbsp;
        Report View:
        <asp:DropDownList ID="ddlChainType"  runat="server"
            DataTextField="description" DataValueField="name">
        </asp:DropDownList>
        &nbsp;
        <asp:RequiredFieldValidator InitialValue="0" ID="RequiredFieldValidator1" runat="server"
            ControlToValidate="ddlChainType" ErrorMessage="Chain type not selected" ValidationGroup="create">*
        </asp:RequiredFieldValidator>
        &nbsp;
        <asp:Button Width="100px" ID="btnCreate" CausesValidation="True" ValidationGroup="create"
            Text="Show Chain" runat="server"></asp:Button>
        <asp:Label ForeColor="Red" runat="server" CssClass="hidden" ID="lblUnitMsg" Text="Unit not selected"></asp:Label>
        <br />
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ValidationGroup="create"
            Width="207px" />
    </div>
    <br />
    <h3>
        Command Chain
    </h3>
    <div id="divChain" class="pcodeTreeSection" runat="server">
        <div class="align-left">
            <div>
                <asp:Label CssClass="labelPasCode" ID="prntCmdLbl" runat="server">  </asp:Label>
                <asp:Label CssClass="labelPasCode" ID="ChainLbl" runat="server">  </asp:Label>
                <br />
                <br />
            </div>
            <asp:Button ID="btnExpandAll" Visible="False" runat="server" Text="+" Width="25px"
                BackColor="White" BorderColor="Gray" BorderStyle="Solid" BorderWidth="1px" Font-Bold="False"
                Font-Size="Medium" Height="19px" Style="margin-top: 0px" ToolTip="Click to expand or collapse all nodes" />
            <asp:Label ID="lblExpand" Visible="False" runat="server" Text="Expand All" Width="76px"
                BackColor="White" />
        </div>
        <br />
        <asp:TreeView ID="PassCodeTree" runat="server" ExpandDepth="1" ShowLines="True" CssClass="labelNormal"
            NodeIndent="70">
        </asp:TreeView>
    </div>
    <br />
    <div id="divNoCommands" style="padding-left: 20px;" visible="false" cssclass="labelNormal"
        runat="server">
        Command does not exist
    </div>     
</asp:Content>
<asp:Content ID="Content2" runat="server" ContentPlaceHolderID="head">
</asp:Content>
