using ALOD.Core.Interfaces;
using NHibernate;
using NHibernate.Type;
using System;

namespace ALOD.Data
{
    /// <summary>
    /// Automatically updated the CreatedDate and ModifiedDate fields
    /// for classes implementing the <see cref="IAuditable"/> interface
    /// </summary>
    internal class AuditInterceptor : EmptyInterceptor
    {
        /// <summary>
        /// Called when NHibernate flushes dirty (modified) entities to update the ModifiedDate property.
        /// </summary>
        /// <param name="entity">The entity being updated.</param>
        /// <param name="id">The entity identifier.</param>
        /// <param name="currentState">The current state of the entity properties.</param>
        /// <param name="previousState">The previous state of the entity properties.</param>
        /// <param name="propertyNames">The names of the entity properties.</param>
        /// <param name="types">The types of the entity properties.</param>
        /// <returns>True if any property was modified; otherwise, false.</returns>
        public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
        {
            bool found = false;

            if (entity is IAuditable)
            {
                for (int i = 0; i < propertyNames.Length; i++)
                {
                    if (propertyNames[i].Equals("ModifiedDate"))
                    {
                        currentState[i] = DateTime.Now;
                        found = true;
                    }
                }

                return found;
            }

            return false;
        }

        /// <summary>
        /// Called when NHibernate saves a new entity to set the CreatedDate and ModifiedDate properties.
        /// </summary>
        /// <param name="entity">The entity being saved.</param>
        /// <param name="id">The entity identifier.</param>
        /// <param name="state">The state of the entity properties.</param>
        /// <param name="propertyNames">The names of the entity properties.</param>
        /// <param name="types">The types of the entity properties.</param>
        /// <returns>True if any property was set; otherwise, false.</returns>
        public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            bool found = false;

            if (entity is IAuditable)
            {
                for (int i = 0; i < propertyNames.Length; i++)
                {
                    if (propertyNames[i].Equals("CreatedDate") || propertyNames[i].Equals("ModifiedDate"))
                    {
                        state[i] = DateTime.Now;
                        found = true;
                    }
                }

                return found;
            }

            return false;
        }
    }
}