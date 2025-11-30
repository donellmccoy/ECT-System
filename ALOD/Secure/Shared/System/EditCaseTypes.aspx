<%@ Page Title="" Language="VB" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Sys.EditCaseTypes" Codebehind="EditCaseTypes.aspx.vb" %>

<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="head">
</asp:Content>

<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="ContentFooter">
    <script type="text/javascript">
        $(function() {
            var block = element('<%=pnlWorkflowMaps.ClientID%>');
            if (block !== null) {
                block.scrollIntoView(true);
            }

            block = element('<%=pnlSubCaseTypeMaps.ClientID%>');
            if (block !== null) {
                block.scrollIntoView(true);
            }
        });
    </script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <div class="indent-small">
        <div class="dataBlock">
            <div class="dataBlock-header">
                1 - Case Type Options
            </div>

            <div class="dataBlock-body">
                <asp:GridView runat="server" ID="gdvCaseTypes" AutoGenerateColumns="False" Width="100%" DataKeyNames="Id">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" ItemStyle-Width="5%" />
                        <asp:TemplateField HeaderText="Name">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtName" runat="server" MaxLength="50" Width="250px" Text='<%# Bind("Name") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblName" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="65%" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="lbtnEditWorkflowMaps" CommandName="EditWorkflowMaps" Text="Workflows" CommandArgument='<%# Eval("Id").ToString() + "|" + Container.DataItemIndex.ToString() %>' />
                                <asp:LinkButton runat="server" ID="lbtnEditSubCaseTypeMaps" CommandName="EditSubCaseTypeMaps" Text="Sub Types" CommandArgument='<%# Eval("Id").ToString() + "|" + Container.DataItemIndex.ToString() %>' />
                            </ItemTemplate>
                            <ItemStyle Width="15%" />
                        </asp:TemplateField>
                        <asp:CommandField CausesValidation="False" InsertVisible="False" ShowEditButton="True" ItemStyle-Width="15%" />
                    </Columns>
                </asp:GridView>
                <br />
                <asp:Label runat="server" ID="lblAddCaseType" Text="Add New Case Type:" />
                &nbsp;
                <asp:TextBox runat="server" ID="txtAddCaseType" MaxLength="50" Width="250px" />
                &nbsp;
                <asp:Button runat="server" ID="btnAddCaseType" Text="Add" />
            </div>
        </div>

        <asp:Panel runat ="server" ID="pnlSubCaseTypes" CssClass="dataBlock">
            <div class="dataBlock-header">
                2 - Sub Case Type Options
            </div>

            <div class="dataBlock-body">
                <asp:GridView runat="server" ID="gdvSubCaseTypes" AutoGenerateColumns="False" Width="100%" DataKeyNames="Id">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" ItemStyle-Width="5%" />
                        <asp:TemplateField HeaderText="Name">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtSubName" runat="server" MaxLength="50" Width="250px" Text='<%# Bind("Name") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblSubName" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="80%" />
                        </asp:TemplateField>
                        <asp:CommandField CausesValidation="False" InsertVisible="False" ShowEditButton="True" ItemStyle-Width="15%" />
                    </Columns>
                </asp:GridView>
                <br />
                <asp:Label runat="server" ID="lblAddSubCaseType" Text="Add New Sub Case Type:" />
                &nbsp;
                <asp:TextBox runat="server" ID="txtAddSubCaseType" MaxLength="50" Width="250px" />
                &nbsp;
                <asp:Button runat="server" ID="btnAddSubCaseType" Text="Add" />
            </div>
        </asp:Panel>

        <asp:Panel runat ="server" ID="pnlWorkflowMaps" Visible="false" CssClass="dataBlock">
            <div class="dataBlock-header">
                3 - Case Type Option to Workflows Mapping
            </div>

            <div class="dataBlock-body">
                <asp:GridView runat="server" ID="gdvCaseTypeWorkflows" AutoGenerateColumns="False" Width="100%">
                    <Columns>
                        <asp:TemplateField Visible="false">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="txtWorkflowId" Text='<%# Bind("WorkflowId")%>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Workflow">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="txtWorkflowTitle" Text='<%# Eval("WorkflowTitle") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Is Mapped To">
                            <ItemTemplate>
                                <asp:CheckBox runat="server" ID="chkWorkflowAssociated" Text="" Checked='<%# Bind("IsAssociated")%>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <br />
                <asp:Button runat="server" ID="btnUpdateWorkflowMappings" Text="Save" Width="60px" />&nbsp;
                <asp:Button runat="server" ID="btnCancelWorkflowMappings" Text="Cancel" Width="60px" />
            </div>
        </asp:Panel>

        <asp:Panel runat ="server" ID="pnlSubCaseTypeMaps" Visible="false" CssClass="dataBlock">
            <div class="dataBlock-header">
                3 - Case Type Option to Sub Case Types Mapping
            </div>

            <div class="dataBlock-body">
                <asp:GridView runat="server" ID="gdvCaseTypeSubCaseTypes" AutoGenerateColumns="False" Width="100%">
                    <Columns>
                        <asp:TemplateField Visible="false">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="txtSubCaseTypeId" Text='<%# Bind("WorkflowId")%>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Workflow">
                            <ItemTemplate>
                                <asp:Label runat="server" ID="txtSubCaseTypeTitle" Text='<%# Eval("WorkflowTitle")%>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Is Mapped To">
                            <ItemTemplate>
                                <asp:CheckBox runat="server" ID="chkSubCaseTypeAssociated" Text="" Checked='<%# Bind("IsAssociated")%>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <br />
                <asp:Button runat="server" ID="btnUpdateSubCaseTypeMappings" Text="Save" Width="60px" />&nbsp;
                <asp:Button runat="server" ID="btnCancelSubCaseTypeMappings" Text="Cancel" Width="60px" />
            </div>
        </asp:Panel>
    </div>
</asp:Content>