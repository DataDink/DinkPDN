using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    public static class ObjectExtensions
    {
        public static T GetValue<T>(this object value, T fallback = default(T)) { return value is T ? (T)value : fallback; }
    }
}
