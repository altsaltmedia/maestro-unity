﻿/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [CreateAssetMenu(menuName = "AltSalt/Color Variable")]
    public class ColorVariable : ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        public string DeveloperDescription = "";
#endif
        public Color Value = new Color(1,1,1,1);
        public bool hasDefault;

        [ShowIf("hasDefault")]
        public Color DefaultValue;

        public void SetValue(Color value)
        {
            Value = value;
        }

        public void SetValue(ColorVariable value)
        {
            Value = value.Value;
        }

        public void SetDefaultValue()
        {
            if(hasDefault) {
                Value = DefaultValue;   
            } else {
                Debug.LogWarning("Method SetDefaultValue() called on " + this.name + ", but var does not have a default value assigned.");
            }
        }
    }
}