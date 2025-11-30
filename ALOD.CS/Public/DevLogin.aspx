<%@ Page Title="" Language="C#" MasterPageFile="~/Public/Public.master" AutoEventWireup="false" Inherits="ALOD.Web.Public_DevLogin" MaintainScrollPositionOnPostback="true" CodeBehind="DevLogin.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentMain" runat="Server">
    <script type="text/javascript">

        var appHostname = '<%= ResolveUrl("~/") %>';

        // Cache-friendly banner cycling with preloaded images
        var bannerConfig = {
            images: [
                appHostname + 'App_Themes/DefaultBlue/images/ALOD1.jpg',
                appHostname + 'App_Themes/DefaultBlue/images/ALOD2.jpg',
                appHostname + 'App_Themes/DefaultBlue/images/ALOD3.jpg',
                appHostname + 'App_Themes/DefaultBlue/images/ALOD4.jpg',
                appHostname + 'App_Themes/DefaultBlue/images/ALOD5.jpg'
            ],
            currentIndex: 0,
            preloadedImages: [],
            bannerElement: null,
            cycleInterval: null
        };

        // Preload all images to ensure they're cached
        function preloadBannerImages() {
            for (var i = 0; i < bannerConfig.images.length; i++) {
                var img = new Image();
                img.src = bannerConfig.images[i];
                bannerConfig.preloadedImages.push(img);
            }
        }

        // Cache-friendly banner cycling function
        function cycleBanner() {
            if (!bannerConfig.bannerElement) {
                bannerConfig.bannerElement = document.getElementsByName('banner')[0];
                if (!bannerConfig.bannerElement) return; // Exit if banner element not found
            }

            bannerConfig.currentIndex = (bannerConfig.currentIndex + 1) % bannerConfig.images.length;
            
            // Use preloaded image source (already cached)
            bannerConfig.bannerElement.src = bannerConfig.preloadedImages[bannerConfig.currentIndex].src;
        }

        // Initialize banner cycling when page is ready
        $(function () {
            // Preload all images first
            preloadBannerImages();
            
            // Wait a moment for images to start loading, then begin cycling
            setTimeout(function() {
                // Start cycling after preload
                bannerConfig.cycleInterval = setInterval(cycleBanner, 5000);
            }, 1000);
        });

        // Cleanup function to prevent memory leaks
        $(window).on('beforeunload', function() {
            if (bannerConfig.cycleInterval) {
                clearInterval(bannerConfig.cycleInterval);
            }
        });
    </script>
    <div>
        <div id="front-actions">
            <div style="float: left; width: 600px; height: 275px; background-repeat: no-repeat; background-color: transparent;">
                <img style="width: 600px; height: 275px" src="../App_Themes/DefaultBlue/images/ALOD1.jpg"
                    name="banner" alt="ECT Banner" />
            </div>
            <div id="front-login">
                <table border="0" cellpadding="5" cellspacing="0" style="width: 100%">
                    <tr>
                        <td align="left">
                            <h2>Login By EDIPIN:</h2>
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:Label runat="server" AssociatedControlID="EDIPIN" Text="EDIPIN:" />
                            <asp:TextBox runat="Server" TextMode="Password" ID="EDIPIN"></asp:TextBox><br />
                            <asp:Label ID="lblBadLogin" runat="server" Font-Bold="True" ForeColor="DarkRed" Text="Login not found" Visible="False"></asp:Label><br />
                            <asp:Button runat="server" ID="btnLogin" Text="Login to ECT" />
                        </td>
                    </tr>
                    <tr>
                        <td align="center">
                            <asp:LinkButton ID="lnkNormalLogin" runat="server">Normal Login</asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div id="frontpage-content">
            <br />
            <br />
            <div style="padding: 0 0 20px 20px;">
                <h2>Login by User Role:</h2>
                <div style="padding: 0 0 0 10px;">
                    <asp:DropDownList ID="cbCompo" runat="server" AutoPostBack="True" Visible="true">
                        <asp:ListItem Value="5">Air National Guard</asp:ListItem>
                        <asp:ListItem Value="6">Air Force Reserve</asp:ListItem>
                    </asp:DropDownList>
                    Select Wing:
                    <asp:DropDownList runat="server" ID="WingSelect" AutoPostBack="true" DataSourceID="WingDataSource"
                        DataTextField="Name" DataValueField="Id">
                    </asp:DropDownList>
                    <asp:CheckBox ID="ShowBoard" runat="server" AutoPostBack="True" Text="Show Board Members" />
                </div>
                <br />
                <asp:RadioButtonList runat="server" ID="rblRoles" DataSourceID="SqlDataSource1" DataTextField="name"
                    DataValueField="edipin" RepeatColumns="3" CellPadding="2" CellSpacing="10" Width="90%" AutoPostBack="True">
                </asp:RadioButtonList>
                <br />
                &nbsp;&nbsp;<asp:Button ID="btnRoleLogin" runat="server" Text="Login" />
                <br />
            </div>
            <%--<div style="margin: 25px 25px;">VERSION 6-27-2018 05-00</div>--%>
            <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString="<%$ ConnectionStrings:LOD %>"
                SelectCommand="dev_sp_GetDevLogins" SelectCommandType="StoredProcedure">
                <SelectParameters>
                    <asp:ControlParameter ControlID="cbCompo" Name="compo" PropertyName="SelectedValue"
                        Type="String" />
                    <asp:ControlParameter ControlID="WingSelect" Name="unit" PropertyName="SelectedValue"
                        Type="Int32" />
                    <asp:Parameter Direction="ReturnValue" Name="RETURN_VALUE" Type="Int32" />
                    <asp:ControlParameter ControlID="ShowBoard" Name="board" PropertyName="Checked"
                        Type="Boolean" />
                </SelectParameters>
            </asp:SqlDataSource>
            <asp:SqlDataSource ID="WingDataSource" runat="server"
                ConnectionString="<%$ ConnectionStrings:LOD %>"
                ProviderName="<%$ ConnectionStrings:LOD.ProviderName %>"
                SelectCommand="dev_sp_GetUnits" SelectCommandType="StoredProcedure"></asp:SqlDataSource>
        </div>
    </div>
</asp:Content>
