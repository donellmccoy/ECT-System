Imports ALOD.Core.Domain.Common
Imports ALODWebUtility.TabNavigation

Namespace Web.UserControls

    Partial Class TabControls
        Inherits TabControlBase

        Protected _navigator As TabNavigator

#Region "Members / Properties"

        Public Overrides Property BackEnabled() As Boolean
            Get
                Return btnBack.Enabled
            End Get
            Set(ByVal Value As Boolean)
                btnBack.Enabled = Value
            End Set
        End Property

        Public Overrides Property NextEnabled() As Boolean
            Get
                Return btnNext.Enabled
            End Get
            Set(ByVal Value As Boolean)
                btnNext.Enabled = False
            End Set
        End Property

        Default Public Overrides ReadOnly Property Item(ByVal type As NavigatorButtonType) As System.Web.UI.WebControls.Button
            '	Default Public ReadOnly Property Item(ByVal type As NavigatorButtonType)
            Get
                Select Case type
                    Case NavigatorButtonType.NextStep
                        Return Me.btnNext
                    Case NavigatorButtonType.PreviousStep
                        Return Me.btnBack
                    Case NavigatorButtonType.Save
                        Return Me.btnSave
                    Case NavigatorButtonType.Print
                        Return Me.btnPrint
                    Case NavigatorButtonType.Delete
                        Return Me.btnDelete
                End Select

                Return Nothing
            End Get
        End Property

#End Region

        Protected Sub btnCommand_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCommand.Click

            Select Case txtCommand.Text.ToLower()
                Case "next"
                    If (_navigator.RaiseNavigationEvent(NavigatorButtonType.NextStep)) Then
                        _navigator.StepCompleted()
                        _navigator.MoveToNextPage()
                    End If

                Case "prev"
                    If (_navigator.RaiseNavigationEvent(NavigatorButtonType.PreviousStep)) Then
                        _navigator.MoveToPrevPage()
                    End If

                Case "save"
                    _navigator.RaiseNavigationEvent(NavigatorButtonType.Save)

                Case "print"
                    _navigator.RaiseNavigationEvent(NavigatorButtonType.Print)

                Case "delete"
                    _navigator.RaiseNavigationEvent(NavigatorButtonType.Delete)

                Case Else 'tab header
                    _navigator.RaiseNavigationEvent(NavigatorButtonType.NavigatedAway)

            End Select

        End Sub

        Protected Sub WriteScript()

            Dim buffer As New StringBuilder()

            If (_navigator.PrintScript.Length > 0) Then
                btnPrint.Attributes.Add("onclick", _navigator.PrintScript + "; return false;")
            Else
                btnPrint.Attributes.Add("onclick", "return submitCommand('print');")
            End If

            btnSave.Attributes.Add("onclick", "return submitCommand('save');")
            btnBack.Attributes.Add("onclick", "return submitCommand('prev');")
            btnNext.Attributes.Add("onclick", "return submitCommand('next');")
            btnDelete.Attributes.Add("onclick", "if (confirmAction('delete')) { submitCommand('delete'); } ")

            buffer.Append("<script type='text/javascript'>" + vbCrLf)
            buffer.Append("function submitCommand(cmd) {" + vbCrLf)

            Dim item As TabItem = _navigator.CurrentStep()

            'if we have a client script routine for this page, add it here
            If (item.ClientScript.Length > 0) Then
                buffer.Append(" if (!" + item.ClientScript + "()) { return false; }" + vbCrLf)
            End If

            buffer.Append("element('" + Me.ClientID + "_txtCommand').value = cmd;" + vbCrLf)
            buffer.Append("element('" + Me.ClientID + "_btnCommand').click();" + vbCrLf)
            buffer.Append("$.blockUI();" + vbCrLf)
            buffer.Append("}" + vbCrLf)
            buffer.Append("</script>" + vbCrLf)

            Page.ClientScript.RegisterClientScriptBlock(Me.GetType().BaseType, "commandscript", buffer.ToString())

        End Sub

        Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

            'find our wizard control
            For Each ctrl As Control In Parent.Controls

                If (TypeOf ctrl Is TabNavigator) Then
                    _navigator = ctrl
                    Exit For
                End If

            Next

            If _navigator Is Nothing Then
                Throw New ArgumentNullException("WizardNavigator was not found in page. This control requires a WizardNavigator control in the same page")
            End If

            'set the reference to ourselves in the wizardNavigator
            _navigator.NavControls = Me

            'write out our command script
            WriteScript()

            If (IsPostBack) Then
                btnCommand_Click(sender, e)
            Else
                'enable/disable button controls
                btnBack.Enabled = _navigator.PrevButtonEnabled
                btnNext.Enabled = _navigator.NextButtonEnabled
                btnPrint.Visible = _navigator.PrintButtonVisible
                btnDelete.Visible = _navigator.DeleteButtonVisible
                'JumpBox1.Visible = _navigator.JumpBoxVisible
                'btnDelete.Attributes.Add("onclick", "return confirmAction('delete');")
            End If

        End Sub

    End Class

End Namespace