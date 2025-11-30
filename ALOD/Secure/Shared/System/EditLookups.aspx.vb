Imports ALODWebUtility.Common

Namespace Web.Sys

    Partial Class Secure_Shared_System_EditLookups
        Inherits System.Web.UI.Page

        Protected Sub btnAddMAJCOM_Click(sender As Object, e As System.EventArgs) Handles btnAddMAJCOM.Click
            DisableRequiredFieldValidators()

            rfvMAJCOM.Enabled = True

            If (Page.IsValid) Then
                Dim mc As New majcom()
                mc.InsertMAJCOM(0, txtMAJCOMName.Text.Trim, 0)
                MAJCOMGridView.DataBind()
            End If

            EnableRequiredFieldValidators()
        End Sub

        Protected Sub btnAddPEPPRating_Click(sender As Object, e As System.EventArgs) Handles btnAddPEPPRating.Click
            DisableRequiredFieldValidators()

            rfvPEPPRating.Enabled = True

            If (Page.IsValid) Then
                Dim businessObj As New PEPPRating()
                businessObj.InsertPEPPRating(0, txtPEPPRatingName.Text.Trim, 0) ' Passing in 0 for the Value indicates to the stored procedure to insert a new record
                PEPPRatingGridView.DataBind()
            End If

            EnableRequiredFieldValidators()
        End Sub

        Protected Sub btnAddPEPPType_Click(sender As Object, e As System.EventArgs) Handles btnAddPEPPType.Click
            DisableRequiredFieldValidators()

            rfvPEPPType.Enabled = True

            If (Page.IsValid) Then
                Dim businessObj As New PEPPType()
                businessObj.InsertPEPPType(0, txtPEPPTypeName.Text.Trim, 0) ' Passing in 0 for the Value indicates to the stored procedure to insert a new record
                PEPPTypeGridView.DataBind()
            End If

            EnableRequiredFieldValidators()
        End Sub

        'Protected Sub btnAddPEPPDisposition_Click(sender As Object, e As System.EventArgs) Handles btnAddPEPPDisposition.Click
        '    DisableRequiredFieldValidators()

        '    rfvPEPPDisposition.Enabled = True

        '    If (Page.IsValid) Then
        '        Dim businessObj As New PEPPDisposition()
        '        businessObj.InsertPEPPDisposition(0, txtPEPPDispositionName.Text.Trim, 0) ' Passing in 0 for the Value indicates to the stored procedure to insert a new record
        '        PEPPDispositionGridView.DataBind()
        '    End If

        '    EnableRequiredFieldValidators()
        'End Sub

        Protected Sub btnAddRMU_Click(sender As Object, e As System.EventArgs) Handles btnAddRMU.Click
            DisableRequiredFieldValidators()

            rfvRMU.Enabled = True
            rfvRMUPAS.Enabled = True

            If (Page.IsValid) Then
                Dim businessObj As New RMU()
                businessObj.InsertRMU(0, txtRMUName.Text.Trim, txtRMUPAS.Text.Trim, chkCollocated.Checked) ' Passing in 0 for the Value indicates to the stored procedure to insert a new record
                RMUGridView.DataBind()
            End If

            EnableRequiredFieldValidators()
        End Sub

        Private Sub DisableRequiredFieldValidators()
            rfvPEPPType.Enabled = False
            rfvPEPPRating.Enabled = False
            'rfvPEPPDisposition.Enabled = False
            rfvRMU.Enabled = False
            rfvRMUPAS.Enabled = False
            rfvMAJCOM.Enabled = False
        End Sub

        Private Sub EnableRequiredFieldValidators()
            rfvPEPPType.Enabled = True
            rfvPEPPRating.Enabled = True
            'rfvPEPPDisposition.Enabled = True
            rfvRMU.Enabled = True
            rfvRMUPAS.Enabled = True
            rfvMAJCOM.Enabled = True
        End Sub

        Private Sub Secure_Shared_System_EditLookups_Load(sender As Object, e As EventArgs) Handles Me.Load

            If Session(SESSIONKEY_COMPO) = "6" Then
                rfvRMU.Text = "RMU name is required."
                rfvRMUPAS.Text = "RMU PAS Code is required."
            Else
                rfvRMU.Text = "GMU name is required."
                rfvRMUPAS.Text = "GMU PAS Code is required."
            End If

        End Sub

    End Class

End Namespace