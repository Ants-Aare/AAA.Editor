using AAA.Editor.Runtime.Attributes;
using UnityEditor;
using UnityEngine;

namespace AAA.Editor.Editor.Attributes
{
    [CustomPropertyDrawer(typeof(NamedArrayAttribute))]
    public class NamedArrayDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Properly configure height for expanded contents.
            return EditorGUI.GetPropertyHeight(property, label, property.isExpanded);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Replace label with enum name if possible.
            try
            {
                var config = attribute as NamedArrayAttribute;
                if (config.TargetEnum != null)
                {
                    var enumNames = System.Enum.GetNames(config.TargetEnum);
                    int pos = int.Parse(property.propertyPath.Split('[', ']')[1]);
                    var labelName = enumNames.GetValue(pos) as string;
                    // Make names nicer to read (but won't exactly match enum definition).
                    labelName = ObjectNames.NicifyVariableName(labelName.ToLower());
                    label = new GUIContent(labelName);
                }
                else if (config.Names != null || config.Names.Length != 0)
                {
                    var pos = int.Parse(property.propertyPath.Split('[', ']')[1]);
                    if (pos < config.Names.Length)
                    {
                        var labelName = config.Names[pos];
                        label = new GUIContent(labelName);
                    }
                }
            }
            catch
            {
                // keep default label
            }

            EditorGUI.PropertyField(position, property, label, property.isExpanded);
        }
    }
}