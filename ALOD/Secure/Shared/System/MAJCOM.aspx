<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Sys.Secure_Shared_System_MAJCOM" Codebehind="MAJCOM.aspx.vb" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" Runat="Server">
    <h2>Add/Edit MAJCOM</h2>

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
        
            <table>
            <tr>
                <td class="style2">
                <table>
                <tr>
                    <td>Name: </td>
                    <td class="style1"><asp:TextBox ID="txtName" runat="server" MaxLength="20"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" 
                            ControlToValidate="txtName" Display="Dynamic" 
                            ErrorMessage="MAJCOM name is required." SetFocusOnError="True" 
                            ValidationGroup="add"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td class="style1"><asp:Button ID="btnAdd" runat="server" Text="Add" Width="60px" 
                            ValidationGroup="add" /></td>
                </tr>
                </table>       
                </td>

                <td>
                    <asp:GridView ID="GridView1" runat="server" Width="350px" 
                        AutoGenerateColumns="False" DataSourceID="ObjectDataSource1" 
                        DataKeyNames="Value" PageSize="20" EmptyDataText="No Record Found">
                        <Columns>
                            <asp:BoundField DataField="Value" HeaderText="Id" ReadOnly="True" />
                            <asp:TemplateField HeaderText="Name">
                                <EditItemTemplate>
                                    <asp:TextBox ID="txtName" runat="server" Text='<%# Bind("Name") %>'></asp:TextBox>
                                </EditItemTemplate>
                                <ItemTemplate>
                                    <asp:Label ID="lblName" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:CheckBoxField DataField="Active" HeaderText="Active" />
                            <asp:CommandField CausesValidation="False" InsertVisible="False" 
                                ShowEditButton="True" />
                        </Columns>
                    </asp:GridView>
                    <asp:ObjectDataSource ID="ObjectDataSource1" runat="server"
                        OldValuesParameterFormatString="{0}" SelectMethod="GetMAJCOM" 
                        TypeName="ALODWebUtility.Common.majcom" UpdateMethod="UpdateMAJCOM">
                        <UpdateParameters>
                            <asp:ControlParameter ControlID="GridView1" Name="Value" 
                                PropertyName="SelectedValue" Type="Int32" />
                            <asp:Parameter Name="Name" Type="String" />
                            <asp:Parameter Name="Active" Type="Int32" />
                        </UpdateParameters>
                    </asp:ObjectDataSource>
                    <br />
                </td>
            </tr>   
            </table>


        </ContentTemplate>  
    </asp:UpdatePanel>
        </asp:Content>

<asp:Content ID="Content2" runat="server" contentplaceholderid="head">
    <style type="text/css">
        .style1
        {
            width: 150px;
            padding-right:10px;
        }
        .style2
        {
            width: 189px;
        }
    </style>
</asp:Content>



