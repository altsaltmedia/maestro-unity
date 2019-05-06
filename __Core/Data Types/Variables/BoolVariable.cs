/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using UnityEngine;

namespace AltSalt
{
    [CreateAssetMenu(menuName = "AltSalt/Bool Variable")]
    public class BoolVariable : ScriptableObject
    {
#if UNITY_EDITOR
        [Multiline]
        [SerializeField]
        [Header("Bool Variable")]
        string DeveloperDescription = "";
#endif
        public bool Value;

        public void SetValue(bool value)
        {
            Value = value;
        }

        public void SetValue(BoolVariable value)
        {
            Value = value.Value;
        }

        public void Toggle()
        {
            Value = !Value;
        }
    }
}