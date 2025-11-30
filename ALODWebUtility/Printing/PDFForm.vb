Imports System.Text
Imports System.Web
Imports ALOD.Core.Interfaces
Imports ALOD.Data.Services
Imports ALODWebUtility.Printing.FormFieldParsers
Imports WebSupergoo.ABCpdf8
Imports WebSupergoo.ABCpdf8.Objects

Namespace Printing

    Public Class PDFForm
        Inherits PrintDocument
        Implements IDisposable

#Region "Members"

        Public Const DATATYPE_BOOL As String = "bool"
        Public Const DATATYPE_CHOICE As String = "choice"
        Public Const DATATYPE_STRING As String = "string"
        Private _doc As Doc = Nothing
        Private _formFieldParser As IPDFFormFieldParser
        Private _formFieldParserName As String = String.Empty
        Private _open As Boolean = False

#End Region

#Region "Properties"

        Public ReadOnly Property Buffer() As Byte()
            Get
                Return _doc.GetData()
            End Get
        End Property

        Public ReadOnly Property Document() As Doc
            Get
                Return _doc
            End Get
        End Property

        Public ReadOnly Property Fields() As Fields
            Get
                Return _doc.Form.Fields
            End Get
        End Property

        Public Property FormFieldParser As IPDFFormFieldParser
            Get
                Return _formFieldParser
            End Get
            Private Set(value As IPDFFormFieldParser)
                _formFieldParser = value
            End Set
        End Property

        Public Property FormFieldParserName As String
            Get
                Return _formFieldParserName
            End Get

            Private Set(value As String)
                _formFieldParserName = value
            End Set
        End Property

        Public ReadOnly Property IsOpen() As Boolean
            Get
                Return _open
            End Get
        End Property

        Public Sub FrameRect()
            _doc.FrameRect()
        End Sub

#End Region

#Region "Constructors"

        Public Sub New(ByVal fileName As String)
            MyBase.New()
            InitializeDocument(fileName)
        End Sub

        Public Sub New(ByVal docId As Integer)
            MyBase.New(docId)
            InitializeDocument(_filename)
        End Sub

#End Region

#Region "Methods"

        Public Sub Close()

            If (_open) Then
                _open = False
            End If
        End Sub

        ''' <summary>
        ''' Retrieves data elements from the data store and populates the form fields
        ''' </summary>
        ''' <param name="keyId"></param>
        ''' <remarks></remarks>
        Public Sub Populate(ByVal keyId As Integer)

            If _doc Is Nothing Then Throw New ApplicationException("Document not initialized")

            Dim ds As DataSet
            Try
                ds = GetDocumentData(keyId)
                For Each dsTable As DataTable In ds.Tables
                    SingleItemLayout(dsTable)
                Next
            Catch dataEx As DataException
                Throw dataEx
            Catch ex As Exception
                Throw New ApplicationException("Problems assembling the document. " + ex.Message)
            End Try
        End Sub

        Public Sub SetBoolField(ByVal fieldName As String, ByVal value As String)

            Dim checked As Boolean = False

            Select Case value.ToLower
                Case "yes", "y", "true", "1"
                    checked = True
            End Select
            If checked Then
                Fields(fieldName).Value = Fields(fieldName).Options(0)
            Else
                Fields(fieldName).Value = "Off"
            End If

        End Sub

        ''' <summary>
        ''' Sets a range of related checkboxes.  Only one will be checked
        ''' </summary>
        ''' <param name="fieldName">the shared prefix for the check boxes</param>
        ''' <param name="value">The value that should be selected</param>
        ''' <remarks></remarks>
        Public Sub SetChoiceField(ByVal fieldName As String, ByVal value As String)

            Dim key As String = fieldName + value

            For Each item As Field In Fields

                If (item.Name.Substring(0, Math.Min(item.Name.Length - 1, fieldName.Length)) = fieldName) Then

                    If (item.Name = key) Then
                        Fields(key).Value = Fields(key).Options(0)
                    Else
                    End If

                End If

            Next

        End Sub

        Public Sub SetField(ByVal fieldName As String, ByVal value As String)
            AssignDocumentValue(fieldName, value, DATATYPE_STRING)
        End Sub

        Public Sub SetField(ByVal fieldname As String, ByVal value As String, ByVal dataType As String)
            AssignDocumentValue(fieldname, value, dataType)
        End Sub

        Public Sub Stamp()
            _doc.Form.Stamp()
        End Sub

        ''' <summary>
        ''' Summary for SuppressThirdPage().
        ''' With the method RemapPages("2"), we are only printing the first two pages of the document (form348).
        ''' </summary>
        ''' <remarks>Print the first document only.</remarks>
        Public Sub SuppressInstructionPages()

            _doc.RemapPages("1,2,3")

        End Sub

        ''' <summary>
        ''' Summary for SuppressSecondPage().
        ''' With the method RemapPages("1"), we are only printing the first page of the document (form348).
        ''' </summary>
        ''' <remarks>Print the first document only.</remarks>
        Public Sub SuppressSecondPage()

            _doc.RemapPages("1")

        End Sub

        Protected Sub InitializeDocument(ByVal fileName As String)
            _filename = fileName
            Dim docPath As String = HttpContext.Current.Server.MapPath("~/Secure/Documents/" & _filename)
            _doc = New Doc()
            _doc.Read(docPath)
            _open = True
            FormFieldParserName = LookupService.GetFormFieldParserById(_formFieldParserId)
            FormFieldParser = FormFieldParserFactory.GetParserByName(FormFieldParserName)
        End Sub

        'Used to support the operations of the AssembleDocument method.
        'Will evaluate the field types from the mappings and set the
        'values to the designated data types.
        'Currently supports: String, Datetime, and Number.
        Private Sub AssignDocumentValue(ByVal fieldName As String, ByVal value As Object, ByVal valueType As String)
            If (fieldName Is Nothing OrElse value Is Nothing) Then
                Exit Sub
            End If

            Try
                Dim field As Field = FormFieldParser.ParseField(_doc, fieldName)

                If (field Is Nothing) Then
                    Exit Sub
                End If

                ' As of the writing of this comment (9/13/2016) the only case ever used by ECT is "string"; therefore the SetChoiceField() and SetBoolField() have not been updated to properly handle the newer versions of PDF files created in Adobe LiveCycle...
                Select Case valueType.ToLower.Trim()
                    Case "string"
                        field.Value = Convert.ToString(value).Trim()
                    Case "datetime"
                        field.Value = Convert.ToDateTime(value)
                    Case "number"
                        field.Value = Convert.ToInt32(value)
                    Case "choice"
                        SetChoiceField(fieldName.Trim(), Convert.ToString(value).Trim())
                    Case "bool"
                        SetBoolField(fieldName.Trim(), Convert.ToString(value).Trim())
                End Select
            Catch ex As Exception
                Throw New Exception("Error setting field " + fieldName)
            End Try
        End Sub

        'Used to assemble multiple data items into a single item assignment in a Document.
        'I.E.: Single document item 'txt_Symptoms' is assigned a grouping of multiple data
        'values returned.
        Private Sub GroupedItemsLayout(ByVal dt As DataTable)
            Dim groupedText As New StringBuilder
            Dim currentFieldName As String = String.Empty
            Dim fieldType As String = String.Empty
            For Each row As DataRow In dt.Rows
                currentFieldName = Convert.ToString(row(1)).Trim()
                fieldType = Convert.ToString(row(3)).Trim()
                groupedText.Append(Convert.ToString(row(2)).Trim())
                groupedText.Append(_sbDelimiter)
            Next
            groupedText.Replace(_sbDelimiter, String.Empty, groupedText.Length - 3, 3)
            AssignDocumentValue(currentFieldName, groupedText.ToString(), fieldType)
        End Sub

        'Used to assemble multiple data items into multiple item assignments constructed
        'with the use of an appended increment.
        'Example: Document items 'txt_Symptom_1', 'txt_Symptom_2' will be assigned data items
        'via an incremented Field assignment. This requires that the field names are defined
        'without the increment value included ('txt_Symptom_') to allow concatenation with the
        'increment.
        Private Sub IncrementedItemsLayout(ByVal dt As DataTable)
            Dim incrementValue As Integer = 0
            For Each row As DataRow In dt.Rows
                incrementValue += 1
                AssignDocumentValue(Convert.ToString(row(1)).Trim() + incrementValue.ToString(), row(2), Convert.ToString(row(3)).Trim())
            Next
        End Sub

        'Used to assemble single item assignments in a Document.
        'I.E.: Single document item 'txt_SoldierName' is assigned single data item row("SoldierName").
        Private Sub SingleItemLayout(ByVal dt As DataTable)
            For Each row As DataRow In dt.Rows
                AssignDocumentValue(Convert.ToString(row(1)).Trim(), row(2), Convert.ToString(row(3)).Trim())
            Next
        End Sub

#End Region

#Region "IDisposable"

        Private disposedValue As Boolean = False        ' To detect redundant calls

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub

        ' IDisposable
        Protected Overridable Sub Dispose(ByVal disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    _doc.Clear()
                End If

                ' TODO: free shared unmanaged resources

            End If
            Me.disposedValue = True
        End Sub

#End Region

    End Class

End Namespace