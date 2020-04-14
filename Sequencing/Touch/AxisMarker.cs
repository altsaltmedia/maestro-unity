using UnityEngine;
using UnityEngine.Timeline;

namespace AltSalt.Maestro.Sequencing.Touch
{
    public class AxisMarker : JoinMarker, IMarker, IMarkerDescription
    {
        [SerializeField]
        private AxisType _axisType;

        public AxisType axisType => _axisType;

        [SerializeField]
        private bool _inverted;

        public bool inverted => _inverted;
    }
}