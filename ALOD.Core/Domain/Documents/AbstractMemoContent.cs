using ALOD.Core.Domain.Users;
using ALOD.Core.Utils;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace ALOD.Core.Domain.Documents
{
    public class AbstractMemoContent : Entity
    {
        private string _formattedContent;
        private string _memoForHeader;

        public AbstractMemoContent()
        { }

        public AbstractMemoContent(AbstractMemorandum memo)
        {
            Parent = memo;
        }

        public virtual string Attachments { get; set; }
        public virtual string Body { get; set; }
        public virtual AppUser CreatedBy { get; set; }
        public virtual DateTime CreatedDate { get; set; }

        public virtual string FormattedContent
        {
            get
            {
                if (string.IsNullOrEmpty(_formattedContent))
                {
                    _formattedContent = GetFormattedContent();
                }

                return _formattedContent;
            }
        }

        public virtual string MemoDate { get; set; }

        public virtual string MemoForHeader
        {
            get
            {
                if (string.IsNullOrEmpty(_memoForHeader))
                {
                    _memoForHeader = GetMemoForHeader();
                }

                return _memoForHeader;
            }
        }

        public virtual AbstractMemorandum Parent { get; set; }
        public virtual string SignatureBlock { get; set; }
        public virtual string SuspenseDate { get; set; }

        public virtual bool ShouldWrap(int templateId)
        {
            if (templateId == (int)MemoType.LodFindingsILOD || templateId == (int)MemoType.LodFindingsILODDeath ||
               templateId == (int)MemoType.LodFindingsNLOD || templateId == (int)MemoType.LodFindingsNLODDeath ||
               templateId == (int)MemoType.ReinvestigationRequestApproved || templateId == (int)MemoType.ReinvestigationRequestDenied ||
               templateId == (int)MemoType.ApprovalAppeal || templateId == (int)MemoType.DisapprovalAppeal ||
               templateId == (int)MemoType.SARC_Determination_ILOD || templateId == (int)MemoType.SARC_Determination_NILOD ||
               templateId == (int)MemoType.SARC_APPEAL_APPROVED || templateId == (int)MemoType.SARC_APPEAL_DISAPPROVAL)
            {
                return true;
            }

            return false;
        }

        protected virtual string GetFormattedContent()
        {
            string SigBlockOffset = MemoHelpers.RepeatString(" ", 71);
            bool useNewSigProcessing = false;
            string newline = Environment.NewLine;
            string content = string.Empty;

            if (Parent.Template.FontSize != 0)
            {
                content = MemoHelpers.ProcessSubjectLine(Body, Parent.Template.FontSize);
            }
            else
            {
                content = MemoHelpers.ProcessSubjectLine(Body, 12);
            }

            if (ShouldWrap(Parent.Template.Id))
            {
                content = MemoHelpers.ProcessOffsetPlaceholders(content);
            }
            else
            {
                content = MemoHelpers.ProcessOffsetPlaceholders(content);
            }

            content = MemoHelpers.ProcessPageBreaks(content);

            useNewSigProcessing = content.Contains("{SIGNATURE_BLOCK}");    // Older memos had the {SIGNATURE_BLOCK} placeholder removed when generating the body portion of the memo content

            if (useNewSigProcessing && !string.IsNullOrEmpty(SignatureBlock))
            {
                content = MemoHelpers.ProcessSignatureLine(content, SignatureBlock, Parent.Template.FontSize, ShouldWrap(Parent.Template.Id));
            }

            StringBuilder buffer = new StringBuilder();
            buffer.Append("<Pre>");
            buffer.Append(content);

            if (SignatureBlock != null && !useNewSigProcessing)
            {
                buffer.Append(MemoHelpers.RepeatString(newline, 5));
                buffer.Append(SigBlockOffset);
                buffer.Append(SignatureBlock.Replace(newline, newline + SigBlockOffset));
            }

            //add attachment if it's there
            if (!string.IsNullOrEmpty(Attachments))
            {
                buffer.Append(MemoHelpers.RepeatString(newline, 3));
                buffer.Append(Attachments);
            }

            buffer.Append("</Pre>");

            return buffer.Replace(Environment.NewLine, "<br />").ToString();
        }

        protected virtual string GetMemoForHeader()
        {
            Regex regex = new Regex(
                "MEMORANDUM FOR:\\s*?([\\w\\s]+)<br />",
                RegexOptions.IgnoreCase |
                RegexOptions.CultureInvariant |
                RegexOptions.Compiled
                );

            Match m = regex.Match(FormattedContent);

            if (m.Success)
            {
                return "MEMORANDUM FOR: " + m.Groups[m.Groups.Count - 1].Value;
            }

            return "";
        }
    }
}