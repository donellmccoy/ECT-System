using ALOD.Core.Interfaces;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Logging;
using System;
using System.Collections.Generic;
using System.Data;

namespace ALOD.Core.Domain.PsychologicalHealth
{
    public class PHSection : IExtractedEntity
    {
        public PHSection()
        {
            SetPropertiesToDefaultValues();
        }

        private PHSection(DataRow row)
        { }

        public virtual IList<PHSection> Children { get; set; }
        public virtual int DisplayOrder { get; set; }
        public virtual int FieldColumns { get; set; }
        public virtual bool HasPageBreak { get; set; }
        public virtual int Id { get; set; }
        public virtual bool IsTopLevel { get; set; }
        public virtual string Name { get; set; }
        public virtual int ParentId { get; set; }

        /// <inheritdoc/>
        public bool ExtractFromDataRow(DataRow row)
        {
            try
            {
                Id = DataHelpers.GetIntFromDataRow("Id", row);
                Name = DataHelpers.GetStringFromDataRow("Name", row);
                ParentId = DataHelpers.GetIntFromDataRow("ParentId", row);
                FieldColumns = DataHelpers.GetIntFromDataRow("FieldColumns", row);
                DisplayOrder = DataHelpers.GetIntFromDataRow("DisplayOrder", row);
                IsTopLevel = DataHelpers.GetBoolFromDataRow("IsTopLevel", row);
                HasPageBreak = DataHelpers.GetBoolFromDataRow("PageBreak", row);
                Children = new List<PHSection>();

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
            try
            {
                return ExtractFromDataRow(row);
            }
            catch (Exception e)
            {
                SetPropertiesToDefaultValues();
                LogManager.LogError(e);
                return false;
            }
        }

        protected void SetPropertiesToDefaultValues()
        {
            Id = 0;
            Name = string.Empty;
            ParentId = 0;
            IsTopLevel = false;
            HasPageBreak = false;
            Children = new List<PHSection>();
        }
    }
}