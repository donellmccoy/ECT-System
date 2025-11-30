using System.IO;
using WebSupergoo.ABCpdf8;

namespace ALOD.Core.Utils
{
    public static class ABCPDFUtility
    {
        public static XColor ConvertStringToXColor(string colorName)
        {
            XColor color = new XColor();
            color.Color = System.Drawing.Color.FromName(colorName);
            return color;
        }

        public static XImage GetFileAsXImage(string filePath, string fileName)
        {
            XImage image = new XImage();
            string fullPath = filePath + Path.DirectorySeparatorChar + fileName;

            if (string.IsNullOrEmpty(fileName))
                return image;

            if (!File.Exists(fullPath))
                return image;

            image.SetFile(fullPath);

            return image;
        }
    }
}