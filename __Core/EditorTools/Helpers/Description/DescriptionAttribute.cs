using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AltSalt
{
    public class DescriptionAttribute : PropertyAttribute {

        public FontStyle fontStyle;
        public TextAnchor textAnchor;

        public DescriptionAttribute(FontStyle fontStyle, TextAnchor textAnchor)
        {
            this.fontStyle = fontStyle;
            this.textAnchor = textAnchor;
        }
    }

}