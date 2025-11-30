using ALOD.Core.Interfaces.DAOInterfaces;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ALOD.Data
{
    /// <summary>
    /// Abstract base class for all NHibernate-based data access objects.
    /// Provides common CRUD operations and query functionality for entities.
    /// </summary>
    /// <typeparam name="T">The entity type that this DAO manages.</typeparam>
    /// <typeparam name="IdT">The type of the entity's identifier (e.g., int, long, Guid).</typeparam>
    public abstract class AbstractNHibernateDao<T, IdT> : IDao<T, IdT>
    {
        /// <summary>
        /// The runtime type of the persistent entity managed by this DAO.
        /// </summary>
        private Type persitentType = typeof(T);

        /// <summary>
        /// Exposes the ISession used within the DAO.
        /// </summary>
        /// <remarks>
        /// Sessions are managed by <see cref="NHibernateSessionManager"/> and are scoped per HTTP request in web contexts.
        /// </remarks>
        protected ISession NHibernateSession
        {
            get
            {
                return NHibernateSessionManager.Instance.GetSession();
            }
        }

        /// <summary>
        /// Commits changes regardless of whether there's an open transaction or not.
        /// </summary>
        /// <remarks>
        /// If a transaction is open, commits the transaction. Otherwise, flushes changes directly to the database.
        /// </remarks>
        public void CommitChanges()
        {
            if (NHibernateSessionManager.Instance.HasOpenTransaction())
            {
                NHibernateSessionManager.Instance.CommitTransaction();
            }
            else
            {
                // If there's no transaction, just flush the changes
                NHibernateSessionManager.Instance.GetSession().Flush();
            }
        }

        /// <summary>
        /// Deletes the specified entity from the database using NHibernate.
        /// </summary>
        /// <param name="entity">The entity to delete. Must not be null.</param>
        /// <exception cref="ArgumentNullException">Thrown when entity is null.</exception>
        /// <remarks>
        /// The entity will be removed from the database when the session is flushed or the transaction is committed.
        /// </remarks>
        public void Delete(T entity)
        {
            NHibernateSession.Delete(entity);
        }

        /// <summary>
        /// Evicts the specified entity from the NHibernate first-level cache (session cache).
        /// </summary>
        /// <param name="entity">The entity to evict from the cache. Must not be null.</param>
        /// <exception cref="ArgumentNullException">Thrown when entity is null.</exception>
        /// <remarks>
        /// After eviction, changes to the entity will not be tracked by NHibernate until it is reloaded.
        /// Use this when you need to force a reload from the database or to prevent memory leaks with large result sets.
        /// </remarks>
        public void Evict(T entity)
        {
            NHibernateSession.Evict(entity);
        }

        /// <summary>
        /// Finds an entity by its identifier using the NHibernate Criteria API.
        /// </summary>
        /// <param name="id">The identifier of the entity to find.</param>
        /// <returns>The entity if found; otherwise, the default value for type T (typically null for reference types).</returns>
        /// <remarks>
        /// This method uses the Criteria API with an equality restriction on the Id property.
        /// Unlike GetById, this method does not throw an exception if the entity is not found.
        /// </remarks>
        public T FindById(IdT id)
        {
            IList<T> list = NHibernateSession.CreateCriteria(typeof(T))
                .Add(Expression.Eq("Id", id))
                .SetMaxResults(1)
                .List<T>();

            T entity = default(T);

            if (list.Count > 0)
                entity = (T)list[0];

            return entity;
        }

        /// <summary>
        /// Loads every instance of the requested type with no filtering.
        /// </summary>
        /// <returns>An IQueryable containing all entities of type T.</returns>
        public IQueryable<T> GetAll()
        {
            return GetByCriteria();
        }

        /// <summary>
        /// Loads every instance of the requested type using the supplied <see cref="ICriterion" />.
        /// If no <see cref="ICriterion" /> is supplied, this behaves like <see cref="GetAll" />.
        /// </summary>
        /// <param name="criterion">Optional criteria to filter the results.</param>
        /// <returns>An IQueryable containing entities matching the specified criteria.</returns>
        public IQueryable<T> GetByCriteria(params ICriterion[] criterion)
        {
            ICriteria criteria = NHibernateSession.CreateCriteria(persitentType);

            foreach (ICriterion criterium in criterion)
            {
                criteria.Add(criterium);
            }

            return criteria.List<T>().AsQueryable<T>();
        }

        /// <summary>
        /// Retrieves entities using the Query By Example (QBE) pattern.
        /// </summary>
        /// <param name="exampleInstance">An instance of type T with properties set to the values you want to match. Null properties are ignored.</param>
        /// <param name="propertiesToExclude">Optional array of property names to exclude from the example matching.</param>
        /// <returns>An IQueryable containing all entities that match the non-null properties of the example instance.</returns>
        /// <remarks>
        /// QBE is useful for building dynamic queries without writing HQL or Criteria code.
        /// By default, NHibernate ignores null properties and uses exact matching for non-null values.
        /// </remarks>
        public IQueryable<T> GetByExample(T exampleInstance, params string[] propertiesToExclude)
        {
            ICriteria criteria = NHibernateSession.CreateCriteria(persitentType);
            Example example = Example.Create(exampleInstance);

            foreach (string propertyToExclude in propertiesToExclude)
            {
                example.ExcludeProperty(propertyToExclude);
            }

            criteria.Add(example);

            return criteria.List<T>().AsQueryable<T>();
        }

        /// <summary>
        /// Retrieves an entity by its identifier without applying a database lock.
        /// </summary>
        /// <param name="id">The identifier of the entity to retrieve.</param>
        /// <returns>The entity with the specified identifier.</returns>
        /// <exception cref="ObjectNotFoundException">Thrown when no entity exists with the specified identifier.</exception>
        /// <remarks>
        /// This method uses NHibernate's Load method, which returns a proxy that is lazily loaded.
        /// If the entity doesn't exist, an exception is thrown when the proxy is accessed.
        /// </remarks>
        public T GetById(IdT id)
        {
            return GetById(id, false);
        }

        /// <summary>
        /// Loads an instance of type TypeOfListItem from the DB based on its ID.
        /// </summary>
        /// <param name="id">The identifier of the entity to load.</param>
        /// <param name="shouldLock">If true, acquires an upgrade lock on the entity for pessimistic locking.</param>
        /// <returns>The entity with the specified identifier.</returns>
        /// <remarks>
        /// When shouldLock is true, uses LockMode.Upgrade to prevent other transactions from modifying the entity.
        /// </remarks>
        public T GetById(IdT id, bool shouldLock)
        {
            T entity;

            if (shouldLock)
            {
                entity = (T)NHibernateSession.Load(persitentType, id, LockMode.Upgrade);
            }
            else
            {
                entity = (T)NHibernateSession.Load(persitentType, id);
            }

            return entity;
        }

        /// <summary>
        /// Looks for a single instance using the example provided.
        /// </summary>
        /// <param name="exampleInstance">An instance with properties set to values you want to match.</param>
        /// <param name="propertiesToExclude">Properties to exclude from the comparison.</param>
        /// <returns>The unique entity matching the example, or default(T) if none found.</returns>
        /// <exception cref="NonUniqueResultException">Thrown when more than one entity matches the example.</exception>
        public T GetUniqueByExample(T exampleInstance, params string[] propertiesToExclude)
        {
            List<T> foundList = GetByExample(exampleInstance, propertiesToExclude).ToList<T>();

            if (foundList.Count > 1)
            {
                throw new NonUniqueResultException(foundList.Count);
            }

            if (foundList.Count > 0)
            {
                return foundList[0];
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// Merges the state of a detached entity into the current NHibernate session.
        /// </summary>
        /// <param name="entity">The detached entity to merge. Must not be null.</param>
        /// <returns>The merged, persistent entity instance. Use this returned instance for further operations.</returns>
        /// <exception cref="ArgumentNullException">Thrown when entity is null.</exception>
        /// <remarks>
        /// This method is essential for updating detached entities (e.g., entities loaded in a previous session).
        /// The original entity parameter remains detached; always use the returned instance.
        /// If an entity with the same identifier is already in the session, its state is updated.
        /// </remarks>
        public T Merge(T entity)
        {
            NHibernateSession.Merge(entity);
            return entity;
        }

        /// <summary>
        /// For entities that have assigned ID's, you must explicitly call Save to add a new one.
        /// See http://www.hibernate.org/hib_docs/reference/en/html/mapping.html#mapping-declaration-id-assigned.
        /// </summary>
        /// <param name="entity">The entity to save.</param>
        /// <returns>The saved entity.</returns>
        /// <remarks>
        /// Use this method for entities with manually-assigned identifiers. For entities with auto-generated IDs, use SaveOrUpdate instead.
        /// </remarks>
        public virtual T Save(T entity)
        {
            NHibernateSession.Save(entity);
            return entity;
        }

        /// <inheritdoc/>
        /// <summary>
        /// Saves the approval authority ID associated with a reference ID.
        /// </summary>
        /// <param name="aAId">The approval authority user ID.</param>
        /// <param name="refId">The reference ID to associate with the approval authority.</param>
        public void SaveAAId(int aAId, int refId)
        {
            SqlDataStore store = new SqlDataStore();
            store.ExecuteDataSet("Form348_sp_SaveAppAuthUserId", aAId, refId);
        }

        /// <summary>
        /// For entities with automatatically generated IDs, such as identity, SaveOrUpdate may
        /// be called when saving a new entity.  SaveOrUpdate can also be called to update any
        /// entity, even if its ID is assigned.
        /// </summary>
        /// <param name="entity">The entity to save or update.</param>
        /// <returns>The saved or updated entity.</returns>
        /// <remarks>
        /// NHibernate determines whether to INSERT or UPDATE based on the entity's ID and whether it exists in the session.
        /// </remarks>
        public virtual T SaveOrUpdate(T entity)
        {
            NHibernateSession.SaveOrUpdate(entity);
            return entity;
        }
    }
}