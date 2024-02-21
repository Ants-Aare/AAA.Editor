using System;
using System.Reflection;
using UnityEditor;

public static class ManagedReferenceUtility
{
    public static object SetManagedReference(this SerializedProperty property, Type type)
    {
        object obj = (type != null) ? Activator.CreateInstance(type) : null;
        property.managedReferenceValue = obj;
        return obj;
    }

    public static Type GetType(string typeName)
    {
        var splitIndex = typeName.IndexOf(' ');
        var assembly = Assembly.Load(splitIndex == -1 ? typeName : typeName.Substring(0, splitIndex));
        return assembly.GetType(typeName.Substring(splitIndex + 1));
    }
}