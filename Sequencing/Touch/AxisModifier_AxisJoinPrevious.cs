using UnityEngine;

namespace AltSalt.Sequencing.Touch
{
    public class AxisModifier_AxisJoinPrevious : AxisModifier_AxisMarker, IJoinSequence
    {
        [SerializeField]
        private Sequence _previousSequence;

        public ScriptableObject joinDestination => _previousSequence;
        
        public override MarkerPlacement markerPlacement => MarkerPlacement.StartOfSequence;
    }
}