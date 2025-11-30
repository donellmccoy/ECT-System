<%@ Control Language="VB" AutoEventWireup="false"
    Inherits="ALOD.Web.UserControls.Secure_Shared_UserControls_FindingsControl" Codebehind="FindingsControl.ascx.vb" %>
<div id="prevFindings" Visible="false" runat="server">
    <table>
        <tr>
            <td class="number">
                <asp:Label runat="server" ID="prevlbl">&nbsp;</asp:Label>
            </td>
            <td class="label align-left">
                <asp:Label ID="PrvFindingLabel"  runat="server">Findings By  </asp:Label>&nbsp;
            </td>
            <td>
                <asp:Label ID="PrvFindingControl" runat="server" CssClass="lblDisableText"></asp:Label>
            </td>
        </tr>
    </table>
</div>
<div id="dvDecision" runat="server">
    <table>
        <tr>
            <td class="number">
                <asp:Label runat="server" ID="DecisionLabel">&nbsp;</asp:Label>
            </td>
            <td class="label">
                <asp:Label ID="divDecsionLable" runat="server">  Decision:</asp:Label>&nbsp;
            </td>
            <td>
                <asp:Label ID="lblDecsion" runat="server" CssClass="lblDisableText"></asp:Label>
                <span class="normal">
                    <asp:RadioButtonList ID="rblDecison" CssClass="fieldNormal" runat="server" RepeatDirection="Vertical">
                        <asp:ListItem Value="Y">Concur with the action of Investigation Officer</asp:ListItem>
                        <asp:ListItem Value="N">Non Concur with the action of Investigation Officer</asp:ListItem>
                    </asp:RadioButtonList>
                </span>
            </td>
        </tr>
    </table>
</div>
<div id="dvFindings" runat="server">
    <table>
        <tr>
            <td class="number">
                <asp:Label runat="server" ID="FindingsLabel">&nbsp;</asp:Label>
            </td>
            <td class="label">
                <asp:Label ID="divFindingLabel" runat="server">  Findings:</asp:Label>
            </td>
            <td class="value">
                <asp:Label ID="lblFindings" runat="server" CssClass="lblDisableText"></asp:Label>
                <asp:RadioButtonList ID="rblFindings" CssClass="fieldNormal" runat="server" RepeatDirection="Vertical"
                    DataTextField="Description" DataValueField="Id">
                </asp:RadioButtonList>
            </td>
        </tr>
    </table>
</div>
<div id="dvRemarks" runat="server">
    <table runat="server" id="RemarksTable">
        <tr>
            <td class="number">
                <asp:Label runat="server" ID="RemarksNumber">&nbsp;</asp:Label>
            </td>
            <td class="label">
                <asp:Label ID="lblRemarks" Text="Comments" runat="server">  </asp:Label>
            </td>
            <td class="value">
                <asp:Label ID="RemarkTextlbl" runat="server"></asp:Label>
                <asp:TextBox ID="txtRemarks" runat="server" Width="500px" TextMode="MultiLine" Rows="5"></asp:TextBox>
            </td>
        </tr>
    </table>
    <div runat="server" id="dvAdditionalRemarks" visible="false">
        <table runat="server" id="AdditionalRemarksTable">
            <tr>
                <td class="number">
                    <asp:Label runat="server" ID="lblAdditionalRemarksNumber">&nbsp;</asp:Label>
                </td>
                <td class="label">
                    <asp:Label ID="lblAdditionalRemarks" Text="Additional Comments" runat="server">  </asp:Label>
                </td>
                <td class="value">
                    <asp:Label ID="lblAdditionalRemarksText" runat="server"></asp:Label>
                    <asp:TextBox ID="txtAdditionalRemarks" runat="server" Width="500px" TextMode="MultiLine" Rows="5"></asp:TextBox>
                </td>
            </tr>
        </table>
    </div>
    <div id="dvFormText" visible="false" runat="server">
        <table>
            <tr>
                <td class="number">
                    <asp:Label runat="server" ID="FindingsNumber">&nbsp;</asp:Label>
                </td>
                <td class="label">
                    <asp:Label runat="server" ID="ReasonsLabel" Text="Findings:" /> <br />
                     <asp:Label runat="server" ID="AppearsLabel" Text="(Shown on Form 348)" />
                </td>
                <td class="value">
                    <asp:Label ID="RemarkTextlbl2" runat="server"></asp:Label>
                    <asp:TextBox ID="txtRemarks2" runat="server" Width="500px" TextMode="MultiLine" MaxLength="1200" Rows="5"></asp:TextBox>
                </td>
            </tr>
        </table>
    </div>
</div>
    <script type="text/javascript">
        function BindControlEvents() {
            var descn = $(document.getElementById('<%=Me.ClientID%>' + '_rblDecison_0'));
            if (descn) {
                if ($("input[@id='" + '<%=Me.ClientID%>' + '_rblDecison_0' + "']:checked", '#rblDecison').val() == 'Y') {
                    var fndg = $(document.getElementById('<%=Me.ClientID%>' + '_rblFindings'));
                    if (fndg) {
                        var rbl = document.getElementById('<%=Me.ClientID%>' + '_rblFindings_0');
                        if (rbl)
                         {
                             unCheckRadioList($(rbl).attr("name"));
                         } 
                         fndg.attr("disabled", true);
                    }
                }
            }
        }

        (function() {
            BindControlEvents();
        });


    </script>

    <asp:ScriptManagerProxy runat="server" ID="smp">
        <Scripts>
            <asp:ScriptReference Path="~/Script/FindingsControl.js" />
        </Scripts>
    </asp:ScriptManagerProxy>

