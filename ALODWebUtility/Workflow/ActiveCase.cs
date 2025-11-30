using System;
using ALOD.Core.Domain.Workflow;

namespace ALODWebUtility.Workflow
{
    public class ActiveCase
    {
        protected string _descr;
        protected string _modDesc;
        protected int _parentId;
        protected int _refId;
        protected string _sep = "|";
        protected string _title;
        protected ModuleType _type;

        public string Description
        {
            get { return _descr; }
            set { _descr = value; }
        }

        public string ModuleTitle
        {
            get { return _modDesc; }
            set { _modDesc = value; }
        }

        public int ParentId
        {
            get { return _parentId; }
            set { _parentId = value; }
        }

        public int RefId
        {
            get { return _refId; }
            set { _refId = value; }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        public ModuleType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public void FromString(string input)
        {
            string[] parts = input.Split(new string[] { _sep }, StringSplitOptions.None);

            if (parts.Length != 3)
            {
                return;
            }

            int.TryParse(parts[0], out _refId);
            int.TryParse(parts[1], out _parentId);
            
            byte typeVal;
            if (byte.TryParse(parts[2], out typeVal))
            {
                _type = (ModuleType)typeVal;
            }
        }

        public override string ToString()
        {
            return _modDesc + " (" + _descr + ")";
        }

        public string ValueString()
        {
            return _refId.ToString() + _sep + _parentId.ToString() + _sep + ((byte)_type).ToString();
        }
    }
}
