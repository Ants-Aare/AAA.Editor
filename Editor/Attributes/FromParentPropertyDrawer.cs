using System;
using AAA.Editor.Runtime.Attributes;
using UnityEditor;
using UnityEngine;

namespace AAA.Editor.Editor.Attributes
{
    [CustomPropertyDrawer(typeof(FromParentAttribute))]
    public class FromParentPropertyDrawer : PropertyDrawer
    {
        Action<Rect, SerializedProperty, GUIContent> _onGui;

        public override void OnGUI(
            Rect position,
            SerializedProperty property,
            GUIContent label
        )
        {
            if (_onGui == null)
            {
                _onGui = GetOnGui(property);
            }

            _onGui.Invoke(position, property, label);
        }

        Action<Rect, SerializedProperty, GUIContent> GetOnGui(SerializedProperty property)
        {
            if (!(property.serializedObject.targetObject is Component))
            {
                return OnGuiNotComponent;
            }

            if (!typeof(Component).IsAssignableFrom(fieldInfo.FieldType))
            {
                return OnGuiNotComponentField;
            }

            return OnGuiComponent;
        }

        void OnGuiComponent(
            Rect rect,
            SerializedProperty property,
            GUIContent label
        )
        {
            if (property.objectReferenceValue == null)
            {
                var monoBehaviour = (Component)property.serializedObject.targetObject;
                if(monoBehaviour.transform.parent != null)
                    property.objectReferenceValue = monoBehaviour.GetComponentInParent(fieldInfo.FieldType);
            }

            EditorGUI.PropertyField(rect, property, label);
        }

        void OnGuiNotComponent(
            Rect arg1,
            SerializedProperty arg2,
            GUIContent arg3
        )
        {
            EditorGUI.LabelField(arg1, "Self can only be used on Component");
        }

        void OnGuiNotComponentField(
            Rect arg1,
            SerializedProperty arg2,
            GUIContent arg3
        )
        {
            EditorGUI.LabelField(arg1, "Self can only be used on Component fields");
        }
    }
}