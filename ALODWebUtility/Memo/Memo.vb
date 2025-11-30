Imports System.Collections.Specialized
Imports System.Data.Common
Imports System.Drawing
Imports System.Text
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Data
Imports ALOD.Data.Services
Imports ALODWebUtility.Printing

Namespace Memo

    Public Class Memo

#Region "Members/Properties"

        Protected _adapter As SqlDataStore
        Protected _addDate As Boolean = True
        Protected _addSignature As Boolean = True
        Protected _addSuspenseDate As Boolean = False
        Protected _approvalRequired As Boolean = False
        Protected _authorGroup As Byte
        Protected _authorId As Integer
        Protected _content As String = String.Empty
        Protected _createdDate As Date
        Protected _currentPage As Short = 0
        Protected _dataKey As Integer = 0
        Protected _dataProc As String = String.Empty
        Protected _deleted As Boolean = False
        Protected _description As String = String.Empty
        Protected _isTemplate As Boolean = False
        Protected _lastEdit As Date
        Protected _letterHead As Int16 = 0
        Protected _letterId As Integer = 0
        Protected _mailingAddress As String = String.Empty
        Protected _memoDate As String = String.Empty
        Protected _officeSymbol As String = String.Empty
        Protected _pageOffset As Short = 0
        Protected _phi As Boolean = False
        Protected _refId As Integer = 0
        Protected _scope As Byte
        Protected _secondaryId As Integer = 0
        Protected _sigBlock As String = String.Empty
        Protected _state As String = String.Empty
        Protected _suspenseDate As String = String.Empty
        Protected _templateData As StringDictionary
        Protected _templateFileName As String = "Letterhead-1.pdf"
        Protected _templateId As Integer = 0
        Protected _viewRestricted As Boolean = False
        Private Const DATE_FORMAT As String = "d MMMM yyyy"
        Private Const LINE_COUNT As Integer = 34
        Private Const LINE_WIDTH As Single = 809.55
        Private _user As AppUser = Nothing

        Private SigBlockOffset As String = Space(71)

        Public Property AccessScope() As Byte
            Get
                Return _scope
            End Get
            Set(ByVal value As Byte)
                _scope = value
            End Set
        End Property

        Public ReadOnly Property AddDate() As Boolean
            Get
                Return _addDate
            End Get
        End Property

        Public ReadOnly Property AddSignature() As Boolean
            Get
                Return _addSignature
            End Get
        End Property

        Public ReadOnly Property AddSuspenseDate() As Boolean
            Get
                Return _addSuspenseDate
            End Get
        End Property

        Public Property ApprovalRequired() As Boolean
            Get
                Return _approvalRequired
            End Get
            Set(ByVal value As Boolean)
                _approvalRequired = value
            End Set
        End Property

        Public Property AuthorGroup() As Byte
            Get
                Return _authorGroup
            End Get
            Set(ByVal value As Byte)
                _authorGroup = value
            End Set
        End Property

        Public Property AuthorId() As Integer
            Get
                If (_authorId = 0) Then
                    Return CurrentUser.Id
                End If
                Return _authorId
            End Get
            Set(ByVal value As Integer)
                _authorId = value
            End Set
        End Property

        Public Property Content() As String
            Get
                Return _content
            End Get
            Set(ByVal value As String)
                _content = value
            End Set
        End Property

        Public ReadOnly Property ContentLength() As Integer
            Get
                Return FormattedContent.Length
            End Get
        End Property

        Public ReadOnly Property CurrentPage() As Short
            Get
                Return _currentPage
            End Get
        End Property

        Public Property DataKey() As Integer
            Get
                If (_dataKey = 0) Then
                    Return _refId
                End If
                Return _dataKey
            End Get
            Set(ByVal value As Integer)
                _dataKey = value
            End Set
        End Property

        Public Property DateCreated() As Date
            Get
                Return _createdDate
            End Get
            Set(ByVal value As Date)
                _createdDate = value
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

        Public ReadOnly Property FormatedSignatureBlock() As String
            Get
                'if this is not from a template, load whatever we have
                If (Not _isTemplate) Then
                    Return _sigBlock
                End If

                'otherwise, format the sigblock
                Dim signature As String = _sigBlock

                If (Not _approvalRequired) Then
                    'Update the signature block to show that a letter has been signed
                    If signature <> "" AndAlso
                     signature.IndexOf("<< Digitally Signed by ") = -1 AndAlso
                     signature.IndexOf("<< Signed by ") = -1 Then

                        'Add the signature block to the end of the letter
                        signature = "<< Signed by " & CurrentUser.FullName & " >>" & vbCrLf & signature
                    End If
                End If

                Return signature
            End Get
        End Property

        Public ReadOnly Property FormattedContent() As String
            Get
                Return RTrim(Content & RepeatStr(vbCrLf, 5) & SigBlockOffset & SignatureBlock.Replace(vbCrLf, vbCrLf & SigBlockOffset))
            End Get
        End Property

        Public ReadOnly Property IsSigned() As Boolean
            Get
                If (_sigBlock.Length > 0) Then
                    If (_sigBlock.IndexOf("<< Digitally Signed by ") <> -1) OrElse (_sigBlock.IndexOf("<< Signed by ") <> -1) Then
                        Return True
                    End If
                End If
                Return False
            End Get
        End Property

        Public ReadOnly Property IsTemplate() As Boolean
            Get
                Return _isTemplate
            End Get
        End Property

        Public Property LastEditDate() As Date
            Get
                Return _lastEdit
            End Get
            Set(ByVal value As Date)
                _lastEdit = value
            End Set
        End Property

        Public Property LetterId() As Integer
            Get
                Return _letterId
            End Get
            Set(ByVal value As Integer)
                _letterId = value
            End Set
        End Property

        Public Property MailingAddress() As String
            Get
                Return _mailingAddress
            End Get
            Set(ByVal value As String)
                _mailingAddress = value
            End Set
        End Property

        Public Property MemoDate() As String
            Get
                'if this is a letter always return what we have
                If (_letterId <> 0) Then
                    Return _memoDate
                End If

                'otherwise, this is a template
                If (_addDate) Then
                    Return Now.ToString(DATE_FORMAT)
                End If

                'otherwise, return nothing
                Return ""
            End Get

            Set(ByVal value As String)
                _memoDate = value
            End Set
        End Property

        Public Property OfficeSymbol() As String
            Get
                Return _officeSymbol
            End Get
            Set(ByVal value As String)
                _officeSymbol = value
            End Set
        End Property

        Public Property Phi() As Boolean
            Get
                Return _phi
            End Get
            Set(ByVal value As Boolean)
                _phi = value
            End Set
        End Property

        Public Property RefId() As Integer
            Get
                Return _refId
            End Get
            Set(ByVal value As Integer)
                _refId = value
            End Set
        End Property

        Public Property SecondaryId() As Integer
            Get
                Return _secondaryId
            End Get
            Set(ByVal value As Integer)
                _secondaryId = value
            End Set
        End Property

        Public Property SignatureBlock() As String
            Get
                Return _sigBlock
            End Get
            Set(ByVal value As String)
                _sigBlock = value
            End Set
        End Property

        Public ReadOnly Property State() As String
            Get
                Return _state
            End Get
        End Property

        Public Property SuspenseDate() As String
            Get
                'if this is a letter always return what we have
                If (_letterId <> 0) Then
                    Return _suspenseDate
                End If

                'otherwise, this is a template
                If (_addSuspenseDate) Then
                    Return Now.AddDays(30).ToString(DATE_FORMAT)
                End If

                'otherwise, return nothing
                Return ""
            End Get

            Set(ByVal value As String)
                _suspenseDate = value
            End Set
        End Property

        Public Property TemplateFileName() As String
            Get
                Return _templateFileName
            End Get
            Set(ByVal value As String)
                _templateFileName = value
            End Set
        End Property

        Public Property TemplateId() As Integer
            Get
                Return _templateId
            End Get
            Set(ByVal value As Integer)
                _templateId = value
            End Set
        End Property

        Public Property ViewRestricted() As Boolean
            Get
                Return _viewRestricted
            End Get
            Set(ByVal value As Boolean)
                _viewRestricted = value
            End Set
        End Property

        'use the default in case one is not found
        Protected ReadOnly Property DataStore() As SqlDataStore
            Get
                If (_adapter Is Nothing) Then
                    _adapter = New SqlDataStore()
                End If

                Return _adapter
            End Get
        End Property

        Private ReadOnly Property CurrentUser() As AppUser
            Get
                If (_user Is Nothing) Then
                    _user = UserService.CurrentUser()
                End If
                Return _user
            End Get
        End Property

#End Region

        Public ReadOnly Property PageContent() As String
            Get
                Dim part As String = ""

                Dim font As New Font("Arial", 12)
                Dim bmp As New Bitmap(100, 100)
                Dim gfx As System.Drawing.Graphics
                gfx = Graphics.FromImage(bmp)

                Dim format As New StringFormat
                format.FormatFlags = StringFormatFlags.LineLimit

                Dim rect As New SizeF(LINE_WIDTH, font.GetHeight(gfx) * LINE_COUNT)
                Dim chars As Integer = 0
                Dim lines As Integer = 0

                'start with what we have left
                Dim lengthLeft As Integer = ContentLength - _pageOffset
                part = FormattedContent.Substring(_pageOffset, lengthLeft)

                'now determine how much of what we have left will fit on one page
                Dim actual As SizeF = gfx.MeasureString(part, font, rect, format, chars, lines)
                part = FormattedContent.Substring(_pageOffset, chars)
                _pageOffset += chars

                If (lengthLeft > chars) Then
                    'we now know how many chars will fit, but that is probably mid-word, so find the end
                    'of the preceding sentence and break there.
                    Dim index As Integer = part.LastIndexOfAny(New Char() {".", "?", "!"})

                    If index <> -1 Then
                        _pageOffset = _pageOffset - (part.Length - index - 1)
                        part = part.Substring(0, index + 1)
                    End If
                End If

                Return part
            End Get

        End Property

        Public Sub LoadCoverLetter(ByVal key As Integer, ByVal type As ModuleType)

            _refId = key

        End Sub

        Public Sub LoadCurrentLetterByTemplate(ByVal refId As Integer, ByVal templateId As Integer)

            _refId = refId
            _templateId = templateId
            _isTemplate = False

            Dim cmd As DbCommand
            cmd = DataStore.GetStoredProcCommand("core_memo_sp_GetCurrentMemoByTemplateId")
            DataStore.AddInParameter(cmd, "@refId", Data.DbType.Int32, _refId)
            DataStore.AddInParameter(cmd, "@template", Data.DbType.Byte, _templateId)
            DataStore.AddOutParameter(cmd, "@letterId", Data.DbType.Int32, 32)

            DataStore.ExecuteReader(AddressOf MemoReader, cmd)

            If (Not IsDBNull(cmd.Parameters("@letterId").Value)) Then
                _letterId = CInt(cmd.Parameters("@letterId").Value)
            End If

        End Sub

        Public Sub LoadLetter(ByVal letterId As Integer)

            _letterId = letterId
            DataStore.ExecuteReader(AddressOf MemoReader, "core_memo_sp_GetMemoById", _letterId)

        End Sub

        Public Sub LoadTemplate(ByVal refId As Integer, ByVal templateId As Integer, ByVal dataKey As Integer)

            _refId = refId
            _templateId = templateId
            _isTemplate = True
            _dataKey = dataKey

            DataStore.ExecuteReader(AddressOf MemoReader, "core_memo_sp_GetTemplateById", refId, templateId, CurrentUser.CurrentRole.Group.Id)

            PopulateData()

        End Sub

        Public Function NextPage() As Boolean
            If (_pageOffset < ContentLength) Then
                _currentPage += 1
                Return True
            End If

            Return False
        End Function

        Public Function Save() As Boolean

            _letterId = CInt(DataStore.ExecuteScalar("core_memo_sp_InsertMemo",
                _refId, _templateId, _letterId, CurrentUser.Id, _mailingAddress, _officeSymbol,
                _memoDate, _suspenseDate, _content, FormatedSignatureBlock))

            Return _letterId <> 0

        End Function

        Public Sub SetField(ByVal key As String, ByVal value As String)
            _content = _content.Replace("{" + key.ToUpper + "}", value)
        End Sub

        Public Function ToPdf(Optional ByVal isPreview As Boolean = False) As PDFDocument

            Dim form As PDFForm
            Dim pages As New List(Of PDFForm)
            Dim doc As New PDFDocument()
            Dim ctField As String = "Content"

            If (isPreview) Then
                doc.WaterMark = "PREVIEW DOCUMENT"
            End If

            If (_phi) Then
                doc.WaterMark = "Protected Health Information"
            End If

            While (Me.NextPage())

                form = New PDFForm(TemplateFileName)

                If (_currentPage = 1) Then
                    'add our header info to the first page
                    form.SetField("Address", MailingAddress)

                    If (OfficeSymbol.Length > 0) Then
                        form.SetField("OfficeSymbol", OfficeSymbol)
                        form.SetField("SuspenseDate", IIf(SuspenseDate.Length > 0, "S: " + SuspenseDate, SuspenseDate))  'IIf is fine because SuspenseDate will not error upon evaluation
                        form.SetField("Date", MemoDate)
                        ctField = "Content"
                    Else
                        ctField = "Content_full"
                    End If
                Else
                    'user the higher field for subsequent pages
                    ctField = "Content_full"
                End If

                Dim ct As String = PageContent()
                form.SetField(ctField, ct)
                form.Stamp()

                pages.Add(form)

            End While

            For Each page As PDFForm In pages
                doc.AddForm(page)
            Next

            Return doc

        End Function

        Protected Function RepeatStr(ByVal Value As String, ByVal Count As Integer) As String
            Dim sb As New StringBuilder
            For i As Integer = 0 To Count - 1
                sb.Append(Value)
            Next
            Return sb.ToString
        End Function

        Protected Sub TemplateDataReader(ByVal adapter As SqlDataStore, ByVal reader As System.Data.IDataReader)
            _templateData.Add(adapter.GetString(reader, 0), adapter.GetString(reader, 1))
        End Sub

        Private Sub MemoReader(ByVal adapter As SqlDataStore, ByVal reader As System.Data.IDataReader)

            '0-Description, 1-Content, 2-MailingAddress, 3-addSignature
            '4-dataSource, 5-officeSymbol, 6-addDate, 7-addSuspenseDate, 8-ApprovalRequired
            '9-sigblock, 10-deleted, 11-letterhead, 12-userId, 13-accessScope, 14-creationDate
            '15-memoDate, 16-suspenseDate, 17-restricted, 18-authorGroup, 19-fileName

            _description = adapter.GetString(reader, 0)
            _content = adapter.GetString(reader, 1)
            _mailingAddress = adapter.GetString(reader, 2)
            _addSignature = adapter.GetBoolean(reader, 3, False)
            _dataProc = adapter.GetString(reader, 4)
            _officeSymbol = adapter.GetString(reader, 5)
            _addDate = adapter.GetBoolean(reader, 6, True)
            _addSuspenseDate = adapter.GetBoolean(reader, 7, False)
            _approvalRequired = adapter.GetBoolean(reader, 8, False)
            _sigBlock = adapter.GetString(reader, 9)
            _deleted = adapter.GetBoolean(reader, 10)
            _letterHead = adapter.GetByte(reader, 11)
            _authorId = adapter.GetInteger(reader, 12)
            _scope = adapter.GetByte(reader, 13)
            _createdDate = adapter.GetDateTime(reader, 14)
            _memoDate = adapter.GetString(reader, 15)
            _suspenseDate = adapter.GetString(reader, 16)
            _authorGroup = adapter.GetByte(reader, 18)

            'get the letterhead file to use (if it's set)
            Dim file As String = adapter.GetString(reader, 19)
            If (file.Length > 0) Then
                _templateFileName = file
            End If

            _viewRestricted = adapter.GetBoolean(reader, 20)
            _phi = adapter.GetBoolean(reader, 21)

        End Sub

        Private Function PopulateData() As Boolean

            If (_dataProc.Length = 0) Then
                Return False
            End If

            _templateData = New StringDictionary

            DataStore.ExecuteReader(AddressOf TemplateDataReader, _dataProc, _refId, DataKey, _templateId, CurrentUser.Id, CurrentUser.CurrentRole.Group.Id)

            For Each key As String In _templateData.Keys
                _content = _content.Replace("{" + key.ToUpper + "}", _templateData(key))
            Next
            Return True
        End Function

    End Class

End Namespace