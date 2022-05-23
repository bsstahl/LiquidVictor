using LiquidVictor.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Extensions
{
    public static class ContentItemExtensions
    {
        public static ICollection<KeyValuePair<int, ContentItem>> Clone(this IEnumerable<KeyValuePair<int, ContentItem>> contentItemPairs)
        {
            var result = new List<KeyValuePair<int, ContentItem>>();
            foreach (var pair in contentItemPairs)
            {
                result.Add(new KeyValuePair<int, ContentItem>(pair.Key, pair.Value));
            }
            return result;
        }

        public static byte[] DecodeContent(this string content, string contentType)
        {
            byte[] result = null;
            if (contentType.ToLower().StartsWith("text"))
            {
                // Unencoded, just convert to byte array
                result = System.Text.Encoding.UTF8.GetBytes(content);
            }
            else
            {
                // Unencode base 64 string
                result = System.Convert.FromBase64String(content);
            }
            return result;
        }

        public static string EncodeContent(this byte[] content, string contentType)
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
