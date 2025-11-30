Imports System.IO
Imports System.Security.Cryptography
Imports System.Text
Imports System.Web
Imports ALOD.Logging

Namespace Common

    Public Class SecureQueryString
        Implements IDictionary

#Region "Members/Properties"

        Protected _hash As String = String.Empty
        Protected _isValid As Boolean = False
        Protected _querySeperators As Char() = {"?", "&"}
        Protected _queryVar As String = "data"
        Protected _rawQuery As String = String.Empty
        Protected _sep As Char = "|"
        Protected _validHash As Boolean = False
        Private Shared _password As String = "f1dr32eg@@#WWEa123D&*"
        Private Shared _salt As String = "saltyDoggy"
        Private _hashMark As String = "|_h="
        Private _values As Hashtable

        ''' <summary>
        ''' Returns the entire query string encrypted as a block
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property EncryptedString() As String
            Get
                'we have to UrlEncode this twice in order to avoid the random encoding problems that
                'can occur with single encoding
                'if a single encoded string randomly hits the sequence *On*= (or in regex terms: [^a-zA-Z]on[a-zA-Z]*\s*= )
                'then the CrossSiteScriptingValidation will flag it as potentially dangerous
                'and throw and error, so we encode twice which will get around that
                Return HttpContext.Current.Server.UrlEncode(
                    HttpContext.Current.Server.UrlEncode(Encrypt(Me.RawString)))
            End Get
            Set(ByVal Value As String)
                Parse(Decrypt(Value))
            End Set
        End Property

        ''' <summary>
        ''' Indicates if the decrypted string matches its hash
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks>If this value is FALSE the data cannot be trusted</remarks>
        Public ReadOnly Property IsValid() As Boolean
            Get
                Return (_isValid AndAlso _validHash)
            End Get
        End Property

        Public Property QueryStringItem() As String
            Get
                Return _queryVar
            End Get
            Set(ByVal value As String)
                _queryVar = value
            End Set
        End Property

        ''' <summary>
        ''' Returns the unencrypted query string
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property RawString() As String

            Get
                Return Me.ToString
            End Get
            Set(ByVal Value As String)
                Parse(Value)
                _rawQuery = Me.ToString
            End Set

        End Property

        ''' <summary>
        ''' Returns the key/value pairs stored in the query string as plain text
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Overrides Function ToString() As String
            Dim buffer As New StringBuilder

            'convert our key value pairs to a string
            For Each item As DictionaryEntry In _values

                If (buffer.Length > 0) Then
                    buffer.Append(_sep)
                End If

                buffer.Append(item.Key.ToString() + "=" + item.Value.ToString())
            Next

            Return buffer.ToString()
        End Function

#End Region

#Region "Getters"

        Public Function GetBoolean(ByVal key As String) As Boolean

            If (Me(key).Length > 0) Then

                Try
                    Return Boolean.Parse(Me(key))
                Catch argNull As ArgumentNullException
                    LogManager.LogError(argNull)
                    Dim rethrow As Boolean = ExceptionPolicy.HandleException(argNull, "General")
                    If rethrow Then Throw
                Catch format As FormatException
                    LogManager.LogError(format)
                    Dim rethrow As Boolean = ExceptionPolicy.HandleException(format, "General")
                    If rethrow Then Throw
                End Try
            End If

            Return Nothing

        End Function

        Public Function GetByte(ByVal key As String) As Byte

            'if they key is not found it returns an empty string
            'either way, isnumeric will fail if it's not a number or not found
            If (IsNumeric(Me(key))) Then
                Try
                    Return Byte.Parse(Me(key))
                Catch argNull As ArgumentNullException
                    LogManager.LogError(argNull, "SecureQueryString.GetByte")
                    Dim rethrow As Boolean = ExceptionPolicy.HandleException(argNull, "General")
                    If rethrow Then Throw
                Catch format As FormatException
                    LogManager.LogError(format, "SecureQueryString.GetByte")
                    Dim rethrow As Boolean = ExceptionPolicy.HandleException(format, "General")
                    If rethrow Then Throw
                End Try
            End If
            Return Nothing

        End Function

        Public Function GetDateTime(ByVal key As String) As DateTime

            If (_values.ContainsKey(key)) Then

                Try
                    Return DateTime.Parse(Me(key))
                Catch argNull As ArgumentNullException
                    LogManager.LogError(argNull, "SecureQueryString.GetDateTime")
                    Dim rethrow As Boolean = ExceptionPolicy.HandleException(argNull, "General")
                    If rethrow Then Throw
                Catch format As FormatException
                    LogManager.LogError(format, "SecureQueryString.GetDateTime")
                    Dim rethrow As Boolean = ExceptionPolicy.HandleException(format, "General")
                    If rethrow Then Throw
                End Try

            End If
            Return Nothing
        End Function

        Public Function GetDouble(ByVal key As String) As Integer

            'if they key is not found it returns an empty string
            'either way, isnumeric will fail if it's not a number or not found
            If (IsNumeric(Me(key))) Then
                Try
                    Return Double.Parse(Me(key))
                Catch argNull As ArgumentNullException
                    LogManager.LogError(argNull, "SecureQueryString.GetDouble")
                    Dim rethrow As Boolean = ExceptionPolicy.HandleException(argNull, "General")
                    If rethrow Then Throw
                Catch format As FormatException
                    LogManager.LogError(format, "SecureQueryString.GetDouble")
                    Dim rethrow As Boolean = ExceptionPolicy.HandleException(format, "General")
                    If rethrow Then Throw
                End Try
            End If
            Return Nothing

        End Function

        Public Function GetInt16(ByVal key As String) As Int16

            'if they key is not found it returns an empty string
            'either way, isnumeric will fail if it's not a number or not found
            If (IsNumeric(Me(key))) Then
                Try
                    Return Int16.Parse(Me(key))
                Catch argNull As ArgumentNullException
                    LogManager.LogError(argNull, "SecureQueryString.GetInt16")
                    Dim rethrow As Boolean = ExceptionPolicy.HandleException(argNull, "General")
                    If rethrow Then Throw
                Catch format As FormatException
                    LogManager.LogError(format, "SecureQueryString.GetInt16")
                    Dim rethrow As Boolean = ExceptionPolicy.HandleException(format, "General")
                    If rethrow Then Throw
                End Try
            End If
            Return Nothing
        End Function

        Public Function GetInt32(ByVal key As String) As Integer

            'if they key is not found it returns an empty string
            'either way, isnumeric will fail if it's not a number or not found
            If (IsNumeric(Me(key))) Then
                Try
                    Return Integer.Parse(Me(key))
                Catch argNull As ArgumentNullException
                    LogManager.LogError(argNull, "SecureQueryString.GetInt32")
                    Dim rethrow As Boolean = ExceptionPolicy.HandleException(argNull, "General")
                    If rethrow Then Throw
                Catch format As FormatException
                    LogManager.LogError(format, "SecureQueryString.GetInt32")
                    Dim rethrow As Boolean = ExceptionPolicy.HandleException(format, "General")
                    If rethrow Then Throw
                End Try
            End If

            Return Nothing

        End Function

        Public Function GetInt64(ByVal key As String) As Int64

            'if they key is not found it returns an empty string
            'either way, isnumeric will fail if it's not a number or not found
            If (IsNumeric(Me(key))) Then
                Try
                    Return Int64.Parse(Me(key))
                Catch argNull As ArgumentNullException
                    LogManager.LogError(argNull, "SecureQueryString.GetInt64")
                    Dim rethrow As Boolean = ExceptionPolicy.HandleException(argNull, "General")
                    If rethrow Then Throw
                Catch format As FormatException
                    LogManager.LogError(format, "SecureQueryString.GetInt64")
                    Dim rethrow As Boolean = ExceptionPolicy.HandleException(format, "General")
                    If rethrow Then Throw
                End Try
            End If
            Return Nothing
        End Function

        Public Function GetInteger(ByVal key As String) As Integer
            Return GetInt32(key)
        End Function

        Public Function GetString(ByVal key As String) As String
            Return Me(key)
        End Function

#End Region

#Region "Constructors"

        ''' <summary>
        ''' Creates an empty query string
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub New()
            _values = New Hashtable
        End Sub

        ''' <summary>
        ''' Used to decrypt an encrypted query string
        ''' </summary>
        ''' <param name="input">Request.RawUrl</param>
        ''' <remarks>The raw url data from the request must be parsed for this to function properly</remarks>
        Public Sub New(ByVal input As String)

            Dim parts() As String = input.Split(_querySeperators)
            Dim dataPart As String = String.Empty

            For Each key As String In parts
                If (key.IndexOf(_queryVar) = 0) Then
                    Dim index As Short = key.IndexOf("=")
                    dataPart = key.Substring(index + 1)
                    Exit For
                End If
            Next

            'if we didn't find our data= block then we can assume we were handed a raw data block
            'rather then a query string
            If (dataPart.Length = 0) Then
                dataPart = input
            End If

            'make sure we are dealing with an unencoded string
            'if the string has already been decoded calling decode again won't hurt anything
            dataPart = HttpContext.Current.Server.UrlDecode(HttpContext.Current.Server.UrlDecode(dataPart))

            'decode has a bad habit of replacing + with a space, so undo that
            'since spaces are not valid in an encrypted string, we can safely do this
            dataPart = dataPart.Replace(" ", "+")

            _values = New Hashtable

            If (dataPart.Length > 0) Then
                'make sure we have something to decrypt
                Parse(Decrypt(dataPart))
            End If

        End Sub

        Public Sub New(ByVal input As String, ByVal IsRawQuery As Boolean)

            Dim parts() As String = input.Split(_querySeperators)
            Dim dataPart As String = String.Empty

            For Each key As String In parts
                If (key.IndexOf(_queryVar) = 0) Then
                    Dim index As Short = key.IndexOf("=")
                    dataPart = key.Substring(index + 1)
                    Exit For
                End If
            Next

            'if we didn't find our data= block then we can assume we were handed a raw data block
            'rather then a query string
            If (dataPart.Length = 0) Then
                dataPart = input
            End If

            'make sure we are dealing with an unencoded string
            'if the string has already been decoded calling decode again won't hurt anything
            dataPart = HttpContext.Current.Server.UrlDecode(HttpContext.Current.Server.UrlDecode(dataPart))

            'decode has a bad habit of replacing + with a space, so undo that
            'since spaces are not valid in an encrypted string, we can safely do this
            dataPart = dataPart.Replace(" ", "+")

            _values = New Hashtable

            If (dataPart.Length > 0) Then
                'make sure we have something to decrypt
                If Not (IsRawQuery) Then
                    Parse(Decrypt(dataPart))
                Else
                    ParseRaw(dataPart)
                End If

            End If

        End Sub

#End Region

#Region "Parsers"

        Protected Sub Parse(ByVal input As String)

            'strip off the leading seperator if there is one (there shouldn't be)
            If (input.IndexOf(_sep) = 0) Then
                input = input.Substring(1)
            End If

            'split our string into key/value pairs
            Dim parts() As String = input.Split(_sep)
            Dim pair() As String

            For Each value As String In parts

                'now split each key/value pair
                pair = value.Split("=", 2, StringSplitOptions.RemoveEmptyEntries)

                'if they are both valid add them to the hashtable
                If (pair.Length = 2) Then
                    _values.Add(pair(0).ToLower(), pair(1))
                End If

            Next

            _isValid = True

        End Sub

        Protected Sub ParseRaw(ByVal input As String)

            Dim _qSep As Char() = {";", "&"}

            'strip off the leading seperator if there is one (there shouldn't be)
            If (input.IndexOf(_sep) = 0) Then
                input = input.Substring(1)
            End If

            'split our string into key/value pairs
            Dim parts() As String = input.Split(_qSep)
            Dim pair() As String

            For Each value As String In parts

                'now split each key/value pair
                pair = value.Split("=", 2, StringSplitOptions.RemoveEmptyEntries)

                'if they are both valid add them to the hashtable
                If (pair.Length = 2) Then
                    _values.Add(pair(0).ToLower(), pair(1))
                End If

            Next

            _isValid = True

        End Sub

#End Region

#Region "Encrypt/Decrypt"

        Protected Shared Function DecryptData(ByVal data As Byte(), ByVal paddingMode As PaddingMode) As Byte()

            If (data Is Nothing) OrElse (data.Length = 0) Then
                Throw New ArgumentNullException("data")
            End If

            If (_password.Length = 0) Then
                Throw New ArgumentNullException("password")
            End If

            Dim pdb As New Rfc2898DeriveBytes(_password, Encoding.UTF8.GetBytes(_salt))
            Dim rm As New RijndaelManaged
            rm.Padding = paddingMode
            Dim decryptor As ICryptoTransform = rm.CreateDecryptor(pdb.GetBytes(16), pdb.GetBytes(16))
            Dim msDecrypt As New MemoryStream(data)
            Dim csDecrypt As New CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)
            ' Decrypted bytes will always be less then encrypted bytes, so len of encrypted data will be big enouph for buffer.
            Dim fromEncrypt(data.Length) As Byte

            ' Read as many bytes as possible.
            Dim read As Integer = csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length)

            If (read < fromEncrypt.Length) Then
                ' Return a byte array of proper size.
                Dim clearBytes(read) As Byte
                Buffer.BlockCopy(fromEncrypt, 0, clearBytes, 0, read)
                Return clearBytes
            End If

            Return fromEncrypt

        End Function

        Protected Shared Function EncryptData(ByVal data As Byte(), ByVal paddingMode As PaddingMode) As Byte()

            If (data Is Nothing) OrElse (data.Length = 0) Then
                Throw New ArgumentNullException("data")
            End If

            If (_password.Length = 0) Then
                Throw New ArgumentNullException("password")
            End If

            Dim pdb As Rfc2898DeriveBytes = New Rfc2898DeriveBytes(_password, Encoding.UTF8.GetBytes(_salt))
            Dim rm As RijndaelManaged = New RijndaelManaged
            rm.Padding = paddingMode
            Dim encryptor As ICryptoTransform = rm.CreateEncryptor(pdb.GetBytes(16), pdb.GetBytes(16))

            Dim msEncrypt As New MemoryStream
            Dim encStream As New CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)
            encStream.Write(data, 0, data.Length)
            encStream.FlushFinalBlock()
            Return msEncrypt.ToArray()

        End Function

        Protected Function Decrypt(ByVal data As String) As String

            If (data Is Nothing) Then
                Throw New ArgumentNullException("data")
            End If

            If (_password.Length = 0) Then
                Throw New ArgumentNullException("password")
            End If

            Try

                Dim encBytes() As Byte = Convert.FromBase64String(data)

                If (encBytes.Length > 0) Then
                    'only decrypt it if we have a valid string to work with
                    Dim decBytes() As Byte = DecryptData(encBytes, PaddingMode.PKCS7)
                    _rawQuery = Encoding.UTF8.GetString(decBytes)
                End If
            Catch ex As FormatException
                'we got some bad data, might have been tampered with, might just be an error
                'either way, we can't decrypt it
                'ErrorLog.LogError(ex, "Decrypt")
            End Try

            'chop off the trailing null if it's there
            'on multi-part strings an extra null gets appended, so strip it off
            If (_rawQuery.Length > 0) AndAlso (_rawQuery.Chars(_rawQuery.Length - 1) = Nothing) Then
                _rawQuery = _rawQuery.Substring(0, _rawQuery.Length - 1)
            End If

            'before we return our string, pull our hash off the end and make sure it's valid
            Dim index As Short = _rawQuery.IndexOf(_hashMark)

            If (index <> -1) Then
                'we have a hash, parse it out
                Dim hash As String = _rawQuery.Substring(index + _hashMark.Length)
                _rawQuery = _rawQuery.Substring(0, index)
                'now get the hash from what's left  and compare the two
                Dim queryHash As String = GetHash(_rawQuery)
                _validHash = (queryHash = hash)
            End If

            If (Not _validHash) Then
                'this is a bad string, clear out our values and mark it as invalid
                _validHash = False
                _rawQuery = String.Empty
            End If

            Return _rawQuery

        End Function

        Protected Function Encrypt(ByVal data As String) As String

            If (data Is Nothing) Then
                Throw New ArgumentNullException("data")
            End If

            If (_password.Length = 0) Then
                Throw New ArgumentNullException("password")
            End If

            'first, get the hash of our string
            Dim hash As String = GetHash(data)

            'now append our hash to our string
            data += _hashMark + hash

            'now encrypt the whole thing
            Dim encBytes() As Byte = EncryptData(Encoding.UTF8.GetBytes(data), PaddingMode.PKCS7)
            Return Convert.ToBase64String(encBytes)

        End Function

        Protected Function GetHash(ByVal data As String) As String

            Dim rawBytes() As Byte = Encoding.UTF8.GetBytes(data)
            Dim hash() As Byte = New MD5CryptoServiceProvider().ComputeHash(rawBytes)

            Dim buffer As New StringBuilder(hash.Length)

            'convert our hash to a hex string
            For i As Integer = 0 To hash.Length - 1
                buffer.Append(hash(i).ToString("X2"))
            Next

            Return buffer.ToString()

        End Function

#End Region

#Region "IDictionary Methods"

        Public ReadOnly Property Count() As Integer Implements System.Collections.ICollection.Count
            Get
                Return _values.Count
            End Get
        End Property

        Public ReadOnly Property IsFixedSize() As Boolean Implements System.Collections.IDictionary.IsFixedSize
            Get
                Return _values.IsFixedSize
            End Get
        End Property

        Public ReadOnly Property IsReadOnly() As Boolean Implements System.Collections.IDictionary.IsReadOnly
            Get
                Return _values.IsReadOnly
            End Get
        End Property

        Public ReadOnly Property IsSynchronized() As Boolean Implements System.Collections.ICollection.IsSynchronized
            Get
                Return _values.IsSynchronized
            End Get
        End Property

        Public ReadOnly Property Keys() As System.Collections.ICollection Implements System.Collections.IDictionary.Keys
            Get
                Return _values.Keys
            End Get
        End Property

        Public ReadOnly Property SyncRoot() As Object Implements System.Collections.ICollection.SyncRoot
            Get
                Return _values.SyncRoot
            End Get
        End Property

        Public ReadOnly Property Values() As System.Collections.ICollection Implements System.Collections.IDictionary.Values
            Get
                Return _values.Values
            End Get
        End Property

        Default Public Property Item(ByVal key As Object) As Object Implements System.Collections.IDictionary.Item
            Get
                Dim skey As String = key.ToString.ToLower()

                If (_values.ContainsKey(skey)) Then

                    Dim o As Object = _values(skey)
                    If (TypeOf o Is String) Then
                        Return CStr(o).Trim()
                    End If

                    Return o
                End If

                Return String.Empty

            End Get
            Set(ByVal Value As Object)

                Dim skey As String = key.ToString.ToLower()

                If (_values.ContainsKey(skey)) Then
                    _values(skey) = Value
                Else
                    'this is a new one, so add it first
                    _values.Add(skey, Value)
                End If

            End Set
        End Property

        Public Sub Add(ByVal key As Object, ByVal value As Object) Implements System.Collections.IDictionary.Add
            _values.Add(key.ToString.ToLower(), value)
        End Sub

        Public Sub Clear() Implements System.Collections.IDictionary.Clear
            _values.Clear()
        End Sub

        Public Function Contains(ByVal key As Object) As Boolean Implements System.Collections.IDictionary.Contains
            Return _values.Contains(key.ToString.ToLower())
        End Function

        Public Sub CopyTo(ByVal array As System.Array, ByVal index As Integer) Implements System.Collections.ICollection.CopyTo
            _values.CopyTo(array, index)
        End Sub

        Public Function GetEnumerator() As System.Collections.IDictionaryEnumerator Implements System.Collections.IDictionary.GetEnumerator
            Return _values.GetEnumerator()
        End Function

        Public Function GetEnumerator1() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
            Return _values.GetEnumerator
        End Function

        Public Sub Remove(ByVal key As Object) Implements System.Collections.IDictionary.Remove
            _values.Remove(key.ToString.ToLower())
        End Sub

#End Region

    End Class

End Namespace