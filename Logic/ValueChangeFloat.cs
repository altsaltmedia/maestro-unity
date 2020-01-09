/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro.Logic {
    
    public class ValueChangeFloat : ValueChange {

        [ValidateInput("IsPopulated")]
        public FloatReference floatValue = new FloatReference();

        protected override void InitValue()
        {
            slider.value = floatValue.value;
            base.InitValue();
        }

        // Update is called once per frame
        public void SetValue (float newValue)
        {
            floatValue.variable.SetValue(newValue);
            updateVariables.RaiseEvent(this.gameObject);
		}

        private static bool IsPopulated(FloatReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }
	
}