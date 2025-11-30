namespace ALOD.Core.Interfaces
{
    /// <summary>
    /// Provides an interface for retrieving tracking data
    /// </summary>
    public interface ITrackingSource
    {
        byte ModuleType { get; }
        int Status { get; }
        int TrackingId { get; }
    }
}