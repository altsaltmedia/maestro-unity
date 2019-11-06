using AltSalt.Sequencing.Navigate;
using UnityEngine;

namespace AltSalt.Sequencing.Touch
{
    public class AxisModifier_AxisMarker : JoinTools_Marker, IMarkerDescription
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