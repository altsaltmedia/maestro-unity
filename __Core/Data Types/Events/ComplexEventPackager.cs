using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    public class ComplexEventPackager : MonoBehaviour
    {
        [Required]
        [SerializeField]
        ComplexEvent Event;

        [SerializeField]
        bool hasString;

        [SerializeField]
        [ShowIf("hasString")]
        string stringDescription = "";

        [SerializeField]
        [ShowIf("hasString")]
        string stringValue;

        [SerializeField]
        bool hasFloat;

        [SerializeField]
        [ShowIf("hasFloat")]
        string floatDescription = "";

        [SerializeField]
        [ShowIf("hasFloat")]
        float floatValue;

        [SerializeField]
        bool hasBool;

        [SerializeField]
        [ShowIf("hasBool")]
        string boolDescription = "";

        [SerializeField]
        [ShowIf("hasBool")]
        [ValueDropdown("boolValueList")]
        bool boolValue;

        private ValueDropdownList<bool> boolValueList = new ValueDropdownList<bool>(){
            {"FALSE", false },
            {"TRUE", true }
        };

        // Update is called once per frame
        public void RaiseComplexEvent ()
        {
            EventPayload eventPayload = new EventPayload();
            if(hasString) {
                eventPayload.Set(EventPayloadType.stringPayload.ToString(), stringValue);
            }
            if(hasFloat) {
                eventPayload.Set(EventPayloadType.floatPayload.ToString(), floatValue);
            }
            if (hasBool) {
                eventPayload.Set(EventPayloadType.boolPayload.ToString(), boolValue);
            }
            Event.Raise(eventPayload);
        }
    }

}