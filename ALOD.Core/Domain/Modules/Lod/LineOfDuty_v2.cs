// Decompiled with JetBrains decompiler
// Type: ALOD.Core.Domain.Modules.Lod.LineOfDuty_v2
// Assembly: ALOD.Core, Version=1.0.9.34207, Culture=neutral, PublicKeyToken=null
// MVID: 3217D62A-03A8-41D0-9C2E-145DC45E27D3
// Assembly location: C:\Users\zbook\Desktop\AlodChangeset#543\bin\ALOD.Core.dll

using ALOD.Core.Domain.Common;
using ALOD.Core.Domain.Documents;
using ALOD.Core.Domain.Lookup;
using ALOD.Core.Domain.Users;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces;
using ALOD.Core.Interfaces.DAOInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

//#nullable disable
namespace ALOD.Core.Domain.Modules.Lod
{
    [Serializable]
    public class LineOfDuty_v2 : LineOfDuty
    {
        public virtual string AppointingUnit { get; set; }

        public virtual bool DirectReply { get; set; }

        public override int DocumentViewId => 25;

        public override bool FormalInvestigationRecommended
        {
            get
            {
                LineOfDutyFindings byType = this.FindByType((short)5);
                bool investigationRecommended = false;
                if (byType != null && byType.Finding.HasValue)
                {
                    short? finding = byType.Finding;
                    int num;
                    if (finding.Value != (short)6)
                    {
                        finding = byType.Finding;
                        if (finding.Value != (short)4)
                        {
                            finding = byType.Finding;
                            num = finding.Value == (short)5 ? 1 : 0;
                            goto label_5;
                        }
                    }
                    num = 1;
                label_5:
                    if (num != 0)
                        investigationRecommended = true;
                }
                return investigationRecommended;
            }
        }

        public virtual LineOfDutyMedical_v2 LODMedical_v2 => (LineOfDutyMedical_v2)this.LODMedical;
        public virtual LineOfDutyUnit_v2 LODUnit_v2 => (LineOfDutyUnit_v2)this.LODUnit;
        public virtual int? NILODsubFinding { get; set; }

        public override void AddSignature(
          IDaoFactory daoFactory,
          UserGroups groupId,
          string title,
          AppUser user)
        {
            string str = "AFRC";
            if (user.Component == "5")
                str = "ANG";
            daoFactory.GetSigMetaDataDao().AddForWorkStatus(new SignatureMetaData()
            {
                refId = this.Id,
                workflowId = this.Workflow,
                workStatus = this.Status,
                userGroup = (int)groupId,
                userId = user.Id,
                date = DateTime.Now,
                NameAndRank = user.AlternateSignatureName,
                Title = title
            });
            if (this.CurrentStatusCode == 10 && this.LODInvestigation != null)
            {
                this.LODInvestigation.DateSignedAppointing = new DateTime?(DateTime.Now);
                this.LODInvestigation.SignatureInfoAppointing = new PersonnelData()
                {
                    Name = user.FormName,
                    Grade = user.Rank.Rank,
                    SSN = user.SSN,
                    PasCodeDescription = user.Unit.Name,
                    Branch = str
                };
            }
            if (this.CurrentStatusCode != 8 || this.LODInvestigation == null)
                return;
            this.LODInvestigation.DateSignedIO = new DateTime?(DateTime.Now);
            this.LODInvestigation.SignatureInfoIO = new PersonnelData()
            {
                Name = user.FormName,
                Grade = user.Rank.Rank,
                SSN = user.SSN,
                PasCodeDescription = user.Unit.Name,
                Branch = str
            };
        }

        public override IEnumerable<WorkflowStatusOption> GetCurrentOptions(
                  int lastStatus,
          IDaoFactory daoFactory,
          int userId)
        {
            this.Validations.Clear();
            this.Validate(userId);
            this.ProcessDocuments(daoFactory);
            this.ProcessOption(lastStatus, daoFactory);
            return (IEnumerable<WorkflowStatusOption>)this.RuleAppliedOptions;
        }

        public virtual string GetMemberRankAndGradeForForm(ILookupDao lookupDao)
        {
            string abbreviationByType = lookupDao.GetRankAbbreviationByType(this.MemberRank, "Form 348 Member");
            return string.IsNullOrEmpty(abbreviationByType) ? this.MemberRank.FormattedGrade : abbreviationByType;
        }

        public override bool IsReconductFormalInvestigationRequested(ILookupDao lookupDao)
        {
            if (!this.Formal || !lookupDao.GetIsReinvestigationLod(this.Id))
                return false;
            LineOfDutyFindings byType = this.FindByType((short)13);
            return byType != null && byType.Finding.HasValue && byType.Finding.Value == (short)6;
        }

        public override void ProcessDocuments(IDaoFactory daoFactory)
        {
            IDocumentDao documentDao = daoFactory.GetDocumentDao();
            IDocCategoryViewDao docCategoryViewDao = daoFactory.GetDocCategoryViewDao();
            if (!this.DocumentGroupId.HasValue)
                return;
            this.Documents = documentDao.GetDocumentsByGroupId(this.DocumentGroupId.Value);
            List<DocumentCategory2> byDocumentViewId = (List<DocumentCategory2>)docCategoryViewDao.GetCategoriesByDocumentViewId(this.DocumentViewId);
            foreach (WorkflowStatusOption workStatusOption in (IEnumerable<WorkflowStatusOption>)this.WorkflowStatus.WorkStatusOptionList)
            {
                foreach (WorkflowOptionRule rule in (IEnumerable<WorkflowOptionRule>)workStatusOption.RuleList)
                {
                    switch (rule.RuleTypes.Name.ToLower())
                    {
                        case "document":
                            string[] strArray = rule.RuleValue.ToString().Split(',');
                            for (int index = 0; index < strArray.Length; ++index)
                            {
                                if (!string.IsNullOrEmpty(strArray[index]))
                                {
                                    string docName = strArray[index];
                                    bool flag1 = true;
                                    bool isValid = false;
                                    if (this.Documents != null && this.Documents.Where<Document>((Func<Document, bool>)(p => p.DocType.ToString() == docName)).ToList<Document>().Count > 0)
                                        isValid = true;
                                    if (docName == DocumentType.AutopsyReportDeathCertificate.ToString() && this.LODMedical.DeathInvolved != "Yes")
                                    {
                                        isValid = true;
                                        flag1 = false;
                                    }
                                    if ((docName == DocumentType.Maps.ToString() || docName == DocumentType.AccidentReport.ToString()) && this.LODMedical.MvaInvolved != "Yes")
                                    {
                                        isValid = true;
                                        flag1 = false;
                                    }
                                    if (docName == DocumentType.UntimelySubmissionOfIncidentReport.ToString())
                                    {
                                        if (this.TimelyMemberNotify.HasValue)
                                        {
                                            bool? timelyMemberNotify = this.TimelyMemberNotify;
                                            bool flag2 = true;
                                            if (timelyMemberNotify.GetValueOrDefault() == flag2 & timelyMemberNotify.HasValue)
                                            {
                                                isValid = true;
                                                flag1 = false;
                                            }
                                        }
                                        else
                                        {
                                            isValid = true;
                                            flag1 = false;
                                        }
                                    }
                                    this.AddReqdDocs(docName, isValid);
                                    if (this.Active.ContainsKey(docName))
                                    {
                                        if (!isValid)
                                        {
                                            string str = byDocumentViewId.Where<DocumentCategory2>((Func<DocumentCategory2, bool>)(p => p.DocCatId.ToString() == docName)).Select<DocumentCategory2, string>((Func<DocumentCategory2, string>)(p => p.CategoryDescription)).Single<string>();
                                            this.AddValidationItem(new ValidationItem("Documents", docName, str + "  document not found."));
                                        }
                                        this.Active[docName] = flag1;
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }

        public override Dictionary<string, PageAccessType> ReadSectionList(int role)
        {
            PageAccessType pageAccessType1 = PageAccessType.None;
            Dictionary<string, PageAccessType> dictionary1 = new Dictionary<string, PageAccessType>();
            if (this.CurrentStatusCode == 16)
                pageAccessType1 = PageAccessType.ReadOnly;
            dictionary1.Add(SectionNames.MED_TECH_REV.ToString(), pageAccessType1);
            Dictionary<string, PageAccessType> dictionary2 = dictionary1;
            SectionNames sectionNames = SectionNames.RLB;
            string key1 = sectionNames.ToString();
            int num1 = (int)pageAccessType1;
            dictionary2.Add(key1, (PageAccessType)num1);
            Dictionary<string, PageAccessType> dictionary3 = dictionary1;
            sectionNames = SectionNames.MED_OFF_REV;
            string key2 = sectionNames.ToString();
            int num2 = (int)pageAccessType1;
            dictionary3.Add(key2, (PageAccessType)num2);
            Dictionary<string, PageAccessType> dictionary4 = dictionary1;
            sectionNames = SectionNames.UNIT_CMD_REV;
            string key3 = sectionNames.ToString();
            int num3 = (int)pageAccessType1;
            dictionary4.Add(key3, (PageAccessType)num3);
            Dictionary<string, PageAccessType> dictionary5 = dictionary1;
            sectionNames = SectionNames.APP_AUTH_REV;
            string key4 = sectionNames.ToString();
            int num4 = (int)pageAccessType1;
            dictionary5.Add(key4, (PageAccessType)num4);
            Dictionary<string, PageAccessType> dictionary6 = dictionary1;
            sectionNames = SectionNames.APPOINT_IO;
            string key5 = sectionNames.ToString();
            int num5 = (int)pageAccessType1;
            dictionary6.Add(key5, (PageAccessType)num5);
            Dictionary<string, PageAccessType> dictionary7 = dictionary1;
            sectionNames = SectionNames.NOTIFY_IO;
            string key6 = sectionNames.ToString();
            int num6 = (int)pageAccessType1;
            dictionary7.Add(key6, (PageAccessType)num6);
            Dictionary<string, PageAccessType> dictionary8 = dictionary1;
            sectionNames = SectionNames.IO;
            string key7 = sectionNames.ToString();
            int num7 = (int)pageAccessType1;
            dictionary8.Add(key7, (PageAccessType)num7);
            Dictionary<string, PageAccessType> dictionary9 = dictionary1;
            sectionNames = SectionNames.WING_JA_REV;
            string key8 = sectionNames.ToString();
            int num8 = (int)pageAccessType1;
            dictionary9.Add(key8, (PageAccessType)num8);
            Dictionary<string, PageAccessType> dictionary10 = dictionary1;
            sectionNames = SectionNames.FORMAL_ACTION_APP_AUTH;
            string key9 = sectionNames.ToString();
            int num9 = (int)pageAccessType1;
            dictionary10.Add(key9, (PageAccessType)num9);
            Dictionary<string, PageAccessType> dictionary11 = dictionary1;
            sectionNames = SectionNames.FORMAL_ACTION_WING_JA;
            string key10 = sectionNames.ToString();
            int num10 = (int)pageAccessType1;
            dictionary11.Add(key10, (PageAccessType)num10);
            PageAccessType pageAccessType2 = PageAccessType.ReadOnly;
            Dictionary<string, PageAccessType> dictionary12 = dictionary1;
            sectionNames = SectionNames.BOARD_REV;
            string key11 = sectionNames.ToString();
            int num11 = (int)pageAccessType2;
            dictionary12.Add(key11, (PageAccessType)num11);
            Dictionary<string, PageAccessType> dictionary13 = dictionary1;
            sectionNames = SectionNames.BOARD_MED_REV;
            string key12 = sectionNames.ToString();
            int num12 = (int)pageAccessType2;
            dictionary13.Add(key12, (PageAccessType)num12);
            Dictionary<string, PageAccessType> dictionary14 = dictionary1;
            sectionNames = SectionNames.SENIOR_MED_REV;
            string key13 = sectionNames.ToString();
            int num13 = (int)pageAccessType2;
            dictionary14.Add(key13, (PageAccessType)num13);
            Dictionary<string, PageAccessType> dictionary15 = dictionary1;
            sectionNames = SectionNames.BOARD_LEGAL_REV;
            string key14 = sectionNames.ToString();
            int num14 = (int)pageAccessType2;
            dictionary15.Add(key14, (PageAccessType)num14);
            Dictionary<string, PageAccessType> dictionary16 = dictionary1;
            sectionNames = SectionNames.BOARD_APPROVING_AUTH_REV;
            string key15 = sectionNames.ToString();
            int num15 = (int)pageAccessType2;
            dictionary16.Add(key15, (PageAccessType)num15);
            Dictionary<string, PageAccessType> dictionary17 = dictionary1;
            sectionNames = SectionNames.BOARD_SENIOR_REV;
            string key16 = sectionNames.ToString();
            int num16 = (int)pageAccessType2;
            dictionary17.Add(key16, (PageAccessType)num16);
            Dictionary<string, PageAccessType> dictionary18 = dictionary1;
            sectionNames = SectionNames.BOARD_PERSONNEL_REV;
            string key17 = sectionNames.ToString();
            int num17 = (int)pageAccessType2;
            dictionary18.Add(key17, (PageAccessType)num17);
            if (!this.Formal)
                pageAccessType2 = PageAccessType.None;
            Dictionary<string, PageAccessType> dictionary19 = dictionary1;
            sectionNames = SectionNames.FORMAL_BOARD_REV;
            string key18 = sectionNames.ToString();
            int num18 = (int)pageAccessType2;
            dictionary19.Add(key18, (PageAccessType)num18);
            Dictionary<string, PageAccessType> dictionary20 = dictionary1;
            sectionNames = SectionNames.FORMAL_BOARD_MED_REV;
            string key19 = sectionNames.ToString();
            int num19 = (int)pageAccessType2;
            dictionary20.Add(key19, (PageAccessType)num19);
            Dictionary<string, PageAccessType> dictionary21 = dictionary1;
            sectionNames = SectionNames.FORMAL_SENIOR_MED_REV;
            string key20 = sectionNames.ToString();
            int num20 = (int)pageAccessType2;
            dictionary21.Add(key20, (PageAccessType)num20);
            Dictionary<string, PageAccessType> dictionary22 = dictionary1;
            sectionNames = SectionNames.FORMAL_BOARD_LEGAL_REV;
            string key21 = sectionNames.ToString();
            int num21 = (int)pageAccessType2;
            dictionary22.Add(key21, (PageAccessType)num21);
            Dictionary<string, PageAccessType> dictionary23 = dictionary1;
            sectionNames = SectionNames.FORMAL_BOARD_APPROVING_AUTH_REV;
            string key22 = sectionNames.ToString();
            int num22 = (int)pageAccessType2;
            dictionary23.Add(key22, (PageAccessType)num22);
            Dictionary<string, PageAccessType> dictionary24 = dictionary1;
            sectionNames = SectionNames.FORMAL_BOARD_SENIOR_REV;
            string key23 = sectionNames.ToString();
            int num23 = (int)pageAccessType2;
            dictionary24.Add(key23, (PageAccessType)num23);
            Dictionary<string, PageAccessType> dictionary25 = dictionary1;
            sectionNames = SectionNames.FORMAL_BOARD_PERSONNEL_REV;
            string key24 = sectionNames.ToString();
            int num24 = (int)pageAccessType2;
            dictionary25.Add(key24, (PageAccessType)num24);
            switch (role)
            {
                case 2:
                    if (this.CurrentStatusCode == 4)
                    {
                        Dictionary<string, PageAccessType> dictionary26 = dictionary1;
                        sectionNames = SectionNames.UNIT_CMD_REV;
                        string key25 = sectionNames.ToString();
                        dictionary26[key25] = PageAccessType.ReadWrite;
                        Dictionary<string, PageAccessType> dictionary27 = dictionary1;
                        sectionNames = SectionNames.RLB;
                        string key26 = sectionNames.ToString();
                        dictionary27[key26] = PageAccessType.ReadWrite;
                        break;
                    }
                    break;

                case 3:
                    if (this.CurrentStatusCode == 1)
                    {
                        Dictionary<string, PageAccessType> dictionary28 = dictionary1;
                        sectionNames = SectionNames.MED_TECH_REV;
                        string key27 = sectionNames.ToString();
                        dictionary28[key27] = PageAccessType.ReadWrite;
                        Dictionary<string, PageAccessType> dictionary29 = dictionary1;
                        sectionNames = SectionNames.RLB;
                        string key28 = sectionNames.ToString();
                        dictionary29[key28] = PageAccessType.ReadWrite;
                        break;
                    }
                    break;

                case 4:
                    if (this.CurrentStatusCode == 3)
                    {
                        Dictionary<string, PageAccessType> dictionary30 = dictionary1;
                        sectionNames = SectionNames.MED_OFF_REV;
                        string key29 = sectionNames.ToString();
                        dictionary30[key29] = PageAccessType.ReadWrite;
                        Dictionary<string, PageAccessType> dictionary31 = dictionary1;
                        sectionNames = SectionNames.RLB;
                        string key30 = sectionNames.ToString();
                        dictionary31[key30] = PageAccessType.ReadWrite;
                        break;
                    }
                    break;

                case 5:
                    if (this.CurrentStatusCode == 6)
                    {
                        Dictionary<string, PageAccessType> dictionary32 = dictionary1;
                        sectionNames = SectionNames.APP_AUTH_REV;
                        string key31 = sectionNames.ToString();
                        dictionary32[key31] = PageAccessType.ReadWrite;
                        Dictionary<string, PageAccessType> dictionary33 = dictionary1;
                        sectionNames = SectionNames.RLB;
                        string key32 = sectionNames.ToString();
                        dictionary33[key32] = PageAccessType.ReadWrite;
                        Dictionary<string, PageAccessType> dictionary34 = dictionary1;
                        sectionNames = SectionNames.APPOINT_IO;
                        string key33 = sectionNames.ToString();
                        dictionary34[key33] = PageAccessType.ReadWrite;
                    }
                    if (this.CurrentStatusCode == 10)
                    {
                        Dictionary<string, PageAccessType> dictionary35 = dictionary1;
                        sectionNames = SectionNames.FORMAL_ACTION_APP_AUTH;
                        string key34 = sectionNames.ToString();
                        dictionary35[key34] = PageAccessType.ReadWrite;
                        break;
                    }
                    break;

                case 6:
                    if (this.CurrentStatusCode == 5)
                    {
                        Dictionary<string, PageAccessType> dictionary36 = dictionary1;
                        sectionNames = SectionNames.WING_JA_REV;
                        string key35 = sectionNames.ToString();
                        dictionary36[key35] = PageAccessType.ReadWrite;
                        Dictionary<string, PageAccessType> dictionary37 = dictionary1;
                        sectionNames = SectionNames.RLB;
                        string key36 = sectionNames.ToString();
                        dictionary37[key36] = PageAccessType.ReadWrite;
                    }
                    if (this.CurrentStatusCode == 9)
                    {
                        Dictionary<string, PageAccessType> dictionary38 = dictionary1;
                        sectionNames = SectionNames.FORMAL_ACTION_WING_JA;
                        string key37 = sectionNames.ToString();
                        dictionary38[key37] = PageAccessType.ReadWrite;
                        break;
                    }
                    break;

                case 7:
                    if (this.CurrentStatusCode == 11)
                    {
                        Dictionary<string, PageAccessType> dictionary39 = dictionary1;
                        sectionNames = SectionNames.BOARD_REV;
                        string key38 = sectionNames.ToString();
                        dictionary39[key38] = PageAccessType.ReadWrite;
                    }
                    if (this.CurrentStatusCode == 20)
                    {
                        Dictionary<string, PageAccessType> dictionary40 = dictionary1;
                        sectionNames = SectionNames.FORMAL_BOARD_REV;
                        string key39 = sectionNames.ToString();
                        dictionary40[key39] = PageAccessType.ReadWrite;
                    }
                    Dictionary<string, PageAccessType> dictionary41 = dictionary1;
                    sectionNames = SectionNames.RLB;
                    string key40 = sectionNames.ToString();
                    dictionary41[key40] = PageAccessType.ReadOnly;
                    break;

                case 8:
                    if (this.CurrentStatusCode == 13)
                    {
                        Dictionary<string, PageAccessType> dictionary42 = dictionary1;
                        sectionNames = SectionNames.BOARD_LEGAL_REV;
                        string key41 = sectionNames.ToString();
                        dictionary42[key41] = PageAccessType.ReadWrite;
                    }
                    if (this.CurrentStatusCode == 22)
                    {
                        Dictionary<string, PageAccessType> dictionary43 = dictionary1;
                        sectionNames = SectionNames.FORMAL_BOARD_LEGAL_REV;
                        string key42 = sectionNames.ToString();
                        dictionary43[key42] = PageAccessType.ReadWrite;
                        break;
                    }
                    break;

                case 9:
                    if (this.CurrentStatusCode == 12)
                    {
                        Dictionary<string, PageAccessType> dictionary44 = dictionary1;
                        sectionNames = SectionNames.BOARD_MED_REV;
                        string key43 = sectionNames.ToString();
                        dictionary44[key43] = PageAccessType.ReadWrite;
                    }
                    if (this.CurrentStatusCode == 21)
                    {
                        Dictionary<string, PageAccessType> dictionary45 = dictionary1;
                        sectionNames = SectionNames.FORMAL_BOARD_MED_REV;
                        string key44 = sectionNames.ToString();
                        dictionary45[key44] = PageAccessType.ReadWrite;
                        break;
                    }
                    break;

                case 11:
                    if (this.CurrentStatusCode == 15)
                    {
                        Dictionary<string, PageAccessType> dictionary46 = dictionary1;
                        sectionNames = SectionNames.BOARD_APPROVING_AUTH_REV;
                        string key45 = sectionNames.ToString();
                        dictionary46[key45] = PageAccessType.ReadWrite;
                    }
                    if (this.CurrentStatusCode == 24)
                    {
                        Dictionary<string, PageAccessType> dictionary47 = dictionary1;
                        sectionNames = SectionNames.FORMAL_BOARD_APPROVING_AUTH_REV;
                        string key46 = sectionNames.ToString();
                        dictionary47[key46] = PageAccessType.ReadWrite;
                        break;
                    }
                    break;

                case 13:
                    if (this.CurrentStatusCode == 7)
                    {
                        Dictionary<string, PageAccessType> dictionary48 = dictionary1;
                        sectionNames = SectionNames.NOTIFY_IO;
                        string key47 = sectionNames.ToString();
                        dictionary48[key47] = PageAccessType.ReadWrite;
                        break;
                    }
                    break;

                case 14:
                    if (this.CurrentStatusCode == 8)
                    {
                        Dictionary<string, PageAccessType> dictionary49 = dictionary1;
                        sectionNames = SectionNames.IO;
                        string key48 = sectionNames.ToString();
                        dictionary49[key48] = PageAccessType.ReadWrite;
                        break;
                    }
                    break;

                case 92:
                    if (this.CurrentStatusCode == 229)
                    {
                        Dictionary<string, PageAccessType> dictionary50 = dictionary1;
                        sectionNames = SectionNames.SENIOR_MED_REV;
                        string key49 = sectionNames.ToString();
                        dictionary50[key49] = PageAccessType.ReadWrite;
                    }
                    if (this.CurrentStatusCode == 249)
                    {
                        Dictionary<string, PageAccessType> dictionary51 = dictionary1;
                        sectionNames = SectionNames.FORMAL_SENIOR_MED_REV;
                        string key50 = sectionNames.ToString();
                        dictionary51[key50] = PageAccessType.ReadWrite;
                        break;
                    }
                    break;

                case 95:
                    if (this.CurrentStatusCode == 1)
                    {
                        Dictionary<string, PageAccessType> dictionary52 = dictionary1;
                        sectionNames = SectionNames.MED_TECH_REV;
                        string key51 = sectionNames.ToString();
                        dictionary52[key51] = PageAccessType.ReadWrite;
                        Dictionary<string, PageAccessType> dictionary53 = dictionary1;
                        sectionNames = SectionNames.RLB;
                        string key52 = sectionNames.ToString();
                        dictionary53[key52] = PageAccessType.ReadWrite;
                        break;
                    }
                    break;

                case 96:
                    if (this.CurrentStatusCode == 7)
                    {
                        Dictionary<string, PageAccessType> dictionary54 = dictionary1;
                        sectionNames = SectionNames.NOTIFY_IO;
                        string key53 = sectionNames.ToString();
                        dictionary54[key53] = PageAccessType.ReadWrite;
                        break;
                    }
                    break;

                case 97:
                    if (this.CurrentStatusCode == 169)
                    {
                        Dictionary<string, PageAccessType> dictionary55 = dictionary1;
                        sectionNames = SectionNames.BOARD_PERSONNEL_REV;
                        string key54 = sectionNames.ToString();
                        dictionary55[key54] = PageAccessType.ReadWrite;
                    }
                    if (this.CurrentStatusCode == 170)
                    {
                        Dictionary<string, PageAccessType> dictionary56 = dictionary1;
                        sectionNames = SectionNames.FORMAL_BOARD_PERSONNEL_REV;
                        string key55 = sectionNames.ToString();
                        dictionary56[key55] = PageAccessType.ReadWrite;
                    }
                    if (this.CurrentStatusCode == 266)
                    {
                        Dictionary<string, PageAccessType> dictionary57 = dictionary1;
                        sectionNames = SectionNames.BOARD_REV;
                        string key56 = sectionNames.ToString();
                        dictionary57[key56] = PageAccessType.ReadWrite;
                    }
                    if (this.CurrentStatusCode == 269)
                    {
                        Dictionary<string, PageAccessType> dictionary58 = dictionary1;
                        sectionNames = SectionNames.FORMAL_BOARD_REV;
                        string key57 = sectionNames.ToString();
                        dictionary58[key57] = PageAccessType.ReadWrite;
                        break;
                    }
                    break;
            }
            return dictionary1;
        }

        public override void RemoveSignature(
          IDaoFactory daoFactory,
          int deleteWorkStatus,
          int newStatusCodeOut)
        {
            daoFactory.GetSigMetaDataDao().DeleteForWorkStatus(this.Id, this.Workflow, deleteWorkStatus);
            if (10 == newStatusCodeOut && this.LODInvestigation != null)
            {
                this.LODInvestigation.DateSignedAppointing = new DateTime?();
                this.LODInvestigation.SignatureInfoAppointing = (PersonnelData)null;
            }
            if (8 != newStatusCodeOut || this.LODInvestigation == null)
                return;
            this.LODInvestigation.DateSignedIO = new DateTime?();
            this.LODInvestigation.SignatureInfoIO = (PersonnelData)null;
        }

        public override void Validate(int userid)
        {
            LineOfDutyFindings byType1 = this.FindByType((short)19);
            LineOfDutyFindings byType2 = this.FindByType((short)3);
            foreach (WorkflowStatusOption workStatusOption in (IEnumerable<WorkflowStatusOption>)this.WorkflowStatus.WorkStatusOptionList)
            {
                foreach (WorkflowOptionRule rule in (IEnumerable<WorkflowOptionRule>)workStatusOption.RuleList)
                {
                    switch (rule.RuleTypes.Name.ToLower())
                    {
                        case "medical":
                            this.ValidateModule("medical", (IValidatable)this.LODMedical_v2, userid);
                            break;

                        case "unit":
                            this.LODUnit.UnitFinding = byType2;
                            this.ValidateModule("unit", (IValidatable)this.LODUnit_v2, userid);
                            break;

                        case "investigation":
                            this.LODInvestigation.IOFinding = byType1;
                            this.ValidateModule("investigation", (IValidatable)this.LODInvestigation, userid);
                            break;
                    }
                }
            }
        }

        protected override bool HasPostCompletionDigitalSignatureBeenGenerated(IDaoFactory daoFactory)
        {
            ISignatueMetaDateDao sigMetaDataDao = daoFactory.GetSigMetaDataDao();
            return sigMetaDataDao.GetByWorkStatus(this.Id, this.Workflow, 220) != null || sigMetaDataDao.GetByWorkStatus(this.Id, this.Workflow, 330) != null;
        }

        private void AddModuleStatus(string section, bool isValid)
        {
            if (this.ModuleStatus.ContainsKey(section))
                return;
            this.ModuleStatus.Add(section, isValid);
        }

        private void AddReqdDocs(string section, bool isValid)
        {
            if (this.Required.ContainsKey(section))
                return;
            this.Required.Add(section, isValid);
        }

        private void AddValidationItem(IList<ValidationItem> itemsList)
        {
            foreach (ValidationItem items in (IEnumerable<ValidationItem>)itemsList)
            {
                if (!this.Validations.Contains(items))
                    this.AddValidationItem(items);
            }
        }

        private void AddValidationItem(ValidationItem item)
        {
            if (this.Validations.Where<ValidationItem>((Func<ValidationItem, bool>)(p => p.Section == item.Section && p.Field == item.Field)).ToList<ValidationItem>().Count != 0)
                return;
            this.Validations.Add(item);
        }

        private void ApplyRulesToOption(
          WorkflowStatusOption o,
          WorkflowOptionRule r,
          int lastStatus,
          IDaoFactory daoFactory)
        {
            IMemoDao memoDao = daoFactory.GetMemoDao();
            ILookupDao lookupDao = daoFactory.GetLookupDao();
            short? finding;
            int? nullable;
            if (r.RuleTypes.ruleTypeId == 1)
            {
                string lower = r.RuleTypes.Name.ToLower();
                if (lower != null)
                {
                    switch (lower.Length)
                    {
                        case 6:
                            switch (lower[2])
                            {
                                case 'j':
                                    if (lower == "injury")
                                    {
                                        o.OptionVisible = this.LODMedical.NatureOfIncidentDescription == "Injury (non MVA)";
                                        break;
                                    }
                                    break;

                                case 'n':
                                    if (lower == "nonmva")
                                    {
                                        int num = string.IsNullOrEmpty(this.LODMedical.MvaInvolved) ? 0 : (this.LODMedical.MvaInvolved == "No" ? 1 : 0);
                                        o.OptionVisible = num != 0;
                                        break;
                                    }
                                    break;

                                case 'r':
                                    if (lower == "formal" && Convert.ToBoolean(r.RuleValue.ToString().ToLower()) != this.Formal)
                                    {
                                        o.OptionVisible = false;
                                        break;
                                    }
                                    break;

                                case 's':
                                    if (lower == "issarc")
                                    {
                                        int num = !this.SarcCase ? 0 : (r.RuleValue.ToString().Equals("True") ? 1 : 0);
                                        o.OptionVisible = num != 0;
                                        break;
                                    }
                                    break;
                            }
                            break;

                        case 9:
                            if (lower == "8yearrule")
                            {
                                LineOfDutyFindings byType = this.FindByType((short)5);
                                int num = !this.LODUnit_v2.MemberCredible.Equals("Yes") || !byType.Description.Equals("Not ILOD - Not Due To Own Misconduct") || !this.LODUnit_v2.MemberOnOrders.Equals("Yes") ? 0 : (this.LODMedical_v2.MobilityStandards.Equals("Yes") ? 1 : 0);
                                o.OptionVisible = num != 0;
                                break;
                            }
                            break;

                        case 11:
                            switch (lower[0])
                            {
                                case 'd':
                                    if (lower == "directreply")
                                    {
                                        if (this.DirectReply && r.RuleValue.ToString().Equals("True"))
                                        {
                                            o.OptionVisible = true;
                                            break;
                                        }
                                        int num = this.DirectReply ? 0 : (r.RuleValue.ToString().Equals("False") ? 1 : 0);
                                        o.OptionVisible = num != 0;
                                        break;
                                    }
                                    break;

                                case 'w':
                                    if (lower == "wasinstatus")
                                    {
                                        bool flag = false;
                                        string[] source = r.RuleValue.ToString().Split(',');
                                        IList<WorkStatusTracking> statusTracking = lookupDao.GetStatusTracking(this.Id, this.ModuleType);
                                        if (statusTracking == null || statusTracking.Count == 0)
                                        {
                                            o.OptionVisible = false;
                                            break;
                                        }
                                        foreach (WorkStatusTracking workStatusTracking in (IEnumerable<WorkStatusTracking>)statusTracking)
                                        {
                                            if (((IEnumerable<string>)source).Contains<string>(workStatusTracking.WorkflowStatus.Id.ToString()))
                                            {
                                                flag = true;
                                                break;
                                            }
                                        }
                                        o.OptionVisible = flag;
                                        break;
                                    }
                                    break;
                            }
                            break;

                        case 13:
                            if (lower == "laststatuswas")
                            {
                                if (!((IEnumerable<string>)r.RuleValue.Split(',')).Contains<string>(lastStatus.ToString()))
                                {
                                    o.OptionVisible = false;
                                    break;
                                }
                                break;
                            }
                            break;

                        case 14:
                            switch (lower[0])
                            {
                                case 's':
                                    if (lower == "sarcrestricted")
                                    {
                                        bool flag = bool.Parse(r.RuleValue);
                                        o.OptionVisible = flag == this.IsRestricted;
                                        break;
                                    }
                                    break;

                                case 'w':
                                    if (lower == "wasnotinstatus")
                                    {
                                        bool flag = false;
                                        string[] source = r.RuleValue.ToString().Split(',');
                                        IList<WorkStatusTracking> statusTracking = lookupDao.GetStatusTracking(this.Id, this.ModuleType);
                                        if (statusTracking == null)
                                        {
                                            o.OptionVisible = false;
                                            break;
                                        }
                                        if (statusTracking.Count == 0)
                                        {
                                            o.OptionVisible = true;
                                            break;
                                        }
                                        foreach (WorkStatusTracking workStatusTracking in (IEnumerable<WorkStatusTracking>)statusTracking)
                                        {
                                            if (((IEnumerable<string>)source).Contains<string>(workStatusTracking.WorkflowStatus.Id.ToString()))
                                            {
                                                flag = true;
                                                break;
                                            }
                                        }
                                        o.OptionVisible = !flag;
                                        break;
                                    }
                                    break;
                            }
                            break;

                        case 15:
                            if (lower == "unitccquestions")
                            {
                                int num = string.IsNullOrEmpty(this.LODUnit_v2.MemberCredible) ? 0 : (!string.IsNullOrEmpty(this.LODUnit_v2.MemberOnOrders) ? 1 : 0);
                                o.OptionVisible = num != 0;
                                break;
                            }
                            break;

                        case 16:
                            if (lower == "laststatuswasnot")
                            {
                                if (((IEnumerable<string>)r.RuleValue.ToString().Split(',')).Contains<string>(lastStatus.ToString()))
                                {
                                    o.OptionVisible = false;
                                    break;
                                }
                                break;
                            }
                            break;

                        case 17:
                            switch (lower[0])
                            {
                                case 'b':
                                    if (lower == "boardfinalization")
                                    {
                                        int num = string.IsNullOrEmpty(this.LODMedical_v2.BoardFinalization) ? 1 : (this.LODMedical_v2.BoardFinalization.Equals("Yes") ? 1 : 0);
                                        o.OptionVisible = num == 0;
                                        break;
                                    }
                                    break;

                                case 'f':
                                    if (lower == "formalrecommended")
                                    {
                                        LineOfDutyFindings byType = this.FindByType((short)5);
                                        bool flag1 = !string.IsNullOrEmpty(r.RuleValue) && bool.Parse(r.RuleValue);
                                        bool flag2 = false;
                                        int num1;
                                        if (byType != null)
                                        {
                                            finding = byType.Finding;
                                            num1 = finding.HasValue ? 1 : 0;
                                        }
                                        else
                                            num1 = 0;
                                        if (num1 != 0)
                                        {
                                            finding = byType.Finding;
                                            int num2;
                                            if (finding.Value != (short)6)
                                            {
                                                finding = byType.Finding;
                                                num2 = finding.Value == (short)4 ? 1 : 0;
                                            }
                                            else
                                                num2 = 1;
                                            if (num2 != 0)
                                            {
                                                flag2 = true;
                                            }
                                            else
                                            {
                                                finding = byType.Finding;
                                                if (finding.Value == (short)5)
                                                {
                                                    nullable = this.NILODsubFinding;
                                                    int num3 = 1;
                                                    if (nullable.GetValueOrDefault() == num3 & nullable.HasValue)
                                                        flag2 = true;
                                                }
                                            }
                                        }
                                        o.OptionVisible = flag1 == flag2;
                                        break;
                                    }
                                    break;
                            }
                            break;

                        case 18:
                            if (lower == "icd9codenotpresent")
                            {
                                o.OptionVisible = !this.LODMedical_v2.IsSelectedICDCodeADisease(lookupDao);
                                break;
                            }
                            break;

                        case 19:
                            if (lower == "injurynonmvanonepts")
                            {
                                o.OptionVisible = false;
                                if (!string.IsNullOrEmpty(this.LODMedical.MvaInvolved) && this.LODMedical.MvaInvolved == "No")
                                {
                                    LineOfDutyFindings byType = this.FindByType((short)5);
                                    o.OptionVisible = false;
                                    int num4;
                                    if (byType != null)
                                    {
                                        finding = byType.Finding;
                                        num4 = finding.HasValue ? 1 : 0;
                                    }
                                    else
                                        num4 = 0;
                                    if (num4 != 0)
                                    {
                                        finding = byType.Finding;
                                        int num5;
                                        if (finding.Value != (short)3)
                                        {
                                            finding = byType.Finding;
                                            num5 = finding.Value == (short)1 ? 1 : 0;
                                        }
                                        else
                                            num5 = 1;
                                        if (num5 != 0)
                                        {
                                            o.OptionVisible = true;
                                        }
                                        else
                                        {
                                            finding = byType.Finding;
                                            if (finding.Value == (short)5)
                                            {
                                                int num6;
                                                if (this.NILODsubFinding.HasValue)
                                                {
                                                    nullable = this.NILODsubFinding;
                                                    num6 = nullable.Value == 2 ? 1 : 0;
                                                }
                                                else
                                                    num6 = 0;
                                                if (num6 != 0)
                                                    o.OptionVisible = true;
                                            }
                                        }
                                    }
                                    break;
                                }
                                break;
                            }
                            break;

                        case 20:
                            if (lower == "isreinvestigationlod")
                            {
                                o.OptionVisible = lookupDao.GetIsReinvestigationLod(this.Id) == Convert.ToBoolean(r.RuleValue.ToString().ToLower());
                                break;
                            }
                            break;

                        case 23:
                            if (lower == "cancelformalrecommended")
                            {
                                o.OptionVisible = this.IsFormalCancelRecommended;
                                break;
                            }
                            break;

                        case 25:
                            if (lower == "ispersonnelfindingpresent")
                            {
                                string[] strArray = r.RuleValue.Split(',');
                                if (strArray.Length != 2)
                                {
                                    o.OptionVisible = false;
                                    break;
                                }
                                LineOfDutyFindings byType = this.FindByType(short.Parse(strArray[0]));
                                int num;
                                if (byType != null)
                                {
                                    finding = byType.Finding;
                                    num = !finding.HasValue ? 1 : 0;
                                }
                                else
                                    num = 1;
                                if (num != 0)
                                {
                                    o.OptionVisible = false;
                                    break;
                                }
                                finding = byType.Finding;
                                o.OptionVisible = (int)finding.Value == (int)short.Parse(strArray[1]);
                                break;
                            }
                            break;
                    }
                }
            }
            if (r.RuleTypes.ruleTypeId != 2)
                return;
            string lower1 = r.RuleTypes.Name.ToLower();
            if (lower1 != null)
            {
                switch (lower1.Length)
                {
                    case 4:
                        switch (lower1[0])
                        {
                            case 'm':
                                if (lower1 == "memo")
                                {
                                    bool flag3 = true;
                                    bool flag4 = false;
                                    string empty = string.Empty;
                                    string[] memos = r.RuleValue.ToString().Split(',');
                                    for (int i = 0; i < memos.Length; i++)
                                    {
                                        if (!string.IsNullOrEmpty(memos[i]))
                                        {
                                            empty = ((MemoType)Convert.ToByte(memos[i])).ToString();
                                            IList<Memorandum> list = (IList<Memorandum>)memoDao.GetByRefId(this.Id).Where<Memorandum>((Func<Memorandum, bool>)(m => !m.Deleted && m.Template.Id == (int)Convert.ToByte(memos[i]))).ToList<Memorandum>();
                                            if (lookupDao.GetIsReinvestigationLod(this.Id) && list.Count > 0 && Convert.ToByte(memos[i]) == (byte)5)
                                            {
                                                DateTime dateTime = DateTime.MinValue;
                                                foreach (Memorandum memorandum in (IEnumerable<Memorandum>)list)
                                                {
                                                    if (memorandum.Contents[0] != null && memorandum.Contents[0].CreatedDate > dateTime)
                                                        dateTime = memorandum.Contents[0].CreatedDate;
                                                }
                                                if (dateTime < this.CreatedDate)
                                                {
                                                    flag3 = false;
                                                    this.AddValidationItem(new ValidationItem("Memos", "Memo", "New " + memoDao.GetAllTemplates().Where<MemoTemplate>((Func<MemoTemplate, bool>)(m => m.Id == (int)Convert.ToByte(memos[i]))).Select<MemoTemplate, string>((Func<MemoTemplate, string>)(m => m.Title)).Single<string>() + "  Memo  not found."));
                                                }
                                                else
                                                    flag4 = true;
                                            }
                                            else if (list.Count > 0)
                                            {
                                                flag4 = true;
                                            }
                                            else
                                            {
                                                flag3 = false;
                                                this.AddValidationItem(new ValidationItem("Memos", "Memo", memoDao.GetAllTemplates().Where<MemoTemplate>((Func<MemoTemplate, bool>)(m => m.Id == (int)Convert.ToByte(memos[i]))).Select<MemoTemplate, string>((Func<MemoTemplate, string>)(m => m.Title)).Single<string>() + "  Memo  not found."));
                                            }
                                        }
                                    }
                                    bool? checkAll = r.CheckAll;
                                    o.OptionValid = !checkAll.Value ? flag4 : flag3;
                                    break;
                                }
                                break;

                            case 'u':
                                if (lower1 == "unit" && !this.ModuleStatus[r.RuleTypes.Name.ToLower()])
                                {
                                    o.OptionValid = this.ModuleStatus[r.RuleTypes.Name.ToLower()];
                                    break;
                                }
                                break;
                        }
                        break;

                    case 7:
                        switch (lower1[0])
                        {
                            case 'c':
                                if (lower1 == "canrwoa")
                                {
                                    LineOfDutyFindings byType = this.FindByType((short)6);
                                    int num;
                                    if (byType != null)
                                    {
                                        finding = byType.Finding;
                                        num = finding.HasValue ? 1 : 0;
                                    }
                                    else
                                        num = 0;
                                    if (num != 0)
                                    {
                                        o.OptionValid = false;
                                        this.AddValidationItem(new ValidationItem("Board Review", "Board Review", "To return to wing level no finding can be present"));
                                        break;
                                    }
                                    o.OptionValid = true;
                                    break;
                                }
                                break;

                            case 'm':
                                if (lower1 == "medical" && !this.ModuleStatus[r.RuleTypes.Name.ToLower()])
                                {
                                    o.OptionValid = this.ModuleStatus[r.RuleTypes.Name.ToLower()];
                                    break;
                                }
                                break;
                        }
                        break;

                    case 8:
                        if (lower1 == "document")
                        {
                            string[] strArray = r.RuleValue.ToString().Split(',');
                            bool flag6 = true;
                            bool flag7 = false;
                            for (int index = 0; index < strArray.Length; ++index)
                            {
                                if (!string.IsNullOrEmpty(strArray[index]))
                                {
                                    string key = strArray[index];
                                    if (this.Active.ContainsKey(key) && !this.Required[key])
                                        flag6 = false;
                                    else
                                        flag7 = true;
                                }
                            }
                            bool? checkAll = r.CheckAll;
                            o.OptionValid = !checkAll.Value ? flag7 : flag6;
                            break;
                        }
                        break;

                    case 13:
                        switch (lower1[0])
                        {
                            case 'c':
                                if (lower1 == "canrwoaformal")
                                {
                                    LineOfDutyFindings byType = this.FindByType((short)14);
                                    int num;
                                    if (byType != null)
                                    {
                                        finding = byType.Finding;
                                        num = finding.HasValue ? 1 : 0;
                                    }
                                    else
                                        num = 0;
                                    if (num != 0)
                                    {
                                        o.OptionValid = false;
                                        this.AddValidationItem(new ValidationItem("Formal Board Review", "Formal Board Review", "To return to wing level no finding can be present"));
                                        break;
                                    }
                                    o.OptionValid = true;
                                    break;
                                }
                                break;

                            case 'i':
                                if (lower1 == "investigation" && !this.ModuleStatus[r.RuleTypes.Name.ToLower()])
                                {
                                    o.OptionValid = this.ModuleStatus[r.RuleTypes.Name.ToLower()];
                                    break;
                                }
                                break;
                        }
                        break;

                    case 14:
                        switch (lower1[4])
                        {
                            case 'c':
                                if (lower1 == "wingccfindings")
                                {
                                    LineOfDutyFindings byType = this.FindByType((short)5);
                                    if (byType != null)
                                    {
                                        o.OptionValid = true;
                                        finding = byType.Finding;
                                        if (!finding.HasValue)
                                        {
                                            o.OptionValid = false;
                                            this.AddValidationItem(new ValidationItem("Findings", "Appointing Authority", "Appointing Authority Findings not present", true));
                                        }
                                        if (this.CurrentStatusCode != 296)
                                            break;
                                        break;
                                    }
                                    o.OptionValid = false;
                                    this.AddValidationItem(new ValidationItem("Findings", "Appointing Authority", "Appointing Authority Findings not present", true));
                                    break;
                                }
                                break;

                            case 'j':
                                if (lower1 == "wingjafindings")
                                {
                                    LineOfDutyFindings byType = this.FindByType((short)4);
                                    if (byType != null)
                                    {
                                        if (byType.DecisionYN == "Y")
                                            o.OptionValid = true;
                                        else if (byType.DecisionYN == "N")
                                        {
                                            finding = byType.Finding;
                                            o.OptionValid = finding.HasValue;
                                        }
                                        else
                                            o.OptionValid = false;
                                    }
                                    else
                                        o.OptionValid = false;
                                    if (!o.OptionValid)
                                    {
                                        this.AddValidationItem(new ValidationItem("Findings", "Wing JA", "Wing Judge Advocate Findings not present", true));
                                        break;
                                    }
                                    break;
                                }
                                break;
                        }
                        break;

                    case 16:
                        if (lower1 == "wingccioassigned")
                        {
                            bool flag = true;
                            if (string.IsNullOrEmpty(this.IoSsn) && this.AppointedIO == null)
                            {
                                flag = false;
                                this.AddValidationItem(new ValidationItem("Formal Investigation", "Investigating Officer", "Investigating Officer must be selected"));
                            }
                            DateTime? ioCompletionDate = this.IoCompletionDate;
                            if (!ioCompletionDate.HasValue)
                            {
                                flag = false;
                                this.AddValidationItem(new ValidationItem("Formal Investigation", "Investigation Completion Date", "Date investigation to be completed by must be entered"));
                            }
                            ioCompletionDate = this.IoCompletionDate;
                            if (ioCompletionDate.HasValue)
                            {
                                ioCompletionDate = this.IoCompletionDate;
                                if (ioCompletionDate.Value < DateTime.Now)
                                {
                                    flag = false;
                                    this.AddValidationItem(new ValidationItem("Formal Investigation", "Investigation Completion Date", "Date Investigation to be completed by can not be a past date."));
                                }
                            }
                            o.OptionValid = flag;
                            break;
                        }
                        break;

                    case 18:
                        if (lower1 == "wingccformalaction")
                        {
                            LineOfDutyFindings byType = this.FindByType((short)13);
                            if (byType != null)
                            {
                                if (byType.DecisionYN == "Y")
                                {
                                    o.OptionValid = true;
                                    break;
                                }
                                if (byType.DecisionYN == "N")
                                {
                                    finding = byType.Finding;
                                    if (finding.HasValue)
                                    {
                                        finding = byType.Finding;
                                        if (finding.Value != (short)6)
                                        {
                                            o.OptionValid = true;
                                            break;
                                        }
                                        o.OptionValid = false;
                                        this.AddValidationItem(new ValidationItem("Findings", "Formal Appointing Authority", "Formal Investigation has been directed.", true));
                                        break;
                                    }
                                    o.OptionValid = false;
                                    this.AddValidationItem(new ValidationItem("Findings", "Formal Appointing Authority", "Formal Action by Appointing Authority not present", true));
                                    break;
                                }
                                o.OptionValid = false;
                                this.AddValidationItem(new ValidationItem("Findings", "Formal Appointing Authority", "Formal Action by Appointing Authority not present", true));
                                break;
                            }
                            o.OptionValid = false;
                            this.AddValidationItem(new ValidationItem("Findings", "Formal Appointing Authority", "Formal Action by Appointing Authority not present", true));
                            break;
                        }
                        break;

                    case 19:
                        if (lower1 == "appointedioverified")
                        {
                            bool flag = true;
                            if (this.AppointedIO == null)
                            {
                                this.AddValidationItem(new ValidationItem("Formal Investigation", "Investigating Officer", "A valid Investigating Officer has not been appointed!"));
                                flag = false;
                            }
                            else if (string.IsNullOrEmpty(this.AppointedIO.EDIPIN))
                            {
                                this.AddValidationItem(new ValidationItem("Formal Investigation", "Investigating Officer", "The appointed Investigating Officer does not have an EDIPI!"));
                                flag = false;
                            }
                            else if (!this.AppointedIO.AccountExpiration.HasValue)
                            {
                                this.AddValidationItem(new ValidationItem("Formal Investigation", "Investigating Officer", "The appointed Investigating Officer does not have an IA Training Date!"));
                                flag = false;
                            }
                            else
                            {
                                DateTime now = this.AppointedIO.AccountExpiration.Value;
                                DateTime date1 = now.Date;
                                now = DateTime.Now;
                                DateTime date2 = now.Date;
                                if (date1 <= date2)
                                {
                                    this.AddValidationItem(new ValidationItem("Formal Investigation", "Investigating Officer", "The appointed Investigating Officer has an expired IA Training Date!"));
                                    flag = false;
                                }
                                else if (this.AppointedIO.Unit == null)
                                {
                                    this.AddValidationItem(new ValidationItem("Formal Investigation", "Investigating Officer", "The appointed Investigating Officer does not belong to a unit!"));
                                    flag = false;
                                }
                            }
                            o.OptionValid = flag;
                            break;
                        }
                        break;

                    case 20:
                        switch (lower1[0])
                        {
                            case 'b':
                                if (lower1 == "boardfindingspresent")
                                {
                                    LineOfDutyFindings byType1 = this.FindByType((short)10);
                                    LineOfDutyFindings byType2 = this.FindByType((short)6);
                                    int num7;
                                    if (byType1 != null)
                                    {
                                        finding = byType1.Finding;
                                        num7 = finding.HasValue ? 1 : 0;
                                    }
                                    else
                                        num7 = 0;
                                    if (num7 != 0)
                                    {
                                        o.OptionValid = true;
                                        break;
                                    }
                                    int num8;
                                    if (byType2 != null)
                                    {
                                        finding = byType2.Finding;
                                        num8 = finding.HasValue ? 1 : 0;
                                    }
                                    else
                                        num8 = 0;
                                    if (num8 != 0)
                                    {
                                        nullable = this.ApprovingAuthorityUserId;
                                        if (nullable.HasValue)
                                        {
                                            o.OptionValid = true;
                                            break;
                                        }
                                        this.AddValidationItem(new ValidationItem("Findings", "Approving Authority", "Board Technician must select an Approving Authority on the LOD Board Page in order to complete the case without going throught the Approving Authority Action step."));
                                        o.OptionValid = false;
                                        break;
                                    }
                                    this.AddValidationItem(new ValidationItem("Findings", "Approving Authority", "Approving Authority Findings not found."));
                                    o.OptionValid = false;
                                    break;
                                }
                                break;

                            case 'w':
                                if (lower1 == "wingjaformalfindings")
                                {
                                    LineOfDutyFindings byType = this.FindByType((short)12);
                                    if (byType != null)
                                    {
                                        if (byType.DecisionYN == "Y" && r.RuleValue.Equals("True"))
                                        {
                                            o.OptionValid = true;
                                            break;
                                        }
                                        if (byType.DecisionYN == "N" && r.RuleValue.Equals("False"))
                                        {
                                            finding = byType.Finding;
                                            o.OptionValid = finding.HasValue;
                                            break;
                                        }
                                        o.OptionValid = false;
                                        break;
                                    }
                                    o.OptionValid = false;
                                    this.AddValidationItem(new ValidationItem("Findings", "Formal Wing JA", "Wing Judge Advocate Formal Action not present", true));
                                    break;
                                }
                                break;
                        }
                        break;

                    case 22:
                        switch (lower1[6])
                        {
                            case '1':
                                if (lower1 == "boarda1findingspresent")
                                {
                                    bool flag = true;
                                    LineOfDutyFindings byType = this.FindByType((short)22);
                                    int num;
                                    if (byType != null)
                                    {
                                        finding = byType.Finding;
                                        num = finding.HasValue ? 0 : (!(byType.DecisionYN == "Y") ? 1 : 0);
                                    }
                                    else
                                        num = 1;
                                    if (num != 0)
                                    {
                                        this.AddValidationItem(new ValidationItem("Findings", "Board Admin", "Board Admin findings not found."));
                                        flag = false;
                                    }
                                    o.OptionValid = flag;
                                    break;
                                }
                                break;

                            case 'a':
                                if (lower1 == "boardaafindingspresent")
                                {
                                    LineOfDutyFindings byType = this.FindByType((short)10);
                                    int num;
                                    if (byType != null)
                                    {
                                        finding = byType.Finding;
                                        num = finding.HasValue ? 1 : 0;
                                    }
                                    else
                                        num = 0;
                                    if (num != 0)
                                    {
                                        o.OptionValid = true;
                                        break;
                                    }
                                    this.AddValidationItem(new ValidationItem("Findings", "Approving Authority", "Approving Authority findings not found."));
                                    o.OptionValid = false;
                                    break;
                                }
                                break;

                            case 'r':
                                if (lower1 == "boardsrfindingspresent")
                                {
                                    LineOfDutyFindings byType = this.FindByType((short)9);
                                    int num;
                                    if (byType != null)
                                    {
                                        finding = byType.Finding;
                                        num = finding.HasValue ? 1 : (byType.DecisionYN == "Y" ? 1 : 0);
                                    }
                                    else
                                        num = 0;
                                    if (num != 0)
                                    {
                                        o.OptionValid = true;
                                        break;
                                    }
                                    this.AddValidationItem(new ValidationItem("Findings", "Board Senior Reviewer", "Board Senior Reviewer findings not found."));
                                    o.OptionValid = false;
                                    break;
                                }
                                break;
                        }
                        break;

                    case 23:
                        if (lower1 == "isnonmvainjuryandeptssa")
                            break;
                        break;

                    case 25:
                        switch (lower1[0])
                        {
                            case 'b':
                                if (lower1 == "boardlegalfindingspresent")
                                {
                                    LineOfDutyFindings byType = this.FindByType((short)7);
                                    int num;
                                    if (byType != null)
                                    {
                                        finding = byType.Finding;
                                        num = finding.HasValue ? 1 : (byType.DecisionYN == "Y" ? 1 : 0);
                                    }
                                    else
                                        num = 0;
                                    if (num != 0)
                                    {
                                        o.OptionValid = true;
                                        break;
                                    }
                                    this.AddValidationItem(new ValidationItem("Findings", "Board Legal", "Board Legal findings not found."));
                                    o.OptionValid = false;
                                    break;
                                }
                                break;

                            case 'w':
                                if (lower1 == "wingccfindingsforcomplete")
                                {
                                    o.OptionValid = this.ValidateWingCcFindingsForCompleteRule(lookupDao);
                                    break;
                                }
                                break;
                        }
                        break;

                    case 26:
                        switch (lower1[10])
                        {
                            case '1':
                                if (lower1 == "frmlboarda1findingspresent")
                                {
                                    bool flag = true;
                                    LineOfDutyFindings byType = this.FindByType((short)23);
                                    if (byType == null || byType.DecisionYN == null || !(byType.DecisionYN != ""))
                                    {
                                        this.AddValidationItem(new ValidationItem("Findings", "Formal Board Admin", "Formal Board Admin findings not found."));
                                        flag = false;
                                    }
                                    o.OptionValid = flag;
                                    break;
                                }
                                break;

                            case 'a':
                                if (lower1 == "frmlboardaafindingspresent")
                                {
                                    LineOfDutyFindings byType = this.FindByType((short)18);
                                    int num;
                                    if (byType != null)
                                    {
                                        finding = byType.Finding;
                                        num = finding.HasValue ? 1 : 0;
                                    }
                                    else
                                        num = 0;
                                    if (num != 0)
                                    {
                                        o.OptionValid = true;
                                        break;
                                    }
                                    this.AddValidationItem(new ValidationItem("Findings", "Formal Approving Authority", "Formal Approving Authority findings not found."));
                                    o.OptionValid = false;
                                    break;
                                }
                                break;

                            case 'd':
                                if (lower1 == "formalboardfindingspresent")
                                {
                                    LineOfDutyFindings byType3 = this.FindByType((short)18);
                                    LineOfDutyFindings byType4 = this.FindByType((short)14);
                                    int num9;
                                    if (byType3 != null)
                                    {
                                        finding = byType3.Finding;
                                        num9 = finding.HasValue ? 1 : 0;
                                    }
                                    else
                                        num9 = 0;
                                    if (num9 != 0)
                                    {
                                        o.OptionValid = true;
                                        break;
                                    }
                                    int num10;
                                    if (byType4 != null)
                                    {
                                        finding = byType4.Finding;
                                        num10 = finding.HasValue ? 1 : 0;
                                    }
                                    else
                                        num10 = 0;
                                    if (num10 != 0)
                                    {
                                        nullable = this.ApprovingAuthorityUserId;
                                        if (nullable.HasValue)
                                        {
                                            o.OptionValid = true;
                                            break;
                                        }
                                        this.AddValidationItem(new ValidationItem("Findings", "Formal Approving Authority", "Board Technician must select an Approving Authority on the LOD Board Page in order to complete the case without going throught the Formal Approving Authority Action step."));
                                        o.OptionValid = false;
                                        break;
                                    }
                                    this.AddValidationItem(new ValidationItem("Findings", "Formal Approving Authority", "Formal Approving Authority Findings not found."));
                                    o.OptionValid = false;
                                    break;
                                }
                                break;

                            case 'r':
                                if (lower1 == "frmlboardsrfindingspresent")
                                {
                                    LineOfDutyFindings byType = this.FindByType((short)17);
                                    if (byType != null && byType.DecisionYN != null && byType.DecisionYN != "")
                                    {
                                        o.OptionValid = true;
                                        break;
                                    }
                                    this.AddValidationItem(new ValidationItem("Findings", "Formal Board Senior", "Formal Board Senior Reviewer findings not found."));
                                    o.OptionValid = false;
                                    break;
                                }
                                break;
                        }
                        break;

                    case 27:
                        if (lower1 == "boardmedicalfindingspresent")
                        {
                            LineOfDutyFindings byType = this.FindByType((short)8);
                            int num;
                            if (byType != null)
                            {
                                finding = byType.Finding;
                                num = finding.HasValue ? 1 : (byType.DecisionYN == "Y" ? 1 : 0);
                            }
                            else
                                num = 0;
                            if (num != 0)
                            {
                                o.OptionValid = true;
                                break;
                            }
                            this.AddValidationItem(new ValidationItem("Findings", "Board Surgeon", "Board Surgeon findings not found."));
                            o.OptionValid = false;
                            break;
                        }
                        break;

                    case 29:
                        if (lower1 == "frmlboardlegalfindingspresent")
                        {
                            LineOfDutyFindings byType = this.FindByType((short)15);
                            if (byType != null && byType.DecisionYN != null && byType.DecisionYN != "")
                            {
                                o.OptionValid = true;
                                break;
                            }
                            this.AddValidationItem(new ValidationItem("Findings", "Formal Board Legal", "Formal Board Legal findings not found."));
                            o.OptionValid = false;
                            break;
                        }
                        break;

                    case 31:
                        if (lower1 == "frmlboardmedicalfindingspresent")
                        {
                            LineOfDutyFindings byType = this.FindByType((short)16);
                            if (byType != null && byType.DecisionYN != null && byType.DecisionYN != "")
                            {
                                o.OptionValid = true;
                                break;
                            }
                            this.AddValidationItem(new ValidationItem("Findings", "Formal Board Surgeon", "Formal Board Surgeon findings not found."));
                            o.OptionValid = false;
                            break;
                        }
                        break;
                }
            }
        }

        private void ProcessOption(int lastStatus, IDaoFactory daoFactor)
        {
            foreach (WorkflowStatusOption o in this.WorkflowStatus.WorkStatusOptionList.Where<WorkflowStatusOption>((Func<WorkflowStatusOption, bool>)(o => o.Compo == this.MemberCompo || string.Equals(o.Compo, this.MemberCompo) || string.IsNullOrEmpty(o.Compo) || o.Compo == "0")))
            {
                bool flag = true;
                foreach (WorkflowOptionRule r in o.RuleList.Where<WorkflowOptionRule>((Func<WorkflowOptionRule, bool>)(r => r.RuleTypes.ruleTypeId == 1)))
                {
                    this.ApplyRulesToOption(o, r, lastStatus, daoFactor);
                    if (!o.OptionVisible)
                        flag = false;
                }
                if (flag)
                    this.RuleAppliedOptions.Add(o);
            }
            foreach (WorkflowStatusOption o in this.RuleAppliedOptions.Select<WorkflowStatusOption, WorkflowStatusOption>((Func<WorkflowStatusOption, WorkflowStatusOption>)(o => o)))
            {
                bool flag = true;
                foreach (WorkflowOptionRule r in o.RuleList.Where<WorkflowOptionRule>((Func<WorkflowOptionRule, bool>)(r => r.RuleTypes.ruleTypeId == 2)))
                {
                    this.ApplyRulesToOption(o, r, lastStatus, daoFactor);
                    if (!o.OptionValid)
                        flag = false;
                }
                o.OptionValid = flag;
            }
        }

        private void UpdateActiveCategories(string section, bool isReqd)
        {
            if (this.Active.ContainsKey(section))
                this.Active[section] = isReqd;
            if (this.Active.ContainsKey(section))
                return;
            this.Active.Add(section, isReqd);
        }

        private void ValidateModule(string section, IValidatable item, int userid)
        {
            bool isValid = item.Validate(userid);
            this.AddValidationItem(item.ValidationItems);
            this.AddModuleStatus(section, isValid);
        }

        private bool ValidateWingCcFindingsForCompleteRule(ILookupDao lookupDao)
        {
            bool flag = false;
            LineOfDutyFindings byType = this.FindByType((short)5);
            short? finding;
            int num1;
            if (byType != null)
            {
                finding = byType.Finding;
                num1 = finding.HasValue ? 1 : 0;
            }
            else
                num1 = 0;
            int? nullable1;
            if (num1 != 0)
            {
                finding = byType.Finding;
                int? nullable2 = finding.HasValue ? new int?((int)finding.GetValueOrDefault()) : new int?();
                int num2 = 1;
                if (nullable2.GetValueOrDefault() == num2 & nullable2.HasValue)
                    flag = true;
                finding = byType.Finding;
                int? nullable3 = finding.HasValue ? new int?((int)finding.GetValueOrDefault()) : new int?();
                int num3 = 5;
                int num4;
                if (nullable3.GetValueOrDefault() == num3 & nullable3.HasValue)
                {
                    nullable1 = this.NILODsubFinding;
                    if (nullable1.HasValue)
                    {
                        nullable1 = this.NILODsubFinding;
                        num4 = nullable1.Value == 2 ? 1 : 0;
                        goto label_10;
                    }
                }
                num4 = 0;
            label_10:
                if (num4 != 0)
                    flag = true;
            }
            if (!flag)
            {
                this.AddValidationItem(new ValidationItem("Findings", "Appointing Authority Complete", "Appointing Authority Findings not enough to allow closing.", true));
                int num5;
                if (byType != null)
                {
                    finding = byType.Finding;
                    if (finding.HasValue)
                    {
                        finding = byType.Finding;
                        nullable1 = finding.HasValue ? new int?((int)finding.GetValueOrDefault()) : new int?();
                        int num6 = 5;
                        if (nullable1.GetValueOrDefault() == num6 & nullable1.HasValue)
                        {
                            nullable1 = this.NILODsubFinding;
                            num5 = !nullable1.HasValue ? 1 : 0;
                            goto label_19;
                        }
                    }
                }
                num5 = 0;
            label_19:
                if (num5 != 0)
                    this.AddValidationItem(new ValidationItem("Findings", "Appointing Authority Complete", "Appointing Authority NILOD finding needs reason.", true));
            }
            return flag;
        }
    }
}