﻿using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Extensions
{
    public static class StringExtensions
    {
        public static string? NullIfEmpty(this string value)
        {
            return string.IsNullOrEmpty(value)
                ? null
                : value;
        }

        public static byte[] AsByteArray(this string content)
        {
            return Encoding.ASCII.GetBytes(content);
        }

        public static byte[] FromBase64String(this string content)
        {
            return Convert.FromBase64String(content);
        }

        public static string AsBase64String(this string content)
        {
            return content.AsByteArray().AsBase64String();
        }

        public static string AsFilename(this string name)
        {
            return name?
                .Replace(" ", "_") ?? string.Empty;
        }

        public static string Clean(this string value)
        {
            return value?
                .Replace(":", "_")
                .Replace(",", "_")
                .Replace(" & ", " and ")
                .Replace("&", "and")
                .Replace("_ ", "_")
                .Replace(" _", "_")
                .Replace(" - ", "-")
                .Replace("- ", "-")
                .Replace("'", "") ?? string.Empty;
        }

        public static string AsFileExtension(this string imageFormat)
        {
            var result = string.Empty;
            imageFormat ??= string.Empty;
            if (imageFormat.Equals("image/jpg", StringComparison.OrdinalIgnoreCase) || imageFormat.Equals("image/jpeg", StringComparison.OrdinalIgnoreCase))
                result = "jpg";
            return result;
        }

        //public static string NormalizeWhiteSpace(this string[] lines)
        //{
        //    bool preNormalized = false;

        //    string line1 = null;
        //    // find first non-empty line
        //    for (int i = 0; i < lines.Length; i++)
        //    {
        //        line1 = lines[i];
        //        if (!string.IsNullOrEmpty(line1))
        //            break;
        //    }

        //    if (string.IsNullOrEmpty(line1))
        //        preNormalized = true;

        //    int whitespaceCount = 0;
        //    if (!preNormalized)
        //    {
        //        string trimLine = line1.TrimStart();
        //        whitespaceCount = line1.Length - trimLine.Length;
        //        if (whitespaceCount == 0)
        //            preNormalized = true;
        //    }

        //    string result;
        //    if (preNormalized)
        //        result = string.Join("\r\n", lines);
        //    else
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        for (int i = 0; i < lines.Length; i++)
        //        {
        //            if (lines[i].Length > whitespaceCount)
        //                sb.AppendLine(lines[i].Substring(whitespaceCount));
        //            else
        //                sb.AppendLine(lines[i]);
        //        }
        //        result = sb.ToString();
        //    }

        //    return result;
        //}

    }
}
