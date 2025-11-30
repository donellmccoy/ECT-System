using ALOD.Core.Domain.Common;
using ALOD.Core.Domain.Modules.Lod;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface IICD9CodeDao : IDao<ICD9Code, int>
    {
        bool DoesCodeExist(string code);

        string Get7thCharacterDefinition(int codeId, string seventhCharacter);

        DataSet Get7thCharacters(int codeId);

        IList<NatureOfIncident> GetAssociatedNatureOfIncidentValues(int codeId);

        DataSet GetChildren(int parentId, int version, bool onlyActive);

        string GetCodeVersion(int codeId);

        bool HasChildren(int parentId, bool onlyActive);

        void InsertNatureOfIncidentMapping(int codeId, string noiValue);

        void UpdateCode(int codeId, string code, string description, bool isDisease, bool active);
    }
}