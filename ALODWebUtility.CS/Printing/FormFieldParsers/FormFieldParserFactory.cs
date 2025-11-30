using ALOD.Core.Interfaces;

namespace ALODWebUtility.Printing.FormFieldParsers
{
    public static class FormFieldParserFactory
    {
        public static IPDFFormFieldParser GetAdobeLiveCycleHierarchicalParser()
        {
            return new AdobeLiveCycleHierarchicalParser();
        }

        public static IPDFFormFieldParser GetNonHierarchicalParser()
        {
            return new NonHierarchicalParser();
        }

        public static IPDFFormFieldParser GetParserByName(string name)
        {
            switch (name)
            {
                case "NonHierarchicalParser":
                    return GetNonHierarchicalParser();

                case "AdobeLiveCycleHierachicalParser":
                    return GetAdobeLiveCycleHierarchicalParser();

                default:
                    return GetNonHierarchicalParser();
            }
        }
    }
}
