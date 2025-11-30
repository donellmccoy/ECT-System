<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/Secure/Secure.master" CodeBehind="PHTrend.aspx.vb" Inherits="ALOD.Web.Reports.PHTrend" %>



<%@ Register src="../Shared/UserControls/ReportNavPH.ascx" tagname="ReportNavPH" tagprefix="uc1" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentMain" runat="server">

    <div>
        <div class="indent">
            <asp:UpdatePanel ID="resultsUpdatePanel" runat="server">
                <ContentTemplate>
                    <div>
                        <asp:Repeater ID="rptNav" runat="server">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkButton" runat="server"></asp:LinkButton>
                            </ItemTemplate>
                            <SeparatorTemplate>
                                <b>></b>
                            </SeparatorTemplate>
                        </asp:Repeater>
                    </div>
                    <br />
                    <div>



                        <uc1:ReportNavPH ID="ReportNavPH1" runat="server" />



                    </div>

                    <div>
                        <div id="rpt">
                            <div id="spSpacer" class="emptyItem" style="margin-top: 0px; margin-bottom: 4px;
                                height: 22px;">
                                <div id="spWait" class="" style="display: none;">
                                    &nbsp;<asp:Image runat="server" ID="imgWait" SkinID="imgBusy" AlternateText="busy"
                                        ImageAlign="AbsMiddle" />&nbsp;Loading...
                                </div>
                            </div>
                            <asp:Panel runat="server" ID="FeedbackPanel" Visible="false" CssClass="ui-state-highlight info-block msg-panel">
                                <asp:Label runat="server" ID="FeedbackMessageLabel" />
                            </asp:Panel>
                            <br />


                            <asp:Label runat="server" ID="lblDate"></asp:Label>



                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>

        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentFooter" runat="server">
</asp:Content>
