<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Admin.Secure_Shared_Admin_MemberData" Codebehind="MemberData.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <div class="indent">
        <div class="border-thin">
            <div class="searchTitle">
                User Search</div>
            <div class="searchBody">
                <p style="text-align: center;">
                    <asp:Label runat="server" AssociatedControlID="txtLastName" Text="Last Name:" />
                    <asp:TextBox ID="txtLastName" runat="server" Width="100px" AutoPostBack="true" MaxLength="50"></asp:TextBox>&nbsp;&nbsp;
                    <asp:Label runat="server" AssociatedControlID="txtFirstName" Text="First Name:" />
                    <asp:TextBox ID="txtFirstName" runat="server" Width="100px" AutoPostBack="true" MaxLength="50"></asp:TextBox>&nbsp;&nbsp;
                    <asp:Label runat="server" AssociatedControlID="txtMiddleName" Text="Middle Name:" />
                    <asp:TextBox ID="txtMiddleName" runat="server" Width="100px" AutoPostBack="true" MaxLength="60"></asp:TextBox>&nbsp;&nbsp;
                    <br />
                    <br />
                    <asp:Label runat="server" AssociatedControlID="SsnText" Text="Last Four SSN Digits:" />
                    <asp:TextBox ID="SsnText" runat="server" Width="70px" AutoPostBack="true" MaxLength="4"></asp:TextBox>&nbsp;&nbsp;
                    <asp:Label runat="server" AssociatedControlID="UnitSelect" Text="Unit:" />
                    <asp:DropDownList ID="UnitSelect" Width="280px" AutoPostBack="true" runat="server">
                    </asp:DropDownList>
                </p>
                <p style="text-align: right;">
                    <asp:Button ID="SearchButton" runat="server" Text="View in Browser" CausesValidation="False">
                    </asp:Button>&nbsp;
                    <asp:Button runat="server" ID="ExportButton" Text="Export to Excel" Visible="True" />
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
                <asp:GridView ID="MembersGrid" runat="server" Width="900px" HorizontalAlign="Center"
                    AllowSorting="True" AllowPaging="True" PageSize="30" DataKeyNames="SSAN" AutoGenerateColumns="False"
                    DataSourceID="MemberData">
                    <Columns>
                        <asp:TemplateField HeaderText="Name" SortExpression="LastName">
                            <ItemTemplate>
                                <%# Eval("LastName") + ", " + Eval("FirstName") + " " + Eval("MiddleName") %>
                            </ItemTemplate>
                            <ItemStyle Width="300px"></ItemStyle>
                        </asp:TemplateField>
                        <asp:BoundField HeaderText="SSN" SortExpression="LastFour" DataField="LastFour">
                            <ItemStyle Width="150px" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="Unit" SortExpression="CurrentUnitName">
                            <ItemTemplate>
                                <%#Eval("CurrentUnitName")%>
                            </ItemTemplate>
                            <ItemStyle Width="540px"></ItemStyle>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Role Name" SortExpression="RoleName">
                            <ItemTemplate>
                                <asp:Label Text='<%#Eval("RoleName")%>' ID="RoleLabel" runat="server" Visible='<%# Eval("Id") <> 0 %>' />
                            </ItemTemplate>
                            <ItemStyle Width="300px"></ItemStyle>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="">
                            <ItemTemplate>
                                <asp:ImageButton ID="lnkCreateUser" SkinID="imgUserEdit" ToolTip="Create User" runat="server"
                                    Visible='<%# Eval("Id") = 0 %>' CommandName="CreateUser" CommandArgument="<%#     Ctype( Container,GridViewRow ).RowIndex %>">
                                </asp:ImageButton>
                                <asp:ImageButton ID="EditUserImg" ToolTip="Edit User " SkinID="imgUserEdit" runat="server"
                                    Visible='<%# Eval("Id") <> 0 %>' CommandName="EditUserInfo" CommandArgument='<%# Eval("Id") %>'>
                                </asp:ImageButton>
                            </ItemTemplate>
                            <ItemStyle Width="100px"></ItemStyle>
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:ImageButton ID="EditMemberImg" ToolTip="Edit Member Data" SkinID="buttonEdit"
                                    runat="server" CommandName="EditMemberInfo" CommandArgument="<%# Ctype( Container,GridViewRow ).RowIndex %>">
                                </asp:ImageButton>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                            <ItemStyle HorizontalAlign="Center"></ItemStyle>
                        </asp:TemplateField>
                    </Columns>
                    <EmptyDataRowStyle Font-Bold="true" CssClass="emptyItem" />
                    <EmptyDataTemplate>
                        No users found</EmptyDataTemplate>
                    <PagerStyle HorizontalAlign="Center" VerticalAlign="Bottom"></PagerStyle>
                </asp:GridView>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="SearchButton" EventName="Click" />
                <asp:AsyncPostBackTrigger ControlID="UnitSelect" EventName="SelectedIndexChanged" />
                <asp:AsyncPostBackTrigger ControlID="SsnText" EventName="TextChanged" />
                <asp:AsyncPostBackTrigger ControlID="txtLastName" EventName="TextChanged" />
                <asp:AsyncPostBackTrigger ControlID="txtFirstName" EventName="TextChanged" />
                <asp:AsyncPostBackTrigger ControlID="txtMiddleName" EventName="TextChanged" />
            </Triggers>
        </asp:UpdatePanel>
    </div>
    <asp:ObjectDataSource ID="MemberData" runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="SearchMemberData" TypeName="ALOD.Data.Services.UserService">
        <SelectParameters>
            <asp:SessionParameter Name="userId" SessionField="UserId" Type="Int32" />
            <asp:SessionParameter Name="rptView" SessionField="ReportView" Type="Int32" />
            <asp:ControlParameter ControlID="SsnText" Name="ssn" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtLastName" Name="lastName" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtFirstName" Name="firstName" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="txtMiddleName" Name="middleName" PropertyName="Text" Type="String" />
            <asp:ControlParameter ControlID="UnitSelect" Name="unitId" PropertyName="SelectedValue" Type="Int32" />
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
