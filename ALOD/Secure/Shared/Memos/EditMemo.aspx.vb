Imports ALOD.Core.Domain.Common
Imports ALOD.Core.Domain.DBSign
Imports ALOD.Core.Domain.Documents
Imports ALOD.Core.Domain.Modules.Lod
Imports ALOD.Core.Domain.Modules.SARC
Imports ALOD.Core.Domain.Modules.SpecialCases
Imports ALOD.Core.Domain.Users
Imports ALOD.Core.Domain.Workflow
Imports ALOD.Core.Interfaces
Imports ALOD.Core.Interfaces.DAOInterfaces
Imports ALOD.Data
Imports ALOD.Data.Services

Namespace Web.Memos

    Partial Class Secure_Shared_Memos_EditMemo
        Inherits System.Web.UI.Page

        Private _BoardSGSig As SignatureMetaData
        Private _daoFactory As NHibernateDaoFactory
        Private _sigDao As ISignatueMetaDateDao
        Private _Signature As IMemoSignatureDao
        Dim dao As ISpecialCaseDAO
        Private memoSource As MemoDao
        Private memoSource2 As MemoDao2
        Private userDao As UserDao

        ReadOnly Property MemoSignatureDao() As IMemoSignatureDao
            Get
                If (_Signature Is Nothing) Then
                    _Signature = New NHibernateDaoFactory().GetMemoSignatureDao()
                End If

                Return _Signature
            End Get
        End Property

        ReadOnly Property SCDao() As ISpecialCaseDAO
            Get
                If (dao Is Nothing) Then
                    dao = DaoFactory.GetSpecialCaseDAO()
                End If

                Return dao
            End Get
        End Property

        Protected ReadOnly Property DaoFactory As NHibernateDaoFactory
            Get
                If (_daoFactory Is Nothing) Then
                    _daoFactory = New NHibernateDaoFactory()
                End If

                Return _daoFactory
            End Get
        End Property

        Private ReadOnly Property MemoStore() As MemoDao
            Get
                If (memoSource Is Nothing) Then
                    memoSource = DaoFactory.GetMemoDao()
                End If
                Return memoSource
            End Get
        End Property

        Private ReadOnly Property MemoStore2() As MemoDao2
            Get
                If (memoSource2 Is Nothing) Then
                    memoSource2 = DaoFactory.GetMemoDao2()
                End If
                Return memoSource2
            End Get
        End Property

        Private ReadOnly Property SigDao() As SignatureMetaDataDao
            Get
                If (_sigDao Is Nothing) Then
                    _sigDao = New NHibernateDaoFactory().GetSigMetaDataDao()
                End If

                Return _sigDao
            End Get
        End Property

        Private ReadOnly Property UserStore() As UserDao
            Get
                If (userDao Is Nothing) Then
                    userDao = DaoFactory.GetUserDao()
                End If
                Return userDao
            End Get
        End Property

        Private ReadOnly Property BoardSGSig(SpecCase As SpecialCase) As SignatureMetaData
            Get
                If (_BoardSGSig Is Nothing) Then
                    _BoardSGSig = SigDao.GetByWorkStatus(SpecCase.Id, SpecCase.Workflow, GetMedicalStep(SpecCase.moduleId))
                End If

                Return _BoardSGSig
            End Get
        End Property

        Protected Function GetMedicalStep(moduleId As Integer) As Integer
            If (moduleId = ModuleType.SpecCaseFT) Then
                Return SpecCaseFTWorkStatus.MedicalReview
            ElseIf (moduleId = ModuleType.SpecCaseMEB) Then
                Return SpecCaseMEBWorkStatus.MedicalReview
            ElseIf (moduleId = ModuleType.SpecCaseWWD) Then
                Return SpecCaseWWDWorkStatus.MedicalReview
            ElseIf (moduleId = ModuleType.SpecCaseRS) Then
                Return SpecCaseRSWorkStatus.MedicalReview
            ElseIf (moduleId = ModuleType.SpecCaseRW) Then
                Return SpecCaseRWWorkStatus.MedicalReview
            ElseIf (moduleId = ModuleType.SpecCaseAGR) Then
                Return SpecCaseAGRWorkStatus.MedicalReview
            Else

                Return 0
            End If
        End Function

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

            If (Not IsPostBack) Then
                InitEditor()
            End If

        End Sub

        Protected Sub SaveButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles SaveButton.Click
            Dim memoId As Integer = CInt(ViewState("memo"))
            Dim templateId As Integer = CInt(ViewState("template"))
            Dim refId As Integer = CInt(ViewState("refId"))
            Dim user As AppUser = UserService.CurrentUser()
            Dim moduleId As Integer = CInt(ViewState("moduleId"))
            'LogManager.LogError("memoID=" + memoId.ToString() + " /templateId=" + templateId.ToString() + " /refId=" + refId.ToString() + " /user=" + user.ToString() + " /moduleId=" + moduleId.ToString())

            'If (templateId = 0) Then
            '    templateId = MemoStore2.GetMemoTemplateId("PSC Determination")
            'End If
            If moduleId = 2 Then
                Dim memo As New Memorandum

                If (memoId > 0) Then
                    memo = MemoStore.GetById(memoId)
                Else
                    memo.Template = MemoStore.GetTemplateById(templateId)
                    memo.Letterhead = MemoStore.GetEffectiveLetterHead(user.Component)
                    memo.CreatedBy = user
                    memo.CreatedDate = DateTime.Now
                    memo.Deleted = False
                    memo.ReferenceId = refId
                End If

                'now add the content
                Dim content As New MemoContent()
                content.Body = MemoBody.Text
                content.SignatureBlock = MemoSignature.Text.Trim
                content.MemoDate = MemoDate.Text.Trim
                content.CreatedBy = user
                content.CreatedDate = DateTime.Now
                content.Attachments = MemoAttachments.Text.Trim

                memo.AddContent(content)
                MemoStore.SaveOrUpdate(memo)
            Else
                Dim memo As New Memorandum2

                If (memoId > 0) Then
                    memo = MemoStore2.GetById(memoId)
                Else
                    memo.Template = MemoStore2.GetTemplateById(templateId)
                    memo.Letterhead = MemoStore2.GetEffectiveLetterHead(user.Component)
                    memo.CreatedBy = user
                    memo.CreatedDate = DateTime.Now
                    memo.Deleted = False
                    memo.ReferenceId = refId
                    memo.ModuleId = moduleId
                End If

                Dim page As Integer = 0
                If (MemoBody.Text.ToString().Contains("{NEW_PAGE}")) Then
                    page = MemoBody.Text.ToString().IndexOf("{NEW_PAGE}")

                End If

                If (page = 0) Then
                    'now add the content
                    Dim content As New MemoContent2()
                    content.Body = MemoBody.Text
                    content.SignatureBlock = MemoSignature.Text
                    content.MemoDate = MemoDate.Text.Trim
                    content.CreatedBy = user
                    content.CreatedDate = DateTime.Now
                    content.Attachments = MemoAttachments.Text.Trim

                    memo.AddContent(content)
                Else
                    'now add the content
                    Dim content As New MemoContent2()
                    content.Body = MemoBody.Text.ToString().Substring(0, page)
                    content.SignatureBlock = MemoSignature.Text
                    content.MemoDate = MemoDate.Text.Trim
                    content.CreatedBy = user
                    content.CreatedDate = DateTime.Now
                    content.Attachments = MemoAttachments.Text.Trim

                    'now add the content
                    Dim content2 As New MemoContent2()
                    content2.Body = MemoBody.Text.ToString().Substring(page + 10)
                    content2.SignatureBlock = MemoSignature.Text
                    content2.MemoDate = MemoDate.Text.Trim
                    content2.CreatedBy = user
                    content2.CreatedDate = DateTime.Now
                    content2.Attachments = MemoAttachments.Text.Trim

                    memo.AddContent(content2)
                    memo.AddContent(content)
                End If

                MemoStore2.SaveOrUpdate(memo)
            End If
            Response.Write("<script>window.close();</script>")
        End Sub

        Private Function GetSigner(ByVal refId As Integer, ByVal templateId As Integer) As DigitalSignatureInfo

            Dim lod As LineOfDuty = LodService.GetById(refId)
            Dim VerifySource As DBSignService
            Dim signatureStatus As DBSignResult

            'Check Approving Authority Signature
            Select Case templateId
                Case MemoType.ANGLodFindingsNLODDeath, MemoType.LodFindingsNLODDeath
                    VerifySource = New DBSignService(DBSignTemplateId.Form348Findings, refId, Core.Domain.Modules.Lod.PersonnelTypes.UNIT_CMDR)

                Case Else
                    If (lod.Formal) Then
                        VerifySource = New DBSignService(DBSignTemplateId.Form348Findings, refId, Core.Domain.Modules.Lod.PersonnelTypes.FORMAL_BOARD_AA)
                    Else
                        VerifySource = New DBSignService(DBSignTemplateId.Form348Findings, refId, Core.Domain.Modules.Lod.PersonnelTypes.BOARD_AA)
                    End If
            End Select

            signatureStatus = VerifySource.VerifySignature()

            If (signatureStatus = DBSignResult.SignatureValid) Then
                Return VerifySource.GetSignerInfo()
            End If

            'Check Board Technician Signature
            If (lod.Formal) Then
                VerifySource = New DBSignService(DBSignTemplateId.Form348Findings, refId, Core.Domain.Modules.Lod.PersonnelTypes.FORMAL_BOARD_RA)
            Else
                VerifySource = New DBSignService(DBSignTemplateId.Form348Findings, refId, Core.Domain.Modules.Lod.PersonnelTypes.BOARD)
            End If
            signatureStatus = VerifySource.VerifySignature()

            If (signatureStatus = DBSignResult.SignatureValid) Then
                Return VerifySource.GetSignerInfo
            End If

            'Check Appointing Authority Signature
            VerifySource = New DBSignService(DBSignTemplateId.WingCC, refId, Core.Domain.Modules.Lod.PersonnelTypes.APPOINT_AUTH)
            signatureStatus = VerifySource.VerifySignature()

            If (signatureStatus = DBSignResult.SignatureValid) Then
                Return VerifySource.GetSignerInfo
            End If

            Return Nothing

        End Function

        Private Sub InitEditor()

            Dim memoId As Integer = 0
            Dim templateId As Integer = 0
            Dim refId As Integer = 0
            Dim moduleId As Integer = ModuleType.LOD

            If (Request.QueryString("memo") IsNot Nothing) Then
                Integer.TryParse(Request.QueryString("memo"), memoId)
            End If

            If (Request.QueryString("template") IsNot Nothing) Then
                Integer.TryParse(Request.QueryString("template"), templateId)
            End If

            If (Request.QueryString("id") IsNot Nothing) Then
                Integer.TryParse(Request.QueryString("id"), refId)
            End If

            If (Request.QueryString("mod") IsNot Nothing) Then
                Integer.TryParse(Request.QueryString("mod"), moduleId)
            End If

            If (refId = 0 Or (memoId = 0 And templateId = 0)) Then
                Exit Sub
            End If

            If (memoId = 0) Then
                'this is a template (new memo)
                LoadFromTemplate(refId, templateId, moduleId)
            Else
                'editing an existing memo
                LoadFromMemo(refId, templateId, memoId, moduleId)
            End If

        End Sub

        Private Sub LoadFromMemo(ByVal refId As Integer, ByVal templateId As Integer, ByVal memoId As Integer, ByVal moduleId As Integer)
            ViewState("template") = templateId
            ViewState("refId") = refId
            ViewState("memo") = memoId
            ViewState("moduleId") = moduleId

            If moduleId = 2 Then
                'we start with the most recent content
                Dim memo As Memorandum = MemoStore.GetById(memoId)
                SetPageTitle("Edit " + memo.Template.Title + " Memorandum")

                Dim content As MemoContent = (From c In memo.Contents Select c Order By c.Id Descending).First

                If (content Is Nothing) Then
                    Exit Sub
                End If

                MemoDate.Text = content.MemoDate
                MemoBody.Text = content.Body
                MemoSignature.Text = content.SignatureBlock

                If (Not String.IsNullOrEmpty(content.Attachments)) Then
                    MemoAttachments.Text = content.Attachments
                End If
            Else
                'we start with the most recent content
                Dim memo As Memorandum2 = MemoStore2.GetById(memoId)
                SetPageTitle("Edit " + memo.Template.Title + " Memorandum")

                Dim content As MemoContent2 = (From c In memo.Contents Select c Order By c.Id Descending).First

                If (content Is Nothing) Then
                    Exit Sub
                End If

                MemoDate.Text = content.MemoDate
                MemoBody.Text = content.Body
                MemoSignature.Text = content.SignatureBlock

                If (Not String.IsNullOrEmpty(content.Attachments)) Then
                    MemoAttachments.Text = content.Attachments
                End If
            End If
        End Sub

        Private Sub LoadFromTemplate(ByVal refId As Integer, ByVal templateId As Integer, ByVal moduleId As Integer)
            ViewState("template") = templateId
            ViewState("refId") = refId
            ViewState("memo") = 0
            ViewState("moduleId") = moduleId
            Dim signer As MemoSignature = Nothing

            Dim template As MemoTemplate
            If moduleId = ModuleType.LOD Then
                template = MemoStore.GetTemplateById(templateId)
                SetPageTitle("Create " + template.Title + " Memorandum")
                MemoBody.Text = template.PopulatedBody(MemoStore.GetMemoData(refId, template.DataSource))
            Else
                template = MemoStore2.GetTemplateById(templateId)
                SetPageTitle("Create " + template.Title + " Memorandum")
                MemoBody.Text = template.PopulatedBody(MemoStore2.GetMemoData(refId, template.DataSource))
                MemoAttachments.Text = template.GetCC(MemoStore2.GetMemoData(refId, template.DataSource))

            End If

            If (template.AddDate) Then
                MemoDate.Text = DateTime.Now.ToString(template.DateFormat)
            End If

            'now get the signature  -- only get signatures for Investigating Officer Appointment Letter???
            If (moduleId = ModuleType.LOD) Then

                LODSignature(refId, templateId)

            ElseIf (moduleId = ModuleType.SARC) Then

                Dim sarc As RestrictedSARC = New NHibernateDaoFactory().GetSARCDao().GetById(refId)
                Dim VerifySource As DBSignService = New DBSignService(DBSignTemplateId.Form348SARCFindings, refId, Core.Domain.Modules.Lod.PersonnelTypes.BOARD_AA)
                Dim sigLine As String = String.Empty
                Dim sig As String = String.Empty

                Dim signatureStatus As DBSignResult = VerifySource.VerifySignature()

                If (signatureStatus = DBSignResult.SignatureValid) Then

                    Dim signInfo As DigitalSignatureInfo = VerifySource.GetSignerInfo()
                    If (signInfo IsNot Nothing) Then
                        sig = signInfo.Signature
                    End If

                    sigLine = "Digitally signed by " + sig + Environment.NewLine + "Date: " + signInfo.DateSigned.ToString("dd MMM yyyy")

                    sigLine = sigLine + Environment.NewLine + Environment.NewLine + "Commander"

                    MemoSignature.Text = sigLine
                Else
                    MemoSignature.Text = ""
                End If
            Else
                Dim sigLine As String = ""
                Dim sc As New SpecialCase
                sc = SCDao.GetById(refId)

                Select Case moduleId
                    Case ModuleType.SpecCaseFT
                        Dim scModule As New SC_FastTrack
                        WriteSignature(scModule, refId, sigLine, sc)

                    Case ModuleType.SpecCaseMEB
                        Dim scModule As New SC_MEB
                        WriteSignature(scModule, refId, sigLine, sc)

                    Case ModuleType.SpecCaseWWD
                        Dim scModule As New SC_WWD
                        WriteSignature(scModule, refId, sigLine, sc)

                    Case ModuleType.SpecCaseRS
                        Dim scModule As New SC_RS
                        WriteSignature(scModule, refId, sigLine, sc)

                    Case ModuleType.SpecCaseRW
                        Dim scModule As New SC_RW
                        WriteSignature(scModule, refId, sigLine, sc)

                    Case ModuleType.SpecCaseAGR
                        Dim scModule As New SC_AGRCert
                        WriteSignature(scModule, refId, sigLine, sc)

                    Case ModuleType.SpecCasePSCD
                        Dim scModule As New SC_PSCD
                        WriteSignature(scModule, refId, sigLine, sc)

                    Case Else
                        MemoBody.Text = MemoBody.Text.Replace("{SIGNATURE_BLOCK}", String.Empty)
                End Select

            End If

            'If (Not String.IsNullOrEmpty(template.Attachments)) Then
            '    MemoAttachments.Text = template.Attachments
            'End If

            'clear the dao to ensure any changes to the template don't get propogated back to the db
            If moduleId = 2 Then
                MemoStore.EvictTemplate(template)
            Else
                MemoStore2.EvictTemplate(template)
            End If
        End Sub

        Private Sub LODSignature(ByVal refId As Integer, ByVal templateId As Integer)

            Dim lod As LineOfDuty = LodService.GetById(refId)

            Dim sigLine As String = ""

            Select Case templateId
                Case MemoType.LodAppointIo
                    Dim workstatus = 0
                    If (lod.Workflow = 1) Then
                        workstatus = LodWorkStatus.AppointingAutorityReview
                    Else
                        workstatus = LodWorkStatus_v2.AppointingAutorityReview
                    End If
                    Dim appointAuthSig As SignatureMetaData = SigDao.GetByWorkStatus(lod.Id, lod.Workflow, workstatus)

                    If (appointAuthSig IsNot Nothing AndAlso lod.AAUserId IsNot Nothing) Then
                        sigLine = appointAuthSig.NameAndRank + vbCrLf + IIf(UserStore.GetById(lod.AAUserId).Component = "6", "USAF ", "ANG ") + UserStore.GetById(lod.AAUserId).Unit.Name + "/CC"
                    End If

                Case MemoType.LodFindingsILOD, MemoType.LodFindingsILODDeath, MemoType.LodFindingsNLOD, MemoType.ANGLodFindingsILOD, MemoType.ANGLodFindingsILODDeath, MemoType.ANGLodFindingsNLOD
                    Dim signInfo As DigitalSignatureInfo = GetSigner(refId, templateId)

                    If (signInfo IsNot Nothing) Then
                        Dim sig As String = signInfo.Signature
                        Dim sigDate As String = signInfo.DateSigned.ToString("dd MMM yyyy")
                        sigLine = "Digitally signed by " + sig + Environment.NewLine + "Date: " + sigDate
                        sigLine = sigLine + Environment.NewLine + Environment.NewLine + "Commander"
                    End If

                Case MemoType.LodFindingsNLODDeath, MemoType.ANGLodFindingsNLODDeath
                    Dim signInfo As DigitalSignatureInfo = GetSigner(refId, templateId)

                    If (signInfo IsNot Nothing) Then
                        Dim sig As String = signInfo.Signature
                        Dim sigDate As String = signInfo.DateSigned.ToString("dd MMM yyyy")
                        sigLine = "Digitally signed by " + IIf(lod.MemberCompo = "6", "AFRC/A1 ", "NGB/A1") + sig + Environment.NewLine + "Date: " + sigDate
                        sigLine = sigLine + Environment.NewLine + Environment.NewLine + "Commander"
                    End If

                Case Else
                    'Do Nothing/Handled elsewhere
            End Select

            MemoSignature.Text = sigLine

        End Sub

        Private Sub SetPageTitle(ByVal title As String)
            CType(Page.Master, Secure_Popup).SetHeaderTitle(title)
        End Sub

        Private Sub WriteSignature(ByRef scModule As Object, ByVal refId As Integer, ByVal sigLine As String, ByVal sc As ALOD.Core.Domain.Modules.SpecialCases.SpecialCase)
            Const DIGITAL_SIGNATURE_DATE_FORMAT As String = "yyyy.MM.dd HH:mm:ss zz\'00\'"
            Dim VerifySource As DBSignService
            VerifySource = New DBSignService(DBSignTemplateId.SignOnly, refId, 0)

            Dim signatureStatus As DBSignResult = VerifySource.VerifySignature()
            Dim user As AppUser = UserService.CurrentUser()
            Dim sigDateTime As String = String.Empty
            Dim sigStaticBlock As String = "Signed by HQ Air Force Reserve Command (AFRC)" + Environment.NewLine + "Office of the Command Surgeon" + Environment.NewLine + "Aerospace Medicine Division" + Environment.NewLine + "Physical Exams and Standards Branch"

            If (signatureStatus = DBSignResult.SignatureValid) Then
                'if it's valid, add the signing info to the form
                Dim signInfo As DigitalSignatureInfo = VerifySource.GetSignerInfo()
                'sigLine = "Digitally signed by " + signInfo.Signature
                sigLine = sigStaticBlock
                sigDateTime = "Date: " + signInfo.DateSigned.ToString(DIGITAL_SIGNATURE_DATE_FORMAT)
            Else
                Dim sigdate As String = String.Empty

                If (BoardSGSig(sc) IsNot Nothing) Then
                    sigdate = BoardSGSig(sc).date
                Else
                    sigdate = DateTime.UtcNow.ToString()
                End If

                'sigLine = Environment.NewLine + "Signed by " + user.SignatureName _
                '            + Environment.NewLine + user.CurrentUnitName _
                '            + Environment.NewLine + "Date: " + Date.Today

                'sigLine = Environment.NewLine + "Signed by " + sigName _
                '            + Environment.NewLine + "Date: " + sigdate

                sigLine = Environment.NewLine + sigStaticBlock + Environment.NewLine + sigdate
            End If

            'MemoBody.Text = MemoBody.Text.Replace("{SIGNATURE_BLOCK}", String.Empty)
            MemoSignature.Text = sigLine
        End Sub

    End Class

End Namespace