using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Extensions
{
    public static class StringExtensions
    {
        public static byte[] AsByteArray(this string content)
        {
            return Encoding.ASCII.GetBytes(content);
        }

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

        public static string NormalizeWhiteSpace(this string[] lines)
        {
            bool preNormalized = false;

            string line1 = null;
            // find first non-empty line
            for (int i = 0; i < lines.Length; i++)
            {
                line1 = lines[i];
                if (!string.IsNullOrEmpty(line1))
                    break;
            }

            if (string.IsNullOrEmpty(line1))
                preNormalized = true;

            int whitespaceCount = 0;
            if (!preNormalized)
            {
                string trimLine = line1.TrimStart();
                whitespaceCount = line1.Length - trimLine.Length;
                if (whitespaceCount == 0)
                    preNormalized = true;
            }

            string result;
            if (preNormalized)
                result = string.Join("\r\n", lines);
            else
            {
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Length > whitespaceCount)
                        sb.AppendLine(lines[i].Substring(whitespaceCount));
                    else
                        sb.AppendLine(lines[i]);
                }
                result = sb.ToString();
            }

            return result;
        }

    }
}
