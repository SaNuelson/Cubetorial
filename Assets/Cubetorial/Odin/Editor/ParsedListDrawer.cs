using System;
using Cubetorial.Odin.Attributes;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using UnityEditor;
using UnityEngine;

namespace Cubetorial.Odin.Editor
{
    public class ParsedListDrawer : OdinAttributeDrawer<ParsedListAttribute, string[]>
    {
        protected override void DrawPropertyLayout(GUIContent label)
        {
            var list = ValueEntry.SmartValue;
            var text = list.IsNullOrEmpty()
                ? ""
                : string.Join(Attribute.Separator, list);

            var newText = EditorGUILayout.TextField(text);
            ValueEntry.SmartValue = newText.Split(new[] { Attribute.Separator }, StringSplitOptions.None);
        }
    }
}