using ALOD.Core.Interfaces;
using ALOD.Core.Interfaces.DAOInterfaces;
using ALOD.Core.Utils;
using ALOD.Logging;
using System;
using System.Data;

namespace ALOD.Core.Domain.Workflow
{
    public class Module : IExtractedEntity
    {
        public Module()
        {
            SetPropertiesToDefaultValues();
        }

        private Module(DataRow row)
        { }

        public virtual int Id { get; set; }

        public ModuleType IdAsModuleType
        {
            get
            {
                return (ModuleType)Id;
            }
        }

        public virtual bool IsSpecialCase { get; set; }
        public virtual string Name { get; set; }

        /// <inheritdoc/>
        public bool ExtractFromDataRow(DataRow row)
        {
            try
            {
                Id = DataHelpers.GetIntFromDataRow("moduleId", row);
                Name = DataHelpers.GetStringFromDataRow("moduleName", row);
                IsSpecialCase = DataHelpers.GetBoolFromDataRow("isSpecialCase", row);

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
            IsSpecialCase = false;
        }
    }
}