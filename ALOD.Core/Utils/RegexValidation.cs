using System;
using System.Text.RegularExpressions;

namespace ALOD.Core.Utils
{
    public static class RegexValidation
    {
        public static MatchCollection GetMatchesDocumentDesc(String data)
        {
            Regex regex = new Regex(
                   @"([a-zA-Z0-9 ])*",
                RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled);

            return regex.Matches(data);
        }

        public static Boolean IsValidCaseId(String caseId)
        {
            Regex regex = new Regex(
                      @"\d{8}-\d{3}-?",
                    RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

            return regex.IsMatch(caseId);
        }

        public static Boolean IsValidEmail(String email)
        {
            Regex regex = new Regex(
                  @"([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\." +
                  @")|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

            return regex.IsMatch(email);
        }

        public static Boolean IsValidPhoneNumber(String number)
        {
            Regex regex = new Regex(@"\d{3}-\d{3}-\d{4}",
                RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

            return regex.IsMatch(number);
        }

        public static Boolean IsValidSSN(String ssn)
        {
            Regex regex = new Regex(
                 @"\d{3}-?\d{2}-?\d{3}[a-zA-Z0-9]",
               RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);

            return regex.IsMatch(ssn);
        }
    }
}