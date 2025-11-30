Namespace Common

    Public NotInheritable Class [WorkingEnum](Of T As Structure)
        Private Shared ReadOnly All As IEnumerable(Of T) = [Enum].GetValues(GetType(T)).Cast(Of T)()

        Private Shared ReadOnly InsensitiveNames As Dictionary(Of String, T) = All.ToDictionary(Function(k) [Enum].GetName(GetType(T), k).ToLowerInvariant())

        Private Shared ReadOnly Names As Dictionary(Of T, String) = All.ToDictionary(Function(k) k, Function(v) v.ToString())

        Private Shared ReadOnly SensitiveNames As Dictionary(Of String, T) = All.ToDictionary(Function(k) [Enum].GetName(GetType(T), k))

        Private Shared ReadOnly Values As Dictionary(Of Integer, T) = All.ToDictionary(Function(k) Convert.ToInt32(k))

        Private Sub New()
        End Sub

        Public Shared Function CastOrNull(ByVal value As Integer) As System.Nullable(Of T)
            '6/21/19
            Dim foundValue As T = Nothing

            If Values.TryGetValue(value, foundValue) Then
                Return foundValue
            End If

            Return Nothing
        End Function

        Public Shared Function GetName(ByVal value As T) As String
            Dim name As String = ""
            Names.TryGetValue(value, name)
            Return name
        End Function

        Public Shared Function GetNames() As String()
            Return Names.Values.ToArray()
        End Function

        Public Shared Function GetValues() As IEnumerable(Of T)
            Return All
        End Function

        Public Shared Function IsDefined(ByVal value As T) As Boolean
            Return Names.Keys.Contains(value)
        End Function

        Public Shared Function IsDefined(ByVal value As String) As Boolean
            Return SensitiveNames.Keys.Contains(value)
        End Function

        Public Shared Function IsDefined(ByVal value As Integer) As Boolean
            Return Values.Keys.Contains(value)
        End Function

        Public Shared Function Parse(ByVal value As String) As T
            Dim parsed As T = Nothing
            If Not SensitiveNames.TryGetValue(value, parsed) Then
                Throw New ArgumentException("Value is not one of the named constants defined for the enumeration", "value")
            End If
            Return parsed
        End Function

        Public Shared Function Parse(ByVal value As String, ByVal ignoreCase As Boolean) As T
            If Not ignoreCase Then
                Return Parse(value)
            End If

            Dim parsed As T = Nothing
            If Not InsensitiveNames.TryGetValue(value.ToLowerInvariant(), parsed) Then
                Throw New ArgumentException("Value is not one of the named constants defined for the enumeration", "value")
            End If
            Return parsed
        End Function

        Public Shared Function ParseOrNull(ByVal value As String) As System.Nullable(Of T)
            If [String].IsNullOrEmpty(value) Then
                Return Nothing
            End If
            '6/21/19
            Dim foundValue As T = Nothing

            If InsensitiveNames.TryGetValue(value.ToLowerInvariant(), foundValue) Then
                Return foundValue
            End If

            Return Nothing
        End Function

        Public Shared Function TryParse(ByVal value As String, ByVal returnValue As T) As Boolean
            Return SensitiveNames.TryGetValue(value, returnValue)
        End Function

        Public Shared Function TryParse(ByVal value As String, ByVal ignoreCase As Boolean, ByVal returnValue As T) As Boolean
            If Not ignoreCase Then
                Return TryParse(value, returnValue)
            End If

            Return InsensitiveNames.TryGetValue(value.ToLowerInvariant(), returnValue)
        End Function

    End Class

End Namespace

'=======================================================
'Service provided by Telerik (www.telerik.com)
'Conversion powered by NRefactory.
'Twitter: @telerik, @toddanglin
'Facebook: facebook.com/telerik
'=======================================================