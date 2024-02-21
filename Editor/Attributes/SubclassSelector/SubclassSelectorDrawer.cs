using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

[CustomPropertyDrawer(typeof(SubclassSelectorAttribute))]
public class SubclassSelectorDrawer : PropertyDrawer
{
    private struct TypePopupCache
    {
        public AdvancedTypePopup TypePopup { get; }
        public AdvancedDropdownState State { get; }

        public TypePopupCache(AdvancedTypePopup typePopup, AdvancedDropdownState state)
        {
            TypePopup = typePopup;
            State = state;
        }
    }

    private const int MaxTypePopupLineCount = 13;
    private static readonly Type UnityObjectType = typeof(UnityEngine.Object);
    private static readonly GUIContent NullDisplayName = new(TypeMenuUtility.NullDisplayName);
    private static readonly GUIContent IsNotManagedReferenceLabel = new("The property type is not manage reference.");

    private readonly Dictionary<string, TypePopupCache> typePopups = new();
    private readonly Dictionary<string, GUIContent> typeNameCaches = new();

    private SerializedProperty targetProperty;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        if (property.propertyType == SerializedPropertyType.ManagedReference)
        {
            var popupPosition = new Rect(position);
            popupPosition.width -= EditorGUIUtility.labelWidth;
            popupPosition.x += EditorGUIUtility.labelWidth;
            popupPosition.height = EditorGUIUtility.singleLineHeight;

            if (EditorGUI.DropdownButton(popupPosition, GetTypeName(property), FocusType.Keyboard))
            {
                var popup = GetTypePopup(property);
                targetProperty = property;
                popup.TypePopup.Show(popupPosition);
            }

            EditorGUI.PropertyField(position, property, label, true);
        }
        else if (property.propertyType == SerializedPropertyType.ObjectReference && fieldInfo.FieldType.IsSubclassOf(typeof(MonoBehaviour)) || (fieldInfo.FieldType.IsArray && fieldInfo.FieldType.GetElementType()!.IsSubclassOf(typeof(MonoBehaviour))))
        {
            // if (property.objectReferenceValue == null)
            // {
                EditorGUI.PropertyField(position, property);
                var popupPosition = new Rect(position);
                popupPosition.width = Mathf.Max(EditorGUIUtility.labelWidth * 0.5f, 50);
                popupPosition.x += EditorGUIUtility.labelWidth - popupPosition.width;
                popupPosition.height = EditorGUIUtility.singleLineHeight;

                if (EditorGUI.DropdownButton(popupPosition, new GUIContent((fieldInfo.FieldType.IsArray ? fieldInfo.FieldType.GetElementType()! : fieldInfo.FieldType).Name), FocusType.Keyboard))
                {
                    var popup = GetTypePopupForMonoBehaviours(property);
                    targetProperty = property;
                    popup.TypePopup.Show(popupPosition);
                }
            // }
            // else
            // {
            //     EditorGUI.PropertyField(position, property);
            // }
        }
        else
        {
            EditorGUI.LabelField(position, label, IsNotManagedReferenceLabel);
        }
        // var target = property.serializedObject.targetObject;
        // Debug.Log(target.name + target.GetType());
        // Debug.Log($"Type: {fieldInfo.FieldType} {fieldInfo.FieldType.IsSubclassOf(typeof(MonoBehaviour)) || (fieldInfo.FieldType.IsArray && fieldInfo.FieldType.GetElementType()!.IsSubclassOf(typeof(MonoBehaviour)))} {property.propertyType}");
        // EditorGUI.LabelField(position, label, IsNotManagedReferenceLabel);

        EditorGUI.EndProperty();
    }

    private TypePopupCache GetTypePopup(SerializedProperty property)
    {
        // Cache this string. This property internally call Assembly.GetName, which result in a large allocation.
        var managedReferenceFieldTypename = property.managedReferenceFieldTypename;

        if (typePopups.TryGetValue(managedReferenceFieldTypename, out var result)) return result;
        var state = new AdvancedDropdownState();

        var baseType = ManagedReferenceUtility.GetType(managedReferenceFieldTypename);
        var popup = new AdvancedTypePopup(
            TypeCache.GetTypesDerivedFrom(baseType).Append(baseType).Where(p =>
                (p.IsPublic || p.IsNestedPublic) &&
                !p.IsAbstract &&
                !p.IsGenericType &&
                !UnityObjectType.IsAssignableFrom(p) &&
                Attribute.IsDefined(p, typeof(SerializableAttribute))
            ),
            MaxTypePopupLineCount,
            state
        );
        popup.OnItemSelected += item =>
        {
            var type = item.Type;
            var obj = targetProperty.SetManagedReference(type);
            targetProperty.isExpanded = (obj != null);
            targetProperty.serializedObject.ApplyModifiedProperties();
            targetProperty.serializedObject.Update();
        };

        result = new TypePopupCache(popup, state);
        typePopups.Add(managedReferenceFieldTypename, result);

        return result;
    }

    private TypePopupCache GetTypePopupForMonoBehaviours(SerializedProperty property)
    {
        // Cache this string. This property internally call Assembly.GetName, which result in a large allocation.
        var baseType = fieldInfo.FieldType.IsArray ? fieldInfo.FieldType.GetElementType()! : fieldInfo.FieldType;
        var typeName = baseType.Name;

        if (typePopups.TryGetValue(typeName, out var result)) return result;
        var state = new AdvancedDropdownState();

        var popup = new AdvancedTypePopup(
            TypeCache.GetTypesDerivedFrom(baseType).Append(baseType).Where(p
                => p.IsPublic
                   && !p.IsAbstract
                   && !p.IsGenericType
            ),
            MaxTypePopupLineCount,
            state
        );
        popup.OnItemSelected += item =>
        {
            var type = item.Type;
            // var obj = targetProperty.SetManagedReference(type);
            if (type == null)
            {
                targetProperty.objectReferenceValue = null;
            }
            else
            {
                var gameObject = ((MonoBehaviour)targetProperty.serializedObject.targetObject).gameObject;
                var component = gameObject.AddComponent(type);
                targetProperty.objectReferenceValue = component;
            }

            // targetProperty.isExpanded = (obj != null);
            targetProperty.serializedObject.ApplyModifiedProperties();
            targetProperty.serializedObject.Update();
        };

        result = new TypePopupCache(popup, state);
        typePopups.Add(typeName, result);

        return result;
    }

    private GUIContent GetTypeName(SerializedProperty property)
    {
        // Cache this string.
        var managedReferenceFullTypename = property.managedReferenceFullTypename;

        if (string.IsNullOrEmpty(managedReferenceFullTypename))
        {
            return NullDisplayName;
        }

        if (typeNameCaches.TryGetValue(managedReferenceFullTypename, out var cachedTypeName))
        {
            return cachedTypeName;
        }

        var type = ManagedReferenceUtility.GetType(managedReferenceFullTypename);
        string typeName = null;

        var typeMenu = TypeMenuUtility.GetAttribute(type);
        if (typeMenu != null)
        {
            typeName = typeMenu.GetTypeNameWithoutPath();
            if (!string.IsNullOrWhiteSpace(typeName))
            {
                typeName = ObjectNames.NicifyVariableName(typeName);
            }
        }

        if (string.IsNullOrWhiteSpace(typeName))
        {
            typeName = ObjectNames.NicifyVariableName(type.Name);
        }

        var result = new GUIContent(typeName);
        typeNameCaches.Add(managedReferenceFullTypename, result);
        return result;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, true);
    }
}