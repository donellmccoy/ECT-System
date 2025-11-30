using System.Collections.Generic;
using ALODWebUtility.TabNavigation;

namespace ALODWebUtility.Common
{
    public struct SeniorMedicalReviewerTabVisibilityArgs
    {
        public int ModuleId;
        public int RefId;
        public TabItemList Steps;
        public string TabTitle;
        public List<int> WorkStatusIds;
    }
}
