using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace ALODWebUtility.Common
{
    public class VerifyIOSignature
    {
        public string ErrorMessage;
        private bool ListLoaded;
        private List<int> lodList = new List<int>();

        public bool VerifyLod(int lod)
        {
            LoadList();

            if (lodList.Contains(lod))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void LoadList()
        {
            string LodsFromFile;

            try
            {
                LodsFromFile = File.ReadAllText(HttpContext.Current.Server.MapPath("~/App_Data/VerifyLodsForIOSignature.txt"));

                string[] Lods = LodsFromFile.Split(new char[] { ',' });

                foreach (string lod in Lods)
                {
                    lodList.Add(Convert.ToInt32(lod));
                }

                ListLoaded = true;
            }
            catch (FileNotFoundException ex)
            {
                ListLoaded = false;
                ErrorMessage = ex.Message;
            }
            catch (FileLoadException ex)
            {
                ListLoaded = false;
                ErrorMessage = ex.Message;
            }
        }
    }
}
