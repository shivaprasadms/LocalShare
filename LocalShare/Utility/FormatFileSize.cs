using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalShare.Utility
{
    public class FormatFileSize
    {
        public static string GetSize(long fileSizeInBytes)
        {
            string[] sizeSuffixes = { "B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            const int byteConversion = 1024;

            if (fileSizeInBytes == 0)
            {
                return "0B";
            }

            int place = Convert.ToInt32(Math.Floor(Math.Log(fileSizeInBytes, byteConversion)));
            double fileSize = Math.Round(fileSizeInBytes / Math.Pow(byteConversion, place), 1);

            return $"{fileSize} {sizeSuffixes[place]}";
        }
    }
}
