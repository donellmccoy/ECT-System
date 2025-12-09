<%@ Page Title="" Language="C#" MasterPageFile="~/Secure/Secure.master" AutoEventWireup="false" Inherits="ALOD.Web.Sys.Secure_Shared_System_EditCertificationStamps" CodeBehind="EditCertificationStamps.aspx.cs" %>

<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="head">
</asp:Content>

<asp:Content runat="server" ID="Content3" ContentPlaceHolderID="ContentFooter">
    <script type="text/javascript">
       (function() {
            var block = element('<%=pnlWorkflowMaps.ClientID%>');
            if (block !== null) {
                block.scrollIntoView(true);
            }

            block = element('<%=pnlEditCertificationStamp.ClientID%>');
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
                1 - Certification Stamps
            </div>

            <div class="dataBlock-body">
                <p style="text-align: right;">
                    <asp:Button runat="server" ID="btnAddCertificationStamp" Text="Add New Stamp" />
                </p>

                <asp:GridView runat="server" ID="gdvCertificationStamps" AutoGenerateColumns="False" Width="100%" DataKeyNames="Id">
                    <Columns>
                        <asp:BoundField DataField="Id" HeaderText="Id" ReadOnly="True" ItemStyle-Width="5%" />
                        <asp:TemplateField HeaderText="Name">
                            <ItemTemplate>
                                <asp:Label ID="lblName" runat="server" Text='<%# Eval("Name")%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="50%" />
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Is Qualified">
                            <ItemTemplate>
                                <asp:Label ID="lblQualified" runat="server" Text='<%# Eval("IsQualified").ToString()%>'></asp:Label>
                            </ItemTemplate>
                            <ItemStyle Width="30%" />
                        </asp:TemplateField>
                        <asp:TemplateField>
                            <ItemTemplate>
                                <asp:LinkButton runat="server" ID="lbtnEditWorkflowMaps" CommandName="EditWorkflowMaps" Text="Workflows" CommandArgument='<%# Eval("Id").ToString() + "|" + Container.DataItemIndex.ToString() %>' />
                                &nbsp;
                                <asp:LinkButton runat="server" ID="lbtnEditCertificationStamp" CommandName="EditCertificationStamp" Text="Edit" CommandArgument='<%# Eval("Id").ToString() + "|" + Container.DataItemIndex.ToString() %>' />
                            </ItemTemplate>
                            <ItemStyle Width="15%" />
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>

        <asp:Panel runat ="server" ID="pnlWorkflowMaps" Visible="false" CssClass="dataBlock">
            <div class="dataBlock-header">
                2 - Certification Stamp to Workflows Mapping
            </div>

            <div class="dataBlock-body">
                <asp:GridView runat="server" ID="gdvCertificationStampWorkflows" AutoGenerateColumns="False" Width="100%">
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
                <asp:Button runat="server" ID="btnUpdateWorkflowMappings" Text="Save" Width="60px" />
                &nbsp;
                <asp:Button runat="server" ID="btnCancelWorkflowMappings" Text="Cancel" Width="60px" />
            </div>
        </asp:Panel>

        <asp:Panel runat ="server" ID="pnlEditCertificationStamp" Visible="false" CssClass="dataBlock">
            <div class="dataBlock-header">
                <asp:Label runat="server" ID="lblEditBlockLabel" Text="2 - Edit Certification Stamp" />
            </div>

            <div class="dataBlock-body">
                <table>
                    <tr>
                        <td class="number">
                            A
                        </td>
                        <td class="label labelRequired">
                            Name:
                        </td>
                        <td class="value">
                            <asp:TextBox runat="server" ID="txtStampName" MaxLength="50" Width="250px" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            B
                        </td>
                        <td class="label labelRequired">
                            Body:
                        </td>
                        <td class="value">
                            <asp:TextBox runat="server" ID="txtStampBody" TextMode="MultiLine" Columns="80" Rows="10" MaxLength="250" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            C
                        </td>
                        <td class="label labelRequired">
                            Is Qualified:
                        </td>
                        <td class="value">
                            <asp:CheckBox runat="server" ID="chkStampIsQualified" />
                        </td>
                    </tr>
                    <tr>
                        <td class="number">
                            D
                        </td>
                        <td class="label">
                            Action:
                        </td>
                        <td class="value">
                            <asp:Button runat="server" ID="btnInsertCertificationStamp" Text="Save" Width="60px" Visible="false" />
                            <asp:Button runat="server" ID="btnUpdateCertificationStamp" Text="Save" Width="60px" Visible="false" />
                            &nbsp;
                            <asp:Button runat="server" ID="btnCancelCertificationStamp" Text="Cancel" Width="60px" />
                        </td>
                    </tr>
                    <tr runat="server" id="trValidationErrors" visible="false">
                        <td class="number">
                            E
                        </td>
                        <td class="label">
                            ERRORS:
                        </td>
                        <td>
                            <asp:BulletedList runat="server" ID="bllValidationErrors" CssClass="labelRequired" />
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>
    </div>
</asp:Content>
