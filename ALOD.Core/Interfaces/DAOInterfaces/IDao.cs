using System.Linq;

namespace ALOD.Core.Interfaces.DAOInterfaces
{
    public interface IDao<T, IdT>
    {
        void CommitChanges();

        void Delete(T entity);

        void Evict(T entity);

        T FindById(IdT id);

        IQueryable<T> GetAll();

        IQueryable<T> GetByExample(T exampleInstance, params string[] propertiesToExclude);

        T GetById(IdT id);

        T GetById(IdT id, bool shouldLock);

        T GetUniqueByExample(T exampleInstance, params string[] propertiesToExclude);

        T Merge(T entity);

        T Save(T entity);

        void SaveAAId(int aaId, int refId);

        T SaveOrUpdate(T entity);
    }
}