using ALOD.Core.Domain.Common;
using System.Collections.Generic;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface IEmailTemplateDao : IDao<EmailTemplate, int>
    {
        IList<EmailTemplate> GetSystemTemplates();

        EmailTemplate GetTemplateByTitle(string title);
    }
}