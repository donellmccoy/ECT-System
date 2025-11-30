Imports System.Web

Namespace Common

#Region "Contol Class"

    Public Class ModControl

#Region "Private Variables"

        Private mboolIsMultiList As Boolean
        Private mstrProperty1 As String
        Private mstrProperty2 As String
        Private mstrPropertyToCheck As String
        Private mstrPropertyToCheckValue As String
        Private mstrType As String

#End Region

#Region "Public Properties"

        Public Property IsMultiList() As Boolean
            Get
                Return mboolIsMultiList
            End Get
            Set(ByVal Value As Boolean)
                mboolIsMultiList = Value
            End Set
        End Property

        Public Property Property1() As String
            Get
                Return mstrProperty1
            End Get
            Set(ByVal Value As String)
                mstrProperty1 = Value
            End Set
        End Property

        Public Property Property2() As String
            Get
                Return mstrProperty2
            End Get
            Set(ByVal Value As String)
                mstrProperty2 = Value
            End Set
        End Property

        Public Property PropertyToCheck() As String
            Get
                Return mstrPropertyToCheck
            End Get
            Set(ByVal Value As String)
                mstrPropertyToCheck = Value
            End Set
        End Property

        Public Property PropertyToCheckValue() As String
            Get
                Return mstrPropertyToCheckValue
            End Get
            Set(ByVal Value As String)
                mstrPropertyToCheckValue = Value
            End Set
        End Property

        Public Property Type() As String
            Get
                Return mstrType
            End Get
            Set(ByVal Value As String)
                mstrType = Value
            End Set
        End Property

        ' The rest of the property definitions go here

#End Region

    End Class

#End Region

#Region "Control Collections Class"

    Public Class ModControls
        Inherits System.Collections.ArrayList
        Private mstrType As String = String.Empty

#Region "Load Method"

        Public Sub LoadControls(ByVal FileName As String)
            Dim ds As New DataSet
            Dim dc As ModControl

            ds.ReadXml(FileName)
            For Each dr As DataRow In ds.Tables(0).Rows
                dc = New ModControl
                dc.Type = dr("Type").ToString()
                dc.Property1 = dr("Property1").ToString()
                dc.Property2 = dr("Property2").ToString()
                dc.IsMultiList = ToBoolean(dr("IsMultiList").ToString())
                dc.PropertyToCheck = dr("PropertyToCheck").ToString()
                dc.PropertyToCheckValue = dr("PropertyToCheckValue").ToString()
                Me.Add(dc)
            Next
        End Sub

        Private Function ToBoolean(ByVal value As String) As Boolean
            Dim boolRet As Boolean = False

            value = value.ToString().ToLower().Trim()

            If value = "yes" Then
                boolRet = True
            End If

            Return boolRet
        End Function

#End Region

#Region "Get Methods"

        Public Overridable Function GetByType(ByVal Type As String) As ModControl
            Dim dc As ModControl = Nothing

            mstrType = Type

            For Each dc In Me
                If dc.Type = mstrType Then Return dc
            Next
            dc = Nothing
            Return dc
        End Function

        Private Function FindByType(ByVal dc As ModControl) As Boolean
            If dc.Type = mstrType Then
                Return True
            Else
                Return False
            End If
        End Function

#End Region

    End Class

#End Region

#Region "Class for Loading Controls"

    Public Class ControlLoad

#Region "Private Shared Variables"

        Private Shared mControlsList As ModControls = Nothing

#End Region

#Region "Public Shared Properties"

        Public Shared Property mControlList() As ModControls
            Get
                If mControlsList Is Nothing Then
                    LoadMControls()
                End If
                Return mControlsList
            End Get
            Set(ByVal value As ModControls)
                mControlsList = value
            End Set
        End Property

#End Region

#Region "LoadMControls Method"

        Private Shared Sub LoadMControls()
            ' Load up the DirtyControls List
            mControlsList = New ModControls
            Dim strDir As String
            strDir = GetCurrentDirectory()
            mControlsList.LoadControls(strDir + "Script\ControlsXML.xml")

        End Sub

#End Region

#Region "GetCurrentDirectory Method"

        Public Shared Function GetCurrentDirectory() As String
            Dim strPath As String

            strPath = HttpContext.Current.Request.PhysicalApplicationPath
            Return strPath
        End Function

#End Region

    End Class

#End Region

End Namespace