namespace ALOD.Core.Domain.Documents
{
    public class MemoContent2 : AbstractMemoContent
    {
        /*
         * This class is currently empty because it used to contain all of the code in AbstractMemoContent.cs.
         * That code also existed in a copy/paste fashion in the MemoContent.cs class. In February of 2017 MemoContent2
         * and MemoContent were refactored and had their code placed in AbstractMemoContent which was added for the
         * refactoring effort. The reason MemoContent and MemoContent2 need to remain is due to the NHibernate mapping
         * to the core_MemoContents and core_MemoContents2 tables.
         */
    }
}