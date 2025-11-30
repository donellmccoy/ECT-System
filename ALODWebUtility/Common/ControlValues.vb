Imports System.Reflection
Imports System.Web.UI
Imports System.Web.UI.HtmlControls
Imports System.Web.UI.WebControls

Namespace Common

    Public Class ControlValues

#Region "ControlValues GetMethods"

        Public Shared Function GetAttribute(ByVal ctl As Object, ByVal TypeString As String, ByVal AttrName As String) As String
            Dim ret As String = ""
            Select Case (TypeString)
                Case "System.Web.UI.WebControls.ListBox"
                    ret = CType(ctl, ListBox).Attributes(AttrName)
                Case "System.Web.UI.WebControls.CheckBoxList"
                    ret = CType(ctl, CheckBoxList).Attributes(AttrName)
                Case "System.Web.UI.WebControls.DropDownList"
                    ret = CType(ctl, DropDownList).Attributes(AttrName)
                Case "System.Web.UI.WebControls.CheckBox"
                    ret = CType(ctl, CheckBox).Attributes(AttrName)
                Case "System.Web.UI.WebControls.TextBox"
                    ret = CType(ctl, TextBox).Attributes(AttrName)
                Case "System.Web.UI.WebControls.GridView"
                    ret = CType(ctl, GridView).Attributes(AttrName)
                Case "System.Web.UI.WebControls.RadioButtonList"
                    ret = CType(ctl, RadioButtonList).Attributes(AttrName)
                Case "System.Web.UI.WebControls.RadioButton"
                    ret = CType(ctl, RadioButton).Attributes(AttrName)
                Case "System.Web.UI.HtmlControls.HtmlInputHidden"
                    ret = CType(ctl, HtmlInputHidden).Attributes(AttrName)

            End Select
            Return ret

        End Function

        Public Shared Function GetControls(ByVal ctls As System.Web.UI.ControlCollection) As String
            Dim sb As New System.Text.StringBuilder(1024)
            Dim dc As ModControl
            Dim TypeString As String

            For Each ctl As Web.UI.Control In ctls
                If ctl.HasControls Then
                    ' Recurse into any "Grouping" controls
                    sb.Append(GetControls(ctl.Controls))
                End If

                ' Get Control Definition

                TypeString = ctl.GetType().ToString()
                dc = ControlLoad.mControlList.GetByType(TypeString)
                If Not (dc Is Nothing) Then
                    If dc.IsMultiList And dc.PropertyToCheck = String.Empty Then
                        If TypeString = "System.Web.UI.WebControls.ListBox" Then
                            sb.Append(GetMultiListValues(CType(ctl, ListBox)))
                        ElseIf TypeString = "System.Web.UI.WebControls.CheckBoxList" Then
                            sb.Append(GetMultiListValues(CType(ctl, CheckBoxList)))
                        ElseIf TypeString = "System.Web.UI.WebControls.RadioButtonList" Then
                            sb.Append(GetMultiListValues(CType(ctl, RadioButtonList)))

                        End If
                    ElseIf dc.IsMultiList And dc.PropertyToCheck <> String.Empty Then
                        If GetControlValue(ctl, dc.PropertyToCheck) = dc.PropertyToCheckValue Then
                            sb.Append(GetControlValue(ctl, dc.Property1))
                        Else
                            sb.Append(GetMultiListValues(CType(ctl, ListBox)))
                        End If
                    ElseIf dc.IsMultiList = False And dc.PropertyToCheck <> String.Empty Then
                        If GetControlValue(ctl, dc.PropertyToCheck) = dc.PropertyToCheckValue Then
                            sb.Append(GetControlValue(ctl, dc.Property1))
                        Else
                            sb.Append(GetControlValue(ctl, dc.Property2))
                        End If
                    ElseIf dc.IsMultiList = False And dc.Property2 <> String.Empty Then
                        sb.Append(GetControlValue(ctl, dc.Property1))
                        sb.Append(GetControlValue(ctl, dc.Property2))
                    Else
                        sb.Append(GetControlValue(ctl, dc.Property1))
                    End If
                End If
            Next

            Return sb.ToString()
        End Function

        Public Shared Function GetControlValue(ByVal ctl As Control, ByVal PropertyName As String) As String
            Dim t As Type
            Dim value As String = ""
            t = ctl.GetType()
            Dim TypeString As String = t.ToString()
            If (TypeString = "System.Web.UI.WebControls.DropDownList") Then
                'Customized to get the Selected Item Text instead of Selected Index
                Dim ddl As DropDownList
                ddl = CType(ctl, DropDownList)

                If ddl.SelectedIndex >= 0 Then
                    value = ddl.SelectedItem.Text
                End If
            Else
                ' Use Reflection to get the property value
                value = Convert.ToString(t.InvokeMember(PropertyName, BindingFlags.GetProperty, Nothing, ctl, Nothing))
            End If

            Return value
        End Function

        Public Shared Function GetKeyValues(ByVal ctls As System.Web.UI.ControlCollection, ByRef lstPgCrtls As lodControlList, ByVal parentAttr As String) As String

            Dim sb As New System.Text.StringBuilder(1024)
            Dim dc As ModControl
            Dim valString As String = String.Empty
            Dim sectionString As String = String.Empty
            Dim nameString As String = String.Empty
            Dim fieldString As String = String.Empty

            Dim TypeString As String = String.Empty

            For Each ctl As Control In ctls
                TypeString = ctl.GetType().ToString()
                If ctl.HasControls Then
                    parentAttr = GetAttribute(ctl, TypeString, "Section")

                    If (TypeString <> "System.Web.UI.WebControls.CheckBoxList" AndAlso TypeString <> "System.Web.UI.WebControls.RadioButtonList") Then 'This is to stop it for recursing for any child controls for CheckBox list since we take care of that in getting multilist values
                        ' Recurse into any "Grouping" controls ,pass the parentAttribute
                        sb.Append(GetKeyValues(ctl.Controls, lstPgCrtls, parentAttr))
                    End If
                End If

                ' Get Control Definition
                dc = ControlLoad.mControlList.GetByType(TypeString)

                If Not (dc Is Nothing) Then
                    If dc.IsMultiList Then
                        If (dc.PropertyToCheck = String.Empty) Then
                            If TypeString = "System.Web.UI.WebControls.ListBox" Then
                                valString = GetMultiListValues(CType(ctl, ListBox))
                                sb.Append(valString)
                            ElseIf TypeString = "System.Web.UI.WebControls.CheckBoxList" Then
                                valString = GetMultiListValues(CType(ctl, CheckBoxList))
                                sb.Append(valString)
                            ElseIf TypeString = "System.Web.UI.WebControls.RadioButtonList" Then
                                valString = GetMultiListValues(CType(ctl, RadioButtonList))
                                sb.Append(valString)
                            End If
                        Else
                            'Case for ListBox for single and multiple selection for values
                            If GetControlValue(ctl, dc.PropertyToCheck) = dc.PropertyToCheckValue Then
                                valString = GetControlValue(ctl, dc.Property1)
                                sb.Append(valString)
                            Else
                                valString = GetMultiListValues(CType(ctl, ListBox))
                                sb.Append(valString)
                            End If
                        End If
                    Else
                        valString = GetControlValue(ctl, dc.Property1)
                        sb.Append(valString)
                    End If

                    If (parentAttr = "") Then
                        sectionString = GetAttribute(ctl, TypeString, "Section")
                    Else
                        sectionString = parentAttr
                    End If

                    fieldString = GetAttribute(ctl, TypeString, "Field")

                    lstPgCrtls.Add(New lodControl(ctl.ClientID, valString, sectionString, fieldString))
                End If

            Next
            Return sb.ToString()
        End Function

        Public Shared Function GetMultiListValues(ByVal ctl As Object) As String

            Dim sb As New System.Text.StringBuilder(200)
            Dim lstItem As ListItem
            Dim i As Integer
            ' Build a concatenated list of selected indices
            For Each lstItem In ctl.Items
                If lstItem.Selected = True Then
                    i = ctl.Items.IndexOf(lstItem)
                    sb.Append(lstItem.Text + ",")
                End If
            Next

            If (sb.Length > 0) Then
                sb = sb.Remove(sb.Length - 1, 1)
            End If

            Return sb.ToString()
        End Function

#End Region

#Region "Static Functions to be called from Pages "

        'Following Static Code is used to access Values
        '**************************************************
        'Code to get Hash Values
        Public Shared Function GetInitialHash(ByVal ctls As System.Web.UI.ControlCollection) As Integer
            ' Set the Initial Hash Value
            Dim InitialHash As Integer
            InitialHash = GetControls(ctls).GetHashCode()
            Return InitialHash
        End Function

        Public Shared Function HasChanged(ByVal ctls As System.Web.UI.ControlCollection, ByVal InitialHash As String) As Boolean

            If (InitialHash = "") Then Return False 'There was no initial Value so no mods

            Dim newHash As Integer
            ' Get the Ending Hash Value
            newHash = GetControls(ctls).GetHashCode()
            Return (Int32.Parse(InitialHash) <> newHash)
        End Function

        '**************************************************

#End Region

    End Class

End Namespace