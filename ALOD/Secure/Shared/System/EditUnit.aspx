<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Sys.EditUnit" Codebehind="EditUnit.aspx.vb" MaintainScrollPositionOnPostback="true"%>

<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="head">
</asp:Content>

<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="ContentFooter">
    <script type="text/javascript">
    </script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">

    <asp:Panel runat ="server" ID="pnlUnitDuty" CssClass="dataBlock">
            <div class="dataBlock-header">
                1 - Member Duty Status
            </div>

            <div class="dataBlock-body">
                <asp:GridView runat="server" ID="gdvUnitDuty" AutoGenerateColumns="False" Width="100%" DataKeyNames="Id">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" ItemStyle-Width="5%" />
                        <asp:TemplateField HeaderText="Sort Order">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDutysort" runat="server" MaxLength="20" Width="50px" Text='<%# Bind("sort_order")%>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblDutySort" runat="server" Text='<%# Bind("sort_order")%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="20%" />
                        </asp:TemplateField>

                        
                        <asp:BoundField DataField="type" HeaderText="Type" ReadOnly="True" ItemStyle-Width="25%" />

                        <asp:TemplateField HeaderText="Name">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtDuty" runat="server" MaxLength="50" Width="250px" Text='<%# Bind("description")%>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblDuty" runat="server" Text='<%# Bind("description")%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="80%" />
                        </asp:TemplateField>
                        
                        <asp:CommandField CausesValidation="False" InsertVisible="False" ShowEditButton="True" ItemStyle-Width="15%" />
                    </Columns>
                </asp:GridView>
                <br />
                <asp:Label runat="server" ID="lblAddDuty" Text="Add New Duty Status description:" />
                &nbsp;
                <asp:TextBox runat="server" ID="txtAddDuty" MaxLength="50" Width="250px" />
                <br />
                <br />
                <asp:Label runat="server" ID="lblSortOrderDuty" Text="Add Duty Status sort order:" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:TextBox runat="server" ID="txtSortOrderDuty" MaxLength="50" Width="250px" />
                <br />
                <br />
                <asp:Label runat="server" ID="lblTypeDuty" Text="Add Duty Status type:" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:TextBox runat="server" ID="txtTypeDuty" MaxLength="50" Width="250px" />

                <br />
                <br />

                <asp:Button runat="server" ID="btnAddDuty" Text="Add" />
            </div>
        </asp:Panel>

    <asp:Panel runat ="server" ID="pnlUnitSource" CssClass="dataBlock">
            <div class="dataBlock-header">
                2 - Unit Information Source
            </div>

            <div class="dataBlock-body">
                <asp:GridView runat="server" ID="gdvUnitSource" AutoGenerateColumns="False" Width="100%" DataKeyNames="Id">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" ItemStyle-Width="5%" />
                        <asp:TemplateField HeaderText="Sort Order">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtSortSource" runat="server" MaxLength="20" Width="50px" Text='<%# Bind("sort_order")%>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblSortSource" runat="server" Text='<%# Bind("sort_order")%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="20%" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Name">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtSource" runat="server" MaxLength="50" Width="250px" Text='<%# Bind("description")%>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblSource" runat="server" Text='<%# Bind("description")%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="80%" />
                        </asp:TemplateField>
                        
                        <asp:CommandField CausesValidation="False" InsertVisible="False" ShowEditButton="True" ItemStyle-Width="15%" />
                    </Columns>
                </asp:GridView>
                <br />
                <asp:Label runat="server" ID="lblAddSource" Text="Add New Source description:" />
                &nbsp;
                <asp:TextBox runat="server" ID="txtAddSource" MaxLength="50" Width="250px" />
                <br />
                <br />
                <asp:Label runat="server" ID="lblSortOrderSource" Text="Add Info Source sort order:" />
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:TextBox runat="server" ID="txtSortOrderSource" MaxLength="50" Width="250px" />

                <br />
                <br />

                <asp:Button runat="server" ID="btnAddSource" Text="Add" />
            </div>
        </asp:Panel>

    <asp:Panel runat ="server" ID="pnlUnitOccurrence" CssClass="dataBlock">
            <div class="dataBlock-header">
                3 - Unit Occurrence
            </div>

            <div class="dataBlock-body">
                <asp:GridView runat="server" ID="gdvUnitOccurrence" AutoGenerateColumns="False" Width="100%" DataKeyNames="Id">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" ItemStyle-Width="5%" />
                        <asp:TemplateField HeaderText="Sort Order">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtSortOccurrence" runat="server" MaxLength="20" Width="50px" Text='<%# Bind("sort_order")%>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblSortOccurrence" runat="server" Text='<%# Bind("sort_order")%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="20%" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Name">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtOccurrence" runat="server" MaxLength="50" Width="250px" Text='<%# Bind("description")%>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblOccurrence" runat="server" Text='<%# Bind("description")%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="80%" />
                        </asp:TemplateField>
                        
                        <asp:CommandField CausesValidation="False" InsertVisible="False" ShowEditButton="True" ItemStyle-Width="15%" />
                    </Columns>
                </asp:GridView>
                <br />
                <asp:Label runat="server" ID="lblAddOccurrence" Text="Add New Occurrence description:" />
                &nbsp;
                <asp:TextBox runat="server" ID="txtAddOccurrence" MaxLength="50" Width="250px" />
                <br />
                <br />
                <asp:Label runat="server" ID="lblSortOrderOccurrence" Text="Add Occurrence sort order:" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:TextBox runat="server" ID="txtSortOrderOccurrence" MaxLength="50" Width="250px" />

                <br />
                <br />

                <asp:Button runat="server" ID="btnAddOccurrence" Text="Add" />
            </div>
        </asp:Panel>

    <asp:Panel runat ="server" ID="pnlUnitProximate" CssClass="dataBlock">
            <div class="dataBlock-header">
                4 - Unit Proximate cause
            </div>

            <div class="dataBlock-body">
                <asp:GridView runat="server" ID="gdvUnitProximate" AutoGenerateColumns="False" Width="100%" DataKeyNames="Id">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" ItemStyle-Width="5%" />
                        <asp:TemplateField HeaderText="Sort Order">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtSortProximate" runat="server" MaxLength="20" Width="50px" Text='<%# Bind("sort_order")%>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblSortProximate" runat="server" Text='<%# Bind("sort_order")%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="20%" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Name">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtProximate" runat="server" MaxLength="50" Width="250px" Text='<%# Bind("description")%>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblProximate" runat="server" Text='<%# Bind("description")%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="80%" />
                        </asp:TemplateField>
                        
                        <asp:CommandField CausesValidation="False" InsertVisible="False" ShowEditButton="True" ItemStyle-Width="15%" />
                    </Columns>
                </asp:GridView>
                <br />
                <asp:Label runat="server" ID="lblAddProximate" Text="Add New Proximate Cause description:" />
                &nbsp;
                <asp:TextBox runat="server" ID="txtAddProximate" MaxLength="50" Width="250px" />
                <br />
                <br />
                <asp:Label runat="server" ID="lblSortOrderProximate" Text="Add Proximate Cause sort order:" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:TextBox runat="server" ID="txtSortOrderProximate" MaxLength="50" Width="250px" />

                <br />
                <br />

                <asp:Button runat="server" ID="btnAddProximate" Text="Add" />
            </div>
        </asp:Panel>
</asp:Content>