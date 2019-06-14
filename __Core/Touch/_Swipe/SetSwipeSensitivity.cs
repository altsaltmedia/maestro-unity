using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt {
    
    public class SetSwipeSensitivity : MonoBehaviour {

        [ValidateInput("IsPopulated", "Value must be greater than 0", InfoMessageType.Error)]
        public float defaultYSensitivity;

        [ValidateInput("IsPopulated", "Value must be greater than 0", InfoMessageType.Error)]
        public float defaultXSensitivity;

        [ValidateInput("IsPopulated")]
        public FloatReference ySensitivity;

        [ValidateInput("IsPopulated")]
        public FloatReference xSensitivity;

        void Start ()
        {
            ySensitivity.Variable.SetValue(defaultYSensitivity);
            xSensitivity.Variable.SetValue(defaultXSensitivity);
        }

        private static bool IsPopulated(float attribute)
        {
            if(attribute > 0) {
                return true;
            } else {
                return false;
            }
        }

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }
    }
    
}