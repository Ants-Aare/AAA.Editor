using System;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
public sealed class AddTypeMenuAttribute : Attribute
{
    public string MenuName { get; }

    public int Order { get; }

    public AddTypeMenuAttribute(string menuName, int order = 0)
    {
        MenuName = menuName;
        Order = order;
    }

    static readonly char[] separators = new char[] { '/' };

    public string[] GetSplitMenuName()
    {
        return !string.IsNullOrWhiteSpace(MenuName) ? MenuName.Split(separators, StringSplitOptions.RemoveEmptyEntries) : Array.Empty<string>();
    }

    public string GetTypeNameWithoutPath()
    {
        var splitDisplayName = GetSplitMenuName();
        return (splitDisplayName.Length != 0) ? splitDisplayName[^1] : null;
    }
}
