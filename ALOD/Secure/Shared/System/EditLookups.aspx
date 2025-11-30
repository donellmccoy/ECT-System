<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Sys.Secure_Shared_System_EditLookups" Codebehind="EditLookups.aspx.vb" %>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" Runat="Server">
    <asp:UpdatePanel ID="TypeAndRatingUpdatePanel" runat="server">
        <ContentTemplate>
            <table>
                <tr>
                    <td class="style4"></td>
                    <td class="blueText1">Edit PEPP Type</td>
                    <td class="style3"></td>
                    <td class="blueText1">Edit PEPP Rating</td>
                </tr>
                <tr>
                    <td class="style4"></td>
                    <td>
                        <table>
                            <tr>
                                <td>
                                    <asp:GridView ID="PEPPTypeGridView" runat="server" Width="350px" 
                                        AutoGenerateColumns="False" DataSourceID="PEPPTypeDataSource" 
                                        DataKeyNames="Value" PageSize="10" EmptyDataText="No Record Found" 
                                        AllowPaging="True" EnableModelValidation="True">
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
                                    <asp:ObjectDataSource ID="PEPPTypeDataSource" runat="server" 
                                        OldValuesParameterFormatString="{0}" SelectMethod="GetRecords" 
                                        TypeName="ALODWebUtility.Common.PEPPType" UpdateMethod="UpdatePEPPType">
                                        <UpdateParameters>
                                            <asp:ControlParameter ControlID="PEPPTypeGridView" Name="Value" 
                                                PropertyName="SelectedValue" Type="Int32" />
                                            <asp:Parameter Name="Name" Type="String" />
                                            <asp:Parameter Name="Active" Type="Int32" />
                                        </UpdateParameters>
                                    </asp:ObjectDataSource>
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td class="style2">
                                    <table>
                                        <tr>
                                            <td>Name: </td>
                                            <td class="style1"><asp:TextBox ID="txtPEPPTypeName" runat="server" MaxLength="50"></asp:TextBox></td>
                                            <td class="style1"><asp:Button ID="btnAddPEPPType" runat="server" Text="Add" Width="60px" 
                                                    ValidationGroup="peppType" /></td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;</td>
                                            <td>
                                                <asp:RequiredFieldValidator ID="rfvPEPPType" runat="server" 
                                                    ControlToValidate="txtPEPPTypeName" Display="Dynamic" 
                                                    ErrorMessage="PEPP Type name is required." SetFocusOnError="True" 
                                                    ValidationGroup="peppType"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                    </table>       
                                </td>
                            </tr>
                        </table>
                    </td>

                    <td class="style3">
                    
                    </td>

                    <td>
                        <table>
                            <tr>
                                <td>
                                    <asp:GridView ID="PEPPRatingGridView" runat="server" Width="350px" 
                                        AutoGenerateColumns="False" DataSourceID="PEPPRatingDataSource" 
                                        DataKeyNames="Value" PageSize="10" EmptyDataText="No Record Found" 
                                        AllowPaging="True" EnableModelValidation="True">
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
                                    <asp:ObjectDataSource ID="PEPPRatingDataSource" runat="server"
                                        OldValuesParameterFormatString="{0}" SelectMethod="GetRecords" 
                                        TypeName="ALODWebUtility.Common.PEPPRating" UpdateMethod="UpdatePEPPRating">
                                        <UpdateParameters>
                                            <asp:ControlParameter ControlID="PEPPRatingGridView" Name="Value" 
                                                PropertyName="SelectedValue" Type="Int32" />
                                            <asp:Parameter Name="Name" Type="String" />
                                            <asp:Parameter Name="Active" Type="Int32" />
                                        </UpdateParameters>
                                    </asp:ObjectDataSource>
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td class="style2">
                                    <table>
                                        <tr>
                                            <td>Name: </td>
                                            <td class="style1"><asp:TextBox ID="txtPEPPRatingName" runat="server" MaxLength="50"></asp:TextBox></td>
                                            <td class="style1"><asp:Button ID="btnAddPEPPRating" runat="server" Text="Add" Width="60px" 
                                                    ValidationGroup="peppRating" /></td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;</td>
                                            <td>
                                                <asp:RequiredFieldValidator ID="rfvPEPPRating" runat="server" 
                                                    ControlToValidate="txtPEPPRatingName" Display="Dynamic" 
                                                    ErrorMessage="PEPP Rating name is required." SetFocusOnError="True" 
                                                    ValidationGroup="peppRating"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                    </table>       
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>

    <hr />

<%--    <asp:UpdatePanel ID="DispositionAndRMUUpdatePanel" runat="server">
        <ContentTemplate>
            <table>
                <tr>
                    <td class="style4"></td>
                    <td class="blueText1">Edit PEPP Disposition</td>
                    <td class="style3"></td>
                    <td class="blueText1">Edit RMU</td>
                </tr>
                <tr>
                    <td class="style4"></td>
                    <td>
                        <table>
                            <tr>
                                <td>
                                    <asp:GridView ID="PEPPDispositionGridView" runat="server" Width="350px" 
                                        AutoGenerateColumns="False" DataSourceID="PEPPDispositionDataSource" 
                                        DataKeyNames="Value" PageSize="10" EmptyDataText="No Record Found" 
                                        AllowPaging="True" EnableModelValidation="True">
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
                                    <asp:ObjectDataSource ID="PEPPDispositionDataSource" runat="server"
                                        OldValuesParameterFormatString="{0}" SelectMethod="GetRecords" 
                                        TypeName="ALODWebUtility.Common.PEPPDisposition" UpdateMethod="UpdatePEPPDisposition">
                                        <UpdateParameters>
                                            <asp:ControlParameter ControlID="PEPPDispositionGridView" Name="Value" 
                                                PropertyName="SelectedValue" Type="Int32" />
                                            <asp:Parameter Name="Name" Type="String" />
                                            <asp:Parameter Name="Active" Type="Int32" />
                                        </UpdateParameters>
                                    </asp:ObjectDataSource>
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td class="style2">
                                    <table>
                                        <tr>
                                            <td>Name: </td>
                                            <td class="style1"><asp:TextBox ID="txtPEPPDispositionName" runat="server" MaxLength="50"></asp:TextBox></td>
                                            <td class="style1"><asp:Button ID="btnAddPEPPDisposition" runat="server" Text="Add" Width="60px" 
                                                    ValidationGroup="peppDisposition" /></td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;</td>
                                            <td>
                                                <asp:RequiredFieldValidator ID="rfvPEPPDisposition" runat="server" 
                                                    ControlToValidate="txtPEPPDispositionName" Display="Dynamic" 
                                                    ErrorMessage="PEPP Disposition name is required." SetFocusOnError="True" 
                                                    ValidationGroup="peppDisposition"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                    </table>      
                                </td>
                            </tr>
                        </table>
                    </td>

                    <td class="style3">
                    
                    </td>

                    
                </tr>
            </table>
        </ContentTemplate>
    </asp:UpdatePanel>

    <hr />--%>

    <asp:UpdatePanel ID="MAJCOMUpdatePanel" runat="server">
        <ContentTemplate>
            <table>
                <tr>
                    <td class="style4"></td>
                    <td class="blueText1">Edit MAJCOM</td>
                    <td class="style3"></td>
                    <td class="blueText1">Edit RMU</td>
                </tr>
                <tr>
                    <td class="style4"></td>
                    <td style="vertical-align:top;">
                        <table>
                            <tr>
                                <td>
                                    <asp:GridView ID="MAJCOMGridView" runat="server" Width="350px" 
                                        AutoGenerateColumns="False" DataSourceID="MAJCOMDataSource" 
                                        DataKeyNames="Value" PageSize="10" EmptyDataText="No Record Found"
                                        AllowPaging="True">
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
                                    <asp:ObjectDataSource ID="MAJCOMDataSource" runat="server"
                                        OldValuesParameterFormatString="{0}" SelectMethod="GetMAJCOM" 
                                        TypeName="ALODWebUtility.Common.majcom" UpdateMethod="UpdateMAJCOM">
                                        <UpdateParameters>
                                            <asp:ControlParameter ControlID="MAJCOMGridView" Name="Value" 
                                                PropertyName="SelectedValue" Type="Int32" />
                                            <asp:Parameter Name="Name" Type="String" />
                                            <asp:Parameter Name="Active" Type="Int32" />
                                        </UpdateParameters>
                                    </asp:ObjectDataSource>
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td class="style2">
                                    <table>
                                        <tr>
                                            <td>Name: </td>
                                            <td class="style1"><asp:TextBox ID="txtMAJCOMName" runat="server" MaxLength="50"></asp:TextBox></td>
                                            <td class="style1"><asp:Button ID="btnAddMAJCOM" runat="server" Text="Add" Width="60px" 
                                                    ValidationGroup="majcom" /></td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;</td>
                                            <td>
                                                <asp:RequiredFieldValidator ID="rfvMAJCOM" runat="server" 
                                                    ControlToValidate="txtMAJCOMName" Display="Dynamic" 
                                                    ErrorMessage="MAJCOM name is required." SetFocusOnError="True" 
                                                    ValidationGroup="majcom"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                    </table>        
                                </td>
                            </tr>
                        </table>
                    </td>

                    <td class="style3">
                    </td>

                    <td>
                        <table>
                            <tr>
                                <td>
                                    <asp:GridView ID="RMUGridView" runat="server" Width="350px" 
                                        AutoGenerateColumns="False" DataSourceID="RMUDataSource" 
                                        DataKeyNames="Value" PageSize="10" EmptyDataText="No Record Found" 
                                        AllowPaging="True" EnableModelValidation="True">
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
                                            <asp:TemplateField HeaderText="PAS">
                                                <EditItemTemplate>
                                                    <asp:TextBox ID="txtPAS" runat="server" Text='<%# Bind("PAS")%>' Width="50px"></asp:TextBox>
                                                </EditItemTemplate>
                                                <ItemTemplate>
                                                    <asp:Label ID="lblPAS" runat="server" Text='<%# Bind("PAS")%>'></asp:Label>
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:CheckBoxField DataField="Collocated" HeaderText="Collocated" ItemStyle-HorizontalAlign="Center" />
                                            <asp:CommandField CausesValidation="False" InsertVisible="False" ShowEditButton="True" />
                                        </Columns>
                                    </asp:GridView>
                                    <asp:ObjectDataSource ID="RMUDataSource" runat="server"
                                        OldValuesParameterFormatString="{0}" SelectMethod="GetRecords" 
                                        TypeName="ALODWebUtility.Common.RMU" UpdateMethod="UpdateRMU">
                                        <UpdateParameters>
                                            <asp:ControlParameter ControlID="RMUGridView" Name="Value" PropertyName="SelectedValue" Type="Int32" />
                                            <asp:Parameter Name="Name" Type="String" />
                                            <asp:Parameter Name="PAS" Type="String" />
                                            <asp:Parameter Name="Collocated" Type="Int32" />
                                        </UpdateParameters>
                                    </asp:ObjectDataSource>
                                    <br />
                                </td>
                            </tr>
                            <tr>
                                <td valign="top" class="style2">
                                    <table>
                                        <tr>
                                            <td>Name: </td>
                                            <td class="style1"><asp:TextBox ID="txtRMUName" runat="server" MaxLength="50"></asp:TextBox></td>
                                            <td class="style1"><asp:Button ID="btnAddRMU" runat="server" Text="Add" Width="60px" ValidationGroup="rmu" /></td>
                                        </tr>
                                        <tr>
                                            <td>PAS Code: </td>
                                            <td class="style1"><asp:TextBox ID="txtRMUPAS" runat="server" MaxLength="4"></asp:TextBox></td>
                                        </tr>
                                        <tr>
                                            <td>
                                                Collocated:
                                            </td>
                                            <td class="style1">
                                                <asp:CheckBox runat="server" ID="chkCollocated" Checked="true" />
                                            </td>
                                        </tr>
                                        <tr>
                                            <td>&nbsp;</td>
                                            <td>
                                                <asp:RequiredFieldValidator ID="rfvRMU" runat="server" 
                                                    ControlToValidate="txtRMUName" Display="Dynamic" 
                                                    ErrorMessage="RMU name is required." SetFocusOnError="True" 
                                                    ValidationGroup="rmu"></asp:RequiredFieldValidator>
                                                <asp:RequiredFieldValidator ID="rfvRMUPAS" runat="server" 
                                                    ControlToValidate="txtRMUPAS" Display="Dynamic" 
                                                    ErrorMessage="RMU PAS Code is required." SetFocusOnError="True" 
                                                    ValidationGroup="rmu"></asp:RequiredFieldValidator>
                                            </td>
                                        </tr>
                                    </table>        
                                </td>
                            </tr>
                        </table>
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
        .style3
        {
            width:125px;
        }
        .style4
        {
            width:55px;
        }
        .blueText1
        {
            margin: 0 0 .5em 0;
            padding: 5px;
            color: #08003C;
            text-indent: 0px;
            font-size: 14px;
            font-weight:bold;
            background-color: transparent;   
        }
    </style>
</asp:Content>



