using AltSalt.Maestro.Sequencing.Navigate;
using UnityEngine;

namespace AltSalt.Maestro.Sequencing.Touch
{
    public class AxisMarker : JoinMarker, IMarkerDescription
    {
        [SerializeField]
        private string _description;

        public string description => _description;

        [SerializeField]
        private AxisType _axisType;

        public AxisType axisType => _axisType;

        [SerializeField]
        private bool _inverted;

        public bool inverted => _inverted;
    }
}