namespace ALODWebUtility.Printing
{
    public class PDFString
    {
        private string _alignment = "center";
        private string _color = "#FF0000/90";
        private string _font = "arial";
        private int _fontSize = 12;
        private string _fontWeight = string.Empty;
        private int _linespacing = 0;
        private int _postNewLines = 0;
        private int _preNewLines = 0;
        private int _rotation = 0;
        private string _text = string.Empty;

        public string Alignment
        {
            get { return this._alignment; }
            set { this._alignment = value; }
        }

        public string Color
        {
            get { return this._color; }
            set { this._color = value; }
        }

        public string Font
        {
            get { return this._font; }
            set { this._font = value; }
        }

        public int FontSize
        {
            get { return this._fontSize; }
            set { this._fontSize = value; }
        }

        public string FontWeight
        {
            get { return this._fontWeight; }
            set { this._fontWeight = value; }
        }

        public int Linespacing
        {
            get { return this._linespacing; }
            set { this._linespacing = value; }
        }

        public int PostNewLines
        {
            get { return this._postNewLines; }
            set { this._postNewLines = value; }
        }

        public int PreNewLines
        {
            get { return this._preNewLines; }
            set { this._preNewLines = value; }
        }

        public int Rotation
        {
            get { return this._rotation; }
            set { this._rotation = value; }
        }

        public string Text
        {
            get { return this._text; }
            set { this._text = value; }
        }

        public string GetHTML()
        {
            string htmlString = string.Empty;

            string sLinespacing = string.Empty;
            string sSize = string.Empty;
            string sAlignment = string.Empty;
            string sFontFace = string.Empty;
            string sColor = string.Empty;

            if (Linespacing != 0)
            {
                sLinespacing = "linespacing=" + Linespacing.ToString();
            }

            if (FontSize >= 0)
            {
                sSize = "fontsize=" + FontSize.ToString();
            }

            if (!string.IsNullOrEmpty(Alignment))
            {
                sAlignment = "align=" + Alignment;
            }

            if (!string.IsNullOrEmpty(Font))
            {
                sFontFace = "face='" + Font;
            }

            if (!string.IsNullOrEmpty(FontWeight))
            {
                sFontFace = sFontFace + "-" + FontWeight + "'";
            }
            else
            {
                sFontFace = sFontFace + "'";
            }

            if (!string.IsNullOrEmpty(Color))
            {
                sColor = "color=" + Color;
            }

            htmlString = GetBreakTagString(PreNewLines);
            htmlString = htmlString + "<p " + Linespacing + " " + sSize + " " + sAlignment + "><StyleRun rotate=" + Rotation + "><Font " + sFontFace + " " + sColor + ">" + Text + "</Font></StyleRun></p>";
            htmlString = htmlString + GetBreakTagString(PostNewLines);

            return htmlString;
        }

        private string GetBreakTagString(int numberOfBreaks)
        {
            if (numberOfBreaks <= 0)
            {
                return string.Empty;
            }

            string breakTags = string.Empty;

            for (int i = 1; i <= numberOfBreaks; i++)
            {
                breakTags = breakTags + "<br>";
            }

            return breakTags;
        }
    }
}
