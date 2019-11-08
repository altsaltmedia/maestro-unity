/***********************************************

Copyright © 2018 AltSalt Media, LLC.

All rights reserved.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro {
    
	public class ValueChange : MonoBehaviour {

        protected Slider slider;

        [Required]
        public SimpleEventTrigger updateVariables;

        protected virtual void Start()
        {
            slider = GetComponent<Slider>();
            InitValue();
        }

        protected virtual void InitValue()
        {
            updateVariables.RaiseEvent(this.gameObject);
        }

	}
	
}