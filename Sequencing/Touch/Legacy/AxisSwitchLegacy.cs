using System;
using UnityEngine;
using Sirenix.OdinInspector;

namespace AltSalt.Sequencing.Touch
{
    [Serializable]
    public class AxisSwitchLegacy
    {
        [ValidateInput("IsPopulated")]
        [PropertyOrder(1)]
        [SerializeField]
        [InfoBox("This value should be set using an AxisSwitchTrigger using Timeline")]
        protected FloatReference _inflectionPoint = new FloatReference();

        public float inflectionPoint {

            get {
                return _inflectionPoint.Value;
            }

            set {
                _inflectionPoint.UseConstant = true;
                _inflectionPoint.ConstantValue = value;
            }

        }
    }
}