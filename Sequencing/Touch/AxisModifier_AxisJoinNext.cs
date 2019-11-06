using UnityEngine;

namespace AltSalt.Sequencing.Touch
{
    public class AxisModifier_AxisJoinNext : AxisModifier_AxisMarker, IJoinSequence
    {
        [SerializeField]
        private Sequence _nextSequence;

        public ScriptableObject joinDestination => _nextSequence;

        public override MarkerPlacement markerPlacement => MarkerPlacement.EndOfSequence;
    }
}