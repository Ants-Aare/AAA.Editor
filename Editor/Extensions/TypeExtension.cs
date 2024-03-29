using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace AAA.Editor.Editor.Extensions
{
  public static class TypeExtension
  {
    private static readonly Dictionary<string, string> BuiltInTypesToString = new Dictionary<string, string>()
    {
      {
        "System.Boolean",
        "bool"
      },
      {
        "System.Byte",
        "byte"
      },
      {
        "System.SByte",
        "sbyte"
      },
      {
        "System.Char",
        "char"
      },
      {
        "System.Decimal",
        "decimal"
      },
      {
        "System.Double",
        "double"
      },
      {
        "System.Single",
        "float"
      },
      {
        "System.Int32",
        "int"
      },
      {
        "System.UInt32",
        "uint"
      },
      {
        "System.Int64",
        "long"
      },
      {
        "System.UInt64",
        "ulong"
      },
      {
        "System.Object",
        "object"
      },
      {
        "System.Int16",
        "short"
      },
      {
        "System.UInt16",
        "ushort"
      },
      {
        "System.String",
        "string"
      },
      {
        "System.Void",
        "void"
      }
    };
    private static readonly Dictionary<string, string> BuiltInTypeStrings = new Dictionary<string, string>()
    {
      {
        "bool",
        "System.Boolean"
      },
      {
        "byte",
        "System.Byte"
      },
      {
        "sbyte",
        "System.SByte"
      },
      {
        "char",
        "System.Char"
      },
      {
        "decimal",
        "System.Decimal"
      },
      {
        "double",
        "System.Double"
      },
      {
        "float",
        "System.Single"
      },
      {
        "int",
        "System.Int32"
      },
      {
        "uint",
        "System.UInt32"
      },
      {
        "long",
        "System.Int64"
      },
      {
        "ulong",
        "System.UInt64"
      },
      {
        "object",
        "System.Object"
      },
      {
        "short",
        "System.Int16"
      },
      {
        "ushort",
        "System.UInt16"
      },
      {
        "string",
        "System.String"
      },
      {
        "void",
        "System.Void"
      }
    };

    public static bool ImplementsInterface<T>(this Type type) => !type.IsInterface && type.GetInterface(typeof (T).FullName) != (Type) null;

    public static string ToCompilableString(this Type type)
    {
      string compilableString;
      if (TypeExtension.BuiltInTypesToString.TryGetValue(type.FullName, out compilableString))
        return compilableString;
      if (type.IsGenericType)
      {
        IEnumerable<string> values = ((IEnumerable<Type>) type.GetGenericArguments()).Select<Type, string>((Func<Type, string>) (argType => argType.ToCompilableString()));
        return type.FullName.Split('`')[0] + "<" + string.Join(", ", values) + ">";
      }
      if (type.IsArray)
        return type.GetElementType().ToCompilableString() + "[" + new string(',', type.GetArrayRank() - 1) + "]";
      return type.IsNested ? type.FullName.Replace('+', '.') : type.FullName;
    }

    public static Type ToType(this string typeString)
    {
      string typeString1 = TypeExtension.GenerateTypeString(typeString);
      Type type1 = Type.GetType(typeString1);
      if (type1 != (Type) null)
        return type1;
      foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
      {
        Type type2 = assembly.GetType(typeString1);
        if (type2 != (Type) null)
          return type2;
      }
      return (Type) null;
    }

    public static string ShortTypeName(this string fullTypeName)
    {
      string[] strArray = fullTypeName.Split('.');
      return strArray[strArray.Length - 1];
    }

    public static string RemoveDots(this string fullTypeName) => fullTypeName.Replace(".", string.Empty);

    private static string GenerateTypeString(string typeString)
    {
      string str;
      if (TypeExtension.BuiltInTypeStrings.TryGetValue(typeString, out str))
      {
        typeString = str;
      }
      else
      {
        typeString = TypeExtension.GenerateGenericArguments(typeString);
        typeString = TypeExtension.GenerateArray(typeString);
      }
      return typeString;
    }

    private static string GenerateGenericArguments(string typeString)
    {
      string[] separator = new string[1]{ ", " };
      typeString = Regex.Replace(typeString, "<(?<arg>.*)>", (MatchEvaluator) (match =>
      {
        string typeString1 = TypeExtension.GenerateTypeString(match.Groups["arg"].Value);
        return string.Format("`{0}[{1}]", (object) typeString1.Split(separator, StringSplitOptions.None).Length, (object) typeString1);
      }));
      return typeString;
    }

    private static string GenerateArray(string typeString)
    {
      typeString = Regex.Replace(typeString, "(?<type>[^\\[]*)(?<rank>\\[,*\\])", (MatchEvaluator) (match => TypeExtension.GenerateTypeString(match.Groups["type"].Value) + match.Groups["rank"].Value));
      return typeString;
    }
  }
}
