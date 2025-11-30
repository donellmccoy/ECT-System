<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/ReinvestigationRequests/ReinvestigationRequest.master" AutoEventWireup="false" Inherits="ALOD.Web.RR.Secure_rr_NextAction" MaintainScrollPositionOnPostback="true" Codebehind="NextAction.aspx.vb" %>

<%@ Register Src="~/Secure/Shared/UserControls/ValidationResults.ascx" TagName="validationresults"
    TagPrefix="lod" %>
<%@ Register Src="../Shared/UserControls/SignatureBlock.ascx" TagName="SignatureBlock"
    TagPrefix="uc1" %>
<%@ Reference Control="~/Secure/Shared/UserControls/TabNavigator.ascx" %>
<%@ MasterType VirtualPath="~/Secure/ReinvestigationRequests/ReinvestigationRequest.master" %>
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

            //set up event handlers for next action options

            $('span.option-routing > input').click(function () {
                checkSelection();
            });

        });                   //end document.ready

       
        function checkSelection() {
            __doPostBack('CheckRoutingSelection', '')
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
            <asp:Label ID="RLBTitle" runat="server" Text="Returned Without Action"></asp:Label>
        </div>
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
                            Reason for return/cancel:
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

                <!-- reason for routing option -->
                <tr id="reasonRow" runat="server" class="reasonRow" visible="false">
                    <td class="number">
                        <asp:Label ID="reasonLetter" runat="server"></asp:Label>
                    </td>
                    <td id="rowTitle" class="label labelRequired" runat="server">
                        <asp:Label ID="reasonTitle" runat="server"></asp:Label>
                    </td>
                    <td class="value">
                        <asp:Label ID="lblReason" runat="server" CssClass="lblDisableText"></asp:Label>
                        <asp:Panel runat="server" ID="ReasonPanel">
                            <asp:RadioButtonList ID="rblReason" runat="server" RepeatDirection="Vertical">
                            </asp:RadioButtonList>
                        </asp:Panel>
                    </td>
                </tr>
                <!-- comments for routing option -->
                <tr id="commentsRow" runat="server" class="reasonRow" visible="false">
                    <td class="number">
                       <asp:Label ID="commentLetter" runat="server"></asp:Label>
                    </td>
                    <td id="Comments" class="label labelRequired" runat="server">
                        <asp:Label ID="commentsTitle" runat="server"></asp:Label>
                    </td>
                    <td class="value">
                        <asp:Label ID="lblComments" runat="server"></asp:Label>
                        <asp:TextBox ID="CommentsTextBox" runat="server" Width="500px" TextMode="MultiLine"
                            Rows="5"></asp:TextBox>
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
