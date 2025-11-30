<%@ Control Language="VB" AutoEventWireup="false" Inherits="ALOD.Web.UserControls.Secure_Shared_UserControls_SearcherControl" Codebehind="SearcherControl.ascx.vb" %>
     <script type="text/javascript">

        function searchStart()
        {
            $('#spWait').show();
            $('#<%= gvResults.ClientId %>').hide();
             enableControl('<%= cmdSearch.ClientId %>',false);
        }
     
    </script>
<div>
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div ID="searchBox" runat="Server" class="searchBox">
                <div class="searchTitle">
                    <asp:Label ID="txtTitle" runat="server" Text="Filter Results" />
                </div>
                <div class="searchBody">
                    <table ID="Table5" style="width:100%;text-align: center;">
                        <tr style="text-align:center;">
                            <td style="height: 26px; width:100%; text-align:center;">
                                <asp:Label ID="lblEnterSSN" runat="server">SSN:  </asp:Label>
                                <asp:TextBox ID="txtSSN" runat="server" MaxLength="9" Width="100px"></asp:TextBox>
                                <asp:Label ID="lblEnterName" runat="server">Name:  </asp:Label>
                                <asp:TextBox ID="txtName" runat="server" Width="100px"></asp:TextBox>
                                <asp:Label ID="lblLODID" runat="server">Case Id:  </asp:Label>
                                <asp:TextBox ID="txtCaseID" runat="server" Width="100px"></asp:TextBox>
                                <asp:Label ID="lblLODStatus" runat="server">Status:  </asp:Label>
                                <asp:DropDownList ID="ddlLodStatus" runat="server"   Width="160px">
                                </asp:DropDownList>
                                <asp:Label ID="lblWorkflow" runat="server" Visible="false">Workflow:  </asp:Label>
                                <asp:DropDownList ID="ddlWorkflow" runat="server"   Width="140px">
                                </asp:DropDownList>
                                <asp:Label ID="lblIsFormal" runat="server" Visible="false">IsFormal:  </asp:Label>
                                <asp:DropDownList ID="ddlIsFormal" runat="server" Visible="false" Width="80px">
                                    <asp:ListItem Value=" ">--Select--</asp:ListItem>
                                    <asp:ListItem Value="1">Yes</asp:ListItem>
                                    <asp:ListItem Value="0">No</asp:ListItem>
                                </asp:DropDownList>
                                <asp:Label ID="lblIsOverdue" runat="server" Visible="false">IsOverdue:  </asp:Label>
                                <asp:DropDownList ID="ddlIsOverdue" runat="server" Visible="false" Width="80px">
                                    <asp:ListItem Value=" ">--Select--</asp:ListItem>
                                    <asp:ListItem Value="1">Yes</asp:ListItem>
                                    <asp:ListItem Value="0">No</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 100%; text-align:right;">
                                <asp:Button ID="cmdSearch" runat="server" CausesValidation="False" 
                                    Text="Search" />
                                &nbsp;</td>
                        </tr>
                    </table>
                </div>
            </div>
            <br />
            <br />
            <div ID="spWait" class="emptyItem" style="display: none;">
                <br />
                <br />
                &nbsp;<asp:Image ID="imgWait" runat="server" AlternateText="busy" 
                    ImageAlign="AbsMiddle" SkinID="imgBusy" />
                &nbsp;Please Wait...
            </div>
            <div ID="divResults" runat="Server" align="center">
                <asp:GridView ID="gvResults" runat="server"  
                      AutoGenerateColumns="False" 
                     PageSize="20" Width="900px">
                    <Columns>
                        <asp:TemplateField HeaderText="Case Id" SortExpression="caseId">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkRefID" runat="server" 
                                    CommandArgument='<%# Eval("refID").ToString()  %>' 
                                    CommandName="view" Text='<%# Eval("caseId") %>'>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataField="workflow" HeaderText="Workflow" 
                            SortExpression="workflow" />
                        <asp:BoundField DataField="name" HeaderText="Name" SortExpression="name" />
                       
                        <asp:BoundField DataField="status" HeaderText="Status" 
                            SortExpression="status" />
                        <asp:BoundField DataField="dateCreated" HeaderText="Date Created" 
                            SortExpression="dateCreated" Visible="False" />
 
                    </Columns>
                </asp:GridView>
                <asp:Label ID="lblMessage" runat="Server"></asp:Label>
                <input id="sort_expr" runat="Server" type="hidden" />
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
  
    <ajax:UpdatePanelAnimationExtender ID="UpdatePanelAnimationExtender1" 
        runat="server" TargetControlID="UpdatePanel1">
        <Animations>
                <OnUpdating>
                    <ScriptAction script="searchStart();" />
                </OnUpdating>
        </Animations>
    </ajax:UpdatePanelAnimationExtender>
</div>
