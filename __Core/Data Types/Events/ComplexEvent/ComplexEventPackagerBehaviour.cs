using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt
{
    [Serializable]
    [ExecuteInEditMode]
    public class ComplexEventPackagerBehaviour : MonoBehaviour
    {
        [SerializeField]
        ComplexEventPackager complexEventPackager;

        [Button(ButtonSizes.Large), GUIColor(0.8f, 0.6f, 1)]
        [InfoBox("Raises event")]
        public void Raise()
        {
            complexEventPackager.RaiseComplexEvent(); 
        }

    }
}