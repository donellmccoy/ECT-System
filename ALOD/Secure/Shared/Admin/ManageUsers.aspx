<%@ Page Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Admin.Secure_Shared_Admin_ManageUsers" Title="Untitled Page" Codebehind="ManageUsers.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <div class="indent">
        <div class="border-thin">
            <div class="searchTitle">
                User Search</div>
            <div class="searchBody">
                <p style="text-align: left">
                    <asp:Label runat="server" AssociatedControlID="NameText" Text="Name:" />
                    <asp:TextBox ID="NameText" runat="server" AutoPostBack="True" Width="100px" MaxLength="30"></asp:TextBox>&nbsp;&nbsp;
                    <asp:Label runat="server" AssociatedControlID="SsnText" Text="Last Four:" />
                    <asp:TextBox ID="SsnText" runat="server" AutoPostBack="True" Width="40px" 
                        MaxLength="4"></asp:TextBox>&nbsp;&nbsp;
                    <asp:Label runat="server" AssociatedControlID="StatusSelect" Text="Status:" />
                    <asp:DropDownList ID="StatusSelect" AutoPostBack="True" runat="server">
                        <asp:ListItem Value="0">All</asp:ListItem>
                        <asp:ListItem Value="3">Approved</asp:ListItem>
                        <asp:ListItem Value="2">Pending</asp:ListItem>
                        <asp:ListItem Value="4">Disabled</asp:ListItem>
                    </asp:DropDownList>
                    &nbsp;&nbsp;
                    <asp:Label runat="server" AssociatedControlID="RoleSelect" Text="Role:" />
                    <asp:DropDownList ID="RoleSelect" AutoPostBack="True" runat="server" Width="136px"
                        DataTextField="name" DataValueField="groupId">
                    </asp:DropDownList>
                    &nbsp;&nbsp;
                    <asp:Label runat="server" AssociatedControlID="UnitSelect" Text="Unit:" />
                    <asp:DropDownList ID="UnitSelect" AutoPostBack="True" Width="250px" runat="server">
                    </asp:DropDownList>
                   
                    <p style="text-align: center">
                        <asp:CheckBox ID="CheckBoxShowAllUsers" Text="Show All Users " Visible ="False" AutoPostBack="True"
                                    runat="server" />
                                    </p>
                <p style="text-align: right;">
                    <asp:Button ID="SearchButton" runat="server" Text="View in Browser" CausesValidation="False">
                    </asp:Button>&nbsp;
                    <asp:Button runat="server" ID="ExportButton" Text="Export to Excel" Visible="True"
                        OnClick="btnExport_Click" />
                </p>
            </div>
        </div>
        <br />
        <br />
        <asp:UpdatePanel ID="resultsUpdatePanel" runat="server" ChildrenAsTriggers="True">
            <ContentTemplate>
                <asp:Panel runat="server" ID="SearchMessage" CssClass="emptyItem" Visible="true">
                    Please enter at least one criteria to search by
                </asp:Panel>
                <div id="spSpacer" class="emptyItem" style="margin-top: 0px; margin-bottom: 4px;
                    height: 22px;">
                    <div id="spWait" class="" style="display: none;">
                        &nbsp;<asp:Image runat="server" ID="imgWait" SkinID="imgBusy" AlternateText="busy"
                            ImageAlign="AbsMiddle" />&nbsp;Loading...
                    </div>
                </div>
                <asp:GridView ID="UsersGrid" runat="server" Width="920px" HorizontalAlign="Center"
                    AllowSorting="True" AllowPaging="True" PageSize="30" AutoGenerateColumns="False"
                    DataKeyNames="Id" DataSourceID="UserData">
                    <Columns>
                        <asp:TemplateField HeaderText="Status" SortExpression="AccessStatusText">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="StatusLabel"><%#Eval("AccessStatusText")%></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Name" SortExpression="LastName">
                            <ItemTemplate>
                                <%#Eval("LastName") + ", " + Eval("FirstName")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="User Name" SortExpression="username">
                            <ItemTemplate>
                                <%#Eval("UserName")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="SSN" SortExpression="LastFour" DataField="LastFour">
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Expiration Date" SortExpression="expirationDate">
                            <ItemTemplate>
                                <%# Eval("expirationDate")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Role" SortExpression="RoleName">
                            <ItemTemplate>
                                <%#Eval("RoleName")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Unit" SortExpression="CurrentUnitName">
                            <ItemTemplate>
                                <%#Eval("CurrentUnitName")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Compo" SortExpression="workCompo">
                            <ItemTemplate>
                                <%#Eval("workCompo")%>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Perms">
                            <ItemTemplate>
                                <asp:ImageButton ID="lnkPerms" SkinID="imgUserPerms" Visible='<%# AdminCanEdit(Eval("groupId"), Eval("ViewOrManaged")) = True %>'
                                    runat="server" CommandArgument='<%# Eval("Id") %>'  CommandName="Perms"></asp:ImageButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Activity">
                            <ItemTemplate>
                                <asp:ImageButton ID="lnkActivity" SkinID="imgUserActivity" Visible='<%# AdminCanEdit(Eval("groupId")) = True %>'
                                    runat="server" CommandArgument='<%# Eval("Id") %>' CommandName="Activity"></asp:ImageButton>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ImageAlign="AbsMiddle" SkinID="imgUserEdit" ID="lnk_account" CommandArgument='<%# Eval("Id") %>'
                                    ToolTip="User Info" runat="server" Visible='<%# AdminCanEdit(Eval("groupId"), Eval("ViewOrManaged")) = True %>'
                                    CommandName="EditInfo" />
                                
                            <%--<input  type="text" id="test8" value="<%#Eval("RoleName")%>"/>
                            <input  type="text" id="test9" value="Session(groupId)= <%#Session("groupId")%>"/>--%>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        </asp:TemplateField>
                        <asp:TemplateField Visible="False">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnk_roles" CommandArgument='<%# Eval("Id") %>' Text="User Roles"
                                    runat="server" CommandName="EditRole" />
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataRowStyle Font-Bold="true" CssClass="emptyItem" />
                    <EmptyDataTemplate>
                        No users found</EmptyDataTemplate>
                    <FooterStyle HorizontalAlign="Center" />
                    <PagerStyle HorizontalAlign="Center" VerticalAlign="Bottom"></PagerStyle>
                </asp:GridView>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="SearchButton" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="UnitSelect" EventName="SelectedIndexChanged" />
                <asp:AsyncPostBackTrigger ControlID="StatusSelect" EventName="SelectedIndexChanged" />
                <asp:AsyncPostBackTrigger ControlID="RoleSelect" EventName="SelectedIndexChanged" />
                <asp:AsyncPostBackTrigger ControlID="SsnText" EventName="TextChanged" />
                <asp:AsyncPostBackTrigger ControlID="NameText" EventName="TextChanged" />
            </Triggers>
        </asp:UpdatePanel>
    </div>
    <asp:ObjectDataSource ID="UserData" runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="SearchUsers" TypeName="ALODWebUtility.LookUps.LookUp">
        <SelectParameters>
            <asp:SessionParameter Name="userId" SessionField="UserId" Type="Int32" />
            <asp:ControlParameter ControlID="SsnText" Name="ssn" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="NameText" Name="name" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="StatusSelect" Name="status" PropertyName="SelectedValue"
                Type="Byte" />
            <asp:ControlParameter ControlID="RoleSelect" Name="role" PropertyName="SelectedValue"
                Type="Int32" />
            <asp:ControlParameter ControlID="UnitSelect" Name="unitId" PropertyName="SelectedValue"
                Type="Int32" />
             <asp:ControlParameter ControlID="CheckBoxShowAllUsers" Name="showAllUsers" PropertyName="Checked"
                Type="Boolean" />
        </SelectParameters>
    </asp:ObjectDataSource>
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
</asp:Content>
