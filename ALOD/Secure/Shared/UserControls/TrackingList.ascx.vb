Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Data.Services

Namespace Web.UserControls

    Partial Class Secure_Shared_UserControls_TrackingList
        Inherits System.Web.UI.UserControl

        Private _lod As LineOfDuty = Nothing
        Private _type As ModuleType

        ReadOnly Property instance() As LineOfDuty
            Get
                If (_lod Is Nothing) Then
                    _lod = LodService.GetById(CInt(Session("RefId")))
                End If
                Return _lod
            End Get
        End Property

        ReadOnly Property StatusCode() As Integer
            Get
                Dim key As String = "TrackStatusCode"

                If (ViewState(key) Is Nothing) Then

                    If (instance Is Nothing) Then
                        Return 0
                    End If

                    ViewState(key) = instance.Status
                End If

                Return CStr(ViewState(key))

            End Get
        End Property

        Public Property ModuleType() As ModuleType
            Get
                Return ViewState("ModuleType")
            End Get
            Set(ByVal value As ModuleType)
                ViewState("ModuleType") = value
            End Set
        End Property

        Protected ReadOnly Property ReferenceId() As Integer
            Get
                Return CInt(Request.QueryString("refId"))
            End Get
        End Property

        Protected ReadOnly Property SecondaryId() As Integer
            Get
                Return 0
            End Get
        End Property

        Protected ReadOnly Property TrackingReferenceId() As Integer
            Get
                Dim key As String = "TrackRefId"

                If (ViewState(key) Is Nothing) Then

                    If (instance Is Nothing) Then
                        Return 0
                    End If

                    ViewState(key) = instance.TrackingId
                End If

                Return CStr(ViewState(key))

            End Get
        End Property

        Protected Sub chkShowAll_CheckedChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles chkShowAll.CheckedChanged
            LoadData()
        End Sub

        Protected Sub LoadData()

            Dim dao As ITrackingDao = New NHibernateDaoFactory().GetTrackingDao()
            gvTracking.DataSource = dao.GetByParentId(TrackingReferenceId, ModuleType, chkShowAll.Checked)
            gvTracking.DataBind()

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
                LoadData()
            End If
        End Sub

        Protected Sub RowBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvTracking.RowDataBound

            If (e.Row.RowType <> DataControlRowType.DataRow) Then
                Exit Sub
            End If

            Dim row As TrackingItem = CType(e.Row.DataItem, TrackingItem)

            Dim id As String = gvTracking.DataKeys(e.Row.RowIndex).Value
            Dim link As HyperLink = CType(e.Row.FindControl("lnkUser"), HyperLink)
            link.NavigateUrl = "#"
            link.Attributes.Add("onclick", "getWhois('" + id + "'); return false;")

            link = CType(e.Row.FindControl("lnkAction"), HyperLink)

            If (Not row.LogChanges) Then
                link.Font.Underline = False
            Else
                link.NavigateUrl = "#"
                link.Attributes.Add("onclick", "showChangeSet('" + row.Id.ToString() + "','0'); return false;")
            End If

        End Sub

    End Class

End Namespace