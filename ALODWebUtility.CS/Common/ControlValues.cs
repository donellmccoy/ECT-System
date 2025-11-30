using System;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Text;

namespace ALODWebUtility.Common
{
    public class ControlValues
    {
        #region ControlValues GetMethods

        public static string GetAttribute(object ctl, string TypeString, string AttrName)
        {
            string ret = "";
            switch (TypeString)
            {
                case "System.Web.UI.WebControls.ListBox":
                    ret = ((ListBox)ctl).Attributes[AttrName];
                    break;
                case "System.Web.UI.WebControls.CheckBoxList":
                    ret = ((CheckBoxList)ctl).Attributes[AttrName];
                    break;
                case "System.Web.UI.WebControls.DropDownList":
                    ret = ((DropDownList)ctl).Attributes[AttrName];
                    break;
                case "System.Web.UI.WebControls.CheckBox":
                    ret = ((CheckBox)ctl).Attributes[AttrName];
                    break;
                case "System.Web.UI.WebControls.TextBox":
                    ret = ((TextBox)ctl).Attributes[AttrName];
                    break;
                case "System.Web.UI.WebControls.GridView":
                    ret = ((GridView)ctl).Attributes[AttrName];
                    break;
                case "System.Web.UI.WebControls.RadioButtonList":
                    ret = ((RadioButtonList)ctl).Attributes[AttrName];
                    break;
                case "System.Web.UI.WebControls.RadioButton":
                    ret = ((RadioButton)ctl).Attributes[AttrName];
                    break;
                case "System.Web.UI.HtmlControls.HtmlInputHidden":
                    ret = ((HtmlInputHidden)ctl).Attributes[AttrName];
                    break;
            }
            return ret;
        }

        public static string GetControls(System.Web.UI.ControlCollection ctls)
        {
            StringBuilder sb = new StringBuilder(1024);
            ModControl dc;
            string TypeString;

            foreach (System.Web.UI.Control ctl in ctls)
            {
                if (ctl.HasControls())
                {
                    // Recurse into any "Grouping" controls
                    sb.Append(GetControls(ctl.Controls));
                }

                // Get Control Definition

                TypeString = ctl.GetType().ToString();
                dc = ControlLoad.mControlList.GetByType(TypeString);
                if (dc != null)
                {
                    if (dc.IsMultiList && dc.PropertyToCheck == string.Empty)
                    {
                        if (TypeString == "System.Web.UI.WebControls.ListBox")
                        {
                            sb.Append(GetMultiListValues((ListBox)ctl));
                        }
                        else if (TypeString == "System.Web.UI.WebControls.CheckBoxList")
                        {
                            sb.Append(GetMultiListValues((CheckBoxList)ctl));
                        }
                        else if (TypeString == "System.Web.UI.WebControls.RadioButtonList")
                        {
                            sb.Append(GetMultiListValues((RadioButtonList)ctl));
                        }
                    }
                    else if (dc.IsMultiList && dc.PropertyToCheck != string.Empty)
                    {
                        if (GetControlValue(ctl, dc.PropertyToCheck) == dc.PropertyToCheckValue)
                        {
                            sb.Append(GetControlValue(ctl, dc.Property1));
                        }
                        else
                        {
                            sb.Append(GetMultiListValues((ListBox)ctl));
                        }
                    }
                    else if (!dc.IsMultiList && dc.PropertyToCheck != string.Empty)
                    {
                        if (GetControlValue(ctl, dc.PropertyToCheck) == dc.PropertyToCheckValue)
                        {
                            sb.Append(GetControlValue(ctl, dc.Property1));
                        }
                        else
                        {
                            sb.Append(GetControlValue(ctl, dc.Property2));
                        }
                    }
                    else if (!dc.IsMultiList && dc.Property2 != string.Empty)
                    {
                        sb.Append(GetControlValue(ctl, dc.Property1));
                        sb.Append(GetControlValue(ctl, dc.Property2));
                    }
                    else
                    {
                        sb.Append(GetControlValue(ctl, dc.Property1));
                    }
                }
            }

            return sb.ToString();
        }

        public static string GetControlValue(Control ctl, string PropertyName)
        {
            Type t;
            string value = "";
            t = ctl.GetType();
            string TypeString = t.ToString();
            if (TypeString == "System.Web.UI.WebControls.DropDownList")
            {
                //Customized to get the Selected Item Text instead of Selected Index
                DropDownList ddl;
                ddl = (DropDownList)ctl;

                if (ddl.SelectedIndex >= 0)
                {
                    value = ddl.SelectedItem.Text;
                }
            }
            else
            {
                // Use Reflection to get the property value
                value = Convert.ToString(t.InvokeMember(PropertyName, BindingFlags.GetProperty, null, ctl, null));
            }

            return value;
        }

        public static string GetKeyValues(System.Web.UI.ControlCollection ctls, ref lodControlList lstPgCrtls, string parentAttr)
        {
            StringBuilder sb = new StringBuilder(1024);
            ModControl dc;
            string valString = string.Empty;
            string sectionString = string.Empty;
            string nameString = string.Empty;
            string fieldString = string.Empty;

            string TypeString = string.Empty;

            foreach (Control ctl in ctls)
            {
                TypeString = ctl.GetType().ToString();
                if (ctl.HasControls())
                {
                    parentAttr = GetAttribute(ctl, TypeString, "Section");

                    if (TypeString != "System.Web.UI.WebControls.CheckBoxList" && TypeString != "System.Web.UI.WebControls.RadioButtonList")
                    { //This is to stop it for recursing for any child controls for CheckBox list since we take care of that in getting multilist values
                        // Recurse into any "Grouping" controls ,pass the parentAttribute
                        sb.Append(GetKeyValues(ctl.Controls, ref lstPgCrtls, parentAttr));
                    }
                }

                // Get Control Definition
                dc = ControlLoad.mControlList.GetByType(TypeString);

                if (dc != null)
                {
                    if (dc.IsMultiList)
                    {
                        if (dc.PropertyToCheck == string.Empty)
                        {
                            if (TypeString == "System.Web.UI.WebControls.ListBox")
                            {
                                valString = GetMultiListValues((ListBox)ctl);
                                sb.Append(valString);
                            }
                            else if (TypeString == "System.Web.UI.WebControls.CheckBoxList")
                            {
                                valString = GetMultiListValues((CheckBoxList)ctl);
                                sb.Append(valString);
                            }
                            else if (TypeString == "System.Web.UI.WebControls.RadioButtonList")
                            {
                                valString = GetMultiListValues((RadioButtonList)ctl);
                                sb.Append(valString);
                            }
                        }
                        else
                        {
                            //Case for ListBox for single and multiple selection for values
                            if (GetControlValue(ctl, dc.PropertyToCheck) == dc.PropertyToCheckValue)
                            {
                                valString = GetControlValue(ctl, dc.Property1);
                                sb.Append(valString);
                            }
                            else
                            {
                                valString = GetMultiListValues((ListBox)ctl);
                                sb.Append(valString);
                            }
                        }
                    }
                    else
                    {
                        valString = GetControlValue(ctl, dc.Property1);
                        sb.Append(valString);
                    }

                    if (parentAttr == "")
                    {
                        sectionString = GetAttribute(ctl, TypeString, "Section");
                    }
                    else
                    {
                        sectionString = parentAttr;
                    }

                    fieldString = GetAttribute(ctl, TypeString, "Field");

                    lstPgCrtls.Add(new lodControl(ctl.ClientID, valString, sectionString, fieldString));
                }

            }
            return sb.ToString();
        }

        public static string GetMultiListValues(object ctl)
        {
            StringBuilder sb = new StringBuilder(200);
            ListItem lstItem;
            int i;
            // Build a concatenated list of selected indices
            // ctl is Object, need to cast to access Items. 
            // ListBox, CheckBoxList, RadioButtonList all have Items property but they don't share a common interface with Items property in .NET 4.8 easily accessible without casting or dynamic.
            // But here we can use dynamic or reflection, or cast based on type.
            // The VB code relied on late binding or implicit interface.
            // I'll use dynamic to avoid repetitive casting if possible, or just cast to ListControl which is the base class for all these.
            
            ListControl listControl = ctl as ListControl;
            if (listControl != null)
            {
                foreach (ListItem item in listControl.Items)
                {
                    if (item.Selected)
                    {
                        i = listControl.Items.IndexOf(item);
                        sb.Append(item.Text + ",");
                    }
                }
            }

            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }

            return sb.ToString();
        }

        #endregion

        #region Static Functions to be called from Pages

        //Following Static Code is used to access Values
        //**************************************************
        //Code to get Hash Values
        public static int GetInitialHash(System.Web.UI.ControlCollection ctls)
        {
            // Set the Initial Hash Value
            int InitialHash;
            InitialHash = GetControls(ctls).GetHashCode();
            return InitialHash;
        }

        public static bool HasChanged(System.Web.UI.ControlCollection ctls, string InitialHash)
        {
            if (InitialHash == "") return false; //There was no initial Value so no mods

            int newHash;
            // Get the Ending Hash Value
            newHash = GetControls(ctls).GetHashCode();
            return (int.Parse(InitialHash) != newHash);
        }

        //**************************************************

        #endregion
    }
}
