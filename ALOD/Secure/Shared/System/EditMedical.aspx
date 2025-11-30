<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Sys.EditMedical" Codebehind="EditMedical.aspx.vb" MaintainScrollPositionOnPostback="true"%>

<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="head">
</asp:Content>

<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="ContentFooter">
    <script type="text/javascript">
    </script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">

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

    <asp:Panel runat ="server" ID="pnlMedicalComponent" CssClass="dataBlock">
            <div class="dataBlock-header">
                1 - Member Component
            </div>

            <div class="dataBlock-body">
                <asp:GridView runat="server" ID="gdvMedicalComponent" AutoGenerateColumns="False" Width="100%" DataKeyNames="Id">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" ItemStyle-Width="5%" />
                        <asp:TemplateField HeaderText="Sort Order">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtComponentsort" runat="server" MaxLength="20" Width="50px" Text='<%# Bind("sort_order")%>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblComponentsort" runat="server" Text='<%# Bind("sort_order")%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="20%" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Name">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtComponent" runat="server" MaxLength="50" Width="250px" Text='<%# Bind("description")%>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblComponent" runat="server" Text='<%# Bind("description")%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="80%" />
                        </asp:TemplateField>
                        
                        <asp:CommandField CausesValidation="False" InsertVisible="False" ShowEditButton="True" ItemStyle-Width="15%" />
                    </Columns>
                </asp:GridView>
                <br />
                <asp:Label runat="server" ID="lblAddComponent" Text="Add New Component description:" />
                &nbsp;
                <asp:TextBox runat="server" ID="txtAddComponent" MaxLength="50" Width="250px" />
                <br />
                <br />
                <asp:Label runat="server" ID="lblSortOrderComponent" Text="Add Component sort order:" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:TextBox runat="server" ID="txtSortOrderComponent" MaxLength="50" Width="250px" />

                <br />
                <br />

                <asp:Button runat="server" ID="btnAddComponent" Text="Add" />
            </div>
        </asp:Panel>


    <asp:Panel runat ="server" ID="pnlMedicalCategory" CssClass="dataBlock">
            <div class="dataBlock-header">
                2 - Member Category
            </div>

            <div class="dataBlock-body">
                <asp:GridView runat="server" ID="gdvMedicalCategory" AutoGenerateColumns="False" Width="100%" DataKeyNames="Id">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" ItemStyle-Width="5%" />
                        <asp:TemplateField HeaderText="Sort Order">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCategorysort" runat="server" MaxLength="20" Width="50px" Text='<%# Bind("sort_order")%>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblCategorysort" runat="server" Text='<%# Bind("sort_order")%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="20%" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Name">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtCategory" runat="server" MaxLength="50" Width="250px" Text='<%# Bind("description")%>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblCategory" runat="server" Text='<%# Bind("description")%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="80%" />
                        </asp:TemplateField>
                        
                        <asp:CommandField CausesValidation="False" InsertVisible="False" ShowEditButton="True" ItemStyle-Width="15%" />
                    </Columns>
                </asp:GridView>
                <br />
                <asp:Label runat="server" ID="lblAddCategory" Text="Add New Categoy description:" />
                &nbsp;
                <asp:TextBox runat="server" ID="txtAddCategory" MaxLength="50" Width="250px" />
                <br />
                <br />
                <asp:Label runat="server" ID="lblSortOrderCategory" Text="Add Category sort order:" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:TextBox runat="server" ID="txtSortOrderCategory" MaxLength="50" Width="250px" />

                <br />
                <br />

                <asp:Button runat="server" ID="btnAddCategory" Text="Add" />
            </div>
        </asp:Panel>

    <asp:Panel runat ="server" ID="pnlMedicalStatus" CssClass="dataBlock">
            <div class="dataBlock-header">
                3 - Member Status
            </div>

            <div class="dataBlock-body">
                <asp:GridView runat="server" ID="gdvMedicalStatus" AutoGenerateColumns="False" Width="100%" DataKeyNames="Id">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" ItemStyle-Width="5%" />

                        <asp:TemplateField HeaderText="Sort Order">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtStatussort" runat="server" MaxLength="20" Width="50px" Text='<%# Bind("sort_order")%>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblStatussort" runat="server" Text='<%# Bind("sort_order")%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="20%" />
                        </asp:TemplateField>

                        <asp:BoundField DataField="type" HeaderText="Type" ReadOnly="True" ItemStyle-Width="25%" />

                        <asp:TemplateField HeaderText="Name">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtStatus" runat="server" MaxLength="50" Width="250px" Text='<%# Bind("description")%>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblStatus" runat="server" Text='<%# Bind("description")%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="80%" />
                        </asp:TemplateField>
                        
                        <asp:CommandField CausesValidation="False" InsertVisible="False" ShowEditButton="True" ItemStyle-Width="15%" />
                    </Columns>
                </asp:GridView>
                <br />
                <asp:Label runat="server" ID="lblAddStatus" Text="Add New Status description:" />
                &nbsp;
                <asp:TextBox runat="server" ID="txtAddStatus" MaxLength="50" Width="250px" />
                <br />
                <br />
                <asp:Label runat="server" ID="lblSortOrderStatus" Text="Add Status sort order:" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:TextBox runat="server" ID="txtSortOrderStatus" MaxLength="50" Width="250px" />
                <br />
                <br />
                <asp:Label runat="server" ID="lblTypeStatus" Text="Add Status type:" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:TextBox runat="server" ID="txtTypeStatus" MaxLength="50" Width="250px" />

                <br />
                <br />

                <asp:Button runat="server" ID="btnAddStatus" Text="Add" />
            </div>
        </asp:Panel>

    <asp:Panel runat ="server" ID="pnlMedicalFrom" CssClass="dataBlock">
            <div class="dataBlock-header">
                4 - Member From
            </div>

            <div class="dataBlock-body">
                <asp:GridView runat="server" ID="gdvMedicalFrom" AutoGenerateColumns="False" Width="100%" DataKeyNames="Id">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" ItemStyle-Width="5%" />
                        <asp:TemplateField HeaderText="Sort Order">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtFromsort" runat="server" MaxLength="20" Width="50px" Text='<%# Bind("sort_order")%>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblFromsort" runat="server" Text='<%# Bind("sort_order")%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="20%" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Name">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtFrom" runat="server" MaxLength="50" Width="250px" Text='<%# Bind("description")%>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblFrom" runat="server" Text='<%# Bind("description")%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="80%" />
                        </asp:TemplateField>
                        
                        <asp:CommandField CausesValidation="False" InsertVisible="False" ShowEditButton="True" ItemStyle-Width="15%" />
                    </Columns>
                </asp:GridView>
                <br />
                <asp:Label runat="server" ID="lblAddFrom" Text="Add New From Location description:" />
                &nbsp;
                <asp:TextBox runat="server" ID="txtAddFrom" MaxLength="50" Width="250px" />
                <br />
                <br />
                <asp:Label runat="server" ID="lblSortOrderFrom" Text="Add From Location sort order:" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:TextBox runat="server" ID="txtSortOrderFrom" MaxLength="50" Width="250px" />

                <br />
                <br />

                <asp:Button runat="server" ID="btnAddFrom" Text="Add" />
            </div>
        </asp:Panel>


    <asp:Panel runat ="server" ID="pnlMedicalFacility" CssClass="dataBlock">
            <div class="dataBlock-header">
                5 - Member Medical Facility
            </div>

            <div class="dataBlock-body">
                <asp:GridView runat="server" ID="gdvMedicalFacility" AutoGenerateColumns="False" Width="100%" DataKeyNames="Id">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" ItemStyle-Width="5%" />
                        <asp:TemplateField HeaderText="Sort Order">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtFacilitysort" runat="server" MaxLength="20" Width="50px" Text='<%# Bind("sort_order")%>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblFacilitysort" runat="server" Text='<%# Bind("sort_order")%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="20%" />
                        </asp:TemplateField>

                        
                        <asp:BoundField DataField="type" HeaderText="Type" ReadOnly="True" ItemStyle-Width="25%" />

                        <asp:TemplateField HeaderText="Name">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtFacility" runat="server" MaxLength="50" Width="250px" Text='<%# Bind("description")%>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblFacility" runat="server" Text='<%# Bind("description")%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="80%" />
                        </asp:TemplateField>
                        
                        <asp:CommandField CausesValidation="False" InsertVisible="False" ShowEditButton="True" ItemStyle-Width="15%" />
                    </Columns>
                </asp:GridView>
                <br />
                <asp:Label runat="server" ID="lblAddFacility" Text="Add New Facility description:" />
                &nbsp;
                <asp:TextBox runat="server" ID="txtAddFacility" MaxLength="50" Width="250px" />
                <br />
                <br />
                <asp:Label runat="server" ID="lblSortOrderFacility" Text="Add Facility sort order:" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:TextBox runat="server" ID="txtSortOrderFacility" MaxLength="50" Width="250px" />
                <br />
                <br />
                <asp:Label runat="server" ID="lblTypeFacility" Text="Add Status type:" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:TextBox runat="server" ID="txtTypeFacility" MaxLength="50" Width="250px" />

                <br />
                <br />

                <asp:Button runat="server" ID="btnAddFacility" Text="Add" />
            </div>
        </asp:Panel>

    <asp:Panel runat ="server" ID="pnlMedicalInfluence" CssClass="dataBlock">
            <div class="dataBlock-header">
                6 - Member Influence
            </div>

            <div class="dataBlock-body">
                <asp:GridView runat="server" ID="gdvMedicalInfluence" AutoGenerateColumns="False" Width="100%" DataKeyNames="Id">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" ItemStyle-Width="5%" />
                        <asp:TemplateField HeaderText="Sort Order">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtInfluencesort" runat="server" MaxLength="20" Width="50px" Text='<%# Bind("sort_order")%>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblInfluencesort" runat="server" Text='<%# Bind("sort_order")%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="20%" />
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Name">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtInfluence" runat="server" MaxLength="50" Width="250px" Text='<%# Bind("description")%>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblInfluence" runat="server" Text='<%# Bind("description")%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="80%" />
                        </asp:TemplateField>
                        
                        <asp:CommandField CausesValidation="False" InsertVisible="False" ShowEditButton="True" ItemStyle-Width="15%" />
                    </Columns>
                </asp:GridView>
                <br />
                <asp:Label runat="server" ID="lblAddInfluence" Text="Add New Influence description:" />
                &nbsp;
                <asp:TextBox runat="server" ID="txtAddInfluence" MaxLength="50" Width="250px" />
                <br />
                <br />
                <asp:Label runat="server" ID="lblSortOrderInfluence" Text="Add Influence sort order:" />
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:TextBox runat="server" ID="txtSortOrderInfluence" MaxLength="50" Width="250px" />

                <br />
                <br />

                <asp:Button runat="server" ID="btnAddInfluence" Text="Add" />
            </div>
        </asp:Panel>


    
</asp:Content>

