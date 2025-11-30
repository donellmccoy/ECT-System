namespace ALOD.Core.Domain.Documents
{
    public class Memorandum : AbstractMemorandum
    {
        /*
         * This class is currently empty because it used to contain all of the code in AbstractMemorandum.cs.
         * That code also existed in a copy/paste fashion in the Memorandum2.cs class. In February of 2017 Memorandum
         * and Memorandum2 were refactord and had their code placed in AbstractMemorandum which was added for the
         * refactoring effort. The reason Memorandum and Memorandum2 need to remain is due to the NHibernate mapping
         * to the core_Memos and core_Memos2 tables.
         */
    }
}