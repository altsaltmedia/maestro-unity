/***********************************************

Copyright © 2018 AltSalt Media, LLC.

https://www.altsalt.com / ricky@altsalt.com
        
**********************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Serialization;

namespace AltSalt.Maestro.Logic {
    
    public class PersistObject : MonoBehaviour {

        [FormerlySerializedAs("persist")]
        [InfoBox("This script will call the Don't Destroy On Load method on this game object.")]
        public bool _persist;

        private bool persist
        {
            get => _persist;
            set => _persist = value;
        }

        private void Awake() {
            if(persist) {
				DontDestroyOnLoad(this.gameObject);
            }
        }
    }

}