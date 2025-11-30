namespace ALOD.Core.Domain.Documents
{
    public class PDFTableHTMLData
    {
        private double _horizontalPosition;

        public PDFTableHTMLData()
        {
            this.Text = string.Empty;
            this.Color = string.Empty;
            this.IsBold = false;
            this.HorizontalPosition = 0.0;
        }

        public PDFTableHTMLData(string htmlText)
        {
            this.Text = htmlText;
            this.Color = string.Empty;
            this.IsBold = false;
            this.HorizontalPosition = 0.0;
        }

        public PDFTableHTMLData(string htmlText, string color)
        {
            this.Text = htmlText;
            this.Color = color;
            this.IsBold = false;
            this.HorizontalPosition = 0.0;
        }

        public PDFTableHTMLData(string htmlText, string color, bool isBold)
        {
            this.Text = htmlText;
            this.Color = color;
            this.IsBold = isBold;
            this.HorizontalPosition = 0.0;
        }

        public PDFTableHTMLData(string htmlText, string color, bool isBold, double hPos)
        {
            this.Text = htmlText;
            this.Color = color;
            this.IsBold = isBold;
            this.HorizontalPosition = hPos;
        }

        public string Color { get; set; }

        public double HorizontalPosition
        {
            get { return _horizontalPosition; }
            set
            {
                if (value < 0)
                    _horizontalPosition = 0.0;
                else if (value > 1)
                    _horizontalPosition = 1.0;
                else
                    _horizontalPosition = value;
            }
        }

        public bool IsBold { get; set; }
        public string Text { get; set; }
    }
}