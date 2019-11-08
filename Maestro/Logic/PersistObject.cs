/***********************************************

Copyright © 2018 AltSalt Media, LLC.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Maestro {
    
    public class PersistObject : MonoBehaviour {

        [InfoBox("This script will call the Don't Destroy On Load method on this game object.")]
        public bool persist;

        void Awake() {
            if(persist) {
				DontDestroyOnLoad(this.gameObject);
            }
        }
    }

}