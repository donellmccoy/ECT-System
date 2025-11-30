using ALOD.Core.Domain.Users;
using System;
using System.Collections.Generic;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface IUnitDao : IDao<Unit, int>
    {
        IList<UnitLookup> GetAllSubUnitsForUnit(int unitId, int viewType);

        Unit GetByNameAndPASCode(string name, string pasCode);

        IList<UnitLookup> GetImmediateChildrenForUnit(int unitId, int viewType);

        Dictionary<string, Int32> GetUnitReportingStructure(int unitId);
    }
}