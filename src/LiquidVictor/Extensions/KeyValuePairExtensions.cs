using System;
using System.Collections.Generic;
using System.Text;

namespace LiquidVictor.Extensions
{
    public static class KeyValuePairExtensions
    {
        public static bool HasValue<TKey, TValue>(this KeyValuePair<TKey,TValue> pair)
        {
            // TODO: Find out if the parameter could ever be null and handle if it could

            bool result = true;
            if (pair.Value == null)
                result = false;
            return result;
        }
    }
}
