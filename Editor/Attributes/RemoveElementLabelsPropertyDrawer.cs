using AAA.Editor.Runtime.Attributes;
using UnityEditor;
using UnityEngine;

namespace AAA.Editor.Editor.Attributes
{
    [CustomPropertyDrawer(typeof(RemoveElementLabelsAttribute))]
    public class RemoveElementLabelsPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, property.isExpanded);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.PropertyField(position, property, GUIContent.none, property.isExpanded);
        }
    }
}