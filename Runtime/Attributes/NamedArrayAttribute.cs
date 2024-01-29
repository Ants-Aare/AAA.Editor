using System;
using UnityEngine;

namespace AAA.Editor.Runtime.Attributes
{
    public class NamedArrayAttribute : PropertyAttribute {
        public readonly Type TargetEnum;
        public readonly string[] Names;
        public NamedArrayAttribute(Type targetEnum) {
            TargetEnum = targetEnum;
        }
        public NamedArrayAttribute(string[] names) {
            Names = names;
        }
    }
}