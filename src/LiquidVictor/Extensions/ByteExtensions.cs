using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Extensions
{
    public static class ByteExtensions
    {
        public static string ToBase64(this byte[] content)
        {
            return Convert.ToBase64String(content);
        }
    }
}
