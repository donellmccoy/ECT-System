using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Interfaces.DAOInterfaces;

namespace ALOD.Data
{
    /// <summary>
    /// Data access object for managing <see cref="LODComment"/> entities.
    /// Provides data persistence operations for Air National Guard LOD comments.
    /// </summary>
    public class ANGLODCommentsDao : AbstractNHibernateDao<LODComment, int>, ILODCommentsDao
    { }
}