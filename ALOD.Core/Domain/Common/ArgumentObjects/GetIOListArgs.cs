using System;

namespace ALOD.Core.Domain.Common.ArgumentObjects
{
    public struct GetIOListArgs
    {
        public int UserId;
        public short RptView;
        public int MemberCaseGradeCode;
        public DateTime? MemberCaseDateOfRank;
    }
}