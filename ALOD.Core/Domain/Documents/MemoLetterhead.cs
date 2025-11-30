using ALOD.Core.Utils;
using System;
using WebSupergoo.ABCpdf8;

namespace ALOD.Core.Domain.Documents
{
    public class MemoLetterhead : Entity
    {
        public static string SEVENTY_YEAR_MEMO = "70 Year Memo";
        public static string STANDARD_MEMO = "Standard Memo";
        public virtual string Component { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual DateTime EffectiveDate { get; set; }
        public virtual string Font { get; set; }
        public virtual int FontSize { get; set; }
        public virtual string FooterImageCenter { get; set; }

        public virtual XColor HeaderFontColor
        {
            get
            {
                return ABCPDFUtility.ConvertStringToXColor(HeaderFontColorName);
            }
        }

        public virtual string HeaderFontColorName { get; set; }
        public virtual string HeaderSubtitle { get; set; }
        public virtual int HeaderSubtitleSize { get; set; }
        public virtual string HeaderTitle { get; set; }
        public virtual int HeaderTitleSize { get; set; }
        public virtual string LogoImageLeft { get; set; }
        public virtual string LogoImageRight { get; set; }
        public virtual string Title { get; set; }
        public virtual int Version { get; set; }

        public virtual XImage GetCenterFooterImage(string documentRootPath)
        {
            return ABCPDFUtility.GetFileAsXImage(documentRootPath, FooterImageCenter);
        }

        public virtual XImage GetLeftLogoImage(string documentRootPath)
        {
            return ABCPDFUtility.GetFileAsXImage(documentRootPath, LogoImageLeft);
        }

        public virtual XImage GetRightLogoImage(string documentRootPath)
        {
            return ABCPDFUtility.GetFileAsXImage(documentRootPath, LogoImageRight);
        }
    }
}