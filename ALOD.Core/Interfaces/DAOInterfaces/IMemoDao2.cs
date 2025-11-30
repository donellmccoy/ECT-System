using ALOD.Core.Domain.Documents;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface IMemoDao2 : IDao<Memorandum2, int>
    {
        void EvictLetterHeader(MemoLetterhead letterhead);

        void EvictTemplate(MemoTemplate template);

        IList<MemoLetterhead> GetAllLetterheads();

        IList<MemoTemplate> GetAllTemplates();

        IList<MemoTemplate> GetByCompo(string compo);

        IList<Memorandum2> GetByRefnModule(int refId, int moduleId);

        MemoLetterhead GetCurrentLetterHead(string title);

        IList<string> GetDataSources();

        MemoLetterhead GetEffectiveLetterHead(string compo);

        IDictionary<string, string> GetMemoData(int refId, string source);

        int GetMemoTemplateId(string title);

        DataSet GetRolePermissions(int templateId, string compo);

        MemoTemplate GetTemplateById(int id);

        IList<MemoTemplate> GetTemplatesByModule(byte module);

        void SaveLetterhead(MemoLetterhead letterhead);

        void SaveOrUpdateTemplate(MemoTemplate template);

        void UpdateRolePermissions(int templateId, DataSet permissions);
    }
}