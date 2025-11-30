using System;
using ALOD.Core.Utils;
using ALOD.Data;

namespace ALODWebUtility.Workflow
{
    public class WorkOptionRule
    {
        protected int _id;
        protected int _optionId;
        protected string _ruleKey;
        protected RuleType _ruleType;
        protected string _ruleValue;

        public WorkOptionRule()
        {
            _id = 0;
            _optionId = 0;
            _ruleKey = string.Empty;
            _ruleValue = string.Empty;
        }

        public WorkOptionRule(int optionid, string ruleKey, string ruleVal, RuleType ruleType)
        {
            _optionId = optionid;
            _ruleKey = ruleKey;
            _ruleValue = ruleVal;
            _ruleType = ruleType;
        }

        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public int OptionId
        {
            get { return _optionId; }
            set { _optionId = value; }
        }

        public string RuleKey
        {
            get { return _ruleKey; }
            set { _ruleKey = value; }
        }

        public RuleType RuleTypeID
        {
            get { return _ruleType; }
            set { _ruleType = value; }
        }

        public string RuleValue
        {
            get { return _ruleValue; }
            set { _ruleValue = value; }
        }

        public void Save()
        {
            SqlDataStore adapter = new SqlDataStore();
            adapter.ExecuteNonQuery("core_workOptionRules_sp_Save", _optionId, _ruleKey, _ruleValue, _ruleType);
        }
    }
}
