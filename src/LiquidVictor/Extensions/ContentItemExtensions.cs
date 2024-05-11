using LiquidVictor.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace LiquidVictor.Extensions
{
    public static class ContentItemExtensions
    {
        public static ICollection<KeyValuePair<int, ContentItem>> Clone(this IEnumerable<KeyValuePair<int, ContentItem>> contentItemPairs, bool createNewId = false)
        {
            var result = new List<KeyValuePair<int, ContentItem>>();
            foreach (var pair in contentItemPairs ?? [])
            {
                result.Add(new KeyValuePair<int, ContentItem>(pair.Key, pair.Value.Clone(createNewId)));
            }
            return result;
        }

        public static byte[] DecodeContent(this string content, string contentType)
        {
            if (string.IsNullOrWhiteSpace(contentType)) 
                throw new ArgumentNullException(nameof(contentType));

            if (content is null)
                throw new ArgumentNullException(nameof(content));

            byte[] result;
            if (contentType.StartsWith("text", StringComparison.OrdinalIgnoreCase))
            {
                // Unencoded, just convert to byte array
                result = Encoding.UTF8.GetBytes(content.Replace("\\r\\n", Environment.NewLine));
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
            string result;
            if (contentType?.StartsWith("text", StringComparison.OrdinalIgnoreCase) ?? false)
            {
                // Unencoded, just convert from byte array and flatten
                result = Encoding.UTF8.GetString(content).Replace(Environment.NewLine, "\\r\\n");
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
