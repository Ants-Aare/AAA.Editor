using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class AdvancedTypePopupItem : AdvancedDropdownItem
{
    public Type Type { get; }

    public AdvancedTypePopupItem(Type type, string name) : base(name)
    {
        Type = type;
    }
}

public class AdvancedTypePopup : AdvancedDropdown
{
    const int MaxNamespaceNestCount = 16;

    static void AddTo(AdvancedDropdownItem root, IEnumerable<Type> types)
    {
        var itemCount = 0;

        var nullItem = new AdvancedTypePopupItem(null, TypeMenuUtility.NullDisplayName)
        {
            id = itemCount++
        };
        root.AddChild(nullItem);

        var typeArray = types.OrderByType().ToArray();

        var isSingleNamespace = true;
        var namespaces = new string[MaxNamespaceNestCount];
        foreach (var type in typeArray)
        {
            var splitTypePath = TypeMenuUtility.GetSplitTypePath(type);
            if (splitTypePath.Length <= 1)
            {
                continue;
            }

            for (var k = 0; (splitTypePath.Length - 1) > k; k++)
            {
                var namespaceName = namespaces[k];
                if (namespaceName == null)
                {
                    namespaces[k] = splitTypePath[k];
                }
                else if (namespaceName != splitTypePath[k])
                {
                    isSingleNamespace = false;
                    break;
                }
            }
        }

        foreach (var type in typeArray)
        {
            var typePath = TypeMenuUtility.GetSplitTypePath(type);
            if (typePath.Length == 0)
            {
                continue;
            }

            AdvancedDropdownItem parent = root;

            if (!isSingleNamespace)
            {
                for (var k = 0; (typePath.Length - 1) > k; k++)
                {
                    var foundItem = GetItem(parent, typePath[k]);
                    if (foundItem != null)
                    {
                        parent = foundItem;
                    }
                    else
                    {
                        var newItem = new AdvancedDropdownItem(typePath[k])
                        {
                            id = itemCount++,
                        };
                        parent.AddChild(newItem);
                        parent = newItem;
                    }
                }
            }

            var item = new AdvancedTypePopupItem(type, ObjectNames.NicifyVariableName(typePath[^1]))
            {
                id = itemCount++
            };
            parent.AddChild(item);
        }
    }

    static AdvancedDropdownItem GetItem(AdvancedDropdownItem parent, string name)
    {
        foreach (AdvancedDropdownItem item in parent.children)
        {
            if (item.name == name)
            {
                return item;
            }
        }

        return null;
    }

    static readonly float k_HeaderHeight = EditorGUIUtility.singleLineHeight * 2f;

    Type[] m_Types;

    public event Action<AdvancedTypePopupItem> OnItemSelected;

    public AdvancedTypePopup(IEnumerable<Type> types, int maxLineCount, AdvancedDropdownState state) : base(state)
    {
        SetTypes(types);
        minimumSize = new Vector2(minimumSize.x, EditorGUIUtility.singleLineHeight * maxLineCount + k_HeaderHeight);
    }

    public void SetTypes(IEnumerable<Type> types)
    {
        m_Types = types.ToArray();
    }

    protected override AdvancedDropdownItem BuildRoot()
    {
        var root = new AdvancedDropdownItem("Select Type");
        AddTo(root, m_Types);
        return root;
    }

    protected override void ItemSelected(AdvancedDropdownItem item)
    {
        base.ItemSelected(item);
        if (item is AdvancedTypePopupItem typePopupItem)
        {
            OnItemSelected?.Invoke(typePopupItem);
        }
    }
}