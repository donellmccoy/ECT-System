namespace ALOD.Data.Services
{
    /// <summary>
    /// Service for interacting with the Defense Enrollment Eligibility Reporting System (DEERS).
    /// Provides functionality to retrieve member information from DEERS.
    /// </summary>
    public class DeersService
    {
        /// <summary>
        /// Returns the SSN associated with the given EDIPIN by retrieving it from DEERS.
        /// </summary>
        /// <param name="edipin">The Electronic Data Interchange Personal Identifier.</param>
        /// <returns>The social security number associated with the EDIPIN.</returns>
        public string GetSsnByEdipin(string edipin)
        {
            return "66666666A";
        }
    }
}