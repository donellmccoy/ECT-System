namespace ALOD.Core.Domain.Reports
{
    public enum ProcessingTimeThresholdStatus
    {
        Under = 1,  // less than threshold tolerance
        Near = 2,   // within threshold tolerance
        Over = 3    // At or over threshold
    }
}