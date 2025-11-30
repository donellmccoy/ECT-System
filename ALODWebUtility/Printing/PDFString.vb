Namespace Printing

    Public Class PDFString
        Private _alignment As String = "center"
        Private _color As String = "#FF0000/90"
        Private _font As String = "arial"
        Private _fontSize As Integer = 12
        Private _fontWeight As String = String.Empty
        Private _linespacing As Integer = 0
        Private _postNewLines As Integer = 0
        Private _preNewLines As Integer = 0
        Private _rotation As Integer = 0
        Private _text As String = String.Empty

        Public Property Alignment() As String
            Get
                Return Me._alignment
            End Get
            Set(value As String)
                Me._alignment = value
            End Set
        End Property

        Public Property Color() As String
            Get
                Return Me._color
            End Get
            Set(value As String)
                Me._color = value
            End Set
        End Property

        Public Property Font() As String
            Get
                Return Me._font
            End Get
            Set(value As String)
                Me._font = value
            End Set
        End Property

        Public Property FontSize As Integer
            Get
                Return Me._fontSize
            End Get
            Set(value As Integer)
                Me._fontSize = value
            End Set
        End Property

        Public Property FontWeight() As String
            Get
                Return Me._fontWeight
            End Get
            Set(value As String)
                Me._fontWeight = value
            End Set
        End Property

        Public Property Linespacing As Integer
            Get
                Return Me._linespacing
            End Get
            Set(value As Integer)
                Me._linespacing = value
            End Set
        End Property

        Public Property PostNewLines As Integer
            Get
                Return Me._postNewLines
            End Get
            Set(value As Integer)
                Me._postNewLines = value
            End Set
        End Property

        Public Property PreNewLines As Integer
            Get
                Return Me._preNewLines
            End Get
            Set(value As Integer)
                Me._preNewLines = value
            End Set
        End Property

        Public Property Rotation As Integer
            Get
                Return Me._rotation
            End Get
            Set(value As Integer)
                Me._rotation = value
            End Set
        End Property

        Public Property Text() As String
            Get
                Return Me._text
            End Get
            Set(value As String)
                Me._text = value
            End Set
        End Property

        Public Function GetHTML() As String
            Dim htmlString As String = String.Empty

            Dim sLinespacing As String = String.Empty
            Dim sSize As String = String.Empty
            Dim sAlignment As String = String.Empty
            Dim sFontFace As String = String.Empty
            Dim sColor As String = String.Empty

            If (Linespacing <> 0) Then
                sLinespacing = "linespacing=" & Linespacing.ToString()
            End If

            If (FontSize >= 0) Then
                sSize = "fontsize=" & FontSize.ToString()
            End If

            If (Not String.IsNullOrEmpty(Alignment)) Then
                sAlignment = "align=" & Alignment
            End If

            If (Not String.IsNullOrEmpty(Font)) Then
                sFontFace = "face='" & Font
            End If

            If (Not String.IsNullOrEmpty(FontWeight)) Then
                sFontFace = sFontFace & "-" & FontWeight & "'"
            Else
                sFontFace = sFontFace & "'"
            End If

            If (Not String.IsNullOrEmpty(Color)) Then
                sColor = "color=" & Color
            End If

            htmlString = GetBreakTagString(PreNewLines)
            htmlString = htmlString & "<p " & Linespacing & " " & sSize & " " & sAlignment + "><StyleRun rotate=" & Rotation & "><Font " & sFontFace & " " & sColor & ">" & Text & "</Font></StyleRun></p>"
            htmlString = htmlString & GetBreakTagString(PostNewLines)

            Return htmlString
        End Function

        Private Function GetBreakTagString(ByVal numberOfBreaks As Integer) As String
            If (numberOfBreaks <= 0) Then
                Return String.Empty
            End If

            Dim breakTags As String = String.Empty

            For i As Integer = 1 To numberOfBreaks
                breakTags = breakTags & "<br>"
            Next

            Return breakTags
        End Function

    End Class

End Namespace