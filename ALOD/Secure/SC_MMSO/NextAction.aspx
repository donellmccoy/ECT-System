<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/SC_MMSO/SC_MMSO.master" AutoEventWireup="false" Inherits="ALOD.Web.Special_Case.MMSO.Secure_sc_mmso_NextAction" MaintainScrollPositionOnPostback="true" Codebehind="NextAction.aspx.vb" %>

<%@ Register Src="~/Secure/Shared/UserControls/ValidationResults.ascx" TagName="validationresults"
    TagPrefix="lod" %>
<%@ Register Src="../Shared/UserControls/SignatureBlock.ascx" TagName="SignatureBlock"
    TagPrefix="uc1" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/SC_MMSO/SC_MMSO.master" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabControls.ascx" %>
<%@ Register Src="~/secure/Shared/UserControls/RLB.ascx" TagName="RLB" TagPrefix="uc" %>
<%@ Reference Control="~/Secure/Shared/UserControls/RLB.ascx" %>
<%@ Reference Control="~/Secure/Shared/UserControls/FindingsControl.ascx" %>
<%@ Register Src="~/secure/Shared/UserControls/FindingsControl.ascx" TagName="Findings"
    TagPrefix="uc" %>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterNested" runat="server">

    <script type="text/javascript">

        function SetUniqueRadioButton(nameregex, current) {
            re = new RegExp(nameregex);
            for (i = 0; i < document.forms[0].elements.length; i++) {
                elm = document.forms[0].elements[i]
                if (elm.type == 'radio') {
                    if (re.test(elm.name)) {
                        elm.checked = false;
                    }
                }
            }
            current.checked = true;
        }


        $(document).ready(function() {

            var show = false;

            $('span.option-rwoa > input').each(function() {
                if (this.checked) {
                    show = true;
                }
            });

            toggleRwoa(show);
            show = false;

            $('span.option-return > input').each(function() {
                if (this.checked) {
                    show = true;
                }
            });

            toggleReturn(show);
            show = false;

            $('span.option-cancel > input').each(function() {
                if (this.checked) {
                    show = true;
                }
            });

            toggleCancel(show);

            //set up event handlers for next action options
            $('span.option-return > input').click(function() {
                toggleReturn(true);
                toggleCancel(false);
                toggleRwoa(false);
            });

            $('span.option-rwoa > input').click(function() {
                toggleReturn(false);
                toggleCancel(false);
                toggleRwoa(true);
            });

            $('span.option-cancel > input').click(function() {
                toggleReturn(false);
                toggleCancel(true);
                toggleRwoa(false);
            });

            $('span.option-forward > input').click(function() {
                toggleReturn(false);
                toggleCancel(false);
                toggleRwoa(false);
            });

        });                   //end document.ready

        function toggleReturn(shown) {
            $('tr.commentRow').css('display', shown ? 'table-row' : 'none');
        }

        function toggleRwoa(shown) {
            $('tr.rwoaRow').css('display', shown ? 'table-row' : 'none');
        }

        function toggleCancel(shown) {
            $('tr.cancelRow').css('display', shown ? 'table-row' : 'none');
        }
    
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentNested" runat="Server">
    <div class="dataBlock">
        <div class="dataBlock-header">
            Validation
        </div>
        <div class="dataBlock-body">
            <br />
            <lod:validationresults ID="results" runat="server" />
            <br />
        </div>
    </div>
    <asp:Panel CssClass="dataBlock" runat="server" ID="divRLB" Visible="false">
        <div class="dataBlock-header">
            Returned Without Action</div>
        <div class="dataBlock-body">
            <uc:RLB ID="ucRLB" runat="server"></uc:RLB>
        </div>
    </asp:Panel>
    <div class="dataBlock">
        <div class="dataBlock-header">
            Next Action</div>
        <br />
        <div class="dataBlock-body">
            <asp:Label ID="signCancelLbl" runat="server" ForeColor="Red" Text=""></asp:Label>
            <asp:Panel runat="server" ID="CommentPanel">
                <table>
                    <tr>
                        <td class="number">
                            &nbsp;
                        </td>
                        <td class="label">
                            Reason for return:
                        </td>
                        <td class="value">
                            <asp:Label runat="server" ID="CommentLabel" />
                            <br />
                            <br />
                            <br />
                        </td>
                    </tr>
                </table>
                <br />
                <br />
            </asp:Panel>
            <table>
                <tr>
                    <td class="number">
                        A
                    </td>
                    <td class="label labelRequired">
                        * Routing
                    </td>
                    <!-- next action options -->
                    <td class="value">
                        <asp:Panel runat="server" ID="OptionPanel">
                            <asp:Repeater ID="rbtOption" runat="server">
                                <ItemTemplate>
                                    <ALod:ValueRadioButton ID="rblOptions" runat="server" GroupName="option" />
                                </ItemTemplate>
                                <SeparatorTemplate>
                                    <div class="repeaterSpacer"></div>
                                </SeparatorTemplate>
                            </asp:Repeater>
                        </asp:Panel>
                        <br />
                        <br />
                    </td>
                </tr>
                <!-- return without action -->
                <tr class="rwoaRow">
                    <td class="number">
                        B
                    </td>
                    <td class="label labelRequired">
                        * Reason for return:
                    </td>
                    <td class="value">
                        <asp:Label ID="lblRwoaReason" runat="server" CssClass="lblDisableText"></asp:Label>
                        <asp:Panel runat="server" ID="RwoaPanel">
                            <asp:RadioButtonList ID="rblReasons" runat="server" RepeatDirection="Vertical">
                                <asp:ListItem Value="1">Cancel LOD</asp:ListItem>
                                <asp:ListItem Value="2">Multiple diagnoses</asp:ListItem>
                                <asp:ListItem Value="3">No orders</asp:ListItem>
                                <asp:ListItem Value="4">Wrong orders provided</asp:ListItem>
                                <asp:ListItem Value="5">Wrong diagnosis</asp:ListItem>
                                <asp:ListItem Value="6">Orders do not cover active duty service in question</asp:ListItem>
                                <asp:ListItem Value="7">Police report not included with MVAs</asp:ListItem>
                                <asp:ListItem Value="8">No medical documentation</asp:ListItem>
                                <asp:ListItem Value="9">Insufficient medical documentation</asp:ListItem>
                                <asp:ListItem Value="10">Supporting documentation pertains to different individual</asp:ListItem>
                                <asp:ListItem Value="11">Supporting documentation is distorted/unreadable</asp:ListItem>
                            </asp:RadioButtonList>
                        </asp:Panel>
                    </td>
                </tr>
                <tr class="rwoaRow">
                    <td class="number">
                        C
                    </td>
                    <td class="label">
                        <asp:Label ID="lblSendersComments" runat="server" Text="RFA Comments:"></asp:Label>
                    </td>
                    <td class="value">
                        <asp:Label ID="SenderCommentslbl" runat="server"></asp:Label>
                        <asp:TextBox ID="txtSendersComments" runat="server" Width="500px" TextMode="MultiLine"
                            Rows="5"></asp:TextBox>
                    </td>
                </tr>
                <!-- Cancel investigation -->
                <tr class="cancelRow">
                    <td class="number">
                        B
                    </td>
                    <td class="label labelRequired">
                        * Reason for Cancellation:
                    </td>
                    <td class="value">
                        <asp:Label ID="lblrbCancel" Text="" runat="server" />
                        <asp:Panel runat="server" ID="CancelPanel">
                            <asp:RadioButtonList ID="rblCancellation" runat="server" RepeatLayout="Table">
                                <asp:ListItem Value="1">Duplicate LOD</asp:ListItem>
                                <asp:ListItem Value="2">LOD Started in error by MedTech</asp:ListItem>
                                <asp:ListItem Value="3">Annotation made in medical record</asp:ListItem>
                                <asp:ListItem Value="4">No Diagnosis</asp:ListItem>
                                <asp:ListItem Value="5">Member not in status when conditon incurred</asp:ListItem>
                                <asp:ListItem Value="6">Member failed to provide documentation</asp:ListItem>
                                <asp:ListItem Value="7">Other, please specify below</asp:ListItem>
                            </asp:RadioButtonList>
                        </asp:Panel>
                    </td>
                </tr>
                <tr class="cancelRow">
                    <td class="number">
                        C
                    </td>
                    <td class="label">
                        Comments:
                    </td>
                    <td class="value">
                        <asp:Label ID="lblCancelExp" Text="" runat="server" />
                        <asp:TextBox ID="txtCancel" runat="server" Rows="6" MaxLength="400" TextMode="MultiLine"
                            Columns="60" />
                    </td>
                </tr>
                <!-- return comments -->
                <tr class="commentRow">
                    <td class="number">
                        B
                    </td>
                    <td class="label">
                        Return Comment:
                    </td>
                    <td class="value">
                        <asp:TextBox runat="server" ID="CommentBox" TextMode="MultiLine" MaxLength="1000"
                            Columns="60" Rows="6"></asp:TextBox>
                    </td>
                </tr>
                <!-- action -->
                <tr>
                    <td class="number">
                        &nbsp;
                    </td>
                    <td class="label">
                        Action
                    </td>
                    <td class="value">
                        <asp:Button ID="SignButton" runat="server" Text="Digitally Sign" /><br />
                        <uc1:SignatureBlock ID="SigBlock" runat="server" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
