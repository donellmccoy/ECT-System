using ALOD.Core.Utils;
using NHibernate.Engine;
using NHibernate.SqlTypes;
using NHibernate.UserTypes;
using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace ALOD.Data.Types
{
    /// <summary>
    /// Represents an SQL XML data type for NHibernate mappings.
    /// Used to specify that a column should be mapped as DbType.Xml.
    /// </summary>
    public class SqlXmlType : SqlType
    {
        /// <summary>
        /// Initializes a new instance of the SqlXmlType class with DbType.Xml.
        /// </summary>
        public SqlXmlType()
            : base(System.Data.DbType.Xml)
        {
        }
    }

    /// <summary>
    /// Abstract base class for NHibernate custom types that serialize entities to/from XML.
    /// Provides XML serialization/deserialization functionality for persisting complex objects as XML columns in the database.
    /// </summary>
    /// <typeparam name="T">The type of entity to serialize/deserialize.</typeparam>
    public abstract class XmlType<T> : IUserType
    {
        /// <summary>
        /// Gets a value indicating whether instances of the mapped type are mutable.
        /// </summary>
        public bool IsMutable => true;

        /// <summary>
        /// Gets the type of object returned by NullSafeGet.
        /// </summary>
        public Type ReturnedType => typeof(T);

        /// <summary>
        /// Gets the SQL types used for this custom type.
        /// Returns an array containing SqlXmlType to map to XML database columns.
        /// </summary>
        public SqlType[] SqlTypes
        {
            get
            {
                return new SqlType[] { new SqlXmlType() };
            }
        }

        /// <summary>
        /// Reconstructs an object from its cached representation.
        /// </summary>
        /// <param name="cached">The cached object to assemble.</param>
        /// <param name="owner">The parent entity that owns this object.</param>
        /// <returns>The assembled object.</returns>
        public object Assemble(object cached, object owner)
        {
            return cached;
        }

        /// <summary>
        /// Creates a deep copy of the object by serializing and deserializing it.
        /// </summary>
        /// <param name="value">The object to copy.</param>
        /// <returns>A deep copy of the object.</returns>
        public object DeepCopy(object value)
        {
            string string_in = Serialize((T)value);
            T entity = Deserialize(string_in);
            return entity;
        }

        /// <summary>
        /// Transforms the object into its cacheable representation.
        /// </summary>
        /// <param name="value">The object to disassemble.</param>
        /// <returns>The cacheable representation.</returns>
        public object Disassemble(object value)
        {
            return value;
        }

        /// <summary>
        /// Compares two instances of the type for equality by comparing their XML representations.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>True if the objects are equal; otherwise, false.</returns>
        public new bool Equals(object x, object y)
        {
            string value_x = Serialize((T)x);
            string value_y = Serialize((T)y);

            return value_x.Equals(value_y);
        }

        /// <summary>
        /// Gets a hash code for the instance.
        /// </summary>
        /// <param name="x">The object to get a hash code for.</param>
        /// <returns>The hash code.</returns>
        public int GetHashCode(object x)
        {
            return x.GetHashCode();
        }

        /// <summary>
        /// Retrieves an object from the database and deserializes it from XML.
        /// Legacy method signature for NHibernate compatibility.
        /// </summary>
        /// <param name="rs">The data reader containing the XML data.</param>
        /// <param name="names">The column names to read from.</param>
        /// <param name="owner">The parent entity.</param>
        /// <returns>The deserialized entity, or null if the database value is null.</returns>
        public object NullSafeGet(IDataReader rs, string[] names, object owner)
        {
            Check.Require(names.Length == 1, "Names array has more than one element");

            string val = rs[names[0]] as string;

            if (val == null)
            {
                return null;
            }

            return Deserialize(val);
        }

        /// <summary>
        /// Retrieves an object from the database and deserializes it from XML.
        /// </summary>
        /// <param name="rs">The data reader containing the XML data.</param>
        /// <param name="names">The column names to read from.</param>
        /// <param name="session">The NHibernate session.</param>
        /// <param name="owner">The parent entity.</param>
        /// <returns>The deserialized entity, or null if the database value is null.</returns>
        public object NullSafeGet(DbDataReader rs, string[] names, ISessionImplementor session, object owner)
        {
            string val = rs[names[0]] as string;

            if (val == null)
            {
                return null;
            }

            return Deserialize(val);
        }

        /// <summary>
        /// Serializes an object to XML and sets it as a parameter value.
        /// Legacy method signature for NHibernate compatibility.
        /// </summary>
        /// <param name="cmd">The database command.</param>
        /// <param name="value">The entity to serialize, or null.</param>
        /// <param name="index">The parameter index.</param>
        public void NullSafeSet(IDbCommand cmd, object value, int index)
        {
            DbParameter parameter = (DbParameter)cmd.Parameters[index];

            if (value == null)
            {
                parameter.Value = DBNull.Value;
                return;
            }

            parameter.Value = Serialize((T)value);
        }

        /// <summary>
        /// Serializes an object to XML and sets it as a parameter value.
        /// </summary>
        /// <param name="cmd">The database command.</param>
        /// <param name="value">The entity to serialize, or null.</param>
        /// <param name="index">The parameter index.</param>
        /// <param name="session">The NHibernate session.</param>
        public void NullSafeSet(DbCommand cmd, object value, int index, ISessionImplementor session)
        {
            DbParameter parameter = (DbParameter)cmd.Parameters[index];

            if (value == null)
            {
                parameter.Value = DBNull.Value;
                return;
            }

            parameter.Value = Serialize((T)value);
        }

        /// <summary>
        /// Replaces the target object with the original during merge operations.
        /// </summary>
        /// <param name="original">The original object.</param>
        /// <param name="target">The target object to replace.</param>
        /// <param name="owner">The parent entity.</param>
        /// <returns>The replacement object.</returns>
        public object Replace(object original, object target, object owner)
        {
            return original;
        }

        /// <summary>
        /// Deserializes an entity from its XML string representation.
        /// </summary>
        /// <param name="value">The XML string to deserialize.</param>
        /// <returns>The deserialized entity.</returns>
        protected T Deserialize(string value)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(T));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(value));
            TextReader reader = new StreamReader(stream);

            T entity = (T)deserializer.Deserialize(reader);
            reader.Close();

            return entity;
        }

        /// <summary>
        /// Serializes an entity to its XML string representation.
        /// </summary>
        /// <param name="entity">The entity to serialize.</param>
        /// <returns>The XML string representation of the entity.</returns>
        protected string Serialize(T entity)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            MemoryStream stream = new MemoryStream();
            serializer.Serialize(stream, entity);
            string value = System.Text.Encoding.UTF8.GetString(stream.GetBuffer(), 0, (int)stream.Length);
            stream.Close();
            return value;
        }
    }
}