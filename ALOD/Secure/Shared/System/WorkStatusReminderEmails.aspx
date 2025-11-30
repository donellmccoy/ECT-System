<%@ Page Language="VB" MasterPageFile="~/Secure/Popup.master" AutoEventWireup="false" Inherits="ALOD.Web.Sys.Secure_Shared_System_WorkStatusReminderEmails"
    Title="Action Editor" Codebehind="WorkStatusReminderEmails.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="Server">

    <div align="right">
        <input type="button" value="Return to Options" onclick="window.opener.location.reload(); window.parent.close(); return false;" />
    </div>
    <div align="left">
        <table>
            <tr>
                <td>
                    Compo:
                </td>
                <td align="left">
                    <asp:Label ID="lblCompo" CssClass="labelNormal" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    Workflow:
                </td>
                <td align="left">
                    <asp:Label ID="lblWorkflow" CssClass="labelNormal" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    Step:
                </td>
                <td align="left">
                    <asp:Label ID="lblStep" CssClass="labelNormal" runat="server" Text=""></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    Option:
                </td>
                <td align="left">
                    <asp:Label CssClass="labelNormal" runat="server" ID="lblOption"> </asp:Label>
                </td>
            </tr>
        </table>
    </div>
    <br />

        <h3>
        Current Reminder Emails</h3>
    <asp:GridView ID="gvReminders" Width="840px" runat="server" AutoGenerateColumns="False">
        <Columns>
            <asp:TemplateField HeaderText="Name">
                <ItemTemplate>
                    <asp:Label runat="server" Text="Send Email"></asp:Label>
                </ItemTemplate>
                <ItemStyle Width="200px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Target">
                <ItemTemplate>
                    <%# Eval("groupName")%>
                </ItemTemplate>
                <ItemStyle Width="200px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Template">
                <ItemTemplate>
                    <%# Eval("templateName")%>
                </ItemTemplate>
                <ItemStyle Width="200px" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Interval Time">
                <ItemTemplate>
                    <%# Eval("intervalTime")%>
                </ItemTemplate>
                <ItemStyle Width="200px" />
            </asp:TemplateField>
            <asp:TemplateField ShowHeader="False">
                <ItemTemplate>
                    <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Delete"
                        CommandArgument='<%#Eval("id") %>' Text="Delete"></asp:LinkButton>
                    <ajax:ConfirmButtonExtender ID="ConfirmButtonExtender1" runat="server" ConfirmText="Are you sure you want to delete this action?"
                        TargetControlID="LinkButton1">
                    </ajax:ConfirmButtonExtender>
                </ItemTemplate>
                <ItemStyle Width="200px" HorizontalAlign="Right" />
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <br />
    <br />
    <h3> Add new action:</h3>

    <div runat="server" id="divAdd" >
        <table cellpadding="2" >
            <tr>
                <td width="100px">
             
                </td>
                <td align="left" width="220px">
                    <asp:DropDownList ID="cbType" Width="100%" runat="server" AutoPostBack="True" DataTextField="text"
                        DataValueField="value" DataSourceID="ActionTypesData">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td width="100px">
                    Target:
                </td>
                <td align="left" width="220px">
                    <asp:DropDownList ID="cbTarget" Width="100%" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td width="100px">
                    Template:
                </td>
                <td align="left" width="220px">
                    <asp:DropDownList ID="cbData" Width="100%" runat="server" DataTextField="Text" DataValueField="Value">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td width="100px">
                    Interval Time(days):
                </td>
                <td align="left" width="220px">
                    <asp:TextBox ID="intervalTime" runat="server" Text="" Width="215px"></asp:TextBox>
                </td>
                <td>
                    <asp:Label ID="intervalError" runat="server" Text="Interval Time must be a number between 1 and 999." style="color:Red;" Visible="false"></asp:Label>
                </td>
            </tr>
        </table>
        <br />
        <table>
            <tr>
                <td align="right" width="400px">
                    <asp:Button ID="btnAddAction" Width="60px" runat="server" Text="Add" />
                </td>
            </tr>
        </table>
    </div>

    <asp:ObjectDataSource runat="server" ID="ActionTypesData" TypeName="ALODWebUtility.LookUps.LookUp" OldValuesParameterFormatString="original_{0}"
        SelectMethod="GetWorkflowActionTypes"></asp:ObjectDataSource>
    <asp:ObjectDataSource runat="server" ID="WorkStatusData" TypeName="ALODWebUtility.Worklfow.WorkStatusList"
        OldValuesParameterFormatString="original_{0}" SelectMethod="GetByWorklfow">
        <SelectParameters>
            <asp:QueryStringParameter Name="workflow" QueryStringField="workflow" Type="Int32" />
        </SelectParameters>
    </asp:ObjectDataSource>
</asp:Content>
