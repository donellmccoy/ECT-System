Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALOD.Logging
Imports ALODWebUtility.Common

Namespace Web.UserControls

    Partial Class Secure_Shared_UserControls_CaseTracking
        Inherits System.Web.UI.UserControl

        Private _details As IQueryable(Of TrackingItem)

        Public Property ModuleType() As Byte
            Get
                If (ViewState("moduleId") Is Nothing) Then
                    ViewState("moduleId") = 0
                End If
                Return CByte(ViewState("moduleId"))
            End Get
            Set(value As Byte)
                ViewState("moduleId") = value
            End Set
        End Property

        Public Property RequestId() As Integer
            Get
                If (ViewState("requestId") Is Nothing) Then
                    ViewState("requestId") = 0
                End If
                Return CInt(ViewState("requestId"))
            End Get
            Set(value As Integer)
                ViewState("requestId") = value
            End Set
        End Property

        Protected Property CaseId() As String
            Get
                If (ViewState("CaseId") Is Nothing) Then
                    ViewState("CaseId") = ""
                End If
                Return ViewState("CaseId")
            End Get
            Set(ByVal value As String)
                ViewState("CaseId") = value
            End Set
        End Property

        Protected Property MemberUnitId() As Integer
            Get
                If (ViewState("MemberUnitId") Is Nothing) Then
                    ViewState("MemberUnitId") = 0
                End If
                Return CInt(ViewState("MemberUnitId"))
            End Get
            Set(value As Integer)
                ViewState("MemberUnitId") = value
            End Set
        End Property

        Protected Property wsStatus() As Integer
            Get
                If (ViewState("wsStatus") Is Nothing) Then
                    ViewState("wsStatus") = 0
                End If
                Return CInt(ViewState("wsStatus"))
            End Get
            Set(value As Integer)
                ViewState("wsStatus") = value
            End Set
        End Property

        Public Sub Initialize(hostPage As Page, moduleId As Byte, refId As Integer, case_id As String, status As Integer, unitId As Integer)

            ModuleType = moduleId
            RequestId = refId
            CaseId = case_id
            wsStatus = status
            MemberUnitId = unitId

            ScriptManager.GetCurrent(hostPage).RegisterAsyncPostBackControl(Me)

        End Sub

        Protected Sub chkShowAll_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkShowAll.CheckedChanged
            UpdateTrackingDisplay()
        End Sub

        Protected Sub DetailsList_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs)

            Dim details As Repeater = CType(sender, Repeater)

            If (e.Item.ItemType <> ListItemType.Item AndAlso e.Item.ItemType <> ListItemType.AlternatingItem) Then
                Exit Sub
            End If

            Dim row As TrackingItem = CType(e.Item.DataItem, TrackingItem)

            Dim link As HyperLink = CType(e.Item.FindControl("lnkUser"), HyperLink)
            link.NavigateUrl = "#"
            link.Attributes.Add("onclick", "getWhois('" + row.UserId.ToString() + "'); return false;")

            link = CType(e.Item.FindControl("lnkAction"), HyperLink)

            If (Not row.LogChanges) Then
                link.Font.Underline = False
            Else
                link.NavigateUrl = "#"
                link.Attributes.Add("onclick", "showChangeSet('" + row.Id.ToString() + "','0'); return false;")
            End If

        End Sub

        Protected Function GetDetails() As IQueryable(Of TrackingItem)
            If (_details Is Nothing) Then
                Dim dao As ITrackingDao = New NHibernateDaoFactory().GetTrackingDao()
                _details = dao.GetByParentId(RequestId, ModuleType, chkShowAll.Checked)
            End If
            Return _details
        End Function

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            AddStyleSheet(Page, "~/Styles/CaseTracking.css")

            If (Not IsPostBack) Then

                LogManager.LogAction(ModuleType, UserAction.ViewPage, RequestId, "Viewed Page: Tracking")
                'display case history

                UpdateTrackingDisplay()

            End If
        End Sub

        Protected Sub TrackingList_ItemDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles TrackingList.ItemDataBound

            If (e.Item.ItemType <> ListItemType.Item AndAlso e.Item.ItemType <> ListItemType.AlternatingItem) Then
                Exit Sub
            End If

            Dim data As WorkStatusTracking = CType(e.Item.DataItem, WorkStatusTracking)

            Dim link As HyperLink = CType(e.Item.FindControl("lnkUser"), HyperLink)
            link.NavigateUrl = "#"
            link.Attributes.Add("onclick", "getWhois('" + data.CompletedBy.ToString() + "'); return false;")
            If (Not data.CompletedBy Is Nothing) Then
                link.Text = GetUserName(data.CompletedBy)
            End If
            Dim panel As Panel = CType(e.Item.FindControl("DetailsPanel"), Panel)

            Dim img As Image = CType(e.Item.FindControl("ExpandImage"), Image)
            img.Attributes.Add("onclick", "toggleDetails('" + panel.ClientID + "','" + img.ClientID + "');")

            Dim endDate As DateTime

            If (data.EndDate.HasValue) Then
                endDate = data.EndDate.Value
            Else
                endDate = Date.Now
            End If

            GetDetails()
            Dim DetailsRepeater As Repeater = CType(e.Item.FindControl("DetailsRepeater"), Repeater)
            Dim results As IOrderedQueryable(Of TrackingItem)

            If (data.WorkflowStatus.StatusCodeType.IsFinal) Then

                ' Get other WorkStatusTracking items that represent a Complete status that occured AFTER the current WorkStatusTracking item.
                Dim otherCompletes = From w In LookupService.GetWorkStatusTracking(RequestId, ModuleType)
                                     Where w.WorkflowStatus.StatusCodeType.IsFinal And Not w.WorkflowStatus.StatusCodeType.IsCancel And w.StartDate > endDate
                                     Select w
                                     Order By w.StartDate

                ' This check is needed for the case where a LOD is overriden and re-completed. Without this check PostCompleted actions could become associated with the incorrect completed step
                If (otherCompletes.Count > 0) Then
                    results = From d In _details
                              Where (d.ActionId <> UserAction.PostCompletion And d.ActionDate >= data.StartDate) _
                                Or (d.ActionId = UserAction.PostCompletion And d.ActionDate >= data.StartDate And d.ActionDate < otherCompletes.First().StartDate)
                              Select d
                              Order By d.Id Descending
                Else
                    results = From d In _details
                              Where ((d.ActionId <> UserAction.PostCompletion And d.ActionDate >= data.StartDate) _
                                 Or (d.ActionId = UserAction.PostCompletion And d.ActionDate >= data.StartDate))
                              Select d
                              Order By d.Id Descending
                End If
            Else
                results = From d In _details Where (d.ActionId <> UserAction.PostCompletion And d.ActionDate >= data.StartDate And d.ActionDate <= endDate) Select d Order By d.Id Descending
            End If

            DetailsRepeater.DataSource = results
            DetailsRepeater.DataBind()

        End Sub

        Protected Sub UpdateTrackingDisplay()

            TrackingList.DataSource = LookupService.GetWorkStatusTracking(RequestId, ModuleType)
            TrackingList.DataBind()

        End Sub

    End Class

End Namespace