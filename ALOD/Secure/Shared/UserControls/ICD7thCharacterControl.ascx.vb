Imports ALOD.Data
Imports ALODWebUtility.Common

Namespace Web.UserControls

    Public Class ICD7thCharacterControl
        Inherits System.Web.UI.UserControl

        Private icdUserControl As ICDCodeControl = Nothing
        Private isTransitioning As Boolean = False
        'Private saved7thCharacterId As String

        Public Property EnableAutoPostback As Boolean
            Get
                Return ddl7thCharacter.AutoPostBack
            End Get
            Set(value As Boolean)
                ddl7thCharacter.AutoPostBack = value
            End Set
        End Property

        Public Property Saved7thCharacter() As String
            Get
                Return ViewState("saved7thCharacter")
            End Get
            Set(ByVal value As String)
                ViewState("saved7thCharacter") = value
            End Set
        End Property

        Public ReadOnly Property Selected7thCharacter As String
            Get
                Return ddl7thCharacter.SelectedValue
            End Get
        End Property

        ''' <summary>
        ''' Disables the dropdownlist and makes it invisible. Makes the 7th character label visisble.
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub DisplayReadOnly()
            ddl7thCharacter.Enabled = False

            ddl7thCharacter.Visible = False
            lbl7thCharacter.Visible = True
        End Sub

        ''' <summary>
        ''' Enables the dropdownlist and makes it visible. Makes the 7th character label invisible.
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub DisplayReadWrite()
            If (ddl7thCharacter.Items.Count > 1) Then
                ddl7thCharacter.Enabled = True
            Else
                ddl7thCharacter.Enabled = False
            End If

            ddl7thCharacter.Visible = True
            lbl7thCharacter.Visible = False
        End Sub

        ''' <summary>
        ''' Hooks up the necessary components to allow the ICD7thCharacterControl user control to work with the ICDCodeControl user control.
        ''' </summary>
        ''' <param name="uc"></param>
        ''' <remarks></remarks>
        Public Sub Initialize(ByVal uc As ICDCodeControl)
            AddHandler uc.ICDCodeSelectionChanged, AddressOf DataBindDDLEventHandler
            icdUserControl = uc
        End Sub

        ''' <summary>
        ''' Binds data to the 7th character dropdownlist (ddl) and enables or disables the ddl based on the ID of the ICD code passed into the method.
        ''' </summary>
        ''' <param name="icdCodeId"></param>
        ''' <remarks></remarks>
        Public Sub InitializeCharacters(ByVal icdCodeId As Integer, ByVal characterId As String)
            If (icdCodeId > 0) Then
                ddl7thCharacter.Enabled = True
            Else
                ddl7thCharacter.Enabled = False
            End If

            Saved7thCharacter = characterId

            DataBindDDL(icdCodeId)

            If (ddl7thCharacter.Items.Count > 0 AndAlso ddl7thCharacter.Items.FindByValue(Saved7thCharacter) IsNot Nothing) Then
                ddl7thCharacter.SelectedValue = Saved7thCharacter
            Else
                ddl7thCharacter.SelectedIndex = 0
            End If
        End Sub

        ''' <summary>
        ''' Returns whether or not an ICD code has been selected in the last ICD dropdown listbox.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Is7thCharacterSelected() As Boolean
            Return (Not String.IsNullOrEmpty(ddl7thCharacter.SelectedValue))
        End Function

        ''' <summary>
        ''' Updates the 7th character label with the description of the specified 7th character ID.
        ''' </summary>
        ''' <param name="codeId"></param>
        ''' <param name="character"></param>
        ''' <remarks></remarks>
        Public Sub Update7thCharacterLabel(ByVal codeId As Integer, ByVal character As String)
            If (IsNothing(codeId) OrElse codeId = 0 OrElse String.IsNullOrEmpty(character)) Then
                Exit Sub
            End If

            lbl7thCharacter.Text = ""

            Dim items As System.Data.DataSet = Nothing
            Dim icdDao As ICD9CodeDao = New NHibernateDaoFactory().GetICD9CodeDao()

            items = icdDao.Get7thCharacters(codeId)

            If (items IsNot Nothing AndAlso items.Tables.Count > 0) Then
                For Each row As DataRow In items.Tables(0).Rows
                    If (row("Character").ToString.Equals(character)) Then
                        lbl7thCharacter.Text = row("Definition").ToString()
                    End If
                Next
            End If
        End Sub

        ''' <summary>
        ''' Handles the automatic updating of the contents of the 7th character dropdownlist.
        ''' </summary>
        ''' <param name="sender"></param>
        ''' <param name="e"></param>
        ''' <remarks></remarks>
        Protected Sub DataBindDDLEventHandler(ByVal sender As Object, ByVal e As ICDCodeSelectedEventArgs)
            DataBindDDL(e.SelectedICDCodeId)

            If (ddl7thCharacter.Items.Count > 0 AndAlso ddl7thCharacter.Items.FindByValue(Saved7thCharacter) IsNot Nothing) Then
                ddl7thCharacter.SelectedValue = Saved7thCharacter
            Else
                ddl7thCharacter.SelectedIndex = 0
            End If
        End Sub

        Protected Sub ddl7thCharacter_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles ddl7thCharacter.PreRender
            If (ddl7thCharacter.Items.Count > 0 AndAlso ddl7thCharacter.Items.FindByValue(Saved7thCharacter) IsNot Nothing) Then
                'ddl7thCharacter.SelectedValue = Saved7thCharacter
            End If
        End Sub

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        End Sub

        ''' <summary>
        ''' Queries the database for a dataset and binds the resulting dataset to the 7th character dropdownlist.
        ''' </summary>
        ''' <param name="icdCodeId"></param>
        ''' <remarks></remarks>
        Private Sub DataBindDDL(ByVal icdCodeId As Integer)
            Dim items As System.Data.DataSet = Nothing

            If (icdCodeId > 0) Then
                Dim icdDao As ICD9CodeDao = New NHibernateDaoFactory().GetICD9CodeDao()
                items = icdDao.Get7thCharacters(icdCodeId)
            End If

            ddl7thCharacter.Items.Clear()
            ddl7thCharacter.SelectedValue = Nothing
            ddl7thCharacter.DataTextField = "Definition"
            ddl7thCharacter.DataValueField = "Character"
            ddl7thCharacter.DataSource = items
            ddl7thCharacter.DataBind()

            If (ddl7thCharacter.Items.FindByText("-- Select 7th Character --") Is Nothing) Then
                ddl7thCharacter.Items.Insert(0, New ListItem("-- Select 7th Character --", ""))
            End If

            If (ddl7thCharacter.Items.Count > 1) Then
                ddl7thCharacter.Enabled = True
            Else
                ddl7thCharacter.Enabled = False
            End If
        End Sub

    End Class

End Namespace