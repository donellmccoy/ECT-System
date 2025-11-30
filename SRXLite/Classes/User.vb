Option Strict On

Imports System.Web.Services.Protocols
Imports System.Data.SqlClient
Imports SRXLite.DataAccess
Imports SRXLite.Modules

Namespace Classes

    ''' <summary>
    '''
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ServiceLogin
        Inherits SoapHeader

        Public Password As String
        Public SubuserName As String
        Public UserName As String
    End Class

    ''' <summary>
    '''
    ''' </summary>
    ''' <remarks></remarks>
    Public Class ServiceUser
        Inherits User

        Private _authenticated As Boolean = False

        Public Sub New()
        End Sub

#Region " Properties "

        Public ReadOnly Property IsAuthenticated() As Boolean
            Get
                Return _authenticated
            End Get
        End Property

#End Region

#Region " Authenticate "

        ''' <summary>
        ''' Validates the user's credentials (username/password).  Throws an
        ''' UnauthorizedAccessException if authentication fails.
        ''' </summary>
        ''' <param name="login"></param>
        ''' <remarks></remarks>
        Public Sub Authenticate(ByVal login As ServiceLogin)
            If _authenticated Then Exit Sub

            If IsNothing(login) OrElse IsNothing(login.UserName) OrElse IsNothing(login.Password) Then
                Throw New UnauthorizedAccessException("Login information is missing.")
            End If

            'Validate username/password -----------------------
            'TODO: store password securely with hash/salt?
            Dim command As New SqlCommand
            command.CommandText = "dsp_User_Authenticate"
            command.CommandType = CommandType.StoredProcedure
            With command.Parameters
                .Add(getSqlParameter("@UserName", login.UserName))
                .Add(getSqlParameter("@Password", login.Password))
                .Add(getSqlParameter("@SubuserName", login.SubuserName))
                .Add(getSqlParameter("@UserID", Nothing, SqlDbType.SmallInt, ParameterDirection.Output))
                .Add(getSqlParameter("@SubuserID", Nothing, SqlDbType.Int, ParameterDirection.Output))
            End With

            DB.ExecuteNonQuery(command)

            Dim userID As Short = ShortCheck(command.Parameters.Item("@UserID").Value)
            Dim subuserID As Integer = IntCheck(command.Parameters.Item("@SubuserID").Value)

            'Check for valid userID ---------------------------
            If userID = 0 Then
                Throw New UnauthorizedAccessException(String.Format("Login failed for user ""{0}"".", login.UserName))
            End If

            'User validated, set properties -------------------
            Me._userID = userID
            Me._userName = login.UserName
            Me._authenticated = True
            Me._subuserID = subuserID
            Me._subuserName = login.SubuserName
        End Sub

#End Region

    End Class

    ''' <summary>
    '''
    ''' </summary>
    ''' <remarks></remarks>
    Public Class User

        Protected _subuserID As Integer
        Protected _subuserName As String
        Protected _userID As Short
        Protected _userName As String

#Region " Properties "

        Public ReadOnly Property SubuserID() As Integer
            Get
                Return _subuserID
            End Get
        End Property

        Public ReadOnly Property SubuserName() As String
            Get
                Return _subuserName
            End Get
        End Property

        Public ReadOnly Property UserID() As Short
            Get
                Return _userID
            End Get
        End Property

        Public ReadOnly Property UserName() As String
            Get
                Return _userName
            End Get
        End Property

#End Region

#Region " Constructors "

        Public Sub New()
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="userID"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal userID As Short)
            _userID = userID
        End Sub

        ''' <summary>
        '''
        ''' </summary>
        ''' <param name="userID"></param>
        ''' <param name="subuserID"></param>
        ''' <remarks></remarks>
        Public Sub New(ByVal userID As Short, ByVal subuserID As Integer)
            _userID = userID
            _subuserID = subuserID
        End Sub

#End Region

    End Class

End Namespace