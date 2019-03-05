using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Data.JsonFileSystem
{
    internal class ContentItem
    {
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public string Title { get; set; }
        public string EncodedContent { get; set; }

        internal static byte[] DecodeContent(string contentType, string encodedContent)
        {
            byte[] result = null;
            if (contentType.ToLower().StartsWith("text"))
            {
                // Unencoded, just convert to byte array
                result = System.Text.Encoding.UTF8.GetBytes(encodedContent);
            }
            else
            {
                // Unencode base 64 string
                result = System.Convert.FromBase64String(encodedContent);
            }
            return result;
        }

        internal static string EncodeContent(string contentType, byte[] content)
        {
            string result = string.Empty;
            if (contentType.ToLower().StartsWith("text"))
            {
                // Unencoded, just convert from byte array
                result = System.Text.Encoding.UTF8.GetString(content);
            }
            else
            {
                // Base 64 encode the string
                result = System.Convert.ToBase64String(content);
            }
            return result;
        }
    }
}
