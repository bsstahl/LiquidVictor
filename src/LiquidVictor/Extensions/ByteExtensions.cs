using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Extensions
{
    public static class ByteExtensions
    {
        public static string AsBase64String(this byte[] content)
        {
            return Convert.ToBase64String(content);
        }

        public static string AsString(this byte[] content)
        {
            return System.Text.Encoding.UTF8.GetString(content);
        }
    }
}
