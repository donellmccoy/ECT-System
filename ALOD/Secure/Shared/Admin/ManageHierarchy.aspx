<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false"
    Codebehind="ManageHierarchy.aspx.vb" Inherits="ALOD.Web.Admin.Secure_Shared_Admin_ManageHierarchy" %>

<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="ContentFooter">

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
            element(element('<%=hdnNameClient.ClientId %>').value).innerHTML = element(srcherId + '_hdnSelectedUnitName').value;
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
<asp:Content ID="Content3" runat="server" ContentPlaceHolderID="head">
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
   <asp:Panel runat="server" ID="FeedbackPanel" Visible="false" CssClass="ui-state-highlight info-block msg-panel">
            <asp:Label runat="server" ID="FeedbackMessageLabel" />
    </asp:Panel>
    <div id="srchBox" class="hidden" title="Find Unit">
        <lod:unitSearcher ID="unitSearcher"  ActiveOnly="true" runat="server" />
    </div>
    <table>
        <tr>
            <td style="width:60px; text-align:left;" class="labelNormal">
                Name:
            </td>
            <td class="align-left">
                <asp:Label ID="lblUnitName" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td style="width:60px; text-align: left;" class="labelNormal">
                PasCode:
            </td>
            <td class="align-left">
                <asp:Label ID="lblPasCode" runat="server"></asp:Label>
            </td>
        </tr>
    </table>
    <br />
    <div id="dvReporting" style="text-align:center;"  >
        <asp:GridView ID="gvReporting"   Width="700px" runat="server"
            AutoGenerateColumns="False" EmptyDataText="No Record Found" DataKeyNames="cs_id">
            <Columns>
                <asp:TemplateField Visible="false" HeaderText="Hierarchy Type">
                    <ItemTemplate>
                        <asp:Label ID="lblChainType" runat="server" Text='<%# Bind("Chain_Type") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Width="40px" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Hierarchy Type">
                    <ItemTemplate>
                        <asp:Label ID="lblChainDescription" runat="server" Text='<%# Bind("Chain_Description") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Width="360px" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Unit Reports To">
                    <ItemTemplate>
                        <asp:Label ID="lblUnitName" runat="server" Text='<%# Eval("parent_long_name") + " (" + Eval("parent_pas") + ")"%>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle Width="400px" />
                </asp:TemplateField>
                <%--!       <asp:TemplateField HeaderText="Change Reporting">
                            <ItemTemplate>
                                <asp:DropDownList ID="ddlReportingUnit" DataSourceID="lnqCommandStruct" DataTextField="LONG_NAME"
                                    DataValueField="CS_ID" SelectedValue='<%# IIF(Eval("parent_cs_Id") IS NOTHING,"",Eval("parent_cs_Id")) %>'
                                    runat="server">
                                </asp:DropDownList>
                            </ItemTemplate>
                            <ItemStyle Width="150px" />
                        </asp:TemplateField>--%>
                <asp:TemplateField HeaderText="Change To">
                    <ItemTemplate>
                        <asp:Label ID="lblNewUnitName" runat="server" Text=""></asp:Label>
                        <input type="hidden" id="hdnUnit" value='<%# Bind("parent_cs_id") %>' runat="Server" />
                    </ItemTemplate>
                    <ItemStyle Width="300px" />
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Edit">
                    <ItemTemplate>
                        <asp:Button ID="btnEdit"  CssClass="ui-state-default ui-corner-all"  Text="Find" runat="server"></asp:Button>
                    </ItemTemplate>
                    <ItemStyle Width="60px" />
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>
    <div>
        <table style="text-align:center; width:700px;">
            <tr>
                <td class="align-right">
                    <asp:Button runat="server" ID="btnUpdate" Text="Save" />
                    <asp:Button runat="server" ID="btnCancel" Text="Return" />
                </td>
            </tr>
        </table>
    </div>
    <div id="DataSources">
  
        <input type="hidden" id="hdnIdClient" runat="Server" />
        <input type="hidden" id="hdnNameClient" runat="Server" />
    </div>
</asp:Content>
