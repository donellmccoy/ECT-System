using ALOD.Core.Interfaces;

namespace ALODWebUtility.Documents
{
    public class DocumentProcessingFactory
    {
        public IDocumentProcessingStrategy GetCertificationStampStrategy()
        {
            return new CertificationStampStrategy();
        }

        public IDocumentProcessingStrategy GetDefaultStrategy()
        {
            return new DefaultProcessingStrategy();
        }

        public IDocumentProcessingStrategy GetStrategyByType(ProcessingStrategyType type)
        {
            switch (type)
            {
                case ProcessingStrategyType._Default:
                    return GetDefaultStrategy();

                case ProcessingStrategyType.CertificationStamp:
                    return GetCertificationStampStrategy();

                default:
                    return GetDefaultStrategy();
            }
        }
    }
}
