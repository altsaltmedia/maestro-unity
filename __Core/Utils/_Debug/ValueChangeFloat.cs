/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace AltSalt {
    
    public class ValueChangeFloat : ValueChange {

        [ValidateInput("IsPopulated")]
        public FloatReference floatValue = new FloatReference();

        protected override void InitValue()
        {
            slider.value = floatValue.Value;
            base.InitValue();
        }

        // Update is called once per frame
        public void SetValue (float newValue)
        {
            floatValue.Variable.SetValue(newValue);
            updateVariables.Raise();
		}

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }
	
}