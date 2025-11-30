using System;
using System.Collections.Generic;
using ALOD.Core.Domain.Modules.Lod;
using ALOD.Core.Domain.Workflow;
using ALOD.Core.Utils;
using ALOD.Data.Services;

namespace ALODWebUtility.PrintingUtil
{
    public static class PrintingUtil
    {
        public static bool CheckForPriorStatus(short status, int refId)
        {
            LineOfDuty lod = LodService.GetById(refId);

            if (status == LodStatusCode.Complete)
            {
                IList<WorkStatusTracking> ws = WorkFlowService.GetWorkStatusTracking(lod.Id, ModuleType.LOD);

                if (ws != null)
                {
                    if (ws.Count > 1)
                    {
                        if (ws[1].WorkflowStatus.StatusCodeType.Id == LodStatusCode.AppointingAutorityReview)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }

            return false;
        }

        public static void ClearFormField(Printing.PDFForm doc, string field)
        {
            SetFormField(doc, field, string.Empty);
        }

        public static string GetFindingFormText(Finding findingValue)
        {
            switch (findingValue)
            {
                case Finding.In_Line_Of_Duty:
                    return "Line of Duty";
                case Finding.Epts_Lod_Not_Applicable:
                    return "EPTS-LOD Not Applicable";
                case Finding.Nlod_Due_To_Own_Misconduct:
                    return "Not ILOD-Due to Own Misconduct";
                case Finding.Epts_Service_Aggravated:
                    return "EPTS-Service Aggravated";
                case Finding.Nlod_Not_Due_To_OwnMisconduct:
                    return "Not ILOD-Not Due to Own Misconduct";
                case Finding.Recommend_Formal_Investigation:
                    return "Formal Investigation";
                default:
                    return string.Empty;
            }
        }

        public static bool IsValidSignature(SignatureEntry signature)
        {
            if (signature == null)
            {
                return false;
            }

            return signature.IsSigned;
        }

        public static string RemoveNewLinesFromString(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return string.Empty;
            }

            string replacedString = data;
            bool replaceFinished = false;
            int len;

            while (!replaceFinished)
            {
                len = replacedString.Length;
                replacedString = replacedString.Replace(Environment.NewLine, " ");

                if (len == replacedString.Length)
                {
                    replaceFinished = true;
                }
            }

            return replacedString;
        }

        public static void SetCheckboxField(Printing.PDFForm doc, string field, bool? value)
        {
            if (!value.HasValue || value.Value == false)
            {
                return;
            }

            SetFormField(doc, field, "1");
        }

        public static void SetDateTimeField(Printing.PDFForm doc, string field, DateTime? value, string format = "ddMMMyyyy", bool toUpper = false)
        {
            if (!value.HasValue)
            {
                return;
            }

            if (toUpper)
            {
                SetFormField(doc, field, value.Value.ToString(format).ToUpper());
            }
            else
            {
                SetFormField(doc, field, value.Value.ToString(format));
            }
        }

        public static void SetFormField(Printing.PDFForm doc, string field, string value)
        {
            if (!string.IsNullOrEmpty(field))
            {
                try
                {
                    doc.SetField(field, value);
                }
                catch (Exception ex)
                {
                    // Silently catch exceptions
                }
            }
        }

        public static string[] SplitString(string chopString, int length)
        {
            string top, bottom;
            string[] temp = new string[2];
            int dif = 0;

            if (string.IsNullOrEmpty(chopString) || chopString.Length == 0)
            {
                temp[0] = "";
                temp[1] = "";
            }
            else
            {
                if (chopString.Length > length)
                {
                    top = chopString.Substring(0, length);
                    temp = top.Split(' ');
                    dif = temp[temp.Length - 1].Length;

                    top = chopString.Substring(0, length - dif);
                    bottom = chopString.Substring(top.Length, chopString.Length - top.Length);

                    temp[0] = top;
                    temp[1] = bottom;
                }
                else
                {
                    temp[0] = chopString;
                    temp[1] = string.Empty;
                }
            }

            return temp;
        }
    }
}
