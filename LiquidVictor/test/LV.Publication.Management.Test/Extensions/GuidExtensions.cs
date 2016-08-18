using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LV.Publication.Management.Test.Extensions
{
    public static class GuidExtensions
    {
        public static bool IsEven(this Guid guid)
        {
            var elements = guid.ToByteArray();
            return (elements[15] % 2 == 0);
        }
    }
}
