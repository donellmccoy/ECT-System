Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Data.Services

Namespace Web.Sys

    Partial Class Secure_Shared_System_PageAccess
        Inherits System.Web.UI.Page

        '6/21/19
        Dim WithEvents btnUpdate As Button

        Private _daoFactory As IDaoFactory
        Private _pageAccessList As IList(Of ALOD.Core.Domain.Workflow.PageAccess)

        Protected ReadOnly Property DaoFactory As IDaoFactory
            Get
                If (_daoFactory Is Nothing) Then
                    _daoFactory = New NHibernateDaoFactory()
                End If

                Return _daoFactory
            End Get
        End Property

        Protected Sub btnUpdate_ServerClick(sender As Object, e As System.EventArgs) Handles btnUpdate.Click
            Dim updatedPageAccessObjects As New List(Of ALOD.Core.Domain.Workflow.PageAccess)

            For Each group As RepeaterItem In rptGroups.Items
                If (Not IsDataListItem(group.ItemType)) Then
                    Continue For
                End If

                Dim groupLabel As Label = CType(group.FindControl("lblGroupId"), Label)
                Dim pages As Repeater = CType(group.FindControl("rptPages"), Repeater)

                If (groupLabel Is Nothing OrElse pages Is Nothing) Then
                    Continue For
                End If

                Dim groupId As Byte = CByte(groupLabel.Text)

                For Each page As RepeaterItem In pages.Items
                    If (Not IsDataListItem(page.ItemType)) Then
                        Continue For
                    End If

                    Dim pageLabel As Label = CType(page.FindControl("lblPageId"), Label)
                    Dim cbAccess As DropDownList = CType(page.FindControl("cbAccess"), DropDownList)

                    If (pageLabel Is Nothing OrElse cbAccess Is Nothing) Then
                        Continue For
                    End If

                    updatedPageAccessObjects.Add(CreatePageAccessObject(groupId, CByte(pageLabel.Text), CType(cbAccess.SelectedValue, ALOD.Core.Domain.Workflow.PageAccess.AccessLevel)))
                Next
            Next

            DaoFactory.GetPageAccessDao().UpdateList(cbCompo.SelectedValue, CByte(cbWorkflow.SelectedValue), cbStatus.SelectedValue, updatedPageAccessObjects)
        End Sub

        Protected Sub btnUpdate_ServerClick()

        End Sub

        Protected Sub cbStatus_DataBound(sender As Object, e As System.EventArgs) Handles cbStatus.DataBound
            LoadPageAccess()
        End Sub

        Protected Sub cbStatus_SelectedIndexChanged(sender As Object, e As System.EventArgs) Handles cbStatus.SelectedIndexChanged
            LoadPageAccess()
        End Sub

        Protected Sub ChildDataBound(sender As Object, e As System.Web.UI.WebControls.RepeaterItemEventArgs)
            If (Not IsDataListItem(e.Item.ItemType)) Then
                Exit Sub
            End If

            Dim data As ALOD.Core.Domain.Workflow.PageAccess = CType(e.Item.DataItem, ALOD.Core.Domain.Workflow.PageAccess)
            Dim list As DropDownList = CType(e.Item.FindControl("cbAccess"), DropDownList)

            If (list IsNot Nothing) Then
                list.SelectedValue = CStr(data.Access)
            End If
        End Sub

        Protected Sub InitPageAccessList()
            If (_pageAccessList Is Nothing) Then
                Dim compo As String = cbCompo.SelectedValue
                Dim workflow As Byte = CByte(cbWorkflow.SelectedValue)
                Dim status As Integer = cbStatus.SelectedValue

                Dim dao As IPageAccessDao = New NHibernateDaoFactory().GetPageAccessDao()
                _pageAccessList = dao.GetByWorkflowAndStatus(compo, workflow, status)
            End If
        End Sub

        Protected Sub LoadPageAccess()
            Dim groups As IList(Of UserGroup) = LookupService.GetGroupsByCompo(cbCompo.SelectedValue)

            rptGroups.DataSource = From g In groups Select g Order By g.Description
            rptGroups.DataBind()
        End Sub

        Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
            If (Not IsPostBack) Then
            End If
        End Sub

        Protected Sub rptGroups_ItemDataBound(sender As Object, e As System.Web.UI.WebControls.RepeaterItemEventArgs) Handles rptGroups.ItemDataBound
            If (Not IsDataListItem(e.Item.ItemType)) Then
                Exit Sub
            End If

            InitPageAccessList()
            Dim group As UserGroup = CType(e.Item.DataItem, UserGroup)
            Dim access = From pageAccess In _pageAccessList Where pageAccess.GroupId = group.Id Select pageAccess Order By pageAccess.PageId

            Dim child As Repeater = CType(e.Item.FindControl("rptPages"), Repeater)
            child.DataSource = access
            child.DataBind()
        End Sub

        Private Function CreatePageAccessObject(groupId As Byte, pageId As Short, accessLevel As ALOD.Core.Domain.Workflow.PageAccess.AccessLevel) As ALOD.Core.Domain.Workflow.PageAccess
            Dim access As New ALOD.Core.Domain.Workflow.PageAccess

            access.GroupId = groupId
            access.PageId = pageId
            access.Access = accessLevel

            Return access
        End Function

        Private Function IsDataListItem(itemType As ListItemType) As Boolean
            If (itemType <> ListItemType.Item AndAlso itemType <> ListItemType.AlternatingItem) Then
                Return False
            End If

            Return True
        End Function

    End Class

End Namespace