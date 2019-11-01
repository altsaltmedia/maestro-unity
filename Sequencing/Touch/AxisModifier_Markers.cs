using UnityEngine;

namespace AltSalt.Sequencing.Touch
{
    public class AxisModifier_AxisMarker : JoinTools_Marker
    {
        [SerializeField]
        private AxisType _axisType;

        public AxisType axisType => _axisType;

        [SerializeField]
        private bool _inverted;

        public bool inverted => _inverted;
    }

    public class AxisModifier_AxisJoinPrevious : AxisModifier_AxisMarker, IJoinSequence
    {
        [SerializeField]
        private Sequence _previousSequence;

        public Sequence sequenceToJoin => _previousSequence;
        
        public override MarkerPlacement markerPlacement => MarkerPlacement.StartOfSequence;
    }
    
    public class AxisModifier_AxisJoinNext : AxisModifier_AxisMarker, IJoinSequence
    {
        [SerializeField]
        private Sequence _nextSequence;

        public Sequence sequenceToJoin => _nextSequence;

        public override MarkerPlacement markerPlacement => MarkerPlacement.EndOfSequence;
    }
}