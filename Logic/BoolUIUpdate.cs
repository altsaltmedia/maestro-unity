using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DoozyUI;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Logic
{

    public class BoolUIUpdate : MonoBehaviour
    {

        // TO DO - abstract this so that it can update both float and text values if needed
        // ALSO... figure out how this script works in conjunction with localization
        [ValidateInput(nameof(IsPopulated))]
        [FormerlySerializedAs("boolValue")]
        [SerializeField]
        private BoolReference _boolValue = new BoolReference();

        private bool boolValue => _boolValue.GetValue(this.gameObject);

        // Use this for initialization
        void Start()
        {
            UIToggle toggle = GetComponent<UIToggle>();
            toggle.IsOn = boolValue;
        }

        private static bool IsPopulated(BoolReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }

}