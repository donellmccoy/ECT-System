Imports System.Reflection
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data

Namespace Web.Sys

    Partial Class Secure_Shared_System_StatusCodes
        Inherits System.Web.UI.Page

        Private _workflowDao As IWorkflowDao
        Private _workflowId As Byte

        Public Property WorkflowId() As Byte
            Get
                Return _workflowId
            End Get
            Set(ByVal value As Byte)
                _workflowId = value
            End Set
        End Property

        Protected ReadOnly Property WorkflowDao As IWorkflowDao
            Get
                If (_workflowDao Is Nothing) Then
                    _workflowDao = New NHibernateDaoFactory().GetWorkflowDao()
                End If

                Return _workflowDao
            End Get
        End Property

        Protected Sub btnAdd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAdd.Click

            Dim code As New ALODWebUtility.Worklfow.StatusCode

            code.Description = txtDescr.Text.Trim
            code.ModuleId = CByte(ddlModule2.SelectedValue)
            code.GroupId = CByte(cbAddGroup.SelectedValue)
            code.Compo = Session("Compo")
            code.IsFinal = cbFinal.Checked
            code.IsApproved = cbApproved.Checked
            code.CanAppeal = cbAppeal.Checked
            code.IsDisposition = cbDisposition.Checked
            code.IsFormal = cbFormal.Checked

            If (code.Insert()) Then

                txtDescr.Text = ""

                gvCodes.DataBind()

            End If

        End Sub

        Protected Sub GroupBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles cbAddGroup.DataBound
            Dim list As DropDownList = CType(sender, DropDownList)
            list.Items.Insert(0, New ListItem("None", "0"))
        End Sub

        Protected Sub gvCodes_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles gvCodes.RowDataBound
            Dim ddlTemp As New DropDownList
            If e.Row.RowType = DataControlRowType.DataRow Then
                ddlTemp = DirectCast(e.Row.FindControl("DropDownList1"), DropDownList)
                If Not ddlTemp Is Nothing Then
                    For Each currField As FieldInfo In GetType(ModuleType).GetFields
                        Dim myListItem As New ListItem
                        Dim modTypeTemp As ModuleType
                        Dim currValue As Integer
                        currValue = CType(currField.GetValue(modTypeTemp), Byte)
                        myListItem.Value = currValue
                        myListItem.Text = WorkflowDao.GetCaseType(currValue)  'CurrField.Name
                        If myListItem.Value <> 0 Then
                            ddlTemp.Items.Add(myListItem)
                        End If
                    Next
                    ''uncommon
                    ddlTemp.SelectedIndex = ddlModule.SelectedIndex
                End If
            End If
        End Sub

        Protected Sub Secure_Shared_System_StatusCodes_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            'If (Not Page.IsPostBack) Then

            Dim curMod As Integer

            'Populate Module drop down
            For Each currField As FieldInfo In GetType(ModuleType).GetFields
                Dim myListItem As New ListItem
                Dim modTypeTemp As ModuleType
                Dim blnInsert As Boolean = True
                Dim currValue As Integer
                currValue = CType(currField.GetValue(modTypeTemp), Byte)
                myListItem.Value = currValue
                myListItem.Text = WorkflowDao.GetCaseType(currValue)  'CurrField.Name
                For Each currItem In ddlModule.Items  'Keep from loading duplicate values on page reload
                    If currItem.ToString() = myListItem.Text.ToString() Then
                        blnInsert = False
                    End If
                Next
                If blnInsert Then
                    ddlModule.Items.Add(myListItem)
                    ddlModule2.Items.Add(myListItem)
                End If
            Next

            'If (Request.QueryString("module") IsNot Nothing) Then
            '    curMod = Request.QueryString("module")
            'Else
            '    curMod = ModuleType.LOD
            'End If

            WorkflowId = ddlModule.SelectedValue

            'If (ddlModule.SelectedValue = 0) Then
            If (WorkflowId = 0) Then
                curMod = ModuleType.LOD
            Else
                curMod = WorkflowId
            End If

            ddlModule.SelectedValue = curMod
            ddlModule2.SelectedValue = curMod

            Session("moduleId") = curMod
            'End If

        End Sub

    End Class

End Namespace