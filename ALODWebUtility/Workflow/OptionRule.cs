using System;
using ALOD.Data;

namespace ALODWebUtility.Workflow
{
    public class OptionRule
    {
        #region Members

        protected bool _chkAll = true;
        protected string _ruleData;
        protected byte _ruleId;
        protected int _wsoId;
        protected byte _wsrId = 0;

        #endregion

        #region Property

        public bool CheckAll
        {
            get { return _chkAll; }
            set { _chkAll = value; }
        }

        public string RuleData
        {
            get { return _ruleData; }
            set { _ruleData = value; }
        }

        public byte RuleId
        {
            get { return _ruleId; }
            set { _ruleId = value; }
        }

        public int WSOId
        {
            get { return _wsoId; }
            set { _wsoId = value; }
        }

        public byte WSRId
        {
            get { return _wsrId; }
            set { _wsrId = value; }
        }

        #endregion

        #region Save

        public void Save()
        {
            SqlDataStore adapter = new SqlDataStore();
            adapter.ExecuteScalar("core_rules_sp_InsertOptionRule", WSOId, RuleId, RuleData, CheckAll);
        }

        #endregion
    }
}
