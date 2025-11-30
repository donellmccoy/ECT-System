using ALOD.Core.Interfaces;
using WebSupergoo.ABCpdf8;
using WebSupergoo.ABCpdf8.Objects;

namespace ALODWebUtility.Printing.FormFieldParsers
{
    public class AdobeLiveCycleHierarchicalParser : IPDFFormFieldParser
    {
        public Field ParseField(Doc document, string desiredField)
        {
            string[] fieldNames = document.Form.GetFieldNames();
            Field currentField = null;

            foreach (string fieldName in fieldNames)
            {
                currentField = document.Form[fieldName];

                if (ExtractFieldName(currentField).Equals(desiredField))
                {
                    return currentField;
                }
            }

            return null;
        }

        protected string ExtractFieldName(Field fieldObj)
        {
            // Adobe LiveCycle field name format: form#[#].Page#[#].fieldName[#]
            // Field.PartialName property grabs the fieldName[#] portion for the field name
            return fieldObj.PartialName.Substring(0, fieldObj.PartialName.Length - 3);
        }
    }
}
