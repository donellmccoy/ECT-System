using System;
using System.Collections.Generic;
using System.Linq;

namespace ALOD.Core.Utils
{
    public static class Helpers
    {
        public static bool IsEven(int x)
        {
            return (x % 2 == 0);
        }

        public static bool IsOdd(int x)
        {
            return !IsEven(x);
        }

        public static double NormalRound(double value)
        {
            return Math.Round(value, MidpointRounding.AwayFromZero);
        }

        public static string WordWrapByCharacterLength(string text, int maxLineLength)
        {
            List<string> parts = text.Split(' ').ToList<string>();
            List<string> newlineParts;
            string result = string.Empty;
            string lastElement = string.Empty;
            int currentLength = 0;

            // Iterate through each string separated by a space...
            foreach (string s in parts)
            {
                newlineParts = s.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList<string>();

                if (newlineParts.Count > 1)
                {
                    lastElement = newlineParts.ElementAt(newlineParts.Count - 1);

                    // Iterate through each string separated by a newline...
                    foreach (string p in newlineParts)
                    {
                        if (currentLength + p.Length < maxLineLength)
                        {
                            result += p;
                            currentLength += p.Length;
                        }
                        else
                        {
                            result += (Environment.NewLine + p);
                            currentLength = p.Length;
                        }

                        if (!p.Equals(lastElement))
                        {
                            result += Environment.NewLine;
                            currentLength = 0;
                        }
                        else
                        {
                            result += " ";
                            currentLength += 1;
                        }
                    }
                }
                else
                {
                    if (currentLength + s.Length < maxLineLength)
                    {
                        result += (s + " ");
                        currentLength += (s.Length + 1);
                    }
                    else
                    {
                        result += (Environment.NewLine + s + " ");
                        currentLength = s.Length + 1;
                    }
                }
            }

            return result.Trim();
        }
    }
}