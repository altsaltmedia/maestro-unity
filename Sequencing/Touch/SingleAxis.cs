using UnityEngine;

namespace AltSalt.Sequencing.Touch
{
    public class SingleAxis : AxisModifierMarker
    {
        [SerializeField]
        private AxisType _axisType;

        public AxisType axisType
        {
            get => _axisType;
        }

        [SerializeField]
        private bool _inverted;

        public bool inverted
        {
            get => _inverted;
        }
    }
}