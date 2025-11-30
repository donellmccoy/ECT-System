using System;

namespace ALODWebUtility.Common
{
    public class PwaiverDate
    {
        /// <summary>
        /// Difference Between the last day of the month[Source Date] and the day picked by the Board Med.
        /// </summary>
        private int differenceBetweenDays;

        /// <summary>
        /// Variable to hold the PWaiver Length.
        /// </summary>
        /// <remarks>It can be mora than 90 days.</remarks>
        private int mPWaiverLength;

        /// <summary>
        /// Source Date, the date that the Board Medical pick in the Baord Med tab.
        /// </summary>
        private DateTime SourceDate;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="OrigDate">Source Date picked by the Board Med.</param>
        public PwaiverDate(DateTime OrigDate)
        {
            SourceDate = OrigDate;
        }

        public int PWaiverLength
        {
            get { return mPWaiverLength; }
            set { mPWaiverLength = value; }
        }

        /// <summary>
        /// Calculate the expiration date. Calculate the PWaiver Length (in Days)
        /// </summary>
        /// <returns>Date.</returns>
        public DateTime GetPWaiverExpirationDate()
        {
            DateTime returnValue;

            differenceBetweenDays = CalculateDays(SourceDate.AddDays(90));
            PWaiverLength = differenceBetweenDays + 90;

            returnValue = SourceDate.AddDays(PWaiverLength);

            return returnValue;
        }

        /// <summary>
        /// Calculate the Difference Between the last day of the month[Source Date] and the day picked by the Board Med.
        /// </summary>
        /// <param name="Static90SourceDate"></param>
        /// <returns></returns>
        private int CalculateDays(DateTime Static90SourceDate)
        {
            int result = 0;

            switch (Static90SourceDate.Month)
            {
                case 1:
                case 3:
                case 5:
                case 7:
                case 8:
                case 10:
                case 12:
                    //January, March, May, July August, October, December  31 days
                    result = (31 - Static90SourceDate.Day);
                    break;

                case 4:
                case 6:
                case 9:
                case 11:
                    //April, June, September, November 30 days
                    result = (30 - Static90SourceDate.Day);
                    break;

                case 2:
                    // February, variable 28 and 29 days
                    if (Static90SourceDate.Year % 4 == 0)
                    {
                        result = (29 - Static90SourceDate.Day);
                    }
                    else
                    {
                        result = (28 - Static90SourceDate.Day);
                    }
                    break;
            }

            return result;
        }
    }
}
