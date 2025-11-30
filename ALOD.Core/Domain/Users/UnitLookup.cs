using ALOD.Core.Interfaces;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Logging;
using System;
using System.Data;

namespace ALOD.Core.Domain.Users
{
    /// <summary>
    /// UnitLookup is a class that contains essential information for conductings basic lookups and use of core unit data. This uses the IExtractedEntity interface and
    /// SqlDataSource pattern which is more effienct than NHibernate. Normal unit retrievals from the database are slow when getting multiple units.
    /// </summary>
    public class UnitLookup : IExtractedEntity
    {
        public UnitLookup()
        {
            SetPropertiesToDefaultValues();
        }

        private UnitLookup(DataRow row)
        { }

        public virtual int Id { get; set; }
        public virtual int LevelDown { get; set; }
        public virtual string Name { get; set; }

        public virtual string NameAndPasCode
        {
            get
            {
                return Name + " (" + PasCode + ")";
            }
        }

        public virtual int ParentId { get; set; }
        public virtual string PasCode { get; set; }

        /// <inheritdoc/>
        public bool ExtractFromDataRow(DataRow row)
        {
            try
            {
                this.Id = DataHelpers.GetIntFromDataRow("CS_ID", row);
                this.Name = DataHelpers.GetStringFromDataRow("LONG_NAME", row);
                this.PasCode = DataHelpers.GetStringFromDataRow("PAS_CODE", row);
                this.ParentId = DataHelpers.GetIntFromDataRow("Parent_CS_ID", row);
                this.LevelDown = DataHelpers.GetIntFromDataRow("LevelDown", row);

                return true;
            }
            catch (Exception e)
            {
                SetPropertiesToDefaultValues();
                LogManager.LogError(e);
                return false;
            }
        }

        /// <inheritdoc/>
        public bool ExtractFromDataRow(DataRow row, IDaoFactory daoFactory)
        {
            return ExtractFromDataRow(row);
        }

        protected virtual void SetPropertiesToDefaultValues()
        {
            this.Id = 0;
            this.Name = string.Empty;
            this.PasCode = string.Empty;
            this.ParentId = 0;
            this.LevelDown = 0;
        }
    }
}