using ALOD.Core.Domain.DBSign;
using ALOD.Core.Domain.Documents;
using ALOD.Core.Domain.Users;
using ALOD.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace ALOD.Core.Utils
{
    public static class MemoHelpers
    {
        private const int lineLength = 35;
        private const int pageWidth = 675;
        private static PaintEventArgs _e = null;

        public static string AdjustSignatureNewLines(string NewResultSig, int newLines, int offset)
        {
            string[] lines = NewResultSig.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            int length = lines.Length - 2;
            int index = lines.Length - 2;
            string holder = lines[index];
            index--;

            while (index > -1)
            {
                if (String.IsNullOrWhiteSpace(lines[index]) && length > 4)
                {
                    length--;
                }
                else
                {
                    holder = lines[index] + Environment.NewLine + holder;
                }

                index--;
            }

            string[] final = holder.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            if (!String.IsNullOrEmpty(final[0]))
            {
                holder = holder.Replace(final[0], final[0].Replace(RepeatString(" ", offset), ""));
            }

            return holder;
        }

        public static bool DoesUserHaveCreatePermissionForTemplate(AppUser userGeneratingMemo, MemoTemplate template)
        {
            if (userGeneratingMemo == null || template == null)
                return false;

            MemoGroup userGroupPermissions = template.GroupPermissions.FirstOrDefault(x => x.Group.Id == userGeneratingMemo.CurrentRole.Group.Id);

            if (userGroupPermissions == null)
                return false;

            return userGroupPermissions.CanCreate;
        }

        public static string GetMemoFormattedSignature(IDigitalSignatureService sigService)
        {
            string sigBody = "UNKNOWN";
            string sigDate = "UNKNOWN";

            if (sigService.VerifySignature() == DBSignResult.SignatureValid)
            {
                DigitalSignatureInfo digitalSignatureInfo = sigService.GetSignerInfo();
                sigBody = digitalSignatureInfo.Signature;
                sigDate = digitalSignatureInfo.DateSigned.ToString();
            }

            return "Digitally signed by " + sigBody + Environment.NewLine + "Date: " + sigDate;
        }

        public static int ParseOutOffsetValue(string s)
        {
            if (string.IsNullOrEmpty(s))
                return 0;

            int startIndex = 0;
            int endIndex = 0;
            int offset = 0;

            startIndex = s.IndexOf("_") + 1;
            endIndex = s.LastIndexOf("}");

            if (startIndex >= endIndex)
                return 0;

            if (!int.TryParse(s.Substring(startIndex, (endIndex - startIndex)), out offset))
                return 0;

            if (offset < 0)
                return 0;

            return offset;
        }

        public static string ProcessOffsetPlaceholders(string content)
        {
            string pattern = @"\{OFFSET_\d+\}";
            int currentOffset = 0;
            string resultString = string.Empty;

            // Find all instances of the {OFFSET_#} placeholder where '#' is any positive number...
            MatchCollection matches = Regex.Matches(content, pattern);

            if (matches == null || matches.Count == 0)
                return content;

            HashSet<string> uniqueMatches = new HashSet<string>();

            if (uniqueMatches == null)
                return content;

            // Generate a collection of unique offset values...
            foreach (Match m in matches)
            {
                uniqueMatches.Add(m.Value);
            }

            if (uniqueMatches.Count == 0)
                return content;

            resultString = content;

            // Replace each offset placeholder with the placeholder's specified number of empty spaces...
            foreach (string s in uniqueMatches)
            {
                currentOffset = ParseOutOffsetValue(s);

                resultString = resultString.Replace(s, RepeatString(" ", currentOffset));
            }

            return resultString;
        }

        public static string ProcessPageBreaks(string content)
        {
            string pageBreakDiv = "<div style=\"page -break-before:always\">&nbsp;</div>";

            return content.Replace("{PAGE_BREAK}", pageBreakDiv);
        }

        public static string ProcessSignatureLine(string content, string signature, int fontSize, bool shouldWrap)
        {
            int endIndex = 0;
            int startIndex = 0;
            string offsetArea = string.Empty;
            int offset = 0;
            string resultString = string.Empty;
            string resultSig = string.Empty;

            // Find the offset area...the offset area is the substring between the last newline which occurs before the {SIGNATURE_BLOCK} placeholder and the start of the {SIGNATURE_BLOCK} placeholder...
            endIndex = content.IndexOf("{SIGNATURE_BLOCK}");

            if (endIndex <= 0)
                return content;

            startIndex = content.Substring(0, endIndex).LastIndexOf(Environment.NewLine) + Environment.NewLine.Length; // Environment.NewLine = \r\n (ie. two characters)

            if ((startIndex + (endIndex - startIndex)) > content.Length - 1)
                return content;

            offset = endIndex - startIndex;
            offsetArea = content.Substring(startIndex, offset);

            //Determine if the offset area only contains empty space...
            if (offsetArea.Trim().Length == 0)
            {
                // Modify the signature information such that each individual line of the signature is prepended with
                // empty spaces, the number of which equals the size of the offset...
                resultSig = signature.Replace(Environment.NewLine, Environment.NewLine + RepeatString(" ", offset));
            }
            else
            {
                // Modify the signature information such that each individual line of the signature is prepended with
                // a comma and an empty space...
                resultSig = signature.Replace(Environment.NewLine, ", ");
            }

            if (shouldWrap)
            {
                resultSig = WrapSignature(resultSig, offset, fontSize);
            }

            // Replace the {SIGNATURE_BLOCK} placeholder with the newly formatted signature string...
            resultString = content.Replace("{SIGNATURE_BLOCK}", resultSig);

            return resultString;
        }

        public static string ProcessSubjectLine(string content, int fontSize)
        {
            string newLine = string.Empty;
            string finalLine = string.Empty;
            int startIndex = content.IndexOf("SUBJECT:");

            if (startIndex < 0)
            {
                return content;
            }
            string line = GetSubjectLine(content, startIndex);
            SizeF stringSize = MeasureString(line, fontSize);

            if (stringSize.Width > pageWidth)
            {
                foreach (string word in line.Split(' '))
                {
                    if (String.IsNullOrEmpty(word))
                    {
                        continue;
                    }

                    if (word.Equals("SUBJECT:"))
                    {
                        newLine = word + " ";
                        continue;
                    }

                    if (MeasureString(newLine + " " + word, fontSize).Width > pageWidth)
                    {
                        finalLine = finalLine + newLine + Environment.NewLine + "{OFFSET_20}";
                        newLine = word;
                    }
                    else
                    {
                        newLine = newLine + " " + word;
                    }
                }

                if (String.IsNullOrEmpty(finalLine))
                {
                    finalLine = newLine;
                }
                else
                {
                    finalLine = finalLine + newLine;
                }

                content = content.Replace(line, finalLine);
            }

            return content;
        }

        public static string ProcessSubjectLine(string content)
        {
            int start = content.IndexOf("SUBJECT:");

            if (start <= 0)
            {
                return content;
            }

            string line = content.Substring(start, content.Length - 1).Split(new string[] { Environment.NewLine }, StringSplitOptions.None)[0];

            return content;
        }

        public static string RepeatString(string Value, int Count)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < Count; i++)
            {
                sb.Append(Value);
            }
            return sb.ToString();
        }

        private static PaintEventArgs getPaint()
        {
            if (_e == null)
            {
                _e = new PaintEventArgs(Graphics.FromImage(new Bitmap(1, 1)), new Rectangle());
            }

            return _e;
        }

        private static string GetSubjectLine(string content, int startIndex)
        {
            return content.Substring(startIndex, content.Length - startIndex).Split(new string[] { Environment.NewLine }, StringSplitOptions.None)[0];
        }

        private static SizeF MeasureString(string line, int fontSize)
        {
            PaintEventArgs e = getPaint();

            Font stringFont = new Font("Times-Rowman", fontSize);

            // Measure string.
            SizeF stringSize = new SizeF();
            stringSize = e.Graphics.MeasureString(line, stringFont);

            return stringSize;
        }

        private static string WrapSignature(string resultSig, int offset, int fontSize)
        {
            int lineCount = 0;
            int indent = 0;
            int newLines = 0;
            string NewResultSig = string.Empty;
            string newLine = string.Empty;
            string finalLine = string.Empty;

            foreach (string line in resultSig.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
            {
                newLine = string.Empty;
                finalLine = string.Empty;

                if (lineCount == 0)
                {
                    //first line doesnt have indentation
                    indent = offset;
                }
                else
                {
                    indent = 0;
                }

                //check if line can fit on the page without a wrap
                if (MeasureString(RepeatString(" ", indent) + line, fontSize).Width > pageWidth)
                {
                    //For any digital signature line except the first
                    if (lineCount > 0)
                    {
                        //newLine = RepeatString(" ", offset);

                        //seperate the line by spaces
                        foreach (string word in line.Split(' '))
                        {
                            if (String.IsNullOrEmpty(word))
                            {
                                continue;
                            }

                            //if it goes over add a new line and start measuring next line
                            if (MeasureString(RepeatString(" ", offset) + newLine + " " + word, fontSize).Width > pageWidth)
                            {
                                finalLine = finalLine + RepeatString(" ", offset) + newLine + Environment.NewLine;
                                newLine = word;
                                newLines++;
                            }
                            else
                            {
                                newLine = newLine + " " + word;
                            }
                        }

                        //put line or lines together
                        if (String.IsNullOrEmpty(finalLine))
                        {
                            finalLine = newLine;
                        }
                        else
                        {
                            finalLine = finalLine + RepeatString(" ", offset) + newLine;
                        }

                        //add the the main signature result
                        NewResultSig = NewResultSig + finalLine + Environment.NewLine;
                    }
                    else
                    {
                        //split by periods
                        if (line.Split('.').Length > 1)
                        {
                            foreach (string word in line.Split('.'))
                            {
                                if (String.IsNullOrEmpty(word))
                                {
                                    continue;
                                }

                                //always first words
                                if (word.Contains("Digitally signed by"))
                                {
                                    newLine = word;
                                    continue;
                                }

                                //if it goes over add a new line and start measuring next line
                                if (MeasureString(RepeatString(" ", indent) + newLine + "." + word, fontSize).Width > pageWidth)
                                {
                                    finalLine = finalLine + RepeatString(" ", offset) + newLine + "." + Environment.NewLine;
                                    newLine = RepeatString(" ", offset) + word;
                                    indent = 0;
                                    newLines++;
                                }
                                else
                                {
                                    newLine = newLine + "." + word;
                                }
                            }
                        }
                        else
                        {
                            //seperate the line by comma
                            foreach (string word in line.Split(','))
                            {
                                if (String.IsNullOrEmpty(word))
                                {
                                    continue;
                                }

                                if (word.Contains("Digitally signed by"))
                                {
                                    newLine = word;
                                    continue;
                                }

                                //if it goes over add a new line and start measuring next line
                                if (MeasureString(RepeatString(" ", indent) + newLine + "," + word, fontSize).Width > pageWidth)
                                {
                                    finalLine = finalLine + RepeatString(" ", offset) + newLine + Environment.NewLine;
                                    newLine = RepeatString(" ", offset) + word;
                                    indent = 0;
                                    newLines++;
                                }
                                else
                                {
                                    newLine = newLine + "," + word;
                                }
                            }
                        }

                        //put line or lines together
                        if (String.IsNullOrEmpty(finalLine))
                        {
                            finalLine = newLine;
                        }
                        else
                        {
                            finalLine = finalLine + newLine;
                        }

                        //add the the main signature result
                        NewResultSig = NewResultSig + finalLine + Environment.NewLine;
                    }
                }
                else
                {
                    NewResultSig = NewResultSig + RepeatString(" ", indent) + line + Environment.NewLine;
                }

                lineCount++;
            }

            NewResultSig = Environment.NewLine + NewResultSig;

            NewResultSig = AdjustSignatureNewLines(NewResultSig, newLines, offset);

            return NewResultSig;
        }
    }
}