using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AltSalt.Maestro
{
    [CustomPropertyDrawer(typeof(DescriptionAttribute))]
    public class DescriptionDrawer : PropertyDrawer
    {
        GUIStyle guiStyle = new GUIStyle("Label");

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DescriptionAttribute descriptionStyles = attribute as DescriptionAttribute;

            guiStyle.wordWrap = true;
            guiStyle.fontStyle = descriptionStyles.fontStyle;
            guiStyle.alignment = descriptionStyles.textAnchor;

            EditorGUI.LabelField(new Rect(position.width * .2f, position.y, position.width * .6f, position.height + 20), property.stringValue, guiStyle);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label) + 20;
        }
    }
}