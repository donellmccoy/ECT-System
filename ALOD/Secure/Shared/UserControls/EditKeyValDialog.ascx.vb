Imports ALOD.Core.Domain.Common.KeyVal
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Logging
Imports ALODWebUtility.Common

Namespace Web.UserControls

    Public Class EditKeyValDialog
        Inherits System.Web.UI.UserControl

        Private _keyValDao As IKeyValDao

        Public Event KeyValValueEdited()

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

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        End Sub

        Protected Sub UpdateKeyValButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnUpdateKeyVal.Click

            Dim values() As String = txtKeyValEditValues.Text.Split("|")

            If values.Length <> 4 Then
                Exit Sub
            End If

            Dim id As Integer
            Dim keyId As Integer
            Dim valueDescription As String
            Dim value As String

            Try
                id = Integer.Parse(values(0))
                keyId = Integer.Parse(values(1))
                valueDescription = HTMLEncodeNulls(values(2))
                value = HTMLEncodeNulls(values(3))
            Catch ex As Exception
                LogManager.LogError(ex)
                Exit Sub
            End Try

            If (id = 0 OrElse keyId = 0 OrElse String.IsNullOrEmpty(valueDescription) = True OrElse String.IsNullOrEmpty(value) = True) Then
                Exit Sub
            End If

            KeyValDao.UpdateKeyValueById(id, keyId, valueDescription, value)

            LogManager.LogAction(ModuleType.System, UserAction.KeyValEdited, "Edited KeyVal Value: " + valueDescription + " = " + value + ".")

            RaiseEvent KeyValValueEdited()

        End Sub

    End Class

End Namespace