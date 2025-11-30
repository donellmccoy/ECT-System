<%@ Page Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false"
    MaintainScrollPositionOnPostback="true" Inherits="ALOD.Web.Sys.Secure_Shared_System_WorkStatus" Codebehind="WorkStatus.aspx.vb" %>

<asp:Content ID="Content3" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentFooter" runat="server">
    <script type="text/javascript">

        $(function () {
            $('#optionDlg').dialog({
                autoOpen: false,
                modal: true,
                resizable: false,
                width: 600,
                height: 210,
                buttons: {
                    'Save': function () {

                        serializeEditor();
                        $(this).dialog('close');
                        //$(this).dialog('option', 'modal', false);
                        element('<%= btnOptionGo.ClientId %>').click();
                    },
                    'Cancel': function () {

                        $(this).dialog('close');

                    }
                }

            });
        });

        function serializeEditor() {
            var value = element('<%= txtWorkStatus.ClientId %>').value + "|";
            var value = value + element('<%= txtOptionId.ClientId %>').value + "|";
            var value = value + getSelectedValue('<%= cbOptionStatusOut.ClientId %>') + "|";
            var value = value + element('<%= txtOptionOrder.ClientId %>').value + "|";
            var value = value + element('<%= cbOptionActive.ClientId %>').checked + "|";
            var value = value + element('<%= txtOptionText.ClientId %>').value + "|";
            var value = value + getSelectedValue('<%= TemplateDropDown.ClientId %>') + "|";
            var value = value + getSelectedValue('<%= cbCompo.ClientId %>');

            element('<%= txtEditValue.ClientId %>').value = value;
        }

        function showEditor(url) {
            showPopup({
                'Url': url,
                'Width': '900',
                'Height': '700',
                'Resizable': true,
                'Center': false,
                'Reload': false,
                'ReloadButton': element('<%= btnRefresh.ClientId %>')
            });
        }

        function editOption(workstatus, optionId, statusOut, order, text, template, compo) {
            //store some values for the postback
            element('<%= txtWorkStatus.ClientId %>').value = String(workstatus);
            element('<%= txtOptionId.ClientId %>').value = String(optionId);

            //set the title
            //element('spTitle').innerHTML = optionId === 0 ? "Add New Option" : "Edit Option";
            //element('btnOptionGo').value = optionId === 0 ? "Add" : "Update";

            //set the status in and out
            setSelectedValue('<%= cbOptionStatusIn.ClientId %>', String(workstatus));
            enableControl('<%= cbOptionStatusIn.ClientId %>', false);

            if (statusOut !== 0) {
                setSelectedValue('<%= cbOptionStatusOut.ClientId %>', String(statusOut));
            }

            //default the order and text
            element('<%= txtOptionText.ClientId %>').value = text;
            element('<%= txtOptionOrder.ClientId %>').value = String(order);

            //dbsign template
            setSelectedValue('<%= TemplateDropDown.ClientId %>', String(template));
            setSelectedValue('<%= cbCompo.ClientId %>', String(compo));

            //show the dialog window
            $('#optionDlg').dialog({width: 700,height:300}).dialog('open');

            //move the focus to the text box
            element('<%= txtOptionText.ClientId %>').focus();
        }

        function moveStatus(up, id, order) {
            element('<%= txtMoveId.ClientId %>').value = id;
            element('<%= txtMoveOrder.ClientId %>').value = order;
            element(up ? '<%= btnMoveUp.ClientId %>' : '<%= btnMoveDown.ClientId %>').click();
        }
    </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <div>
        <asp:TextBox runat="server" ID="txtMoveId" CssClass="hidden" />
        <asp:TextBox runat="server" ID="txtMoveOrder" CssClass="hidden" />
        <asp:Button runat="server" ID="btnMoveUp" CssClass="hidden" />
        <asp:Button runat="server" ID="btnMoveDown" CssClass="hidden" />
        <asp:Button runat="server" ID="btnRefresh" CssClass="hidden" />
        <asp:Button runat="server" ID="btnOptionGo" CssClass="hidden" />
        <asp:TextBox runat="server" ID="txtEditValue" CssClass="hidden" />
        <div id="optionDlg" title="Edit Option" class="hidden" style="height:400px !important;">
            <table border="0">
                <tr>
                    <td class="align-right" style="width: 76px">
                        Status In:
                    </td>
                    <td style="width: 320px">
                        <asp:DropDownList ID="cbOptionStatusIn" runat="server" Width="306px">
                        </asp:DropDownList>
                    </td>
                    <td class="align-right" style="width: 60px">
                        Order:
                    </td>
                    <td>
                        <div>  <%--style="position:absolute; right:120px; top:0px;"--%>
                        <asp:TextBox ID="txtOptionOrder" runat="server" Width="100px" Font-Size="12px" style=""></asp:TextBox><%--position:absolute; right:0px; top:3px;--%>
                        <ajax:NumericUpDownExtender ID="NumericUpDownExtender1" runat="server" TargetControlID="txtOptionOrder" Width="50" /></div>
                    </td>
                </tr>
                <tr>
                    <td class="align-right" style="width: 76px">
                        Status Out:
                    </td>
                    <td style="width: 320px">
                        <asp:DropDownList ID="cbOptionStatusOut" Width="306px" runat="server">
                        </asp:DropDownList>
                    </td>
                    <td class="align-right"" style="width: 60px">
                        Active:
                    </td>
                    <td>
                        <asp:CheckBox ID="cbOptionActive" runat="server" Checked="True" />
                    </td>
                    
                </tr>
                <tr>
                    <td class="align-right" style="width: 76px">
                        Text:
                    </td>
                    <td style="width: 320px">
                        <asp:TextBox ID="txtOptionText" runat="server" Width="300px"></asp:TextBox>
                    </td>
                    <td class="align-right" style="width: 60px">
                        DBSign:
                    </td>
                    <td class="align-left">
                        <asp:DropDownList runat="server" ID="TemplateDropDown">
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="align-right" style="width: 76px">
                       
                    </td>
                    <td style="width: 320px">
                       
                    </td>
                    <td class="align-right" style="width: 60px">
                        Compo:
                    </td>
                    <td class="align-left">
                        <asp:DropDownList Id="cbCompo" runat="server" Visible="true">
                            <asp:ListItem Value="0">All</asp:ListItem>
                            <asp:ListItem Value="5">Air National Guard</asp:ListItem>
                            <asp:ListItem Value="6">Air Force Reserve</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:TextBox runat="server" ID="txtWorkStatus" CssClass="hidden" />
                        <asp:TextBox runat="server" ID="txtOptionId" CssClass="hidden" />
                    </td>
                </tr>
            </table>
        </div>
        
        <br />
        <asp:Repeater ID="rptSteps" runat="server">
            <ItemTemplate>
                <table class="gridViewMain" style="width: 900px;">
                    <tr class="workHeader">
                        <td>
                            <strong>
                                <%#Eval("SortOrder")%>
                                -
                                <%#Eval("Description")%>
                            </strong>&nbsp;&nbsp;(<%#Eval("GroupName")%>)
                        </td>
                        <td style="text-align: right">
                            <asp:Image runat="server" ID="imgAddOption" SkinID="imgAdd" CssClass="pointer" AlternateText="Add Option"
                                ImageAlign="AbsMiddle" />
                            <asp:Image CssClass="pointer" ID="imgMoveUp" runat="server" SkinID="imgArrowUp" AlternateText="Move Up"
                                ImageAlign="AbsMiddle" />
                            <asp:Image CssClass="pointer" ID="imgMoveDown" runat="server" SkinID="imgArrowDown"
                                ImageAlign="AbsMiddle" AlternateText="Move Down" />
                        </td>
                    </tr>
                </table>
                <asp:Repeater runat="server" ID="rptChild" DataSource='<%# GetOptions(Eval("Id")) %>'
                    OnItemDataBound="rptChild_ItemDataBound">
                    <ItemTemplate>
                        <table class='<%#RowStyle(Container.ItemIndex)%>'>
                            <tr>
                                <td style="width: 50px; text-align: right; font-weight: bold;">
                                    Text:
                                </td>
                                <td style="width: 301px">
                                    <asp:Label ID="lblText" runat="server"><%#Eval("Text")%></asp:Label>
                                </td>
                                <td style="width: 50px; text-align: right; font-weight: bold;">
                                    Rules:
                                </td>
                                <td style="width: 98px">
                                    <asp:Label ID="lblRules" runat="server" Text="0"></asp:Label>
                                    &nbsp;[<asp:HyperLink ID="lnkRules" runat="server" NavigateUrl="#">edit</asp:HyperLink>]
                                </td>
                                <td style="text-align: right; font-weight: bold;">
                                    DBSign:
                                </td>
                                <td>
                                    <asp:Label ID="TemplateLabel" runat="server"><%#Eval("DBSignTemplate")%></asp:Label>
                                </td>
                                <td style="text-align: right;">
                                    <asp:LinkButton ID="lnkDelete" runat="server" CommandArgument='<%# Eval("Id") %>'
                                        CommandName="Delete" OnCommand="DeleteOption">Delete</asp:LinkButton>
                                    <ajax:ConfirmButtonExtender TargetControlID="lnkDelete" ConfirmText="This will delete all Rules and Actions associated with this option. Are you sure you want to delete it?"
                                        ID="ConfirmButtonExtender1" runat="server">
                                    </ajax:ConfirmButtonExtender>
                                    &nbsp;
                                    <asp:LinkButton runat="server" ID="lnkEdit">Edit</asp:LinkButton>
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 50px; text-align: right; font-weight: bold;">
                                    Queue:
                                </td>
                                <td style="width: 301px">
                                    <asp:Label ID="lblRole" runat="server"><%#Eval("StatusOutText")%></asp:Label>
                                </td>
                                <td style="width: 50px; text-align: right; font-weight: bold;">
                                    Actions:
                                </td>
                                <td style="width: 98px">
                                    <asp:Label ID="lblActions" runat="server" Text="0"></asp:Label>
                                    &nbsp;[<asp:HyperLink ID="lnkActions" runat="server" NavigateUrl="#">edit</asp:HyperLink>]
                                </td>
                                <td style="text-align: right; font-weight: bold;">
                                    Order:
                                </td>
                                <td>
                                    <asp:Label ID="lblOrder" runat="server"><%#Eval("SortOrder")%></asp:Label>
                                </td>
                                <td style="text-align: right; font-weight: bold; <%# IIF(Eval("Compo") = 5 OrElse Eval("Compo") = 6, "color: red;","") %>">
                                    Compo: <%#Eval("CompoName")%>
                                </td>
                            </tr>
                        </table>
                    </ItemTemplate>
                    <FooterTemplate>
                            
                        <table border="0" class="footerWorkPanel">
                            <tr id="test" runat="server">
                                <td style="width:350px; text-align: Left; font-weight: bold; padding-left: 10px;">Email Reminder
                                </td>
                                <td style="width: 50px; text-align: Left; font-weight: bold;">
                                    Actions:
                                </td>
                                <td style="text-align: Left;">
                                    <asp:Label ID="lblReminderCount" runat="server" Text="0"></asp:Label>
                                    &nbsp;[<asp:HyperLink ID="linkEmailActions" runat="server"  NavigateUrl="#">edit</asp:HyperLink>]
                                </td>
                              </tr>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
            </ItemTemplate>
            <SeparatorTemplate>
                <br />
            </SeparatorTemplate>
        </asp:Repeater>
    </div>
    <asp:HiddenField ID="WorkStatusHiddenField" runat="server" Value="" />
    <asp:HiddenField ID="WsoHiddenField" runat="server" Value="" />
    <asp:HiddenField ID="StatusHiddenField" runat="server" Value="" />
</asp:Content>
