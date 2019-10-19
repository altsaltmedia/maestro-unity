using UnityEngine;

namespace AltSalt.Sequencing.Touch
{
    public partial class AxisModifier
    {
        public abstract partial class BaseAxisSwitch
        {
            [SerializeField]
            private TouchController _touchController;

            public TouchController touchController
            {
                get => _touchController;
                set => _touchController = value;
            }

            [SerializeField]
            private TouchController.TouchData _touchData;

            public TouchController.TouchData touchData
            {
                get => _touchData;
                set => _touchData = value;
            }

            [SerializeField]
            private float _inflectionPoint;

            public float inflectionPoint
            {
                get => _inflectionPoint;
                set => _inflectionPoint = value;
            }
        }
    }
}