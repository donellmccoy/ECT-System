using ALOD.Core.Domain.Documents;
using ALOD.Core.Interfaces.DAOInterfaces;
using NHibernate;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Data
{
    public class MemoDao : AbstractNHibernateDao<Memorandum, int>, IMemoDao
    {
        /// <summary>
        /// Evicts (removes) a memo letterhead from the NHibernate session cache.
        /// </summary>
        /// <param name="letterhead">The MemoLetterhead object to evict from the session.</param>
        public void EvictLetterHeader(MemoLetterhead letterhead)
        {
            NHibernateSession.Evict(letterhead);
        }

        /// <summary>
        /// Evicts (removes) a memo template from the NHibernate session cache.
        /// </summary>
        /// <param name="template">The MemoTemplate object to evict from the session.</param>
        public void EvictTemplate(MemoTemplate template)
        {
            NHibernateSession.Evict(template);
        }

        /// <summary>
        /// Retrieves all memo letterheads from the system.
        /// </summary>
        /// <returns>A list of all MemoLetterhead objects.</returns>
        public IList<MemoLetterhead> GetAllLetterheads()
        {
            ICriteria criteria = NHibernateSession.CreateCriteria(typeof(MemoLetterhead));
            return criteria.List<MemoLetterhead>();
        }

        /// <summary>
        /// Retrieves all memo templates from the system.
        /// </summary>
        /// <returns>A list of all MemoTemplate objects.</returns>
        public IList<MemoTemplate> GetAllTemplates()
        {
            ICriteria criteria = NHibernateSession.CreateCriteria(typeof(MemoTemplate));
            return criteria.List<MemoTemplate>();
        }

        /// <summary>
        /// Retrieves memo templates for a specific component.
        /// </summary>
        /// <param name="compo">The component identifier to filter templates.</param>
        /// <returns>A list of MemoTemplate objects matching the component.</returns>
        public IList<MemoTemplate> GetByCompo(string compo)
        {
            return NHibernateSession.CreateCriteria(typeof(MemoTemplate))
                .Add(Expression.Like("Component", compo))
                .List<MemoTemplate>();
        }

        /// <summary>
        /// Retrieves memos by reference ID.
        /// </summary>
        /// <param name="refId">The reference ID (case ID).</param>
        /// <returns>A list of Memorandum objects matching the reference ID.</returns>
        public IList<Memorandum> GetByRefId(int refId)
        {
            return NHibernateSession.CreateCriteria(typeof(Memorandum))
                .Add(Expression.Eq("ReferenceId", refId))
                .List<Memorandum>();
        }

        /// <summary>
        /// Retrieves the current (most recent version) letterhead by title.
        /// </summary>
        /// <param name="title">The letterhead title to search for.</param>
        /// <returns>The MemoLetterhead with the highest version number matching the title, or null if not found.</returns>
        public MemoLetterhead GetCurrentLetterHead(string title)
        {
            MemoLetterhead letter = (MemoLetterhead)NHibernateSession.CreateCriteria(typeof(MemoLetterhead))
                .Add(Expression.Like("Title", title))
                .AddOrder(Order.Desc("Version"))
                .UniqueResult();

            return letter;
        }

        /// <summary>
        /// Retrieves all available memo data sources.
        /// Executes stored procedure: core_memo_sp_GetDataSources
        /// </summary>
        /// <returns>A list of data source names.</returns>
        public IList<string> GetDataSources()
        {
            IList<string> sources = new List<string>();
            SqlDataStore store = new SqlDataStore();

            SqlDataStore.RowReader del = (SqlDataStore source, IDataReader reader) =>
            {
                sources.Add(source.GetString(reader, 0));
            };

            store.ExecuteReader(del, "core_memo_sp_GetDataSources");
            return sources;
        }

        /// <summary>
        /// Retrieves the effective letterhead for a component based on current date.
        /// </summary>
        /// <param name="compo">The component identifier.</param>
        /// <returns>The most recent MemoLetterhead with an effective date on or before the current UTC date, or null if not found.</returns>
        public MemoLetterhead GetEffectiveLetterHead(string compo)
        {
            IList<MemoLetterhead> letterheads = NHibernateSession.CreateCriteria(typeof(MemoLetterhead))
                .Add(Expression.Le("EffectiveDate", DateTime.UtcNow))
                .AddOrder(Order.Desc("EffectiveDate"))
                .Add(Expression.Eq("Component", compo))
                .List<MemoLetterhead>();

            if (letterheads.Count > 0)
                return letterheads[0];
            else
                return null;
        }

        /// <summary>
        /// Retrieves memo merge field data for a specific reference ID.
        /// Executes the specified data source stored procedure.
        /// </summary>
        /// <param name="refId">The reference ID (case ID).</param>
        /// <param name="dataSource">The name of the stored procedure that provides merge field data.</param>
        /// <returns>A dictionary of field names and values for memo template merging.</returns>
        public IDictionary<string, string> GetMemoData(int refId, string dataSource)
        {
            IDictionary<string, string> values = new Dictionary<string, string>();
            SqlDataStore store = new SqlDataStore();

            SqlDataStore.RowReader del = (SqlDataStore source, IDataReader reader) =>
            {
                values.Add(source.GetString(reader, 0), source.GetString(reader, 1));
            };

            store.ExecuteReader(del, dataSource, refId);
            return values;
        }

        /// <summary>
        /// Retrieves role-based permissions for a memo template.
        /// Executes stored procedure: core_memo_sp_GetUserGroupsByTemplateId
        /// </summary>
        /// <param name="templateId">The memo template ID.</param>
        /// <param name="compo">The component identifier.</param>
        /// <returns>A DataSet containing user group permissions (canView, canEdit, canCreate, canDelete).</returns>
        public DataSet GetRolePermissions(int templateId, string compo)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("core_memo_sp_GetUserGroupsByTemplateId", templateId, compo);
        }

        /// <summary>
        /// Retrieves a memo template by its ID.
        /// </summary>
        /// <param name="id">The memo template ID.</param>
        /// <returns>The MemoTemplate object.</returns>
        public MemoTemplate GetTemplateById(int id)
        {
            return (MemoTemplate)NHibernateSession.Load(typeof(MemoTemplate), id);
        }

        /// <summary>
        /// Saves a new memo letterhead to the database.
        /// </summary>
        /// <param name="letterhead">The MemoLetterhead object to save.</param>
        public void SaveLetterhead(MemoLetterhead letterhead)
        {
            NHibernateSessionManager.Instance.GetSession().Save(letterhead);
        }

        /// <summary>
        /// Saves or updates a memo template.
        /// </summary>
        /// <param name="template">The MemoTemplate object to save or update.</param>
        public void SaveOrUpdateTemplate(MemoTemplate template)
        {
            NHibernateSessionManager.Instance.GetSession().SaveOrUpdate(template);
        }

        /// <summary>
        /// Updates role-based permissions for a memo template.
        /// Converts the DataSet to XML and executes stored procedure: core_memo_sp_UpdateGroups
        /// </summary>
        /// <param name="templateId">The memo template ID.</param>
        /// <param name="groups">A DataSet containing user group permissions with columns: groupId, canView, canEdit, canCreate, canDelete.</param>
        public void UpdateRolePermissions(int templateId, DataSet groups)
        {
            if (groups.Tables.Count == 0)
            {
                return;
            }

            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(stream, System.Text.Encoding.ASCII);

            writer.Formatting = System.Xml.Formatting.Indented;
            writer.WriteStartElement("RoleList");

            foreach (System.Data.DataRow row in groups.Tables[0].Rows)
            {
                writer.WriteStartElement("Role");
                writer.WriteAttributeString("groupId", row["groupId"].ToString());
                writer.WriteAttributeString("templateId", templateId.ToString());
                writer.WriteAttributeString("canView", (bool.Parse(row["canView"].ToString())) ? "1" : "0");
                writer.WriteAttributeString("canEdit", (bool.Parse(row["canEdit"].ToString())) ? "1" : "0");
                writer.WriteAttributeString("canCreate", (bool.Parse(row["canCreate"].ToString())) ? "1" : "0");
                writer.WriteAttributeString("canDelete", (bool.Parse(row["canDelete"].ToString())) ? "1" : "0");
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.Flush();

            byte[] buffer = new byte[stream.Length];
            stream.Seek(0, System.IO.SeekOrigin.Begin);

            System.IO.StreamReader reader = new System.IO.StreamReader(stream);
            string output = reader.ReadToEnd();
            writer.Close();

            SqlDataStore store = new SqlDataStore();
            store.ExecuteNonQuery("core_memo_sp_UpdateGroups", templateId, output);
        }
    }
}