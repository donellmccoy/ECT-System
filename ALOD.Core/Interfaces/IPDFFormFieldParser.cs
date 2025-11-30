using WebSupergoo.ABCpdf8;
using WebSupergoo.ABCpdf8.Objects;

namespace ALOD.Core.Interfaces
{
    public interface IPDFFormFieldParser
    {
        Field ParseField(Doc document, string desiredField);
    }
}