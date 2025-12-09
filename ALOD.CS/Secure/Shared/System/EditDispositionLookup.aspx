<%@ Page Title="" Language="C#" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Sys.Secure_Shared_System_EditDispositionLookup" CodeBehind="EditDispositionLookup.aspx.cs" %>

<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="head">
</asp:Content>

<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="ContentFooter">
    <script type="text/javascript">
        (function() {
            var block = element('<%=pnlWorkflowMaps.ClientID%>');
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
                1 - Disposition Options
            </div>

            <div class="dataBlock-body">
                <asp:GridView runat="server" ID="gdvDispositions" AutoGenerateColumns="False" Width="100%" DataKeyNames="Id">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" ItemStyle-Width="5%" />
                        <asp:TemplateField HeaderText="Name">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtName" runat="server" MaxLength="50" Width="250px" Text='<%# Bind("Name") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblName" runat="server" Text='<%# Bind("Name") %>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="70%" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="lbtnEditWorkflowMaps" CommandName="EditWorkflowMaps" Text="Workflows" CommandArgument='<%# Eval("Id").ToString() + "|" + Container.DataItemIndex.ToString() %>' />
                            </ItemTemplate>
                            <ItemStyle Width="10%" />
                        </asp:TemplateField>
                        <asp:CommandField CausesValidation="False" InsertVisible="False" ShowEditButton="True" ItemStyle-Width="15%" />
                    </Columns>
                </asp:GridView>
                <br />
                <asp:Label runat="server" ID="lblAddDisposition" Text="Add New Disposition:" />
                &nbsp;
                <asp:TextBox runat="server" ID="txtAddDisposition" MaxLength="50" Width="250px" />
                &nbsp;
                <asp:Button runat="server" ID="btnAddDisposition" Text="Add" />
            </div>
        </div>

        <asp:Panel runat ="server" ID="pnlWorkflowMaps" Visible="false" CssClass="dataBlock">
            <div class="dataBlock-header">
                2 - Disposition Option to Workflows Mapping
            </div>

            <div class="dataBlock-body">
                <asp:GridView runat="server" ID="gdvDispositionWorkflows" AutoGenerateColumns="False" Width="100%">
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
                                <asp:CheckBox runat="server" ID="chkAssociated" Text="" Checked='<%# Bind("IsAssociated")%>' />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
                <br />
                <asp:Button runat="server" ID="btnUpdateWorkflowMappings" Text="Save" Width="60px" />&nbsp;
                <asp:Button runat="server" ID="btnCancelWorkflowMappings" Text="Cancel" Width="60px" />
            </div>
        </asp:Panel>
    </div>
</asp:Content>
