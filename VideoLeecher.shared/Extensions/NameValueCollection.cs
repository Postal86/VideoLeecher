﻿using System.Collections.Specialized;
using System.Linq;

namespace VideoLeecher.shared.Extensions
{
    public static class NameValueCollectionExtensions
    {
        public static bool ContainsKey(this NameValueCollection collection, string key)
        {
            if (collection.Get(key) == null)
            {
                return collection.AllKeys.Contains(key);
            }

            return true;
        }
    }
}
