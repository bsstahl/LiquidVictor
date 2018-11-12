using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Extensions
{
    public static class StringExtensions
    {
        public static string AsFilename(this string name)
        {
            return name.Replace(" ", "_");
        }

        public static string AsFileExtension(this string imageFormat)
        {
            var result = string.Empty;

            if (imageFormat.ToLower().Equals("image/jpg") || imageFormat.ToLower().Equals("image/jpeg"))
                result = "jpg";

            return result;
        }
    }
}
