Namespace Web.UserControls

    Partial Class Secure_Shared_UserControls_unitSearcher
        Inherits System.Web.UI.UserControl
        Private _selectedUnit As Int32
        Private active As Boolean

        Public Property ActiveOnly() As Boolean
            Get
                Boolean.TryParse(hdnActiveOnly.Value, active)
                Return active
            End Get
            Set(ByVal value As Boolean)
                hdnActiveOnly.Value = value.ToString()
            End Set
        End Property

        Public Property SelectedUnit() As String
            Get
                Return hdnSelectedUnit.Value
            End Get
            Set(ByVal value As String)
                hdnSelectedUnit.Value = value
            End Set
        End Property

        Public Property SelectedUnitName() As String
            Get
                Return hdnSelectedUnitName.Value
            End Get
            Set(ByVal value As String)
                hdnSelectedUnitName.Value = value
            End Set
        End Property

        Protected Sub cmdSearch_Click(ByVal sender As Object, ByVal e As System.EventArgs)

            '   Dim srchPascode As String ' = txtPasCode.Text.Trim
            '  Dim srchLongName As String ' = txtDescription.Text.Trim

            ' lblMessage.Text = "Please select a search criterian."
            ' If srchPascode = "" AndAlso srchLongName = "" Then
            ' Return
            ' End If

            'lblMessage.Text = "No results found"

            'Dim ctxAdmin As AdminDataContext = New AdminDataContext()

            'Dim results As List(Of core_pascode_sp_searchResult) = ctxAdmin.core_pascode_sp_search(srchPascode, srchLongName).ToList()

            ' If results.Count < 1 Then
            'Return
            'End If

            'lblMessage.Text = ""
            ' lstResults.DataSource = results
            ' lstResults.DataBind()

        End Sub

        Protected Sub lstResults_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs)
            '  SelectedUnit = lstResults.SelectedItem.Value
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If Not Page.IsPostBack Then
                lblMessage.Text = " "
                'cmdSearch.Attributes.Add("onclick", "getData();")
            End If
        End Sub

    End Class

End Namespace