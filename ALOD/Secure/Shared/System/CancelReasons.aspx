<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Sys.CancelReasons" CodeBehind="CancelReasons.aspx.vb" %>

<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="head">
</asp:Content>


<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <div class="indent-small">
        <div class="dataBlock">
            <div class="dataBlock-header">
                1 - Disposition Options
            </div>

            <div class="dataBlock-body">
                <asp:GridView runat="server" ID="gdvCancelReasons" AutoGenerateColumns="False" Width="100%" DataKeyNames="Id">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" ItemStyle-Width="5%" />
                        <asp:TemplateField HeaderText="Description">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDescription" runat="server" MaxLength="50" Width="250px" Text='<%# Bind("Description")%>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblDescription" runat="server" Text='<%# Bind("Description")%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="70%" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Display Order">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDisplay" runat="server" MaxLength="50" Width="250px" Text='<%# Bind("DisplayOrder")%>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblDisplay" runat="server" Text='<%# Bind("DisplayOrder")%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="70%" />
                        </asp:TemplateField>
                        <asp:CommandField CausesValidation="False" InsertVisible="False" ShowEditButton="True" ItemStyle-Width="15%" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>
    </div>
</asp:Content>
