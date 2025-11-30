<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Admin.Secure_Shared_Admin_MemoMappedValues" Codebehind="MemoMappedValues.aspx.vb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <asp:Label runat="server" ID="lblKey" Text="Key:"></asp:Label>
    <asp:DropDownList runat="server" ID="ddlMemoKeys" DataSourceID="memoKeys" DataValueField="ID" DataTextField="Description" AutoPostBack="True">
    </asp:DropDownList>
    <br />
    <br />
    <asp:GridView ID="gvValues" runat="server" AutoGenerateColumns="False" 
        DataSourceID="memoValues" DataKeyNames="Id">
        <Columns>
            <asp:TemplateField HeaderText="Id">
                <ItemTemplate>
                    <asp:Label ID="lblId" runat="server" CssClass="hidden" Text='<%# Bind("Id")%>'></asp:Label>
                    <asp:Label ID="Label5" runat="server" Text='<%# Eval("ValueId")%>'></asp:Label>
                </ItemTemplate>
                <ItemStyle Width="3%" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Description">
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("ValueDescription")%>' Width="133px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="TextBox2" ErrorMessage="*" ValidationGroup="edit">*</asp:RequiredFieldValidator>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label6" runat="server" Text='<%# Bind("ValueDescription")%>'></asp:Label>
                </ItemTemplate>
                <ItemStyle Width="18%" />
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Value">
                <EditItemTemplate>
                    <asp:TextBox ID="TextBox1" runat="server" Text='<%# Bind("Value") %>' Width="633px" TextMode="MultiLine"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="TextBox1" ErrorMessage="*" ValidationGroup="edit">*</asp:RequiredFieldValidator>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:Label ID="Label4" runat="server" Text='<%# Bind("Value") %>'></asp:Label>
                </ItemTemplate>
                <ItemStyle Width="79%" />
            </asp:TemplateField>
            <asp:TemplateField ShowHeader="False">
                <EditItemTemplate>
                    <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="True" CommandName="Update" ValidationGroup="edit" Text="Update"></asp:LinkButton>
                    <asp:LinkButton ID="LinkButton2" runat="server" CausesValidation="False" CommandName="Cancel" Text="Cancel"></asp:LinkButton>
                </EditItemTemplate>
                <ItemTemplate>
                    <asp:LinkButton ID="LinkButton1" runat="server" CausesValidation="False" CommandName="Edit" Text="Edit" ValidationGroup="edit"></asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
            <%--<asp:CommandField CausesValidation="False" InsertVisible="False" ShowEditButton="True" />--%>
        </Columns>
    </asp:GridView>
    <br />
    <br />
    <asp:ObjectDataSource ID="memoValues" runat="server" TypeName="ALOD.Data.KeyValDao" SelectMethod="GetKeyValuesByKeyId" UpdateMethod="UpdateKeyValueById">
        <SelectParameters>
            <asp:ControlParameter ControlID="ddlMemoKeys" Name="keyId" PropertyName="SelectedValue" Type="Int32" />
        </SelectParameters>
        <UpdateParameters>
            <asp:Parameter Name="id" Type="Int32" />
            <asp:ControlParameter ControlID="ddlMemoKeys" Name="keyId" PropertyName="SelectedValue" Type="Int32" />
            <asp:Parameter Name="valueDescription" Type="String" />
            <asp:Parameter Name="value" Type="String" />
        </UpdateParameters>
    </asp:ObjectDataSource>

    <asp:ObjectDataSource ID="memoKeys" runat="server" SelectMethod="GetMemoKeys" TypeName="ALOD.Data.KeyValDao">
    </asp:ObjectDataSource>
</asp:Content>
