using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DoozyUI;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro
{

    public class BoolUIUpdate : MonoBehaviour
    {

        // TO DO - abstract this so that it can update both float and text values if needed
        // ALSO... figure out how this script works in conjunction with localization
        [ValidateInput("IsPopulated")]
        public BoolReference boolValue = new BoolReference();

        // Use this for initialization
        void Start()
        {
            UIToggle toggle = GetComponent<UIToggle>();
            toggle.IsOn = boolValue.Value;
        }

        private static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }

}