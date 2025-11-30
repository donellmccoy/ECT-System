using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Interfaces.DAOInterfaces;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for <see cref="LODComment"/> entities.
    /// Provides database operations for LOD comments.
    /// </summary>
    public class LODCommentsDao : AbstractNHibernateDao<LODComment, int>, ILODCommentsDao
    { }
}