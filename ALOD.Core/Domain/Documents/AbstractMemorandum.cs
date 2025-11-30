using ALOD.Core.Domain.Common.KeyVal;
using ALOD.Core.Domain.Users;
using ALOD.Core.Interfaces.DAOInterfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web;
using WebSupergoo.ABCpdf8;

namespace ALOD.Core.Domain.Documents
{
    public abstract class AbstractMemorandum : Entity
    {
        public const int BOTTOM = 120;
        public const string DATE_FORMAT = "d MMMM yyyy";
        public const int LEFT = 70;
        public const int WIDTH = 468;
        private IList<string> contentPages;
        private string WEB_DOC_ROOT = "~/Secure/Documents/";

        public AbstractMemorandum()
        {
            contentPages = new List<string>();

            if (HttpContext.Current != null)
            {
                DocumentRoot = HttpContext.Current.Server.MapPath(WEB_DOC_ROOT);
            }
            else
            {
                DocumentRoot = "";
            }
        }

        public virtual IList<AbstractMemoContent> Contents { get; protected set; }
        public virtual AppUser CreatedBy { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual bool Deleted { get; set; }
        public virtual string DocumentRoot { get; set; }
        public virtual string FileName { get; set; }
        public virtual MemoLetterhead Letterhead { get; set; }

        public virtual int PageCount
        {
            get { return contentPages.Count; }
        }

        public virtual int ReferenceId { get; set; }
        public virtual MemoTemplate Template { get; set; }
        protected IDaoFactory factory { get; set; }

        public virtual void AddContent(AbstractMemoContent content)
        {
            if (Contents == null)
            {
                Contents = new List<AbstractMemoContent>();
            }

            content.Parent = this;
            Contents.Add(content);
        }

        public virtual Doc ToPdf(IDaoFactory daoFactory)
        {
            if (daoFactory == null)
            {
                return null;
            }

            factory = daoFactory;

            return ToPdf(false);
        }

        protected virtual Doc GeneratePDFForContent(AbstractMemoContent content)
        {
            Doc pdf = new Doc();

            pdf.Width = 4;
            pdf.Font = pdf.AddFont(Letterhead.Font);
            if (Template.FontSize != 0)
            {
                pdf.FontSize = Template.FontSize;
            }
            else
            {
                pdf.FontSize = 12;
            }

            InsertContent(pdf, content);

            pdf.Font = pdf.AddFont(Letterhead.Font);
            pdf.FontSize = Letterhead.HeaderTitleSize;
            pdf.TextStyle.Bold = true;
            pdf.Units = UnitType.Points;

            InsertFOUOWatermark(pdf);
            InsertLetterHead(pdf);
            InsertDate(pdf, content);

            return pdf;
        }

        protected virtual void InsertContent(Doc pdf, AbstractMemoContent content)
        {
            pdf.HPos = 0;
            pdf.Rect.SetRect(LEFT, BOTTOM, WIDTH, 510);

            int id = pdf.AddHtml(content.FormattedContent);

            while (pdf.Chainable(id))
            {
                pdf.Page = pdf.AddPage();

                if (content.MemoForHeader.Length > 0)
                {
                    //add the memofor
                    pdf.Rect.SetRect(LEFT, 708, WIDTH, 588);
                    pdf.AddHtml(content.MemoForHeader);

                    //add the content
                    pdf.Rect.SetRect(LEFT, BOTTOM, WIDTH, 588);
                    id = pdf.AddHtml("", id);
                }
                else
                {
                    //add the content
                    pdf.Rect.SetRect(LEFT, BOTTOM, WIDTH, 588);
                    id = pdf.AddHtml("", id);
                }
            }
        }

        protected virtual void InsertDate(Doc pdf, AbstractMemoContent content)
        {
            pdf.PageNumber = 1;
            pdf.FontSize = Template.FontSize;
            pdf.Rect.SetRect(LEFT, 645, WIDTH, Letterhead.FontSize);
            pdf.HPos = 1.0;
            pdf.AddHtml(content.MemoDate);
        }

        protected virtual void InsertFooterImages(Doc pdf)
        {
            int count = 1;

            while (count <= pdf.PageCount)
            {
                pdf.PageNumber = count;

                XImage footerImage = Letterhead.GetCenterFooterImage(DocumentRoot);
                double x = (pdf.MediaBox.Width / 2) - (footerImage.Width / 2);

                pdf.Rect.SetRect(x, 30, footerImage.Width, footerImage.Height);
                pdf.AddImageObject(footerImage, true);
                count++;
            }
        }

        protected virtual void InsertFOUOWatermark(Doc pdf)
        {
            int count = 1;

            while (count <= pdf.PageCount)
            {
                pdf.Pos.X = 0;
                pdf.Pos.Y = pdf.Rect.Bottom + 24; //_doc.Rect.Top / 2
                double x = (WIDTH / 2) - 75;
                pdf.Rect.SetRect(x, 0, 300, 24);
                pdf.PageNumber = count;
                pdf.AddHtml("<p align=center><StyleRun rotate=0><Font face='arial-bold' color=#FF0000/90>" + ConfigurationManager.AppSettings["WaterMark"] + "</Font></StyleRun></p>");
                count++;
            }
        }

        protected virtual void InsertHeaderLogos(Doc pdf)
        {
            int count = 1;

            while (count <= pdf.PageCount)
            {
                pdf.PageNumber = count;

                if (count == 1)
                {
                    if (!string.IsNullOrEmpty(Letterhead.LogoImageLeft))
                    {
                        pdf.Rect.SetRect(LEFT, 690, LEFT, LEFT);
                        pdf.AddImageObject(Letterhead.GetLeftLogoImage(DocumentRoot), true);
                    }

                    if (!string.IsNullOrEmpty(Letterhead.LogoImageRight))
                    {
                        pdf.Rect.SetRect(WIDTH, 690, LEFT, LEFT);
                        pdf.AddImageObject(Letterhead.GetRightLogoImage(DocumentRoot), true);
                    }
                }
                else
                {
                    pdf.FontSize = Letterhead.FontSize;
                    pdf.Rect.SetRect(WIDTH, 690, LEFT, LEFT);
                    pdf.HPos = 1.0;
                    pdf.AddHtml(count.ToString());
                }

                count++;
            }
        }

        protected virtual void InsertHeaderText(Doc pdf)
        {
            pdf.PageNumber = 1;
            XColor prevColor = new XColor();
            prevColor.SetColor(pdf.Color);

            pdf.TextStyle.Bold = true;
            pdf.Color.SetColor(Letterhead.HeaderFontColor);
            pdf.Rect.SetRect(LEFT, 746, WIDTH, Letterhead.HeaderTitleSize);
            pdf.HPos = 0.5;
            pdf.AddHtml(Letterhead.HeaderTitle);

            pdf.TextStyle.Bold = false;
            pdf.FontSize = Letterhead.HeaderSubtitleSize;
            pdf.Rect.SetRect(LEFT, 734, WIDTH, Letterhead.HeaderSubtitleSize);
            pdf.AddHtml(Letterhead.HeaderSubtitle);

            pdf.Color.SetColor(prevColor);
        }

        protected virtual void InsertLetterHead(Doc pdf)
        {
            InsertHeaderText(pdf);
            InsertHeaderLogos(pdf);
            InsertFooterImages(pdf);
            InsertPrivacyAct(pdf);
        }

        protected virtual void InsertPrivacyAct(Doc pdf)
        {
            pdf.PageNumber = 1;

            IKeyValDao keyDao = factory.GetKeyValDao();
            IList<KeyValValue> values = keyDao.GetKeyValuesByKeyDesciption("Privacy Act Footer");

            string privacy = values[0].Value;

            pdf.FontSize = Template.FontSize;
            pdf.Rect.SetRect(LEFT, 50, WIDTH, 50);
            pdf.HPos = 0;
            pdf.AddHtml(privacy);
        }

        protected virtual Doc ToPdf(bool isPreview)
        {
            Doc pdf = null;

            foreach (AbstractMemoContent content in Contents)
            {
                content.Parent = this;
                if (pdf == null)
                {
                    pdf = GeneratePDFForContent(content);
                    continue;
                }

                pdf.Append(GeneratePDFForContent(content));
            }

            return pdf;
        }
    }
}