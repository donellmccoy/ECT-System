Imports System.Data.Common
Imports ALOD.Core.Domain.Users
Imports ALOD.Data

Namespace Common

    <Serializable()>
    Public Class pascode

#Region "OldPasCodeDefinition"

        Protected _canEdit As Boolean = False
        Protected _description As String = String.Empty
        Protected _designator As String = String.Empty
        Protected _installation As String = String.Empty
        Protected _majcom As String = String.Empty
        Protected _organization As String = String.Empty
        Protected _orgnKind As String = String.Empty

        'Service Plan Activity
        Protected _parent As String = String.Empty

        Protected _pasCode As String
        Protected _spa As String = String.Empty
        Protected _status As AccessStatus = AccessStatus.Pending
        Protected _unit As String = String.Empty

#Region "PropertiesOld"

        Public Property CanEdit() As Boolean
            Get
                Return _canEdit
            End Get
            Set(ByVal value As Boolean)
                _canEdit = value
            End Set
        End Property

        Public Property Description() As String
            Get
                Return _description
            End Get
            Set(ByVal value As String)
                _description = value
            End Set
        End Property

        Public Property Designator() As String
            Get
                Return _designator
            End Get
            Set(ByVal value As String)
                _designator = value
            End Set
        End Property

        Public Property Installation() As String
            Get
                Return _installation
            End Get
            Set(ByVal value As String)
                _installation = value
            End Set
        End Property

        Public Property MAJCOM() As String
            Get
                Return _majcom
            End Get
            Set(ByVal value As String)
                _majcom = value
            End Set
        End Property

        Public Property Organization() As String
            Get
                Return _organization
            End Get
            Set(ByVal value As String)
                _organization = value
            End Set
        End Property

        Public Property OrgnKind() As String
            Get
                Return _orgnKind
            End Get
            Set(ByVal value As String)
                _orgnKind = value
            End Set
        End Property

        Public Property Parent() As String
            Get
                Return _parent
            End Get
            Set(ByVal value As String)
                _parent = value
            End Set
        End Property

        Public Property SPA() As String
            Get
                Return _spa
            End Get
            Set(ByVal value As String)
                _spa = value
            End Set
        End Property

        Public Property Status() As AccessStatus
            Get
                Return _status
            End Get
            Set(ByVal value As AccessStatus)
                _status = value
            End Set
        End Property

        Public Property Unit() As String
            Get
                Return _unit
            End Get
            Set(ByVal value As String)
                _unit = value
            End Set
        End Property

        Public Property Value() As String
            Get
                Return _pasCode
            End Get
            Set(ByVal value As String)
                If (value.Trim.Length = 8) Then
                    _pasCode = value.Substring(4, 4)
                Else
                    _pasCode = value
                End If
            End Set
        End Property

#End Region

#Region "Constructors"

        Public Sub New()

        End Sub

        Public Sub New(ByVal pCode As pascode)
            _pasCode = pCode.Value
            _description = pCode.Description
            _status = pCode.Status
            _canEdit = pCode.CanEdit

            _spa = pCode.SPA
            _majcom = pCode.MAJCOM
            _designator = pCode.Designator
            _unit = pCode.Unit
            _organization = pCode.Organization  'Service Plan Activity
            _installation = pCode.Installation
            _orgnKind = pCode.OrgnKind
            _parent = pCode.Parent
            _orgnKind = pCode.OrgnKind
            _uic = pCode.uic
        End Sub

        Public Sub New(ByVal pasCode As String)
            _pasCode = pasCode
        End Sub

        Public Sub New(ByVal strpasCode As String, ByVal status As AccessStatus, ByVal canEdit As Boolean)
            _status = status
            _pasCode = strpasCode
            _canEdit = canEdit
        End Sub

        Public Sub New(ByVal strpasCode As String, ByVal status As AccessStatus)
            _status = status
            _pasCode = strpasCode
        End Sub

        Public Function IsValidPascode(ByVal pascode As String) As Boolean
            Dim adapter As New SqlDataStore()
            Dim cmd As System.Data.Common.DbCommand = adapter.GetStoredProcCommand("core_pascode_sp_Validation")
            adapter.AddInParameter(cmd, "@pascode", Data.DbType.String, pascode)
            adapter.AddOutParameter(cmd, "@valid", Data.DbType.Boolean, 0)
            adapter.ExecuteNonQuery(cmd)
            Return CType(cmd.Parameters("@valid").Value, Boolean)

        End Function

#End Region

#End Region

#Region "NewPasCodeDefinition"

#Region "Members"

        Protected _address1 As String = String.Empty

        Protected _address2 As String = String.Empty

        Protected _base_code As String = String.Empty

        Protected _city As String = String.Empty

        Protected _component As String = String.Empty

        Protected _country As String = String.Empty

        '--These are the properties
        Protected _cs_id As Int32

        'These need to be initialized to these values since 0 correspoonds to an existing unit
        Protected _cs_id_parent As Int32 = -1

        Protected _cs_level As String = String.Empty
        Protected _cs_oper_type As String = String.Empty
        Protected _email As String = String.Empty
        Protected _gaining_command_cs_id As Int32 = -1
        Protected _long_name As String = String.Empty
        Protected _pas_code As String = String.Empty
        Protected _postal_code As String = String.Empty
        Protected _reportingStructure As Dictionary(Of String, Int32)
        Protected _state As String = String.Empty
        Protected _time_zone As String = String.Empty
        Protected _uic As String = String.Empty
        Protected _unit_det As String = String.Empty
        Protected _unit_kind As String = String.Empty
        Protected _unit_nbr As String = String.Empty
        Protected _unit_type As String = String.Empty

#End Region

#Region "Properties"

        Public Property address1() As String
            Get
                Return _address1
            End Get
            Set(ByVal value As String)
                _address1 = value
            End Set
        End Property

        Public Property address2() As String
            Get
                Return _address2
            End Get
            Set(ByVal value As String)
                _address2 = value
            End Set
        End Property

        Public Property base_code() As String
            Get
                Return _base_code
            End Get
            Set(ByVal value As String)
                _base_code = value
            End Set
        End Property

        Public Property city() As String
            Get
                Return _city
            End Get
            Set(ByVal value As String)
                _city = value
            End Set
        End Property

        Public Property component() As String
            Get
                Return _component
            End Get
            Set(ByVal value As String)
                _component = value
            End Set
        End Property

        Public Property country() As String
            Get
                Return _country
            End Get
            Set(ByVal value As String)
                _country = value
            End Set
        End Property

        Public Property cs_id() As Integer
            Get
                Return _cs_id
            End Get
            Set(ByVal value As Integer)
                _cs_id = value
            End Set
        End Property

        Public Property cs_id_parent() As Integer
            Get
                Return _cs_id_parent
            End Get
            Set(ByVal value As Integer)
                _cs_id_parent = value
            End Set
        End Property

        Public Property cs_level() As String
            Get
                Return _cs_level
            End Get
            Set(ByVal value As String)
                _cs_level = value
            End Set
        End Property

        Public Property cs_oper_type() As String
            Get
                Return _cs_oper_type
            End Get
            Set(ByVal value As String)
                _cs_oper_type = value
            End Set
        End Property

        Public Property Email() As String
            Get
                Return _email
            End Get
            Set(ByVal value As String)
                _email = value
            End Set
        End Property

        Public Property gaining_command_cs_id() As Integer
            Get
                Return _gaining_command_cs_id
            End Get
            Set(ByVal value As Integer)
                _gaining_command_cs_id = value
            End Set
        End Property

        Public Property long_name() As String
            Get
                Return _long_name
            End Get
            Set(ByVal value As String)
                _long_name = value
            End Set
        End Property

        Public Property pas_code() As String
            Get
                Return _pas_code
            End Get
            Set(ByVal value As String)
                _pas_code = value
            End Set
        End Property

        Public Property postal_code() As String
            Get
                Return _postal_code
            End Get
            Set(ByVal value As String)
                _postal_code = value
            End Set
        End Property

        Public Property reportingStructure() As Dictionary(Of String, Int32)
            Get
                If _reportingStructure Is Nothing Then
                    _reportingStructure = New Dictionary(Of String, Int32)
                End If
                Return _reportingStructure
            End Get
            Set(ByVal value As Dictionary(Of String, Int32))
                _reportingStructure = value
            End Set
        End Property

        Public Property state() As String
            Get
                Return _state
            End Get
            Set(ByVal value As String)
                _state = value
            End Set
        End Property

        Public Property time_zone() As String
            Get
                Return _time_zone
            End Get
            Set(ByVal value As String)
                _time_zone = value
            End Set
        End Property

        Public Property uic() As String
            Get
                Return _uic
            End Get
            Set(ByVal value As String)
                _uic = value
            End Set
        End Property

        Public Property unit_det() As String
            Get
                Return _unit_det
            End Get
            Set(ByVal value As String)
                _unit_det = value
            End Set
        End Property

        Public Property unit_kind() As String
            Get
                Return _unit_kind
            End Get
            Set(ByVal value As String)
                _unit_kind = value
            End Set
        End Property

        Public Property unit_nbr() As String
            Get
                Return _unit_nbr
            End Get
            Set(ByVal value As String)
                _unit_nbr = value
            End Set
        End Property

        Public Property unit_type() As String
            Get
                Return _unit_type
            End Get
            Set(ByVal value As String)
                _unit_type = value
            End Set
        End Property

#End Region

#Region "Constructor"

        Public Sub New(ByVal csid As Integer)
            _cs_id = csid
            _reportingStructure = New Dictionary(Of String, Int32)

        End Sub

#End Region

#Region "Load"

        Public Sub LoadPasCode()

            ' Do not use Iif (evaluates both results [Then and Else] before the conditional [If]) - which causes errors
            Dim unitDao As UnitDao = New NHibernateDaoFactory().GetUnitDao()
            Dim pCodeInfo As Unit = unitDao.GetById(cs_id)
            If (pCodeInfo.Name Is Nothing) Then
                long_name = ""
            Else
                long_name = pCodeInfo.Name
            End If
            If (pCodeInfo.UnitNumber Is Nothing) Then
                unit_nbr = ""
            Else
                unit_nbr = pCodeInfo.UnitNumber
            End If
            If (pCodeInfo.UnitKind Is Nothing) Then
                unit_kind = ""
            Else
                unit_kind = pCodeInfo.UnitKind
            End If
            If (pCodeInfo.UnitType Is Nothing) Then
                unit_type = ""
            Else
                unit_type = pCodeInfo.UnitType
            End If
            If (pCodeInfo.UnitDet Is Nothing) Then
                unit_det = ""
            Else
                unit_det = pCodeInfo.UnitDet
            End If
            If (pCodeInfo.Uic Is Nothing) Then
                uic = ""
            Else
                uic = pCodeInfo.Uic
            End If
            If (pCodeInfo.CommandStructLevel Is Nothing) Then
                cs_level = ""
            Else
                cs_level = pCodeInfo.CommandStructLevel
            End If
            If (pCodeInfo.ParentUnit Is Nothing) Then
                cs_id_parent = -1
            Else
                cs_id_parent = pCodeInfo.ParentUnit.Id
            End If
            If (pCodeInfo.GainingCommand Is Nothing) Then
                gaining_command_cs_id = -1
            Else
                gaining_command_cs_id = pCodeInfo.GainingCommand.Id
            End If
            If (pCodeInfo.PasCode Is Nothing) Then
                pas_code = ""
            Else
                pas_code = pCodeInfo.PasCode
            End If
            If (pCodeInfo.BaseCode Is Nothing) Then
                base_code = ""
            Else
                base_code = pCodeInfo.BaseCode
            End If
            If (pCodeInfo.CommandStructOperationType Is Nothing) Then
                cs_oper_type = ""
            Else
                cs_oper_type = pCodeInfo.CommandStructOperationType
            End If
            If (pCodeInfo.TimeZone Is Nothing) Then
                time_zone = ""
            Else
                time_zone = pCodeInfo.TimeZone
            End If
            If (pCodeInfo.Component Is Nothing) Then
                component = ""
            Else
                component = pCodeInfo.Component
            End If
            If (pCodeInfo.Address1 Is Nothing) Then
                address1 = ""
            Else
                address1 = pCodeInfo.Address1
            End If
            If (pCodeInfo.Address2 Is Nothing) Then
                address2 = ""
            Else
                address2 = pCodeInfo.Address2
            End If
            If (pCodeInfo.City Is Nothing) Then
                city = ""
            Else
                city = pCodeInfo.City
            End If
            If (pCodeInfo.State Is Nothing) Then
                state = ""
            Else
                state = pCodeInfo.State
            End If
            If (pCodeInfo.PostalCode Is Nothing) Then
                postal_code = ""
            Else
                postal_code = pCodeInfo.PostalCode
            End If
            If (pCodeInfo.Country Is Nothing) Then
                country = ""
            Else
                country = pCodeInfo.Country
            End If
            If (pCodeInfo.Email Is Nothing) Then
                Email = ""
            Else
                Email = pCodeInfo.Email
            End If

        End Sub

#End Region

#Region "Update"

        Public Sub UpdatePasCode()

            Dim adapter As New SqlDataStore
            Dim dbCommand As DbCommand

            dbCommand = adapter.GetStoredProcCommand("core_pascode_sp_update")

            adapter.AddInParameter(dbCommand, "@CS_ID", DbType.Int32, cs_id)
            If long_name <> "" Then adapter.AddInParameter(dbCommand, "@LONG_NAME", DbType.String, long_name)
            If unit_nbr <> "" Then adapter.AddInParameter(dbCommand, "@UNIT_NBR", DbType.String, unit_nbr)
            If unit_kind <> "" Then adapter.AddInParameter(dbCommand, "@UNIT_KIND", DbType.String, unit_kind)
            If unit_type <> "" Then adapter.AddInParameter(dbCommand, "@UNIT_TYPE", DbType.String, unit_type)
            If unit_det <> "" Then adapter.AddInParameter(dbCommand, "@UNIT_DET", DbType.String, unit_det)
            If uic <> "" Then adapter.AddInParameter(dbCommand, "@UIC", DbType.String, uic)
            If cs_level <> "" Then adapter.AddInParameter(dbCommand, "@CS_LEVEL", DbType.String, cs_level)
            If cs_id_parent <> -1 Then adapter.AddInParameter(dbCommand, "@CS_ID_PARENT", DbType.Int32, cs_id_parent)
            If gaining_command_cs_id <> -1 Then adapter.AddInParameter(dbCommand, "@GAINING_COMMAND_CS_ID", DbType.Int32, gaining_command_cs_id)
            If pas_code <> "" Then adapter.AddInParameter(dbCommand, "@PAS_CODE", DbType.String, pas_code)
            If base_code <> "" Then adapter.AddInParameter(dbCommand, "@BASE_CODE", DbType.String, base_code)
            If cs_oper_type <> "" Then adapter.AddInParameter(dbCommand, "@CS_OPER_TYPE", DbType.String, cs_oper_type)
            If time_zone <> "" Then adapter.AddInParameter(dbCommand, "@TIME_ZONE", DbType.String, time_zone)
            If component <> "" Then adapter.AddInParameter(dbCommand, "@COMPONENT", DbType.String, component)
            If address1 <> "" Then adapter.AddInParameter(dbCommand, "@ADDRESS1", DbType.String, address1)
            If address2 <> "" Then adapter.AddInParameter(dbCommand, "@ADDRESS2", DbType.String, address2)
            If city <> "" Then adapter.AddInParameter(dbCommand, "@CITY", DbType.String, city)
            If country <> "" Then adapter.AddInParameter(dbCommand, "@COUNTRY", DbType.String, country)
            If state <> "" Then adapter.AddInParameter(dbCommand, "@STATE", DbType.String, state)
            If postal_code <> "" Then adapter.AddInParameter(dbCommand, "@POSTAL_CODE", DbType.String, postal_code)
            If Email <> "" Then adapter.AddInParameter(dbCommand, "@E_MAIL", DbType.String, Email)

            adapter.ExecuteNonQuery(dbCommand)

        End Sub

        Public Sub UpdateReporting(ByVal userId As Integer)

            Dim xml As New XMLString("ReportList")
            For Each entry As KeyValuePair(Of String, Int32) In reportingStructure

                xml.BeginElement("command")
                xml.WriteAttribute("cs_id", cs_id)
                xml.WriteAttribute("chain_type", entry.Key)
                xml.WriteAttribute("parent_cs_id", entry.Value)
                xml.EndElement()

            Next

            Dim adapter As New SqlDataStore
            adapter.ExecuteNonQuery("core_pascode_sp_updateReporting", userId, xml.ToString())

        End Sub

#End Region

#End Region

    End Class

End Namespace