Imports ALOD.Core.Domain.Common.KeyVal
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Logging
Imports ALODWebUtility.Common

Namespace Web.UserControls

    Public Class AddKeyValDialog
        Inherits System.Web.UI.UserControl

        Private _keyValDao As IKeyValDao

        Public Event KeyValValueAdded()

        ReadOnly Property KeyValDao() As IKeyValDao
            Get
                If (_keyValDao Is Nothing) Then
                    _keyValDao = New NHibernateDaoFactory().GetKeyValDao()
                End If

                Return _keyValDao
            End Get
        End Property

        Public Sub FillKeySelectControl(ByVal data As IList(Of KeyValKey))
            ddlKeySelect.DataSource = data
            ddlKeySelect.DataTextField = "Description"
            ddlKeySelect.DataValueField = "Id"
            ddlKeySelect.DataBind()
        End Sub

        Protected Sub AddKeyValButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAddKeyVal.Click

            Dim values() As String = txtKeyValAddValues.Text.Split("|")

            If values.Length <> 3 Then
                Exit Sub
            End If

            Dim keyId As Integer
            Dim valueDescription As String
            Dim value As String

            Try
                keyId = Integer.Parse(values(0))
                valueDescription = HTMLEncodeNulls(values(1))
                value = HTMLEncodeNulls(values(2))
            Catch ex As Exception
                LogManager.LogError(ex)
                Exit Sub
            End Try

            If (keyId = 0 OrElse String.IsNullOrEmpty(valueDescription) = True OrElse String.IsNullOrEmpty(value) = True) Then
                Exit Sub
            End If

            KeyValDao.InsertKeyValue(keyId, valueDescription, value)

            LogManager.LogAction(ModuleType.System, UserAction.KeyValAdded, "Added KeyVal Value: " + valueDescription + " = " + value + ".")

            RaiseEvent KeyValValueAdded()

        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        End Sub

    End Class

End Namespace