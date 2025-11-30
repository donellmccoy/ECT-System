<%@ Page Title="" Language="VB" MasterPageFile="~/ANGSecure/ANGSecure.master" AutoEventWireup="false" Inherits="ALOD.Web.LOD.Secure_lod_Inbox" MaintainScrollPositionOnPostback="true" Codebehind="Inbox.aspx.vb" %>

<%@ Import Namespace="ALODWebUtility.Common" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="Server">
    <div class="indent">
        <asp:Panel runat="server" ID="ErrorPanel" Visible="false" CssClass="ui-state-error info-block">
            <asp:Image runat="server" ID="Image1" ImageAlign="AbsMiddle" ImageUrl="~/images/warning.gif" />
            <asp:Label runat="server" ID="ErrorMessageLabel" />
        </asp:Panel>
        <div class="dataBlock">
            <div class="dataBlock-header">
                User Queue
            </div>
            <div class="dataBlock-body">
                <asp:Panel runat="server" ID="StartPanel">
                    <table>
                        <tr>
                            <td class="number">
                                A
                            </td>
                            <td class="label-small">
                                Username:
                            </td>
                            <td class="value">
                                <asp:TextBox runat="server" ID="UsernameInput"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                B
                            </td>
                            <td class="label-small">
                                Action:
                            </td>
                            <td class="value">
                                <asp:Button runat="server" ID="SearchButton" Text="Search" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
                <asp:Panel runat="server" ID="UserPanel" Visible="false">
                    <table>
                        <tr>
                            <td class="number">
                                A
                            </td>
                            <td class="label-small">
                                Select User:
                            </td>
                            <td class="value">
                                <asp:DropDownList runat="server" ID="UserSelect">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                B
                            </td>
                            <td class="label-small">
                                Formal:
                            </td>
                            <td class="value">
                                <asp:DropDownList ID="FormalSelect" runat="server">
                                    <asp:ListItem Value="">-All--</asp:ListItem>
                                    <asp:ListItem Value="False">No</asp:ListItem>
                                    <asp:ListItem Value="True">Yes</asp:ListItem>
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                C
                            </td>
                            <td class="label-small">
                                SARC Restricted:
                            </td>
                            <td class="value">
                                <asp:CheckBox runat="server" ID="SarcCheck" Checked="false" />
                            </td>
                        </tr>
                        <tr>
                            <td class="number">
                                D
                            </td>
                            <td class="label-small">
                                Action:
                            </td>
                            <td class="value">
                                <asp:Button runat="server" ID="ViewInboxButton" Text="View" />
                                &nbsp;
                                <asp:Button runat="server" ID="ResetButton" Text="Reset" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </div>
        </div>
        <br />
        <asp:Label runat="server" ID="UserDataLabel" />
        <br />
        <asp:GridView ID="ResultGrid" runat="server" AutoGenerateColumns="False" PageSize="20"
            Width="100%" DataSourceID="LodData" AllowPaging="True" AllowSorting="True" Visible="false">
            <Columns>
                <asp:TemplateField>
                    <ItemTemplate>
                        <asp:Image runat="server" ID="LockImage" ImageAlign="AbsMiddle" ImageUrl="~/images/lock.gif"
                            Visible="false" AlternateText="This case is locked for editing by another user" />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Case Id" SortExpression="CaseId">
                    <ItemTemplate>
                        <asp:HyperLink runat="server" ID="CaesLink" Text='<%#Eval("CaseId") %>' NavigateUrl='<%# String.Format("~/Secure/Lod/init.aspx?refId={0}",Eval("RefId")) %>' />
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="SSN" HeaderText="SSN" SortExpression="SSN" />
                <asp:TemplateField HeaderText="Name" SortExpression="name">
                    <ItemTemplate>
                        <%#Eval("name").ToString().ToUpper()%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="UnitName" HeaderText="Unit" SortExpression="UnitName" />
                <asp:BoundField DataField="ReceiveDate" HeaderText="Date Received" SortExpression="ReceiveDate" />
                <asp:BoundField DataField="Days" HeaderText="Days" SortExpression="Days" />
                <asp:TemplateField HeaderText="Print">
                    <ItemTemplate>
                        <asp:ImageButton ImageAlign="AbsMiddle" runat="server" ID="PrintImage" AlternateText="Print Forms"
                            ImageUrl="~/images/pdf.ico" />
                            <%--ImageUrl="~/images/pdf.ico" OnClientClick='<%# Eval("refId", "printForms({0}); return false;")%>' />--%>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <EmptyDataRowStyle CssClass="emptyItem" />
            <EmptyDataTemplate>
                No Results Found
            </EmptyDataTemplate>
        </asp:GridView>
    </div>
    <asp:ObjectDataSource ID="LodData" runat="server" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetByUser" TypeName="ALOD.Data.Services.LodService">
        <SelectParameters>
            <asp:Parameter Name="caseID" DefaultValue="" Type="String" />
            <asp:Parameter Name="ssn" DefaultValue="" Type="String" />
            <asp:Parameter Name="name" DefaultValue="" Type="String" />
            <asp:Parameter Name="status" DefaultValue="0" Type="Int32" />
            <asp:Parameter Name="userId" Type="Int32" />
            <asp:Parameter Name="rptView" Type="Int32" />
            <asp:Parameter Name="compo" DefaultValue="6" Type="String" />
            <asp:Parameter Name="maxCount" DefaultValue="0" Type="Int32" />
            <asp:Parameter Name="moduleId" DefaultValue="2" Type="Byte" />
            <asp:ControlParameter ControlID="FormalSelect" DefaultValue="" Name="IsFomal" PropertyName="SelectedValue"
                Type="String" />
            <asp:Parameter Name="unitId" DefaultValue="0" Type="Int32" />
            <asp:ControlParameter ControlID="SarcCheck" PropertyName="Checked" Name="sarcpermission"
                Type="Boolean" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentFooter" runat="Server">

    <script type="text/javascript">
    </script>

</asp:Content>
