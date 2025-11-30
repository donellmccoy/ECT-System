using System.Linq;
using ALOD.Core.Interfaces;
using WebSupergoo.ABCpdf8;
using WebSupergoo.ABCpdf8.Objects;

namespace ALODWebUtility.Printing.FormFieldParsers
{
    public class NonHierarchicalParser : IPDFFormFieldParser
    {
        public Field ParseField(Doc document, string desiredField)
        {
            string[] fieldNames = document.Form.GetFieldNames();
            string fullFieldName = fieldNames.First(x => x.Equals(desiredField.Trim()));
            return document.Form[fullFieldName];
        }
    }
}
