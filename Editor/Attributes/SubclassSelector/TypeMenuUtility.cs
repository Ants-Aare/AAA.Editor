#if UNITY_2019_3_OR_NEWER
using System;
using System.Collections.Generic;
using System.Linq;

    public static class TypeMenuUtility
    {
        public const string NullDisplayName = "<null>";

        public static AddTypeMenuAttribute GetAttribute(Type type)
        {
            return Attribute.GetCustomAttribute(type, typeof(AddTypeMenuAttribute)) as AddTypeMenuAttribute;
        }

        public static string[] GetSplitTypePath(Type type)
        {
            var typeMenu = GetAttribute(type);
            if (typeMenu != null)
            {
                return typeMenu.GetSplitMenuName();
            }
            else
            {
                var splitIndex = type.FullName.LastIndexOf('.');
                return splitIndex >= 0
                    ? new string[] { type.FullName[..splitIndex], type.FullName[(splitIndex + 1)..] }
                    : new string[] { type.Name };
            }
        }

        public static IEnumerable<Type> OrderByType(this IEnumerable<Type> source)
            => source.OrderBy(type => type == null ? -999 : GetAttribute(type)?.Order ?? 0);
    }
#endif
