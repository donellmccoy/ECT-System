namespace ALODWebUtility.LookUps
{
    public class LookUpRow
    {
        protected string _text;
        protected string _value;

        public LookUpRow()
        {
            _text = string.Empty;
            _value = string.Empty;
        }

        public LookUpRow(string text, string value)
        {
            _text = text;
            _value = value;
        }

        public string text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
            }
        }

        public string value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }
    }
}
