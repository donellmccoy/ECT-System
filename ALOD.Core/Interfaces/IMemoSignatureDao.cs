using ALOD.Core.Domain.Common;
using System;

namespace ALOD.Core.Interfaces
{
    public interface IMemoSignatureDao
    {
        MemoSignature GetSignature(int refId, int workflow, int ptype);

        void InsertSignature(int refId, int workflow, String sig, String sig_date, int userId, int ptype);
    }
}