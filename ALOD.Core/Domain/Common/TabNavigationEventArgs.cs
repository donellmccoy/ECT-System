using System;

namespace ALOD.Core.Domain.Common
{
    public enum NavigatorButtonType
    {
        PreviousStep,
        NextStep,
        NavigatedAway,
        Save,
        Print,
        Delete
    }

    public class TabNavigationEventArgs : EventArgs
    {
        public TabNavigationEventArgs(NavigatorButtonType type)
        {
            ButtonType = type;
            Cancel = false;
            TargetUrl = "";
        }

        public TabNavigationEventArgs(NavigatorButtonType type, string url)
        {
            ButtonType = type;
            Cancel = false;
            TargetUrl = url;
        }

        public NavigatorButtonType ButtonType { get; private set; }
        public bool Cancel { get; set; }
        public string TargetUrl { get; set; }
    }
}