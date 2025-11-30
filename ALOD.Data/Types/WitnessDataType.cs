using ALOD.Core.Domain.Common;
using System.Collections.Generic;

namespace ALOD.Data.Types
{
    /// <summary>
    /// NHibernate custom type for serializing and deserializing lists of WitnessData objects to/from XML.
    /// Implements IUserType to allow NHibernate to persist List&lt;WitnessData&gt; as XML in the database.
    /// </summary>
    public class WitnessDataListType : XmlType<List<WitnessData>>
    {
    }

    /// <summary>
    /// NHibernate custom type for serializing and deserializing WitnessData objects to/from XML.
    /// Implements IUserType to allow NHibernate to persist WitnessData as XML in the database.
    /// </summary>
    public class WitnessDataType : XmlType<WitnessData>
    {
    }
}