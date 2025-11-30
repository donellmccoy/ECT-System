using ALOD.Core.Domain.Workflow;
using System;
using System.Collections.Generic;

namespace ALOD.Core.Domain.Modules.Common
{
    public class UserGroupContainsDecisionRuleData
    {
        public const int INDEX_FIRST_DECISION = 1;
        public const int INDEX_USERGROUP_NAME = 0;
        public const int MINIMUM_DATA_ELEMENTS_NEEDED = 2;

        public UserGroupContainsDecisionRuleData(WorkflowOptionRule optionRule)
        {
            if (optionRule == null)
                throw new ArgumentNullException(nameof(optionRule));

            Decisions = new List<string>();
            IsValid = ParseRuleDataValue(optionRule.RuleValue);
        }

        private UserGroupContainsDecisionRuleData()
        { }

        public IList<string> Decisions { get; set; }
        public bool IsValid { get; set; }
        public string UserGroupName { get; set; }

        private bool ParseRuleDataValue(string ruleValue)
        {
            if (string.IsNullOrEmpty(ruleValue))
                return false;

            string[] ruleData = ruleValue.Split(',');

            if (ruleData.Length < MINIMUM_DATA_ELEMENTS_NEEDED)
                return false;

            UserGroupName = ruleData[INDEX_USERGROUP_NAME];

            for (int i = INDEX_FIRST_DECISION; i < ruleData.Length; i++)
            {
                Decisions.Add(ruleData[i]);
            }

            return true;
        }
    }
}