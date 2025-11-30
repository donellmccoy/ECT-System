using ALOD.Core.Domain.Lookup;
using System;
using System.Web.UI.WebControls;

namespace ALOD.Core.Utils
{
    public static class NextActionHelpers
    {
        public static string LogText(string DisplayText, string selectedItem, string text)
        {
            string LogActionDisplayText;

            if (DisplayText.Contains("RFA") || DisplayText.Contains("RWOA") || DisplayText.Contains("Cancel"))
            {
                LogActionDisplayText = String.Format("{0} : {1} : {2}", DisplayText, selectedItem, text);
            }
            else
            {
                LogActionDisplayText = DisplayText;
            }

            return LogActionDisplayText;
        }

        public static void PopulateRadioButtonListWithReturnOptions(RadioButtonList radioButtonList, int workflowId, ILookupDao lookupDao)
        {
            radioButtonList.DataSource = lookupDao.GetWorkflowReturnReasons(workflowId);
            radioButtonList.DataTextField = "Description";
            radioButtonList.DataValueField = "Id";
            radioButtonList.DataBind();

            CollectionHelpers.MoveRadioButtonListItemToEndOfList(radioButtonList, "Other");
        }

        public static void PopulateRadioButtonListWithRwoaOptions(RadioButtonList radioButtonList, int workflowId, ILookupDao lookupDao)
        {
            radioButtonList.DataSource = lookupDao.GetWorkflowRwoaReasons(workflowId);
            radioButtonList.DataTextField = "Description";
            radioButtonList.DataValueField = "Id";
            radioButtonList.DataBind();

            CollectionHelpers.MoveRadioButtonListItemToEndOfList(radioButtonList, "Other");
        }
    }
}