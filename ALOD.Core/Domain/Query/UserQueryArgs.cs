namespace ALOD.Core.Domain.Query
{
    public class UserQueryArgs
    {
        public bool HasSarc { get; set; }
        public int QueryId { get; set; }
        public byte Scope { get; set; }
        public int UnitId { get; set; }
        public int UserId { get; set; }
        public int ViewType { get; set; }
    }
}