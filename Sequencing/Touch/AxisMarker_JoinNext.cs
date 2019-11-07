using UnityEngine;

namespace AltSalt.Sequencing.Touch
{
    public class AxisMarker_JoinNext : AxisMarker, JoinMarker_IJoinSequence
    {
        [SerializeField]
        private Sequence _nextSequence;

        public JoinerDestination joinDestination => _nextSequence;

        public override MarkerPlacement markerPlacement => MarkerPlacement.EndOfSequence;
    }
}