Imports ALOD.Core.Domain.Users
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALODWebUtility.Common

Namespace Web.Admin

    Partial Class Secure_Shared_Admin_Pascodes
        Inherits System.Web.UI.Page

        Private Const FONT_TEXT1 As String = "<font style='color:#FF0000'>"
        Private Const FONT_TEXT2 As String = "</font>"

        Protected Sub ChainTypeDataBound(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddlChainType.DataBound
            ddlChainType.Items.Insert(0, New ListItem("-- Select View --", "0"))
        End Sub

#Region "UserAction"

        Protected Sub btnCreate_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnCreate.Click
            If Page.IsValid Then

                Dim unit As Integer = CInt(SrcUnitIdHdn.Value.Trim)
                Dim chainType As String = ddlChainType.SelectedItem.Value

                CreateChain(unit, chainType)

                btnExpandAll.Text = "+"
                lblExpand.Text = "Expand All"

            End If

        End Sub

        Protected Sub btnExpandAll_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnExpandAll.Click
            If (btnExpandAll.Text = "-") Then
                PassCodeTree.CollapseAll()
                PassCodeTree.ExpandDepth = 1
                PassCodeTree.Nodes(0).Expand()
                btnExpandAll.Text = "+"
                lblExpand.Text = "Expand All"
            Else

                PassCodeTree.ExpandAll()
                btnExpandAll.Text = "-"
                lblExpand.Text = "Collapse"
            End If
        End Sub

        Protected Function GetSearchParameters() As Dictionary(Of String, String)

            Dim unitId = CInt(SrcUnitIdHdn.Value.Trim)
            Dim chainType = Server.HtmlEncode(ddlChainType.SelectedValue)
            Dim unitNAme = Server.HtmlEncode(UnitNameTxt.Text.Trim())

            Dim srchParam As New Dictionary(Of String, String)
            srchParam.Add("UnitName", unitNAme)
            srchParam.Add("ChainType", chainType)
            srchParam.Add("Unit", unitId)
            Return srchParam

        End Function

        Protected Sub LoadPrevSearch()
            Dim srchParam As Dictionary(Of String, String) = CType(Session("ManagedUnit"), Dictionary(Of String, String))
            SrcNameHdn.Value = srchParam("UnitName")
            ddlChainType.SelectedValue = srchParam("ChainType")
            SrcUnitIdHdn.Value = srchParam("Unit")
            UnitNameTxt.Text = srchParam("UnitName")

            CreateChain(srchParam("Unit"), srchParam("ChainType"))

            btnExpandAll.Text = "+"
            lblExpand.Text = "Expand All"
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
            If Not Page.IsPostBack Then
                Dim chainTypes = New NHibernateDaoFactory().GetUnitChainTypeDao().GetAll()
                ddlChainType.DataSource = From p In chainTypes Where p.Active = True Select p
                ddlChainType.DataBind()

                If Not (Session("ManagedUnit") Is Nothing) Then
                    LoadPrevSearch()
                End If

                'Session("ManagedUnit") = Nothing
                btnFindUnit.Attributes.Add("onclick", "showSearcher('" + "Find Unit" + "','" + SrcUnitIdHdn.ClientID + "','" + SrcNameHdn.ClientID + "'); return false;")
            End If

            UnitNameTxt.Text = SrcNameHdn.Value

            If UnitNameTxt.Text = "" Then
                PassCodeTree.Nodes.Clear()
            End If

        End Sub

#End Region

#Region "PassCode"

        Public Sub ChangeNodeTemplate(ByVal parentNode As TreeNode)

            For Each chNode As TreeNode In parentNode.ChildNodes
                Dim valString = chNode.Value.Split(";")
                'IF the  unit falls under user's pascode so add edit link
                If (valString(1) = "1" Or HttpContext.Current.Session("AccessScope") >= AccessScope.Compo Or UserHasPermission(PERMISSION_MANAGE_UNITS)) Then
                    Dim editURL As String
                    editURL = "EditPasCode.aspx?csId=" + valString(0)
                    chNode.Text = chNode.Text + "   <a href='" + editURL + "'>Edit</a>"

                End If
                chNode.SelectAction = TreeNodeSelectAction.None
                ChangeNodeTemplate(chNode)
            Next
        End Sub

        Public Sub CreateChain(ByVal unit As Integer, ByVal chainType As String)

            'Clear the tree
            PassCodeTree.Nodes.Clear()

            Dim parentCmd As ALOD.Core.Domain.Users.Unit = UnitService.GetUnitByID(unit)

            If parentCmd Is Nothing Then
                divNoCommands.Visible = True
                btnExpandAll.Visible = False
                lblExpand.Visible = False
                Exit Sub
            End If

            prntCmdLbl.Text = ""
            prntCmdLbl.Text = ddlChainType.SelectedItem.Text + " units for   " + parentCmd.Name + "(" + parentCmd.PasCode + ")"

            Dim userPasCode As String = CStr(HttpContext.Current.Session("PasCode"))
            Dim userUnit As String = CStr(HttpContext.Current.Session("UnitId"))

            Dim pascode As childUnitsList = New childUnitsList()

            pascode.Read(unit, chainType, userUnit)

            btnExpandAll.Visible = True
            lblExpand.Visible = True
            divNoCommands.Visible = False

            'Create the first node with info from the parent
            Dim record As New ArrayList

            Dim rootPasCode As New childunit With {.cs_id = parentCmd.Id, .childPasCode = parentCmd.PasCode, .childName = parentCmd.Name, .InActive = parentCmd.InActive}
            Dim root As TreeNode = AddNodeAndChildren(rootPasCode, Nothing, pascode, record)

            PassCodeTree.Nodes.Add(root)

            'The node template style should be changed in the end.
            'First change the first one and then all teh children by recurssion
            Dim valString = PassCodeTree.Nodes(0).Value.Split(";")
            If (valString(1) = "1" Or HttpContext.Current.Session("AccessScope") >= AccessScope.Compo Or UserHasPermission(PERMISSION_MANAGE_UNITS)) Then
                Dim editURL As String
                editURL = "EditPasCode.aspx?csId=" + valString(0)
                PassCodeTree.Nodes(0).Text = PassCodeTree.Nodes(0).Text + "   <a href='" + editURL + "'>Edit</a>"

            End If
            PassCodeTree.Nodes(0).SelectAction = TreeNodeSelectAction.None
            ChangeNodeTemplate(PassCodeTree.Nodes(0))
            'Initially expand by one level
            PassCodeTree.Nodes(0).Expand()
            Session("ManagedUnit") = GetSearchParameters()

        End Sub

        Private Function AddNodeAndChildren(ByVal chNode As childunit, ByVal parentNode As TreeNode, ByVal pascode As childUnitsList, ByRef record As ArrayList) As TreeNode

            Dim Value As String

            Value = CType(chNode.cs_id, String)

            ' IF the user unit does not exist that means this unit does not fall under the user's pascode so should not be editable
            If chNode.userUnit = -1 Then
                Value = Value + ";" + "0"
            Else
                Value = Value + ";" + "1"
            End If

            Dim nodeName As String

            Dim node As TreeNode
            If record.Contains(chNode.cs_id) Then
                nodeName = chNode.childName + "   (" + chNode.childPasCode + " ) ---(Repeated Pascode in the chain)   "
            Else
                nodeName = chNode.childName + "   (" + chNode.childPasCode + " )   "
            End If

            If (chNode.InActive) Then
                nodeName = FONT_TEXT1 + nodeName + FONT_TEXT2
            End If

            node = New TreeNode(nodeName, Value)

            If Not record.Contains(chNode.cs_id) Then
                record.Add(chNode.cs_id)
                'Recurse
                Dim qList As List(Of childunit) = (From p In pascode Where p.parentCS_ID = chNode.cs_id Select p).ToList()
                For Each cUnit As childunit In qList
                    Dim child As TreeNode = AddNodeAndChildren(cUnit, node, pascode, record)
                    If Not child Is Nothing Then
                        node.ChildNodes.Add(child)
                    End If

                Next
            End If

            Return node     'Return the new TreeNode

        End Function

#End Region

    End Class

End Namespace