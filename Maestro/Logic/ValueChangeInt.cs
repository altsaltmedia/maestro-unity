﻿/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro {
    
    public class ValueChangeInt : ValueChange {

        [ValidateInput("IsPopulated")]
        public IntReference intValue = new IntReference();

        protected override void InitValue()
        {
            slider.value = intValue.Value;
            base.InitValue();
        }

        // Update is called once per frame
        public void SetValue(float newValue)
        {
            intValue.Variable.SetValue((int)newValue);
            updateVariables.RaiseEvent(this.gameObject);
        }

        private static bool IsPopulated(IntReference attribute)
        {
            return Utils.IsPopulated(attribute);
        }

    }
	
}