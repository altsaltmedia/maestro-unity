/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using UnityEngine;
using UnityEngine.UI;

namespace AltSalt {
    
	public class ValueChange : MonoBehaviour {

        protected Slider slider;
        public SimpleEvent updateVariables;

        protected virtual void Start()
        {
            slider = GetComponent<Slider>();
            InitValue();
        }

        protected virtual void InitValue()
        {
            updateVariables.Raise();
        }

	}
	
}