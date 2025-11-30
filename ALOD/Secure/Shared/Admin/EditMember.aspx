<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Admin.Secure_Shared_Admin_EditMember" Codebehind="EditMember.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <script type="text/javascript">
       (function() {
            $('#srchBox').dialog({
                autoOpen: false,
                modal: true,
                resizable: true,
                width: 530,
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

            
        })

        //Show Searcher
        function showSearcher(title) { 
            //Set Client controls where unit Id and unit names will be transferred
            $('#srchBox').dialog('open');
        }

        //Client accepted so Set Corresponding reporting units 
        function SetReportingUnit() {
            var srcherId = '<%=unitSearcher.ClientId %>'
            //Id is stroed in a label control.Transfer the value from the control
            element('<%=newUnitIDLabel.ClientId %>').value = element(srcherId + '_hdnSelectedUnit').value;
            //Name is stored in a label.Transfer the value from the control
            $('#<%=UnitTextBox.ClientId %>').text(element(srcherId + '_hdnSelectedUnitName').value);
            // return false;

        }
        //Client cancelled so ignore the dialog values
        function CancelSelection() {
            var srcherId = '<%=unitSearcher.ClientId %>'
            element(srcherId + '_hdnSelectedUnit').value = "";
            element(srcherId + '_hdnSelectedUnitName').value = "";
            return false;
        } 
    </script>

    <div id="srchBox" class="hidden" title="Find Unit">
        <lod:unitSearcher ID="unitSearcher" ActiveOnly="true" runat="server" />
        <!-- end search control -->
        <input type="hidden" id="hdnIdClient" runat="Server" />
        <input type="hidden" id="hdnNameClient" runat="Server" />
        <br />
    </div>
    <div id="content" class="indent-small">
        <div style="text-align: right;">
            <asp:LinkButton ID="lnkManageMembers" runat="server">
                <asp:Image runat="server" ID="ReturnImage2" AlternateText="Return to member data"
                    ImageAlign="AbsMiddle" ImageUrl="~/images/arrow_left.gif" />
                Return to Member Data
            </asp:LinkButton>
        </div>
    </div>
    <br />
    <div class="indent-small">
        <asp:Panel runat="server" ID="ErrorPanel" Visible="false" CssClass="ui-state-error info-block">
            <asp:Image runat="server" ID="Image1" ImageAlign="AbsMiddle" ImageUrl="~/images/warning.gif" />
            <asp:Label runat="server" ID="ErrorMessageLabel" />
        </asp:Panel>
        <asp:Panel runat="server" ID="FeedbackPanel" Visible="false" CssClass="ui-state-highlight info-block msg-panel">
            <asp:Label runat="server" ID="FeedbackMessageLabel" />
        </asp:Panel>
        <asp:Panel ID="MemberDataPanel" runat="server" CssClass="dataBlock">
            <div class="dataBlock-header">
                1 - Member Information
            </div>
            <div class="dataBlock-body">
                <table class="dataTable" style="text-align: left;">
                    <tr>
                        <td class="number">
                            A
                        </td>
                        <td class="label">
                            Name:
                        </td>
                        <td class="value">
                            <asp:Label ID="NameLbl" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            B
                        </td>
                        <td class="label">
                            DOB:
                        </td>
                        <td class="value">
                            <asp:Label ID="DobLbl" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            C
                        </td>
                        <td class="label">
                            Local Address:
                        </td>
                        <td class="value">
                            <asp:Label ID="AddressLbl" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            D
                        </td>
                        <td class="label">
                            Duty Phone:
                        </td>
                        <td class="value">
                            <asp:Label ID="DutyPhoneLbl" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            E
                        </td>
                        <td class="label">
                            Component:
                        </td>
                        <td class="value">
                            <asp:Label ID="lblCompo" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            F
                        </td>
                        <td class="label labelRequired">
                            * Rank:
                        </td>
                        <td class="value">
                            <asp:Label runat="server" ID="RankLabel" />
                            <asp:DropDownList runat="server" ID="RankSelect" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            G
                        </td>
                        <td class="label labelRequired">
                            * Unit:
                        </td>
                        <td class="value">
                            <asp:Label runat="server" ID="UnitTextBox" />
                            <asp:LinkButton runat="server" ID="ChangeUnitButton" Text="  [change]" />
                            <asp:TextBox CssClass="hidden" ID="newUnitIDLabel" runat="Server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            H
                        </td>
                        <td class="label">
                            Action:
                        </td>
                        <td class="value">
                            <asp:Button runat="server" ID="SaveMemberButton" Text="Save Member" />&nbsp;
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlSystemAdminChangeHistory" CssClass="dataBlock" Visible="false">
            <div class="dataBlock-header">
                2 - Admin Change History
            </div>
            <div class="dataBlock-body">
                <asp:GridView runat="server" ID="gdvSystemAdminChangeHistory" SkinID="TestGV" AutoGenerateColumns="False" Width="100%" AllowPaging="true" PageSize="10">
                    <Columns>
                        <asp:BoundField DataField="ChangeDate" HeaderText="Modified Date" ReadOnly="True" ItemStyle-Width="18%"/>
                        <asp:BoundField DataField="ChangeType" HeaderText="Change Type" ReadOnly="True" ItemStyle-Width="12%"/>
                        <asp:BoundField DataField="ModifiedByUsername" HeaderText="Modified By" ReadOnly="True"/>
                        <asp:BoundField DataField="ModifiedField" HeaderText="Modified Field" ReadOnly="True"/>
                        <asp:BoundField DataField="OldValue" HeaderText="Old Value" ReadOnly="True"/>
                        <asp:BoundField DataField="NewValue" HeaderText="New Value" ReadOnly="True"/>
                    </Columns>
                </asp:GridView>
            </div>
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlMILPDSChangeHistory" CssClass="dataBlock" Visible="false">
            <div class="dataBlock-header">
                3 - MILPDS Change History
            </div>
            <div class="dataBlock-body">
                <asp:GridView runat="server" ID="gdvMILPDSChangeHistory" SkinID="TestGV" AutoGenerateColumns="False" Width="100%" AllowPaging="true" PageSize="10">
                    <Columns>
                        <asp:BoundField DataField="ChangeDate" HeaderText="Modified Date" ReadOnly="True" ItemStyle-Width="18%"/>
                        <asp:BoundField DataField="ChangeType" HeaderText="Change Type" ReadOnly="True" ItemStyle-Width="12%"/>
                        <asp:TemplateField HeaderText="SSN">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="lblChangeHistorySSN" Text='<%#DataBinder.Eval(Container.DataItem, "Member.MinifiedSSN")%>' />
                            </ItemTemplate>
                            <ItemStyle Width="8%" />
                        </asp:TemplateField>
                        <asp:BoundField DataField="FirstName" HeaderText="First Name" ReadOnly="True"/>
                        <asp:BoundField DataField="MiddleNames" HeaderText="Middle Names" ReadOnly="True"/>
                        <asp:BoundField DataField="LastName" HeaderText="Last Name" ReadOnly="True"/>
                        <asp:BoundField DataField="PASNumber" HeaderText="PAS" ReadOnly="True"/>
                        <asp:BoundField DataField="AttachedPAS" HeaderText="Attached PAS" ReadOnly="True"/>
                    </Columns>
                </asp:GridView>
            </div>
        </asp:Panel>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFooter" runat="Server">
</asp:Content>
