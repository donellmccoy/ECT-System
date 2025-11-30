<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Sys.Secure_Shared_System_PageAccess" Codebehind="PageAccess.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <script type="text/javascript">
        
        function setAll(id, type)
        {
            if (id == '0') {
                
                $('.pgAccess').each(function(){
                    setSelectedValue(this.id, type);
                });
                
            } else {
                $('#dvr_' + id + ' .pgAccess').each(function(){
                    setSelectedValue(this.id, type);
                });
            }
        }
    
    </script>

    Compoo:
    <asp:DropDownList ID="cbCompo" runat="server" AutoPostBack="True">
        <asp:ListItem Value="6" Text="Air Force Reserve" />
        <asp:ListItem Value="5" Text="Air National Guard" />
    </asp:DropDownList>
    &nbsp; 
    Workflow:
    <asp:DropDownList ID="cbWorkflow" runat="server" AutoPostBack="True" DataSourceID="dataWorkflow" DataTextField="Title" DataValueField="Id"/>
    &nbsp; 
    Status:
    <asp:DropDownList ID="cbStatus" runat="server" AutoPostBack="True"  DataSourceID="dataStatus" DataTextField="Description" DataValueField="Id"/>
    <br />
    <br />
    <hr />
    <br />
    <div class="cmdBox">
        <%--<input type="button" value="All None" onclick="setAll('0','0');" />&nbsp;
        <input type="button" value="All Read Only" onclick="setAll('0','1');" />&nbsp;
        <input type="button" value="All Read Write" onclick="setAll('0','2');" />&nbsp;--%>
        <%--<asp:Button runat="server" ID="btnUpdate" Text="Update All" />--%>
        <input type="submit" id="btnUpdate" value="Update All" runat="server" onserverclick="btnUpdate_ServerClick" style="float: right;" />&nbsp;
    </div>
    <br />
    <br />
    <asp:Repeater ID="rptGroups" runat="server">
        <HeaderTemplate>
        </HeaderTemplate>
        <ItemTemplate>
            <div id='dvr_<%# Eval("Id") %>' style="float: left; padding: 10px; border: 1px solid black;">
                <table>
                    <thead>
                        <tr>
                            <th colspan="2" style="width: 290px;">
                                <strong>
                                    <%#Eval("Description")%>
                                    <asp:Label ID="lblGroupId" runat="server" Text='<%#Eval("Id")%>' CssClass="hidden" />
                                </strong>
                            </th>
                        </tr>
                    </thead>
                    <tbody>
                        <tr>
                            <asp:Repeater runat="server" ID="rptPages" OnItemDataBound="ChildDataBound">
                                <ItemTemplate>
                                    <tr class="gridViewRow">
                                        <td>
                                            <asp:Label runat="server" ID="lblPageTitle" Text='<%#Eval("PageTitle")%>' />
                                            <asp:Label runat="server" ID="lblPageId" Text='<%#Eval("PageId")%>' CssClass="hidden" />
                                        </td>
                                        <td>
                                            <asp:DropDownList runat="server" ID="cbAccess" CssClass="pgAccess">
                                                <asp:ListItem Value="0">None</asp:ListItem>
                                                <asp:ListItem Value="1">Read Only</asp:ListItem>
                                                <asp:ListItem Value="2">Read/Write</asp:ListItem>
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                        </tr>
                    </tbody>
                </table>
                <br />
                <%--<input type="button" value="None" onclick="setAll('<%# Eval("Id") %>', '0');" />&nbsp;
                <input type="button" value="Read Only" onclick="setAll('<%# Eval("Id") %>', '1');" />&nbsp;
                <input type="button" value="Read Write" onclick="setAll('<%# Eval("Id") %>', '2');" />&nbsp;--%>
                <br />
                <br />
            </div>
        </ItemTemplate>
        <FooterTemplate>
        </FooterTemplate>
    </asp:Repeater>
    &nbsp;
    <br style="clear: both;" />
    <br />

    <asp:ObjectDataSource ID="dataWorkflow" runat="server" TypeName="ALOD.Data.Services.LookupService" SelectMethod="GetWorkflowsByCompo">
       <SelectParameters>
           <asp:ControlParameter ControlID="cbCompo" Name="compo" PropertyName="SelectedValue" Type="String" />
     </SelectParameters>
       </asp:ObjectDataSource>
       
    <asp:ObjectDataSource ID="dataStatus" runat="server" TypeName="ALOD.Data.Services.LookupService" SelectMethod="GetWorkstatusByWorkflow">
        <SelectParameters>
            <asp:ControlParameter ControlID="cbWorkflow" Name="workflow" PropertyName="SelectedValue" Type="Byte" />
        </SelectParameters>
    </asp:ObjectDataSource> 
</asp:Content>