using ALOD.Core.Domain.Common;
using System.Collections.Generic;

namespace ALOD.Data.Types
{
    /// <summary>
    /// NHibernate custom type for serializing and deserializing lists of PersonnelData objects to/from XML.
    /// Implements IUserType to allow NHibernate to persist List&lt;PersonnelData&gt; as XML in the database.
    /// </summary>
    internal class PersonnelDataListType : XmlType<List<PersonnelData>>
    {
    }

    /// <summary>
    /// NHibernate custom type for serializing and deserializing PersonnelData objects to/from XML.
    /// Implements IUserType to allow NHibernate to persist PersonnelData as XML in the database.
    /// </summary>
    internal class PersonnelDataType : XmlType<PersonnelData>
    {
    }
}