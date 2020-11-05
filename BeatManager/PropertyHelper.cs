using System;
using System.Reflection;

namespace BeatManager
{
    public static class PropertyHelper
    {
        public static object GetPropValue(this object obj, string name)
        {
            foreach (string part in name.Split('.'))
            {
                if (obj == null)
                    return null;

                Type type = obj.GetType();
                PropertyInfo info = type.GetProperty(part);
                if (info == null)
                    return null;

                obj = info.GetValue(obj, null);
            }

            return obj;
        }

        public static T GetPropValue<T>(this object obj, string name)
        {
            object retVal = GetPropValue(obj, name);
            if (retVal == null)
                return default;

            // throws InvalidCastException if types are incompatible
            return (T)retVal;
        }
    }
}
