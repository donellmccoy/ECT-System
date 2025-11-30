using ALOD.Core.Domain.Users;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Interfaces.DAOInterfaces;
using System;
using System.Collections.Generic;

namespace ALOD.Data
{
    public class UserGroupDao : AbstractNHibernateDao<UserGroup, int>, IUserGroupDao
    {
        /// <inheritdoc/>
        public List<UserGroup> GetAll(int compo)
        {
            SqlDataStore data = new SqlDataStore();

            List<UserGroup> groups = new List<UserGroup>();

            System.Data.DataSet set = data.ExecuteDataSet("core_group_sp_GetAll", compo);

            foreach (System.Data.DataRow row in set.Tables[0].Rows)
            {
                UserGroup group = new UserGroup(Convert.ToInt32(row["groupId"]));

                group.Description = row["name"].ToString();
                group.Abbreviation = row["abbr"].ToString();
                group.ShowInfo = (bool)row["showInfo"];
                group.SortOrder = Convert.ToInt32(row["sortOrder"]);
                group.HipaaRequired = (bool)row["hipaaRequired"];
                group.Scope = (AccessScope)Convert.ToInt32(row["accessScope"]);
                group.ReportView = (ReportingView)Convert.ToInt32(row["reportView"]);

                groups.Add(group);
            }

            return groups;
        }

        /// <inheritdoc/>
        public System.Data.DataSet GetAllWithManaged(int groupId)
        {
            SqlDataStore data = new SqlDataStore();
            return data.ExecuteDataSet("core_group_sp_GetAllWithManaged", groupId);
        }

        /// <inheritdoc/>
        public System.Data.DataSet GetAllWithViewBy(int groupId)
        {
            SqlDataStore store = new SqlDataStore();
            return store.ExecuteDataSet("core_group_sp_GetViewBy", groupId);
        }

        /// <inheritdoc/>
        public System.Data.DataSet GetManagedGroups(int groupId)
        {
            SqlDataStore data = new SqlDataStore();
            return data.ExecuteDataSet("core_group_sp_GetManaged", groupId);
        }

        /// <inheritdoc/>
        public string GetNameById(int groupId, int compo)
        {
            List<UserGroup> groups = GetAll(compo);

            foreach (UserGroup group in groups)
            {
                if (group.Id == groupId)
                {
                    return group.Description;
                }
            }

            return "";
        }

        /// <inheritdoc/>
        public void UpdateManagedBy(int groupId, System.Data.DataSet groups)
        {
            if (groups.Tables.Count == 0)
            {
                return;
            }

            if (groups.Tables[0].Rows.Count == 0)
            {
                return;
            }

            System.IO.MemoryStream stream = new System.IO.MemoryStream();
            System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(stream, System.Text.Encoding.ASCII);

            writer.Formatting = System.Xml.Formatting.Indented;
            writer.WriteStartElement("GroupList");
            SqlDataStore store = new SqlDataStore();

            store.ExecuteNonQuery("dbo.core_group_sp_ClearViewByGroups", groupId);

            //String message = "Test";

            foreach (System.Data.DataRow row in groups.Tables[0].Rows)
            {
                writer.WriteStartElement("Group");
                writer.WriteAttributeString("groupId", row["GroupId"].ToString());
                writer.WriteAttributeString("managed", row["Manages"].ToString());
                writer.WriteAttributeString("notify", row["Notify"].ToString());
                if (row["viewBy"].ToString().Equals("1"))
                {
                    if (int.Parse(row[1].ToString()) != 1)
                    {
                        UpdateViewBy(groupId, int.Parse(row["GroupId"].ToString()));
                    }
                }

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
            writer.Flush();

            byte[] buffer = new byte[stream.Length];
            stream.Seek(0, System.IO.SeekOrigin.Begin);

            System.IO.StreamReader reader = new System.IO.StreamReader(stream);
            string output = reader.ReadToEnd();
            writer.Close();

            store.ExecuteNonQuery("core_group_sp_UpdateManagedGroups", groupId, output);
        }

        public void UpdateViewBy(int groupId, int viewById)
        {
            //updates the viewBy results
            SqlDataStore store = new SqlDataStore();
            store.ExecuteNonQuery("dbo.core_group_sp_UpdateViewByGroups", groupId, viewById);
        }
    }
}