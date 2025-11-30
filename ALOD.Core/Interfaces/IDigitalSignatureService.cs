using ALOD.Core.Domain.DBSign;

namespace ALOD.Core.Interfaces
{
    // TODO: Decouple IDigitalSignatureService from DBSign
    public interface IDigitalSignatureService
    {
        int PrimaryId { get; set; }
        DBSignResult Result { get; }
        int SecondaryId { get; set; }
        DBSignTemplate Template { get; }
        DBSignTemplateId TemplateId { get; }
        string TemplateTableName { get; }
        string Text { get; }

        DigitalSignatureInfo GetSignerInfo();

        string GetUrl(DBSignAction action);

        DBSignResult VerifySignature();
    }
}