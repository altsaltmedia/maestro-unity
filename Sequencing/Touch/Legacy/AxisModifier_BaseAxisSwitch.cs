using Sirenix.OdinInspector;
using UnityEngine;

namespace AltSalt.Sequencing.Touch
{
    public partial class AxisModifier
    {
        public partial class BaseAxisSwitchLegacy2
        {
            [SerializeField]
            private float _inflectionPoint;

            public float inflectionPoint
            {
                get => _inflectionPoint;
                set => _inflectionPoint = value;
            }
            
            [SerializeField]
            private Touch_Controller _touchController;

            public Touch_Controller touchController
            {
                get => _touchController;
                set => _touchController = value;
            }

            [SerializeField]
            private Touch_Data _touchData;

            public Touch_Data touchData
            {
                get => _touchData;
                set => _touchData = value;
            }
        }
    }
}